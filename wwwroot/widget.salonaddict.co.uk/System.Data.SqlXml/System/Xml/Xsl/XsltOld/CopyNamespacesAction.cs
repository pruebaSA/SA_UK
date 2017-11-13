namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal sealed class CopyNamespacesAction : Action
    {
        private const int Advance = 5;
        private const int BeginEvent = 2;
        private const int EndEvent = 4;
        private static CopyNamespacesAction s_Action = new CopyNamespacesAction();
        private const int TextEvent = 3;

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            while (processor.CanContinue)
            {
                switch (frame.State)
                {
                    case 0:
                        if (frame.Node.MoveToFirstNamespace(XPathNamespaceScope.ExcludeXml))
                        {
                            break;
                        }
                        frame.Finished();
                        return;

                    case 1:
                    case 3:
                        return;

                    case 2:
                        goto Label_0047;

                    case 4:
                        if (processor.EndEvent(XPathNodeType.Namespace))
                        {
                            goto Label_007C;
                        }
                        return;

                    case 5:
                    {
                        if (!frame.Node.MoveToNextNamespace(XPathNamespaceScope.ExcludeXml))
                        {
                            goto Label_009C;
                        }
                        frame.State = 2;
                        continue;
                    }
                    default:
                        return;
                }
                frame.State = 2;
            Label_0047:
                if (!processor.BeginEvent(XPathNodeType.Namespace, null, frame.Node.LocalName, frame.Node.Value, false))
                {
                    return;
                }
                frame.State = 4;
                continue;
            Label_007C:
                frame.State = 5;
                continue;
            Label_009C:
                frame.Node.MoveToParent();
                frame.Finished();
                return;
            }
        }

        internal static CopyNamespacesAction GetAction() => 
            s_Action;
    }
}

