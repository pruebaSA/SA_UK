namespace PdfSharp.Pdf.Annotations
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using System;

    public sealed class PdfLinkAnnotation : PdfAnnotation
    {
        private int destPage;
        private LinkType linkType;
        private string url;

        public PdfLinkAnnotation()
        {
            base.Elements.SetName("/Subtype", "/Link");
        }

        public PdfLinkAnnotation(PdfDocument document) : base(document)
        {
            base.Elements.SetName("/Subtype", "/Link");
        }

        public static PdfLinkAnnotation CreateDocumentLink(PdfRectangle rect, int destinationPage) => 
            new PdfLinkAnnotation { 
                linkType = LinkType.Document,
                Rectangle = rect,
                destPage = destinationPage
            };

        public static PdfLinkAnnotation CreateFileLink(PdfRectangle rect, string fileName) => 
            new PdfLinkAnnotation { 
                linkType = LinkType.File,
                Rectangle = rect,
                url = fileName
            };

        public static PdfLinkAnnotation CreateWebLink(PdfRectangle rect, string url) => 
            new PdfLinkAnnotation { 
                linkType = LinkType.Web,
                Rectangle = rect,
                url = url
            };

        internal override void WriteObject(PdfWriter writer)
        {
            PdfPage page = null;
            if (base.Elements["/BS"] == null)
            {
                base.Elements["/BS"] = new PdfLiteral("<</Type/Border>>");
            }
            if (base.Elements["/Border"] == null)
            {
                base.Elements["/Border"] = new PdfLiteral("[0 0 0]");
            }
            switch (this.linkType)
            {
                case LinkType.Document:
                {
                    int destPage = this.destPage;
                    if (destPage > this.Owner.PageCount)
                    {
                        destPage = this.Owner.PageCount;
                    }
                    destPage--;
                    page = this.Owner.Pages[destPage];
                    base.Elements["/Dest"] = new PdfLiteral("[{0} 0 R/XYZ null null 0]", new object[] { page.ObjectNumber });
                    break;
                }
                case LinkType.Web:
                    base.Elements["/A"] = new PdfLiteral("<</S/URI/URI{0}>>", new object[] { PdfEncoders.ToStringLiteral(this.url, PdfStringEncoding.WinAnsiEncoding, writer.SecurityHandler) });
                    break;

                case LinkType.File:
                    base.Elements["/A"] = new PdfLiteral("<</Type/Action/S/Launch/F<</Type/Filespec/F{0}>> >>", new object[] { PdfEncoders.ToStringLiteral(this.url, PdfStringEncoding.WinAnsiEncoding, writer.SecurityHandler) });
                    break;
            }
            base.WriteObject(writer);
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal class Keys : PdfAnnotation.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.ArrayOrNameOrString)]
            public const string Dest = "/Dest";
            [KeyInfo("1.2", KeyType.Optional | KeyType.Name)]
            public const string H = "/H";
            private static DictionaryMeta meta;
            [KeyInfo("1.3", KeyType.Optional | KeyType.Dictionary)]
            public const string PA = "/PA";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfLinkAnnotation.Keys));
                    }
                    return meta;
                }
            }
        }

        private enum LinkType
        {
            Document,
            Web,
            File
        }
    }
}

