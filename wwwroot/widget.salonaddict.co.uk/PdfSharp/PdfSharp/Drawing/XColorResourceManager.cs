namespace PdfSharp.Drawing
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public class XColorResourceManager
    {
        internal static ColorResourceInfo[] colorInfos = new ColorResourceInfo[] { 
            new ColorResourceInfo(XKnownColor.Transparent, XColors.Transparent, 0xffffff, "Transparent", "Transparent"), new ColorResourceInfo(XKnownColor.Black, XColors.Black, 0xff000000, "Black", "Schwarz"), new ColorResourceInfo(XKnownColor.DarkSlateGray, XColors.DarkSlateGray, 0xff8fbc8f, "Darkslategray", "Dunkles Schiefergrau"), new ColorResourceInfo(XKnownColor.SlateGray, XColors.SlateGray, 0xff708090, "Slategray", "Schiefergrau"), new ColorResourceInfo(XKnownColor.LightSlateGray, XColors.LightSlateGray, 0xff778899, "Lightslategray", "Helles Schiefergrau"), new ColorResourceInfo(XKnownColor.LightSteelBlue, XColors.LightSteelBlue, 0xffb0c4de, "Lightsteelblue", "Helles Stahlblau"), new ColorResourceInfo(XKnownColor.DimGray, XColors.DimGray, 0xff696969, "Dimgray", "Gedecktes Grau"), new ColorResourceInfo(XKnownColor.Gray, XColors.Gray, 0xff808080, "Gray", "Grau"), new ColorResourceInfo(XKnownColor.DarkGray, XColors.DarkGray, 0xffa9a9a9, "Darkgray", "Dunkelgrau"), new ColorResourceInfo(XKnownColor.Silver, XColors.Silver, 0xffc0c0c0, "Silver", "Silber"), new ColorResourceInfo(XKnownColor.Gainsboro, XColors.Gainsboro, 0xffdcdcdc, "Gainsboro", "Helles Blaugrau"), new ColorResourceInfo(XKnownColor.WhiteSmoke, XColors.WhiteSmoke, 0xfff5f5f5, "Whitesmoke", "Rauchwei\x00df"), new ColorResourceInfo(XKnownColor.GhostWhite, XColors.GhostWhite, 0xfff8f8ff, "Ghostwhite", "Schattenwei\x00df"), new ColorResourceInfo(XKnownColor.White, XColors.White, uint.MaxValue, "White", "Wei\x00df"), new ColorResourceInfo(XKnownColor.Snow, XColors.Snow, 0xfffffafa, "Snow", "Schneewei\x00df"), new ColorResourceInfo(XKnownColor.Ivory, XColors.Ivory, 0xfffffff0, "Ivory", "Elfenbein"),
            new ColorResourceInfo(XKnownColor.FloralWhite, XColors.FloralWhite, 0xfffffaf0, "Floralwhite", "Bl\x00fctenwei\x00df"), new ColorResourceInfo(XKnownColor.SeaShell, XColors.SeaShell, 0xfffff5ee, "Seashell", "Muschel"), new ColorResourceInfo(XKnownColor.OldLace, XColors.OldLace, 0xfffdf5e6, "Oldlace", "Altwei\x00df"), new ColorResourceInfo(XKnownColor.Linen, XColors.Linen, 0xfffaf0e6, "Linen", "Leinen"), new ColorResourceInfo(XKnownColor.AntiqueWhite, XColors.AntiqueWhite, 0xfffaebd7, "Antiquewhite", "Antikes Wei\x00df"), new ColorResourceInfo(XKnownColor.BlanchedAlmond, XColors.BlanchedAlmond, 0xffffebcd, "Blanchedalmond", "Mandelwei\x00df"), new ColorResourceInfo(XKnownColor.PapayaWhip, XColors.PapayaWhip, 0xffffefd5, "Papayawhip", "Papayacreme"), new ColorResourceInfo(XKnownColor.Beige, XColors.Beige, 0xfff5f5dc, "Beige", "Beige"), new ColorResourceInfo(XKnownColor.Cornsilk, XColors.Cornsilk, 0xfffff8dc, "Cornsilk", "Mais"), new ColorResourceInfo(XKnownColor.LightGoldenrodYellow, XColors.LightGoldenrodYellow, 0xfffafad2, "Lightgoldenrodyellow", "Helles Goldgelb"), new ColorResourceInfo(XKnownColor.LightYellow, XColors.LightYellow, 0xffffffe0, "Lightyellow", "Hellgelb"), new ColorResourceInfo(XKnownColor.LemonChiffon, XColors.LemonChiffon, 0xfffffacd, "Lemonchiffon", "Pastellgelb"), new ColorResourceInfo(XKnownColor.PaleGoldenrod, XColors.PaleGoldenrod, 0xffeee8aa, "Palegoldenrod", "Blasses Goldgelb"), new ColorResourceInfo(XKnownColor.Khaki, XColors.Khaki, 0xfff0e68c, "Khaki", "Khaki"), new ColorResourceInfo(XKnownColor.Yellow, XColors.Yellow, 0xffffff00, "Yellow", "Gelb"), new ColorResourceInfo(XKnownColor.Gold, XColors.Gold, 0xffffd700, "Gold", "Gold"),
            new ColorResourceInfo(XKnownColor.Orange, XColors.Orange, 0xffffa500, "Orange", "Orange"), new ColorResourceInfo(XKnownColor.DarkOrange, XColors.DarkOrange, 0xffff8c00, "Darkorange", "Dunkles Orange"), new ColorResourceInfo(XKnownColor.Goldenrod, XColors.Goldenrod, 0xffdaa520, "Goldenrod", "Goldgelb"), new ColorResourceInfo(XKnownColor.DarkGoldenrod, XColors.DarkGoldenrod, 0xffb8860b, "Darkgoldenrod", "Dunkles Goldgelb"), new ColorResourceInfo(XKnownColor.Peru, XColors.Peru, 0xffcd853f, "Peru", "Peru"), new ColorResourceInfo(XKnownColor.Chocolate, XColors.Chocolate, 0xffd2691e, "Chocolate", "Schokolade"), new ColorResourceInfo(XKnownColor.SaddleBrown, XColors.SaddleBrown, 0xff8b4513, "Saddlebrown", "Sattelbraun"), new ColorResourceInfo(XKnownColor.Sienna, XColors.Sienna, 0xffa0522d, "Sienna", "Ocker"), new ColorResourceInfo(XKnownColor.Brown, XColors.Brown, 0xffa52a2a, "Brown", "Braun"), new ColorResourceInfo(XKnownColor.DarkRed, XColors.DarkRed, 0xff8b0000, "Darkred", "Dunkelrot"), new ColorResourceInfo(XKnownColor.Maroon, XColors.Maroon, 0xff800000, "Maroon", "Kastanienbraun"), new ColorResourceInfo(XKnownColor.PaleTurquoise, XColors.PaleTurquoise, 0xffafeeee, "Paleturquoise", "Blasses T\x00fcrkis"), new ColorResourceInfo(XKnownColor.Firebrick, XColors.Firebrick, 0xffb22222, "Firebrick", "Ziegel"), new ColorResourceInfo(XKnownColor.IndianRed, XColors.IndianRed, 0xffcd5c5c, "Indianred", "Indischrot"), new ColorResourceInfo(XKnownColor.Crimson, XColors.Crimson, 0xffdc143c, "Crimson", "Karmesinrot"), new ColorResourceInfo(XKnownColor.Red, XColors.Red, 0xffff0000, "Red", "Rot"),
            new ColorResourceInfo(XKnownColor.OrangeRed, XColors.OrangeRed, 0xffff4500, "Orangered", "Orangerot"), new ColorResourceInfo(XKnownColor.Tomato, XColors.Tomato, 0xffff6347, "Tomato", "Tomate"), new ColorResourceInfo(XKnownColor.Coral, XColors.Coral, 0xffff7f50, "Coral", "Koralle"), new ColorResourceInfo(XKnownColor.Salmon, XColors.Salmon, 0xfffa8072, "Salmon", "Lachs"), new ColorResourceInfo(XKnownColor.LightCoral, XColors.LightCoral, 0xfff08080, "Lightcoral", "Helles Korallenrot"), new ColorResourceInfo(XKnownColor.DarkSalmon, XColors.DarkSalmon, 0xffe9967a, "Darksalmon", "Dunkles Lachs"), new ColorResourceInfo(XKnownColor.LightSalmon, XColors.LightSalmon, 0xffffa07a, "Lightsalmon", "Helles Lachs"), new ColorResourceInfo(XKnownColor.SandyBrown, XColors.SandyBrown, 0xfff4a460, "Sandybrown", "Sandbraun"), new ColorResourceInfo(XKnownColor.RosyBrown, XColors.RosyBrown, 0xffbc8f8f, "Rosybrown", "Rotbraun"), new ColorResourceInfo(XKnownColor.Tan, XColors.Tan, 0xffd2b48c, "Tan", "Gelbbraun"), new ColorResourceInfo(XKnownColor.BurlyWood, XColors.BurlyWood, 0xffdeb887, "Burlywood", "Kr\x00e4ftiges Sandbraun"), new ColorResourceInfo(XKnownColor.Wheat, XColors.Wheat, 0xfff5deb3, "Wheat", "Weizen"), new ColorResourceInfo(XKnownColor.PeachPuff, XColors.PeachPuff, 0xffffdab9, "Peachpuff", "Pfirsich"), new ColorResourceInfo(XKnownColor.NavajoWhite, XColors.NavajoWhite, 0xffffdead, "Navajowhite", "Orangewei\x00df"), new ColorResourceInfo(XKnownColor.Bisque, XColors.Bisque, 0xffffe4c4, "Bisque", "Blasses Rotbraun"), new ColorResourceInfo(XKnownColor.Moccasin, XColors.Moccasin, 0xffffe4b5, "Moccasin", "Mokassin"),
            new ColorResourceInfo(XKnownColor.LavenderBlush, XColors.LavenderBlush, 0xfffff0f5, "Lavenderblush", "Roter Lavendel"), new ColorResourceInfo(XKnownColor.MistyRose, XColors.MistyRose, 0xffffe4e1, "Mistyrose", "Altrosa"), new ColorResourceInfo(XKnownColor.Pink, XColors.Pink, 0xffffc0cb, "Pink", "Rosa"), new ColorResourceInfo(XKnownColor.LightPink, XColors.LightPink, 0xffffb6c1, "Lightpink", "Hellrosa"), new ColorResourceInfo(XKnownColor.HotPink, XColors.HotPink, 0xffff69b4, "Hotpink", "Leuchtendes Rosa"), new ColorResourceInfo(XKnownColor.Magenta, XColors.Magenta, 0xffff00ff, "Magenta", "Magentarot"), new ColorResourceInfo(XKnownColor.DeepPink, XColors.DeepPink, 0xffff1493, "Deeppink", "Tiefrosa"), new ColorResourceInfo(XKnownColor.MediumVioletRed, XColors.MediumVioletRed, 0xffc71585, "Mediumvioletred", "Mittleres Violettrot"), new ColorResourceInfo(XKnownColor.PaleVioletRed, XColors.PaleVioletRed, 0xffdb7093, "Palevioletred", "Blasses Violettrot"), new ColorResourceInfo(XKnownColor.Plum, XColors.Plum, 0xffdda0dd, "Plum", "Pflaume"), new ColorResourceInfo(XKnownColor.Thistle, XColors.Thistle, 0xffd8bfd8, "Thistle", "Distel"), new ColorResourceInfo(XKnownColor.Lavender, XColors.Lavender, 0xffe6e6fa, "Lavender", "Lavendel"), new ColorResourceInfo(XKnownColor.Violet, XColors.Violet, 0xffee82ee, "Violet", "Violett"), new ColorResourceInfo(XKnownColor.Orchid, XColors.Orchid, 0xffda70d6, "Orchid", "Orchidee"), new ColorResourceInfo(XKnownColor.DarkMagenta, XColors.DarkMagenta, 0xff8b008b, "Darkmagenta", "Dunkles Magentarot"), new ColorResourceInfo(XKnownColor.Purple, XColors.Purple, 0xff800080, "Purple", "Violett"),
            new ColorResourceInfo(XKnownColor.Indigo, XColors.Indigo, 0xff4b0082, "Indigo", "Indigo"), new ColorResourceInfo(XKnownColor.BlueViolet, XColors.BlueViolet, 0xff8a2be2, "Blueviolet", "Blauviolett"), new ColorResourceInfo(XKnownColor.DarkViolet, XColors.DarkViolet, 0xff9400d3, "Darkviolet", "Dunkles Violett"), new ColorResourceInfo(XKnownColor.DarkOrchid, XColors.DarkOrchid, 0xff9932cc, "Darkorchid", "Dunkle Orchidee"), new ColorResourceInfo(XKnownColor.MediumPurple, XColors.MediumPurple, 0xff9370db, "Mediumpurple", "Mittleres Violett"), new ColorResourceInfo(XKnownColor.MediumOrchid, XColors.MediumOrchid, 0xffba55d3, "Mediumorchid", "Mittlere Orchidee"), new ColorResourceInfo(XKnownColor.MediumSlateBlue, XColors.MediumSlateBlue, 0xff7b68ee, "Mediumslateblue", "Mittleres Schieferblau"), new ColorResourceInfo(XKnownColor.SlateBlue, XColors.SlateBlue, 0xff6a5acd, "Slateblue", "Schieferblau"), new ColorResourceInfo(XKnownColor.DarkSlateBlue, XColors.DarkSlateBlue, 0xff483d8b, "Darkslateblue", "Dunkles Schiefergrau"), new ColorResourceInfo(XKnownColor.MidnightBlue, XColors.MidnightBlue, 0xff191970, "Midnightblue", "Mitternachtsblau"), new ColorResourceInfo(XKnownColor.Navy, XColors.Navy, 0xff000080, "Navy", "Marineblau"), new ColorResourceInfo(XKnownColor.DarkBlue, XColors.DarkBlue, 0xff00008b, "Darkblue", "Dunkelblau"), new ColorResourceInfo(XKnownColor.LightGray, XColors.LightGray, 0xffd3d3d3, "Lightgray", "Hellgrau"), new ColorResourceInfo(XKnownColor.MediumBlue, XColors.MediumBlue, 0xff0000cd, "Mediumblue", "Mittelblau"), new ColorResourceInfo(XKnownColor.Blue, XColors.Blue, 0xff0000ff, "Blue", "Blau"), new ColorResourceInfo(XKnownColor.RoyalBlue, XColors.RoyalBlue, 0xff4169e1, "Royalblue", "K\x00f6nigsblau"),
            new ColorResourceInfo(XKnownColor.SteelBlue, XColors.SteelBlue, 0xff4682b4, "Steelblue", "Stahlblau"), new ColorResourceInfo(XKnownColor.CornflowerBlue, XColors.CornflowerBlue, 0xff6495ed, "Cornflowerblue", "Kornblumenblau"), new ColorResourceInfo(XKnownColor.DodgerBlue, XColors.DodgerBlue, 0xff1e90ff, "Dodgerblue", "Dodger-Blau"), new ColorResourceInfo(XKnownColor.DeepSkyBlue, XColors.DeepSkyBlue, 0xff00bfff, "Deepskyblue", "Tiefes Himmelblau"), new ColorResourceInfo(XKnownColor.LightSkyBlue, XColors.LightSkyBlue, 0xff87cefa, "Lightskyblue", "Helles Himmelblau"), new ColorResourceInfo(XKnownColor.SkyBlue, XColors.SkyBlue, 0xff87ceeb, "Skyblue", "Himmelblau"), new ColorResourceInfo(XKnownColor.LightBlue, XColors.LightBlue, 0xffadd8e6, "Lightblue", "Hellblau"), new ColorResourceInfo(XKnownColor.Cyan, XColors.Cyan, 0xff00ffff, "Cyan", "Zyan"), new ColorResourceInfo(XKnownColor.PowderBlue, XColors.PowderBlue, 0xffb0e0e6, "Powderblue", "Taubenblau"), new ColorResourceInfo(XKnownColor.LightCyan, XColors.LightCyan, 0xffe0ffff, "Lightcyan", "Helles Cyanblau"), new ColorResourceInfo(XKnownColor.AliceBlue, XColors.AliceBlue, 0xffa0ce00, "Aliceblue", "Aliceblau"), new ColorResourceInfo(XKnownColor.Azure, XColors.Azure, 0xfff0ffff, "Azure", "Himmelblau"), new ColorResourceInfo(XKnownColor.MintCream, XColors.MintCream, 0xfff5fffa, "Mintcream", "Helles Pfefferminzgr\x00fcn"), new ColorResourceInfo(XKnownColor.Honeydew, XColors.Honeydew, 0xfff0fff0, "Honeydew", "Honigmelone"), new ColorResourceInfo(XKnownColor.Aquamarine, XColors.Aquamarine, 0xff7fffd4, "Aquamarine", "Aquamarinblau"), new ColorResourceInfo(XKnownColor.Turquoise, XColors.Turquoise, 0xff40e0d0, "Turquoise", "T\x00fcrkis"),
            new ColorResourceInfo(XKnownColor.MediumTurquoise, XColors.MediumTurquoise, 0xff48d1cc, "Mediumturqoise", "Mittleres T\x00fcrkis"), new ColorResourceInfo(XKnownColor.DarkTurquoise, XColors.DarkTurquoise, 0xff00ced1, "Darkturquoise", "Dunkles T\x00fcrkis"), new ColorResourceInfo(XKnownColor.MediumAquamarine, XColors.MediumAquamarine, 0xff66cdaa, "Mediumaquamarine", "Mittleres Aquamarinblau"), new ColorResourceInfo(XKnownColor.LightSeaGreen, XColors.LightSeaGreen, 0xff20b2aa, "Lightseagreen", "Helles Seegr\x00fcn"), new ColorResourceInfo(XKnownColor.DarkCyan, XColors.DarkCyan, 0xff008b8b, "Darkcyan", "Dunkles Zyanblau"), new ColorResourceInfo(XKnownColor.Teal, XColors.Teal, 0xff008080, "Teal", "Entenblau"), new ColorResourceInfo(XKnownColor.CadetBlue, XColors.CadetBlue, 0xff5f9ea0, "Cadetblue", "Kadettblau"), new ColorResourceInfo(XKnownColor.MediumSeaGreen, XColors.MediumSeaGreen, 0xff3cb371, "Mediumseagreen", "Mittleres Seegr\x00fcn"), new ColorResourceInfo(XKnownColor.DarkSeaGreen, XColors.DarkSeaGreen, 0xff8fbc8f, "Darkseagreen", "Dunkles Seegr\x00fcn"), new ColorResourceInfo(XKnownColor.LightGreen, XColors.LightGreen, 0xff90ee90, "Lightgreen", "Hellgr\x00fcn"), new ColorResourceInfo(XKnownColor.PaleGreen, XColors.PaleGreen, 0xff98fb98, "Palegreen", "Blassgr\x00fcn"), new ColorResourceInfo(XKnownColor.MediumSpringGreen, XColors.MediumSpringGreen, 0xff00fa9a, "Mediumspringgreen", "Mittleres Fr\x00fchlingsgr\x00fcn"), new ColorResourceInfo(XKnownColor.SpringGreen, XColors.SpringGreen, 0xff00ff7f, "Springgreen", "Fr\x00fchlingsgr\x00fcn"), new ColorResourceInfo(XKnownColor.Lime, XColors.Lime, 0xff00ff00, "Lime", "Zitronengr\x00fcn"), new ColorResourceInfo(XKnownColor.LimeGreen, XColors.LimeGreen, 0xff32cd32, "Limegreen", "Gelbgr\x00fcn"), new ColorResourceInfo(XKnownColor.SeaGreen, XColors.SeaGreen, 0xff2e8b57, "Seagreen", "Seegr\x00fcn"),
            new ColorResourceInfo(XKnownColor.ForestGreen, XColors.ForestGreen, 0xff228b22, "Forestgreen", "Waldgr\x00fcn"), new ColorResourceInfo(XKnownColor.Green, XColors.Green, 0xff008000, "Green", "Gr\x00fcn"), new ColorResourceInfo(XKnownColor.LawnGreen, XColors.LawnGreen, 0xff008000, "LawnGreen", "Grasgr\x00fcn"), new ColorResourceInfo(XKnownColor.DarkGreen, XColors.DarkGreen, 0xff006400, "Darkgreen", "Dunkelgr\x00fcn"), new ColorResourceInfo(XKnownColor.OliveDrab, XColors.OliveDrab, 0xff6b8e23, "Olivedrab", "Reife Olive"), new ColorResourceInfo(XKnownColor.DarkOliveGreen, XColors.DarkOliveGreen, 0xff556b2f, "Darkolivegreen", "Dunkles Olivgr\x00fcn"), new ColorResourceInfo(XKnownColor.Olive, XColors.Olive, 0xff808000, "Olive", "Olivgr\x00fcn"), new ColorResourceInfo(XKnownColor.DarkKhaki, XColors.DarkKhaki, 0xffbdb76b, "Darkkhaki", "Dunkles Khaki"), new ColorResourceInfo(XKnownColor.YellowGreen, XColors.YellowGreen, 0xff9acd32, "Yellowgreen", "Gelbgr\x00fcn"), new ColorResourceInfo(XKnownColor.Chartreuse, XColors.Chartreuse, 0xff7fff00, "Chartreuse", "Hellgr\x00fcn"), new ColorResourceInfo(XKnownColor.GreenYellow, XColors.GreenYellow, 0xffadff2f, "Greenyellow", "Gr\x00fcngelb")
        };
        private CultureInfo cultureInfo;

        public XColorResourceManager() : this(Thread.CurrentThread.CurrentUICulture)
        {
        }

        public XColorResourceManager(CultureInfo cultureInfo)
        {
            this.cultureInfo = cultureInfo;
        }

        private static ColorResourceInfo GetColorInfo(XKnownColor knownColor)
        {
            for (int i = 0; i < colorInfos.Length; i++)
            {
                ColorResourceInfo info = colorInfos[i];
                if (info.knownColor == knownColor)
                {
                    return info;
                }
            }
            throw new InvalidEnumArgumentException("Enum is not an XKnownColor.");
        }

        public static XKnownColor GetKnownColor(uint argb)
        {
            XKnownColor knownColor = XKnownColorTable.GetKnownColor(argb);
            if (knownColor == ~XKnownColor.AliceBlue)
            {
                throw new ArgumentException("The argument is not a known color", "argb");
            }
            return knownColor;
        }

        public static XKnownColor[] GetKnownColors(bool includeTransparent)
        {
            int length = colorInfos.Length;
            XKnownColor[] colorArray = new XKnownColor[length - (includeTransparent ? 0 : 1)];
            int index = includeTransparent ? 0 : 1;
            for (int i = 0; index < length; i++)
            {
                colorArray[i] = colorInfos[index].knownColor;
                index++;
            }
            return colorArray;
        }

        public string ToColorName(XColor color)
        {
            if (color.IsKnownColor)
            {
                return this.ToColorName(XKnownColorTable.GetKnownColor(color.Argb));
            }
            return $"{((int) (255.0 * color.A))}, {color.R}, {color.G}, {color.B}";
        }

        public string ToColorName(XKnownColor knownColor)
        {
            ColorResourceInfo colorInfo = GetColorInfo(knownColor);
            if (this.cultureInfo.TwoLetterISOLanguageName == "de")
            {
                return colorInfo.NameDE;
            }
            return colorInfo.Name;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ColorResourceInfo
        {
            public XKnownColor knownColor;
            public XColor color;
            public uint Argb;
            public string Name;
            public string NameDE;
            public ColorResourceInfo(XKnownColor knownColor, XColor color, uint argb, string name, string nameDE)
            {
                this.knownColor = knownColor;
                this.color = color;
                this.Argb = argb;
                this.Name = name;
                this.NameDE = nameDE;
            }
        }
    }
}

