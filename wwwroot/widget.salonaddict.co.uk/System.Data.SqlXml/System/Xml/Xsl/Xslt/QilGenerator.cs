namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;
    using System.Xml.Xsl.XPath;

    internal class QilGenerator : IErrorHelper, IXPathEnvironment, IFocus
    {
        private bool allowCurrent = true;
        private bool allowKey = true;
        private bool allowVariables = true;
        private static readonly XmlTypeCode[] argFnDocument = new XmlTypeCode[] { XmlTypeCode.Item, XmlTypeCode.Node };
        private static readonly XmlTypeCode[] argFnFormatNumber = new XmlTypeCode[] { XmlTypeCode.Double, XmlTypeCode.String, XmlTypeCode.String };
        private static readonly XmlTypeCode[] argFnKey = new XmlTypeCode[] { XmlTypeCode.String, XmlTypeCode.Item };
        private System.Xml.Xsl.Xslt.Compiler compiler;
        private LoopFocus curLoop;
        private static readonly char[] curlyBraces = new char[] { '{', '}' };
        private XmlQueryType elementOrDocumentType;
        private QilList extPars;
        private XsltQilFactory f;
        private bool formatNumberDynamicUsed;
        private int formatterCnt;
        private FunctionFocus funcFocus;
        private QilList functions;
        public static Dictionary<string, XPathBuilder.FunctionInfo<FuncId>> FunctionTable = CreateFunctionTable();
        private QilFunction generalKey;
        private QilList gloVars;
        private const XmlNodeKindFlags InvalidatingNodes = (XmlNodeKindFlags.Namespace | XmlNodeKindFlags.Attribute);
        private InvokeGenerator invkGen;
        private KeyMatchBuilder keyMatchBuilder;
        private XslNode lastScope;
        private MatcherBuilder matcherBuilder;
        private QilName nameCurrent;
        private QilName nameLast;
        private QilName nameNamespaces;
        private QilName namePosition;
        private QilList nsVars;
        private OutputScopeManager outputScope = new OutputScopeManager();
        private HybridDictionary prefixesInUse = new HybridDictionary();
        private XPathPatternBuilder ptrnBuilder;
        private XPathPatternParser ptrnParser;
        private ReferenceReplacer refReplacer;
        private CompilerScopeManager<QilIterator> scope = new CompilerScopeManager<QilIterator>();
        private SingletonFocus singlFocus;
        private QilStrConcatenator strConcat;
        private XmlQueryType textOrAttributeType;
        private readonly StringBuilder unescapedText = new StringBuilder();
        private VariableHelper varHelper;
        private XPathBuilder xpathBuilder;
        private XPathParser<QilNode> xpathParser;
        private XslVersion xslVersion;

        private QilGenerator(bool debug)
        {
            this.f = new XsltQilFactory(new QilFactory(), debug);
            this.xpathBuilder = new XPathBuilder(this);
            this.xpathParser = new XPathParser<QilNode>();
            this.ptrnBuilder = new XPathPatternBuilder(this);
            this.ptrnParser = new XPathPatternParser();
            this.refReplacer = new ReferenceReplacer(this.f.BaseFactory);
            this.invkGen = new InvokeGenerator(this.f, debug);
            this.matcherBuilder = new MatcherBuilder(this.f, this.refReplacer, this.invkGen);
            this.singlFocus = new SingletonFocus(this.f);
            this.funcFocus = new FunctionFocus();
            this.curLoop = new LoopFocus(this.f);
            this.strConcat = new QilStrConcatenator(this.f);
            this.varHelper = new VariableHelper(this.f);
            this.elementOrDocumentType = XmlQueryTypeFactory.DocumentOrElement;
            this.textOrAttributeType = XmlQueryTypeFactory.NodeChoice(XmlNodeKindFlags.Text | XmlNodeKindFlags.Attribute);
            this.nameCurrent = this.f.QName("current", "urn:schemas-microsoft-com:xslt-debug");
            this.namePosition = this.f.QName("position", "urn:schemas-microsoft-com:xslt-debug");
            this.nameLast = this.f.QName("last", "urn:schemas-microsoft-com:xslt-debug");
            this.nameNamespaces = this.f.QName("namespaces", "urn:schemas-microsoft-com:xslt-debug");
            this.formatterCnt = 0;
        }

        private QilNode AddCurrentPositionLast(QilNode content)
        {
            if (this.IsDebug)
            {
                content = this.AddDebugVariable(this.CloneName(this.nameLast), this.GetLastPosition(), content);
                content = this.AddDebugVariable(this.CloneName(this.namePosition), this.GetCurrentPosition(), content);
                content = this.AddDebugVariable(this.CloneName(this.nameCurrent), this.GetCurrentNode(), content);
            }
            return content;
        }

        private QilNode AddDebugVariable(QilName name, QilNode value, QilNode content)
        {
            QilIterator variable = this.f.Let(value);
            variable.DebugName = name.ToString();
            return this.f.Loop(variable, content);
        }

        private void AddImplicitArgs(XslNode node)
        {
            XslFlags none = XslFlags.None;
            if (this.IsDebug)
            {
                none = XslFlags.FocusFilter;
            }
            else if (node.NodeType == XslNodeType.CallTemplate)
            {
                Template template;
                if (this.compiler.NamedTemplates.TryGetValue(node.Name, out template))
                {
                    none = template.Flags;
                }
            }
            else if (node.NodeType == XslNodeType.UseAttributeSet)
            {
                AttributeSet set;
                if (this.compiler.AttributeSets.TryGetValue(node.Name, out set))
                {
                    none = set.Flags;
                }
            }
            else
            {
                if (!this.compiler.ModeFlags.TryGetValue(node.Name, out none))
                {
                    none = XslFlags.None;
                }
                none |= XslFlags.Current;
            }
            List<XslNode> collection = new List<XslNode>();
            if ((none & XslFlags.Current) != XslFlags.None)
            {
                collection.Add(CreateWithParam(this.nameCurrent, this.GetCurrentNode()));
            }
            if ((none & XslFlags.Position) != XslFlags.None)
            {
                collection.Add(CreateWithParam(this.namePosition, this.GetCurrentPosition()));
            }
            if ((none & XslFlags.Last) != XslFlags.None)
            {
                collection.Add(CreateWithParam(this.nameLast, this.GetLastPosition()));
            }
            node.InsertContent(collection);
        }

        private void AddNsDecl(QilList content, string prefix, string nsUri)
        {
            if (this.outputScope.LookupNamespace(prefix) != nsUri)
            {
                this.outputScope.AddNamespace(prefix, nsUri);
                content.Add(this.f.NamespaceDecl(this.f.String(prefix), this.f.String(nsUri)));
            }
        }

        private QilList BuildDebuggerNamespaces()
        {
            if (!this.IsDebug)
            {
                return null;
            }
            QilList list = this.f.BaseFactory.Sequence();
            foreach (CompilerScopeManager<QilIterator>.ScopeRecord record in this.scope)
            {
                list.Add(this.f.NamespaceDecl(this.f.String(record.ncName), this.f.String(record.nsUri)));
            }
            return list;
        }

        [Conditional("DEBUG")]
        private void CheckSingletonFocus()
        {
        }

        private XmlQueryType ChooseBestType(VarPar var)
        {
            if (!this.IsDebug && this.InferXPathTypes)
            {
                switch ((var.Flags & XslFlags.AnyType))
                {
                    case XslFlags.Nodeset:
                        return XmlQueryTypeFactory.NodeNotRtfS;

                    case (XslFlags.Nodeset | XslFlags.Node):
                        return XmlQueryTypeFactory.NodeNotRtfS;

                    case XslFlags.String:
                        return XmlQueryTypeFactory.StringX;

                    case XslFlags.Number:
                        return XmlQueryTypeFactory.DoubleX;

                    case XslFlags.Boolean:
                        return XmlQueryTypeFactory.BooleanX;

                    case XslFlags.Node:
                        return XmlQueryTypeFactory.NodeNotRtf;

                    case XslFlags.Rtf:
                        return XmlQueryTypeFactory.Node;

                    case (XslFlags.Rtf | XslFlags.Node):
                        return XmlQueryTypeFactory.Node;

                    case (XslFlags.Rtf | XslFlags.Nodeset):
                        return XmlQueryTypeFactory.NodeS;

                    case (XslFlags.Rtf | XslFlags.Nodeset | XslFlags.Node):
                        return XmlQueryTypeFactory.NodeS;
                }
            }
            return XmlQueryTypeFactory.ItemS;
        }

        private QilName CloneName(QilName name) => 
            ((QilName) name.ShallowClone(this.f.BaseFactory));

        private QilExpression Compile(System.Xml.Xsl.Xslt.Compiler compiler)
        {
            this.compiler = compiler;
            this.functions = this.f.FunctionList();
            this.extPars = this.f.GlobalParameterList();
            this.gloVars = this.f.GlobalVariableList();
            this.nsVars = this.f.GlobalVariableList();
            compiler.Scripts.CompileScripts();
            if (!this.IsDebug)
            {
                new XslAstAnalyzer().Analyze(compiler);
            }
            this.CreateGlobalVarPars();
            try
            {
                this.CompileKeys();
                this.CompileAndSortMatches(compiler.PrincipalStylesheet);
                this.PrecompileProtoTemplatesHeaders();
                this.CompileGlobalVariables();
                foreach (ProtoTemplate template in compiler.AllTemplates)
                {
                    this.CompileProtoTemplate(template);
                }
            }
            catch (XslLoadException exception)
            {
                exception.SetSourceLineInfo(this.lastScope.SourceLine);
                throw;
            }
            catch (Exception exception2)
            {
                if (!XmlException.IsCatchableException(exception2))
                {
                    throw;
                }
                throw new XslLoadException(exception2, this.lastScope.SourceLine);
            }
            QilNode root = this.CompileRootExpression(compiler.StartApplyTemplates);
            foreach (ProtoTemplate template2 in compiler.AllTemplates)
            {
                foreach (QilParameter parameter in template2.Function.Arguments)
                {
                    if (!this.IsDebug || parameter.Name.Equals(this.nameNamespaces))
                    {
                        parameter.DefaultValue = null;
                    }
                }
            }
            Dictionary<string, Type> scriptClasses = compiler.Scripts.ScriptClasses;
            List<EarlyBoundInfo> list = new List<EarlyBoundInfo>(scriptClasses.Count);
            foreach (KeyValuePair<string, Type> pair in scriptClasses)
            {
                if (pair.Value != null)
                {
                    list.Add(new EarlyBoundInfo(pair.Key, pair.Value));
                }
            }
            QilExpression input = this.f.QilExpression(root, this.f.BaseFactory);
            input.EarlyBoundTypes = list;
            input.FunctionList = this.functions;
            input.GlobalParameterList = this.extPars;
            input.GlobalVariableList = this.gloVars;
            input.WhitespaceRules = compiler.WhitespaceRules;
            input.IsDebug = this.IsDebug;
            input.DefaultWriterSettings = compiler.Output.Settings;
            QilDepthChecker.Check(input);
            return input;
        }

        private void CompileAndSortMatches(Stylesheet sheet)
        {
            foreach (Template template in sheet.Templates)
            {
                if (template.Match != null)
                {
                    this.EnterScope(template);
                    QilNode node = this.CompileMatchPattern(template.Match);
                    if (node.NodeType == QilNodeType.Sequence)
                    {
                        QilList list = (QilList) node;
                        for (int i = 0; i < list.Count; i++)
                        {
                            sheet.AddTemplateMatch(template, (QilLoop) list[i]);
                        }
                    }
                    else
                    {
                        sheet.AddTemplateMatch(template, (QilLoop) node);
                    }
                    this.ExitScope();
                }
            }
            sheet.SortTemplateMatches();
            foreach (Stylesheet stylesheet in sheet.Imports)
            {
                this.CompileAndSortMatches(stylesheet);
            }
        }

        private QilNode CompileApplyImports(XslNode node) => 
            this.GenerateApply((Stylesheet) node.Arg, node);

        private QilNode CompileApplyTemplates(XslNodeEx node)
        {
            IList<XslNode> content = node.Content;
            int varScope = this.varHelper.StartVariables();
            QilIterator let = this.f.Let(this.CompileNodeSetExpression(node.Select));
            this.varHelper.AddVariable(let);
            for (int i = 0; i < content.Count; i++)
            {
                VarPar withParam = content[i] as VarPar;
                if (withParam != null)
                {
                    this.CompileWithParam(withParam);
                    QilNode binding = withParam.Value;
                    if (this.IsDebug || (!(binding is QilIterator) && !(binding is QilLiteral)))
                    {
                        QilIterator iterator2 = this.f.Let(binding);
                        iterator2.DebugName = this.f.QName("with-param " + withParam.Name.QualifiedName, "urn:schemas-microsoft-com:xslt-debug").ToString();
                        this.varHelper.AddVariable(iterator2);
                        withParam.Value = iterator2;
                    }
                }
            }
            LoopFocus curLoop = this.curLoop;
            QilIterator current = this.f.For(let);
            this.curLoop.SetFocus(current);
            this.curLoop.Sort(this.CompileSorts(content, ref curLoop));
            QilNode expr = this.GenerateApply(null, node);
            expr = this.WrapLoopBody(node.ElemNameLi, expr, node.EndTagLi);
            expr = this.AddCurrentPositionLast(expr);
            expr = this.curLoop.ConstructLoop(expr);
            this.curLoop = curLoop;
            return this.varHelper.FinishVariables(expr, varScope);
        }

        private QilNode CompileAttribute(NodeCtor node)
        {
            QilNode node4;
            QilNode ns = this.CompileStringAvt(node.NsAvt);
            QilNode node3 = this.CompileStringAvt(node.NameAvt);
            bool flag = false;
            if ((node3.NodeType == QilNodeType.LiteralString) && ((ns == null) || (ns.NodeType == QilNodeType.LiteralString)))
            {
                string str2;
                string str3;
                string str4;
                string qname = (string) ((QilLiteral) node3);
                bool flag2 = this.compiler.ParseQName(qname, out str2, out str3, this);
                if (ns == null)
                {
                    str4 = flag2 ? this.ResolvePrefix(true, str2) : this.compiler.CreatePhantomNamespace();
                }
                else
                {
                    str4 = (string) ((QilLiteral) ns);
                    flag = true;
                }
                if ((qname == "xmlns") || ((str3 == "xmlns") && (str4.Length == 0)))
                {
                    this.ReportError("Xslt_XmlnsAttr", new string[] { "name", qname });
                }
                node4 = this.f.QName(str3, str4, str2);
            }
            else if (ns != null)
            {
                node4 = this.f.StrParseQName(node3, ns);
            }
            else
            {
                node4 = this.ResolveQNameDynamic(true, node3);
            }
            if (flag)
            {
                this.outputScope.InvalidateNonDefaultPrefixes();
            }
            return this.f.AttributeCtor(node4, this.CompileInstructions(node.Content));
        }

        private QilNode CompileAvt(string source)
        {
            QilList list = this.f.BaseFactory.Sequence();
            int pos = 0;
            while (pos < source.Length)
            {
                QilNode node = this.ExtractText(source, ref pos);
                if (node != null)
                {
                    list.Add(node);
                }
                if (pos < source.Length)
                {
                    pos++;
                    QilNode n = this.CompileXPathExpressionWithinAvt(source, ref pos);
                    list.Add(this.f.ConvertToString(n));
                }
            }
            if (list.Count == 1)
            {
                return list[0];
            }
            return list;
        }

        private QilNode CompileCallTemplate(XslNodeEx node)
        {
            QilNode node3;
            Template template;
            int varScope = this.varHelper.StartVariables();
            IList<XslNode> content = node.Content;
            foreach (VarPar par in content)
            {
                this.CompileWithParam(par);
                if (this.IsDebug)
                {
                    QilNode binding = par.Value;
                    QilIterator let = this.f.Let(binding);
                    let.DebugName = this.f.QName("with-param " + par.Name.QualifiedName, "urn:schemas-microsoft-com:xslt-debug").ToString();
                    this.varHelper.AddVariable(let);
                    par.Value = let;
                }
            }
            if (this.compiler.NamedTemplates.TryGetValue(node.Name, out template))
            {
                node3 = this.GenerateCall(template.Function, node);
            }
            else
            {
                if (!this.compiler.IsPhantomName(node.Name))
                {
                    this.compiler.ReportError(node.SourceLine, "Xslt_InvalidCallTemplate", new string[] { node.Name.QualifiedName });
                }
                node3 = this.f.Sequence();
            }
            if (content.Count > 0)
            {
                node3 = SetLineInfo(node3, node.ElemNameLi);
            }
            node3 = this.varHelper.FinishVariables(node3, varScope);
            if (this.IsDebug)
            {
                return this.f.Nop(node3);
            }
            return node3;
        }

        private QilNode CompileChoose(XslNode node)
        {
            IList<XslNode> content = node.Content;
            QilNode n = null;
            for (int i = content.Count - 1; 0 <= i; i--)
            {
                XslNode node3 = content[i];
                QilList nsList = this.EnterScope(node3);
                if (node3.NodeType == XslNodeType.Otherwise)
                {
                    n = this.CompileInstructions(node3.Content);
                }
                else
                {
                    n = this.CompileWhen(node3, n ?? this.InstructionList());
                }
                this.ExitScope();
                this.SetLineInfoCheck(n, node3.SourceLine);
                n = this.SetDebugNs(n, nsList);
            }
            if (n == null)
            {
                return this.f.Sequence();
            }
            if (!this.IsDebug)
            {
                return n;
            }
            return this.f.Sequence(n);
        }

        private QilNode CompileComment(XslNode node) => 
            this.f.CommentCtor(this.CompileInstructions(node.Content));

        private QilNode CompileCopy(XslNode copy)
        {
            QilNode currentNode = this.GetCurrentNode();
            if ((currentNode.XmlType.NodeKinds & (XmlNodeKindFlags.Namespace | XmlNodeKindFlags.Attribute)) != XmlNodeKindFlags.None)
            {
                this.outputScope.InvalidateAllPrefixes();
            }
            if (currentNode.XmlType.NodeKinds == XmlNodeKindFlags.Element)
            {
                QilList content = this.InstructionList();
                content.Add(this.f.XPathNamespace(currentNode));
                this.outputScope.PushScope();
                this.outputScope.InvalidateAllPrefixes();
                QilNode node2 = this.CompileInstructions(copy.Content, content);
                this.outputScope.PopScope();
                return this.f.ElementCtor(this.f.NameOf(currentNode), node2);
            }
            if (currentNode.XmlType.NodeKinds == XmlNodeKindFlags.Document)
            {
                return this.CompileInstructions(copy.Content);
            }
            if ((currentNode.XmlType.NodeKinds & (XmlNodeKindFlags.Element | XmlNodeKindFlags.Document)) == XmlNodeKindFlags.None)
            {
                return currentNode;
            }
            return this.f.XsltCopy(currentNode, this.CompileInstructions(copy.Content));
        }

        private QilNode CompileCopyOf(XslNode node)
        {
            QilIterator iterator2;
            QilNode expr = this.CompileXPathExpression(node.Select);
            if (expr.XmlType.IsNode)
            {
                QilIterator iterator;
                if ((expr.XmlType.NodeKinds & (XmlNodeKindFlags.Namespace | XmlNodeKindFlags.Attribute)) != XmlNodeKindFlags.None)
                {
                    this.outputScope.InvalidateAllPrefixes();
                }
                if (expr.XmlType.IsNotRtf && ((expr.XmlType.NodeKinds & XmlNodeKindFlags.Document) == XmlNodeKindFlags.None))
                {
                    return expr;
                }
                if (expr.XmlType.IsSingleton)
                {
                    return this.f.XsltCopyOf(expr);
                }
                return this.f.Loop(iterator = this.f.For(expr), this.f.XsltCopyOf(iterator));
            }
            if (expr.XmlType.IsAtomicValue)
            {
                return this.f.TextCtor(this.f.ConvertToString(expr));
            }
            this.outputScope.InvalidateAllPrefixes();
            return this.f.Loop(iterator2 = this.f.For(expr), this.f.Conditional(this.f.IsType(iterator2, XmlQueryTypeFactory.Node), this.f.XsltCopyOf(this.f.TypeAssert(iterator2, XmlQueryTypeFactory.Node)), this.f.TextCtor(this.f.XsltConvert(iterator2, XmlQueryTypeFactory.StringX))));
        }

        private void CompileDataTypeAttribute(string attValue, bool fwdCompat, ref QilNode select, out QilNode select2)
        {
            QilNode binding = this.CompileStringAvt(attValue);
            if (binding != null)
            {
                if (binding.NodeType != QilNodeType.LiteralString)
                {
                    QilIterator iterator;
                    binding = this.f.Loop(iterator = this.f.Let(binding), this.f.Conditional(this.f.Eq(iterator, this.f.String("number")), this.f.False(), this.f.Conditional(this.f.Eq(iterator, this.f.String("text")), this.f.True(), fwdCompat ? this.f.True() : this.f.Loop(this.f.Let(this.ResolveQNameDynamic(true, iterator)), this.f.Error(this.lastScope.SourceLine, "Xslt_BistateAttribute", new string[] { "data-type", "text", "number" })))));
                    QilIterator let = this.f.Let(binding);
                    this.varHelper.AddVariable(let);
                    select2 = select.DeepClone(this.f.BaseFactory);
                    select = this.f.Conditional(let, this.f.ConvertToString(select), this.f.String(string.Empty));
                    select2 = this.f.Conditional(let, this.f.Double(0.0), this.f.ConvertToNumber(select2));
                    return;
                }
                string qname = (string) ((QilLiteral) binding);
                if (qname == "number")
                {
                    select = this.f.ConvertToNumber(select);
                    select2 = null;
                    return;
                }
                if ((qname != "text") && !fwdCompat)
                {
                    string str2;
                    string str3;
                    string str4 = this.compiler.ParseQName(qname, out str2, out str3, this) ? this.ResolvePrefix(true, str2) : this.compiler.CreatePhantomNamespace();
                    int length = str4.Length;
                    this.ReportError("Xslt_BistateAttribute", new string[] { "data-type", "text", "number" });
                }
            }
            select = this.f.ConvertToString(select);
            select2 = null;
        }

        private QilNode CompileElement(NodeCtor node)
        {
            QilNode node4;
            QilNode ns = this.CompileStringAvt(node.NsAvt);
            QilNode node3 = this.CompileStringAvt(node.NameAvt);
            if ((node3.NodeType == QilNodeType.LiteralString) && ((ns == null) || (ns.NodeType == QilNodeType.LiteralString)))
            {
                string str2;
                string str3;
                string str4;
                string qname = (string) ((QilLiteral) node3);
                bool flag = this.compiler.ParseQName(qname, out str2, out str3, this);
                if (ns == null)
                {
                    str4 = flag ? this.ResolvePrefix(false, str2) : this.compiler.CreatePhantomNamespace();
                }
                else
                {
                    str4 = (string) ((QilLiteral) ns);
                }
                node4 = this.f.QName(str3, str4, str2);
            }
            else if (ns != null)
            {
                node4 = this.f.StrParseQName(node3, ns);
            }
            else
            {
                node4 = this.ResolveQNameDynamic(false, node3);
            }
            this.outputScope.PushScope();
            this.outputScope.InvalidateAllPrefixes();
            QilNode content = this.CompileInstructions(node.Content);
            this.outputScope.PopScope();
            return this.f.ElementCtor(node4, content);
        }

        private QilNode CompileElementAvailable(QilNode name)
        {
            if (name.NodeType == QilNodeType.LiteralString)
            {
                XmlQualifiedName name2 = this.ResolveQNameThrow(false, name);
                if (this.EvaluateFuncCalls)
                {
                    return this.f.Boolean(IsElementAvailable(name2));
                }
                name = this.f.QName(name2.Name, name2.Namespace);
            }
            else
            {
                name = this.ResolveQNameDynamic(false, name);
            }
            return this.f.InvokeElementAvailable(name);
        }

        private QilNode CompileError(XslNode node) => 
            this.f.Error(this.f.String(node.Select));

        private QilNode CompileFnDocument(QilNode uris, QilNode baseNode)
        {
            QilIterator iterator;
            if (!this.compiler.Settings.EnableDocumentFunction)
            {
                this.ReportWarning("Xslt_DocumentFuncProhibited", new string[0]);
                return this.f.Error(this.lastScope.SourceLine, "Xslt_DocumentFuncProhibited", new string[0]);
            }
            if (uris.XmlType.IsNode)
            {
                return this.f.DocOrderDistinct(this.f.Loop(iterator = this.f.For(uris), this.CompileSingleDocument(this.f.ConvertToString(iterator), baseNode ?? iterator)));
            }
            if (uris.XmlType.IsAtomicValue)
            {
                return this.CompileSingleDocument(this.f.ConvertToString(uris), baseNode);
            }
            QilIterator expr = this.f.Let(uris);
            QilIterator iterator2 = (baseNode != null) ? this.f.Let(baseNode) : null;
            QilNode body = this.f.Conditional(this.f.Not(this.f.IsType(expr, XmlQueryTypeFactory.AnyAtomicType)), this.f.DocOrderDistinct(this.f.Loop(iterator = this.f.For(this.f.TypeAssert(expr, XmlQueryTypeFactory.NodeS)), this.CompileSingleDocument(this.f.ConvertToString(iterator), iterator2 ?? iterator))), this.CompileSingleDocument(this.f.XsltConvert(expr, XmlQueryTypeFactory.StringX), iterator2));
            body = (baseNode != null) ? this.f.Loop(iterator2, body) : body;
            return this.f.Loop(expr, body);
        }

        private QilNode CompileFnKey(QilNode name, QilNode keys, IFocus env)
        {
            QilNode node;
            QilIterator iterator;
            if (keys.XmlType.IsNode)
            {
                if (keys.XmlType.IsSingleton)
                {
                    node = this.CompileSingleKey(name, this.f.ConvertToString(keys), env);
                }
                else
                {
                    node = this.f.Loop(iterator = this.f.For(keys), this.CompileSingleKey(name, this.f.ConvertToString(iterator), env));
                }
            }
            else if (keys.XmlType.IsAtomicValue)
            {
                node = this.CompileSingleKey(name, this.f.ConvertToString(keys), env);
            }
            else
            {
                QilIterator iterator2;
                QilIterator iterator3;
                node = this.f.Loop(iterator2 = this.f.Let(name), this.f.Loop(iterator3 = this.f.Let(keys), this.f.Conditional(this.f.Not(this.f.IsType(iterator3, XmlQueryTypeFactory.AnyAtomicType)), this.f.Loop(iterator = this.f.For(this.f.TypeAssert(iterator3, XmlQueryTypeFactory.NodeS)), this.CompileSingleKey(iterator2, this.f.ConvertToString(iterator), env)), this.CompileSingleKey(iterator2, this.f.XsltConvert(iterator3, XmlQueryTypeFactory.StringX), env))));
            }
            return this.f.DocOrderDistinct(node);
        }

        private QilNode CompileForEach(XslNodeEx node)
        {
            IList<XslNode> content = node.Content;
            LoopFocus curLoop = this.curLoop;
            QilIterator current = this.f.For(this.CompileNodeSetExpression(node.Select));
            this.curLoop.SetFocus(current);
            int varScope = this.varHelper.StartVariables();
            this.curLoop.Sort(this.CompileSorts(content, ref curLoop));
            QilNode expr = this.CompileInstructions(content);
            expr = this.WrapLoopBody(node.ElemNameLi, expr, node.EndTagLi);
            expr = this.AddCurrentPositionLast(expr);
            expr = this.curLoop.ConstructLoop(expr);
            expr = this.varHelper.FinishVariables(expr, varScope);
            this.curLoop = curLoop;
            return expr;
        }

        private QilNode CompileFormatNumber(QilNode value, QilNode formatPicture, QilNode formatName)
        {
            XmlQualifiedName name;
            if (formatName == null)
            {
                name = new XmlQualifiedName();
                formatName = this.f.String(string.Empty);
            }
            else if (formatName.NodeType == QilNodeType.LiteralString)
            {
                name = this.ResolveQNameThrow(true, formatName);
            }
            else
            {
                name = null;
            }
            if (name != null)
            {
                DecimalFormatDecl decl;
                if (this.compiler.DecimalFormats.Contains(name))
                {
                    decl = this.compiler.DecimalFormats[name];
                }
                else
                {
                    if (name != DecimalFormatDecl.Default.Name)
                    {
                        throw new XslLoadException("Xslt_NoDecimalFormat", new string[] { (QilLiteral) formatName });
                    }
                    decl = DecimalFormatDecl.Default;
                }
                if (formatPicture.NodeType == QilNodeType.LiteralString)
                {
                    QilIterator decimalFormatIndex = this.f.Let(this.f.InvokeRegisterDecimalFormatter(formatPicture, decl));
                    decimalFormatIndex.DebugName = this.f.QName("formatter" + this.formatterCnt++, "urn:schemas-microsoft-com:xslt-debug").ToString();
                    this.gloVars.Add((QilNode) decimalFormatIndex);
                    return this.f.InvokeFormatNumberStatic(value, decimalFormatIndex);
                }
                this.formatNumberDynamicUsed = true;
                QilNode node = this.f.QName(name.Name, name.Namespace);
                return this.f.InvokeFormatNumberDynamic(value, formatPicture, node, formatName);
            }
            this.formatNumberDynamicUsed = true;
            QilIterator qilName = this.f.Let(formatName);
            QilNode decimalFormatName = this.ResolveQNameDynamic(true, qilName);
            return this.f.Loop(qilName, this.f.InvokeFormatNumberDynamic(value, formatPicture, decimalFormatName, qilName));
        }

        private QilNode CompileFunctionAvailable(QilNode name)
        {
            if (name.NodeType == QilNodeType.LiteralString)
            {
                XmlQualifiedName name2 = this.ResolveQNameThrow(true, name);
                if (this.EvaluateFuncCalls && ((name2.Namespace.Length == 0) || (name2.Namespace == "http://www.w3.org/1999/XSL/Transform")))
                {
                    return this.f.Boolean(IsFunctionAvailable(name2.Name, name2.Namespace));
                }
                name = this.f.QName(name2.Name, name2.Namespace);
            }
            else
            {
                name = this.ResolveQNameDynamic(true, name);
            }
            return this.f.InvokeFunctionAvailable(name);
        }

        private QilNode CompileGenerateId(QilNode n)
        {
            QilIterator iterator;
            if (n.XmlType.IsSingleton)
            {
                return this.f.XsltGenerateId(n);
            }
            return this.f.StrConcat(this.f.Loop(iterator = this.f.FirstNode(n), this.f.XsltGenerateId(iterator)));
        }

        private void CompileGlobalVariables()
        {
            this.singlFocus.SetFocus(SingletonFocusType.InitialDocumentNode);
            foreach (VarPar par in this.compiler.ExternalPars)
            {
                this.extPars.Add((QilNode) this.CompileGlobalVarPar(par));
            }
            foreach (VarPar par2 in this.compiler.GlobalVars)
            {
                this.gloVars.Add((QilNode) this.CompileGlobalVarPar(par2));
            }
            this.singlFocus.SetFocus((QilIterator) null);
        }

        private QilIterator CompileGlobalVarPar(VarPar varPar)
        {
            QilIterator iterator = (QilIterator) varPar.Value;
            QilList nsList = this.EnterScope(varPar);
            QilNode n = this.CompileVarParValue(varPar);
            SetLineInfo(n, iterator.SourceLine);
            n = this.AddCurrentPositionLast(n);
            n = this.SetDebugNs(n, nsList);
            iterator.SourceLine = SourceLineInfo.NoSource;
            iterator.Binding = n;
            this.ExitScope();
            return iterator;
        }

        private QilNode CompileGroupingSeparatorAttribute(string attValue, bool fwdCompat)
        {
            QilNode binding = this.CompileStringAvt(attValue);
            if (binding == null)
            {
                return this.f.String(string.Empty);
            }
            if (binding.NodeType == QilNodeType.LiteralString)
            {
                string str = (string) ((QilLiteral) binding);
                if (str.Length == 1)
                {
                    return binding;
                }
                if (!fwdCompat)
                {
                    this.ReportError("Xslt_CharAttribute", new string[] { "grouping-separator" });
                }
                return this.f.String(string.Empty);
            }
            QilIterator variable = this.f.Let(binding);
            return this.f.Loop(variable, this.f.Conditional(this.f.Eq(this.f.StrLength(variable), this.f.Int32(1)), variable, fwdCompat ? this.f.String(string.Empty) : this.f.Error(this.lastScope.SourceLine, "Xslt_CharAttribute", new string[] { "grouping-separator" })));
        }

        private QilNode CompileGroupingSizeAttribute(string attValue, bool fwdCompat)
        {
            QilNode n = this.CompileStringAvt(attValue);
            if (n == null)
            {
                return this.f.Double(0.0);
            }
            if (n.NodeType == QilNodeType.LiteralString)
            {
                string s = (string) ((QilLiteral) n);
                double val = XsltFunctions.Round(XPathConvert.StringToDouble(s));
                if ((0.0 <= val) && (val <= 2147483647.0))
                {
                    return this.f.Double(val);
                }
                return this.f.Double(0.0);
            }
            QilIterator variable = this.f.Let(this.f.ConvertToNumber(n));
            return this.f.Loop(variable, this.f.Conditional(this.f.And(this.f.Lt(this.f.Double(0.0), variable), this.f.Lt(variable, this.f.Double(2147483647.0))), variable, this.f.Double(0.0)));
        }

        private QilNode CompileIf(XslNode ifNode) => 
            this.CompileWhen(ifNode, this.InstructionList());

        private QilNode CompileInstructions(IList<XslNode> instructions) => 
            this.CompileInstructions(instructions, 0, this.InstructionList());

        private QilNode CompileInstructions(IList<XslNode> instructions, int from) => 
            this.CompileInstructions(instructions, from, this.InstructionList());

        private QilNode CompileInstructions(IList<XslNode> instructions, QilList content) => 
            this.CompileInstructions(instructions, 0, content);

        private QilNode CompileInstructions(IList<XslNode> instructions, int from, QilList content)
        {
            for (int i = from; i < instructions.Count; i++)
            {
                XslNode node = instructions[i];
                XslNodeType nodeType = node.NodeType;
                if (nodeType != XslNodeType.Param)
                {
                    QilNode node2;
                    QilList nsList = this.EnterScope(node);
                    switch (nodeType)
                    {
                        case XslNodeType.ApplyImports:
                            node2 = this.CompileApplyImports(node);
                            break;

                        case XslNodeType.ApplyTemplates:
                            node2 = this.CompileApplyTemplates((XslNodeEx) node);
                            break;

                        case XslNodeType.Attribute:
                            node2 = this.CompileAttribute((NodeCtor) node);
                            break;

                        case XslNodeType.CallTemplate:
                            node2 = this.CompileCallTemplate((XslNodeEx) node);
                            break;

                        case XslNodeType.Choose:
                            node2 = this.CompileChoose(node);
                            break;

                        case XslNodeType.Comment:
                            node2 = this.CompileComment(node);
                            break;

                        case XslNodeType.Copy:
                            node2 = this.CompileCopy(node);
                            break;

                        case XslNodeType.CopyOf:
                            node2 = this.CompileCopyOf(node);
                            break;

                        case XslNodeType.Element:
                            node2 = this.CompileElement((NodeCtor) node);
                            break;

                        case XslNodeType.Error:
                            node2 = this.CompileError(node);
                            break;

                        case XslNodeType.ForEach:
                            node2 = this.CompileForEach((XslNodeEx) node);
                            break;

                        case XslNodeType.If:
                            node2 = this.CompileIf(node);
                            break;

                        case XslNodeType.List:
                            node2 = this.CompileList(node);
                            break;

                        case XslNodeType.LiteralAttribute:
                            node2 = this.CompileLiteralAttribute(node);
                            break;

                        case XslNodeType.LiteralElement:
                            node2 = this.CompileLiteralElement(node);
                            break;

                        case XslNodeType.Message:
                            node2 = this.CompileMessage(node);
                            break;

                        case XslNodeType.Nop:
                            node2 = this.CompileNop(node);
                            break;

                        case XslNodeType.Number:
                            node2 = this.CompileNumber((System.Xml.Xsl.Xslt.Number) node);
                            break;

                        case XslNodeType.PI:
                            node2 = this.CompilePI(node);
                            break;

                        case XslNodeType.Text:
                            node2 = this.CompileText((Text) node);
                            break;

                        case XslNodeType.UseAttributeSet:
                            node2 = this.CompileUseAttributeSet(node);
                            break;

                        case XslNodeType.ValueOf:
                            node2 = this.CompileValueOf(node);
                            break;

                        case XslNodeType.ValueOfDoe:
                            node2 = this.CompileValueOfDoe(node);
                            break;

                        case XslNodeType.Variable:
                            node2 = this.CompileVariable(node);
                            break;

                        default:
                            node2 = null;
                            break;
                    }
                    this.ExitScope();
                    if ((node2.NodeType != QilNodeType.Sequence) || (node2.Count != 0))
                    {
                        if ((nodeType != XslNodeType.LiteralAttribute) && (nodeType != XslNodeType.UseAttributeSet))
                        {
                            this.SetLineInfoCheck(node2, node.SourceLine);
                        }
                        node2 = this.SetDebugNs(node2, nsList);
                        if (nodeType == XslNodeType.Variable)
                        {
                            QilIterator iterator = this.f.Let(node2);
                            iterator.DebugName = node.Name.ToString();
                            this.scope.AddVariable(node.Name, iterator);
                            node2 = this.f.Loop(iterator, this.CompileInstructions(instructions, (int) (i + 1)));
                            i = instructions.Count;
                        }
                        content.Add(node2);
                    }
                }
            }
            if (!this.IsDebug && (content.Count == 1))
            {
                return content[0];
            }
            return content;
        }

        private QilNode CompileKeyMatch(string pttrn)
        {
            if (this.keyMatchBuilder == null)
            {
                this.keyMatchBuilder = new KeyMatchBuilder(this);
            }
            this.SetEnvironmentFlags(false, false, false);
            if (pttrn == null)
            {
                return this.PhantomKeyMatch();
            }
            try
            {
                XPathScanner scanner = new XPathScanner(pttrn);
                return this.ptrnParser.Parse(scanner, this.keyMatchBuilder);
            }
            catch (XslLoadException exception)
            {
                if (this.xslVersion != XslVersion.ForwardsCompatible)
                {
                    this.ReportErrorInXPath(exception);
                }
                return this.f.Error(this.f.String(exception.Message));
            }
        }

        private void CompileKeys()
        {
            for (int i = 0; i < this.compiler.Keys.Count; i++)
            {
                foreach (Key key in this.compiler.Keys[i])
                {
                    this.EnterScope(key);
                    QilParameter current = this.f.Parameter(XmlQueryTypeFactory.NodeNotRtf);
                    this.singlFocus.SetFocus(current);
                    QilIterator iterator = this.f.For(this.f.OptimizeBarrier(this.CompileKeyMatch(key.Match)));
                    this.singlFocus.SetFocus(iterator);
                    QilIterator variable = this.f.For(this.CompileKeyUse(key.Use));
                    variable = this.f.For(this.f.OptimizeBarrier(this.f.Loop(variable, this.f.ConvertToString(variable))));
                    QilParameter parameter2 = this.f.Parameter(XmlQueryTypeFactory.StringX);
                    QilFunction n = this.f.Function(this.f.FormalParameterList(current, parameter2), this.f.Filter(iterator, this.f.Not(this.f.IsEmpty(this.f.Filter(variable, this.f.Eq(variable, parameter2))))), this.f.False());
                    n.DebugName = key.GetDebugName();
                    SetLineInfo(n, key.SourceLine);
                    key.Function = n;
                    this.functions.Add((QilNode) n);
                    this.ExitScope();
                }
            }
            this.singlFocus.SetFocus((QilIterator) null);
        }

        private QilNode CompileKeyUse(string expr)
        {
            QilNode node;
            this.SetEnvironmentFlags(false, true, false);
            if (expr == null)
            {
                node = this.PhantomXPathExpression();
            }
            else
            {
                try
                {
                    XPathScanner scanner = new XPathScanner(expr);
                    node = this.xpathParser.Parse(scanner, this.xpathBuilder, LexKind.Eof);
                }
                catch (XslLoadException exception)
                {
                    if (this.xslVersion != XslVersion.ForwardsCompatible)
                    {
                        this.ReportErrorInXPath(exception);
                    }
                    node = this.f.Error(this.f.String(exception.Message));
                }
            }
            if (node is QilIterator)
            {
                node = this.f.Nop(node);
            }
            return node;
        }

        private QilNode CompileLangAttribute(string attValue, bool fwdCompat)
        {
            QilIterator iterator;
            QilNode binding = this.CompileStringAvt(attValue);
            if (binding == null)
            {
                return binding;
            }
            if (binding.NodeType == QilNodeType.LiteralString)
            {
                string lang = (string) ((QilLiteral) binding);
                if (XsltLibrary.LangToLcidInternal(lang, fwdCompat, this) == 0x7f)
                {
                    binding = null;
                }
                return binding;
            }
            return this.f.Loop(iterator = this.f.Let(binding), this.f.Conditional(this.f.Eq(this.f.InvokeLangToLcid(iterator, fwdCompat), this.f.Int32(0x7f)), this.f.String(string.Empty), iterator));
        }

        private QilNode CompileLangAttributeToLcid(string attValue, bool fwdCompat) => 
            this.CompileLangToLcid(this.CompileStringAvt(attValue), fwdCompat);

        private QilNode CompileLangToLcid(QilNode lang, bool fwdCompat)
        {
            if (lang == null)
            {
                return this.f.Double(127.0);
            }
            if (lang.NodeType == QilNodeType.LiteralString)
            {
                return this.f.Double((double) XsltLibrary.LangToLcidInternal((string) ((QilLiteral) lang), fwdCompat, this));
            }
            return this.f.XsltConvert(this.f.InvokeLangToLcid(lang, fwdCompat), XmlQueryTypeFactory.DoubleX);
        }

        private QilNode CompileLetterValueAttribute(string attValue, bool fwdCompat)
        {
            QilNode binding = this.CompileStringAvt(attValue);
            if (binding != null)
            {
                if (binding.NodeType != QilNodeType.LiteralString)
                {
                    QilIterator variable = this.f.Let(binding);
                    return this.f.Loop(variable, this.f.Conditional(this.f.Or(this.f.Eq(variable, this.f.String("alphabetic")), this.f.Eq(variable, this.f.String("traditional"))), variable, fwdCompat ? this.f.String("default") : this.f.Error(this.lastScope.SourceLine, "Xslt_BistateAttribute", new string[] { "letter-value", "alphabetic", "traditional" })));
                }
                switch (((string) ((QilLiteral) binding)))
                {
                    case "alphabetic":
                    case "traditional":
                        return binding;
                }
                if (!fwdCompat)
                {
                    this.ReportError("Xslt_BistateAttribute", new string[] { "letter-value", "alphabetic", "traditional" });
                    return binding;
                }
            }
            return this.f.String("default");
        }

        private QilNode CompileList(XslNode node) => 
            this.CompileInstructions(node.Content);

        private QilNode CompileLiteralAttribute(XslNode node)
        {
            QilName name = node.Name;
            string prefix = name.Prefix;
            string namespaceUri = name.NamespaceUri;
            if (prefix.Length != 0)
            {
                this.compiler.ApplyNsAliases(ref prefix, ref namespaceUri);
            }
            name.Prefix = prefix;
            name.NamespaceUri = namespaceUri;
            return this.f.AttributeCtor(name, this.CompileTextAvt(node.Select));
        }

        private QilNode CompileLiteralElement(XslNode node)
        {
            bool flag = true;
        Label_0002:
            this.prefixesInUse.Clear();
            QilName name = node.Name;
            string prefix = name.Prefix;
            string namespaceUri = name.NamespaceUri;
            this.compiler.ApplyNsAliases(ref prefix, ref namespaceUri);
            if (flag)
            {
                this.prefixesInUse.Add(prefix, namespaceUri);
            }
            else
            {
                prefix = name.Prefix;
            }
            this.outputScope.PushScope();
            QilList content = this.InstructionList();
            foreach (CompilerScopeManager<QilIterator>.ScopeRecord record in this.scope)
            {
                string ncName = record.ncName;
                string nsUri = record.nsUri;
                if ((nsUri == "http://www.w3.org/1999/XSL/Transform") || this.scope.IsExNamespace(nsUri))
                {
                    continue;
                }
                this.compiler.ApplyNsAliases(ref ncName, ref nsUri);
                if (flag)
                {
                    if (this.prefixesInUse.Contains(ncName))
                    {
                        if (((string) this.prefixesInUse[ncName]) == nsUri)
                        {
                            goto Label_0117;
                        }
                        this.outputScope.PopScope();
                        flag = false;
                        goto Label_0002;
                    }
                    this.prefixesInUse.Add(ncName, nsUri);
                }
                else
                {
                    ncName = record.ncName;
                }
            Label_0117:
                this.AddNsDecl(content, ncName, nsUri);
            }
            QilNode node2 = this.CompileInstructions(node.Content, content);
            this.outputScope.PopScope();
            name.Prefix = prefix;
            name.NamespaceUri = namespaceUri;
            return this.f.ElementCtor(name, node2);
        }

        private QilNode CompileMatchPattern(string pttrn)
        {
            QilNode node;
            this.SetEnvironmentFlags(false, false, true);
            try
            {
                XPathScanner scanner = new XPathScanner(pttrn);
                node = this.ptrnParser.Parse(scanner, this.ptrnBuilder);
            }
            catch (XslLoadException exception)
            {
                if (this.xslVersion != XslVersion.ForwardsCompatible)
                {
                    this.ReportErrorInXPath(exception);
                }
                node = this.f.Loop(this.f.For(this.ptrnBuilder.FixupNode), this.f.Error(this.f.String(exception.Message)));
                XPathPatternBuilder.SetPriority(node, 0.5);
            }
            return node;
        }

        private QilNode CompileMessage(XslNode node)
        {
            QilIterator iterator;
            string uri = this.lastScope.SourceLine.Uri;
            QilNode n = this.f.RtfCtor(this.CompileInstructions(node.Content), this.f.String(uri));
            n = this.f.InvokeOuterXml(n);
            if (!((bool) node.Arg))
            {
                return this.f.Warning(n);
            }
            return this.f.Loop(iterator = this.f.Let(n), this.f.Sequence(this.f.Warning(iterator), this.f.Error(iterator)));
        }

        private QilNode CompileMsNodeSet(QilNode n)
        {
            if (n.XmlType.IsNode && n.XmlType.IsNotRtf)
            {
                return n;
            }
            return this.f.XsltConvert(n, XmlQueryTypeFactory.NodeDodS);
        }

        private QilNode CompileNodeSetExpression(string expr)
        {
            QilNode n = this.CompileXPathExpression(expr);
            if (!this.f.CannotBeNodeSet(n))
            {
                return this.f.EnsureNodeSet(n);
            }
            XPathCompileException e = new XPathCompileException(expr, 0, expr.Length, "XPath_NodeSetExpected", null);
            if (this.xslVersion != XslVersion.ForwardsCompatible)
            {
                this.ReportErrorInXPath(e);
            }
            return this.f.Error(this.f.String(e.Message));
        }

        private QilNode CompileNop(XslNode node) => 
            this.f.Nop(this.f.Sequence());

        private QilNode CompileNumber(System.Xml.Xsl.Xslt.Number num)
        {
            QilNode node;
            bool flag;
            if (num.Value != null)
            {
                node = this.f.ConvertToNumber(this.CompileXPathExpression(num.Value));
            }
            else
            {
                QilNode countPattern = (num.Count != null) ? this.CompileNumberPattern(num.Count) : null;
                QilNode fromPattern = (num.From != null) ? this.CompileNumberPattern(num.From) : null;
                switch (num.Level)
                {
                    case NumberLevel.Single:
                        node = this.PlaceMarker(countPattern, fromPattern, false);
                        goto Label_008C;

                    case NumberLevel.Multiple:
                        node = this.PlaceMarker(countPattern, fromPattern, true);
                        goto Label_008C;
                }
                node = this.PlaceMarkerAny(countPattern, fromPattern);
            }
        Label_008C:
            flag = num.ForwardsCompatible;
            return this.f.TextCtor(this.f.InvokeNumberFormat(node, this.CompileStringAvt(num.Format), this.CompileLangAttributeToLcid(num.Lang, flag), this.CompileLetterValueAttribute(num.LetterValue, flag), this.CompileGroupingSeparatorAttribute(num.GroupingSeparator, flag), this.CompileGroupingSizeAttribute(num.GroupingSize, flag)));
        }

        private QilNode CompileNumberPattern(string pttrn)
        {
            this.SetEnvironmentFlags(true, false, true);
            try
            {
                XPathScanner scanner = new XPathScanner(pttrn);
                return this.ptrnParser.Parse(scanner, this.ptrnBuilder);
            }
            catch (XslLoadException exception)
            {
                if (this.xslVersion != XslVersion.ForwardsCompatible)
                {
                    this.ReportErrorInXPath(exception);
                }
                return this.f.Error(this.f.String(exception.Message));
            }
        }

        private QilNode CompileOrderAttribute(string attName, string attValue, string value0, string value1, bool fwdCompat)
        {
            QilIterator iterator;
            QilNode binding = this.CompileStringAvt(attValue);
            if (binding == null)
            {
                return binding;
            }
            if (binding.NodeType == QilNodeType.LiteralString)
            {
                string str = (string) ((QilLiteral) binding);
                if (str == value1)
                {
                    return this.f.String("1");
                }
                if ((str != value0) && !fwdCompat)
                {
                    this.ReportError("Xslt_BistateAttribute", new string[] { attName, value0, value1 });
                }
                return this.f.String("0");
            }
            return this.f.Loop(iterator = this.f.Let(binding), this.f.Conditional(this.f.Eq(iterator, this.f.String(value1)), this.f.String("1"), fwdCompat ? this.f.String("0") : this.f.Conditional(this.f.Eq(iterator, this.f.String(value0)), this.f.String("0"), this.f.Error(this.lastScope.SourceLine, "Xslt_BistateAttribute", new string[] { attName, value0, value1 }))));
        }

        private QilNode CompilePI(XslNode node)
        {
            QilNode name = this.CompileStringAvt(node.Select);
            if (name.NodeType == QilNodeType.LiteralString)
            {
                string str = (string) ((QilLiteral) name);
                this.compiler.ValidatePiName(str, this);
            }
            return this.f.PICtor(name, this.CompileInstructions(node.Content));
        }

        private void CompileProtoTemplate(ProtoTemplate tmpl)
        {
            this.EnterScope(tmpl);
            this.funcFocus.StartFocus(tmpl.Function.Arguments, !this.IsDebug ? tmpl.Flags : XslFlags.FocusFilter);
            foreach (QilParameter parameter in tmpl.Function.Arguments)
            {
                if (parameter.Name.NamespaceUri != "urn:schemas-microsoft-com:xslt-debug")
                {
                    if (this.IsDebug)
                    {
                        VarPar annotation = (VarPar) parameter.Annotation;
                        QilList nsList = this.EnterScope(annotation);
                        parameter.DefaultValue = this.CompileVarParValue(annotation);
                        this.ExitScope();
                        parameter.DefaultValue = this.SetDebugNs(parameter.DefaultValue, nsList);
                    }
                    this.scope.AddVariable(parameter.Name, parameter);
                }
            }
            tmpl.Function.Definition = this.CompileInstructions(tmpl.Content);
            this.funcFocus.StopFocus();
            this.ExitScope();
        }

        private QilNode CompileRootExpression(XslNode applyTmpls)
        {
            QilNode left = this.f.Int32(0);
            if (this.formatNumberDynamicUsed || this.IsDebug)
            {
                foreach (DecimalFormatDecl decl in this.compiler.DecimalFormats)
                {
                    left = this.f.Add(left, this.f.InvokeRegisterDecimalFormat(decl));
                }
            }
            foreach (string str in this.compiler.Scripts.ScriptClasses.Keys)
            {
                left = this.f.Add(left, this.f.InvokeCheckScriptNamespace(str));
            }
            this.singlFocus.SetFocus(SingletonFocusType.InitialContextNode);
            QilNode trueBranch = this.GenerateApply(null, applyTmpls);
            this.singlFocus.SetFocus((QilIterator) null);
            if (left.NodeType == QilNodeType.Add)
            {
                trueBranch = this.f.Conditional(this.f.Eq(left, this.f.Int32(0)), trueBranch, this.f.Sequence());
            }
            return this.f.DocumentCtor(trueBranch);
        }

        private QilNode CompileSingleDocument(QilNode uri, QilNode baseNode)
        {
            QilNode node;
            if (baseNode == null)
            {
                node = this.f.String(this.lastScope.SourceLine.Uri);
            }
            else if (baseNode.XmlType.IsSingleton)
            {
                node = this.f.InvokeBaseUri(baseNode);
            }
            else
            {
                QilIterator iterator;
                node = this.f.StrConcat(this.f.Loop(iterator = this.f.FirstNode(baseNode), this.f.InvokeBaseUri(iterator)));
            }
            return this.f.DataSource(uri, node);
        }

        private QilNode CompileSingleKey(List<Key> defList, QilIterator key, QilIterator context)
        {
            QilList list = this.f.BaseFactory.Sequence();
            QilNode node = null;
            foreach (Key key2 in defList)
            {
                node = this.f.Invoke(key2.Function, this.f.ActualParameterList(context, key));
                list.Add(node);
            }
            if (defList.Count != 1)
            {
                return list;
            }
            return node;
        }

        private QilNode CompileSingleKey(List<Key> defList, QilNode key, IFocus env)
        {
            if (defList.Count == 1)
            {
                return this.f.Invoke(defList[0].Function, this.f.ActualParameterList(env.GetCurrent(), key));
            }
            QilIterator iterator = this.f.Let(key);
            QilNode body = this.f.Sequence();
            foreach (Key key2 in defList)
            {
                body.Add(this.f.Invoke(key2.Function, this.f.ActualParameterList(env.GetCurrent(), iterator)));
            }
            return this.f.Loop(iterator, body);
        }

        private QilNode CompileSingleKey(QilNode name, QilNode key, IFocus env)
        {
            if (name.NodeType == QilNodeType.LiteralString)
            {
                string str2;
                string str3;
                string qname = (string) ((QilLiteral) name);
                this.compiler.ParseQName(qname, out str2, out str3, new ThrowErrorHelper());
                string uri = this.ResolvePrefixThrow(true, str2);
                QilName name2 = this.f.QName(str3, uri, str2);
                if (!this.compiler.Keys.Contains(name2))
                {
                    throw new XslLoadException("Xslt_UndefinedKey", new string[] { qname });
                }
                return this.CompileSingleKey(this.compiler.Keys[name2], key, env);
            }
            if (this.generalKey == null)
            {
                this.generalKey = this.CreateGeneralKeyFunction();
            }
            QilIterator qilName = this.f.Let(name);
            QilNode node2 = this.ResolveQNameDynamic(true, qilName);
            QilNode body = this.f.Invoke(this.generalKey, this.f.ActualParameterList(new QilNode[] { qilName, node2, key, env.GetCurrent() }));
            return this.f.Loop(qilName, body);
        }

        private void CompileSort(Sort sort, QilList keyList, ref LoopFocus parentLoop)
        {
            QilNode node2;
            QilNode node3;
            QilNode node4;
            QilNode node5;
            this.EnterScope(sort);
            bool forwardsCompatible = sort.ForwardsCompatible;
            QilNode select = this.CompileXPathExpression(sort.Select);
            if (((sort.Lang != null) || (sort.DataType != null)) || ((sort.Order != null) || (sort.CaseOrder != null)))
            {
                LoopFocus curLoop = this.curLoop;
                this.curLoop = parentLoop;
                node3 = this.CompileLangAttribute(sort.Lang, forwardsCompatible);
                this.CompileDataTypeAttribute(sort.DataType, forwardsCompatible, ref select, out node2);
                node4 = this.CompileOrderAttribute("order", sort.Order, "ascending", "descending", forwardsCompatible);
                node5 = this.CompileOrderAttribute("case-order", sort.CaseOrder, "lower-first", "upper-first", forwardsCompatible);
                this.curLoop = curLoop;
            }
            else
            {
                select = this.f.ConvertToString(select);
                node2 = node3 = node4 = (QilNode) (node5 = null);
            }
            this.strConcat.Reset();
            this.strConcat.Append("http://collations.microsoft.com");
            this.strConcat.Append('/');
            this.strConcat.Append(node3);
            char ch = '?';
            if (node4 != null)
            {
                this.strConcat.Append(ch);
                this.strConcat.Append("descendingOrder=");
                this.strConcat.Append(node4);
                ch = '&';
            }
            if (node5 != null)
            {
                this.strConcat.Append(ch);
                this.strConcat.Append("upperFirst=");
                this.strConcat.Append(node5);
                ch = '&';
            }
            QilNode collation = this.strConcat.ToQil();
            QilSortKey key = this.f.SortKey(select, collation);
            keyList.Add((QilNode) key);
            if (node2 != null)
            {
                key = this.f.SortKey(node2, collation.DeepClone(this.f.BaseFactory));
                keyList.Add((QilNode) key);
            }
            this.ExitScope();
        }

        private QilNode CompileSorts(IList<XslNode> content, ref LoopFocus parentLoop)
        {
            QilList keyList = this.f.BaseFactory.SortKeyList();
            int index = 0;
            while (index < content.Count)
            {
                Sort sort = content[index] as Sort;
                if (sort != null)
                {
                    this.CompileSort(sort, keyList, ref parentLoop);
                    content.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
            if (keyList.Count == 0)
            {
                return null;
            }
            return keyList;
        }

        private QilNode CompileStringAvt(string avt)
        {
            if (avt == null)
            {
                return null;
            }
            if (avt.IndexOfAny(curlyBraces) == -1)
            {
                return this.f.String(avt);
            }
            return this.f.StrConcat(this.CompileAvt(avt));
        }

        public static QilExpression CompileStylesheet(System.Xml.Xsl.Xslt.Compiler compiler) => 
            new QilGenerator(compiler.IsDebug).Compile(compiler);

        private QilNode CompileSystemProperty(QilNode name)
        {
            if (name.NodeType == QilNodeType.LiteralString)
            {
                XmlQualifiedName name2 = this.ResolveQNameThrow(true, name);
                if (this.EvaluateFuncCalls)
                {
                    XPathItem item = XsltFunctions.SystemProperty(name2);
                    if (item.ValueType == XsltConvert.StringType)
                    {
                        return this.f.String(item.Value);
                    }
                    return this.f.Double(item.ValueAsDouble);
                }
                name = this.f.QName(name2.Name, name2.Namespace);
            }
            else
            {
                name = this.ResolveQNameDynamic(true, name);
            }
            return this.f.InvokeSystemProperty(name);
        }

        private QilNode CompileText(Text node)
        {
            if (node.Hints == SerializationHints.None)
            {
                return this.f.TextCtor(this.f.String(node.Select));
            }
            return this.f.RawTextCtor(this.f.String(node.Select));
        }

        private QilNode CompileTextAvt(string avt)
        {
            if (avt.IndexOfAny(curlyBraces) == -1)
            {
                return this.f.TextCtor(this.f.String(avt));
            }
            QilNode content = this.CompileAvt(avt);
            if (content.NodeType != QilNodeType.Sequence)
            {
                return this.f.TextCtor(content);
            }
            QilList list = this.InstructionList();
            foreach (QilNode node2 in content)
            {
                list.Add(this.f.TextCtor(node2));
            }
            return list;
        }

        private QilNode CompileUnparsedEntityUri(QilNode n) => 
            this.f.Error(this.lastScope.SourceLine, "Xslt_UnsupportedXsltFunction", new string[] { "unparsed-entity-uri" });

        private QilNode CompileUseAttributeSet(XslNode node)
        {
            AttributeSet set;
            this.outputScope.InvalidateAllPrefixes();
            if (this.compiler.AttributeSets.TryGetValue(node.Name, out set))
            {
                return this.GenerateCall(set.Function, node);
            }
            if (!this.compiler.IsPhantomName(node.Name))
            {
                this.compiler.ReportError(node.SourceLine, "Xslt_NoAttributeSet", new string[] { node.Name.QualifiedName });
            }
            return this.f.Sequence();
        }

        private QilNode CompileValueOf(XslNode valueOf) => 
            this.f.TextCtor(this.f.ConvertToString(this.CompileXPathExpression(valueOf.Select)));

        private QilNode CompileValueOfDoe(XslNode valueOf) => 
            this.f.RawTextCtor(this.f.ConvertToString(this.CompileXPathExpression(valueOf.Select)));

        private QilNode CompileVariable(XslNode node)
        {
            if (this.scope.IsLocalVariable(node.Name.LocalName, node.Name.NamespaceUri))
            {
                this.ReportError("Xslt_DupLocalVariable", new string[] { node.Name.QualifiedName });
            }
            return this.CompileVarParValue(node);
        }

        private QilNode CompileVarParValue(XslNode node)
        {
            QilNode node2;
            string uri = this.lastScope.SourceLine.Uri;
            IList<XslNode> content = node.Content;
            string select = node.Select;
            if (select != null)
            {
                QilList list2 = this.InstructionList();
                list2.Add(this.CompileXPathExpression(select));
                node2 = this.CompileInstructions(content, list2);
            }
            else if (content.Count != 0)
            {
                this.outputScope.PushScope();
                this.outputScope.InvalidateAllPrefixes();
                node2 = this.f.RtfCtor(this.CompileInstructions(content), this.f.String(uri));
                this.outputScope.PopScope();
            }
            else
            {
                node2 = this.f.String(string.Empty);
            }
            if (this.IsDebug)
            {
                node2 = this.f.TypeAssert(node2, XmlQueryTypeFactory.ItemS);
            }
            return node2;
        }

        private QilNode CompileWhen(XslNode whenNode, QilNode otherwise) => 
            this.f.Conditional(this.f.ConvertToBoolean(this.CompileXPathExpression(whenNode.Select)), this.CompileInstructions(whenNode.Content), otherwise);

        private void CompileWithParam(VarPar withParam)
        {
            QilList nsList = this.EnterScope(withParam);
            QilNode n = this.CompileVarParValue(withParam);
            this.ExitScope();
            SetLineInfo(n, withParam.SourceLine);
            n = this.SetDebugNs(n, nsList);
            withParam.Value = n;
        }

        private QilNode CompileXPathExpression(string expr)
        {
            QilNode node;
            this.SetEnvironmentFlags(true, true, true);
            if (expr == null)
            {
                node = this.PhantomXPathExpression();
            }
            else
            {
                try
                {
                    XPathScanner scanner = new XPathScanner(expr);
                    node = this.xpathParser.Parse(scanner, this.xpathBuilder, LexKind.Eof);
                }
                catch (XslLoadException exception)
                {
                    if (this.xslVersion != XslVersion.ForwardsCompatible)
                    {
                        this.ReportErrorInXPath(exception);
                    }
                    node = this.f.Error(this.f.String(exception.Message));
                }
            }
            if (node is QilIterator)
            {
                node = this.f.Nop(node);
            }
            return node;
        }

        private QilNode CompileXPathExpressionWithinAvt(string expr, ref int pos)
        {
            QilNode node;
            this.SetEnvironmentFlags(true, true, true);
            try
            {
                XPathScanner scanner = new XPathScanner(expr, pos);
                node = this.xpathParser.Parse(scanner, this.xpathBuilder, LexKind.RBrace);
                pos = scanner.LexStart + 1;
            }
            catch (XslLoadException exception)
            {
                if (this.xslVersion != XslVersion.ForwardsCompatible)
                {
                    this.ReportErrorInXPath(exception);
                }
                node = this.f.Error(this.f.String(exception.Message));
                pos = expr.Length;
            }
            if (node is QilIterator)
            {
                node = this.f.Nop(node);
            }
            return node;
        }

        private static Dictionary<string, XPathBuilder.FunctionInfo<FuncId>> CreateFunctionTable() => 
            new Dictionary<string, XPathBuilder.FunctionInfo<FuncId>>(0x10) { 
                { 
                    "current",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.Current, 0, 0, null)
                },
                { 
                    "document",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.Document, 1, 2, argFnDocument)
                },
                { 
                    "key",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.Key, 2, 2, argFnKey)
                },
                { 
                    "format-number",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.FormatNumber, 2, 3, argFnFormatNumber)
                },
                { 
                    "unparsed-entity-uri",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.UnparsedEntityUri, 1, 1, XPathBuilder.argString)
                },
                { 
                    "generate-id",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.GenerateId, 0, 1, XPathBuilder.argNodeSet)
                },
                { 
                    "system-property",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.SystemProperty, 1, 1, XPathBuilder.argString)
                },
                { 
                    "element-available",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.ElementAvailable, 1, 1, XPathBuilder.argString)
                },
                { 
                    "function-available",
                    new XPathBuilder.FunctionInfo<FuncId>(FuncId.FunctionAvailable, 1, 1, XPathBuilder.argString)
                }
            };

        private QilFunction CreateGeneralKeyFunction()
        {
            QilIterator args = this.f.Parameter(XmlQueryTypeFactory.StringX);
            QilIterator left = this.f.Parameter(XmlQueryTypeFactory.QNameX);
            QilIterator key = this.f.Parameter(XmlQueryTypeFactory.StringX);
            QilIterator context = this.f.Parameter(XmlQueryTypeFactory.NodeNotRtf);
            QilNode falseBranch = this.f.Error("Xslt_UndefinedKey", args);
            for (int i = 0; i < this.compiler.Keys.Count; i++)
            {
                falseBranch = this.f.Conditional(this.f.Eq(left, this.compiler.Keys[i][0].Name.DeepClone(this.f.BaseFactory)), this.CompileSingleKey(this.compiler.Keys[i], key, context), falseBranch);
            }
            QilFunction function = this.f.Function(this.f.FormalParameterList(new QilNode[] { args, left, key, context }), falseBranch, this.f.False());
            function.DebugName = "key";
            this.functions.Add((QilNode) function);
            return function;
        }

        private void CreateGlobalVarPar(VarPar varPar)
        {
            QilIterator iterator;
            XmlQueryType t = this.ChooseBestType(varPar);
            if (varPar.NodeType == XslNodeType.Variable)
            {
                iterator = this.f.Let(this.f.Unknown(t));
            }
            else
            {
                iterator = this.f.Parameter(null, varPar.Name, t);
            }
            iterator.DebugName = varPar.Name.ToString();
            varPar.Value = iterator;
            SetLineInfo(iterator, varPar.SourceLine);
            this.scope.AddVariable(varPar.Name, iterator);
        }

        private void CreateGlobalVarPars()
        {
            foreach (VarPar par in this.compiler.ExternalPars)
            {
                this.CreateGlobalVarPar(par);
            }
            foreach (VarPar par2 in this.compiler.GlobalVars)
            {
                this.CreateGlobalVarPar(par2);
            }
        }

        public static VarPar CreateWithParam(QilName name, QilNode value)
        {
            VarPar par = AstFactory.WithParam(name);
            par.Value = value;
            return par;
        }

        private QilParameter CreateXslParam(QilName name, XmlQueryType xt)
        {
            QilParameter parameter = this.f.Parameter(xt);
            parameter.DebugName = name.ToString();
            parameter.Name = name;
            return parameter;
        }

        private QilList EnterScope(XslNode node)
        {
            this.lastScope = node;
            this.xslVersion = node.XslVersion;
            this.scope.PushScope();
            bool flag = false;
            NsDecl namespaces = node.Namespaces;
            while (namespaces != null)
            {
                this.scope.AddNamespace(namespaces.Prefix, namespaces.NsUri);
                namespaces = namespaces.Prev;
                flag = true;
            }
            if (flag)
            {
                return this.BuildDebuggerNamespaces();
            }
            return null;
        }

        private void ExitScope()
        {
            this.scope.PopScope();
        }

        private QilNode EXslObjectType(QilNode n)
        {
            if (this.EvaluateFuncCalls)
            {
                switch (n.XmlType.TypeCode)
                {
                    case XmlTypeCode.String:
                        return this.f.String("string");

                    case XmlTypeCode.Boolean:
                        return this.f.String("boolean");

                    case XmlTypeCode.Double:
                        return this.f.String("number");
                }
                if (n.XmlType.IsNode && n.XmlType.IsNotRtf)
                {
                    return this.f.String("node-set");
                }
            }
            return this.f.InvokeEXslObjectType(n);
        }

        private QilNode ExtractText(string source, ref int pos)
        {
            int startIndex = pos;
            this.unescapedText.Length = 0;
            int num = pos;
            while (num < source.Length)
            {
                char ch = source[num];
                if ((ch == '{') || (ch == '}'))
                {
                    if (((num + 1) >= source.Length) || (source[num + 1] != ch))
                    {
                        if (ch != '{')
                        {
                            pos = source.Length;
                            if (this.xslVersion != XslVersion.ForwardsCompatible)
                            {
                                this.ReportError("Xslt_SingleRightBraceInAvt", new string[] { source });
                                return null;
                            }
                            return this.f.Error(this.lastScope.SourceLine, "Xslt_SingleRightBraceInAvt", new string[] { source });
                        }
                        break;
                    }
                    num++;
                    this.unescapedText.Append(source, startIndex, num - startIndex);
                    startIndex = num + 1;
                }
                num++;
            }
            pos = num;
            if (this.unescapedText.Length == 0)
            {
                if (num <= startIndex)
                {
                    return null;
                }
                return this.f.String(source.Substring(startIndex, num - startIndex));
            }
            this.unescapedText.Append(source, startIndex, num - startIndex);
            return this.f.String(this.unescapedText.ToString());
        }

        private bool FillupInvokeArgs(IList<QilNode> formalArgs, IList<XslNode> actualArgs, QilList invokeArgs)
        {
            if (actualArgs.Count != formalArgs.Count)
            {
                return false;
            }
            invokeArgs.Clear();
            for (int i = 0; i < formalArgs.Count; i++)
            {
                QilName name = ((QilParameter) formalArgs[i]).Name;
                XmlQueryType xmlType = formalArgs[i].XmlType;
                QilNode node = null;
                for (int j = 0; j < actualArgs.Count; j++)
                {
                    VarPar par = (VarPar) actualArgs[j];
                    if (name.Equals(par.Name))
                    {
                        QilNode node2 = par.Value;
                        XmlQueryType type2 = node2.XmlType;
                        if ((type2 != xmlType) && ((!type2.IsNode || !xmlType.IsNode) || !type2.IsSubtypeOf(xmlType)))
                        {
                            return false;
                        }
                        node = node2;
                        break;
                    }
                }
                if (node == null)
                {
                    return false;
                }
                invokeArgs.Add(node);
            }
            return true;
        }

        private QilNode GenerateApply(Stylesheet sheet, XslNode node)
        {
            if (this.compiler.Settings.CheckOnly)
            {
                return this.f.Sequence();
            }
            this.AddImplicitArgs(node);
            return this.InvokeApplyFunction(sheet, node.Name, node.Content);
        }

        private QilNode GenerateCall(QilFunction func, XslNode node)
        {
            this.AddImplicitArgs(node);
            return this.invkGen.GenerateInvoke(func, node.Content);
        }

        private QilNode GenerateScriptCall(QilName name, XmlExtensionFunction scrFunc, IList<QilNode> args)
        {
            for (int i = 0; i < args.Count; i++)
            {
                XmlQueryType xmlArgumentType = scrFunc.GetXmlArgumentType(i);
                switch (xmlArgumentType.TypeCode)
                {
                    case XmlTypeCode.Node:
                        args[i] = xmlArgumentType.IsSingleton ? this.f.ConvertToNode(args[i]) : this.f.ConvertToNodeSet(args[i]);
                        break;

                    case XmlTypeCode.String:
                        args[i] = this.f.ConvertToString(args[i]);
                        break;

                    case XmlTypeCode.Boolean:
                        args[i] = this.f.ConvertToBoolean(args[i]);
                        break;

                    case XmlTypeCode.Double:
                        args[i] = this.f.ConvertToNumber(args[i]);
                        break;
                }
            }
            return this.f.XsltInvokeEarlyBound(name, scrFunc.Method, scrFunc.XmlReturnType, args);
        }

        private QilNode GetCurrentNode()
        {
            if (this.curLoop.IsFocusSet)
            {
                return this.curLoop.GetCurrent();
            }
            if (this.funcFocus.IsFocusSet)
            {
                return this.funcFocus.GetCurrent();
            }
            return this.singlFocus.GetCurrent();
        }

        private QilNode GetCurrentPosition()
        {
            if (this.curLoop.IsFocusSet)
            {
                return this.curLoop.GetPosition();
            }
            if (this.funcFocus.IsFocusSet)
            {
                return this.funcFocus.GetPosition();
            }
            return this.singlFocus.GetPosition();
        }

        private QilNode GetLastPosition()
        {
            if (this.curLoop.IsFocusSet)
            {
                return this.curLoop.GetLast();
            }
            if (this.funcFocus.IsFocusSet)
            {
                return this.funcFocus.GetLast();
            }
            return this.singlFocus.GetLast();
        }

        private QilIterator GetNsVar(QilList nsList)
        {
            foreach (QilIterator iterator in this.nsVars)
            {
                QilList binding = (QilList) iterator.Binding;
                if (binding.Count == nsList.Count)
                {
                    bool flag = true;
                    for (int i = 0; i < nsList.Count; i++)
                    {
                        if ((((QilLiteral) ((QilBinary) nsList[i]).Right).Value != ((QilLiteral) ((QilBinary) binding[i]).Right).Value) || (((QilLiteral) ((QilBinary) nsList[i]).Left).Value != ((QilLiteral) ((QilBinary) binding[i]).Left).Value))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        return iterator;
                    }
                }
            }
            QilIterator iterator2 = this.f.Let(nsList);
            iterator2.DebugName = this.f.QName("ns" + this.nsVars.Count, "urn:schemas-microsoft-com:xslt-debug").ToString();
            this.gloVars.Add((QilNode) iterator2);
            this.nsVars.Add((QilNode) iterator2);
            return iterator2;
        }

        private QilList InstructionList() => 
            this.f.BaseFactory.Sequence();

        private QilNode InvokeApplyFunction(Stylesheet sheet, QilName mode, IList<XslNode> actualArgs)
        {
            XslFlags none;
            List<QilFunction> list2;
            if (!this.compiler.ModeFlags.TryGetValue(mode, out none))
            {
                none = XslFlags.None;
            }
            if (this.IsDebug)
            {
                none = XslFlags.FocusFilter;
            }
            none |= XslFlags.Current;
            QilList invokeArgs = this.f.ActualParameterList();
            QilFunction item = null;
            Dictionary<QilName, List<QilFunction>> dictionary = (sheet == null) ? this.compiler.ApplyTemplatesFunctions : sheet.ApplyImportsFunctions;
            if (!dictionary.TryGetValue(mode, out list2))
            {
                list2 = dictionary[mode] = new List<QilFunction>();
            }
            foreach (QilFunction function2 in list2)
            {
                if (this.FillupInvokeArgs(function2.Arguments, actualArgs, invokeArgs))
                {
                    item = function2;
                    break;
                }
            }
            if (item == null)
            {
                invokeArgs.Clear();
                QilList args = this.f.FormalParameterList();
                for (int i = 0; i < actualArgs.Count; i++)
                {
                    VarPar par = (VarPar) actualArgs[i];
                    invokeArgs.Add(par.Value);
                    QilParameter parameter = this.f.Parameter((i == 0) ? XmlQueryTypeFactory.NodeNotRtf : par.Value.XmlType);
                    parameter.Name = this.CloneName(par.Name);
                    args.Add((QilNode) parameter);
                    par.Value = parameter;
                }
                item = this.f.Function(args, this.f.False(), XmlQueryTypeFactory.NodeNotRtfS);
                string str = (mode.LocalName.Length == 0) ? string.Empty : (" mode=\"" + mode.QualifiedName + '"');
                item.DebugName = ((sheet == null) ? "<xsl:apply-templates" : "<xsl:apply-imports") + str + '>';
                list2.Add(item);
                this.functions.Add((QilNode) item);
                QilIterator expr = (QilIterator) args[0];
                QilTernary otherwise = this.f.BaseFactory.Conditional(this.f.IsType(expr, this.elementOrDocumentType), this.f.BaseFactory.Nop(this.f.BaseFactory.Unknown(XmlQueryTypeFactory.NodeNotRtfS)), this.f.Conditional(this.f.IsType(expr, this.textOrAttributeType), this.f.TextCtor(this.f.XPathNodeValue(expr)), this.f.Sequence()));
                this.matcherBuilder.CollectPatterns(sheet ?? this.compiler.PrincipalStylesheet, mode, sheet != null);
                item.Definition = this.matcherBuilder.BuildMatcher(expr, actualArgs, otherwise);
                QilIterator variable = this.f.For(this.f.Content(expr));
                QilNode binding = this.f.Filter(variable, this.f.IsType(variable, XmlQueryTypeFactory.Content));
                binding.XmlType = XmlQueryTypeFactory.ContentS;
                LoopFocus curLoop = this.curLoop;
                this.curLoop.SetFocus(this.f.For(binding));
                if ((none & XslFlags.Last) != XslFlags.None)
                {
                    this.curLoop.GetLast();
                }
                List<XslNode> list4 = new List<XslNode>(3);
                int num2 = 0;
                if ((none & XslFlags.Current) != XslFlags.None)
                {
                    list4.Add(actualArgs[num2++]);
                }
                if ((none & XslFlags.Position) != XslFlags.None)
                {
                    list4.Add(actualArgs[num2++]);
                }
                if ((none & XslFlags.Last) != XslFlags.None)
                {
                    list4.Add(actualArgs[num2++]);
                }
                actualArgs = list4;
                int num3 = 0;
                if ((none & XslFlags.Current) != XslFlags.None)
                {
                    ((VarPar) actualArgs[num3++]).Value = this.GetCurrentNode();
                }
                if ((none & XslFlags.Position) != XslFlags.None)
                {
                    ((VarPar) actualArgs[num3++]).Value = this.GetCurrentPosition();
                }
                if ((none & XslFlags.Last) != XslFlags.None)
                {
                    ((VarPar) actualArgs[num3++]).Value = this.GetLastPosition();
                }
                QilNode node2 = this.InvokeApplyFunction(null, mode, actualArgs);
                if (this.IsDebug)
                {
                    node2 = this.f.Sequence(this.InvokeOnCurrentNodeChanged(), node2);
                }
                QilLoop loop = this.curLoop.ConstructLoop(node2);
                this.curLoop = curLoop;
                ((QilUnary) otherwise.Center).Child = loop;
            }
            return this.f.Invoke(item, invokeArgs);
        }

        private QilNode InvokeOnCurrentNodeChanged() => 
            this.f.Loop(this.f.Let(this.f.InvokeOnCurrentNodeChanged(this.curLoop.GetCurrent())), this.f.Sequence());

        public static bool IsElementAvailable(XmlQualifiedName name)
        {
            if (name.Namespace != "http://www.w3.org/1999/XSL/Transform")
            {
                return false;
            }
            string str = name.Name;
            if ((((((str != "apply-imports") && (str != "apply-templates")) && ((str != "attribute") && (str != "call-template"))) && (((str != "choose") && (str != "comment")) && ((str != "copy") && (str != "copy-of")))) && ((((str != "element") && (str != "fallback")) && ((str != "for-each") && (str != "if"))) && (((str != "message") && (str != "number")) && ((str != "processing-instruction") && (str != "text"))))) && (str != "value-of"))
            {
                return (str == "variable");
            }
            return true;
        }

        public static bool IsFunctionAvailable(string localName, string nsUri)
        {
            if (!XPathBuilder.IsFunctionAvailable(localName, nsUri))
            {
                if (nsUri.Length == 0)
                {
                    return (FunctionTable.ContainsKey(localName) && (localName != "unparsed-entity-uri"));
                }
                if (nsUri == "urn:schemas-microsoft-com:xslt")
                {
                    if ((((localName != "node-set") && (localName != "format-date")) && ((localName != "format-time") && (localName != "local-name"))) && (((localName != "namespace-uri") && (localName != "number")) && (localName != "string-compare")))
                    {
                        return (localName == "utc");
                    }
                    return true;
                }
                if (nsUri != "http://exslt.org/common")
                {
                    return false;
                }
                if (localName != "node-set")
                {
                    return (localName == "object-type");
                }
            }
            return true;
        }

        private QilNode MatchCountPattern(QilNode countPattern, QilIterator testNode)
        {
            QilNode node2;
            if (countPattern != null)
            {
                return this.MatchPattern(countPattern, testNode);
            }
            QilNode currentNode = this.GetCurrentNode();
            XmlNodeKindFlags nodeKinds = currentNode.XmlType.NodeKinds;
            if ((nodeKinds & (nodeKinds - 1)) != XmlNodeKindFlags.None)
            {
                return this.f.InvokeIsSameNodeSort(testNode, currentNode);
            }
            switch (nodeKinds)
            {
                case XmlNodeKindFlags.Document:
                    return this.f.IsType(testNode, XmlQueryTypeFactory.Document);

                case XmlNodeKindFlags.Element:
                    node2 = this.f.IsType(testNode, XmlQueryTypeFactory.Element);
                    break;

                case XmlNodeKindFlags.Attribute:
                    node2 = this.f.IsType(testNode, XmlQueryTypeFactory.Attribute);
                    break;

                case XmlNodeKindFlags.Text:
                    return this.f.IsType(testNode, XmlQueryTypeFactory.Text);

                case XmlNodeKindFlags.Comment:
                    return this.f.IsType(testNode, XmlQueryTypeFactory.Comment);

                case XmlNodeKindFlags.PI:
                    return this.f.And(this.f.IsType(testNode, XmlQueryTypeFactory.PI), this.f.Eq(this.f.LocalNameOf(testNode), this.f.LocalNameOf(currentNode)));

                case XmlNodeKindFlags.Namespace:
                    return this.f.And(this.f.IsType(testNode, XmlQueryTypeFactory.Namespace), this.f.Eq(this.f.LocalNameOf(testNode), this.f.LocalNameOf(currentNode)));

                default:
                    return this.f.False();
            }
            return this.f.And(node2, this.f.And(this.f.Eq(this.f.LocalNameOf(testNode), this.f.LocalNameOf(currentNode)), this.f.Eq(this.f.NamespaceUriOf(testNode), this.f.NamespaceUriOf(this.GetCurrentNode()))));
        }

        private QilNode MatchPattern(QilNode pattern, QilIterator testNode)
        {
            QilList list;
            if (pattern.NodeType == QilNodeType.Error)
            {
                return pattern;
            }
            if (pattern.NodeType == QilNodeType.Sequence)
            {
                list = (QilList) pattern;
            }
            else
            {
                list = this.f.BaseFactory.Sequence();
                list.Add(pattern);
            }
            QilNode right = this.f.False();
            for (int i = list.Count - 1; 0 <= i; i--)
            {
                QilLoop loop = (QilLoop) list[i];
                right = this.f.Or(this.refReplacer.Replace(loop.Body, loop.Variable, testNode), right);
            }
            return right;
        }

        private QilNode PhantomKeyMatch() => 
            this.f.TypeAssert(this.f.Sequence(), XmlQueryTypeFactory.NodeNotRtfS);

        private QilNode PhantomXPathExpression() => 
            this.f.TypeAssert(this.f.Sequence(), XmlQueryTypeFactory.ItemS);

        private QilNode PlaceMarker(QilNode countPattern, QilNode fromPattern, bool multiple)
        {
            QilNode node4;
            QilNode node6;
            QilIterator iterator;
            QilIterator iterator2;
            QilNode node = countPattern?.DeepClone(this.f.BaseFactory);
            QilNode collection = this.f.Filter(iterator = this.f.For(this.f.AncestorOrSelf(this.GetCurrentNode())), this.MatchCountPattern(countPattern, iterator));
            if (multiple)
            {
                node4 = this.f.DocOrderDistinct(collection);
            }
            else
            {
                node4 = this.f.Filter(iterator = this.f.For(collection), this.f.Eq(this.f.PositionOf(iterator), this.f.Int32(1)));
            }
            if (fromPattern == null)
            {
                node6 = node4;
            }
            else
            {
                QilNode binding = this.f.Filter(iterator = this.f.For(this.f.AncestorOrSelf(this.GetCurrentNode())), this.MatchPattern(fromPattern, iterator));
                QilNode node5 = this.f.Filter(iterator = this.f.For(binding), this.f.Eq(this.f.PositionOf(iterator), this.f.Int32(1)));
                node6 = this.f.Loop(iterator = this.f.For(node5), this.f.Filter(iterator2 = this.f.For(node4), this.f.Before(iterator, iterator2)));
            }
            return this.f.Loop(iterator2 = this.f.For(node6), this.f.Add(this.f.Int32(1), this.f.Length(this.f.Filter(iterator = this.f.For(this.f.PrecedingSibling(iterator2)), this.MatchCountPattern(node, iterator)))));
        }

        private QilNode PlaceMarkerAny(QilNode countPattern, QilNode fromPattern)
        {
            QilNode node4;
            QilIterator iterator;
            QilIterator iterator3;
            if (fromPattern == null)
            {
                QilNode binding = this.f.NodeRange(this.f.Root(this.GetCurrentNode()), this.GetCurrentNode());
                node4 = this.f.Filter(iterator = this.f.For(binding), this.MatchCountPattern(countPattern, iterator));
            }
            else
            {
                QilIterator iterator2;
                QilNode node2 = this.f.Filter(iterator = this.f.For(this.f.Preceding(this.GetCurrentNode())), this.MatchPattern(fromPattern, iterator));
                QilNode node3 = this.f.Filter(iterator = this.f.For(node2), this.f.Eq(this.f.PositionOf(iterator), this.f.Int32(1)));
                node4 = this.f.Loop(iterator = this.f.For(node3), this.f.Filter(iterator2 = this.f.For(this.f.Filter(iterator3 = this.f.For(this.f.NodeRange(iterator, this.GetCurrentNode())), this.MatchCountPattern(countPattern, iterator3))), this.f.Not(this.f.Is(iterator, iterator2))));
            }
            return this.f.Loop(iterator3 = this.f.Let(this.f.Length(node4)), this.f.Conditional(this.f.Eq(iterator3, this.f.Int32(0)), this.f.Sequence(), iterator3));
        }

        private void PrecompileProtoTemplatesHeaders()
        {
            List<VarPar> list = null;
            Dictionary<VarPar, Template> dictionary = null;
            Dictionary<VarPar, QilFunction> dictionary2 = null;
            foreach (ProtoTemplate template in this.compiler.AllTemplates)
            {
                QilList args = this.f.FormalParameterList();
                XslFlags flags = !this.IsDebug ? template.Flags : XslFlags.FocusFilter;
                QilList nsList = this.EnterScope(template);
                if ((flags & XslFlags.Current) != XslFlags.None)
                {
                    args.Add((QilNode) this.CreateXslParam(this.CloneName(this.nameCurrent), XmlQueryTypeFactory.NodeNotRtf));
                }
                if ((flags & XslFlags.Position) != XslFlags.None)
                {
                    args.Add((QilNode) this.CreateXslParam(this.CloneName(this.namePosition), XmlQueryTypeFactory.DoubleX));
                }
                if ((flags & XslFlags.Last) != XslFlags.None)
                {
                    args.Add((QilNode) this.CreateXslParam(this.CloneName(this.nameLast), XmlQueryTypeFactory.DoubleX));
                }
                if (this.IsDebug && (nsList != null))
                {
                    QilParameter parameter = this.CreateXslParam(this.CloneName(this.nameNamespaces), XmlQueryTypeFactory.NamespaceS);
                    parameter.DefaultValue = this.GetNsVar(nsList);
                    args.Add((QilNode) parameter);
                }
                Template template2 = template as Template;
                if (template2 != null)
                {
                    this.funcFocus.StartFocus(args, flags);
                    for (int i = 0; i < template.Content.Count; i++)
                    {
                        XslNode node = template.Content[i];
                        if (node.NodeType != XslNodeType.Text)
                        {
                            if (node.NodeType != XslNodeType.Param)
                            {
                                break;
                            }
                            VarPar par = (VarPar) node;
                            this.EnterScope(par);
                            if (this.scope.IsLocalVariable(par.Name.LocalName, par.Name.NamespaceUri))
                            {
                                this.ReportError("Xslt_DupLocalVariable", new string[] { par.Name.QualifiedName });
                            }
                            QilParameter n = this.CreateXslParam(par.Name, this.ChooseBestType(par));
                            if (this.IsDebug)
                            {
                                n.Annotation = par;
                            }
                            else if ((par.DefValueFlags & XslFlags.HasCalls) == XslFlags.None)
                            {
                                n.DefaultValue = this.CompileVarParValue(par);
                            }
                            else
                            {
                                QilList list4 = this.f.FormalParameterList();
                                QilList list5 = this.f.ActualParameterList();
                                for (int j = 0; j < args.Count; j++)
                                {
                                    QilParameter parameter3 = this.f.Parameter(args[j].XmlType);
                                    parameter3.DebugName = ((QilParameter) args[j]).DebugName;
                                    parameter3.Name = this.CloneName(((QilParameter) args[j]).Name);
                                    SetLineInfo(parameter3, args[j].SourceLine);
                                    list4.Add((QilNode) parameter3);
                                    list5.Add(args[j]);
                                }
                                par.Flags |= template2.Flags & XslFlags.FocusFilter;
                                QilFunction func = this.f.Function(list4, ((par.DefValueFlags & XslFlags.SideEffects) == XslFlags.None) ? this.f.False() : this.f.True(), this.ChooseBestType(par));
                                func.SourceLine = SourceLineInfo.NoSource;
                                func.DebugName = "<xsl:param name=\"" + par.Name.QualifiedName + "\">";
                                n.DefaultValue = this.f.Invoke(func, list5);
                                if (list == null)
                                {
                                    list = new List<VarPar>();
                                    dictionary = new Dictionary<VarPar, Template>();
                                    dictionary2 = new Dictionary<VarPar, QilFunction>();
                                }
                                list.Add(par);
                                dictionary.Add(par, template2);
                                dictionary2.Add(par, func);
                            }
                            SetLineInfo(n, par.SourceLine);
                            this.ExitScope();
                            this.scope.AddVariable(par.Name, n);
                            args.Add((QilNode) n);
                        }
                    }
                    this.funcFocus.StopFocus();
                }
                this.ExitScope();
                template.Function = this.f.Function(args, ((template.Flags & XslFlags.SideEffects) == XslFlags.None) ? this.f.False() : this.f.True(), (template is AttributeSet) ? XmlQueryTypeFactory.AttributeS : XmlQueryTypeFactory.NodeNotRtfS);
                template.Function.DebugName = template.GetDebugName();
                SetLineInfo(template.Function, template.SourceLine ?? SourceLineInfo.NoSource);
                this.functions.Add((QilNode) template.Function);
            }
            if (list != null)
            {
                foreach (VarPar par2 in list)
                {
                    Template template3 = dictionary[par2];
                    QilFunction function2 = dictionary2[par2];
                    this.funcFocus.StartFocus(function2.Arguments, par2.Flags);
                    this.EnterScope(template3);
                    this.EnterScope(par2);
                    foreach (QilParameter parameter4 in function2.Arguments)
                    {
                        this.scope.AddVariable(parameter4.Name, parameter4);
                    }
                    function2.Definition = this.CompileVarParValue(par2);
                    SetLineInfo(function2.Definition, par2.SourceLine);
                    this.ExitScope();
                    this.ExitScope();
                    this.funcFocus.StopFocus();
                    this.functions.Add((QilNode) function2);
                }
            }
        }

        public void ReportError(string res, params string[] args)
        {
            this.compiler.ReportError(this.lastScope.SourceLine, res, args);
        }

        private void ReportErrorInXPath(XslLoadException e)
        {
            XPathCompileException exception = e as XPathCompileException;
            string str = (exception != null) ? exception.FormatDetailedMessage() : e.Message;
            this.compiler.ReportError(this.lastScope.SourceLine, "Xml_UserException", new string[] { str });
        }

        public void ReportWarning(string res, params string[] args)
        {
            this.compiler.ReportWarning(this.lastScope.SourceLine, res, args);
        }

        private string ResolvePrefix(bool ignoreDefaultNs, string prefix)
        {
            if (ignoreDefaultNs && (prefix.Length == 0))
            {
                return string.Empty;
            }
            string str = this.scope.LookupNamespace(prefix);
            if (str != null)
            {
                return str;
            }
            if (prefix.Length == 0)
            {
                return string.Empty;
            }
            this.ReportError("Xslt_InvalidPrefix", new string[] { prefix });
            return this.compiler.CreatePhantomNamespace();
        }

        private string ResolvePrefixThrow(bool ignoreDefaultNs, string prefix)
        {
            if (!ignoreDefaultNs || (prefix.Length != 0))
            {
                string str = this.scope.LookupNamespace(prefix);
                if (str != null)
                {
                    return str;
                }
                if (prefix.Length != 0)
                {
                    throw new XslLoadException("Xslt_InvalidPrefix", new string[] { prefix });
                }
            }
            return string.Empty;
        }

        private QilNode ResolveQNameDynamic(bool ignoreDefaultNs, QilNode qilName)
        {
            QilList ns = this.f.BaseFactory.Sequence();
            if (ignoreDefaultNs)
            {
                ns.Add(this.f.NamespaceDecl(this.f.String(string.Empty), this.f.String(string.Empty)));
            }
            foreach (CompilerScopeManager<QilIterator>.ScopeRecord record in this.scope)
            {
                string ncName = record.ncName;
                string nsUri = record.nsUri;
                if (!ignoreDefaultNs || (ncName.Length != 0))
                {
                    ns.Add(this.f.NamespaceDecl(this.f.String(ncName), this.f.String(nsUri)));
                }
            }
            return this.f.StrParseQName(qilName, ns);
        }

        private XmlQualifiedName ResolveQNameThrow(bool ignoreDefaultNs, QilNode qilName)
        {
            string str2;
            string str3;
            string qname = (string) ((QilLiteral) qilName);
            this.compiler.ParseQName(qname, out str2, out str3, new ThrowErrorHelper());
            return new XmlQualifiedName(str3, this.ResolvePrefixThrow(ignoreDefaultNs, str2));
        }

        private QilNode SetDebugNs(QilNode n, QilList nsList)
        {
            if ((n != null) && (nsList != null))
            {
                QilNode nsVar = this.GetNsVar(nsList);
                if (nsVar.XmlType.Cardinality == XmlQueryCardinality.One)
                {
                    nsVar = this.f.TypeAssert(nsVar, XmlQueryTypeFactory.NamespaceS);
                }
                n = this.AddDebugVariable(this.CloneName(this.nameNamespaces), nsVar, n);
            }
            return n;
        }

        private void SetEnvironmentFlags(bool allowVariables, bool allowCurrent, bool allowKey)
        {
            this.allowVariables = allowVariables;
            this.allowCurrent = allowCurrent;
            this.allowKey = allowKey;
        }

        private static QilNode SetLineInfo(QilNode n, ISourceLineInfo lineInfo)
        {
            if (((lineInfo != null) && (0 < lineInfo.StartLine)) && (lineInfo.StartLine <= lineInfo.EndLine))
            {
                n.SourceLine = lineInfo;
            }
            return n;
        }

        private void SetLineInfoCheck(QilNode n, ISourceLineInfo lineInfo)
        {
            if (n.SourceLine == null)
            {
                SetLineInfo(n, lineInfo);
            }
        }

        QilNode IFocus.GetCurrent() => 
            this.GetCurrentNode();

        QilNode IFocus.GetLast() => 
            this.GetLastPosition();

        QilNode IFocus.GetPosition() => 
            this.GetCurrentPosition();

        QilNode IXPathEnvironment.ResolveFunction(string prefix, string name, IList<QilNode> args, IFocus env)
        {
            if (prefix.Length == 0)
            {
                XPathBuilder.FunctionInfo<FuncId> info;
                if (!FunctionTable.TryGetValue(name, out info))
                {
                    throw new XslLoadException("Xslt_UnknownXsltFunction", new string[] { System.Xml.Xsl.Xslt.Compiler.ConstructQName(prefix, name) });
                }
                info.CastArguments(args, name, this.f);
                switch (info.id)
                {
                    case FuncId.Current:
                        if (!this.allowCurrent)
                        {
                            throw new XslLoadException("Xslt_CurrentNotAllowed", new string[0]);
                        }
                        return ((IFocus) this).GetCurrent();

                    case FuncId.Document:
                        return this.CompileFnDocument(args[0], (args.Count > 1) ? args[1] : null);

                    case FuncId.Key:
                        if (!this.allowKey)
                        {
                            throw new XslLoadException("Xslt_KeyNotAllowed", new string[0]);
                        }
                        return this.CompileFnKey(args[0], args[1], env);

                    case FuncId.FormatNumber:
                        return this.CompileFormatNumber(args[0], args[1], (args.Count > 2) ? args[2] : null);

                    case FuncId.UnparsedEntityUri:
                        return this.CompileUnparsedEntityUri(args[0]);

                    case FuncId.GenerateId:
                        return this.CompileGenerateId((args.Count > 0) ? args[0] : env.GetCurrent());

                    case FuncId.SystemProperty:
                        return this.CompileSystemProperty(args[0]);

                    case FuncId.ElementAvailable:
                        return this.CompileElementAvailable(args[0]);

                    case FuncId.FunctionAvailable:
                        return this.CompileFunctionAvailable(args[0]);
                }
                return null;
            }
            string ns = this.ResolvePrefixThrow(true, prefix);
            switch (ns)
            {
                case "urn:schemas-microsoft-com:xslt":
                    if (name == "node-set")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 1, name, args.Count);
                        return this.CompileMsNodeSet(args[0]);
                    }
                    if (name == "string-compare")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(2, 4, name, args.Count);
                        return this.f.InvokeMsStringCompare(this.f.ConvertToString(args[0]), this.f.ConvertToString(args[1]), (2 < args.Count) ? this.f.ConvertToString(args[2]) : this.f.String(string.Empty), (3 < args.Count) ? this.f.ConvertToString(args[3]) : this.f.String(string.Empty));
                    }
                    if (name == "utc")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 1, name, args.Count);
                        return this.f.InvokeMsUtc(this.f.ConvertToString(args[0]));
                    }
                    if ((name == "format-date") || (name == "format-time"))
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 3, name, args.Count);
                        return this.f.InvokeMsFormatDateTime(this.f.ConvertToString(args[0]), (1 < args.Count) ? this.f.ConvertToString(args[1]) : this.f.String(string.Empty), (2 < args.Count) ? this.f.ConvertToString(args[2]) : this.f.String(string.Empty), this.f.Boolean(name == "format-date"));
                    }
                    if (name == "local-name")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 1, name, args.Count);
                        return this.f.InvokeMsLocalName(this.f.ConvertToString(args[0]));
                    }
                    if (name == "namespace-uri")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 1, name, args.Count);
                        return this.f.InvokeMsNamespaceUri(this.f.ConvertToString(args[0]), env.GetCurrent());
                    }
                    if (name == "number")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 1, name, args.Count);
                        return this.f.InvokeMsNumber(args[0]);
                    }
                    break;

                case "http://exslt.org/common":
                    if (name == "node-set")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 1, name, args.Count);
                        return this.CompileMsNodeSet(args[0]);
                    }
                    if (name == "object-type")
                    {
                        XPathBuilder.FunctionInfo<FuncId>.CheckArity(1, 1, name, args.Count);
                        return this.EXslObjectType(args[0]);
                    }
                    break;
            }
            for (int i = 0; i < args.Count; i++)
            {
                args[i] = this.f.SafeDocOrderDistinct(args[i]);
            }
            if (this.compiler.Settings.EnableScript)
            {
                XmlExtensionFunction scrFunc = this.compiler.Scripts.ResolveFunction(name, ns, args.Count, this);
                if (scrFunc != null)
                {
                    return this.GenerateScriptCall(this.f.QName(name, ns, prefix), scrFunc, args);
                }
            }
            else if (this.compiler.Scripts.ScriptClasses.ContainsKey(ns))
            {
                this.ReportWarning("Xslt_ScriptsProhibited", new string[0]);
                return this.f.Error(this.lastScope.SourceLine, "Xslt_ScriptsProhibited", new string[0]);
            }
            return this.f.XsltInvokeLateBound(this.f.QName(name, ns, prefix), args);
        }

        string IXPathEnvironment.ResolvePrefix(string prefix) => 
            this.ResolvePrefixThrow(true, prefix);

        QilNode IXPathEnvironment.ResolveVariable(string prefix, string name)
        {
            if (!this.allowVariables)
            {
                throw new XslLoadException("Xslt_VariablesNotAllowed", new string[0]);
            }
            string uri = this.ResolvePrefixThrow(true, prefix);
            QilNode variable = this.scope.LookupVariable(name, uri);
            if (variable == null)
            {
                throw new XslLoadException("Xslt_InvalidVariable", new string[] { System.Xml.Xsl.Xslt.Compiler.ConstructQName(prefix, name) });
            }
            XmlQueryType xmlType = variable.XmlType;
            if ((((variable.NodeType == QilNodeType.Parameter) && xmlType.IsNode) && (xmlType.IsNotRtf && xmlType.MaybeMany)) && !xmlType.IsDod)
            {
                variable = this.f.TypeAssert(variable, XmlQueryTypeFactory.NodeDodS);
            }
            return variable;
        }

        [Conditional("DEBUG")]
        private void VerifyXPathQName(QilName qname)
        {
        }

        private QilNode WrapLoopBody(ISourceLineInfo before, QilNode expr, ISourceLineInfo after)
        {
            if (this.IsDebug)
            {
                return this.f.Sequence(new QilNode[] { SetLineInfo(this.InvokeOnCurrentNodeChanged(), before), expr, SetLineInfo(this.f.Nop(this.f.Sequence()), after) });
            }
            return expr;
        }

        private bool EvaluateFuncCalls =>
            !this.IsDebug;

        private bool InferXPathTypes =>
            !this.IsDebug;

        private bool IsDebug =>
            this.compiler.IsDebug;

        XPathQilFactory IXPathEnvironment.Factory =>
            this.f;

        public enum FuncId
        {
            Current,
            Document,
            Key,
            FormatNumber,
            UnparsedEntityUri,
            GenerateId,
            SystemProperty,
            ElementAvailable,
            FunctionAvailable
        }

        [StructLayout(LayoutKind.Sequential, Size=1)]
        private struct ThrowErrorHelper : IErrorHelper
        {
            public void ReportError(string res, params string[] args)
            {
                throw new XslLoadException("Xml_UserException", new string[] { res });
            }

            public void ReportWarning(string res, params string[] args)
            {
            }
        }

        private class VariableHelper
        {
            private XPathQilFactory f;
            private Stack<QilIterator> vars = new Stack<QilIterator>();

            public VariableHelper(XPathQilFactory f)
            {
                this.f = f;
            }

            public void AddVariable(QilIterator let)
            {
                this.vars.Push(let);
            }

            [Conditional("DEBUG")]
            public void CheckEmpty()
            {
            }

            public QilNode FinishVariables(QilNode node, int varScope)
            {
                int num = this.vars.Count - varScope;
                while (num-- != 0)
                {
                    node = this.f.Loop(this.vars.Pop(), node);
                }
                return node;
            }

            public int StartVariables() => 
                this.vars.Count;
        }
    }
}

