namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfBooleanObject : PdfObject
    {
        private bool value;

        public PdfBooleanObject()
        {
        }

        public PdfBooleanObject(bool value)
        {
            this.value = value;
        }

        public PdfBooleanObject(PdfDocument document, bool value) : base(document)
        {
            this.value = value;
        }

        public override string ToString()
        {
            if (!this.value)
            {
                return bool.FalseString;
            }
            return bool.TrueString;
        }

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(this.value);
            writer.WriteEndObject();
        }

        public bool Value =>
            this.value;
    }
}

