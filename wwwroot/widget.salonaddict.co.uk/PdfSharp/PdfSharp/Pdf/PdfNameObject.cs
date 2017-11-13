namespace PdfSharp.Pdf
{
    using PdfSharp;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfNameObject : PdfObject
    {
        private string value;

        public PdfNameObject()
        {
            this.value = "/";
        }

        public PdfNameObject(PdfDocument document, string value) : base(document)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if ((value.Length == 0) || (value[0] != '/'))
            {
                throw new ArgumentException(PSSR.NameMustStartWithSlash);
            }
            this.value = value;
        }

        public override bool Equals(object obj) => 
            this.value.Equals(obj);

        public override int GetHashCode() => 
            this.value.GetHashCode();

        public static bool operator ==(PdfNameObject name, string str) => 
            (name.value == str);

        public static bool operator !=(PdfNameObject name, string str) => 
            (name.value != str);

        public override string ToString() => 
            this.value;

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(new PdfName(this.value));
            writer.WriteEndObject();
        }

        public string Value
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

