namespace MigraDoc.DocumentObjectModel
{
    using MigraDoc.DocumentObjectModel.Internals;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), DebuggerDisplay("(A={A}, R={R}, G={G}, B={B} C={C}, M={M}, Y={Y}, K={K})")]
    public struct Color : INullableValue
    {
        private static Hashtable stdColors;
        private uint argb;
        private bool isCmyk;
        private float a;
        private float c;
        private float m;
        private float y;
        private float k;
        public static readonly Color Empty;
        public Color(uint argb)
        {
            this.isCmyk = false;
            this.argb = argb;
            this.a = this.c = this.m = this.y = this.k = 0f;
            this.InitCmykFromRgb();
        }

        public Color(byte r, byte g, byte b)
        {
            this.isCmyk = false;
            this.argb = (uint) (((-16777216 | (r << 0x10)) | (g << 8)) | b);
            this.a = this.c = this.m = this.y = this.k = 0f;
            this.InitCmykFromRgb();
        }

        public Color(byte a, byte r, byte g, byte b)
        {
            this.isCmyk = false;
            this.argb = (uint) ((((a << 0x18) | (r << 0x10)) | (g << 8)) | b);
            this.a = this.c = this.m = this.y = this.k = 0f;
            this.InitCmykFromRgb();
        }

        public Color(double alpha, double cyan, double magenta, double yellow, double black)
        {
            this.isCmyk = true;
            this.a = (alpha > 100.0) ? ((float) 100.0) : ((alpha < 0.0) ? ((float) 0.0) : ((float) alpha));
            this.c = (cyan > 100.0) ? ((float) 100.0) : ((cyan < 0.0) ? ((float) 0.0) : ((float) cyan));
            this.m = (magenta > 100.0) ? ((float) 100.0) : ((magenta < 0.0) ? ((float) 0.0) : ((float) magenta));
            this.y = (yellow > 100.0) ? ((float) 100.0) : ((yellow < 0.0) ? ((float) 0.0) : ((float) yellow));
            this.k = (black > 100.0) ? ((float) 100.0) : ((black < 0.0) ? ((float) 0.0) : ((float) black));
            this.argb = 0;
            this.InitRgbFromCmyk();
        }

        public Color(double cyan, double magenta, double yellow, double black) : this(100.0, cyan, magenta, yellow, black)
        {
        }

        private void InitCmykFromRgb()
        {
            this.isCmyk = false;
            int num = 0xff - ((int) this.R);
            int num2 = 0xff - ((int) this.G);
            int num3 = 0xff - ((int) this.B);
            int num4 = Math.Min(num, Math.Min(num2, num3));
            if (num4 == 0xff)
            {
                this.c = this.m = this.y = 0f;
            }
            else
            {
                float num5 = 255f - num4;
                this.c = (100f * (num - num4)) / num5;
                this.m = (100f * (num2 - num4)) / num5;
                this.y = (100f * (num3 - num4)) / num5;
            }
            this.k = (100f * num4) / 255f;
            this.a = ((float) this.A) / 2.55f;
        }

        private void InitRgbFromCmyk()
        {
            this.isCmyk = true;
            float num = (this.k * 2.55f) + 0.5f;
            float num2 = (255f - num) / 100f;
            byte num3 = (byte) ((this.a * 2.55) + 0.5);
            byte num4 = (byte) (255f - Math.Min((float) 255f, (float) ((this.c * num2) + num)));
            byte num5 = (byte) (255f - Math.Min((float) 255f, (float) ((this.m * num2) + num)));
            byte num6 = (byte) (255f - Math.Min((float) 255f, (float) ((this.y * num2) + num)));
            this.argb = (uint) ((((num3 << 0x18) | (num4 << 0x10)) | (num5 << 8)) | num6);
        }

        public bool IsCmyk =>
            this.isCmyk;
        public bool IsEmpty =>
            (this == Empty);
        object INullableValue.GetValue() => 
            this;

        void INullableValue.SetValue(object value)
        {
            if (value is uint)
            {
                this.argb = (uint) value;
            }
            else
            {
                this = Parse(value.ToString());
            }
        }

        void INullableValue.SetNull()
        {
            this = Empty;
        }

        bool INullableValue.IsNull =>
            (this == Empty);
        internal bool IsNull =>
            (this == Empty);
        public uint Argb
        {
            get => 
                this.argb;
            set
            {
                if (this.isCmyk)
                {
                    throw new InvalidOperationException("Cannot change a CMYK color.");
                }
                this.argb = value;
                this.InitCmykFromRgb();
            }
        }
        public uint RGB
        {
            get => 
                this.argb;
            set
            {
                if (this.isCmyk)
                {
                    throw new InvalidOperationException("Cannot change a CMYK color.");
                }
                this.argb = value;
                this.InitCmykFromRgb();
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is Color))
            {
                return false;
            }
            Color color = (Color) obj;
            if (this.isCmyk ^ color.isCmyk)
            {
                return false;
            }
            if (!this.isCmyk)
            {
                return (this.argb == color.argb);
            }
            return ((((this.a == color.a) && (this.c == color.c)) && ((this.m == color.m) && (this.y == color.y))) && (this.k == color.k));
        }

        public override int GetHashCode() => 
            (((((((int) this.argb) ^ this.a.GetHashCode()) ^ this.c.GetHashCode()) ^ this.m.GetHashCode()) ^ this.y.GetHashCode()) ^ this.k.GetHashCode());

        public static bool operator ==(Color color1, Color color2)
        {
            if (color1.isCmyk ^ color2.isCmyk)
            {
                return false;
            }
            if (!color1.isCmyk)
            {
                return (color1.argb == color2.argb);
            }
            return ((((color1.a == color2.a) && (color1.c == color2.c)) && ((color1.m == color2.m) && (color1.y == color2.y))) && (color1.k == color2.k));
        }

        public static bool operator !=(Color color1, Color color2) => 
            !(color1 == color2);

        public static Color Parse(string color)
        {
            Color color2;
            if (color == null)
            {
                throw new ArgumentNullException("color");
            }
            if (color == "")
            {
                throw new ArgumentException("color");
            }
            try
            {
                try
                {
                    return new Color((uint) Enum.Parse(typeof(ColorName), color, true));
                }
                catch
                {
                }
                NumberStyles integer = NumberStyles.Integer;
                string s = color.ToLower();
                if (s.StartsWith("0x"))
                {
                    integer = NumberStyles.HexNumber;
                    s = color.Substring(2);
                }
                color2 = new Color(uint.Parse(s, integer));
            }
            catch (FormatException exception)
            {
                throw new ArgumentException(DomSR.InvalidColorString(color), exception);
            }
            return color2;
        }

        public uint A =>
            ((uint) ((this.argb & -16777216) >> 0x18));
        public uint R =>
            ((uint) ((this.argb & 0xff0000) >> 0x10));
        public uint G =>
            ((uint) ((this.argb & 0xff00) >> 8));
        public uint B =>
            (this.argb & 0xff);
        public double Alpha =>
            ((double) this.a);
        public double C =>
            ((double) this.c);
        public double M =>
            ((double) this.m);
        public double Y =>
            ((double) this.y);
        public double K =>
            ((double) this.k);
        public Color GetMixedTransparencyColor()
        {
            int a = (int) this.A;
            if (a == 0xff)
            {
                return this;
            }
            int r = (int) this.R;
            int g = (int) this.G;
            int b = (int) this.B;
            double num5 = 1.0 - (((double) a) / 255.0);
            r += (int) ((0xff - r) * num5);
            g += (int) ((0xff - g) * num5);
            b += (int) ((0xff - b) * num5);
            return new Color((uint) (((-16777216 | (r << 0x10)) | (g << 8)) | b));
        }

        public override string ToString()
        {
            if (stdColors == null)
            {
                Array names = Enum.GetNames(typeof(ColorName));
                Array values = Enum.GetValues(typeof(ColorName));
                int length = names.GetLength(0);
                stdColors = new Hashtable(length);
                for (int i = 0; i < length; i++)
                {
                    string str = (string) names.GetValue(i);
                    uint key = (uint) values.GetValue(i);
                    if (!stdColors.ContainsKey(key))
                    {
                        stdColors.Add(key, str);
                    }
                }
            }
            if (this.isCmyk)
            {
                if (this.Alpha == 100.0)
                {
                    return string.Format(CultureInfo.InvariantCulture, "CMYK({0:0.##},{1:0.##},{2:0.##},{3:0.##})", new object[] { this.C, this.M, this.Y, this.K });
                }
                return string.Format(CultureInfo.InvariantCulture, "CMYK({0:0.##},{1:0.##},{2:0.##},{3:0.##},{4:0.##})", new object[] { this.Alpha, this.C, this.M, this.Y, this.K });
            }
            if (stdColors.ContainsKey(this.argb))
            {
                return (string) stdColors[this.argb];
            }
            if ((this.argb & 0xff000000) == 0xff000000)
            {
                string[] strArray = new string[] { "RGB(", ((uint) ((this.argb & 0xff0000) >> 0x10)).ToString(CultureInfo.InvariantCulture), ",", ((uint) ((this.argb & 0xff00) >> 8)).ToString(CultureInfo.InvariantCulture), ",", (this.argb & 0xff).ToString(CultureInfo.InvariantCulture), ")" };
                return string.Concat(strArray);
            }
            return ("0x" + this.argb.ToString("X"));
        }

        public static Color FromRgbColor(byte a, Color color) => 
            new Color(a, (byte) color.R, (byte) color.G, (byte) color.B);

        public static Color FromCmyk(double cyan, double magenta, double yellow, double black) => 
            new Color(cyan, magenta, yellow, black);

        public static Color FromCmyk(double alpha, double cyan, double magenta, double yellow, double black) => 
            new Color(alpha, cyan, magenta, yellow, black);

        public static Color FromCmykColor(double alpha, Color color) => 
            new Color(alpha, color.C, color.M, color.Y, color.K);

        static Color()
        {
            Empty = new Color(0);
        }
    }
}

