namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal sealed class CopyNodeSetAction : Action
    {
        private const int Attributes = 5;
        private const int BeginEvent = 2;
        private const int Contents = 3;
        private const int EndEvent = 7;
        private const int Namespaces = 4;
        private static CopyNodeSetAction s_Action = new CopyNodeSetAction();
        private const int Subtree = 6;

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            while (processor.CanContinue)
            {
                XPathNodeType type;
                switch (frame.State)
                {
                    case 0:
                        if (!frame.NextNode(processor))
                        {
                            break;
                        }
                        frame.State = 2;
                        goto Label_004C;

                    case 1:
                        return;

                    case 2:
                        goto Label_004C;

                    case 3:
                        goto Label_0067;

                    case 4:
                        processor.PushActionFrame(CopyAttributesAction.GetAction(), frame.NodeSet);
                        frame.State = 5;
                        return;

                    case 5:
                        if (!frame.Node.HasChildren)
                        {
                            goto Label_00F1;
                        }
                        processor.PushActionFrame(GetAction(), frame.Node.SelectChildren(XPathNodeType.All));
                        frame.State = 6;
                        return;

                    case 6:
                    {
                        frame.State = 7;
                        continue;
                    }
                    case 7:
                        goto Label_0103;

                    default:
                        return;
                }
                frame.Finished();
                return;
            Label_004C:
                if (!SendBeginEvent(processor, frame.Node))
                {
                    return;
                }
                frame.State = 3;
                continue;
            Label_0067:
                type = frame.Node.NodeType;
                if ((type == XPathNodeType.Element) || (type == XPathNodeType.Root))
                {
                    processor.PushActionFrame(CopyNamespacesAction.GetAction(), frame.NodeSet);
                    frame.State = 4;
                    return;
                }
                if (!SendTextEvent(processor, frame.Node))
                {
                    return;
                }
                frame.State = 7;
                continue;
            Label_00F1:
                frame.State = 7;
            Label_0103:
                if (!SendEndEvent(processor, frame.Node))
                {
                    return;
                }
                frame.State = 0;
            }
        }

        internal static CopyNodeSetAction GetAction() => 
            s_Action;

        private static bool SendBeginEvent(Processor processor, XPathNavigator node) => 
            processor.CopyBeginEvent(node, node.IsEmptyElement);

        private static bool SendEndEvent(Processor processor, XPathNavigator node) => 
            processor.CopyEndEvent(node);

        private static bool SendTextEvent(Processor processor, XPathNavigator node) => 
            processor.CopyTextEvent(node);
    }
}

