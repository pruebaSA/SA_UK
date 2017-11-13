namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class ApplyTemplatesAction : ContainerAction
    {
        private XmlQualifiedName mode;
        private const int PositionAdvanced = 4;
        private const int ProcessedChildren = 2;
        private const int ProcessNextNode = 3;
        private static ApplyTemplatesAction s_BuiltInRule = new ApplyTemplatesAction();
        private int selectKey;
        private const int TemplateProcessed = 5;

        internal ApplyTemplatesAction()
        {
            this.selectKey = -1;
        }

        private ApplyTemplatesAction(XmlQualifiedName mode)
        {
            this.selectKey = -1;
            this.mode = mode;
        }

        internal static ApplyTemplatesAction BuiltInRule() => 
            s_BuiltInRule;

        internal static ApplyTemplatesAction BuiltInRule(XmlQualifiedName mode)
        {
            if ((mode != null) && !mode.IsEmpty)
            {
                return new ApplyTemplatesAction(mode);
            }
            return BuiltInRule();
        }

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            this.CompileContent(compiler);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Select))
            {
                this.selectKey = compiler.AddQuery(xpathQuery);
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

        private void CompileContent(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            if (!compiler.Recurse())
            {
                return;
            }
        Label_0012:
            switch (input.NodeType)
            {
                case XPathNodeType.Element:
                {
                    compiler.PushNamespaceScope();
                    string namespaceURI = input.NamespaceURI;
                    string localName = input.LocalName;
                    if (!Keywords.Equals(namespaceURI, input.Atoms.XsltNamespace))
                    {
                        throw compiler.UnexpectedKeyword();
                    }
                    if (!Keywords.Equals(localName, input.Atoms.Sort))
                    {
                        if (!Keywords.Equals(localName, input.Atoms.WithParam))
                        {
                            throw compiler.UnexpectedKeyword();
                        }
                        WithParamAction action = compiler.CreateWithParamAction();
                        base.CheckDuplicateParams(action.Name);
                        base.AddAction(action);
                        break;
                    }
                    base.AddAction(compiler.CreateSortAction());
                    break;
                }
                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                case XPathNodeType.ProcessingInstruction:
                case XPathNodeType.Comment:
                    goto Label_00F3;

                default:
                    throw XsltException.Create("Xslt_InvalidContents", new string[] { "apply-templates" });
            }
            compiler.PopScope();
        Label_00F3:
            if (compiler.Advance())
            {
                goto Label_0012;
            }
            compiler.ToParent();
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    processor.ResetParams();
                    processor.InitSortArray();
                    if ((base.containedActions == null) || (base.containedActions.Count <= 0))
                    {
                        break;
                    }
                    processor.PushActionFrame(frame);
                    frame.State = 2;
                    return;

                case 1:
                    return;

                case 2:
                    break;

                case 3:
                    goto Label_00C2;

                case 4:
                    goto Label_00DB;

                case 5:
                    frame.State = 3;
                    goto Label_00C2;

                default:
                    return;
            }
            if (this.selectKey == -1)
            {
                if (!frame.Node.HasChildren)
                {
                    frame.Finished();
                    return;
                }
                frame.InitNewNodeSet(frame.Node.SelectChildren(XPathNodeType.All));
            }
            else
            {
                frame.InitNewNodeSet(processor.StartQuery(frame.NodeSet, this.selectKey));
            }
            if (processor.SortArray.Count != 0)
            {
                frame.SortNewNodeSet(processor, processor.SortArray);
            }
            frame.State = 3;
        Label_00C2:
            if (frame.NewNextNode(processor))
            {
                frame.State = 4;
            }
            else
            {
                frame.Finished();
                return;
            }
        Label_00DB:
            processor.PushTemplateLookup(frame.NewNodeSet, this.mode, null);
            frame.State = 5;
        }
    }
}

