namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Internal;
    using PdfSharp.Pdf.IO;
    using PdfSharp.Pdf.Security;
    using System;

    internal sealed class PdfTrailer : PdfDictionary
    {
        internal PdfStandardSecurityHandler securityHandler;

        public PdfTrailer(PdfDocument document) : base(document)
        {
            base.document = document;
        }

        internal PdfArray CreateNewDocumentIDs()
        {
            PdfArray array = new PdfArray(base.document);
            byte[] bytes = Guid.NewGuid().ToByteArray();
            string str = PdfEncoders.RawEncoding.GetString(bytes, 0, bytes.Length);
            array.Elements.Add(new PdfString(str, PdfStringFlags.HexLiteral));
            array.Elements.Add(new PdfString(str, PdfStringFlags.HexLiteral));
            base.Elements["/ID"] = array;
            return array;
        }

        internal void Finish()
        {
            PdfReference reference = base.document.trailer.Elements["/Root"] as PdfReference;
            if ((reference != null) && (reference.Value == null))
            {
                reference = base.document.irefTable[reference.ObjectID];
                base.document.trailer.Elements["/Root"] = reference;
            }
            reference = base.document.trailer.Elements["/Info"] as PdfReference;
            if ((reference != null) && (reference.Value == null))
            {
                reference = base.document.irefTable[reference.ObjectID];
                base.document.trailer.Elements["/Info"] = reference;
            }
            reference = base.document.trailer.Elements["/Encrypt"] as PdfReference;
            if (reference != null)
            {
                reference = base.document.irefTable[reference.ObjectID];
                base.document.trailer.Elements["/Encrypt"] = reference;
                reference.Value = base.document.trailer.securityHandler;
                base.document.trailer.securityHandler.Reference = reference;
                reference.Value.Reference = reference;
            }
            base.Elements.Remove("/Prev");
            base.document.irefTable.IsUnderConstruction = false;
        }

        public string GetDocumentID(int index)
        {
            if ((index < 0) || (index > 1))
            {
                throw new ArgumentOutOfRangeException("index", index, "Index must be 0 or 1.");
            }
            PdfArray array = base.Elements["/ID"] as PdfArray;
            if ((array != null) && (array.Elements.Count >= 2))
            {
                PdfItem item = array.Elements[index];
                if (item is PdfString)
                {
                    return ((PdfString) item).Value;
                }
            }
            return "";
        }

        public void SetDocumentID(int index, string value)
        {
            if ((index < 0) || (index > 1))
            {
                throw new ArgumentOutOfRangeException("index", index, "Index must be 0 or 1.");
            }
            PdfArray array = base.Elements["/ID"] as PdfArray;
            if ((array == null) || (array.Elements.Count < 2))
            {
                array = this.CreateNewDocumentIDs();
            }
            array.Elements[index] = new PdfString(value, PdfStringFlags.HexLiteral);
        }

        internal override void WriteObject(PdfWriter writer)
        {
            base.elements.Remove("/XRefStm");
            PdfStandardSecurityHandler securityHandler = writer.SecurityHandler;
            writer.SecurityHandler = null;
            base.WriteObject(writer);
            writer.SecurityHandler = securityHandler;
        }

        public PdfDocumentInformation Info =>
            ((PdfDocumentInformation) base.Elements.GetValue("/Info", VCF.CreateIndirect));

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public PdfCatalog Root =>
            ((PdfCatalog) base.Elements.GetValue("/Root", VCF.CreateIndirect));

        public PdfStandardSecurityHandler SecurityHandler
        {
            get
            {
                if (this.securityHandler == null)
                {
                    this.securityHandler = (PdfStandardSecurityHandler) base.Elements.GetValue("/Encrypt", VCF.CreateIndirect);
                }
                return this.securityHandler;
            }
        }

        public int Size
        {
            get => 
                base.Elements.GetInteger("/Size");
            set
            {
                base.Elements.SetInteger("/Size", value);
            }
        }

        internal sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfStandardSecurityHandler))]
            public const string Encrypt = "/Encrypt";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string ID = "/ID";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfDocumentInformation))]
            public const string Info = "/Info";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string Prev = "/Prev";
            [KeyInfo(KeyType.Required | KeyType.Dictionary, typeof(PdfCatalog))]
            public const string Root = "/Root";
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string Size = "/Size";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string XRefStm = "/XRefStm";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfTrailer.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

