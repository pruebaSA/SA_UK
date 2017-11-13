namespace PdfSharp.Pdf.Annotations
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;

    public abstract class PdfAnnotation : PdfDictionary
    {
        private PdfAnnotations parent;

        protected PdfAnnotation()
        {
            this.Initialize();
        }

        internal PdfAnnotation(PdfDictionary dict) : base(dict)
        {
        }

        protected PdfAnnotation(PdfDocument document) : base(document)
        {
            this.Initialize();
        }

        [Obsolete("Use 'Parent.Remove(this)'")]
        public void Delete()
        {
            this.Parent.Remove(this);
        }

        private void Initialize()
        {
            base.Elements.SetName("/Type", "/Annot");
            base.Elements.SetString("/NM", Guid.NewGuid().ToString("D"));
            base.Elements.SetDateTime("/M", DateTime.Now);
        }

        public XColor Color
        {
            get
            {
                PdfItem item = base.Elements["/C"];
                if (item is PdfArray)
                {
                    PdfArray array = (PdfArray) item;
                    if (array.Elements.Count == 3)
                    {
                        return XColor.FromArgb((int) (array.Elements.GetReal(0) * 255.0), (int) (array.Elements.GetReal(1) * 255.0), (int) (array.Elements.GetReal(2) * 255.0));
                    }
                }
                return XColors.Black;
            }
            set
            {
                PdfArray array = new PdfArray(this.Owner, new PdfReal[] { new PdfReal(((double) value.R) / 255.0), new PdfReal(((double) value.G) / 255.0), new PdfReal(((double) value.B) / 255.0) });
                base.Elements["/C"] = array;
                base.Elements.SetDateTime("/M", DateTime.Now);
            }
        }

        public string Contents
        {
            get => 
                base.Elements.GetString("/Contents", true);
            set
            {
                base.Elements.SetString("/Contents", value);
                base.Elements.SetDateTime("/M", DateTime.Now);
            }
        }

        public PdfAnnotationFlags Flags
        {
            get => 
                ((PdfAnnotationFlags) base.Elements.GetInteger("/F"));
            set
            {
                base.Elements.SetInteger("/F", (int) value);
                base.Elements.SetDateTime("/M", DateTime.Now);
            }
        }

        public double Opacity
        {
            get
            {
                if (!base.Elements.ContainsKey("/CA"))
                {
                    return 1.0;
                }
                return base.Elements.GetReal("/CA", true);
            }
            set
            {
                if ((value < 0.0) || (value > 1.0))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Opacity must be a value in the range from 0 to 1.");
                }
                base.Elements.SetReal("/CA", value);
                base.Elements.SetDateTime("/M", DateTime.Now);
            }
        }

        public PdfAnnotations Parent
        {
            get => 
                this.parent;
            set
            {
                this.parent = value;
            }
        }

        public PdfRectangle Rectangle
        {
            get => 
                base.Elements.GetRectangle("/Rect", true);
            set
            {
                base.Elements.SetRectangle("/Rect", value);
                base.Elements.SetDateTime("/M", DateTime.Now);
            }
        }

        public string Subject
        {
            get => 
                base.Elements.GetString("/Subj", true);
            set
            {
                base.Elements.SetString("/Subj", value);
                base.Elements.SetDateTime("/M", DateTime.Now);
            }
        }

        public string Title
        {
            get => 
                base.Elements.GetString("/T", true);
            set
            {
                base.Elements.SetString("/T", value);
                base.Elements.SetDateTime("/M", DateTime.Now);
            }
        }

        public class Keys : KeysBase
        {
            [KeyInfo("1.1", KeyType.Optional | KeyType.Dictionary)]
            public const string A = "/A";
            [KeyInfo("1.2", KeyType.Optional | KeyType.Dictionary)]
            public const string AP = "/AP";
            [KeyInfo("1.2", KeyType.Optional | KeyType.Dictionary)]
            public const string AS = "/AS";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Border = "/Border";
            [KeyInfo("1.2", KeyType.Optional | KeyType.Dictionary)]
            public const string BS = "/BS";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Array)]
            public const string C = "/C";
            [KeyInfo(KeyType.Optional | KeyType.Real)]
            public const string CA = "/CA";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string Contents = "/Contents";
            [KeyInfo("1.1", KeyType.Optional | KeyType.Integer)]
            public const string F = "/F";
            [KeyInfo(KeyType.Optional | KeyType.Date)]
            public const string M = "/M";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string NM = "/NM";
            [KeyInfo(KeyType.Optional | KeyType.Dictionary)]
            public const string Popup = "/Popup";
            [KeyInfo(KeyType.Required | KeyType.Rectangle)]
            public const string Rect = "/Rect";
            [KeyInfo("1.5", KeyType.Optional | KeyType.TextString)]
            public const string Subj = "/Subj";
            [KeyInfo(KeyType.Required | KeyType.Name)]
            public const string Subtype = "/Subtype";
            [KeyInfo(KeyType.Optional | KeyType.TextString)]
            public const string T = "/T";
            [KeyInfo(KeyType.Optional | KeyType.Name, FixedValue="Annot")]
            public const string Type = "/Type";
        }
    }
}

