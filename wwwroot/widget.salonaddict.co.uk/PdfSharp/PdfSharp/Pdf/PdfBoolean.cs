namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfBoolean : PdfItem
    {
        public static readonly PdfBoolean False = new PdfBoolean(false);
        public static readonly PdfBoolean True = new PdfBoolean(true);
        private bool value;

        public PdfBoolean()
        {
        }

        public PdfBoolean(bool value)
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
            writer.Write(this);
        }

        public bool Value =>
            this.value;
    }
}

