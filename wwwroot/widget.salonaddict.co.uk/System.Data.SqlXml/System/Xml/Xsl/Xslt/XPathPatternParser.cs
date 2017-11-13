namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.XmlConfiguration;
    using System.Xml.XPath;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.XPath;

    internal class XPathPatternParser
    {
        private const int MaxParseRelativePathDepth = 0x200;
        private int parseRelativePath;
        private XPathParser<QilNode> predicateParser = new XPathParser<QilNode>();
        private IPatternBuilder ptrnBuilder;
        private XPathScanner scanner;

        public QilNode Parse(XPathScanner scanner, IPatternBuilder ptrnBuilder)
        {
            QilNode result = null;
            ptrnBuilder.StartBuild();
            try
            {
                this.scanner = scanner;
                this.ptrnBuilder = ptrnBuilder;
                result = this.ParsePattern();
                this.scanner.CheckToken(LexKind.Eof);
            }
            finally
            {
                result = ptrnBuilder.EndBuild(result);
            }
            return result;
        }

        private QilNode ParseIdKeyPattern()
        {
            List<QilNode> args = new List<QilNode>(2);
            if (this.scanner.Name == "id")
            {
                this.scanner.NextLex();
                this.scanner.PassToken(LexKind.LParens);
                this.scanner.CheckToken(LexKind.String);
                args.Add(this.ptrnBuilder.String(this.scanner.StringValue));
                this.scanner.NextLex();
                this.scanner.PassToken(LexKind.RParens);
                return this.ptrnBuilder.Function("", "id", args);
            }
            this.scanner.NextLex();
            this.scanner.PassToken(LexKind.LParens);
            this.scanner.CheckToken(LexKind.String);
            args.Add(this.ptrnBuilder.String(this.scanner.StringValue));
            this.scanner.NextLex();
            this.scanner.PassToken(LexKind.Comma);
            this.scanner.CheckToken(LexKind.String);
            args.Add(this.ptrnBuilder.String(this.scanner.StringValue));
            this.scanner.NextLex();
            this.scanner.PassToken(LexKind.RParens);
            return this.ptrnBuilder.Function("", "key", args);
        }

        private QilNode ParseLocationPathPattern()
        {
            QilNode node;
            LexKind kind = this.scanner.Kind;
            if (kind != LexKind.Slash)
            {
                if (kind == LexKind.SlashSlash)
                {
                    this.scanner.NextLex();
                    return this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.Root, XPathNodeType.All, null, null), this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativePathPattern()));
                }
                if ((kind == LexKind.Name) && ((this.scanner.CanBeFunction && (this.scanner.Prefix.Length == 0)) && ((this.scanner.Name == "id") || (this.scanner.Name == "key"))))
                {
                    node = this.ParseIdKeyPattern();
                    LexKind kind2 = this.scanner.Kind;
                    if (kind2 != LexKind.Slash)
                    {
                        if (kind2 != LexKind.SlashSlash)
                        {
                            return node;
                        }
                    }
                    else
                    {
                        this.scanner.NextLex();
                        return this.ptrnBuilder.JoinStep(node, this.ParseRelativePathPattern());
                    }
                    this.scanner.NextLex();
                    return this.ptrnBuilder.JoinStep(node, this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativePathPattern()));
                }
            }
            else
            {
                this.scanner.NextLex();
                node = this.ptrnBuilder.Axis(XPathAxis.Root, XPathNodeType.All, null, null);
                if (XPathParser<QilNode>.IsStep(this.scanner.Kind))
                {
                    node = this.ptrnBuilder.JoinStep(node, this.ParseRelativePathPattern());
                }
                return node;
            }
            return this.ParseRelativePathPattern();
        }

        private QilNode ParsePattern()
        {
            QilNode left = this.ParseLocationPathPattern();
            while (this.scanner.Kind == LexKind.Union)
            {
                this.scanner.NextLex();
                left = this.ptrnBuilder.Operator(XPathOperator.Union, left, this.ParseLocationPathPattern());
            }
            return left;
        }

        private QilNode ParsePredicate(QilNode context)
        {
            this.scanner.NextLex();
            QilNode node = this.predicateParser.Parse(this.scanner, this.ptrnBuilder.GetPredicateBuilder(context), LexKind.RBracket);
            this.scanner.NextLex();
            return node;
        }

        private QilNode ParseRelativePathPattern()
        {
            if ((++this.parseRelativePath > 0x200) && XsltConfigSection.LimitXPathComplexity)
            {
                throw this.scanner.CreateException("Xslt_CompileError2", new string[0]);
            }
            QilNode left = this.ParseStepPattern();
            if (this.scanner.Kind == LexKind.Slash)
            {
                this.scanner.NextLex();
                left = this.ptrnBuilder.JoinStep(left, this.ParseRelativePathPattern());
            }
            else if (this.scanner.Kind == LexKind.SlashSlash)
            {
                this.scanner.NextLex();
                left = this.ptrnBuilder.JoinStep(left, this.ptrnBuilder.JoinStep(this.ptrnBuilder.Axis(XPathAxis.DescendantOrSelf, XPathNodeType.All, null, null), this.ParseRelativePathPattern()));
            }
            this.parseRelativePath--;
            return left;
        }

        private QilNode ParseStepPattern()
        {
            XPathAxis child;
            XPathNodeType type;
            string str;
            string str2;
            switch (this.scanner.Kind)
            {
                case LexKind.DotDot:
                case LexKind.Dot:
                    throw this.scanner.CreateException("XPath_InvalidAxisInPattern", new string[0]);

                case LexKind.Axis:
                    child = XPathParser<QilNode>.GetAxis(this.scanner.Name, this.scanner);
                    if ((child != XPathAxis.Child) && (child != XPathAxis.Attribute))
                    {
                        throw this.scanner.CreateException("XPath_InvalidAxisInPattern", new string[0]);
                    }
                    this.scanner.NextLex();
                    break;

                case LexKind.Name:
                case LexKind.Star:
                    child = XPathAxis.Child;
                    break;

                case LexKind.At:
                    child = XPathAxis.Attribute;
                    this.scanner.NextLex();
                    break;

                default:
                    throw this.scanner.CreateException("XPath_UnexpectedToken", new string[] { this.scanner.RawValue });
            }
            XPathParser<QilNode>.InternalParseNodeTest(this.scanner, child, out type, out str, out str2);
            QilNode node = this.ptrnBuilder.Axis(child, type, str, str2);
            while (this.scanner.Kind == LexKind.LBracket)
            {
                node = this.ptrnBuilder.Predicate(node, this.ParsePredicate(node), false);
            }
            return node;
        }

        public interface IPatternBuilder : IXPathBuilder<QilNode>
        {
            IXPathBuilder<QilNode> GetPredicateBuilder(QilNode context);
        }
    }
}

