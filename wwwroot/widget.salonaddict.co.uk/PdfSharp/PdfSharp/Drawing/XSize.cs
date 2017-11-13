namespace PdfSharp.Drawing
{
    using PdfSharp.Internal;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("Width={Width.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}, Height={Height.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}")]
    public struct XSize : IFormattable
    {
        internal double width;
        internal double height;
        private static readonly XSize s_empty;
        public XSize(double width, double height)
        {
            if ((width < 0.0) || (height < 0.0))
            {
                throw new ArgumentException("WidthAndHeightCannotBeNegative");
            }
            this.width = width;
            this.height = height;
        }

        [Obsolete("Use explicit conversion to make your code more readable.")]
        public XSize(XPoint pt)
        {
            this.width = pt.X;
            this.height = pt.Y;
        }

        public static bool operator ==(XSize size1, XSize size2) => 
            ((size1.Width == size2.Width) && (size1.Height == size2.Height));

        public static bool operator !=(XSize size1, XSize size2) => 
            !(size1 == size2);

        public static bool Equals(XSize size1, XSize size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            return (size1.Width.Equals(size2.Width) && size1.Height.Equals(size2.Height));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is XSize))
            {
                return false;
            }
            XSize size = (XSize) o;
            return Equals(this, size);
        }

        public bool Equals(XSize value) => 
            Equals(this, value);

        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            return (this.Width.GetHashCode() ^ this.Height.GetHashCode());
        }

        public static XSize Parse(string source)
        {
            XSize empty;
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            TokenizerHelper helper = new TokenizerHelper(source, invariantCulture);
            string str = helper.NextTokenRequired();
            if (str == "Empty")
            {
                empty = Empty;
            }
            else
            {
                empty = new XSize(Convert.ToDouble(str, invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture));
            }
            helper.LastTokenRequired();
            return empty;
        }

        public PointF ToPointF() => 
            new PointF((float) this.width, (float) this.height);

        public XPoint ToXPoint() => 
            new XPoint(this.width, this.height);

        public XVector ToXVector() => 
            new XVector(this.width, this.height);

        public SizeF ToSizeF() => 
            new SizeF((float) this.width, (float) this.height);

        public static XSize FromSize(Size size) => 
            new XSize((double) size.Width, (double) size.Height);

        public static implicit operator XSize(Size size) => 
            new XSize((double) size.Width, (double) size.Height);

        public static XSize FromSizeF(SizeF size) => 
            new XSize((double) size.Width, (double) size.Height);

        public override string ToString() => 
            this.ConvertToString(null, null);

        public string ToString(IFormatProvider provider) => 
            this.ConvertToString(null, provider);

        string IFormattable.ToString(string format, IFormatProvider provider) => 
            this.ConvertToString(format, provider);

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            if (this.IsEmpty)
            {
                return "Empty";
            }
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { numericListSeparator, this.width, this.height });
        }

        public static XSize Empty =>
            s_empty;
        public bool IsEmpty =>
            (this.width < 0.0);
        public double Width
        {
            get => 
                this.width;
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("CannotModifyEmptySize");
                }
                if (value < 0.0)
                {
                    throw new ArgumentException("WidthCannotBeNegative");
                }
                this.width = value;
            }
        }
        public double Height
        {
            get => 
                this.height;
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("CannotModifyEmptySize");
                }
                if (value < 0.0)
                {
                    throw new ArgumentException("HeightCannotBeNegative");
                }
                this.height = value;
            }
        }
        public static explicit operator XVector(XSize size) => 
            new XVector(size.width, size.height);

        public static explicit operator XPoint(XSize size) => 
            new XPoint(size.width, size.height);

        private static XSize CreateEmptySize() => 
            new XSize { 
                width = double.NegativeInfinity,
                height = double.NegativeInfinity
            };

        static XSize()
        {
            s_empty = CreateEmptySize();
        }
    }
}

