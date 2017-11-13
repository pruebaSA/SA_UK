namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal sealed class AvtEvent : TextEvent
    {
        private int key;

        public AvtEvent(int key)
        {
            this.key = key;
        }

        public override string Evaluate(Processor processor, ActionFrame frame) => 
            processor.EvaluateString(frame, this.key);

        public override bool Output(Processor processor, ActionFrame frame) => 
            processor.TextEvent(processor.EvaluateString(frame, this.key));
    }
}

