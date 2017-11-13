namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.IO;

    internal class TextOutput : SequentialOutput
    {
        private TextWriter writer;

        internal TextOutput(Processor processor, Stream stream) : base(processor)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            base.encoding = processor.Output.Encoding;
            this.writer = new StreamWriter(stream, base.encoding);
        }

        internal TextOutput(Processor processor, TextWriter writer) : base(processor)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            base.encoding = writer.Encoding;
            this.writer = writer;
        }

        internal override void Close()
        {
            this.writer.Flush();
            this.writer = null;
        }

        internal override void Write(char outputChar)
        {
            this.writer.Write(outputChar);
        }

        internal override void Write(string outputText)
        {
            this.writer.Write(outputText);
        }
    }
}

