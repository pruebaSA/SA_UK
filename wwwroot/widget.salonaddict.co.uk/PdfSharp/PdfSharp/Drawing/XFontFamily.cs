namespace PdfSharp.Drawing
{
    using System;
    using System.Drawing;

    public sealed class XFontFamily
    {
        internal FontFamily gdiFamily;
        private readonly string name;

        internal XFontFamily()
        {
        }

        internal XFontFamily(FontFamily family)
        {
            this.name = family.Name;
            this.gdiFamily = family;
        }

        public XFontFamily(string name)
        {
            this.name = name;
            this.gdiFamily = new FontFamily(name);
        }

        public int GetCellAscent(XFontStyle style) => 
            this.gdiFamily.GetCellAscent((FontStyle) style);

        public int GetCellDescent(XFontStyle style) => 
            this.gdiFamily.GetCellDescent((FontStyle) style);

        public int GetEmHeight(XFontStyle style) => 
            this.gdiFamily.GetEmHeight((FontStyle) style);

        public static XFontFamily[] GetFamilies(XGraphics graphics)
        {
            FontFamily[] families = null;
            families = FontFamily.Families;
            int length = families.Length;
            XFontFamily[] familyArray = new XFontFamily[length];
            for (int i = 0; i < length; i++)
            {
                familyArray[i] = new XFontFamily(families[i]);
            }
            return familyArray;
        }

        public int GetLineSpacing(XFontStyle style) => 
            this.gdiFamily.GetLineSpacing((FontStyle) style);

        public bool IsStyleAvailable(XFontStyle style) => 
            this.gdiFamily.IsStyleAvailable((FontStyle) style);

        public static XFontFamily[] Families
        {
            get
            {
                FontFamily[] families = FontFamily.Families;
                int length = families.Length;
                XFontFamily[] familyArray2 = new XFontFamily[length];
                for (int i = 0; i < length; i++)
                {
                    familyArray2[i] = new XFontFamily(families[i]);
                }
                return familyArray2;
            }
        }

        public string Name =>
            this.name;
    }
}

