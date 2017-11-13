namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Collections.Generic;

    internal sealed class PdfFontTable : PdfResourceTable
    {
        private readonly Dictionary<FontSelector, PdfFont> fonts;

        public PdfFontTable(PdfDocument document) : base(document)
        {
            this.fonts = new Dictionary<FontSelector, PdfFont>();
        }

        public PdfFont GetFont(XFont font)
        {
            PdfFont font2;
            string name = font.Name;
            FontSelector key = font.selector;
            if (key == null)
            {
                key = new FontSelector(font);
                font.selector = key;
            }
            if (!this.fonts.TryGetValue(key, out font2))
            {
                if (font.Unicode)
                {
                    font2 = new PdfType0Font(base.owner, font, font.IsVertical);
                }
                else
                {
                    font2 = new PdfTrueTypeFont(base.owner, font);
                }
                this.fonts[key] = font2;
            }
            return font2;
        }

        public PdfFont GetFont(string idName, byte[] fontData)
        {
            PdfFont font;
            FontSelector key = new FontSelector(idName);
            if (!this.fonts.TryGetValue(key, out font))
            {
                font = new PdfType0Font(base.owner, idName, fontData, false);
                this.fonts[key] = font;
            }
            return font;
        }

        public void PrepareForSave()
        {
            foreach (PdfFont font in this.fonts.Values)
            {
                font.PrepareForSave();
            }
        }

        public PdfFont TryGetFont(string idName)
        {
            PdfFont font;
            FontSelector key = new FontSelector(idName);
            this.fonts.TryGetValue(key, out font);
            return font;
        }

        public class FontSelector
        {
            private PdfSharp.Pdf.Advanced.FontType fontType;
            private string name;
            private XFontStyle style;

            public FontSelector(XFont font)
            {
                this.name = font.Name;
                this.style = font.Style & XFontStyle.BoldItalic;
                if (((this.style & XFontStyle.Bold) == XFontStyle.Bold) && !font.FontFamily.IsStyleAvailable(XFontStyle.Bold))
                {
                    this.style &= ~XFontStyle.Bold;
                }
                if (((this.style & XFontStyle.Italic) == XFontStyle.Italic) && !font.FontFamily.IsStyleAvailable(XFontStyle.Italic))
                {
                    this.style &= ~XFontStyle.Italic;
                }
                this.fontType = font.Unicode ? PdfSharp.Pdf.Advanced.FontType.Type0 : PdfSharp.Pdf.Advanced.FontType.TrueType;
            }

            public FontSelector(string name)
            {
                this.name = name;
                this.fontType = PdfSharp.Pdf.Advanced.FontType.Type0;
            }

            public FontSelector(XFontFamily family, XFontStyle style)
            {
                throw new NotImplementedException("PdfFontSelector(XFontFamily family, XFontStyle style)");
            }

            public override bool Equals(object obj)
            {
                PdfFontTable.FontSelector selector = obj as PdfFontTable.FontSelector;
                return ((((obj != null) && (this.name == selector.name)) && (this.style == selector.style)) && (this.fontType == selector.fontType));
            }

            public override int GetHashCode() => 
                ((this.name.GetHashCode() ^ this.style.GetHashCode()) ^ this.fontType.GetHashCode());

            public static bool operator ==(PdfFontTable.FontSelector selector1, PdfFontTable.FontSelector selector2)
            {
                if (!object.ReferenceEquals(selector1, null))
                {
                    selector1.Equals(selector2);
                }
                return object.ReferenceEquals(selector2, null);
            }

            public static bool operator !=(PdfFontTable.FontSelector selector1, PdfFontTable.FontSelector selector2) => 
                !(selector1 == selector2);

            public override string ToString()
            {
                string str = "";
                switch (this.style)
                {
                    case XFontStyle.Regular:
                        str = "(Regular)";
                        break;

                    case XFontStyle.Bold:
                        str = "(Bold)";
                        break;

                    case XFontStyle.Italic:
                        str = "(Italic)";
                        break;

                    case XFontStyle.BoldItalic:
                        str = "(BoldItalic)";
                        break;
                }
                return (this.name + str);
            }

            public PdfSharp.Pdf.Advanced.FontType FontType =>
                this.fontType;

            public string Name =>
                this.name;

            public XFontStyle Style =>
                this.style;
        }
    }
}

