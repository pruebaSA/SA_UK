namespace System.Xml.Xsl.XsltOld
{
    using MS.Internal.Xml.XPath;
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal class CopyOfAction : CompiledAction
    {
        private const int NodeSetCopied = 3;
        private const int ResultStored = 2;
        private int selectKey = -1;

        internal override void Compile(Compiler compiler)
        {
            base.CompileAttributes(compiler);
            base.CheckRequiredAttribute(compiler, this.selectKey != -1, "select");
            base.CheckEmpty(compiler);
        }

        internal override bool CompileAttribute(Compiler compiler)
        {
            string localName = compiler.Input.LocalName;
            string xpathQuery = compiler.Input.Value;
            if (Keywords.Equals(localName, compiler.Atoms.Select))
            {
                this.selectKey = compiler.AddQuery(xpathQuery);
                return true;
            }
            return false;
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                {
                    Query valueQuery = processor.GetValueQuery(this.selectKey);
                    object obj2 = valueQuery.Evaluate(frame.NodeSet);
                    if (!(obj2 is XPathNodeIterator))
                    {
                        XPathNavigator nav = obj2 as XPathNavigator;
                        if (nav != null)
                        {
                            processor.PushActionFrame(CopyNodeSetAction.GetAction(), new XPathSingletonIterator(nav));
                            frame.State = 3;
                            return;
                        }
                        string text = XmlConvert.ToXPathString(obj2);
                        if (processor.TextEvent(text))
                        {
                            frame.Finished();
                            return;
                        }
                        frame.StoredOutput = text;
                        frame.State = 2;
                        return;
                    }
                    processor.PushActionFrame(CopyNodeSetAction.GetAction(), new XPathArrayIterator(valueQuery));
                    frame.State = 3;
                    return;
                }
                case 1:
                    break;

                case 2:
                    processor.TextEvent(frame.StoredOutput);
                    frame.Finished();
                    return;

                case 3:
                    frame.Finished();
                    break;

                default:
                    return;
            }
        }
    }
}

