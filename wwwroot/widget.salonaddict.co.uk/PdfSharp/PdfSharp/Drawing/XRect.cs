namespace PdfSharp.Drawing
{
    using PdfSharp.Internal;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("({X}, {Y}, {Width}, {Height})")]
    public struct XRect : IFormattable
    {
        internal double x;
        internal double y;
        internal double width;
        internal double height;
        private static readonly XRect s_empty;
        public XRect(double x, double y, double width, double height)
        {
            if ((width < 0.0) || (height < 0.0))
            {
                throw new ArgumentException("WidthAndHeightCannotBeNegative");
            }
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public XRect(XPoint point1, XPoint point2)
        {
            this.x = Math.Min(point1.x, point2.x);
            this.y = Math.Min(point1.y, point2.y);
            this.width = Math.Max((double) (Math.Max(point1.x, point2.x) - this.x), (double) 0.0);
            this.height = Math.Max((double) (Math.Max(point1.y, point2.y) - this.y), (double) 0.0);
        }

        public XRect(XPoint point, XVector vector) : this(point, point + vector)
        {
        }

        public XRect(XPoint location, XSize size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                this.x = location.x;
                this.y = location.y;
                this.width = size.width;
                this.height = size.height;
            }
        }

        public XRect(XSize size)
        {
            if (size.IsEmpty)
            {
                this = s_empty;
            }
            else
            {
                this.x = this.y = 0.0;
                this.width = size.Width;
                this.height = size.Height;
            }
        }

        public XRect(PointF location, SizeF size)
        {
            this.x = location.X;
            this.y = location.Y;
            this.width = size.Width;
            this.height = size.Height;
        }

        public XRect(RectangleF rect)
        {
            this.x = rect.X;
            this.y = rect.Y;
            this.width = rect.Width;
            this.height = rect.Height;
        }

        public static XRect FromLTRB(double left, double top, double right, double bottom) => 
            new XRect(left, top, right - left, bottom - top);

        public static bool operator ==(XRect rect1, XRect rect2) => 
            ((((rect1.X == rect2.X) && (rect1.Y == rect2.Y)) && (rect1.Width == rect2.Width)) && (rect1.Height == rect2.Height));

        public static bool operator !=(XRect rect1, XRect rect2) => 
            !(rect1 == rect2);

        public static bool Equals(XRect rect1, XRect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            return (((rect1.X.Equals(rect2.X) && rect1.Y.Equals(rect2.Y)) && rect1.Width.Equals(rect2.Width)) && rect1.Height.Equals(rect2.Height));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is XRect))
            {
                return false;
            }
            XRect rect = (XRect) o;
            return Equals(this, rect);
        }

        public bool Equals(XRect value) => 
            Equals(this, value);

        public override int GetHashCode()
        {
            if (this.IsEmpty)
            {
                return 0;
            }
            return (((this.X.GetHashCode() ^ this.Y.GetHashCode()) ^ this.Width.GetHashCode()) ^ this.Height.GetHashCode());
        }

        public static XRect Parse(string source)
        {
            XRect empty;
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            TokenizerHelper helper = new TokenizerHelper(source, invariantCulture);
            string str = helper.NextTokenRequired();
            if (str == "Empty")
            {
                empty = Empty;
            }
            else
            {
                empty = new XRect(Convert.ToDouble(str, invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture));
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
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}", new object[] { numericListSeparator, this.x, this.y, this.width, this.height });
        }

        public static XRect Empty =>
            s_empty;
        public bool IsEmpty =>
            (this.width < 0.0);
        public XPoint Location
        {
            get => 
                new XPoint(this.x, this.y);
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("CannotModifyEmptyRect");
                }
                this.x = value.x;
                this.y = value.y;
            }
        }
        public XSize Size
        {
            get
            {
                if (this.IsEmpty)
                {
                    return XSize.Empty;
                }
                return new XSize(this.width, this.height);
            }
            set
            {
                if (value.IsEmpty)
                {
                    this = s_empty;
                }
                else
                {
                    if (this.IsEmpty)
                    {
                        throw new InvalidOperationException("CannotModifyEmptyRect");
                    }
                    this.width = value.width;
                    this.height = value.height;
                }
            }
        }
        public double X
        {
            get => 
                this.x;
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("CannotModifyEmptyRect");
                }
                this.x = value;
            }
        }
        public double Y
        {
            get => 
                this.y;
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("CannotModifyEmptyRect");
                }
                this.y = value;
            }
        }
        public double Width
        {
            get => 
                this.width;
            set
            {
                if (this.IsEmpty)
                {
                    throw new InvalidOperationException("CannotModifyEmptyRect");
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
                    throw new InvalidOperationException("CannotModifyEmptyRect");
                }
                if (value < 0.0)
                {
                    throw new ArgumentException("WidthCannotBeNegative");
                }
                this.height = value;
            }
        }
        public double Left =>
            this.x;
        public double Top =>
            this.y;
        public double Right
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return (this.x + this.width);
            }
        }
        public double Bottom
        {
            get
            {
                if (this.IsEmpty)
                {
                    return double.NegativeInfinity;
                }
                return (this.y + this.height);
            }
        }
        public XPoint TopLeft =>
            new XPoint(this.Left, this.Top);
        public XPoint TopRight =>
            new XPoint(this.Right, this.Top);
        public XPoint BottomLeft =>
            new XPoint(this.Left, this.Bottom);
        public XPoint BottomRight =>
            new XPoint(this.Right, this.Bottom);
        public XPoint Center =>
            new XPoint(this.x + (this.width / 2.0), this.y + (this.height / 2.0));
        public bool Contains(XPoint point) => 
            this.Contains(point.x, point.y);

        public bool Contains(double x, double y)
        {
            if (this.IsEmpty)
            {
                return false;
            }
            return this.ContainsInternal(x, y);
        }

        public bool Contains(XRect rect) => 
            ((((!this.IsEmpty && !rect.IsEmpty) && ((this.x <= rect.x) && (this.y <= rect.y))) && ((this.x + this.width) >= (rect.x + rect.width))) && ((this.y + this.height) >= (rect.y + rect.height)));

        public bool IntersectsWith(XRect rect) => 
            ((((!this.IsEmpty && !rect.IsEmpty) && ((rect.Left <= this.Right) && (rect.Right >= this.Left))) && (rect.Top <= this.Bottom)) && (rect.Bottom >= this.Top));

        public void Intersect(XRect rect)
        {
            if (!this.IntersectsWith(rect))
            {
                this = Empty;
            }
            else
            {
                double num = Math.Max(this.Left, rect.Left);
                double num2 = Math.Max(this.Top, rect.Top);
                this.width = Math.Max((double) (Math.Min(this.Right, rect.Right) - num), (double) 0.0);
                this.height = Math.Max((double) (Math.Min(this.Bottom, rect.Bottom) - num2), (double) 0.0);
                this.x = num;
                this.y = num2;
            }
        }

        public static XRect Intersect(XRect rect1, XRect rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        public void Union(XRect rect)
        {
            if (this.IsEmpty)
            {
                this = rect;
            }
            else if (!rect.IsEmpty)
            {
                double num = Math.Min(this.Left, rect.Left);
                double num2 = Math.Min(this.Top, rect.Top);
                if ((rect.Width == double.PositiveInfinity) || (this.Width == double.PositiveInfinity))
                {
                    this.width = double.PositiveInfinity;
                }
                else
                {
                    double num3 = Math.Max(this.Right, rect.Right);
                    this.width = Math.Max((double) (num3 - num), (double) 0.0);
                }
                if ((rect.Height == double.PositiveInfinity) || (this.height == double.PositiveInfinity))
                {
                    this.height = double.PositiveInfinity;
                }
                else
                {
                    double num4 = Math.Max(this.Bottom, rect.Bottom);
                    this.height = Math.Max((double) (num4 - num2), (double) 0.0);
                }
                this.x = num;
                this.y = num2;
            }
        }

        public static XRect Union(XRect rect1, XRect rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        public void Union(XPoint point)
        {
            this.Union(new XRect(point, point));
        }

        public static XRect Union(XRect rect, XPoint point)
        {
            rect.Union(new XRect(point, point));
            return rect;
        }

        public void Offset(XVector offsetVector)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("CannotCallMethod");
            }
            this.x += offsetVector.x;
            this.y += offsetVector.y;
        }

        public void Offset(double offsetX, double offsetY)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("CannotCallMethod");
            }
            this.x += offsetX;
            this.y += offsetY;
        }

        public static XRect Offset(XRect rect, XVector offsetVector)
        {
            rect.Offset(offsetVector.X, offsetVector.Y);
            return rect;
        }

        public static XRect Offset(XRect rect, double offsetX, double offsetY)
        {
            rect.Offset(offsetX, offsetY);
            return rect;
        }

        public static XRect operator +(XRect rect, XPoint point) => 
            new XRect(rect.x + point.x, rect.Y + point.y, rect.width, rect.height);

        public static XRect operator -(XRect rect, XPoint point) => 
            new XRect(rect.x - point.x, rect.Y - point.y, rect.width, rect.height);

        public void Inflate(XSize size)
        {
            this.Inflate(size.width, size.height);
        }

        public void Inflate(double width, double height)
        {
            if (this.IsEmpty)
            {
                throw new InvalidOperationException("CannotCallMethod");
            }
            this.x -= width;
            this.y -= height;
            this.width += width;
            this.width += width;
            this.height += height;
            this.height += height;
            if ((this.width < 0.0) || (this.height < 0.0))
            {
                this = s_empty;
            }
        }

        public static XRect Inflate(XRect rect, XSize size)
        {
            rect.Inflate(size.width, size.height);
            return rect;
        }

        public static XRect Inflate(XRect rect, double width, double height)
        {
            rect.Inflate(width, height);
            return rect;
        }

        public static XRect Transform(XRect rect, XMatrix matrix)
        {
            XMatrix.MatrixHelper.TransformRect(ref rect, ref matrix);
            return rect;
        }

        public void Transform(XMatrix matrix)
        {
            XMatrix.MatrixHelper.TransformRect(ref this, ref matrix);
        }

        public void Scale(double scaleX, double scaleY)
        {
            if (!this.IsEmpty)
            {
                this.x *= scaleX;
                this.y *= scaleY;
                this.width *= scaleX;
                this.height *= scaleY;
                if (scaleX < 0.0)
                {
                    this.x += this.width;
                    this.width *= -1.0;
                }
                if (scaleY < 0.0)
                {
                    this.y += this.height;
                    this.height *= -1.0;
                }
            }
        }

        public RectangleF ToRectangleF() => 
            new RectangleF((float) this.x, (float) this.y, (float) this.width, (float) this.height);

        public static implicit operator XRect(Rectangle rect) => 
            new XRect((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);

        public static implicit operator XRect(RectangleF rect) => 
            new XRect((double) rect.X, (double) rect.Y, (double) rect.Width, (double) rect.Height);

        private bool ContainsInternal(double x, double y) => 
            ((((x >= this.x) && ((x - this.width) <= this.x)) && (y >= this.y)) && ((y - this.height) <= this.y));

        private static XRect CreateEmptyRect() => 
            new XRect { 
                x = double.PositiveInfinity,
                y = double.PositiveInfinity,
                width = double.NegativeInfinity,
                height = double.NegativeInfinity
            };

        static XRect()
        {
            s_empty = CreateEmptyRect();
        }
    }
}

