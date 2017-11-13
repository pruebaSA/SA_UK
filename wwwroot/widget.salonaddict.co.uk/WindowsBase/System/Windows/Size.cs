namespace System.Windows
{
    using MS.Internal;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Converters;
    using System.Windows.Markup;

    [Serializable, StructLayout(LayoutKind.Sequential), ValueSerializer(typeof(SizeValueSerializer)), TypeConverter(typeof(SizeConverter))]
    public struct Size : IFormattable
    {
        internal double _width;
        internal double _height;
        private static readonly Size s_empty;
        public static bool operator ==(Size size1, Size size2) => 
            ((size1.Width == size2.Width) && (size1.Height == size2.Height));

        public static bool operator !=(Size size1, Size size2) => 
            !(size1 == size2);

        public static bool Equals(Size size1, Size size2)
        {
            if (size1.IsEmpty)
            {
                return size2.IsEmpty;
            }
            return (size1.Width.Equals(size2.Width) && size1.Height.Equals(size2.Height));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Size))
            {
                return false;
            }
            Size size = (Size) o;
            return Equals(this, size);
        }

        public bool Equals(Size value) => 
            Equals(this, value);

        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            return (this.Width.GetHashCode() ^ this.Height.GetHashCode());
        }

        public static Size Parse(string source)
        {
            Size empty;
            IFormatProvider englishUSCulture = TypeConverterHelper.EnglishUSCulture;
            TokenizerHelper helper = new TokenizerHelper(source, englishUSCulture);
            string str = helper.NextTokenRequired();
            if (str == "Empty")
            {
                empty = Empty;
            }
            else
            {
                empty = new Size(Convert.ToDouble(str, englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture));
            }
            helper.LastTokenRequired();
            return empty;
        }

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
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { numericListSeparator, this._width, this._height });
        }

        public Size(double width, double height)
        {
            if ((width < 0.0) || (height < 0.0))
            {
                throw new ArgumentException(System.Windows.SR.Get("Size_WidthAndHeightCannotBeNegative"));
            }
            this._width = width;
            this._height = height;
        }

        public static Size Empty =>
            s_empty;
        public bool IsEmpty =>
            (this._width < 0.0);
        public double Width
        {
            get => 
                this._width;
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("Size_CannotModifyEmptySize"));
                }
                if (value < 0.0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("Size_WidthCannotBeNegative"));
                }
                this._width = value;
            }
        }
        public double Height
        {
            get => 
                this._height;
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException(System.Windows.SR.Get("Size_CannotModifyEmptySize"));
                }
                if (value < 0.0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("Size_HeightCannotBeNegative"));
                }
                this._height = value;
            }
        }
        public static explicit operator Vector(Size size) => 
            new Vector(size._width, size._height);

        public static explicit operator Point(Size size) => 
            new Point(size._width, size._height);

        private static Size CreateEmptySize() => 
            new Size { 
                _width = double.NegativeInfinity,
                _height = double.NegativeInfinity
            };

        static Size()
        {
            s_empty = CreateEmptySize();
        }
    }
}

