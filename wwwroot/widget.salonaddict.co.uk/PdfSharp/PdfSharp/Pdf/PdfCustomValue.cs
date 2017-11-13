namespace PdfSharp.Pdf
{
    using System;

    public class PdfCustomValue : PdfDictionary
    {
        public PdfCustomValueCompressionMode CompressionMode;

        public PdfCustomValue()
        {
            base.CreateStream(new byte[0]);
        }

        public PdfCustomValue(byte[] bytes)
        {
            base.CreateStream(bytes);
        }

        internal PdfCustomValue(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfCustomValue(PdfDocument document) : base(document)
        {
            base.CreateStream(new byte[0]);
        }

        public byte[] Value
        {
            get => 
                base.Stream.Value;
            set
            {
                base.Stream.Value = value;
            }
        }
    }
}

