namespace PdfSharp.Drawing
{
    using PdfSharp.Internal;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("X={X.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}, Y={Y.ToString(\"0.####\",System.Globalization.CultureInfo.InvariantCulture)}")]
    public struct XPoint : IFormattable
    {
        [Obsolete("For convergence with WPF use new XPoint(), not XPoint.Empty")]
        public static readonly XPoint Empty;
        internal double x;
        internal double y;
        public XPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public XPoint(Point point)
        {
            this.x = point.X;
            this.y = point.Y;
        }

        public XPoint(PointF point)
        {
            this.x = point.X;
            this.y = point.Y;
        }

        public static bool operator ==(XPoint point1, XPoint point2) => 
            ((point1.x == point2.x) && (point1.y == point2.y));

        public static bool operator !=(XPoint point1, XPoint point2) => 
            !(point1 == point2);

        public static bool Equals(XPoint point1, XPoint point2) => 
            (point1.X.Equals(point2.X) && point1.Y.Equals(point2.Y));

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is XPoint))
            {
                return false;
            }
            XPoint point = (XPoint) o;
            return Equals(this, point);
        }

        public bool Equals(XPoint value) => 
            Equals(this, value);

        public override int GetHashCode() => 
            (this.X.GetHashCode() ^ this.Y.GetHashCode());

        public static XPoint Parse(string source)
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            TokenizerHelper helper = new TokenizerHelper(source, invariantCulture);
            string str = helper.NextTokenRequired();
            XPoint point = new XPoint(Convert.ToDouble(str, invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture));
            helper.LastTokenRequired();
            return point;
        }

        public static XPoint[] ParsePoints(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            string[] strArray = value.Split(new char[] { ' ' });
            int length = strArray.Length;
            XPoint[] pointArray = new XPoint[length];
            for (int i = 0; i < length; i++)
            {
                pointArray[i] = Parse(strArray[i]);
            }
            return pointArray;
        }

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
        public PointF ToPointF() => 
            new PointF((float) this.x, (float) this.y);

        public override string ToString() => 
            this.ConvertToString(null, null);

        public string ToString(IFormatProvider provider) => 
            this.ConvertToString(null, provider);

        string IFormattable.ToString(string format, IFormatProvider provider) => 
            this.ConvertToString(format, provider);

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}", new object[] { numericListSeparator, this.x, this.y });
        }

        public void Offset(double offsetX, double offsetY)
        {
            this.x += offsetX;
            this.y += offsetY;
        }

        [Browsable(false), Obsolete("Use '== new XPoint()'")]
        public bool IsEmpty =>
            ((this.x == 0.0) && (this.y == 0.0));
        public static XPoint operator +(XPoint point, XVector vector) => 
            new XPoint(point.x + vector.x, point.y + vector.y);

        public static XPoint operator +(XPoint point, XSize size) => 
            new XPoint(point.x + size.width, point.y + size.height);

        public static XPoint Add(XPoint point, XVector vector) => 
            new XPoint(point.x + vector.x, point.y + vector.y);

        public static XPoint operator -(XPoint point, XVector vector) => 
            new XPoint(point.x - vector.x, point.y - vector.y);

        public static XPoint Subtract(XPoint point, XVector vector) => 
            new XPoint(point.x - vector.x, point.y - vector.y);

        public static XVector operator -(XPoint point1, XPoint point2) => 
            new XVector(point1.x - point2.x, point1.y - point2.y);

        [Obsolete("Use XVector instead of XSize as second parameter.")]
        public static XPoint operator -(XPoint point, XSize size) => 
            new XPoint(point.x - size.width, point.y - size.height);

        public static XVector Subtract(XPoint point1, XPoint point2) => 
            new XVector(point1.x - point2.x, point1.y - point2.y);

        public static XPoint operator *(XPoint point, XMatrix matrix) => 
            matrix.Transform(point);

        public static XPoint Multiply(XPoint point, XMatrix matrix) => 
            matrix.Transform(point);

        public static XPoint operator *(XPoint point, double value) => 
            new XPoint(point.x * value, point.y * value);

        public static XPoint operator *(double value, XPoint point) => 
            new XPoint(value * point.x, value * point.y);

        [Obsolete("Avoid using this operator.")]
        public static XPoint operator /(XPoint point, double value)
        {
            if (value == 0.0)
            {
                throw new DivideByZeroException("Divisor is zero.");
            }
            return new XPoint(point.x / value, point.y / value);
        }

        public static explicit operator XSize(XPoint point) => 
            new XSize(Math.Abs(point.x), Math.Abs(point.y));

        public static explicit operator XVector(XPoint point) => 
            new XVector(point.x, point.y);

        static XPoint()
        {
            Empty = new XPoint();
        }
    }
}

