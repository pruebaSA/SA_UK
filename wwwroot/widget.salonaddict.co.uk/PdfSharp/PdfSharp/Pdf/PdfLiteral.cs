namespace PdfSharp.Pdf
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using System;

    public sealed class PdfLiteral : PdfItem
    {
        private readonly string value;

        public PdfLiteral()
        {
            this.value = string.Empty;
        }

        public PdfLiteral(string value)
        {
            this.value = string.Empty;
            this.value = value;
        }

        public PdfLiteral(string format, params object[] args)
        {
            this.value = string.Empty;
            this.value = PdfEncoders.Format(format, args);
        }

        public static PdfLiteral FromMatrix(XMatrix matrix) => 
            new PdfLiteral("[" + PdfEncoders.ToString(matrix) + "]");

        public override string ToString() => 
            this.value;

        internal override void WriteObject(PdfWriter writer)
        {
            writer.Write(this);
        }

        public string Value =>
            this.value;
    }
}

