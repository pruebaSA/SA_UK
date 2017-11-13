namespace PdfSharp.Drawing
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    public static class XFontWeights
    {
        internal static bool FontWeightStringToKnownWeight(string s, IFormatProvider provider, ref XFontWeight fontWeight)
        {
            int num;
            switch (s.ToLower())
            {
                case "thin":
                    fontWeight = Thin;
                    return true;

                case "extralight":
                    fontWeight = ExtraLight;
                    return true;

                case "ultralight":
                    fontWeight = UltraLight;
                    return true;

                case "light":
                    fontWeight = Light;
                    return true;

                case "normal":
                    fontWeight = Normal;
                    return true;

                case "regular":
                    fontWeight = Regular;
                    return true;

                case "medium":
                    fontWeight = Medium;
                    return true;

                case "semibold":
                    fontWeight = SemiBold;
                    return true;

                case "demibold":
                    fontWeight = DemiBold;
                    return true;

                case "bold":
                    fontWeight = Bold;
                    return true;

                case "extrabold":
                    fontWeight = ExtraBold;
                    return true;

                case "ultrabold":
                    fontWeight = UltraBold;
                    return true;

                case "heavy":
                    fontWeight = Heavy;
                    return true;

                case "black":
                    fontWeight = Black;
                    return true;

                case "extrablack":
                    fontWeight = ExtraBlack;
                    return true;

                case "ultrablack":
                    fontWeight = UltraBlack;
                    return true;
            }
            if (int.TryParse(s, NumberStyles.Integer, provider, out num))
            {
                fontWeight = new XFontWeight(num);
                return true;
            }
            return false;
        }

        internal static bool FontWeightToString(int weight, out string convertedValue)
        {
            switch (weight)
            {
                case 300:
                    convertedValue = "Light";
                    return true;

                case 400:
                    convertedValue = "Normal";
                    return true;

                case 500:
                    convertedValue = "Medium";
                    return true;

                case 100:
                    convertedValue = "Thin";
                    return true;

                case 200:
                    convertedValue = "ExtraLight";
                    return true;

                case 600:
                    convertedValue = "SemiBold";
                    return true;

                case 700:
                    convertedValue = "Bold";
                    return true;

                case 800:
                    convertedValue = "ExtraBold";
                    return true;

                case 900:
                    convertedValue = "Black";
                    return true;

                case 950:
                    convertedValue = "ExtraBlack";
                    return true;
            }
            convertedValue = null;
            return false;
        }

        public static XFontWeight Black =>
            new XFontWeight(900);

        public static XFontWeight Bold =>
            new XFontWeight(700);

        public static XFontWeight DemiBold =>
            new XFontWeight(600);

        public static XFontWeight ExtraBlack =>
            new XFontWeight(950);

        public static XFontWeight ExtraBold =>
            new XFontWeight(800);

        public static XFontWeight ExtraLight =>
            new XFontWeight(200);

        public static XFontWeight Heavy =>
            new XFontWeight(900);

        public static XFontWeight Light =>
            new XFontWeight(300);

        public static XFontWeight Medium =>
            new XFontWeight(500);

        public static XFontWeight Normal =>
            new XFontWeight(400);

        public static XFontWeight Regular =>
            new XFontWeight(400);

        public static XFontWeight SemiBold =>
            new XFontWeight(600);

        public static XFontWeight Thin =>
            new XFontWeight(100);

        public static XFontWeight UltraBlack =>
            new XFontWeight(950);

        public static XFontWeight UltraBold =>
            new XFontWeight(800);

        public static XFontWeight UltraLight =>
            new XFontWeight(200);
    }
}

