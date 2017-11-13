namespace PdfSharp.Fonts.OpenType
{
    using PdfSharp.Drawing;
    using PdfSharp.Fonts;
    using PdfSharp.Pdf.Internal;
    using System;
    using System.Text;

    internal sealed class OpenTypeDescriptor : FontDescriptor
    {
        internal FontData fontData;
        public int[] widths;

        internal OpenTypeDescriptor(XFont font) : this(font, font.PdfOptions)
        {
        }

        internal OpenTypeDescriptor(byte[] fontData)
        {
            try
            {
                this.fontData = new FontData(fontData);
                string name = this.fontData.name.Name;
                if (this.fontData.name.Style.Length != 0)
                {
                    name = name + "," + this.fontData.name.Style;
                }
                name = name.Replace(" ", "");
                base.fontName = name;
                this.Initialize();
            }
            catch
            {
                throw;
            }
        }

        public OpenTypeDescriptor(XFont font, XPdfFontOptions options)
        {
            try
            {
                this.fontData = new FontData(font, options);
                base.fontName = font.Name;
                this.Initialize();
            }
            catch
            {
                throw;
            }
        }

        internal OpenTypeDescriptor(string idName, byte[] fontData)
        {
            try
            {
                this.fontData = new FontData(fontData);
                if ((idName.Contains("XPS-Font-") && (this.fontData.name != null)) && (this.fontData.name.Name.Length != 0))
                {
                    string str = string.Empty;
                    if (idName.IndexOf('+') == 6)
                    {
                        str = idName.Substring(0, 6);
                    }
                    idName = str + "+" + this.fontData.name.Name;
                    if (this.fontData.name.Style.Length != 0)
                    {
                        idName = idName + "," + this.fontData.name.Style;
                    }
                    idName = idName.Replace(" ", "");
                }
                base.fontName = idName;
                this.Initialize();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public int CharCodeToGlyphIndex(char value)
        {
            int num4;
            try
            {
                CMap4 map = this.fontData.cmap.cmap4;
                int num = map.segCountX2 / 2;
                int index = 0;
                while (index < num)
                {
                    if (value <= map.endCount[index])
                    {
                        break;
                    }
                    index++;
                }
                if (value < map.startCount[index])
                {
                    return 0;
                }
                if (map.idRangeOffs[index] == 0)
                {
                    return ((value + map.idDelta[index]) & 0xffff);
                }
                int num3 = ((map.idRangeOffs[index] / 2) + (value - map.startCount[index])) - (num - index);
                if (map.glyphIdArray[num3] == 0)
                {
                    return 0;
                }
                num4 = (map.glyphIdArray[num3] + map.idDelta[index]) & 0xffff;
            }
            catch
            {
                throw;
            }
            return num4;
        }

        internal int DesignUnitsToPdf(double value) => 
            ((int) Math.Round((double) ((value * 1000.0) / ((double) this.fontData.head.unitsPerEm))));

        public int GlyphIndexToPdfWidth(int glyphIndex)
        {
            int num4;
            try
            {
                int numberOfHMetrics = this.fontData.hhea.numberOfHMetrics;
                int unitsPerEm = this.fontData.head.unitsPerEm;
                if (glyphIndex >= numberOfHMetrics)
                {
                    glyphIndex = numberOfHMetrics - 1;
                }
                int advanceWidth = this.fontData.hmtx.metrics[glyphIndex].advanceWidth;
                if (unitsPerEm == 0x3e8)
                {
                    return advanceWidth;
                }
                num4 = (advanceWidth * 0x3e8) / unitsPerEm;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return num4;
        }

        public int GlyphIndexToWidth(int glyphIndex)
        {
            int advanceWidth;
            try
            {
                int numberOfHMetrics = this.fontData.hhea.numberOfHMetrics;
                if (glyphIndex >= numberOfHMetrics)
                {
                    glyphIndex = numberOfHMetrics - 1;
                }
                advanceWidth = this.fontData.hmtx.metrics[glyphIndex].advanceWidth;
            }
            catch (Exception)
            {
                base.GetType();
                throw;
            }
            return advanceWidth;
        }

        private void Initialize()
        {
            base.italicAngle = this.fontData.post.italicAngle;
            base.xMin = this.fontData.head.xMin;
            base.yMin = this.fontData.head.yMin;
            base.xMax = this.fontData.head.xMax;
            base.yMax = this.fontData.head.yMax;
            base.underlinePosition = this.fontData.post.underlinePosition;
            base.underlineThickness = this.fontData.post.underlineThickness;
            base.strikeoutPosition = this.fontData.os2.yStrikeoutPosition;
            base.strikeoutSize = this.fontData.os2.yStrikeoutSize;
            base.stemV = 0;
            base.unitsPerEm = this.fontData.head.unitsPerEm;
            if (this.fontData.os2.sTypoAscender != 0)
            {
                base.ascender = this.fontData.os2.usWinAscent;
            }
            else
            {
                base.ascender = this.fontData.hhea.ascender;
            }
            if (this.fontData.os2.sTypoDescender != 0)
            {
                base.descender = this.fontData.os2.usWinDescent;
                base.descender = Math.Abs(base.descender) * Math.Sign(this.fontData.hhea.descender);
            }
            else
            {
                base.descender = this.fontData.hhea.descender;
            }
            base.leading = this.fontData.hhea.lineGap;
            if ((this.fontData.os2.version >= 2) && (this.fontData.os2.sCapHeight != 0))
            {
                base.capHeight = this.fontData.os2.sCapHeight;
            }
            else
            {
                base.capHeight = this.fontData.hhea.ascender;
            }
            if ((this.fontData.os2.version >= 2) && (this.fontData.os2.sxHeight != 0))
            {
                base.xHeight = this.fontData.os2.sxHeight;
            }
            else
            {
                base.xHeight = (int) (0.66f * base.ascender);
            }
            Encoding winAnsiEncoding = PdfEncoders.WinAnsiEncoding;
            Encoding unicode = Encoding.Unicode;
            byte[] bytes = new byte[0x100];
            bool symbol = this.fontData.cmap.symbol;
            this.widths = new int[0x100];
            for (int i = 0; i < 0x100; i++)
            {
                int num2;
                bytes[i] = (byte) i;
                char ch = (char) i;
                string str = winAnsiEncoding.GetString(bytes, i, 1);
                if ((str.Length != 0) && (str[0] != ch))
                {
                    ch = str[0];
                }
                if (symbol)
                {
                    num2 = i + (this.fontData.os2.usFirstCharIndex & 0xff00);
                    num2 = this.CharCodeToGlyphIndex((char) num2);
                }
                else
                {
                    num2 = this.CharCodeToGlyphIndex(ch);
                }
                this.widths[i] = this.GlyphIndexToPdfWidth(num2);
            }
        }

        public int PdfWidthFromCharCode(char ch)
        {
            int glyphIndex = this.CharCodeToGlyphIndex(ch);
            return this.GlyphIndexToPdfWidth(glyphIndex);
        }

        public override bool IsBoldFace =>
            ((this.fontData.os2.fsSelection & 0x20) != 0);

        public override bool IsItalicFace =>
            ((this.fontData.os2.fsSelection & 1) != 0);
    }
}

