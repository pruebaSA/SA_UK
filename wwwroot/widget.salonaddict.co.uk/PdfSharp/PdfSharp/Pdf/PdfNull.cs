namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;

    public sealed class PdfNull : PdfItem
    {
        public static readonly PdfNull Value = new PdfNull();

        private PdfNull()
        {
        }

        public override string ToString() => 
            "null";

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteRaw(" null ");
        }
    }
}

