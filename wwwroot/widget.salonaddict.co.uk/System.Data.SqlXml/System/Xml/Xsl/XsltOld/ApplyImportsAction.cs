namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.Xsl;

    internal class ApplyImportsAction : CompiledAction
    {
        private XmlQualifiedName mode;
        private Stylesheet stylesheet;
        private const int TemplateProcessed = 2;

        internal override void Compile(Compiler compiler)
        {
            base.CheckEmpty(compiler);
            if (!compiler.CanHaveApplyImports)
            {
                throw XsltException.Create("Xslt_ApplyImports", new string[0]);
            }
            this.mode = compiler.CurrentMode;
            this.stylesheet = compiler.CompiledStylesheet;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    processor.PushTemplateLookup(frame.NodeSet, this.mode, this.stylesheet);
                    frame.State = 2;
                    return;

                case 1:
                    break;

                case 2:
                    frame.Finished();
                    break;

                default:
                    return;
            }
        }
    }
}

