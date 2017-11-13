namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfUIntegerObject : PdfNumberObject
    {
        private uint value;

        public PdfUIntegerObject()
        {
        }

        public PdfUIntegerObject(uint value)
        {
            this.value = value;
        }

        public PdfUIntegerObject(PdfDocument document, uint value) : base(document)
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

        public uint Value =>
            this.value;
    }
}

