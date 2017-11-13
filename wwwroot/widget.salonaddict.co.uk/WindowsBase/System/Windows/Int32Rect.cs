namespace System.Windows
{
    using MS.Internal;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Converters;
    using System.Windows.Markup;

    [Serializable, StructLayout(LayoutKind.Sequential), ValueSerializer(typeof(Int32RectValueSerializer)), TypeConverter(typeof(Int32RectConverter))]
    public struct Int32Rect : IFormattable
    {
        internal int _x;
        internal int _y;
        internal int _width;
        internal int _height;
        private static readonly Int32Rect s_empty;
        public static bool operator ==(Int32Rect int32Rect1, Int32Rect int32Rect2) => 
            ((((int32Rect1.X == int32Rect2.X) && (int32Rect1.Y == int32Rect2.Y)) && (int32Rect1.Width == int32Rect2.Width)) && (int32Rect1.Height == int32Rect2.Height));

        public static bool operator !=(Int32Rect int32Rect1, Int32Rect int32Rect2) => 
            !(int32Rect1 == int32Rect2);

        public static bool Equals(Int32Rect int32Rect1, Int32Rect int32Rect2)
        {
            if (int32Rect1.IsEmpty)
            {
                return int32Rect2.IsEmpty;
            }
            return (((int32Rect1.X.Equals(int32Rect2.X) && int32Rect1.Y.Equals(int32Rect2.Y)) && int32Rect1.Width.Equals(int32Rect2.Width)) && int32Rect1.Height.Equals(int32Rect2.Height));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Int32Rect))
            {
                return false;
            }
            Int32Rect rect = (Int32Rect) o;
            return Equals(this, rect);
        }

        public bool Equals(Int32Rect value) => 
            Equals(this, value);

        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            return (((this.X.GetHashCode() ^ this.Y.GetHashCode()) ^ this.Width.GetHashCode()) ^ this.Height.GetHashCode());
        }

        public static Int32Rect Parse(string source)
        {
            Int32Rect empty;
            IFormatProvider englishUSCulture = TypeConverterHelper.EnglishUSCulture;
            TokenizerHelper helper = new TokenizerHelper(source, englishUSCulture);
            string str = helper.NextTokenRequired();
            if (str == "Empty")
            {
                empty = Empty;
            }
            else
            {
                empty = new Int32Rect(Convert.ToInt32(str, englishUSCulture), Convert.ToInt32(helper.NextTokenRequired(), englishUSCulture), Convert.ToInt32(helper.NextTokenRequired(), englishUSCulture), Convert.ToInt32(helper.NextTokenRequired(), englishUSCulture));
            }
            helper.LastTokenRequired();
            return empty;
        }

        public int X
        {
            get => 
                this._x;
            set
            {
                this._x = value;
            }
        }
        public int Y
        {
            get => 
                this._y;
            set
            {
                this._y = value;
            }
        }
        public int Width
        {
            get => 
                this._width;
            set
            {
                this._width = value;
            }
        }
        public int Height
        {
            get => 
                this._height;
            set
            {
                this._height = value;
            }
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
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}", new object[] { numericListSeparator, this._x, this._y, this._width, this._height });
        }

        public Int32Rect(int x, int y, int width, int height)
        {
            this._x = x;
            this._y = y;
            this._width = width;
            this._height = height;
        }

        public static Int32Rect Empty =>
            s_empty;
        public bool IsEmpty =>
            ((((this._x == 0) && (this._y == 0)) && (this._width == 0)) && (this._height == 0));
        static Int32Rect()
        {
            s_empty = new Int32Rect(0, 0, 0, 0);
        }
    }
}

