namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;
    using System.Xml.Xsl.Runtime;
    using System.Xml.Xsl.XPath;

    internal class XslAstAnalyzer : XslVisitor<XslFlags>
    {
        private Dictionary<ModeName, VarPar> applyTemplatesParams = new Dictionary<ModeName, VarPar>();
        private Compiler compiler;
        private ProtoTemplate currentTemplate;
        private Graph<VarPar> dataFlow = new Graph<VarPar>();
        private Dictionary<Template, Stylesheet> dependsOnApplyImports = new Dictionary<Template, Stylesheet>();
        private Dictionary<QilName, List<ProtoTemplate>> dependsOnMode = new Dictionary<QilName, List<ProtoTemplate>>();
        private Graph<ProtoTemplate> focusDonors = new Graph<ProtoTemplate>();
        private int forEachDepth;
        private CompilerScopeManager<VarPar> scope;
        private Graph<ProtoTemplate> sideEffectDonors = new Graph<ProtoTemplate>();
        private VarPar typeDonor;
        private XPathAnalyzer xpathAnalyzer;

        private void AddApplyTemplatesEdge(QilName mode, ProtoTemplate dependentTemplate)
        {
            List<ProtoTemplate> list;
            if (!this.dependsOnMode.TryGetValue(mode, out list))
            {
                list = new List<ProtoTemplate>();
                this.dependsOnMode.Add(mode, list);
            }
            else if (list[list.Count - 1] == dependentTemplate)
            {
                return;
            }
            list.Add(dependentTemplate);
        }

        private void AddImportDependencies(Stylesheet sheet, Template focusDonor)
        {
            foreach (Template template in sheet.Templates)
            {
                if (template.Mode.Equals(focusDonor.Mode))
                {
                    this.focusDonors.AddEdge(template, focusDonor);
                }
            }
            foreach (Stylesheet stylesheet in sheet.Imports)
            {
                this.AddImportDependencies(stylesheet, focusDonor);
            }
        }

        public XslFlags Analyze(Compiler compiler)
        {
            this.compiler = compiler;
            this.scope = new CompilerScopeManager<VarPar>();
            this.xpathAnalyzer = new XPathAnalyzer(compiler, this.scope);
            foreach (VarPar par in compiler.ExternalPars)
            {
                this.scope.AddVariable(par.Name, par);
            }
            foreach (VarPar par2 in compiler.GlobalVars)
            {
                this.scope.AddVariable(par2.Name, par2);
            }
            foreach (VarPar par3 in compiler.ExternalPars)
            {
                this.Visit(par3);
                par3.Flags |= XslFlags.AnyType;
            }
            foreach (VarPar par4 in compiler.GlobalVars)
            {
                this.Visit(par4);
            }
            XslFlags none = XslFlags.None;
            foreach (ProtoTemplate template in compiler.AllTemplates)
            {
                this.currentTemplate = template;
                none |= (XslFlags) this.Visit(template);
            }
            foreach (ProtoTemplate template2 in compiler.AllTemplates)
            {
                foreach (XslNode node in template2.Content)
                {
                    if (node.NodeType != XslNodeType.Text)
                    {
                        if (node.NodeType != XslNodeType.Param)
                        {
                            break;
                        }
                        VarPar par5 = (VarPar) node;
                        if ((par5.Flags & XslFlags.MayBeDefault) != XslFlags.None)
                        {
                            par5.Flags |= par5.DefValueFlags;
                        }
                    }
                }
            }
            for (int i = 0x20; i != 0; i = i >> 1)
            {
                this.dataFlow.PropagateFlag((XslFlags) i);
            }
            this.dataFlow = null;
            foreach (KeyValuePair<Template, Stylesheet> pair in this.dependsOnApplyImports)
            {
                this.AddImportDependencies(compiler.PrincipalStylesheet, pair.Key);
            }
            this.dependsOnApplyImports = null;
            if ((none & XslFlags.Current) != XslFlags.None)
            {
                this.focusDonors.PropagateFlag(XslFlags.Current);
            }
            if ((none & XslFlags.Position) != XslFlags.None)
            {
                this.focusDonors.PropagateFlag(XslFlags.Position);
            }
            if ((none & XslFlags.Last) != XslFlags.None)
            {
                this.focusDonors.PropagateFlag(XslFlags.Last);
            }
            if ((none & XslFlags.SideEffects) != XslFlags.None)
            {
                this.PropagateSideEffectsFlag();
            }
            this.focusDonors = null;
            this.sideEffectDonors = null;
            this.dependsOnMode = null;
            this.FillModeFlags(compiler.PrincipalStylesheet);
            this.TraceResults();
            return none;
        }

        private void DepthFirstSearch(ProtoTemplate t)
        {
            List<ProtoTemplate> list;
            t.Flags |= XslFlags.Stop | XslFlags.SideEffects;
            foreach (ProtoTemplate template in this.focusDonors.GetAdjList(t))
            {
                if ((template.Flags & XslFlags.Stop) == XslFlags.None)
                {
                    this.DepthFirstSearch(template);
                }
            }
            foreach (ProtoTemplate template2 in this.sideEffectDonors.GetAdjList(t))
            {
                if ((template2.Flags & XslFlags.Stop) == XslFlags.None)
                {
                    this.DepthFirstSearch(template2);
                }
            }
            Template template3 = t as Template;
            if ((template3 != null) && this.dependsOnMode.TryGetValue(template3.Mode, out list))
            {
                this.dependsOnMode.Remove(template3.Mode);
                foreach (ProtoTemplate template4 in list)
                {
                    if ((template4.Flags & XslFlags.Stop) == XslFlags.None)
                    {
                        this.DepthFirstSearch(template4);
                    }
                }
            }
        }

        private void FillModeFlags(Stylesheet sheet)
        {
            foreach (Template template in sheet.Templates)
            {
                XslFlags flags = template.Flags & XslFlags.FocusFilter;
                if (flags != XslFlags.None)
                {
                    XslFlags none;
                    if (!this.compiler.ModeFlags.TryGetValue(template.Mode, out none))
                    {
                        none = XslFlags.None;
                    }
                    this.compiler.ModeFlags[template.Mode] = none | flags;
                }
            }
            foreach (Stylesheet stylesheet in sheet.Imports)
            {
                this.FillModeFlags(stylesheet);
            }
        }

        private XslFlags ProcessAvt(string avt) => 
            (this.xpathAnalyzer.AnalyzeAvt(avt) & ~XslFlags.AnyType);

        private XslFlags ProcessExpr(string expr) => 
            (this.xpathAnalyzer.Analyze(expr) & ~XslFlags.AnyType);

        private XslFlags ProcessPattern(string pattern) => 
            ((this.xpathAnalyzer.Analyze(pattern) & ~XslFlags.AnyType) & ~XslFlags.FocusFilter);

        private XslFlags ProcessVarPar(VarPar node)
        {
            XslFlags flags;
            if (node.Select != null)
            {
                if (node.Content.Count != 0)
                {
                    flags = (this.xpathAnalyzer.Analyze(node.Select) | ((XslFlags) this.VisitChildren(node))) | XslFlags.AnyType;
                    this.typeDonor = null;
                    return flags;
                }
                flags = this.xpathAnalyzer.Analyze(node.Select);
                this.typeDonor = this.xpathAnalyzer.TypeDonor;
                if ((this.typeDonor != null) && (node.NodeType != XslNodeType.WithParam))
                {
                    this.dataFlow.AddEdge(this.typeDonor, node);
                }
                return flags;
            }
            if (node.Content.Count != 0)
            {
                flags = XslFlags.Rtf | ((XslFlags) this.VisitChildren(node));
                this.typeDonor = null;
                return flags;
            }
            flags = XslFlags.String;
            this.typeDonor = null;
            return flags;
        }

        private void PropagateSideEffectsFlag()
        {
            foreach (ProtoTemplate template in this.focusDonors.Keys)
            {
                template.Flags &= ~XslFlags.Stop;
            }
            foreach (ProtoTemplate template2 in this.sideEffectDonors.Keys)
            {
                template2.Flags &= ~XslFlags.Stop;
            }
            foreach (ProtoTemplate template3 in this.focusDonors.Keys)
            {
                if (((template3.Flags & XslFlags.Stop) == XslFlags.None) && ((template3.Flags & XslFlags.SideEffects) != XslFlags.None))
                {
                    this.DepthFirstSearch(template3);
                }
            }
            foreach (ProtoTemplate template4 in this.sideEffectDonors.Keys)
            {
                if (((template4.Flags & XslFlags.Stop) == XslFlags.None) && ((template4.Flags & XslFlags.SideEffects) != XslFlags.None))
                {
                    this.DepthFirstSearch(template4);
                }
            }
        }

        private void TraceResults()
        {
        }

        protected override XslFlags Visit(XslNode node)
        {
            this.scope.PushScope();
            for (NsDecl decl = node.Namespaces; decl != null; decl = decl.Prev)
            {
                this.scope.AddNamespace(decl.Prefix, decl.NsUri);
            }
            XslFlags flags = base.Visit(node);
            this.scope.PopScope();
            if ((this.currentTemplate != null) && ((node.NodeType == XslNodeType.Variable) || (node.NodeType == XslNodeType.Param)))
            {
                this.scope.AddVariable(node.Name, (VarPar) node);
            }
            return flags;
        }

        protected override XslFlags VisitApplyImports(XslNode node)
        {
            this.dependsOnApplyImports[(Template) this.currentTemplate] = (Stylesheet) node.Arg;
            return (XslFlags.HasCalls | XslFlags.Current | XslFlags.Rtf);
        }

        protected override XslFlags VisitApplyTemplates(XslNode node)
        {
            XslFlags flags = this.ProcessExpr(node.Select);
            foreach (XslNode node2 in node.Content)
            {
                flags |= (XslFlags) this.Visit(node2);
                if (node2.NodeType == XslNodeType.WithParam)
                {
                    VarPar par;
                    ModeName key = new ModeName(node.Name, node2.Name);
                    if (!this.applyTemplatesParams.TryGetValue(key, out par))
                    {
                        par = this.applyTemplatesParams[key] = AstFactory.WithParam(node2.Name);
                    }
                    if (this.typeDonor != null)
                    {
                        this.dataFlow.AddEdge(this.typeDonor, par);
                    }
                    else
                    {
                        par.Flags |= node2.Flags & XslFlags.AnyType;
                    }
                }
            }
            if (this.currentTemplate != null)
            {
                this.AddApplyTemplatesEdge(node.Name, this.currentTemplate);
            }
            return ((XslFlags.HasCalls | XslFlags.Rtf) | flags);
        }

        protected override XslFlags VisitAttribute(NodeCtor node) => 
            (((XslFlags.Rtf | this.ProcessAvt(node.NameAvt)) | this.ProcessAvt(node.NsAvt)) | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitAttributeSet(AttributeSet node)
        {
            node.Flags = this.VisitChildren(node);
            return node.Flags;
        }

        protected override XslFlags VisitCallTemplate(XslNode node)
        {
            Template template;
            XslFlags none = XslFlags.None;
            if (this.compiler.NamedTemplates.TryGetValue(node.Name, out template) && (this.currentTemplate != null))
            {
                if (this.forEachDepth == 0)
                {
                    this.focusDonors.AddEdge(template, this.currentTemplate);
                }
                else
                {
                    this.sideEffectDonors.AddEdge(template, this.currentTemplate);
                }
            }
            VarPar[] parArray = new VarPar[node.Content.Count];
            int index = 0;
            foreach (XslNode node2 in node.Content)
            {
                none |= (XslFlags) this.Visit(node2);
                parArray[index++] = this.typeDonor;
            }
            if (template != null)
            {
                foreach (XslNode node3 in template.Content)
                {
                    if (node3.NodeType == XslNodeType.Text)
                    {
                        continue;
                    }
                    if (node3.NodeType != XslNodeType.Param)
                    {
                        break;
                    }
                    VarPar par = (VarPar) node3;
                    VarPar par2 = null;
                    index = 0;
                    foreach (XslNode node4 in node.Content)
                    {
                        if (node4.Name.Equals(par.Name))
                        {
                            par2 = (VarPar) node4;
                            this.typeDonor = parArray[index];
                            break;
                        }
                        index++;
                    }
                    if (par2 != null)
                    {
                        if (this.typeDonor != null)
                        {
                            this.dataFlow.AddEdge(this.typeDonor, par);
                        }
                        else
                        {
                            par.Flags |= par2.Flags & XslFlags.AnyType;
                        }
                    }
                    else
                    {
                        par.Flags |= XslFlags.MayBeDefault;
                    }
                }
            }
            return ((XslFlags.HasCalls | XslFlags.Rtf) | none);
        }

        protected override XslFlags VisitChildren(XslNode node)
        {
            XslFlags none = XslFlags.None;
            foreach (XslNode node2 in node.Content)
            {
                none |= (XslFlags) this.Visit(node2);
            }
            return none;
        }

        protected override XslFlags VisitComment(XslNode node) => 
            (XslFlags.Rtf | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitCopy(XslNode node) => 
            ((XslFlags.Current | XslFlags.Rtf) | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitCopyOf(XslNode node) => 
            (XslFlags.Rtf | this.ProcessExpr(node.Select));

        protected override XslFlags VisitElement(NodeCtor node) => 
            (((XslFlags.Rtf | this.ProcessAvt(node.NameAvt)) | this.ProcessAvt(node.NsAvt)) | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitError(XslNode node) => 
            ((((XslFlags) this.VisitChildren(node)) & ~XslFlags.AnyType) | XslFlags.SideEffects);

        protected override XslFlags VisitForEach(XslNode node)
        {
            XslFlags flags = this.ProcessExpr(node.Select);
            this.forEachDepth++;
            foreach (XslNode node2 in node.Content)
            {
                if (node2.NodeType == XslNodeType.Sort)
                {
                    flags |= (XslFlags) this.Visit(node2);
                }
                else
                {
                    flags |= ((XslFlags) this.Visit(node2)) & ~XslFlags.FocusFilter;
                }
            }
            this.forEachDepth--;
            return flags;
        }

        protected override XslFlags VisitIf(XslNode node) => 
            (this.ProcessExpr(node.Select) | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitLiteralAttribute(XslNode node) => 
            ((XslFlags.Rtf | this.ProcessAvt(node.Select)) | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitLiteralElement(XslNode node) => 
            (XslFlags.Rtf | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitMessage(XslNode node) => 
            ((((XslFlags) this.VisitChildren(node)) & ~XslFlags.AnyType) | XslFlags.SideEffects);

        protected override XslFlags VisitNumber(System.Xml.Xsl.Xslt.Number node) => 
            ((((((((XslFlags.Rtf | this.ProcessPattern(node.Count)) | this.ProcessPattern(node.From)) | ((node.Value != null) ? this.ProcessExpr(node.Value) : XslFlags.Current)) | this.ProcessAvt(node.Format)) | this.ProcessAvt(node.Lang)) | this.ProcessAvt(node.LetterValue)) | this.ProcessAvt(node.GroupingSeparator)) | this.ProcessAvt(node.GroupingSize));

        protected override XslFlags VisitParam(VarPar node)
        {
            Template currentTemplate = this.currentTemplate as Template;
            if ((currentTemplate != null) && (currentTemplate.Match != null))
            {
                VarPar par;
                node.Flags |= XslFlags.MayBeDefault;
                ModeName key = new ModeName(currentTemplate.Mode, node.Name);
                if (!this.applyTemplatesParams.TryGetValue(key, out par))
                {
                    par = this.applyTemplatesParams[key] = AstFactory.WithParam(node.Name);
                }
                this.dataFlow.AddEdge(par, node);
            }
            node.DefValueFlags = this.ProcessVarPar(node);
            return (node.DefValueFlags & ~XslFlags.AnyType);
        }

        protected override XslFlags VisitPI(XslNode node) => 
            ((XslFlags.Rtf | this.ProcessAvt(node.Select)) | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitSort(Sort node) => 
            (((((this.ProcessExpr(node.Select) & ~XslFlags.FocusFilter) | this.ProcessAvt(node.Lang)) | this.ProcessAvt(node.DataType)) | this.ProcessAvt(node.Order)) | this.ProcessAvt(node.CaseOrder));

        protected override XslFlags VisitTemplate(Template node)
        {
            node.Flags = this.VisitChildren(node);
            return node.Flags;
        }

        protected override XslFlags VisitText(Text node) => 
            (XslFlags.Rtf | ((XslFlags) this.VisitChildren(node)));

        protected override XslFlags VisitUseAttributeSet(XslNode node)
        {
            AttributeSet set;
            if (this.compiler.AttributeSets.TryGetValue(node.Name, out set) && (this.currentTemplate != null))
            {
                if (this.forEachDepth == 0)
                {
                    this.focusDonors.AddEdge(set, this.currentTemplate);
                }
                else
                {
                    this.sideEffectDonors.AddEdge(set, this.currentTemplate);
                }
            }
            return (XslFlags.HasCalls | XslFlags.Rtf);
        }

        protected override XslFlags VisitValueOf(XslNode node) => 
            (XslFlags.Rtf | this.ProcessExpr(node.Select));

        protected override XslFlags VisitValueOfDoe(XslNode node) => 
            (XslFlags.Rtf | this.ProcessExpr(node.Select));

        protected override XslFlags VisitVariable(VarPar node)
        {
            node.Flags = this.ProcessVarPar(node);
            return (node.Flags & ~XslFlags.AnyType);
        }

        protected override XslFlags VisitWithParam(VarPar node)
        {
            node.Flags = this.ProcessVarPar(node);
            return (node.Flags & ~XslFlags.AnyType);
        }

        internal class Graph<V> : Dictionary<V, List<V>> where V: XslNode
        {
            private static IList<V> empty;

            static Graph()
            {
                XslAstAnalyzer.Graph<V>.empty = new List<V>().AsReadOnly();
            }

            public void AddEdge(V v1, V v2)
            {
                if (v1 != v2)
                {
                    List<V> list;
                    if (!base.TryGetValue(v1, out list) || (list == null))
                    {
                        list = base[v1] = new List<V>();
                    }
                    list.Add(v2);
                    if (!base.TryGetValue(v2, out list))
                    {
                        base[v2] = null;
                    }
                }
            }

            private void DepthFirstSearch(V v, XslFlags flag)
            {
                V local1 = v;
                local1.Flags |= flag | XslFlags.Stop;
                foreach (V local in this.GetAdjList(v))
                {
                    if ((local.Flags & XslFlags.Stop) == XslFlags.None)
                    {
                        this.DepthFirstSearch(local, flag);
                    }
                }
            }

            public IEnumerable<V> GetAdjList(V v)
            {
                List<V> list;
                if (base.TryGetValue(v, out list) && (list != null))
                {
                    return list;
                }
                return XslAstAnalyzer.Graph<V>.empty;
            }

            public void PropagateFlag(XslFlags flag)
            {
                foreach (V local in base.Keys)
                {
                    V local1 = local;
                    local1.Flags &= ~XslFlags.Stop;
                }
                foreach (V local2 in base.Keys)
                {
                    if (((local2.Flags & XslFlags.Stop) == XslFlags.None) && ((local2.Flags & flag) != XslFlags.None))
                    {
                        this.DepthFirstSearch(local2, flag);
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ModeName
        {
            public QilName Mode;
            public QilName Name;
            public ModeName(QilName mode, QilName name)
            {
                this.Mode = mode;
                this.Name = name;
            }

            public override int GetHashCode() => 
                (this.Mode.GetHashCode() ^ this.Name.GetHashCode());
        }

        [StructLayout(LayoutKind.Sequential, Size=1)]
        internal struct NullErrorHelper : IErrorHelper
        {
            public void ReportError(string res, params string[] args)
            {
            }

            public void ReportWarning(string res, params string[] args)
            {
            }
        }

        internal class XPathAnalyzer : IXPathBuilder<XslFlags>
        {
            private Compiler compiler;
            private static XslFlags[] OperatorType = new XslFlags[] { XslFlags.AnyType, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Number, XslFlags.Number, XslFlags.Number, XslFlags.Number, XslFlags.Number, XslFlags.Number, XslFlags.Nodeset };
            private CompilerScopeManager<VarPar> scope;
            private VarPar typeDonor;
            private static XslFlags[] XPathFunctionFlags = new XslFlags[] { 
                (XslFlags.Last | XslFlags.Number), (XslFlags.Position | XslFlags.Number), XslFlags.Number, XslFlags.String, XslFlags.String, XslFlags.String, XslFlags.String, XslFlags.Number, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, XslFlags.Boolean, (XslFlags.Current | XslFlags.Nodeset), XslFlags.String, XslFlags.Boolean, XslFlags.Boolean,
                XslFlags.String, XslFlags.String, XslFlags.String, XslFlags.Number, XslFlags.String, XslFlags.String, (XslFlags.Current | XslFlags.Boolean), XslFlags.Number, XslFlags.Number, XslFlags.Number, XslFlags.Number
            };
            private XPathParser<XslFlags> xpathParser = new XPathParser<XslFlags>();
            private bool xsltCurrentNeeded;
            private static XslFlags[] XsltFunctionFlags = new XslFlags[] { XslFlags.Node, XslFlags.Nodeset, (XslFlags.Current | XslFlags.Nodeset), XslFlags.String, XslFlags.String, XslFlags.String, (XslFlags.Number | XslFlags.String), XslFlags.Boolean, XslFlags.Boolean };

            public XPathAnalyzer(Compiler compiler, CompilerScopeManager<VarPar> scope)
            {
                this.compiler = compiler;
                this.scope = scope;
            }

            public XslFlags Analyze(string xpathExpr)
            {
                this.typeDonor = null;
                if (xpathExpr == null)
                {
                    return XslFlags.None;
                }
                try
                {
                    this.xsltCurrentNeeded = false;
                    XPathScanner scanner = new XPathScanner(xpathExpr);
                    XslFlags flags = this.xpathParser.Parse(scanner, this, LexKind.Eof);
                    if (this.xsltCurrentNeeded)
                    {
                        flags |= XslFlags.Current;
                    }
                    return flags;
                }
                catch (XslLoadException)
                {
                    return (XslFlags.FocusFilter | XslFlags.AnyType);
                }
            }

            public XslFlags AnalyzeAvt(string source)
            {
                this.typeDonor = null;
                if (source == null)
                {
                    return XslFlags.None;
                }
                try
                {
                    this.xsltCurrentNeeded = false;
                    XslFlags none = XslFlags.None;
                    int startIndex = 0;
                    while (startIndex < source.Length)
                    {
                        startIndex = source.IndexOf('{', startIndex);
                        if (startIndex == -1)
                        {
                            break;
                        }
                        startIndex++;
                        if ((startIndex < source.Length) && (source[startIndex] == '{'))
                        {
                            startIndex++;
                        }
                        else if (startIndex < source.Length)
                        {
                            XPathScanner scanner = new XPathScanner(source, startIndex);
                            none |= (XslFlags) this.xpathParser.Parse(scanner, this, LexKind.RBrace);
                            startIndex = scanner.LexStart + 1;
                        }
                    }
                    if (this.xsltCurrentNeeded)
                    {
                        none |= XslFlags.Current;
                    }
                    return (none & ~XslFlags.AnyType);
                }
                catch (XslLoadException)
                {
                    return XslFlags.FocusFilter;
                }
            }

            public virtual XslFlags Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
            {
                this.typeDonor = null;
                if (((xpathAxis == XPathAxis.Self) && (nodeType == XPathNodeType.All)) && ((prefix == null) && (name == null)))
                {
                    return (XslFlags.Current | XslFlags.Node);
                }
                return (XslFlags.Current | XslFlags.Nodeset);
            }

            public virtual XslFlags EndBuild(XslFlags result) => 
                result;

            public virtual XslFlags Function(string prefix, string name, IList<XslFlags> args)
            {
                this.typeDonor = null;
                XslFlags none = XslFlags.None;
                foreach (XslFlags flags2 in args)
                {
                    none |= flags2;
                }
                XslFlags nodeset = XslFlags.None;
                if (prefix.Length == 0)
                {
                    XPathBuilder.FunctionInfo<XPathBuilder.FuncId> info;
                    if (XPathBuilder.FunctionTable.TryGetValue(name, out info))
                    {
                        XPathBuilder.FuncId id = info.id;
                        nodeset = XPathFunctionFlags[(int) id];
                        if ((args.Count == 0) && ((((id == XPathBuilder.FuncId.LocalName) || (id == XPathBuilder.FuncId.NamespaceUri)) || ((id == XPathBuilder.FuncId.Name) || (id == XPathBuilder.FuncId.String))) || (((id == XPathBuilder.FuncId.Number) || (id == XPathBuilder.FuncId.StringLength)) || (id == XPathBuilder.FuncId.Normalize))))
                        {
                            nodeset |= XslFlags.Current;
                        }
                    }
                    else
                    {
                        XPathBuilder.FunctionInfo<QilGenerator.FuncId> info2;
                        if (QilGenerator.FunctionTable.TryGetValue(name, out info2))
                        {
                            QilGenerator.FuncId id2 = info2.id;
                            nodeset = XsltFunctionFlags[(int) id2];
                            if (id2 == QilGenerator.FuncId.Current)
                            {
                                this.xsltCurrentNeeded = true;
                            }
                            else if ((id2 == QilGenerator.FuncId.GenerateId) && (args.Count == 0))
                            {
                                nodeset |= XslFlags.Current;
                            }
                        }
                    }
                }
                else
                {
                    string ns = this.ResolvePrefix(prefix);
                    if (ns == "urn:schemas-microsoft-com:xslt")
                    {
                        switch (name)
                        {
                            case "node-set":
                                nodeset = XslFlags.Nodeset;
                                break;

                            case "string-compare":
                                nodeset = XslFlags.Number;
                                break;

                            case "utc":
                                nodeset = XslFlags.String;
                                break;

                            case "format-date":
                                nodeset = XslFlags.String;
                                break;

                            case "format-time":
                                nodeset = XslFlags.String;
                                break;

                            case "local-name":
                                nodeset = XslFlags.String;
                                break;

                            case "namespace-uri":
                                nodeset = XslFlags.String;
                                break;

                            case "number":
                                nodeset = XslFlags.Number;
                                break;
                        }
                    }
                    else
                    {
                        string str3;
                        if ((ns == "http://exslt.org/common") && ((str3 = name) != null))
                        {
                            if (str3 == "node-set")
                            {
                                nodeset = XslFlags.Nodeset;
                            }
                            else if (str3 == "object-type")
                            {
                                nodeset = XslFlags.String;
                            }
                        }
                    }
                    if (nodeset == XslFlags.None)
                    {
                        nodeset = XslFlags.AnyType;
                        if (this.compiler.Settings.EnableScript && (ns != null))
                        {
                            XmlExtensionFunction function = this.compiler.Scripts.ResolveFunction(name, ns, args.Count, new XslAstAnalyzer.NullErrorHelper());
                            if (function != null)
                            {
                                XmlQueryType xmlReturnType = function.XmlReturnType;
                                if (xmlReturnType == XmlQueryTypeFactory.StringX)
                                {
                                    nodeset = XslFlags.String;
                                }
                                else if (xmlReturnType == XmlQueryTypeFactory.DoubleX)
                                {
                                    nodeset = XslFlags.Number;
                                }
                                else if (xmlReturnType == XmlQueryTypeFactory.BooleanX)
                                {
                                    nodeset = XslFlags.Boolean;
                                }
                                else if (xmlReturnType == XmlQueryTypeFactory.NodeNotRtf)
                                {
                                    nodeset = XslFlags.Node;
                                }
                                else if (xmlReturnType == XmlQueryTypeFactory.NodeDodS)
                                {
                                    nodeset = XslFlags.Nodeset;
                                }
                                else if (xmlReturnType == XmlQueryTypeFactory.ItemS)
                                {
                                    nodeset = XslFlags.AnyType;
                                }
                                else if (xmlReturnType == XmlQueryTypeFactory.Empty)
                                {
                                    nodeset = XslFlags.Nodeset;
                                }
                            }
                        }
                        nodeset |= XslFlags.SideEffects;
                    }
                }
                return ((none & ~XslFlags.AnyType) | nodeset);
            }

            public virtual XslFlags JoinStep(XslFlags left, XslFlags right)
            {
                this.typeDonor = null;
                return ((left & ~XslFlags.AnyType) | XslFlags.Nodeset);
            }

            public virtual XslFlags Number(double value)
            {
                this.typeDonor = null;
                return XslFlags.Number;
            }

            public virtual XslFlags Operator(XPathOperator op, XslFlags left, XslFlags right)
            {
                this.typeDonor = null;
                XslFlags flags = (left | right) & ~XslFlags.AnyType;
                return (flags | OperatorType[(int) op]);
            }

            public virtual XslFlags Predicate(XslFlags nodeset, XslFlags predicate, bool isReverseStep)
            {
                this.typeDonor = null;
                return (((nodeset & ~XslFlags.AnyType) | XslFlags.Nodeset) | (predicate & XslFlags.SideEffects));
            }

            private string ResolvePrefix(string prefix)
            {
                if (prefix.Length == 0)
                {
                    return string.Empty;
                }
                return this.scope.LookupNamespace(prefix);
            }

            private VarPar ResolveVariable(string prefix, string name)
            {
                string uri = this.ResolvePrefix(prefix);
                if (uri == null)
                {
                    return null;
                }
                return this.scope.LookupVariable(name, uri);
            }

            public virtual void StartBuild()
            {
            }

            public virtual XslFlags String(string value)
            {
                this.typeDonor = null;
                return XslFlags.String;
            }

            public virtual XslFlags Variable(string prefix, string name)
            {
                this.typeDonor = this.ResolveVariable(prefix, name);
                if (this.typeDonor == null)
                {
                    return XslFlags.AnyType;
                }
                return XslFlags.None;
            }

            public VarPar TypeDonor =>
                this.typeDonor;
        }
    }
}

