namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Text;

    internal class StringOutput : SequentialOutput
    {
        private StringBuilder builder;
        private string result;

        internal StringOutput(Processor processor) : base(processor)
        {
            this.builder = new StringBuilder();
        }

        internal override void Close()
        {
            this.result = this.builder.ToString();
        }

        internal override void Write(char outputChar)
        {
            this.builder.Append(outputChar);
        }

        internal override void Write(string outputText)
        {
            this.builder.Append(outputText);
        }

        internal string Result =>
            this.result;
    }
}

