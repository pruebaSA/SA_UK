namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;

    public sealed class PdfNullObject : PdfObject
    {
        public PdfNullObject()
        {
        }

        public PdfNullObject(PdfDocument document) : base(document)
        {
        }

        public override string ToString() => 
            "null";

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.WriteRaw(" null ");
            writer.WriteEndObject();
        }
    }
}

