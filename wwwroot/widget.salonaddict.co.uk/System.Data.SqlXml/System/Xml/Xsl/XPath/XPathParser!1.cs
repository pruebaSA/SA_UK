namespace System.Xml.Xsl.XPath
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Xml.XmlConfiguration;
    using System.Xml.XPath;

    internal class XPathParser<Node>
    {
        private IXPathBuilder<Node> builder;
        private const int MaxParseRelativePathDepth = 0x200;
        private const int MaxParseSubExprDepth = 0x400;
        private int parseRelativePath;
        private int parseSubExprDepth;
        private Stack<int> posInfo;
        private XPathScanner scanner;

        public XPathParser()
        {
            this.posInfo = new Stack<int>();
        }

        internal static XPathAxis GetAxis(string axisName, XPathScanner scanner)
        {
            switch (axisName)
            {
                case "ancestor":
                    return XPathAxis.Ancestor;

                case "ancestor-or-self":
                    return XPathAxis.AncestorOrSelf;

                case "attribute":
                    return XPathAxis.Attribute;

                case "child":
                    return XPathAxis.Child;

                case "descendant":
                    return XPathAxis.Descendant;

                case "descendant-or-self":
                    return XPathAxis.DescendantOrSelf;

                case "following":
                    return XPathAxis.Following;

                case "following-sibling":
                    return XPathAxis.FollowingSibling;

                case "namespace":
                    return XPathAxis.Namespace;

                case "parent":
                    return XPathAxis.Parent;

                case "preceding":
                    return XPathAxis.Preceding;

                case "preceding-sibling":
                    return XPathAxis.PrecedingSibling;

                case "self":
                    return XPathAxis.Self;
            }
            throw scanner.CreateException("XPath_UnknownAxis", new string[] { axisName });
        }

        internal static void InternalParseNodeTest(XPathScanner scanner, XPathAxis axis, out XPathNodeType nodeType, out string nodePrefix, out string nodeName)
        {
            LexKind kind = scanner.Kind;
            if (kind == LexKind.Star)
            {
                nodePrefix = null;
                nodeName = null;
                nodeType = XPathParser<Node>.PrincipalNodeType(axis);
                scanner.NextLex();
            }
            else
            {
                if (kind != LexKind.Name)
                {
                    throw scanner.CreateException("XPath_NodeTestExpected", new string[] { scanner.RawValue });
                }
                if (!scanner.CanBeFunction || !XPathParser<Node>.IsNodeType(scanner))
                {
                    nodePrefix = scanner.Prefix;
                    nodeName = scanner.Name;
                    nodeType = XPathParser<Node>.PrincipalNodeType(axis);
                    scanner.NextLex();
                    if (nodeName == "*")
                    {
                        nodeName = null;
                    }
                }
                else
                {
                    nodePrefix = null;
                    nodeName = null;
                    switch (scanner.Name)
                    {
                        case "comment":
                            nodeType = XPathNodeType.Comment;
                            break;

                        case "text":
                            nodeType = XPathNodeType.Text;
                            break;

                        case "node":
                            nodeType = XPathNodeType.All;
                            break;

                        default:
                            nodeType = XPathNodeType.ProcessingInstruction;
                            break;
                    }
                    scanner.NextLex();
                    scanner.PassToken(LexKind.LParens);
                    if ((nodeType == XPathNodeType.ProcessingInstruction) && (scanner.Kind != LexKind.RParens))
                    {
                        scanner.CheckToken(LexKind.String);
                        nodePrefix = string.Empty;
                        nodeName = scanner.StringValue;
                        scanner.NextLex();
                    }
                    scanner.PassToken(LexKind.RParens);
                }
            }
        }

        private static bool IsNodeType(XPathScanner scanner)
        {
            if (scanner.Prefix.Length != 0)
            {
                return false;
            }
            if (((scanner.Name != "node") && (scanner.Name != "text")) && (scanner.Name != "processing-instruction"))
            {
                return (scanner.Name == "comment");
            }
            return true;
        }

        private bool IsPrimaryExpr() => 
            ((((this.scanner.Kind == LexKind.String) || (this.scanner.Kind == LexKind.Number)) || ((this.scanner.Kind == LexKind.Dollar) || (this.scanner.Kind == LexKind.LParens))) || (((this.scanner.Kind == LexKind.Name) && this.scanner.CanBeFunction) && !XPathParser<Node>.IsNodeType(this.scanner)));

        private static bool IsReverseAxis(XPathAxis axis)
        {
            if (((axis != XPathAxis.Ancestor) && (axis != XPathAxis.Preceding)) && (axis != XPathAxis.AncestorOrSelf))
            {
                return (axis == XPathAxis.PrecedingSibling);
            }
            return true;
        }

        internal static bool IsStep(LexKind lexKind)
        {
            if ((((lexKind != LexKind.Dot) && (lexKind != LexKind.DotDot)) && ((lexKind != LexKind.At) && (lexKind != LexKind.Axis))) && (lexKind != LexKind.Star))
            {
                return (lexKind == LexKind.Name);
            }
            return true;
        }

        public Node Parse(XPathScanner scanner, IXPathBuilder<Node> builder, LexKind endLex)
        {
            Node result = default(Node);
            this.scanner = scanner;
            this.builder = builder;
            this.posInfo.Clear();
            try
            {
                builder.StartBuild();
                result = this.ParseExpr();
                scanner.CheckToken(endLex);
            }
            catch (XPathCompileException exception)
            {
                if (exception.queryString == null)
                {
                    exception.queryString = scanner.Source;
                    this.PopPosInfo(out exception.startChar, out exception.endChar);
                }
                throw;
            }
            finally
            {
                result = builder.EndBuild(result);
            }
            return result;
        }

        private Node ParseAdditiveExpr()
        {
            bool flag;
            Node left = this.ParseMultiplicativeExpr();
            while ((flag = this.scanner.Kind == LexKind.Plus) || (this.scanner.Kind == LexKind.Minus))
            {
                XPathOperator op = flag ? XPathOperator.Plus : XPathOperator.Minus;
                this.scanner.NextLex();
                left = this.builder.Operator(op, left, this.ParseMultiplicativeExpr());
            }
            return left;
        }

        private Node ParseAndExpr()
        {
            Node left = this.ParseEqualityExpr();
            while (this.scanner.IsKeyword("and"))
            {
                this.scanner.NextLex();
                left = this.builder.Operator(XPathOperator.And, left, this.ParseEqualityExpr());
            }
            return left;
        }

        private Node ParseEqualityExpr()
        {
            bool flag;
            Node left = this.ParseRelationalExpr();
            while ((flag = this.scanner.Kind == LexKind.Eq) || (this.scanner.Kind == LexKind.Ne))
            {
                XPathOperator op = flag ? XPathOperator.Eq : XPathOperator.Ne;
                this.scanner.NextLex();
                left = this.builder.Operator(op, left, this.ParseRelationalExpr());
            }
            return left;
        }

        private Node ParseExpr()
        {
            if ((++this.parseSubExprDepth > 0x400) && XsltConfigSection.LimitXPathComplexity)
            {
                throw this.scanner.CreateException("Xslt_CompileError2", new string[0]);
            }
            Node left = this.ParseAndExpr();
            while (this.scanner.IsKeyword("or"))
            {
                this.scanner.NextLex();
                left = this.builder.Operator(XPathOperator.Or, left, this.ParseAndExpr());
            }
            this.parseSubExprDepth--;
            return left;
        }

        private Node ParseFilterExpr()
        {
            int lexStart = this.scanner.LexStart;
            Node node = this.ParsePrimaryExpr();
            int prevLexEnd = this.scanner.PrevLexEnd;
            while (this.scanner.Kind == LexKind.LBracket)
            {
                this.PushPosInfo(lexStart, prevLexEnd);
                node = this.builder.Predicate(node, this.ParsePredicate(), false);
                this.PopPosInfo();
            }
            return node;
        }

        private Node ParseFunctionCall()
        {
            List<Node> args = new List<Node>();
            string name = this.scanner.Name;
            string prefix = this.scanner.Prefix;
            int lexStart = this.scanner.LexStart;
            this.scanner.PassToken(LexKind.Name);
            this.scanner.PassToken(LexKind.LParens);
            if (this.scanner.Kind != LexKind.RParens)
            {
                while (true)
                {
                    args.Add(this.ParseExpr());
                    if (this.scanner.Kind != LexKind.Comma)
                    {
                        this.scanner.CheckToken(LexKind.RParens);
                        break;
                    }
                    this.scanner.NextLex();
                }
            }
            this.scanner.NextLex();
            this.PushPosInfo(lexStart, this.scanner.PrevLexEnd);
            Node local = this.builder.Function(prefix, name, args);
            this.PopPosInfo();
            return local;
        }

        private Node ParseLocationPath()
        {
            if (this.scanner.Kind == LexKind.Slash)
            {
                this.scanner.NextLex();
                Node left = this.builder.Axis(XPathAxis.Root, XPathNodeType.All, null, null);
                if (XPathParser<Node>.IsStep(this.scanner.Kind))
                {
                    left = this.builder.JoinStep(left, this.ParseRelativeLocationPath());
                }
                return left;
            }
            if (this.scanner.Kind == LexKind.SlashSlash)
            {
                this.scanner.NextLex();
                return this.builder.JoinStep(this.builder.Axis(XPathAxis.Root, XPathNodeType.All, null, null), this.builder.JoinStep(this.builder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativeLocationPath()));
            }
            return this.ParseRelativeLocationPath();
        }

        private Node ParseMultiplicativeExpr()
        {
            Node left = this.ParseUnaryExpr();
            while (true)
            {
                XPathOperator multiply;
                if (this.scanner.Kind == LexKind.Star)
                {
                    multiply = XPathOperator.Multiply;
                }
                else if (this.scanner.IsKeyword("div"))
                {
                    multiply = XPathOperator.Divide;
                }
                else if (this.scanner.IsKeyword("mod"))
                {
                    multiply = XPathOperator.Modulo;
                }
                else
                {
                    return left;
                }
                this.scanner.NextLex();
                left = this.builder.Operator(multiply, left, this.ParseUnaryExpr());
            }
        }

        private Node ParseNodeTest(XPathAxis axis)
        {
            XPathNodeType type;
            string str;
            string str2;
            int lexStart = this.scanner.LexStart;
            XPathParser<Node>.InternalParseNodeTest(this.scanner, axis, out type, out str, out str2);
            this.PushPosInfo(lexStart, this.scanner.PrevLexEnd);
            Node local = this.builder.Axis(axis, type, str, str2);
            this.PopPosInfo();
            return local;
        }

        private Node ParsePathExpr()
        {
            if (!this.IsPrimaryExpr())
            {
                return this.ParseLocationPath();
            }
            int lexStart = this.scanner.LexStart;
            Node left = this.ParseFilterExpr();
            int prevLexEnd = this.scanner.PrevLexEnd;
            if (this.scanner.Kind == LexKind.Slash)
            {
                this.scanner.NextLex();
                this.PushPosInfo(lexStart, prevLexEnd);
                left = this.builder.JoinStep(left, this.ParseRelativeLocationPath());
                this.PopPosInfo();
                return left;
            }
            if (this.scanner.Kind == LexKind.SlashSlash)
            {
                this.scanner.NextLex();
                this.PushPosInfo(lexStart, prevLexEnd);
                left = this.builder.JoinStep(left, this.builder.JoinStep(this.builder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativeLocationPath()));
                this.PopPosInfo();
            }
            return left;
        }

        private Node ParsePredicate()
        {
            this.scanner.PassToken(LexKind.LBracket);
            Node local = this.ParseExpr();
            this.scanner.PassToken(LexKind.RBracket);
            return local;
        }

        private Node ParsePrimaryExpr()
        {
            Node local;
            switch (this.scanner.Kind)
            {
                case LexKind.Dollar:
                {
                    int lexStart = this.scanner.LexStart;
                    this.scanner.NextLex();
                    this.scanner.CheckToken(LexKind.Name);
                    this.PushPosInfo(lexStart, this.scanner.LexStart + this.scanner.LexSize);
                    local = this.builder.Variable(this.scanner.Prefix, this.scanner.Name);
                    this.PopPosInfo();
                    this.scanner.NextLex();
                    return local;
                }
                case LexKind.LParens:
                    this.scanner.NextLex();
                    local = this.ParseExpr();
                    this.scanner.PassToken(LexKind.RParens);
                    return local;

                case LexKind.Number:
                    local = this.builder.Number(this.scanner.NumberValue);
                    this.scanner.NextLex();
                    return local;

                case LexKind.String:
                    local = this.builder.String(this.scanner.StringValue);
                    this.scanner.NextLex();
                    return local;
            }
            return this.ParseFunctionCall();
        }

        private Node ParseRelationalExpr()
        {
            Node left = this.ParseAdditiveExpr();
            while (true)
            {
                XPathOperator lt;
                switch (this.scanner.Kind)
                {
                    case LexKind.Lt:
                        lt = XPathOperator.Lt;
                        break;

                    case LexKind.Eq:
                        return left;

                    case LexKind.Gt:
                        lt = XPathOperator.Gt;
                        break;

                    case LexKind.Ge:
                        lt = XPathOperator.Ge;
                        break;

                    case LexKind.Le:
                        lt = XPathOperator.Le;
                        break;

                    default:
                        return left;
                }
                this.scanner.NextLex();
                left = this.builder.Operator(lt, left, this.ParseAdditiveExpr());
            }
        }

        private Node ParseRelativeLocationPath()
        {
            if ((++this.parseRelativePath > 0x200) && XsltConfigSection.LimitXPathComplexity)
            {
                throw this.scanner.CreateException("Xslt_CompileError2", new string[0]);
            }
            Node left = this.ParseStep();
            if (this.scanner.Kind == LexKind.Slash)
            {
                this.scanner.NextLex();
                left = this.builder.JoinStep(left, this.ParseRelativeLocationPath());
            }
            else if (this.scanner.Kind == LexKind.SlashSlash)
            {
                this.scanner.NextLex();
                left = this.builder.JoinStep(left, this.builder.JoinStep(this.builder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativeLocationPath()));
            }
            this.parseRelativePath--;
            return left;
        }

        private Node ParseStep()
        {
            Node local;
            XPathAxis attribute;
            if (LexKind.Dot == this.scanner.Kind)
            {
                this.scanner.NextLex();
                local = this.builder.Axis(XPathAxis.Self, XPathNodeType.All, null, null);
                if (LexKind.LBracket == this.scanner.Kind)
                {
                    throw this.scanner.CreateException("XPath_PredicateAfterDot", new string[0]);
                }
                return local;
            }
            if (LexKind.DotDot == this.scanner.Kind)
            {
                this.scanner.NextLex();
                local = this.builder.Axis(XPathAxis.Parent, XPathNodeType.All, null, null);
                if (LexKind.LBracket == this.scanner.Kind)
                {
                    throw this.scanner.CreateException("XPath_PredicateAfterDotDot", new string[0]);
                }
                return local;
            }
            LexKind kind = this.scanner.Kind;
            if (kind <= LexKind.At)
            {
                switch (kind)
                {
                    case LexKind.Star:
                        goto Label_010A;

                    case LexKind.At:
                        attribute = XPathAxis.Attribute;
                        this.scanner.NextLex();
                        goto Label_0135;
                }
                goto Label_010E;
            }
            if (kind != LexKind.Axis)
            {
                if (kind == LexKind.Name)
                {
                    goto Label_010A;
                }
                goto Label_010E;
            }
            attribute = XPathParser<Node>.GetAxis(this.scanner.Name, this.scanner);
            this.scanner.NextLex();
            goto Label_0135;
        Label_010A:
            attribute = XPathAxis.Child;
            goto Label_0135;
        Label_010E:;
            throw this.scanner.CreateException("XPath_UnexpectedToken", new string[] { this.scanner.RawValue });
        Label_0135:
            local = this.ParseNodeTest(attribute);
            while (LexKind.LBracket == this.scanner.Kind)
            {
                local = this.builder.Predicate(local, this.ParsePredicate(), XPathParser<Node>.IsReverseAxis(attribute));
            }
            return local;
        }

        private Node ParseUnaryExpr()
        {
            if (this.scanner.Kind == LexKind.Minus)
            {
                this.scanner.NextLex();
                return this.builder.Operator(XPathOperator.UnaryMinus, this.ParseUnaryExpr(), default(Node));
            }
            return this.ParseUnionExpr();
        }

        private Node ParseUnionExpr()
        {
            int lexStart = this.scanner.LexStart;
            Node right = this.ParsePathExpr();
            if (this.scanner.Kind == LexKind.Union)
            {
                this.PushPosInfo(lexStart, this.scanner.PrevLexEnd);
                right = this.builder.Operator(XPathOperator.Union, default(Node), right);
                this.PopPosInfo();
                while (this.scanner.Kind == LexKind.Union)
                {
                    this.scanner.NextLex();
                    lexStart = this.scanner.LexStart;
                    Node local2 = this.ParsePathExpr();
                    this.PushPosInfo(lexStart, this.scanner.PrevLexEnd);
                    right = this.builder.Operator(XPathOperator.Union, right, local2);
                    this.PopPosInfo();
                }
            }
            return right;
        }

        private void PopPosInfo()
        {
            this.posInfo.Pop();
            this.posInfo.Pop();
        }

        private void PopPosInfo(out int startChar, out int endChar)
        {
            endChar = this.posInfo.Pop();
            startChar = this.posInfo.Pop();
        }

        private static XPathNodeType PrincipalNodeType(XPathAxis axis)
        {
            if (axis == XPathAxis.Attribute)
            {
                return XPathNodeType.Attribute;
            }
            if (axis != XPathAxis.Namespace)
            {
                return XPathNodeType.Element;
            }
            return XPathNodeType.Namespace;
        }

        private void PushPosInfo(int startChar, int endChar)
        {
            this.posInfo.Push(startChar);
            this.posInfo.Push(endChar);
        }
    }
}

