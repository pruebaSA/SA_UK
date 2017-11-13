namespace System.Windows
{
    using MS.Internal;
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using System.Windows.Converters;
    using System.Windows.Markup;
    using System.Windows.Media;

    [Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(VectorConverter)), ValueSerializer(typeof(VectorValueSerializer))]
    public struct Vector : IFormattable
    {
        internal double _x;
        internal double _y;
        public static bool operator ==(Vector vector1, Vector vector2) => 
            ((vector1.X == vector2.X) && (vector1.Y == vector2.Y));

        public static bool operator !=(Vector vector1, Vector vector2) => 
            !(vector1 == vector2);

        public static bool Equals(Vector vector1, Vector vector2) => 
            (vector1.X.Equals(vector2.X) && vector1.Y.Equals(vector2.Y));

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Vector))
            {
                return false;
            }
            Vector vector = (Vector) o;
            return Equals(this, vector);
        }

        public bool Equals(Vector value) => 
            Equals(this, value);

        public override int GetHashCode() => 
            (this.X.GetHashCode() ^ this.Y.GetHashCode());

        public static Vector Parse(string source)
        {
            IFormatProvider englishUSCulture = TypeConverterHelper.EnglishUSCulture;
            TokenizerHelper helper = new TokenizerHelper(source, englishUSCulture);
            string str = helper.NextTokenRequired();
            Vector vector = new Vector(Convert.ToDouble(str, englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture));
            helper.LastTokenRequired();
            return vector;
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

        public Vector(double x, double y)
        {
            this._x = x;
            this._y = y;
        }

        public double Length =>
            Math.Sqrt((this._x * this._x) + (this._y * this._y));
        public double LengthSquared =>
            ((this._x * this._x) + (this._y * this._y));
        public void Normalize()
        {
            this = (Vector) (this / Math.Max(Math.Abs(this._x), Math.Abs(this._y)));
            this = (Vector) (this / this.Length);
        }

        public static double CrossProduct(Vector vector1, Vector vector2) => 
            ((vector1._x * vector2._y) - (vector1._y * vector2._x));

        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            double y = (vector1._x * vector2._y) - (vector2._x * vector1._y);
            double x = (vector1._x * vector2._x) + (vector1._y * vector2._y);
            return (Math.Atan2(y, x) * 57.295779513082323);
        }

        public static Vector operator -(Vector vector) => 
            new Vector(-vector._x, -vector._y);

        public void Negate()
        {
            this._x = -this._x;
            this._y = -this._y;
        }

        public static Vector operator +(Vector vector1, Vector vector2) => 
            new Vector(vector1._x + vector2._x, vector1._y + vector2._y);

        public static Vector Add(Vector vector1, Vector vector2) => 
            new Vector(vector1._x + vector2._x, vector1._y + vector2._y);

        public static Vector operator -(Vector vector1, Vector vector2) => 
            new Vector(vector1._x - vector2._x, vector1._y - vector2._y);

        public static Vector Subtract(Vector vector1, Vector vector2) => 
            new Vector(vector1._x - vector2._x, vector1._y - vector2._y);

        public static Point operator +(Vector vector, Point point) => 
            new Point(point._x + vector._x, point._y + vector._y);

        public static Point Add(Vector vector, Point point) => 
            new Point(point._x + vector._x, point._y + vector._y);

        public static Vector operator *(Vector vector, double scalar) => 
            new Vector(vector._x * scalar, vector._y * scalar);

        public static Vector Multiply(Vector vector, double scalar) => 
            new Vector(vector._x * scalar, vector._y * scalar);

        public static Vector operator *(double scalar, Vector vector) => 
            new Vector(vector._x * scalar, vector._y * scalar);

        public static Vector Multiply(double scalar, Vector vector) => 
            new Vector(vector._x * scalar, vector._y * scalar);

        public static Vector operator /(Vector vector, double scalar) => 
            ((Vector) (vector * (1.0 / scalar)));

        public static Vector Divide(Vector vector, double scalar) => 
            ((Vector) (vector * (1.0 / scalar)));

        public static Vector operator *(Vector vector, Matrix matrix) => 
            matrix.Transform(vector);

        public static Vector Multiply(Vector vector, Matrix matrix) => 
            matrix.Transform(vector);

        public static double operator *(Vector vector1, Vector vector2) => 
            ((vector1._x * vector2._x) + (vector1._y * vector2._y));

        public static double Multiply(Vector vector1, Vector vector2) => 
            ((vector1._x * vector2._x) + (vector1._y * vector2._y));

        public static double Determinant(Vector vector1, Vector vector2) => 
            ((vector1._x * vector2._y) - (vector1._y * vector2._x));

        public static explicit operator Size(Vector vector) => 
            new Size(Math.Abs(vector._x), Math.Abs(vector._y));

        public static explicit operator Point(Vector vector) => 
            new Point(vector._x, vector._y);
    }
}

