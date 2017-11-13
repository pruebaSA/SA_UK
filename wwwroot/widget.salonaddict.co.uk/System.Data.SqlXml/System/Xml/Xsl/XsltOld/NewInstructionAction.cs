namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class NewInstructionAction : ContainerAction
    {
        private bool fallback;
        private string name;
        private string parent;

        internal override void Compile(Compiler compiler)
        {
            XPathNavigator navigator = compiler.Input.Navigator.Clone();
            this.name = navigator.Name;
            navigator.MoveToParent();
            this.parent = navigator.Name;
            if (compiler.Recurse())
            {
                this.CompileSelectiveTemplate(compiler);
                compiler.ToParent();
            }
        }

        internal void CompileSelectiveTemplate(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            do
            {
                if (Keywords.Equals(input.NamespaceURI, input.Atoms.XsltNamespace) && Keywords.Equals(input.LocalName, input.Atoms.Fallback))
                {
                    this.fallback = true;
                    if (compiler.Recurse())
                    {
                        base.CompileTemplate(compiler);
                        compiler.ToParent();
                    }
                }
            }
            while (compiler.Advance());
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (!this.fallback)
                    {
                        throw XsltException.Create("Xslt_UnknownExtensionElement", new string[] { this.name });
                    }
                    if ((base.containedActions != null) && (base.containedActions.Count > 0))
                    {
                        processor.PushActionFrame(frame);
                        frame.State = 1;
                        return;
                    }
                    break;

                case 1:
                    break;

                default:
                    return;
            }
            frame.Finished();
        }
    }
}

