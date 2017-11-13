namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class CommentAction : ContainerAction
    {
        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            if (compiler.Recurse())
            {
                base.CompileTemplate(compiler);
                compiler.ToParent();
            }
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    if (processor.BeginEvent(XPathNodeType.Comment, string.Empty, string.Empty, string.Empty, false))
                    {
                        processor.PushActionFrame(frame);
                        frame.State = 1;
                        return;
                    }
                    return;

                case 1:
                    if (processor.EndEvent(XPathNodeType.Comment))
                    {
                        frame.Finished();
                        return;
                    }
                    return;
            }
        }
    }
}

