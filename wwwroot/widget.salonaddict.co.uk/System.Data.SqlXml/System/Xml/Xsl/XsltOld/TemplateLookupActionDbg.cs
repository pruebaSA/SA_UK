namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal class TemplateLookupActionDbg : TemplateLookupAction
    {
        internal override void Execute(Processor processor, ActionFrame frame)
        {
            Action action = null;
            if (base.mode == Compiler.BuiltInMode)
            {
                base.mode = processor.GetPrevioseMode();
            }
            processor.SetCurrentMode(base.mode);
            if (base.mode != null)
            {
                action = (base.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node, base.mode) : base.importsOf.FindTemplateImports(processor, frame.Node, base.mode);
            }
            else
            {
                action = (base.importsOf == null) ? processor.Stylesheet.FindTemplate(processor, frame.Node) : base.importsOf.FindTemplateImports(processor, frame.Node);
            }
            if ((action == null) && (processor.RootAction.builtInSheet != null))
            {
                action = processor.RootAction.builtInSheet.FindTemplate(processor, frame.Node, Compiler.BuiltInMode);
            }
            if (action == null)
            {
                action = base.BuiltInTemplate(frame.Node);
            }
            if (action != null)
            {
                frame.SetAction(action);
            }
            else
            {
                frame.Finished();
            }
        }
    }
}

