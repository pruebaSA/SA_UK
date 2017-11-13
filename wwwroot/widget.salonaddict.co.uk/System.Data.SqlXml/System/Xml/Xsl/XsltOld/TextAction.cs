namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class TextAction : CompiledAction
    {
        private bool disableOutputEscaping;
        private string text;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            this.CompileContent(compiler);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string str2 = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.DisableOutputEscaping))
            {
                this.disableOutputEscaping = compiler.GetYesNo(str2);
                return true;
            }
            return false;
        }

        private void CompileContent(Compiler compiler)
        {
            if (compiler.Recurse())
            {
                NavigatorInput input = compiler.Input;
                this.text = string.Empty;
                do
                {
                    switch (input.NodeType)
                    {
                        case XPathNodeType.Text:
                        case XPathNodeType.SignificantWhitespace:
                        case XPathNodeType.Whitespace:
                            this.text = this.text + input.Value;
                            break;

                        case XPathNodeType.ProcessingInstruction:
                        case XPathNodeType.Comment:
                            break;

                        default:
                            throw compiler.UnexpectedKeyword();
                    }
                }
                while (compiler.Advance());
                compiler.ToParent();
            }
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            if ((frame.State == 0) && processor.TextEvent(this.text, this.disableOutputEscaping))
            {
                frame.Finished();
            }
        }
    }
}

