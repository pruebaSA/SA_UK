namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal abstract class CompiledAction : Action
    {
        protected CompiledAction()
        {
        }

        public void CheckEmpty(Compiler compiler)
        {
            string name = compiler.Input.Name;
            if (compiler.Recurse())
            {
                do
                {
                    XPathNodeType nodeType = compiler.Input.NodeType;
                    if (((nodeType != XPathNodeType.Whitespace) && (nodeType != XPathNodeType.Comment)) && (nodeType != XPathNodeType.ProcessingInstruction))
                    {
                        throw XsltException.Create("Xslt_NotEmptyContents", new string[] { name });
                    }
                }
                while (compiler.Advance());
                compiler.ToParent();
            }
        }

        public void CheckRequiredAttribute(Compiler compiler, bool attr, string attrName)
        {
            if (!attr)
            {
                throw XsltException.Create("Xslt_MissingAttribute", new string[] { attrName });
            }
        }

        public void CheckRequiredAttribute(Compiler compiler, object attrValue, string attrName)
        {
            this.CheckRequiredAttribute(compiler, attrValue != null, attrName);
        }

        internal abstract void Compile(Compiler compiler);
        internal virtual bool CompileAttribute(Compiler compiler) => 
            false;

        public void CompileAttributes(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            string localName = input.LocalName;
            if (input.MoveToFirstAttribute())
            {
                do
                {
                    if (Keywords.Equals(input.NamespaceURI, input.Atoms.Empty))
                    {
                        try
                        {
                            if (!this.CompileAttribute(compiler))
                            {
                                throw XsltException.Create("Xslt_InvalidAttribute", new string[] { input.LocalName, localName });
                            }
                        }
                        catch
                        {
                            if (!compiler.ForwardCompatibility)
                            {
                                throw;
                            }
                        }
                    }
                }
                while (input.MoveToNextAttribute());
                input.ToParent();
            }
        }

        internal static string PrecalculateAvt(ref Avt avt)
        {
            string str = null;
            if ((avt != null) && avt.IsConstant)
            {
                str = avt.Evaluate(null, null);
                avt = null;
            }
            return str;
        }
    }
}

