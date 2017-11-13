namespace PdfSharp.Pdf
{
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("({Value})")]
    public sealed class PdfStringObject : PdfObject
    {
        private PdfStringFlags flags;
        private string value;

        public PdfStringObject()
        {
            this.flags = PdfStringFlags.RawEncoding;
        }

        public PdfStringObject(PdfDocument document, string value) : base(document)
        {
            this.value = value;
            this.flags = PdfStringFlags.RawEncoding;
        }

        public PdfStringObject(string value, PdfStringEncoding encoding)
        {
            this.value = value;
            this.flags = (PdfStringFlags) encoding;
        }

        internal PdfStringObject(string value, PdfStringFlags flags)
        {
            this.value = value;
            this.flags = flags;
        }

        public override string ToString() => 
            this.value;

        internal override void WriteObject(PdfWriter writer)
        {
            writer.WriteBeginObject(this);
            writer.Write(new PdfString(this.value, this.flags));
            writer.WriteEndObject();
        }

        public PdfStringEncoding Encoding
        {
            get => 
                (((PdfStringEncoding) this.flags) & ((PdfStringEncoding) 15));
            set
            {
                this.flags = (this.flags & ~PdfStringFlags.EncodingMask) | (((PdfStringFlags) ((int) value)) & PdfStringFlags.EncodingMask);
            }
        }

        internal byte[] EncryptionValue
        {
            get
            {
                if (this.value != null)
                {
                    return PdfEncoders.RawEncoding.GetBytes(this.value);
                }
                return new byte[0];
            }
            set
            {
                this.value = PdfEncoders.RawEncoding.GetString(value, 0, value.Length);
            }
        }

        public bool HexLiteral
        {
            get => 
                ((this.flags & PdfStringFlags.HexLiteral) != PdfStringFlags.RawEncoding);
            set
            {
                this.flags = value ? (this.flags | PdfStringFlags.HexLiteral) : (this.flags & ~PdfStringFlags.HexLiteral);
            }
        }

        public int Length
        {
            get
            {
                if (this.value != null)
                {
                    return this.value.Length;
                }
                return 0;
            }
        }

        public string Value
        {
            get => 
                (this.value ?? "");
            set
            {
                this.value = value ?? "";
            }
        }
    }
}

