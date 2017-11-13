namespace System.Xml.Xsl.XsltOld
{
    using System;

    internal class TextEvent : Event
    {
        private string text;

        protected TextEvent()
        {
        }

        public TextEvent(string text)
        {
            this.text = text;
        }

        public TextEvent(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            this.text = input.Value;
        }

        public virtual string Evaluate(Processor processor, ActionFrame frame) => 
            this.text;

        public override bool Output(Processor processor, ActionFrame frame) => 
            processor.TextEvent(this.text);
    }
}

