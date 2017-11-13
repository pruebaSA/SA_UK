namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.Xsl;

    internal class UseAttributeSetsAction : CompiledAction
    {
        private const int ProcessingSets = 2;
        private XmlQualifiedName[] useAttributeSets;
        private string useString;

        internal override void Compile(Compiler compiler)
        {
            this.useString = compiler.Input.Value;
            if (this.useString.Length == 0)
            {
                this.useAttributeSets = new XmlQualifiedName[0];
            }
            else
            {
                string[] strArray = XmlConvert.SplitString(this.useString);
                try
                {
                    this.useAttributeSets = new XmlQualifiedName[strArray.Length];
                    for (int i = 0; i < strArray.Length; i++)
                    {
                        this.useAttributeSets[i] = compiler.CreateXPathQName(strArray[i]);
                    }
                }
                catch (XsltException)
                {
                    if (!compiler.ForwardCompatibility)
                    {
                        throw;
                    }
                    this.useAttributeSets = new XmlQualifiedName[0];
                }
            }
        }

        internal override void Execute(Processor processor, ActionFrame frame)
        {
            switch (frame.State)
            {
                case 0:
                    frame.Counter = 0;
                    frame.State = 2;
                    break;

                case 1:
                    return;

                case 2:
                    break;

                default:
                    return;
            }
            if (frame.Counter < this.useAttributeSets.Length)
            {
                AttributeSetAction attributeSet = processor.RootAction.GetAttributeSet(this.useAttributeSets[frame.Counter]);
                frame.IncrementCounter();
                processor.PushActionFrame(attributeSet, frame.NodeSet);
            }
            else
            {
                frame.Finished();
            }
        }

        internal XmlQualifiedName[] UsedSets =>
            this.useAttributeSets;
    }
}

