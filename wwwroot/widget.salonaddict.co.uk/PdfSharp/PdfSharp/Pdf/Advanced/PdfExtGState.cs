namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfExtGState : PdfDictionary
    {
        public PdfExtGState(PdfDocument document) : base(document)
        {
            base.Elements.SetName("/Type", "/ExtGState");
        }

        internal void SetDefault1()
        {
            base.Elements.SetBoolean("/AIS", false);
            base.Elements.SetName("/BM", "/Normal");
            this.StrokeAlpha = 1.0;
            this.NonStrokeAlpha = 1.0;
            base.Elements.SetBoolean("/op", false);
            base.Elements.SetBoolean("/OP", false);
            base.Elements.SetBoolean("/SA", true);
            base.Elements.SetName("/SMask", "/None");
        }

        internal void SetDefault2()
        {
            base.Elements.SetBoolean("/AIS", false);
            base.Elements.SetName("/BM", "/Normal");
            this.StrokeAlpha = 1.0;
            this.NonStrokeAlpha = 1.0;
            base.Elements.SetBoolean("/op", true);
            base.Elements.SetBoolean("/OP", true);
            base.Elements.SetInteger("/OPM", 1);
            base.Elements.SetBoolean("/SA", true);
            base.Elements.SetName("/SMask", "/None");
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public double NonStrokeAlpha
        {
            set
            {
                base.Elements.SetReal("/ca", value);
            }
        }

        public PdfSoftMask SoftMask
        {
            set
            {
                base.Elements.SetReference("/SMask", value);
            }
        }

        public double StrokeAlpha
        {
            set
            {
                base.Elements.SetReal("/CA", value);
            }
        }

        internal sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string AIS = "/AIS";
            [KeyInfo(KeyType.Optional | KeyType.Function)]
            public const string BG = "/BG";
            [KeyInfo(KeyType.Optional | KeyType.FunctionOrName)]
            public const string BG2 = "/BG2";
            [KeyInfo(KeyType.Optional | KeyType.NameOrArray)]
            public const string BM = "/BM";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string ca = "/ca";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string CA = "/CA";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string D = "/D";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Font = "/Font";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string LC = "/LC";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string LJ = "/LJ";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string LW = "/LW";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string ML = "/ML";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string op = "/op";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string OP = "/OP";
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string OPM = "/OPM";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string RI = "/RI";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string SA = "/SA";
            [KeyInfo(KeyType.Optional | KeyType.NameOrDictionary)]
            public const string SMask = "/SMask";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string TK = "/TK";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Type = "/Type";
            [KeyInfo(KeyType.Optional | KeyType.Function)]
            public const string UCR = "/UCR";
            [KeyInfo(KeyType.Optional | KeyType.FunctionOrName)]
            public const string UCR2 = "/UCR2";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfExtGState.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

