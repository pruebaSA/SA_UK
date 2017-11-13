namespace System.Drawing
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true), TypeConverter(typeof(PointConverter))]
    public struct Point
    {
        public static readonly Point Empty;
        private int x;
        private int y;
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Size sz)
        {
            this.x = sz.Width;
            this.y = sz.Height;
        }

        public Point(int dw)
        {
            this.x = (short) LOWORD(dw);
            this.y = (short) HIWORD(dw);
        }

        [Browsable(false)]
        public bool IsEmpty =>
            ((this.x == 0) && (this.y == 0));
        public int X
        {
            get => 
                this.x;
            set
            {
                this.x = value;
            }
        }
        public int Y
        {
            get => 
                this.y;
            set
            {
                this.y = value;
            }
        }
        public static implicit operator PointF(Point p) => 
            new PointF((float) p.X, (float) p.Y);

        public static explicit operator Size(Point p) => 
            new Size(p.X, p.Y);

        public static Point operator +(Point pt, Size sz) => 
            Add(pt, sz);

        public static Point operator -(Point pt, Size sz) => 
            Subtract(pt, sz);

        public static bool operator ==(Point left, Point right) => 
            ((left.X == right.X) && (left.Y == right.Y));

        public static bool operator !=(Point left, Point right) => 
            !(left == right);

        public static Point Add(Point pt, Size sz) => 
            new Point(pt.X + sz.Width, pt.Y + sz.Height);

        public static Point Subtract(Point pt, Size sz) => 
            new Point(pt.X - sz.Width, pt.Y - sz.Height);

        public static Point Ceiling(PointF value) => 
            new Point((int) Math.Ceiling((double) value.X), (int) Math.Ceiling((double) value.Y));

        public static Point Truncate(PointF value) => 
            new Point((int) value.X, (int) value.Y);

        public static Point Round(PointF value) => 
            new Point((int) Math.Round((double) value.X), (int) Math.Round((double) value.Y));

        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }
            Point point = (Point) obj;
            return ((point.X == this.X) && (point.Y == this.Y));
        }

        public override int GetHashCode() => 
            (this.x ^ this.y);

        public void Offset(int dx, int dy)
        {
            this.X += dx;
            this.Y += dy;
        }

        public void Offset(Point p)
        {
            this.Offset(p.X, p.Y);
        }

        public override string ToString() => 
            ("{X=" + this.X.ToString(CultureInfo.CurrentCulture) + ",Y=" + this.Y.ToString(CultureInfo.CurrentCulture) + "}");

        private static int HIWORD(int n) => 
            ((n >> 0x10) & 0xffff);

        private static int LOWORD(int n) => 
            (n & 0xffff);

        static Point()
        {
            Empty = new Point();
        }
    }
}

