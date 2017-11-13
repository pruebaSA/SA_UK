namespace PdfSharp.Drawing
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct XVector : IFormattable
    {
        internal double x;
        internal double y;
        public XVector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator ==(XVector vector1, XVector vector2) => 
            ((vector1.x == vector2.x) && (vector1.y == vector2.y));

        public static bool operator !=(XVector vector1, XVector vector2)
        {
            if (vector1.x == vector2.x)
            {
                return (vector1.y != vector2.y);
            }
            return true;
        }

        public static bool Equals(XVector vector1, XVector vector2) => 
            (vector1.X.Equals(vector2.X) && vector1.Y.Equals(vector2.Y));

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is XVector))
            {
                return false;
            }
            XVector vector = (XVector) o;
            return Equals(this, vector);
        }

        public bool Equals(XVector value) => 
            Equals(this, value);

        public override int GetHashCode() => 
            (this.x.GetHashCode() ^ this.y.GetHashCode());

        public double X
        {
            get => 
                this.x;
            set
            {
                this.x = value;
            }
        }
        public double Y
        {
            get => 
                this.y;
            set
            {
                this.y = value;
            }
        }
        public override string ToString() => 
            this.ConvertToString(null, null);

        public string ToString(IFormatProvider provider) => 
            this.ConvertToString(null, provider);

        string IFormattable.ToString(string format, IFormatProvider provider) => 
            this.ConvertToString(format, provider);

        internal string ConvertToString(string format, IFormatProvider provider) => 
            string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { ',', this.x, this.y });

        public double Length =>
            Math.Sqrt((this.x * this.x) + (this.y * this.y));
        public double LengthSquared =>
            ((this.x * this.x) + (this.y * this.y));
        public void Normalize()
        {
            this = (XVector) (this / Math.Max(Math.Abs(this.x), Math.Abs(this.y)));
            this = (XVector) (this / this.Length);
        }

        public static double CrossProduct(XVector vector1, XVector vector2) => 
            ((vector1.x * vector2.y) - (vector1.y * vector2.x));

        public static double AngleBetween(XVector vector1, XVector vector2)
        {
            double y = (vector1.x * vector2.y) - (vector2.x * vector1.y);
            double x = (vector1.x * vector2.x) + (vector1.y * vector2.y);
            return (Math.Atan2(y, x) * 57.295779513082323);
        }

        public static XVector operator -(XVector vector) => 
            new XVector(-vector.x, -vector.y);

        public void Negate()
        {
            this.x = -this.x;
            this.y = -this.y;
        }

        public static XVector operator +(XVector vector1, XVector vector2) => 
            new XVector(vector1.x + vector2.x, vector1.y + vector2.y);

        public static XVector Add(XVector vector1, XVector vector2) => 
            new XVector(vector1.x + vector2.x, vector1.y + vector2.y);

        public static XVector operator -(XVector vector1, XVector vector2) => 
            new XVector(vector1.x - vector2.x, vector1.y - vector2.y);

        public static XVector Subtract(XVector vector1, XVector vector2) => 
            new XVector(vector1.x - vector2.x, vector1.y - vector2.y);

        public static XPoint operator +(XVector vector, XPoint point) => 
            new XPoint(point.x + vector.x, point.y + vector.y);

        public static XPoint Add(XVector vector, XPoint point) => 
            new XPoint(point.x + vector.x, point.y + vector.y);

        public static XVector operator *(XVector vector, double scalar) => 
            new XVector(vector.x * scalar, vector.y * scalar);

        public static XVector Multiply(XVector vector, double scalar) => 
            new XVector(vector.x * scalar, vector.y * scalar);

        public static XVector operator *(double scalar, XVector vector) => 
            new XVector(vector.x * scalar, vector.y * scalar);

        public static XVector Multiply(double scalar, XVector vector) => 
            new XVector(vector.x * scalar, vector.y * scalar);

        public static XVector operator /(XVector vector, double scalar) => 
            ((XVector) (vector * (1.0 / scalar)));

        public static XVector Divide(XVector vector, double scalar) => 
            ((XVector) (vector * (1.0 / scalar)));

        public static XVector operator *(XVector vector, XMatrix matrix) => 
            matrix.Transform(vector);

        public static XVector Multiply(XVector vector, XMatrix matrix) => 
            matrix.Transform(vector);

        public static double operator *(XVector vector1, XVector vector2) => 
            ((vector1.x * vector2.x) + (vector1.y * vector2.y));

        public static double Multiply(XVector vector1, XVector vector2) => 
            ((vector1.x * vector2.x) + (vector1.y * vector2.y));

        public static double Determinant(XVector vector1, XVector vector2) => 
            ((vector1.x * vector2.y) - (vector1.y * vector2.x));

        public static explicit operator XSize(XVector vector) => 
            new XSize(Math.Abs(vector.x), Math.Abs(vector.y));

        public static explicit operator XPoint(XVector vector) => 
            new XPoint(vector.x, vector.y);
    }
}

