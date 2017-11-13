namespace System.Windows
{
    using MS.Internal;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Converters;
    using System.Windows.Markup;
    using System.Windows.Media;

    [Serializable, StructLayout(LayoutKind.Sequential), ValueSerializer(typeof(PointValueSerializer)), TypeConverter(typeof(PointConverter))]
    public struct Point : IFormattable
    {
        internal double _x;
        internal double _y;
        public static bool operator ==(Point point1, Point point2) => 
            ((point1.X == point2.X) && (point1.Y == point2.Y));

        public static bool operator !=(Point point1, Point point2) => 
            !(point1 == point2);

        public static bool Equals(Point point1, Point point2) => 
            (point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y));

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Point))
            {
                return false;
            }
            Point point = (Point) o;
            return Equals(this, point);
        }

        public bool Equals(Point value) => 
            Equals(this, value);

        public override int GetHashCode() => 
            (this.X.GetHashCode() ^ this.Y.GetHashCode());

        public static Point Parse(string source)
        {
            IFormatProvider englishUSCulture = TypeConverterHelper.EnglishUSCulture;
            TokenizerHelper helper = new TokenizerHelper(source, englishUSCulture);
            string str = helper.NextTokenRequired();
            Point point = new Point(Convert.ToDouble(str, englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture));
            helper.LastTokenRequired();
            return point;
        }

        public double X
        {
            get => 
                this._x;
            set
            {
                this._x = value;
            }
        }
        public double Y
        {
            get => 
                this._y;
            set
            {
                this._y = value;
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
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { numericListSeparator, this._x, this._y });
        }

        public Point(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        public void Offset(double offsetX, double offsetY)
        {
            this._x += offsetX;
            this._y += offsetY;
        }

        public static Point operator +(Point point, Vector vector) => 
            new Point(point._x + vector._x, point._y + vector._y);

        public static Point Add(Point point, Vector vector) => 
            new Point(point._x + vector._x, point._y + vector._y);

        public static Point operator -(Point point, Vector vector) => 
            new Point(point._x - vector._x, point._y - vector._y);

        public static Point Subtract(Point point, Vector vector) => 
            new Point(point._x - vector._x, point._y - vector._y);

        public static Vector operator -(Point point1, Point point2) => 
            new Vector(point1._x - point2._x, point1._y - point2._y);

        public static Vector Subtract(Point point1, Point point2) => 
            new Vector(point1._x - point2._x, point1._y - point2._y);

        public static Point operator *(Point point, Matrix matrix) => 
            matrix.Transform(point);

        public static Point Multiply(Point point, Matrix matrix) => 
            matrix.Transform(point);

        public static explicit operator Size(Point point) => 
            new Size(Math.Abs(point._x), Math.Abs(point._y));

        public static explicit operator Vector(Point point) => 
            new Vector(point._x, point._y);
    }
}

