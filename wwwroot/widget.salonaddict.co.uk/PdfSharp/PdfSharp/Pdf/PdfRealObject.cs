namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Globalization;

    public sealed class PdfRealObject : PdfNumberObject
    {
        private double value;

        public PdfRealObject()
        {
        }

        public PdfRealObject(double value)
        {
            this.value = value;
        }

        public PdfRealObject(PdfDocument document, double value) : base(document)
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

        public double Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

