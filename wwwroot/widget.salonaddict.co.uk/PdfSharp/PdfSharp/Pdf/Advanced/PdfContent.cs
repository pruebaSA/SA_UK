namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing.Pdf;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Filters;
    using PdfSharp.Pdf.IO;
    using System;

    public sealed class PdfContent : PdfDictionary
    {
        internal XGraphicsPdfRenderer pdfRenderer;

        public PdfContent(PdfDictionary dict) : base(dict)
        {
            this.Decode();
        }

        public PdfContent(PdfDocument document) : base(document)
        {
        }

        internal PdfContent(PdfPage page) : base(page?.Owner)
        {
        }

        private void Decode()
        {
            if ((base.Stream != null) && (base.Stream.Value != null))
            {
                PdfItem filterItem = base.Elements["/Filter"];
                if (filterItem != null)
                {
                    byte[] buffer = Filtering.Decode(base.Stream.Value, filterItem);
                    if (buffer != null)
                    {
                        base.Stream.Value = buffer;
                        base.Elements.Remove("/Filter");
                        base.Elements.SetInteger("/Length", base.Stream.Length);
                    }
                }
            }
        }

        internal void PreserveGraphicsState()
        {
            if (base.Stream != null)
            {
                byte[] sourceArray = base.Stream.Value;
                int length = sourceArray.Length;
                if ((length != 0) && ((sourceArray[0] != 0x71) || (sourceArray[1] != 10)))
                {
                    byte[] destinationArray = new byte[(length + 2) + 3];
                    destinationArray[0] = 0x71;
                    destinationArray[1] = 10;
                    Array.Copy(sourceArray, 0, destinationArray, 2, length);
                    destinationArray[length + 2] = 0x20;
                    destinationArray[length + 3] = 0x51;
                    destinationArray[length + 4] = 10;
                    base.Stream.Value = destinationArray;
                    base.Elements.SetInteger("/Length", base.Stream.Length);
                }
            }
        }

        internal override void WriteObject(PdfWriter writer)
        {
            if (this.pdfRenderer != null)
            {
                this.pdfRenderer.Close();
            }
            if (base.Stream != null)
            {
                if (this.Owner.Options.CompressContentStreams)
                {
                    base.Stream.Value = Filtering.FlateDecode.Encode(base.Stream.Value);
                    base.Elements["/Filter"] = new PdfName("/FlateDecode");
                }
                base.Elements.SetInteger("/Length", base.Stream.Length);
            }
            base.WriteObject(writer);
        }

        public bool Compressed
        {
            set
            {
                if (value && (base.Elements["/Filter"] == null))
                {
                    byte[] buffer = Filtering.FlateDecode.Encode(base.Stream.Value);
                    base.Stream.Value = buffer;
                    base.Elements.SetInteger("/Length", base.Stream.Length);
                    base.Elements.SetName("/Filter", "/FlateDecode");
                }
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal sealed class Keys : PdfDictionary.PdfStream.Keys
        {
            private static DictionaryMeta meta;

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfContent.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

