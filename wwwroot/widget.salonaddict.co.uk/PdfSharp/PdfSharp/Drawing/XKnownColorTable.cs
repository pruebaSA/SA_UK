namespace PdfSharp.Drawing
{
    using System;

    internal class XKnownColorTable
    {
        internal static uint[] colorTable;

        public static XKnownColor GetKnownColor(uint argb)
        {
            for (int i = 0; i < colorTable.Length; i++)
            {
                if (colorTable[i] == argb)
                {
                    return (XKnownColor) i;
                }
            }
            return ~XKnownColor.AliceBlue;
        }

        private static void InitColorTable()
        {
            colorTable = new uint[] { 
                0xfff0f8ff, 0xfffaebd7, 0xff00ffff, 0xff7fffd4, 0xfff0ffff, 0xfff5f5dc, 0xffffe4c4, 0xff000000, 0xffffebcd, 0xff0000ff, 0xff8a2be2, 0xffa52a2a, 0xffdeb887, 0xff5f9ea0, 0xff7fff00, 0xffd2691e,
                0xffff7f50, 0xff6495ed, 0xfffff8dc, 0xffdc143c, 0xff00ffff, 0xff00008b, 0xff008b8b, 0xffb8860b, 0xffa9a9a9, 0xff006400, 0xffbdb76b, 0xff8b008b, 0xff556b2f, 0xffff8c00, 0xff9932cc, 0xff8b0000,
                0xffe9967a, 0xff8fbc8b, 0xff483d8b, 0xff2f4f4f, 0xff00ced1, 0xff9400d3, 0xffff1493, 0xff00bfff, 0xff696969, 0xff1e90ff, 0xffb22222, 0xfffffaf0, 0xff228b22, 0xffff00ff, 0xffdcdcdc, 0xfff8f8ff,
                0xffffd700, 0xffdaa520, 0xff808080, 0xff008000, 0xffadff2f, 0xfff0fff0, 0xffff69b4, 0xffcd5c5c, 0xff4b0082, 0xfffffff0, 0xfff0e68c, 0xffe6e6fa, 0xfffff0f5, 0xff7cfc00, 0xfffffacd, 0xffadd8e6,
                0xfff08080, 0xffe0ffff, 0xfffafad2, 0xffd3d3d3, 0xff90ee90, 0xffffb6c1, 0xffffa07a, 0xff20b2aa, 0xff87cefa, 0xff778899, 0xffb0c4de, 0xffffffe0, 0xff00ff00, 0xff32cd32, 0xfffaf0e6, 0xffff00ff,
                0xff800000, 0xff66cdaa, 0xff0000cd, 0xffba55d3, 0xff9370db, 0xff3cb371, 0xff7b68ee, 0xff00fa9a, 0xff48d1cc, 0xffc71585, 0xff191970, 0xfff5fffa, 0xffffe4e1, 0xffffe4b5, 0xffffdead, 0xff000080,
                0xfffdf5e6, 0xff808000, 0xff6b8e23, 0xffffa500, 0xffff4500, 0xffda70d6, 0xffeee8aa, 0xff98fb98, 0xffafeeee, 0xffdb7093, 0xffffefd5, 0xffffdab9, 0xffcd853f, 0xffffc0cb, 0xffdda0dd, 0xffb0e0e6,
                0xff800080, 0xffff0000, 0xffbc8f8f, 0xff4169e1, 0xff8b4513, 0xfffa8072, 0xfff4a460, 0xff2e8b57, 0xfffff5ee, 0xffa0522d, 0xffc0c0c0, 0xff87ceeb, 0xff6a5acd, 0xff708090, 0xfffffafa, 0xff00ff7f,
                0xff4682b4, 0xffd2b48c, 0xff008080, 0xffd8bfd8, 0xffff6347, 0xffffff, 0xff40e0d0, 0xffee82ee, 0xfff5deb3, uint.MaxValue, 0xfff5f5f5, 0xffffff00, 0xff9acd32
            };
        }

        public static bool IsKnownColor(uint argb)
        {
            for (int i = 0; i < colorTable.Length; i++)
            {
                if (colorTable[i] == argb)
                {
                    return true;
                }
            }
            return false;
        }

        public static uint KnownColorToArgb(XKnownColor color)
        {
            if (colorTable == null)
            {
                InitColorTable();
            }
            if (color <= XKnownColor.YellowGreen)
            {
                return colorTable[(int) color];
            }
            return 0;
        }
    }
}

