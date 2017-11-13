namespace PdfSharp.Drawing
{
    using PdfSharp;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), DebuggerDisplay("(A={A}, R={R}, G={G}, B={B} C={C}, M={M}, Y={Y}, K={K})")]
    public struct XColor
    {
        public static XColor Empty;
        private XColorSpace cs;
        private float a;
        private byte r;
        private byte g;
        private byte b;
        private float c;
        private float m;
        private float y;
        private float k;
        private float gs;
        private XColor(uint argb)
        {
            this.cs = XColorSpace.Rgb;
            this.a = ((float) ((byte) ((argb >> 0x18) & 0xff))) / 255f;
            this.r = (byte) ((argb >> 0x10) & 0xff);
            this.g = (byte) ((argb >> 8) & 0xff);
            this.b = (byte) (argb & 0xff);
            this.c = 0f;
            this.m = 0f;
            this.y = 0f;
            this.k = 0f;
            this.gs = 0f;
            this.RgbChanged();
            this.cs.GetType();
        }

        private XColor(byte alpha, byte red, byte green, byte blue)
        {
            this.cs = XColorSpace.Rgb;
            this.a = ((float) alpha) / 255f;
            this.r = red;
            this.g = green;
            this.b = blue;
            this.c = 0f;
            this.m = 0f;
            this.y = 0f;
            this.k = 0f;
            this.gs = 0f;
            this.RgbChanged();
            this.cs.GetType();
        }

        private XColor(double alpha, double cyan, double magenta, double yellow, double black)
        {
            this.cs = XColorSpace.Cmyk;
            this.a = (alpha > 1.0) ? ((float) 1.0) : ((alpha < 0.0) ? ((float) 0.0) : ((float) alpha));
            this.c = (cyan > 1.0) ? ((float) 1.0) : ((cyan < 0.0) ? ((float) 0.0) : ((float) cyan));
            this.m = (magenta > 1.0) ? ((float) 1.0) : ((magenta < 0.0) ? ((float) 0.0) : ((float) magenta));
            this.y = (yellow > 1.0) ? ((float) 1.0) : ((yellow < 0.0) ? ((float) 0.0) : ((float) yellow));
            this.k = (black > 1.0) ? ((float) 1.0) : ((black < 0.0) ? ((float) 0.0) : ((float) black));
            this.r = 0;
            this.g = 0;
            this.b = 0;
            this.gs = 0f;
            this.CmykChanged();
        }

        private XColor(double cyan, double magenta, double yellow, double black) : this(1.0, cyan, magenta, yellow, black)
        {
        }

        private XColor(double gray)
        {
            this.cs = XColorSpace.GrayScale;
            if (gray < 0.0)
            {
                this.gs = 0f;
            }
            else if (gray > 1.0)
            {
                this.gs = 1f;
            }
            this.gs = (float) gray;
            this.a = 1f;
            this.r = 0;
            this.g = 0;
            this.b = 0;
            this.c = 0f;
            this.m = 0f;
            this.y = 0f;
            this.k = 0f;
            this.GrayChanged();
        }

        private XColor(Color color) : this(color.A, color.R, color.G, color.B)
        {
        }

        private XColor(KnownColor knownColor) : this(Color.FromKnownColor(knownColor))
        {
        }

        internal XColor(XKnownColor knownColor) : this(XKnownColorTable.KnownColorToArgb(knownColor))
        {
        }

        public static XColor FromArgb(int argb) => 
            new XColor((byte) (argb >> 0x18), (byte) (argb >> 0x10), (byte) (argb >> 8), (byte) argb);

        public static XColor FromArgb(uint argb) => 
            new XColor((byte) (argb >> 0x18), (byte) (argb >> 0x10), (byte) (argb >> 8), (byte) argb);

        public static XColor FromArgb(int red, int green, int blue)
        {
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new XColor(0xff, (byte) red, (byte) green, (byte) blue);
        }

        public static XColor FromArgb(int alpha, int red, int green, int blue)
        {
            CheckByte(alpha, "alpha");
            CheckByte(red, "red");
            CheckByte(green, "green");
            CheckByte(blue, "blue");
            return new XColor((byte) alpha, (byte) red, (byte) green, (byte) blue);
        }

        public static XColor FromArgb(Color color) => 
            new XColor(color);

        public static XColor FromArgb(int alpha, XColor color)
        {
            color.A = ((double) ((byte) alpha)) / 255.0;
            return color;
        }

        public static XColor FromArgb(int alpha, Color color) => 
            new XColor((double) alpha, (double) color.R, (double) color.G, (double) color.B);

        public static XColor FromCmyk(double cyan, double magenta, double yellow, double black) => 
            new XColor(cyan, magenta, yellow, black);

        public static XColor FromCmyk(double alpha, double cyan, double magenta, double yellow, double black) => 
            new XColor(alpha, cyan, magenta, yellow, black);

        public static XColor FromGrayScale(double grayScale) => 
            new XColor(grayScale);

        public static XColor FromKnownColor(XKnownColor color) => 
            new XColor(color);

        public static XColor FromKnownColor(KnownColor color) => 
            new XColor(color);

        public static XColor FromName(string name)
        {
            try
            {
                return new XColor((KnownColor) Enum.Parse(typeof(KnownColor), name, true));
            }
            catch
            {
            }
            return Empty;
        }

        public XColorSpace ColorSpace
        {
            get => 
                this.cs;
            set
            {
                if (!Enum.IsDefined(typeof(XColorSpace), value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(XColorSpace));
                }
                this.cs = value;
            }
        }
        public bool IsEmpty =>
            (this == Empty);
        public static implicit operator XColor(Color color) => 
            new XColor(color);

        public Color ToGdiColor() => 
            Color.FromArgb((int) (this.a * 255f), this.r, this.g, this.b);

        public override bool Equals(object obj)
        {
            if (obj is XColor)
            {
                XColor color = (XColor) obj;
                if ((((this.r == color.r) && (this.g == color.g)) && ((this.b == color.b) && (this.c == color.c))) && (((this.m == color.m) && (this.y == color.y)) && ((this.k == color.k) && (this.gs == color.gs))))
                {
                    return (this.a == color.a);
                }
            }
            return false;
        }

        public override int GetHashCode() => 
            (((((byte) (this.a * 255f)) ^ this.r) ^ this.g) ^ this.b);

        public static bool operator ==(XColor left, XColor right) => 
            (((((left.r == right.r) && (left.g == right.g)) && ((left.b == right.b) && (left.c == right.c))) && (((left.m == right.m) && (left.y == right.y)) && ((left.k == right.k) && (left.gs == right.gs)))) && (left.a == right.a));

        public static bool operator !=(XColor left, XColor right) => 
            !(left == right);

        public bool IsKnownColor =>
            XKnownColorTable.IsKnownColor(this.Argb);
        public double GetHue()
        {
            if ((this.r == this.g) && (this.g == this.b))
            {
                return 0.0;
            }
            double num = ((double) this.r) / 255.0;
            double num2 = ((double) this.g) / 255.0;
            double num3 = ((double) this.b) / 255.0;
            double num4 = 0.0;
            double num5 = num;
            double num6 = num;
            if (num2 > num5)
            {
                num5 = num2;
            }
            if (num3 > num5)
            {
                num5 = num3;
            }
            if (num2 < num6)
            {
                num6 = num2;
            }
            if (num3 < num6)
            {
                num6 = num3;
            }
            double num7 = num5 - num6;
            if (num == num5)
            {
                num4 = (num2 - num3) / num7;
            }
            else if (num2 == num5)
            {
                num4 = 2.0 + ((num3 - num) / num7);
            }
            else if (num3 == num5)
            {
                num4 = 4.0 + ((num - num2) / num7);
            }
            num4 *= 60.0;
            if (num4 < 0.0)
            {
                num4 += 360.0;
            }
            return num4;
        }

        public double GetSaturation()
        {
            double num = ((double) this.r) / 255.0;
            double num2 = ((double) this.g) / 255.0;
            double num3 = ((double) this.b) / 255.0;
            double num4 = 0.0;
            double num5 = num;
            double num6 = num;
            if (num2 > num5)
            {
                num5 = num2;
            }
            if (num3 > num5)
            {
                num5 = num3;
            }
            if (num2 < num6)
            {
                num6 = num2;
            }
            if (num3 < num6)
            {
                num6 = num3;
            }
            if (num5 == num6)
            {
                return num4;
            }
            double num7 = (num5 + num6) / 2.0;
            if (num7 <= 0.5)
            {
                return ((num5 - num6) / (num5 + num6));
            }
            return ((num5 - num6) / ((2.0 - num5) - num6));
        }

        public double GetBrightness()
        {
            double num = ((double) this.r) / 255.0;
            double num2 = ((double) this.g) / 255.0;
            double num3 = ((double) this.b) / 255.0;
            double num4 = num;
            double num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            return ((num4 + num5) / 2.0);
        }

        private void RgbChanged()
        {
            this.cs = XColorSpace.Rgb;
            int num = 0xff - this.r;
            int num2 = 0xff - this.g;
            int num3 = 0xff - this.b;
            int num4 = Math.Min(num, Math.Min(num2, num3));
            if (num4 == 0xff)
            {
                this.c = this.m = this.y = 0f;
            }
            else
            {
                float num5 = 255f - num4;
                this.c = ((float) (num - num4)) / num5;
                this.m = ((float) (num2 - num4)) / num5;
                this.y = ((float) (num3 - num4)) / num5;
            }
            this.k = this.gs = ((float) num4) / 255f;
        }

        private void CmykChanged()
        {
            this.cs = XColorSpace.Cmyk;
            float num = this.k * 255f;
            float num2 = 255f - num;
            this.r = (byte) (255f - Math.Min((float) 255f, (float) ((this.c * num2) + num)));
            this.g = (byte) (255f - Math.Min((float) 255f, (float) ((this.m * num2) + num)));
            this.b = (byte) (255f - Math.Min((float) 255f, (float) ((this.y * num2) + num)));
            this.gs = (float) (1.0 - Math.Min((double) 1.0, (double) ((((0.3f * this.c) + (0.59f * this.m)) + (0.11 * this.y)) + this.k)));
        }

        private void GrayChanged()
        {
            this.cs = XColorSpace.GrayScale;
            this.r = (byte) (this.gs * 255f);
            this.g = (byte) (this.gs * 255f);
            this.b = (byte) (this.gs * 255f);
            this.c = 0f;
            this.m = 0f;
            this.y = 0f;
            this.k = 1f - this.gs;
        }

        public double A
        {
            get => 
                ((double) this.a);
            set
            {
                if (value < 0.0)
                {
                    this.a = 0f;
                }
                else if (value > 1.0)
                {
                    this.a = 1f;
                }
                else
                {
                    this.a = (float) value;
                }
            }
        }
        public byte R
        {
            get => 
                this.r;
            set
            {
                this.r = value;
                this.RgbChanged();
            }
        }
        public byte G
        {
            get => 
                this.g;
            set
            {
                this.g = value;
                this.RgbChanged();
            }
        }
        public byte B
        {
            get => 
                this.b;
            set
            {
                this.b = value;
                this.RgbChanged();
            }
        }
        internal uint Rgb =>
            ((uint) (((this.r << 0x10) | (this.g << 8)) | this.b));
        internal uint Argb =>
            ((uint) ((((((uint) (this.a * 255f)) << 0x18) | (this.r << 0x10)) | (this.g << 8)) | this.b));
        public double C
        {
            get => 
                ((double) this.c);
            set
            {
                if (value < 0.0)
                {
                    this.c = 0f;
                }
                else if (value > 1.0)
                {
                    this.c = 1f;
                }
                else
                {
                    this.c = (float) value;
                }
                this.CmykChanged();
            }
        }
        public double M
        {
            get => 
                ((double) this.m);
            set
            {
                if (value < 0.0)
                {
                    this.m = 0f;
                }
                else if (value > 1.0)
                {
                    this.m = 1f;
                }
                else
                {
                    this.m = (float) value;
                }
                this.CmykChanged();
            }
        }
        public double Y
        {
            get => 
                ((double) this.y);
            set
            {
                if (value < 0.0)
                {
                    this.y = 0f;
                }
                else if (value > 1.0)
                {
                    this.y = 1f;
                }
                else
                {
                    this.y = (float) value;
                }
                this.CmykChanged();
            }
        }
        public double K
        {
            get => 
                ((double) this.k);
            set
            {
                if (value < 0.0)
                {
                    this.k = 0f;
                }
                else if (value > 1.0)
                {
                    this.k = 1f;
                }
                else
                {
                    this.k = (float) value;
                }
                this.CmykChanged();
            }
        }
        public double GS
        {
            get => 
                ((double) this.gs);
            set
            {
                if (value < 0.0)
                {
                    this.gs = 0f;
                }
                else if (value > 1.0)
                {
                    this.gs = 1f;
                }
                else
                {
                    this.gs = (float) value;
                }
                this.GrayChanged();
            }
        }
        public string RgbCmykG
        {
            get => 
                string.Format(CultureInfo.InvariantCulture, "{0};{1};{2};{3};{4};{5};{6};{7};{8}", new object[] { this.r, this.g, this.b, this.c, this.m, this.y, this.k, this.gs, this.a });
            set
            {
                string[] strArray = value.Split(new char[] { ';' });
                this.r = byte.Parse(strArray[0], CultureInfo.InvariantCulture);
                this.g = byte.Parse(strArray[1], CultureInfo.InvariantCulture);
                this.b = byte.Parse(strArray[2], CultureInfo.InvariantCulture);
                this.c = float.Parse(strArray[3], CultureInfo.InvariantCulture);
                this.m = float.Parse(strArray[4], CultureInfo.InvariantCulture);
                this.y = float.Parse(strArray[5], CultureInfo.InvariantCulture);
                this.k = float.Parse(strArray[6], CultureInfo.InvariantCulture);
                this.gs = float.Parse(strArray[7], CultureInfo.InvariantCulture);
                this.a = float.Parse(strArray[8], CultureInfo.InvariantCulture);
            }
        }
        private static void CheckByte(int val, string name)
        {
            if ((val < 0) || (val > 0xff))
            {
                throw new ArgumentException(PSSR.InvalidValue(val, name, 0, 0xff));
            }
        }
    }
}

