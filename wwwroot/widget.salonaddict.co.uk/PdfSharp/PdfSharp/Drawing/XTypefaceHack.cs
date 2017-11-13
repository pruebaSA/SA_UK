namespace PdfSharp.Drawing
{
    using System;

    internal class XTypefaceHack
    {
        private string fontFamilyName;
        private XFontStretch stretch;
        private XFontStyle style;
        private XFontWeight weight;

        public XTypefaceHack(string typefaceName) : this(typefaceName, XFontStyle.Regular, XFontWeights.Normal, new XFontStretch())
        {
        }

        public XTypefaceHack(string typefaceName, XFontStyle style, XFontWeight weight) : this(typefaceName, style, weight, new XFontStretch())
        {
        }

        public XTypefaceHack(string fontFamilyName, XFontStyle style, XFontWeight weight, XFontStretch stretch)
        {
            if (string.IsNullOrEmpty(fontFamilyName))
            {
                throw new ArgumentNullException("fontFamilyName");
            }
            this.fontFamilyName = fontFamilyName;
            this.style = style;
            this.weight = weight;
            this.stretch = stretch;
        }

        public string FontFamilyName =>
            this.fontFamilyName;

        public XFontStretch Stretch =>
            this.stretch;

        public XFontStyle Style =>
            this.style;

        public XFontWeight Weight =>
            this.weight;
    }
}

