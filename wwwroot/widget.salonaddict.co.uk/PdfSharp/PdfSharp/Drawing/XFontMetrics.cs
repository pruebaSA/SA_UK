namespace PdfSharp.Drawing
{
    using System;

    public sealed class XFontMetrics
    {
        private int ascent;
        private int averageWidth;
        private int capHeight;
        private int descent;
        private int leading;
        private int maxWidth;
        private string name;
        private int stemH;
        private int stemV;
        private int unitsPerEm;
        private int xHeight;

        internal XFontMetrics(string name, int unitsPerEm, int ascent, int descent, int leading, int capHeight, int xHeight, int stemV, int stemH, int averageWidth, int maxWidth)
        {
            this.name = name;
            this.unitsPerEm = unitsPerEm;
            this.ascent = ascent;
            this.descent = descent;
            this.leading = leading;
            this.capHeight = capHeight;
            this.xHeight = xHeight;
            this.stemV = stemV;
            this.stemH = stemH;
            this.averageWidth = averageWidth;
            this.maxWidth = maxWidth;
        }

        public int Ascent =>
            this.ascent;

        public int AverageWidth =>
            this.averageWidth;

        public int CapHeight =>
            this.capHeight;

        public int Descent =>
            this.descent;

        public int Leading =>
            this.leading;

        public int MaxWidth =>
            this.maxWidth;

        public string Name =>
            this.name;

        public int StemH =>
            this.stemH;

        public int StemV =>
            this.stemV;

        public int UnitsPerEm =>
            this.unitsPerEm;

        public int XHeight =>
            this.xHeight;
    }
}

