namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfDate : PdfItem
    {
        private DateTime value;

        public PdfDate()
        {
        }

        public PdfDate(DateTime value)
        {
            this.value = value;
        }

        public PdfDate(string value)
        {
            this.value = Parser.ParseDateTime(value, DateTime.MinValue);
        }

        public override string ToString()
        {
            string str = this.value.ToString("zzz").Replace(':', '\'');
            return $"D:{this.value:yyyyMMddHHmmss}{str}'";
        }

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteDocString(this.ToString());
        }

        public DateTime Value =>
            this.value;
    }
}

