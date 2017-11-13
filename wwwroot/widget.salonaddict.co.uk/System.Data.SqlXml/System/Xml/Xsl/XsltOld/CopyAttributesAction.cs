namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal sealed class CopyAttributesAction : Action
    {
        private const int Advance = 5;
        private const int BeginEvent = 2;
        private const int EndEvent = 4;
        private static CopyAttributesAction s_Action = new CopyAttributesAction();
        private const int TextEvent = 3;

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            while (processor.CanContinue)
            {
                switch (frame.State)
                {
                    case 0:
                        if (frame.Node.HasAttributes && frame.Node.MoveToFirstAttribute())
                        {
                            break;
                        }
                        frame.Finished();
                        return;

                    case 1:
                        return;

                    case 2:
                        goto Label_0053;

                    case 3:
                        if (SendTextEvent(processor, frame.Node))
                        {
                            goto Label_007A;
                        }
                        return;

                    case 4:
                        if (SendEndEvent(processor, frame.Node))
                        {
                            goto Label_0092;
                        }
                        return;

                    case 5:
                    {
                        if (!frame.Node.MoveToNextAttribute())
                        {
                            goto Label_00B1;
                        }
                        frame.State = 2;
                        continue;
                    }
                    default:
                        return;
                }
                frame.State = 2;
            Label_0053:
                if (!SendBeginEvent(processor, frame.Node))
                {
                    return;
                }
                frame.State = 3;
                continue;
            Label_007A:
                frame.State = 4;
                continue;
            Label_0092:
                frame.State = 5;
                continue;
            Label_00B1:
                frame.Node.MoveToParent();
                frame.Finished();
                return;
            }
        }

        internal static CopyAttributesAction GetAction() => 
            s_Action;

        private static bool SendBeginEvent(Processor processor, XPathNavigator node) => 
            processor.BeginEvent(XPathNodeType.Attribute, node.Prefix, node.LocalName, node.NamespaceURI, false);

        private static bool SendEndEvent(Processor processor, XPathNavigator node) => 
            processor.EndEvent(XPathNodeType.Attribute);

        private static bool SendTextEvent(Processor processor, XPathNavigator node) => 
            processor.TextEvent(node.Value);
    }
}

