namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfIntegerObject : PdfNumberObject
    {
        private int value;

        public PdfIntegerObject()
        {
        }

        public PdfIntegerObject(int value)
        {
            this.value = value;
        }

        public PdfIntegerObject(PdfDocument document, int value) : base(document)
        {
            this.value = value;
        }

        public override string ToString() => 
            this.value.ToString(CultureInfo.InvariantCulture);

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(this.value);
            writer.WriteEndObject();
        }

        public int Value =>
            this.value;
    }
}

