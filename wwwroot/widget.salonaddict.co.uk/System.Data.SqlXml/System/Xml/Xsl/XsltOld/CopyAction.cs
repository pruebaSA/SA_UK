namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class CopyAction : ContainerAction
    {
        private const int ChildrenOnly = 8;
        private const int ContentsCopy = 6;
        private const int CopyText = 4;
        private bool empty;
        private const int NamespaceCopy = 5;
        private const int ProcessChildren = 7;
        private string useAttributeSets;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            if (compiler.Recurse())
            {
                base.CompileTemplate(compiler);
                compiler.ToParent();
            }
            if (base.containedActions == null)
            {
                this.empty = true;
            }
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string str2 = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.UseAttributeSets))
            {
                this.useAttributeSets = str2;
                base.AddAction(compiler.CreateUseAttributeSetsAction());
                return true;
            }
            return false;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            while (processor.CanContinue)
            {
                switch (frame.State)
                {
                    case 0:
                        if (!Processor.IsRoot(frame.Node))
                        {
                            break;
                        }
                        processor.PushActionFrame(frame);
                        frame.State = 8;
                        return;

                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        return;

                    case 5:
                        frame.State = 6;
                        if (frame.Node.NodeType != XPathNodeType.Element)
                        {
                            continue;
                        }
                        processor.PushActionFrame(CopyNamespacesAction.GetAction(), frame.NodeSet);
                        return;

                    case 6:
                        if ((frame.Node.NodeType != XPathNodeType.Element) || this.empty)
                        {
                            goto Label_00BD;
                        }
                        processor.PushActionFrame(frame);
                        frame.State = 7;
                        return;

                    case 7:
                        if (processor.CopyEndEvent(frame.Node))
                        {
                            frame.Finished();
                        }
                        return;

                    case 8:
                        frame.Finished();
                        return;

                    default:
                        return;
                }
                if (!processor.CopyBeginEvent(frame.Node, this.empty))
                {
                    return;
                }
                frame.State = 5;
                continue;
            Label_00BD:
                if (!processor.CopyTextEvent(frame.Node))
                {
                    break;
                }
                frame.State = 7;
            }
        }
    }
}

