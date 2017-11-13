namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class CallTemplateAction : ContainerAction
    {
        private XmlQualifiedName name;
        private const int ProcessedChildren = 2;
        private const int ProcessedTemplate = 3;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, this.name, "name");
            this.CompileContent(compiler);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string qname = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Name))
            {
                this.name = compiler.CreateXPathQName(qname);
                return true;
            }
            return false;
        }

        private void CompileContent(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            if (compiler.Recurse())
            {
                do
                {
                    switch (input.NodeType)
                    {
                        case XPathNodeType.Element:
                        {
                            compiler.PushNamespaceScope();
                            string namespaceURI = input.NamespaceURI;
                            string localName = input.LocalName;
                            if (!Keywords.Equals(namespaceURI, input.Atoms.XsltNamespace) || !Keywords.Equals(localName, input.Atoms.WithParam))
                            {
                                throw compiler.UnexpectedKeyword();
                            }
                            WithParamAction action = compiler.CreateWithParamAction();
                            base.CheckDuplicateParams(action.Name);
                            base.AddAction(action);
                            compiler.PopScope();
                            break;
                        }
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                        case XPathNodeType.ProcessingInstruction:
                        case XPathNodeType.Comment:
                            break;

                        default:
                            throw XsltException.Create("Xslt_InvalidContents", new string[] { "call-template" });
                    }
                }
                while (compiler.Advance());
                compiler.ToParent();
            }
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    processor.ResetParams();
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
                    frame.Finished();
                    return;

                default:
                    return;
            }
            TemplateAction action = processor.Stylesheet.FindTemplate(this.name);
            if (action == null)
            {
                throw XsltException.Create("Xslt_InvalidCallTemplate", new string[] { this.name.ToString() });
            }
            frame.State = 3;
            processor.PushActionFrame(action, frame.NodeSet);
        }
    }
}

