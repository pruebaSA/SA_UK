namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class AttributeSetAction : ContainerAction
    {
        internal XmlQualifiedName name;

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
            }
            else if (Keywords.Equals(localName, compiler.Atoms.UseAttributeSets))
            {
                base.AddAction(compiler.CreateUseAttributeSetsAction());
            }
            else
            {
                return false;
            }
            return true;
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
                            if (!Keywords.Equals(namespaceURI, input.Atoms.XsltNamespace) || !Keywords.Equals(localName, input.Atoms.Attribute))
                            {
                                throw compiler.UnexpectedKeyword();
                            }
                            base.AddAction(compiler.CreateAttributeAction());
                            compiler.PopScope();
                            break;
                        }
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                        case XPathNodeType.ProcessingInstruction:
                        case XPathNodeType.Comment:
                            break;

                        default:
                            throw XsltException.Create("Xslt_InvalidContents", new string[] { "attribute-set" });
                    }
                }
                while (compiler.Advance());
                compiler.ToParent();
            }
        }

        internal void Merge(AttributeSetAction attributeAction)
        {
            Action action;
            for (int i = 0; (action = attributeAction.GetAction(i)) != null; i++)
            {
                base.AddAction(action);
            }
        }

        internal XmlQualifiedName Name =>
            this.name;
    }
}

