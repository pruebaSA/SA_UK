namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Internal;
    using System;

    public sealed class PdfShading : PdfDictionary
    {
        public PdfShading(PdfDocument document) : base(document)
        {
        }

        public void SetupFromBrush(XLinearGradientBrush brush)
        {
            if (brush == null)
            {
                throw new ArgumentNullException("brush");
            }
            PdfColorMode colorMode = base.document.Options.ColorMode;
            XColor color = ColorSpaceHelper.EnsureColorMode(colorMode, brush.color1);
            XColor color2 = ColorSpaceHelper.EnsureColorMode(colorMode, brush.color2);
            PdfDictionary dictionary = new PdfDictionary();
            base.Elements["/ShadingType"] = new PdfInteger(2);
            if (colorMode != PdfColorMode.Cmyk)
            {
                base.Elements["/ColorSpace"] = new PdfName("/DeviceRGB");
            }
            else
            {
                base.Elements["/ColorSpace"] = new PdfName("/DeviceCMYK");
            }
            double x = 0.0;
            double y = 0.0;
            double num3 = 0.0;
            double num4 = 0.0;
            if (brush.useRect)
            {
                switch (brush.linearGradientMode)
                {
                    case XLinearGradientMode.Horizontal:
                        x = brush.rect.x;
                        y = brush.rect.y;
                        num3 = brush.rect.x + brush.rect.width;
                        num4 = brush.rect.y;
                        break;

                    case XLinearGradientMode.Vertical:
                        x = brush.rect.x;
                        y = brush.rect.y;
                        num3 = brush.rect.x;
                        num4 = brush.rect.y + brush.rect.height;
                        break;

                    case XLinearGradientMode.ForwardDiagonal:
                        x = brush.rect.x;
                        y = brush.rect.y;
                        num3 = brush.rect.x + brush.rect.width;
                        num4 = brush.rect.y + brush.rect.height;
                        break;

                    case XLinearGradientMode.BackwardDiagonal:
                        x = brush.rect.x + brush.rect.width;
                        y = brush.rect.y;
                        num3 = brush.rect.x;
                        num4 = brush.rect.y + brush.rect.height;
                        break;
                }
            }
            else
            {
                x = brush.point1.x;
                y = brush.point1.y;
                num3 = brush.point2.x;
                num4 = brush.point2.y;
            }
            base.Elements["/Coords"] = new PdfLiteral("[{0:0.###} {1:0.###} {2:0.###} {3:0.###}]", new object[] { x, y, num3, num4 });
            base.Elements["/Function"] = dictionary;
            string str = "[" + PdfEncoders.ToString(color, colorMode) + "]";
            string str2 = "[" + PdfEncoders.ToString(color2, colorMode) + "]";
            dictionary.Elements["/FunctionType"] = new PdfInteger(2);
            dictionary.Elements["/C0"] = new PdfLiteral(str);
            dictionary.Elements["/C1"] = new PdfLiteral(str2);
            dictionary.Elements["/Domain"] = new PdfLiteral("[0 1]");
            dictionary.Elements["/N"] = new PdfInteger(1);
        }

        internal override DictionaryMeta Meta =>
            Keys.Meta;

        internal sealed class Keys : KeysBase
        {
            [KeyInfo(KeyType.Optional | KeyType.Boolean)]
            public const string AntiAlias = "/AntiAlias";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Background = "/Background";
            [KeyInfo(KeyType.Optional | KeyType.Rectangle)]
            public const string BBox = "/BBox";
            [KeyInfo(KeyType.Required | KeyType.NameOrArray)]
            public const string ColorSpace = "/ColorSpace";
            [KeyInfo(KeyType.Required | KeyType.Array)]
            public const string Coords = "/Coords";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Domain = "/Domain";
            [KeyInfo(KeyType.Optional | KeyType.Array)]
            public const string Extend = "/Extend";
            [KeyInfo(KeyType.Required | KeyType.Function)]
            public const string Function = "/Function";
            private static DictionaryMeta meta;
            [KeyInfo(KeyType.Required | KeyType.Integer)]
            public const string ShadingType = "/ShadingType";

            internal static DictionaryMeta Meta
            {
                get
                {
                    if (meta == null)
                    {
                        meta = KeysBase.CreateMeta(typeof(PdfShading.Keys));
                    }
                    return meta;
                }
            }
        }
    }
}

