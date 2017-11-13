namespace PdfSharp.Pdf
{
    using System;

    public sealed class PdfViewerPreferences : PdfDictionary
    {
        private PdfViewerPreferences(PdfDictionary dict) : base(dict)
        {
        }

        internal PdfViewerPreferences(PdfDocument document) : base(document)
        {
        }

        public bool CenterWindow
        {
            get => 
                base.Elements.GetBoolean("/CenterWindow");
            set
            {
                base.Elements.SetBoolean("/CenterWindow", value);
            }
        }

        public PdfReadingDirection? Direction
        {
            get
            {
                switch (base.Elements.GetName("/Direction"))
                {
                    case "L2R":
                        return 0;

                    case "R2L":
                        return 1;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    if (((PdfReadingDirection) value.Value) == PdfReadingDirection.RightToLeft)
                    {
                        base.Elements.SetName("/Direction", "R2L");
                    }
                    else
                    {
                        base.Elements.SetName("/Direction", "L2R");
                    }
                }
                else
                {
                    base.Elements.Remove("/Direction");
                }
            }
        }

        public bool DisplayDocTitle
        {
            get => 
                base.Elements.GetBoolean("/DisplayDocTitle");
            set
            {
                base.Elements.SetBoolean("/DisplayDocTitle", value);
            }
        }

        public bool FitWindow
        {
            get => 
                base.Elements.GetBoolean("/FitWindow");
            set
            {
                base.Elements.SetBoolean("/FitWindow", value);
            }
        }

        public bool HideMenubar
        {
            get => 
                base.Elements.GetBoolean("/HideMenubar");
            set
            {
                base.Elements.SetBoolean("/HideMenubar", value);
            }
        }

        public bool HideToolbar
        {
            get => 
                base.Elements.GetBoolean("/HideToolbar");
            set
            {
                base.Elements.SetBoolean("/HideToolbar", value);
            }
        }

        public bool HideWindowUI
        {
            get => 
                base.Elements.GetBoolean("/HideWindowUI");
            set
            {
                base.Elements.SetBoolean("/HideWindowUI", value);
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string CenterWindow = "/CenterWindow";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string Direction = "/Direction";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string DisplayDocTitle = "/DisplayDocTitle";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string FitWindow = "/FitWindow";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string HideMenubar = "/HideMenubar";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string HideToolbar = "/HideToolbar";
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string HideWindowUI = "/HideWindowUI";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string NonFullScreenPageMode = "/NonFullScreenPageMode";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string PrintArea = "/PrintArea";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string PrintClip = "/PrintClip";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string PrintScaling = "/PrintScaling";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string ViewArea = "/ViewArea";
            [KeyInfo(KeyType.Optional | KeyType.Name)]
            public const string ViewClip = "/ViewClip";

            public static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfViewerPreferences.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

