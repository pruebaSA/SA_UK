namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfReal : PdfNumber
    {
        private double value;

        public PdfReal()
        {
        }

        public PdfReal(double value)
        {
            this.value = value;
        }

        public override string ToString() => 
            this.value.ToString("0.###", CultureInfo.InvariantCulture);

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        public double Value =>
            this.value;
    }
}

