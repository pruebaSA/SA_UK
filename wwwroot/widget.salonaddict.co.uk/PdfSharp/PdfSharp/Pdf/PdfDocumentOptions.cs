namespace PdfSharp.Pdf
{
    using System;

    public sealed class PdfDocumentOptions
    {
        private PdfColorMode colorMode;
        private bool compressContentStreams = true;
        private bool noCompression;

        internal PdfDocumentOptions(PdfDocument document)
        {
        }

        public PdfColorMode ColorMode
        {
            get => 
                this.colorMode;
            set
            {
                this.colorMode = value;
            }
        }

        public bool CompressContentStreams
        {
            get => 
                this.compressContentStreams;
            set
            {
                this.compressContentStreams = value;
            }
        }

        public bool NoCompression
        {
            get => 
                this.noCompression;
            set
            {
                this.noCompression = value;
            }
        }
    }
}

