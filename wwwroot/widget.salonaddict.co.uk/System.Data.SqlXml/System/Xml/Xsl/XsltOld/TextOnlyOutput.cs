namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.IO;
    using System.Xml;

    internal class TextOnlyOutput : RecordOutput
    {
        private Processor processor;
        private TextWriter writer;

        internal TextOnlyOutput(Processor processor, Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }
            this.processor = processor;
            this.writer = new StreamWriter(stream, this.Output.Encoding);
        }

        internal TextOnlyOutput(Processor processor, TextWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            this.processor = processor;
            this.writer = writer;
        }

        public Processor.OutputResult RecordDone(RecordBuilder record)
        {
            BuilderInfo mainNode = record.MainNode;
            switch (mainNode.NodeType)
            {
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    this.writer.Write(mainNode.Value);
                    break;
            }
            record.Reset();
            return Processor.OutputResult.Continue;
        }

        public void TheEnd()
        {
            this.writer.Flush();
        }

        internal XsltOutput Output =>
            this.processor.Output;

        public TextWriter Writer =>
            this.writer;
    }
}

