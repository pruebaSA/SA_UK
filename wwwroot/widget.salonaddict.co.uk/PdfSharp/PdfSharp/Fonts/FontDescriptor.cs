namespace PdfSharp.Fonts
{
    using PdfSharp.Drawing;
    using System;

    internal class FontDescriptor
    {
        protected int ascender;
        protected int capHeight;
        protected int descender;
        protected string encodingScheme;
        protected string familyName;
        protected int flags;
        protected string fontFile;
        private XFontMetrics fontMetrics;
        protected string fontName;
        protected string fontType;
        protected string fullName;
        protected bool isFixedPitch;
        protected float italicAngle;
        protected int leading;
        protected int stemV;
        protected int strikeoutPosition;
        protected int strikeoutSize;
        protected int underlinePosition;
        protected int underlineThickness;
        protected int unitsPerEm;
        protected string version;
        protected string weight;
        protected int xHeight;
        protected int xMax;
        protected int xMin;
        protected int yMax;
        protected int yMin;

        public int Ascender =>
            this.ascender;

        public int CapHeight =>
            this.capHeight;

        public int Descender =>
            this.descender;

        public string EncodingScheme =>
            this.encodingScheme;

        public string FamilyName =>
            this.familyName;

        public int Flags =>
            this.flags;

        public string FontFile =>
            this.fontFile;

        public XFontMetrics FontMetrics
        {
            get
            {
                if (this.fontMetrics == null)
                {
                    this.fontMetrics = new XFontMetrics(this.fontName, this.unitsPerEm, this.ascender, this.descender, this.leading, this.capHeight, this.xHeight, this.stemV, 0, 0, 0);
                }
                return this.fontMetrics;
            }
        }

        public string FontName =>
            this.fontName;

        public string FontType =>
            this.fontType;

        public string FullName =>
            this.fullName;

        public virtual bool IsBoldFace =>
            false;

        public bool IsFixedPitch =>
            this.isFixedPitch;

        public virtual bool IsItalicFace =>
            false;

        public float ItalicAngle =>
            this.italicAngle;

        public int Leading =>
            this.leading;

        public int StemV =>
            this.stemV;

        public int StrikeoutPosition =>
            this.strikeoutPosition;

        public int StrikeoutSize =>
            this.strikeoutSize;

        public int UnderlinePosition =>
            this.underlinePosition;

        public int UnderlineThickness =>
            this.underlineThickness;

        public int UnitsPerEm =>
            this.unitsPerEm;

        public string Version =>
            this.version;

        public string Weight =>
            this.weight;

        public int XHeight =>
            this.xHeight;

        public int XMax =>
            this.xMax;

        public int XMin =>
            this.xMin;

        public int YMax =>
            this.yMax;

        public int YMin =>
            this.yMin;
    }
}

