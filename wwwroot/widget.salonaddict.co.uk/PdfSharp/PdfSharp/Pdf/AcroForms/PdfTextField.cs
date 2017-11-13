namespace PdfSharp.Pdf.AcroForms
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;

    public sealed class PdfTextField : PdfAcroField
    {
        private XColor backColor;
        private XFont font;
        private XColor foreColor;

        internal PdfTextField(PdfDictionary dict) : base(dict)
        {
            this.font = new XFont("Courier New", 10.0);
            this.foreColor = XColors.Black;
            this.backColor = XColor.Empty;
        }

        internal PdfTextField(PdfDocument document) : base(document)
        {
            this.font = new XFont("Courier New", 10.0);
            this.foreColor = XColors.Black;
            this.backColor = XColor.Empty;
        }

        internal override void PrepareForSave()
        {
            base.PrepareForSave();
            this.RenderAppearance();
        }

        private void RenderAppearance()
        {
            PdfRectangle rectangle = base.Elements.GetRectangle("/Rect");
            XForm form = new XForm(base.document, rectangle.Size);
            XGraphics graphics = XGraphics.FromForm(form);
            if (this.backColor != XColor.Empty)
            {
                graphics.DrawRectangle(new XSolidBrush(this.BackColor), rectangle.ToXRect() - rectangle.Location);
            }
            if (this.Text.Length > 0)
            {
                graphics.DrawString(this.Text, this.Font, new XSolidBrush(this.ForeColor), (rectangle.ToXRect() - rectangle.Location) + new XPoint(2.0, 0.0), XStringFormats.TopLeft);
            }
            form.DrawingFinished();
            PdfDictionary dictionary = base.Elements["/AP"] as PdfDictionary;
            if (dictionary == null)
            {
                dictionary = new PdfDictionary(base.document);
                base.Elements["/AP"] = dictionary;
            }
            dictionary.Elements["/N"] = form.PdfForm.Reference;
        }

        public XColor BackColor
        {
            get => 
                this.backColor;
            set
            {
                this.backColor = value;
            }
        }

        public XFont Font
        {
            get => 
                this.font;
            set
            {
                this.font = value;
            }
        }

        public XColor ForeColor
        {
            get => 
                this.foreColor;
            set
            {
                this.foreColor = value;
            }
        }

        public int MaxLength
        {
            get => 
                base.Elements.GetInteger("/MaxLen");
            set
            {
                base.Elements.SetInteger("/MaxLen", value);
            }
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        public bool MultiLine
        {
            get => 
                ((base.Flags & PdfAcroFieldFlags.Multiline) != ((PdfAcroFieldFlags) 0));
            set
            {
                if (value)
                {
                    base.SetFlags |= PdfAcroFieldFlags.Multiline;
                }
                else
                {
                    base.SetFlags &= ~PdfAcroFieldFlags.Multiline;
                }
            }
        }

        public bool Password
        {
            get => 
                ((base.Flags & PdfAcroFieldFlags.Password) != ((PdfAcroFieldFlags) 0));
            set
            {
                if (value)
                {
                    base.SetFlags |= PdfAcroFieldFlags.Password;
                }
                else
                {
                    base.SetFlags &= ~PdfAcroFieldFlags.Password;
                }
            }
        }

        public string Text
        {
            get => 
                base.Elements.GetString("/V");
            set
            {
                base.Elements.SetString("/V", value);
                this.RenderAppearance();
            }
        }

        public class Keys : PdfAcroField.Keys
        {
            [KeyInfo(KeyType.Optional | KeyType.Integer)]
            public const string MaxLen = "/MaxLen";
            private static DictionaryMeta meta;

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfTextField.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

