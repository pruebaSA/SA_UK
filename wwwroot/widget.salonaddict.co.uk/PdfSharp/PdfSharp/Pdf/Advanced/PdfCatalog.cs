namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.AcroForms;
    using PdfSharp.Pdf.IO;
    using System;

    public sealed class PdfCatalog : PdfDictionary
    {
        private PdfAcroForm acroForm;
        private PdfOutline outline;
        private PdfPages pages;
        private string version;
        private PdfViewerPreferences viewerPreferences;

        private PdfCatalog(PdfDictionary dictionary) : base(dictionary)
        {
            this.version = "1.3";
        }

        public PdfCatalog(PdfDocument document) : base(document)
        {
            this.version = "1.3";
            base.Elements.SetName("/Type", "/Catalog");
            this.version = "1.4";
        }

        internal override void PrepareForSave()
        {
            if (this.pages != null)
            {
                this.pages.PrepareForSave();
            }
            if ((this.outline != null) && (this.outline.Outlines.Count > 0))
            {
                if (base.Elements["/PageMode"] == null)
                {
                    this.PageMode = PdfPageMode.UseOutlines;
                }
                this.outline.PrepareForSave();
            }
        }

        internal override void WriteObject(PdfWriter writer)
        {
            if (((this.outline != null) && (this.outline.Outlines.Count > 0)) && (base.Elements["/PageMode"] == null))
            {
                this.PageMode = PdfPageMode.UseOutlines;
            }
            base.WriteObject(writer);
        }

        public PdfAcroForm AcroForm
        {
            get
            {
                if (this.acroForm == null)
                {
                    this.acroForm = (PdfAcroForm) base.Elements.GetValue("/AcroForm");
                }
                return this.acroForm;
            }
        }

        public string Language
        {
            get => 
                base.Elements.GetString("/Lang");
            set
            {
                if (value == null)
                {
                    base.Elements.Remove("/Lang");
                }
                else
                {
                    base.Elements.SetString("/Lang", value);
                }
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal PdfOutline.PdfOutlineCollection Outlines =>
            this.outline?.Outlines;

        internal PdfPageLayout PageLayout
        {
            get => 
                ((PdfPageLayout) base.Elements.GetEnumFromName("/PageLayout", PdfPageLayout.SinglePage));
            set
            {
                base.Elements.SetEnumAsName("/PageLayout", value);
            }
        }

        internal PdfPageMode PageMode
        {
            get => 
                ((PdfPageMode) base.Elements.GetEnumFromName("/PageMode", PdfPageMode.UseNone));
            set
            {
                base.Elements.SetEnumAsName("/PageMode", value);
            }
        }

        public PdfPages Pages
        {
            get
            {
                if (this.pages == null)
                {
                    this.pages = (PdfPages) base.Elements.GetValue("/Pages", VCF.CreateIndirect);
                    if (this.Owner.IsImported)
                    {
                        this.pages.FlattenPageTree();
                    }
                }
                return this.pages;
            }
        }

        public string Version
        {
            get => 
                this.version;
            set
            {
                switch (value)
                {
                    case "1.0":
                    case "1.1":
                    case "1.2":
                        throw new InvalidOperationException("Unsupported PDF version.");

                    case "1.3":
                    case "1.4":
                        this.version = value;
                        return;

                    case "1.5":
                    case "1.6":
                        throw new InvalidOperationException("Unsupported PDF version.");
                }
                throw new ArgumentException("Invalid version.");
            }
        }

        internal PdfViewerPreferences ViewerPreferences
        {
            get
            {
                if (this.viewerPreferences == null)
                {
                    this.viewerPreferences = (PdfViewerPreferences) base.Elements.GetValue("/ViewerPreferences", VCF.CreateIndirect);
                }
                return this.viewerPreferences;
            }
        }

        internal sealed class Keys : KeysBase
        {
            [KeyInfo("1.4", KeyType.Optional | KeyType.Dictionary)]
            public const string AA = "/AA";
            [KeyInfo("1.2", KeyType.Optional | KeyType.Dictionary, typeof(PdfAcroForm))]
            public const string AcroForm = "/AcroForm";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Dictionary)]
            public const string Dests = "/Dests";
            [KeyInfo("1.4", KeyType.Optional | KeyType.String)]
            public const string Lang = "/Lang";
            [KeyInfo("1.5", KeyType.Optional | KeyType.Dictionary)]
            public const string Legal = "/Legal";
            [KeyInfo("1.4", KeyType.Optional | KeyType.Dictionary)]
            public const string MarkInfo = "/MarkInfo";
            private static DictionaryMeta meta;
            [KeyInfo("1.4", KeyType.MustBeIndirect | KeyType.Optional | KeyType.Dictionary)]
            public const string Metadata = "/Metadata";
            [KeyInfo("1.2", KeyType.Optional | KeyType.Dictionary)]
            public const string Names = "/Names";
            [KeyInfo("1.5", KeyType.Optional | KeyType.Dictionary)]
            public const string OCProperties = "/OCProperties";
            [KeyInfo("1.1", KeyType.Optional | KeyType.ArrayOrDictionary)]
            public const string OpenAction = "/OpenAction";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfOutline))]
            public const string Outlines = "/Outlines";
            [KeyInfo("1.4", KeyType.Optional | KeyType.Array)]
            public const string OutputIntents = "/OutputIntents";
            [KeyInfo("1.3", KeyType.Optional | KeyType.NumberTree)]
            public const string PageLabels = "/PageLabels";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string PageLayout = "/PageLayout";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string PageMode = "/PageMode";
            [KeyInfo(KeyType.MustBeIndirect | KeyType.Required | KeyType.Dictionary, typeof(PdfPages))]
            public const string Pages = "/Pages";
            [KeyInfo("1.5", KeyType.Optional | KeyType.Dictionary)]
            public const string Perms = "/Perms";
            [KeyInfo("1.4", KeyType.Optional | KeyType.Dictionary)]
            public const string PieceInfo = "/PieceInfo";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Dictionary)]
            public const string SpiderInfo = "/SpiderInfo";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Dictionary)]
            public const string StructTreeRoot = "/StructTreeRoot";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Array)]
            public const string Threads = "/Threads";
            [KeyInfo(KeyType.Required | KeyType.Name, FixedValue="Catalog")]
            public const string Type = "/Type";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Dictionary)]
            public const string URI = "/URI";
            [KeyInfo("1.4", KeyType.Optional | KeyType.Name)]
            public const string Version = "/Version";
            [KeyInfo("1.2", KeyType.Optional | KeyType.Dictionary, typeof(PdfViewerPreferences))]
            public const string ViewerPreferences = "/ViewerPreferences";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfCatalog.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

