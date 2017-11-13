namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;

    public sealed class PdfResources : PdfDictionary
    {
        private PdfResourceMap colorSpaces;
        private int ExtGStateNumber;
        private PdfResourceMap extGStates;
        private int fontNumber;
        private PdfResourceMap fonts;
        private int formNumber;
        private int imageNumber;
        private Dictionary<string, object> importedResourceNames;
        private int PatternNumber;
        private PdfResourceMap patterns;
        private PdfResourceMap properties;
        private readonly Dictionary<PdfObject, string> resources;
        private int ShadingNumber;
        private PdfResourceMap shadings;
        private PdfResourceMap xObjects;

        internal PdfResources(PdfDictionary dict) : base(dict)
        {
            this.resources = new Dictionary<PdfObject, string>();
        }

        public PdfResources(PdfDocument document) : base(document)
        {
            this.resources = new Dictionary<PdfObject, string>();
            base.Elements["/ProcSet"] = new PdfLiteral("[/PDF/Text/ImageB/ImageC/ImageI]");
        }

        public string AddExtGState(PdfExtGState extGState)
        {
            string nextExtGStateName;
            if (!this.resources.TryGetValue(extGState, out nextExtGStateName))
            {
                nextExtGStateName = this.NextExtGStateName;
                this.resources[extGState] = nextExtGStateName;
                if (extGState.Reference == null)
                {
                    this.Owner.irefTable.Add(extGState);
                }
                this.ExtGStates.Elements[nextExtGStateName] = extGState.Reference;
            }
            return nextExtGStateName;
        }

        public string AddFont(PdfFont font)
        {
            string nextFontName;
            if (!this.resources.TryGetValue(font, out nextFontName))
            {
                nextFontName = this.NextFontName;
                this.resources[font] = nextFontName;
                if (font.Reference == null)
                {
                    this.Owner.irefTable.Add(font);
                }
                this.Fonts.Elements[nextFontName] = font.Reference;
            }
            return nextFontName;
        }

        public string AddForm(PdfFormXObject form)
        {
            string nextFormName;
            if (!this.resources.TryGetValue(form, out nextFormName))
            {
                nextFormName = this.NextFormName;
                this.resources[form] = nextFormName;
                if (form.Reference == null)
                {
                    this.Owner.irefTable.Add(form);
                }
                this.XObjects.Elements[nextFormName] = form.Reference;
            }
            return nextFormName;
        }

        public string AddImage(PdfImage image)
        {
            string nextImageName;
            if (!this.resources.TryGetValue(image, out nextImageName))
            {
                nextImageName = this.NextImageName;
                this.resources[image] = nextImageName;
                if (image.Reference == null)
                {
                    this.Owner.irefTable.Add(image);
                }
                this.XObjects.Elements[nextImageName] = image.Reference;
            }
            return nextImageName;
        }

        public string AddPattern(PdfShadingPattern pattern)
        {
            string nextPatternName;
            if (!this.resources.TryGetValue(pattern, out nextPatternName))
            {
                nextPatternName = this.NextPatternName;
                this.resources[pattern] = nextPatternName;
                if (pattern.Reference == null)
                {
                    this.Owner.irefTable.Add(pattern);
                }
                this.Patterns.Elements[nextPatternName] = pattern.Reference;
            }
            return nextPatternName;
        }

        public string AddPattern(PdfTilingPattern pattern)
        {
            string nextPatternName;
            if (!this.resources.TryGetValue(pattern, out nextPatternName))
            {
                nextPatternName = this.NextPatternName;
                this.resources[pattern] = nextPatternName;
                if (pattern.Reference == null)
                {
                    this.Owner.irefTable.Add(pattern);
                }
                this.Patterns.Elements[nextPatternName] = pattern.Reference;
            }
            return nextPatternName;
        }

        public string AddShading(PdfShading shading)
        {
            string nextShadingName;
            if (!this.resources.TryGetValue(shading, out nextShadingName))
            {
                nextShadingName = this.NextShadingName;
                this.resources[shading] = nextShadingName;
                if (shading.Reference == null)
                {
                    this.Owner.irefTable.Add(shading);
                }
                this.Shadings.Elements[nextShadingName] = shading.Reference;
            }
            return nextShadingName;
        }

        internal bool ExistsResourceNames(string name) => 
            this.importedResourceNames?.ContainsKey(name);

        internal PdfResourceMap ColorSpaces
        {
            get
            {
                if (this.colorSpaces == null)
                {
                    this.colorSpaces = (PdfResourceMap) base.Elements.GetValue("/ColorSpace", VCF.Create);
                }
                return this.colorSpaces;
            }
        }

        internal PdfResourceMap ExtGStates
        {
            get
            {
                if (this.extGStates == null)
                {
                    this.extGStates = (PdfResourceMap) base.Elements.GetValue("/ExtGState", VCF.Create);
                }
                return this.extGStates;
            }
        }

        internal PdfResourceMap Fonts
        {
            get
            {
                if (this.fonts == null)
                {
                    this.fonts = (PdfResourceMap) base.Elements.GetValue("/Font", VCF.Create);
                }
                return this.fonts;
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        private string NextExtGStateName
        {
            get
            {
                string str;
                while (this.ExistsResourceNames(str = $"/GS{this.ExtGStateNumber++}"))
                {
                }
                return str;
            }
        }

        private string NextFontName
        {
            get
            {
                string str;
                while (this.ExistsResourceNames(str = $"/F{this.fontNumber++}"))
                {
                }
                return str;
            }
        }

        private string NextFormName
        {
            get
            {
                string str;
                while (this.ExistsResourceNames(str = $"/Fm{this.formNumber++}"))
                {
                }
                return str;
            }
        }

        private string NextImageName
        {
            get
            {
                string str;
                while (this.ExistsResourceNames(str = $"/I{this.imageNumber++}"))
                {
                }
                return str;
            }
        }

        private string NextPatternName
        {
            get
            {
                string str;
                while (this.ExistsResourceNames(str = $"/Pa{this.PatternNumber++}"))
                {
                }
                return str;
            }
        }

        private string NextShadingName
        {
            get
            {
                string str;
                while (this.ExistsResourceNames(str = $"/Sh{this.ShadingNumber++}"))
                {
                }
                return str;
            }
        }

        internal PdfResourceMap Patterns
        {
            get
            {
                if (this.patterns == null)
                {
                    this.patterns = (PdfResourceMap) base.Elements.GetValue("/Pattern", VCF.Create);
                }
                return this.patterns;
            }
        }

        internal PdfResourceMap Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = (PdfResourceMap) base.Elements.GetValue("/Properties", VCF.Create);
                }
                return this.properties;
            }
        }

        internal PdfResourceMap Shadings
        {
            get
            {
                if (this.shadings == null)
                {
                    this.shadings = (PdfResourceMap) base.Elements.GetValue("/Shading", VCF.Create);
                }
                return this.shadings;
            }
        }

        internal PdfResourceMap XObjects
        {
            get
            {
                if (this.xObjects == null)
                {
                    this.xObjects = (PdfResourceMap) base.Elements.GetValue("/XObject", VCF.Create);
                }
                return this.xObjects;
            }
        }

        public sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResourceMap))]
            public const string ColorSpace = "/ColorSpace";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResourceMap))]
            public const string ExtGState = "/ExtGState";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResourceMap))]
            public const string Font = "/Font";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResourceMap))]
            public const string Pattern = "/Pattern";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string ProcSet = "/ProcSet";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResourceMap))]
            public const string Properties = "/Properties";
            [KeyInfo("1.3", KeyType.Optional | KeyType.Dictionary, typeof(PdfResourceMap))]
            public const string Shading = "/Shading";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary, typeof(PdfResourceMap))]
            public const string XObject = "/XObject";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfResources.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

