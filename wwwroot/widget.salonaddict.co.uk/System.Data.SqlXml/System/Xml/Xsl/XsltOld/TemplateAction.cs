namespace System.Xml.Xsl.XsltOld
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class TemplateAction : TemplateBaseAction
    {
        private int matchKey = -1;
        private XmlQualifiedName mode;
        private XmlQualifiedName name;
        private double priority = double.NaN;
        private bool replaceNSAliasesDone;
        private int templateId;

        private void AnalyzePriority(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            if (double.IsNaN(this.priority) && (this.matchKey != -1))
            {
                UnionExpr expr2;
                TheQuery query = compiler.QueryStore[this.MatchKey];
                CompiledXpathExpr compiledQuery = query.CompiledQuery;
                Query queryTree = compiledQuery.QueryTree;
                while ((expr2 = queryTree as UnionExpr) != null)
                {
                    TemplateAction template = this.CloneWithoutName();
                    compiler.QueryStore.Add(new TheQuery(new CompiledXpathExpr(expr2.qy2, compiledQuery.Expression, false), query._ScopeManager));
                    template.matchKey = compiler.QueryStore.Count - 1;
                    template.priority = expr2.qy2.XsltDefaultPriority;
                    compiler.AddTemplate(template);
                    queryTree = expr2.qy1;
                }
                if (compiledQuery.QueryTree != queryTree)
                {
                    compiler.QueryStore[this.MatchKey] = new TheQuery(new CompiledXpathExpr(queryTree, compiledQuery.Expression, false), query._ScopeManager);
                }
                this.priority = queryTree.XsltDefaultPriority;
            }
        }

        private TemplateAction CloneWithoutName() => 
            new TemplateAction { 
                containedActions = base.containedActions,
                mode = this.mode,
                variableCount = base.variableCount,
                replaceNSAliasesDone = true
            };

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            if (this.matchKey == -1)
            {
                if (this.name == null)
                {
                    throw XsltException.Create("Xslt_TemplateNoAttrib", new string[0]);
                }
                if (this.mode != null)
                {
                    throw XsltException.Create("Xslt_InvalidModeAttribute", new string[0]);
                }
            }
            compiler.BeginTemplate(this);
            if (compiler.Recurse())
            {
                this.CompileParameters(compiler);
                base.CompileTemplate(compiler);
                compiler.ToParent();
            }
            compiler.EndTemplate();
            this.AnalyzePriority(compiler);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Match))
            {
                this.matchKey = compiler.AddQuery(xpathQuery, false, true, true);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Name))
            {
                this.name = compiler.CreateXPathQName(xpathQuery);
            }
            else if (Keywords.Equals(localName, compiler.Atoms.Priority))
            {
                this.priority = XmlConvert.ToXPathDouble(xpathQuery);
                if (double.IsNaN(this.priority) && !compiler.ForwardCompatibility)
                {
                    throw XsltException.Create("Xslt_InvalidAttrValue", new string[] { "priority", xpathQuery });
                }
            }
            else
            {
                if (!Keywords.Equals(localName, compiler.Atoms.Mode))
                {
                    return false;
                }
                if (compiler.AllowBuiltInMode && (xpathQuery == "*"))
                {
                    this.mode = Compiler.BuiltInMode;
                }
                else
                {
                    this.mode = compiler.CreateXPathQName(xpathQuery);
                }
            }
            return true;
        }

        protected void CompileParameters(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
        Label_0007:
            switch (input.NodeType)
            {
                case XPathNodeType.Element:
                    if (!Keywords.Equals(input.NamespaceURI, input.Atoms.XsltNamespace) || !Keywords.Equals(input.LocalName, input.Atoms.Param))
                    {
                        break;
                    }
                    compiler.PushNamespaceScope();
                    base.AddAction(compiler.CreateVariableAction(VariableType.LocalParameter));
                    compiler.PopScope();
                    goto Label_0084;

                case XPathNodeType.Text:
                    break;

                case XPathNodeType.SignificantWhitespace:
                    base.AddEvent(compiler.CreateTextEvent());
                    goto Label_0084;

                default:
                    goto Label_0084;
            }
            return;
        Label_0084:
            if (input.Advance())
            {
                goto Label_0007;
            }
        }

        internal virtual void CompileSingle(Compiler compiler)
        {
            this.matchKey = compiler.AddQuery("/", false, true, true);
            this.priority = 0.5;
            base.CompileOnceTemplate(compiler);
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (base.variableCount > 0)
                    {
                        frame.AllocateVariables(base.variableCount);
                    }
                    if ((base.containedActions != null) && (base.containedActions.Count > 0))
                    {
                        processor.PushActionFrame(frame);
                        frame.State = 1;
                    }
                    else
                    {
                        frame.Finished();
                    }
                    return;

                case 1:
                    frame.Finished();
                    return;
            }
        }

        internal override void ReplaceNamespaceAlias(Compiler compiler)
        {
            if (!this.replaceNSAliasesDone)
            {
                base.ReplaceNamespaceAlias(compiler);
                this.replaceNSAliasesDone = true;
            }
        }

        internal int MatchKey =>
            this.matchKey;

        internal XmlQualifiedName Mode =>
            this.mode;

        internal XmlQualifiedName Name =>
            this.name;

        internal double Priority =>
            this.priority;

        internal int TemplateId
        {
            get => 
                this.templateId;
            set
            {
                this.templateId = value;
            }
        }
    }
}

