namespace PdfSharp.Drawing
{
    using PdfSharp.Internal;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), DebuggerDisplay("({M11}, {M12}, {M21}, {M22}, {OffsetX}, {OffsetY})")]
    public struct XMatrix : IFormattable
    {
        private double m11;
        private double m12;
        private double m21;
        private double m22;
        private double offsetX;
        private double offsetY;
        private XMatrixTypes type;
        private int padding;
        private static readonly XMatrix s_identity;
        public XMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.type = XMatrixTypes.Unknown;
            this.padding = 0;
            this.DeriveMatrixType();
        }

        public static XMatrix Identity =>
            s_identity;
        public void SetIdentity()
        {
            this.type = XMatrixTypes.Identity;
        }

        private void InitIdentity()
        {
            this.m11 = 1.0;
            this.m22 = 1.0;
        }

        public bool IsIdentity
        {
            get
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    return true;
                }
                if ((((this.m11 == 1.0) && (this.m12 == 0.0)) && ((this.m21 == 0.0) && (this.m22 == 1.0))) && ((this.offsetX == 0.0) && (this.offsetY == 0.0)))
                {
                    this.type = XMatrixTypes.Identity;
                    return true;
                }
                return false;
            }
        }
        [Obsolete("Use GetElements().")]
        public double[] Elements =>
            this.GetElements();
        public double[] GetElements()
        {
            if (this.type == XMatrixTypes.Identity)
            {
                double[] numArray = new double[6];
                numArray[0] = 1.0;
                numArray[3] = 1.0;
                return numArray;
            }
            return new double[] { this.m11, this.m12, this.m21, this.m22, this.offsetX, this.offsetY };
        }

        public static XMatrix operator *(XMatrix trans1, XMatrix trans2)
        {
            MatrixHelper.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        public static XMatrix Multiply(XMatrix trans1, XMatrix trans2)
        {
            MatrixHelper.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        public void Append(XMatrix matrix)
        {
            this *= matrix;
        }

        public void Prepend(XMatrix matrix)
        {
            this = matrix * this;
        }

        [Obsolete("Use Append.")]
        public void Multiply(XMatrix matrix)
        {
            this.Append(matrix);
        }

        [Obsolete("Use Prepend.")]
        public void MultiplyPrepend(XMatrix matrix)
        {
            this.Prepend(matrix);
        }

        public void Multiply(XMatrix matrix, XMatrixOrder order)
        {
            if (this.type == XMatrixTypes.Identity)
            {
                this.InitIdentity();
            }
            double num = this.M11;
            double num2 = this.M12;
            double num3 = this.M21;
            double num4 = this.M22;
            double offsetX = this.OffsetX;
            double offsetY = this.OffsetY;
            if (order == XMatrixOrder.Append)
            {
                this.m11 = (num * matrix.M11) + (num2 * matrix.M21);
                this.m12 = (num * matrix.M12) + (num2 * matrix.M22);
                this.m21 = (num3 * matrix.M11) + (num4 * matrix.M21);
                this.m22 = (num3 * matrix.M12) + (num4 * matrix.M22);
                this.offsetX = ((offsetX * matrix.M11) + (offsetY * matrix.M21)) + matrix.OffsetX;
                this.offsetY = ((offsetX * matrix.M12) + (offsetY * matrix.M22)) + matrix.OffsetY;
            }
            else
            {
                this.m11 = (num * matrix.M11) + (num3 * matrix.M12);
                this.m12 = (num2 * matrix.M11) + (num4 * matrix.M12);
                this.m21 = (num * matrix.M21) + (num3 * matrix.M22);
                this.m22 = (num2 * matrix.M21) + (num4 * matrix.M22);
                this.offsetX = ((num * matrix.OffsetX) + (num3 * matrix.OffsetY)) + offsetX;
                this.offsetY = ((num2 * matrix.OffsetX) + (num4 * matrix.OffsetY)) + offsetY;
            }
            this.DeriveMatrixType();
        }

        [Obsolete("Use TranslateAppend or TranslatePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Translate(double offsetX, double offsetY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void TranslateAppend(double offsetX, double offsetY)
        {
            if (this.type == XMatrixTypes.Identity)
            {
                this.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, XMatrixTypes.Translation);
            }
            else if (this.type == XMatrixTypes.Unknown)
            {
                this.offsetX += offsetX;
                this.offsetY += offsetY;
            }
            else
            {
                this.offsetX += offsetX;
                this.offsetY += offsetY;
                this.type |= XMatrixTypes.Translation;
            }
        }

        public void TranslatePrepend(double offsetX, double offsetY)
        {
            this = CreateTranslation(offsetX, offsetY) * this;
        }

        public void Translate(double offsetX, double offsetY, XMatrixOrder order)
        {
            if (this.type == XMatrixTypes.Identity)
            {
                this.InitIdentity();
            }
            if (order == XMatrixOrder.Append)
            {
                this.offsetX += offsetX;
                this.offsetY += offsetY;
            }
            else
            {
                this.offsetX += (offsetX * this.m11) + (offsetY * this.m21);
                this.offsetY += (offsetX * this.m12) + (offsetY * this.m22);
            }
            this.DeriveMatrixType();
        }

        [Obsolete("Use ScaleAppend or ScalePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Scale(double scaleX, double scaleY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void ScaleAppend(double scaleX, double scaleY)
        {
            this *= CreateScaling(scaleX, scaleY);
        }

        public void ScalePrepend(double scaleX, double scaleY)
        {
            this = CreateScaling(scaleX, scaleY) * this;
        }

        public void Scale(double scaleX, double scaleY, XMatrixOrder order)
        {
            if (this.type == XMatrixTypes.Identity)
            {
                this.InitIdentity();
            }
            if (order == XMatrixOrder.Append)
            {
                this.m11 *= scaleX;
                this.m12 *= scaleY;
                this.m21 *= scaleX;
                this.m22 *= scaleY;
                this.offsetX *= scaleX;
                this.offsetY *= scaleY;
            }
            else
            {
                this.m11 *= scaleX;
                this.m12 *= scaleX;
                this.m21 *= scaleY;
                this.m22 *= scaleY;
            }
            this.DeriveMatrixType();
        }

        [Obsolete("Use ScaleAppend or ScalePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Scale(double scaleXY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void ScaleAppend(double scaleXY)
        {
            this.Scale(scaleXY, scaleXY, XMatrixOrder.Append);
        }

        public void ScalePrepend(double scaleXY)
        {
            this.Scale(scaleXY, scaleXY, XMatrixOrder.Prepend);
        }

        public void Scale(double scaleXY, XMatrixOrder order)
        {
            this.Scale(scaleXY, scaleXY, order);
        }

        [Obsolete("Use ScaleAtAppend or ScaleAtPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void ScaleAtAppend(double scaleX, double scaleY, double centerX, double centerY)
        {
            this *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }

        public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
        {
            this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;
        }

        [Obsolete("Use RotateAppend or RotatePrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Rotate(double angle)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void RotateAppend(double angle)
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * 0.017453292519943295);
        }

        public void RotatePrepend(double angle)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * 0.017453292519943295) * this;
        }

        public void Rotate(double angle, XMatrixOrder order)
        {
            if (this.type == XMatrixTypes.Identity)
            {
                this.InitIdentity();
            }
            angle *= 0.017453292519943295;
            double num = Math.Cos(angle);
            double num2 = Math.Sin(angle);
            if (order == XMatrixOrder.Append)
            {
                double num3 = this.m11;
                double num4 = this.m12;
                double num5 = this.m21;
                double num6 = this.m22;
                double offsetX = this.offsetX;
                double offsetY = this.offsetY;
                this.m11 = (num3 * num) - (num4 * num2);
                this.m12 = (num3 * num2) + (num4 * num);
                this.m21 = (num5 * num) - (num6 * num2);
                this.m22 = (num5 * num2) + (num6 * num);
                this.offsetX = (offsetX * num) - (offsetY * num2);
                this.offsetY = (offsetX * num2) + (offsetY * num);
            }
            else
            {
                double num9 = this.m11;
                double num10 = this.m12;
                double num11 = this.m21;
                double num12 = this.m22;
                this.m11 = (num9 * num) + (num11 * num2);
                this.m12 = (num10 * num) + (num12 * num2);
                this.m21 = (-num9 * num2) + (num11 * num);
                this.m22 = (-num10 * num2) + (num12 * num);
            }
            this.DeriveMatrixType();
        }

        [Obsolete("Use RotateAtAppend or RotateAtPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void RotateAt(double angle, double centerX, double centerY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void RotateAtAppend(double angle, double centerX, double centerY)
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY);
        }

        public void RotateAtPrepend(double angle, double centerX, double centerY)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY) * this;
        }

        [Obsolete("Use RotateAtAppend or RotateAtPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void RotateAt(double angle, XPoint point)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void RotateAtAppend(double angle, XPoint point)
        {
            this.RotateAt(angle, point, XMatrixOrder.Append);
        }

        public void RotateAtPrepend(double angle, XPoint point)
        {
            this.RotateAt(angle, point, XMatrixOrder.Prepend);
        }

        public void RotateAt(double angle, XPoint point, XMatrixOrder order)
        {
            if (order == XMatrixOrder.Append)
            {
                angle = angle % 360.0;
                this *= CreateRotationRadians(angle * 0.017453292519943295, point.x, point.y);
            }
            else
            {
                angle = angle % 360.0;
                this = CreateRotationRadians(angle * 0.017453292519943295, point.x, point.y) * this;
            }
            this.DeriveMatrixType();
        }

        [Obsolete("Use ShearAppend or ShearPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Shear(double shearX, double shearY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void ShearAppend(double shearX, double shearY)
        {
            this.Shear(shearX, shearY, XMatrixOrder.Append);
        }

        public void ShearPrepend(double shearX, double shearY)
        {
            this.Shear(shearX, shearY, XMatrixOrder.Prepend);
        }

        public void Shear(double shearX, double shearY, XMatrixOrder order)
        {
            if (this.type == XMatrixTypes.Identity)
            {
                this.InitIdentity();
            }
            double num = this.m11;
            double num2 = this.m12;
            double num3 = this.m21;
            double num4 = this.m22;
            double offsetX = this.offsetX;
            double offsetY = this.offsetY;
            if (order == XMatrixOrder.Append)
            {
                this.m11 += shearX * num2;
                this.m12 += shearY * num;
                this.m21 += shearX * num4;
                this.m22 += shearY * num3;
                this.offsetX += shearX * offsetY;
                this.offsetY += shearY * offsetX;
            }
            else
            {
                this.m11 += shearY * num3;
                this.m12 += shearY * num4;
                this.m21 += shearX * num;
                this.m22 += shearX * num2;
            }
            this.DeriveMatrixType();
        }

        [Obsolete("Use SkewAppend or SkewPrepend explicitly, because in GDI+ and WPF the defaults are contrary.", true)]
        public void Skew(double skewX, double skewY)
        {
            throw new InvalidOperationException("Temporarily out of order.");
        }

        public void SkewAppend(double skewX, double skewY)
        {
            skewX = skewX % 360.0;
            skewY = skewY % 360.0;
            this *= CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295);
        }

        public void SkewPrepend(double skewX, double skewY)
        {
            skewX = skewX % 360.0;
            skewY = skewY % 360.0;
            this = CreateSkewRadians(skewX * 0.017453292519943295, skewY * 0.017453292519943295) * this;
        }

        public XPoint Transform(XPoint point)
        {
            XPoint point2 = point;
            this.MultiplyPoint(ref point2.x, ref point2.y);
            return point2;
        }

        public void Transform(XPoint[] points)
        {
            if (points != null)
            {
                int length = points.Length;
                for (int i = 0; i < length; i++)
                {
                    this.MultiplyPoint(ref points[i].x, ref points[i].y);
                }
            }
        }

        public void TransformPoints(XPoint[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (!this.IsIdentity)
            {
                int length = points.Length;
                for (int i = 0; i < length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    points[i].X = ((x * this.m11) + (y * this.m21)) + this.offsetX;
                    points[i].Y = ((x * this.m12) + (y * this.m22)) + this.offsetY;
                }
            }
        }

        public void TransformPoints(Point[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (!this.IsIdentity)
            {
                int length = points.Length;
                for (int i = 0; i < length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    points[i].X = (int) (((x * this.m11) + (y * this.m21)) + this.offsetX);
                    points[i].Y = (int) (((x * this.m12) + (y * this.m22)) + this.offsetY);
                }
            }
        }

        public XVector Transform(XVector vector)
        {
            XVector vector2 = vector;
            this.MultiplyVector(ref vector2.x, ref vector2.y);
            return vector2;
        }

        public void Transform(XVector[] vectors)
        {
            if (vectors != null)
            {
                int length = vectors.Length;
                for (int i = 0; i < length; i++)
                {
                    this.MultiplyVector(ref vectors[i].x, ref vectors[i].y);
                }
            }
        }

        public void TransformVectors(PointF[] points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points");
            }
            if (!this.IsIdentity)
            {
                int length = points.Length;
                for (int i = 0; i < length; i++)
                {
                    double x = points[i].X;
                    double y = points[i].Y;
                    points[i].X = (float) (((x * this.m11) + (y * this.m21)) + this.offsetX);
                    points[i].Y = (float) (((x * this.m12) + (y * this.m22)) + this.offsetY);
                }
            }
        }

        public double Determinant
        {
            get
            {
                switch (this.type)
                {
                    case XMatrixTypes.Identity:
                    case XMatrixTypes.Translation:
                        return 1.0;

                    case XMatrixTypes.Scaling:
                    case (XMatrixTypes.Scaling | XMatrixTypes.Translation):
                        return (this.m11 * this.m22);
                }
                return ((this.m11 * this.m22) - (this.m12 * this.m21));
            }
        }
        public bool HasInverse =>
            !DoubleUtil.IsZero(this.Determinant);
        public void Invert()
        {
            double determinant = this.Determinant;
            if (DoubleUtil.IsZero(determinant))
            {
                throw new InvalidOperationException("NotInvertible");
            }
            switch (this.type)
            {
                case XMatrixTypes.Identity:
                    break;

                case XMatrixTypes.Translation:
                    this.offsetX = -this.offsetX;
                    this.offsetY = -this.offsetY;
                    return;

                case XMatrixTypes.Scaling:
                    this.m11 = 1.0 / this.m11;
                    this.m22 = 1.0 / this.m22;
                    return;

                case (XMatrixTypes.Scaling | XMatrixTypes.Translation):
                    this.m11 = 1.0 / this.m11;
                    this.m22 = 1.0 / this.m22;
                    this.offsetX = -this.offsetX * this.m11;
                    this.offsetY = -this.offsetY * this.m22;
                    return;

                default:
                {
                    double num2 = 1.0 / determinant;
                    this.SetMatrix(this.m22 * num2, -this.m12 * num2, -this.m21 * num2, this.m11 * num2, ((this.m21 * this.offsetY) - (this.offsetX * this.m22)) * num2, ((this.offsetX * this.m12) - (this.m11 * this.offsetY)) * num2, XMatrixTypes.Unknown);
                    break;
                }
            }
        }

        public double M11
        {
            get
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    return 1.0;
                }
                return this.m11;
            }
            set
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    this.SetMatrix(value, 0.0, 0.0, 1.0, 0.0, 0.0, XMatrixTypes.Scaling);
                }
                else
                {
                    this.m11 = value;
                    if (this.type != XMatrixTypes.Unknown)
                    {
                        this.type |= XMatrixTypes.Scaling;
                    }
                }
            }
        }
        public double M12
        {
            get
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    return 0.0;
                }
                return this.m12;
            }
            set
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    this.SetMatrix(1.0, value, 0.0, 1.0, 0.0, 0.0, XMatrixTypes.Unknown);
                }
                else
                {
                    this.m12 = value;
                    this.type = XMatrixTypes.Unknown;
                }
            }
        }
        public double M21
        {
            get
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    return 0.0;
                }
                return this.m21;
            }
            set
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    this.SetMatrix(1.0, 0.0, value, 1.0, 0.0, 0.0, XMatrixTypes.Unknown);
                }
                else
                {
                    this.m21 = value;
                    this.type = XMatrixTypes.Unknown;
                }
            }
        }
        public double M22
        {
            get
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    return 1.0;
                }
                return this.m22;
            }
            set
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, value, 0.0, 0.0, XMatrixTypes.Scaling);
                }
                else
                {
                    this.m22 = value;
                    if (this.type != XMatrixTypes.Unknown)
                    {
                        this.type |= XMatrixTypes.Scaling;
                    }
                }
            }
        }
        public double OffsetX
        {
            get
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    return 0.0;
                }
                return this.offsetX;
            }
            set
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, 1.0, value, 0.0, XMatrixTypes.Translation);
                }
                else
                {
                    this.offsetX = value;
                    if (this.type != XMatrixTypes.Unknown)
                    {
                        this.type |= XMatrixTypes.Translation;
                    }
                }
            }
        }
        public double OffsetY
        {
            get
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    return 0.0;
                }
                return this.offsetY;
            }
            set
            {
                if (this.type == XMatrixTypes.Identity)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, value, XMatrixTypes.Translation);
                }
                else
                {
                    this.offsetY = value;
                    if (this.type != XMatrixTypes.Unknown)
                    {
                        this.type |= XMatrixTypes.Translation;
                    }
                }
            }
        }
        public Matrix ToGdiMatrix() => 
            new Matrix((float) this.m11, (float) this.m12, (float) this.m21, (float) this.m22, (float) this.offsetX, (float) this.offsetY);

        [Obsolete("Use ToGdiMatrix.")]
        public Matrix ToGdipMatrix() => 
            new Matrix((float) this.m11, (float) this.m12, (float) this.m21, (float) this.m22, (float) this.offsetX, (float) this.offsetY);

        public static explicit operator Matrix(XMatrix matrix)
        {
            if (matrix.IsIdentity)
            {
                return new Matrix();
            }
            return new Matrix((float) matrix.m11, (float) matrix.m12, (float) matrix.m21, (float) matrix.m22, (float) matrix.offsetX, (float) matrix.offsetY);
        }

        public static implicit operator XMatrix(Matrix matrix)
        {
            float[] elements = matrix.Elements;
            return new XMatrix((double) elements[0], (double) elements[1], (double) elements[2], (double) elements[3], (double) elements[4], (double) elements[5]);
        }

        public static bool operator ==(XMatrix matrix1, XMatrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            return (((((matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12)) && ((matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22))) && (matrix1.OffsetX == matrix2.OffsetX)) && (matrix1.OffsetY == matrix2.OffsetY));
        }

        public static bool operator !=(XMatrix matrix1, XMatrix matrix2) => 
            !(matrix1 == matrix2);

        public static bool Equals(XMatrix matrix1, XMatrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            return ((((matrix1.M11.Equals(matrix2.M11) && matrix1.M12.Equals(matrix2.M12)) && (matrix1.M21.Equals(matrix2.M21) && matrix1.M22.Equals(matrix2.M22))) && matrix1.OffsetX.Equals(matrix2.OffsetX)) && matrix1.OffsetY.Equals(matrix2.OffsetY));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is XMatrix))
            {
                return false;
            }
            XMatrix matrix = (XMatrix) o;
            return Equals(this, matrix);
        }

        public bool Equals(XMatrix value) => 
            Equals(this, value);

        public override int GetHashCode()
        {
            if (this.IsDistinguishedIdentity)
            {
                return 0;
            }
            return (((((this.M11.GetHashCode() ^ this.M12.GetHashCode()) ^ this.M21.GetHashCode()) ^ this.M22.GetHashCode()) ^ this.OffsetX.GetHashCode()) ^ this.OffsetY.GetHashCode());
        }

        public static XMatrix Parse(string source)
        {
            IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
            TokenizerHelper helper = new TokenizerHelper(source, invariantCulture);
            string str = helper.NextTokenRequired();
            XMatrix matrix = (str == "Identity") ? Identity : new XMatrix(Convert.ToDouble(str, invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture), Convert.ToDouble(helper.NextTokenRequired(), invariantCulture));
            helper.LastTokenRequired();
            return matrix;
        }

        public override string ToString() => 
            this.ConvertToString(null, null);

        public string ToString(IFormatProvider provider) => 
            this.ConvertToString(null, provider);

        string IFormattable.ToString(string format, IFormatProvider provider) => 
            this.ConvertToString(format, provider);

        internal string ConvertToString(string format, IFormatProvider provider)
        {
            if (this.IsIdentity)
            {
                return "Identity";
            }
            char numericListSeparator = TokenizerHelper.GetNumericListSeparator(provider);
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}", new object[] { numericListSeparator, this.m11, this.m12, this.m21, this.m22, this.offsetX, this.offsetY });
        }

        internal void MultiplyVector(ref double x, ref double y)
        {
            switch (this.type)
            {
                case XMatrixTypes.Identity:
                case XMatrixTypes.Translation:
                    return;

                case XMatrixTypes.Scaling:
                case (XMatrixTypes.Scaling | XMatrixTypes.Translation):
                    x *= this.m11;
                    y *= this.m22;
                    return;
            }
            double num = y * this.m21;
            double num2 = x * this.m12;
            x *= this.m11;
            x += num;
            y *= this.m22;
            y += num2;
        }

        internal void MultiplyPoint(ref double x, ref double y)
        {
            switch (this.type)
            {
                case XMatrixTypes.Identity:
                    return;

                case XMatrixTypes.Translation:
                    x += this.offsetX;
                    y += this.offsetY;
                    return;

                case XMatrixTypes.Scaling:
                    x *= this.m11;
                    y *= this.m22;
                    return;

                case (XMatrixTypes.Scaling | XMatrixTypes.Translation):
                    x *= this.m11;
                    x += this.offsetX;
                    y *= this.m22;
                    y += this.offsetY;
                    return;
            }
            double num = (y * this.m21) + this.offsetX;
            double num2 = (x * this.m12) + this.offsetY;
            x *= this.m11;
            x += num;
            y *= this.m22;
            y += num2;
        }

        internal static XMatrix CreateRotationRadians(double angle) => 
            CreateRotationRadians(angle, 0.0, 0.0);

        internal static XMatrix CreateRotationRadians(double angle, double centerX, double centerY)
        {
            XMatrix matrix = new XMatrix();
            double num = Math.Sin(angle);
            double num2 = Math.Cos(angle);
            double offsetX = (centerX * (1.0 - num2)) + (centerY * num);
            double offsetY = (centerY * (1.0 - num2)) - (centerX * num);
            matrix.SetMatrix(num2, num, -num, num2, offsetX, offsetY, XMatrixTypes.Unknown);
            return matrix;
        }

        internal static XMatrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(scaleX, 0.0, 0.0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY), XMatrixTypes.Scaling | XMatrixTypes.Translation);
            return matrix;
        }

        internal static XMatrix CreateScaling(double scaleX, double scaleY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0, XMatrixTypes.Scaling);
            return matrix;
        }

        internal static XMatrix CreateSkewRadians(double skewX, double skewY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0, XMatrixTypes.Unknown);
            return matrix;
        }

        internal static XMatrix CreateTranslation(double offsetX, double offsetY)
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, XMatrixTypes.Translation);
            return matrix;
        }

        private static XMatrix CreateIdentity()
        {
            XMatrix matrix = new XMatrix();
            matrix.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0, XMatrixTypes.Identity);
            return matrix;
        }

        private void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY, XMatrixTypes type)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
            this.offsetX = offsetX;
            this.offsetY = offsetY;
            this.type = type;
        }

        private void DeriveMatrixType()
        {
            this.type = XMatrixTypes.Identity;
            if ((this.m21 != 0.0) || (this.m12 != 0.0))
            {
                this.type = XMatrixTypes.Unknown;
            }
            else
            {
                if ((this.m11 != 1.0) || (this.m22 != 1.0))
                {
                    this.type = XMatrixTypes.Scaling;
                }
                if ((this.offsetX != 0.0) || (this.offsetY != 0.0))
                {
                    this.type |= XMatrixTypes.Translation;
                }
                if ((this.type & (XMatrixTypes.Scaling | XMatrixTypes.Translation)) == XMatrixTypes.Identity)
                {
                    this.type = XMatrixTypes.Identity;
                }
            }
        }

        private bool IsDistinguishedIdentity =>
            (this.type == XMatrixTypes.Identity);
        static XMatrix()
        {
            s_identity = CreateIdentity();
        }
        internal static class MatrixHelper
        {
            internal static void MultiplyMatrix(ref XMatrix matrix1, ref XMatrix matrix2)
            {
                XMatrix.XMatrixTypes type = matrix1.type;
                XMatrix.XMatrixTypes types2 = matrix2.type;
                if (types2 != XMatrix.XMatrixTypes.Identity)
                {
                    if (type == XMatrix.XMatrixTypes.Identity)
                    {
                        matrix1 = matrix2;
                    }
                    else if (types2 == XMatrix.XMatrixTypes.Translation)
                    {
                        matrix1.offsetX += matrix2.offsetX;
                        matrix1.offsetY += matrix2.offsetY;
                        if (type != XMatrix.XMatrixTypes.Unknown)
                        {
                            matrix1.type |= XMatrix.XMatrixTypes.Translation;
                        }
                    }
                    else if (type == XMatrix.XMatrixTypes.Translation)
                    {
                        double offsetX = matrix1.offsetX;
                        double offsetY = matrix1.offsetY;
                        matrix1 = matrix2;
                        matrix1.offsetX = ((offsetX * matrix2.m11) + (offsetY * matrix2.m21)) + matrix2.offsetX;
                        matrix1.offsetY = ((offsetX * matrix2.m12) + (offsetY * matrix2.m22)) + matrix2.offsetY;
                        if (types2 == XMatrix.XMatrixTypes.Unknown)
                        {
                            matrix1.type = XMatrix.XMatrixTypes.Unknown;
                        }
                        else
                        {
                            matrix1.type = XMatrix.XMatrixTypes.Scaling | XMatrix.XMatrixTypes.Translation;
                        }
                    }
                    else
                    {
                        switch (((((int) type) << 4) | types2))
                        {
                            case 0x22:
                                matrix1.m11 *= matrix2.m11;
                                matrix1.m22 *= matrix2.m22;
                                return;

                            case 0x23:
                                matrix1.m11 *= matrix2.m11;
                                matrix1.m22 *= matrix2.m22;
                                matrix1.offsetX = matrix2.offsetX;
                                matrix1.offsetY = matrix2.offsetY;
                                matrix1.type = XMatrix.XMatrixTypes.Scaling | XMatrix.XMatrixTypes.Translation;
                                return;

                            case 0x24:
                            case 0x34:
                            case 0x42:
                            case 0x43:
                            case 0x44:
                                matrix1 = new XMatrix((matrix1.m11 * matrix2.m11) + (matrix1.m12 * matrix2.m21), (matrix1.m11 * matrix2.m12) + (matrix1.m12 * matrix2.m22), (matrix1.m21 * matrix2.m11) + (matrix1.m22 * matrix2.m21), (matrix1.m21 * matrix2.m12) + (matrix1.m22 * matrix2.m22), ((matrix1.offsetX * matrix2.m11) + (matrix1.offsetY * matrix2.m21)) + matrix2.offsetX, ((matrix1.offsetX * matrix2.m12) + (matrix1.offsetY * matrix2.m22)) + matrix2.offsetY);
                                return;

                            case 50:
                                matrix1.m11 *= matrix2.m11;
                                matrix1.m22 *= matrix2.m22;
                                matrix1.offsetX *= matrix2.m11;
                                matrix1.offsetY *= matrix2.m22;
                                return;

                            case 0x33:
                                matrix1.m11 *= matrix2.m11;
                                matrix1.m22 *= matrix2.m22;
                                matrix1.offsetX = (matrix2.m11 * matrix1.offsetX) + matrix2.offsetX;
                                matrix1.offsetY = (matrix2.m22 * matrix1.offsetY) + matrix2.offsetY;
                                break;

                            default:
                                return;
                        }
                    }
                }
            }

            internal static void PrependOffset(ref XMatrix matrix, double offsetX, double offsetY)
            {
                if (matrix.type == XMatrix.XMatrixTypes.Identity)
                {
                    matrix = new XMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
                    matrix.type = XMatrix.XMatrixTypes.Translation;
                }
                else
                {
                    matrix.offsetX += (matrix.m11 * offsetX) + (matrix.m21 * offsetY);
                    matrix.offsetY += (matrix.m12 * offsetX) + (matrix.m22 * offsetY);
                    if (matrix.type != XMatrix.XMatrixTypes.Unknown)
                    {
                        matrix.type |= XMatrix.XMatrixTypes.Translation;
                    }
                }
            }

            internal static void TransformRect(ref XRect rect, ref XMatrix matrix)
            {
                if (!rect.IsEmpty)
                {
                    XMatrix.XMatrixTypes type = matrix.type;
                    if (type != XMatrix.XMatrixTypes.Identity)
                    {
                        if ((type & XMatrix.XMatrixTypes.Scaling) != XMatrix.XMatrixTypes.Identity)
                        {
                            rect.x *= matrix.m11;
                            rect.y *= matrix.m22;
                            rect.width *= matrix.m11;
                            rect.height *= matrix.m22;
                            if (rect.width < 0.0)
                            {
                                rect.x += rect.width;
                                rect.width = -rect.width;
                            }
                            if (rect.height < 0.0)
                            {
                                rect.y += rect.height;
                                rect.height = -rect.height;
                            }
                        }
                        if ((type & XMatrix.XMatrixTypes.Translation) != XMatrix.XMatrixTypes.Identity)
                        {
                            rect.x += matrix.offsetX;
                            rect.y += matrix.offsetY;
                        }
                        if (type == XMatrix.XMatrixTypes.Unknown)
                        {
                            XPoint point = matrix.Transform(rect.TopLeft);
                            XPoint point2 = matrix.Transform(rect.TopRight);
                            XPoint point3 = matrix.Transform(rect.BottomRight);
                            XPoint point4 = matrix.Transform(rect.BottomLeft);
                            rect.x = Math.Min(Math.Min(point.X, point2.X), Math.Min(point3.X, point4.X));
                            rect.y = Math.Min(Math.Min(point.Y, point2.Y), Math.Min(point3.Y, point4.Y));
                            rect.width = Math.Max(Math.Max(point.X, point2.X), Math.Max(point3.X, point4.X)) - rect.x;
                            rect.height = Math.Max(Math.Max(point.Y, point2.Y), Math.Max(point3.Y, point4.Y)) - rect.y;
                        }
                    }
                }
            }
        }

        [Flags]
        internal enum XMatrixTypes
        {
            Identity = 0,
            Scaling = 2,
            Translation = 1,
            Unknown = 4
        }
    }
}

