namespace System.Windows.Media
{
    using MS.Internal;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Markup;
    using System.Windows.Media.Converters;

    [Serializable, StructLayout(LayoutKind.Sequential), TypeConverter(typeof(MatrixConverter)), ValueSerializer(typeof(MatrixValueSerializer))]
    public struct Matrix : IFormattable
    {
        private const int c_identityHashCode = 0;
        private static Matrix s_identity;
        internal double _m11;
        internal double _m12;
        internal double _m21;
        internal double _m22;
        internal double _offsetX;
        internal double _offsetY;
        internal MatrixTypes _type;
        internal int _padding;
        public Matrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            this._padding = 0;
            this.DeriveMatrixType();
        }

        public static Matrix Identity =>
            s_identity;
        public void SetIdentity()
        {
            this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
        }

        public bool IsIdentity =>
            ((this._type == MatrixTypes.TRANSFORM_IS_IDENTITY) || (((((this._m11 == 1.0) && (this._m12 == 0.0)) && ((this._m21 == 0.0) && (this._m22 == 1.0))) && (this._offsetX == 0.0)) && (this._offsetY == 0.0)));
        public static Matrix operator *(Matrix trans1, Matrix trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        public static Matrix Multiply(Matrix trans1, Matrix trans2)
        {
            MatrixUtil.MultiplyMatrix(ref trans1, ref trans2);
            return trans1;
        }

        public void Append(Matrix matrix)
        {
            this *= matrix;
        }

        public void Prepend(Matrix matrix)
        {
            this = matrix * this;
        }

        public void Rotate(double angle)
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * 0.017453292519943295);
        }

        public void RotatePrepend(double angle)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * 0.017453292519943295) * this;
        }

        public void RotateAt(double angle, double centerX, double centerY)
        {
            angle = angle % 360.0;
            this *= CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY);
        }

        public void RotateAtPrepend(double angle, double centerX, double centerY)
        {
            angle = angle % 360.0;
            this = CreateRotationRadians(angle * 0.017453292519943295, centerX, centerY) * this;
        }

        public void Scale(double scaleX, double scaleY)
        {
            this *= CreateScaling(scaleX, scaleY);
        }

        public void ScalePrepend(double scaleX, double scaleY)
        {
            this = CreateScaling(scaleX, scaleY) * this;
        }

        public void ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
        {
            this *= CreateScaling(scaleX, scaleY, centerX, centerY);
        }

        public void ScaleAtPrepend(double scaleX, double scaleY, double centerX, double centerY)
        {
            this = CreateScaling(scaleX, scaleY, centerX, centerY) * this;
        }

        public void Skew(double skewX, double skewY)
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

        public void Translate(double offsetX, double offsetY)
        {
            if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                this.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
            }
            else if (this._type == MatrixTypes.TRANSFORM_IS_UNKNOWN)
            {
                this._offsetX += offsetX;
                this._offsetY += offsetY;
            }
            else
            {
                this._offsetX += offsetX;
                this._offsetY += offsetY;
                this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }
        }

        public void TranslatePrepend(double offsetX, double offsetY)
        {
            this = CreateTranslation(offsetX, offsetY) * this;
        }

        public Point Transform(Point point)
        {
            Point point2 = point;
            this.MultiplyPoint(ref point2._x, ref point2._y);
            return point2;
        }

        public void Transform(Point[] points)
        {
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    this.MultiplyPoint(ref points[i]._x, ref points[i]._y);
                }
            }
        }

        public Vector Transform(Vector vector)
        {
            Vector vector2 = vector;
            this.MultiplyVector(ref vector2._x, ref vector2._y);
            return vector2;
        }

        public void Transform(Vector[] vectors)
        {
            if (vectors != null)
            {
                for (int i = 0; i < vectors.Length; i++)
                {
                    this.MultiplyVector(ref vectors[i]._x, ref vectors[i]._y);
                }
            }
        }

        public double Determinant
        {
            get
            {
                switch (this._type)
                {
                    case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                        return 1.0;

                    case MatrixTypes.TRANSFORM_IS_SCALING:
                    case (MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION):
                        return (this._m11 * this._m22);
                }
                return ((this._m11 * this._m22) - (this._m12 * this._m21));
            }
        }
        public bool HasInverse =>
            !DoubleUtil.IsZero(this.Determinant);
        public void Invert()
        {
            double determinant = this.Determinant;
            if (DoubleUtil.IsZero(determinant))
            {
                throw new InvalidOperationException(System.Windows.SR.Get("Transform_NotInvertible"));
            }
            switch (this._type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    break;

                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    this._offsetX = -this._offsetX;
                    this._offsetY = -this._offsetY;
                    return;

                case MatrixTypes.TRANSFORM_IS_SCALING:
                    this._m11 = 1.0 / this._m11;
                    this._m22 = 1.0 / this._m22;
                    return;

                case (MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION):
                    this._m11 = 1.0 / this._m11;
                    this._m22 = 1.0 / this._m22;
                    this._offsetX = -this._offsetX * this._m11;
                    this._offsetY = -this._offsetY * this._m22;
                    return;

                default:
                {
                    double num2 = 1.0 / determinant;
                    this.SetMatrix(this._m22 * num2, -this._m12 * num2, -this._m21 * num2, this._m11 * num2, ((this._m21 * this._offsetY) - (this._offsetX * this._m22)) * num2, ((this._offsetX * this._m12) - (this._m11 * this._offsetY)) * num2, MatrixTypes.TRANSFORM_IS_UNKNOWN);
                    break;
                }
            }
        }

        public double M11
        {
            get
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0;
                }
                return this._m11;
            }
            set
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(value, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
                }
                else
                {
                    this._m11 = value;
                    if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypes.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }
        public double M12
        {
            get
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._m12;
            }
            set
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, value, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    this._m12 = value;
                    this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
            }
        }
        public double M21
        {
            get
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._m21;
            }
            set
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, value, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
                }
                else
                {
                    this._m21 = value;
                    this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                }
            }
        }
        public double M22
        {
            get
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 1.0;
                }
                return this._m22;
            }
            set
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, value, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
                }
                else
                {
                    this._m22 = value;
                    if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypes.TRANSFORM_IS_SCALING;
                    }
                }
            }
        }
        public double OffsetX
        {
            get
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._offsetX;
            }
            set
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, 1.0, value, 0.0, MatrixTypes.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    this._offsetX = value;
                    if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }
        public double OffsetY
        {
            get
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    return 0.0;
                }
                return this._offsetY;
            }
            set
            {
                if (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    this.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, value, MatrixTypes.TRANSFORM_IS_TRANSLATION);
                }
                else
                {
                    this._offsetY = value;
                    if (this._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
            }
        }
        internal void MultiplyVector(ref double x, ref double y)
        {
            switch (this._type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    return;

                case MatrixTypes.TRANSFORM_IS_SCALING:
                case (MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION):
                    x *= this._m11;
                    y *= this._m22;
                    return;
            }
            double num = y * this._m21;
            double num2 = x * this._m12;
            x *= this._m11;
            x += num;
            y *= this._m22;
            y += num2;
        }

        internal void MultiplyPoint(ref double x, ref double y)
        {
            switch (this._type)
            {
                case MatrixTypes.TRANSFORM_IS_IDENTITY:
                    return;

                case MatrixTypes.TRANSFORM_IS_TRANSLATION:
                    x += this._offsetX;
                    y += this._offsetY;
                    return;

                case MatrixTypes.TRANSFORM_IS_SCALING:
                    x *= this._m11;
                    y *= this._m22;
                    return;

                case (MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION):
                    x *= this._m11;
                    x += this._offsetX;
                    y *= this._m22;
                    y += this._offsetY;
                    return;
            }
            double num = (y * this._m21) + this._offsetX;
            double num2 = (x * this._m12) + this._offsetY;
            x *= this._m11;
            x += num;
            y *= this._m22;
            y += num2;
        }

        internal static Matrix CreateRotationRadians(double angle) => 
            CreateRotationRadians(angle, 0.0, 0.0);

        internal static Matrix CreateRotationRadians(double angle, double centerX, double centerY)
        {
            Matrix matrix = new Matrix();
            double num = Math.Sin(angle);
            double num2 = Math.Cos(angle);
            double offsetX = (centerX * (1.0 - num2)) + (centerY * num);
            double offsetY = (centerY * (1.0 - num2)) - (centerX * num);
            matrix.SetMatrix(num2, num, -num, num2, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_UNKNOWN);
            return matrix;
        }

        internal static Matrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
        {
            Matrix matrix = new Matrix();
            matrix.SetMatrix(scaleX, 0.0, 0.0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY), MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION);
            return matrix;
        }

        internal static Matrix CreateScaling(double scaleX, double scaleY)
        {
            Matrix matrix = new Matrix();
            matrix.SetMatrix(scaleX, 0.0, 0.0, scaleY, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_SCALING);
            return matrix;
        }

        internal static Matrix CreateSkewRadians(double skewX, double skewY)
        {
            Matrix matrix = new Matrix();
            matrix.SetMatrix(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_UNKNOWN);
            return matrix;
        }

        internal static Matrix CreateTranslation(double offsetX, double offsetY)
        {
            Matrix matrix = new Matrix();
            matrix.SetMatrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY, MatrixTypes.TRANSFORM_IS_TRANSLATION);
            return matrix;
        }

        private static Matrix CreateIdentity()
        {
            Matrix matrix = new Matrix();
            matrix.SetMatrix(1.0, 0.0, 0.0, 1.0, 0.0, 0.0, MatrixTypes.TRANSFORM_IS_IDENTITY);
            return matrix;
        }

        private void SetMatrix(double m11, double m12, double m21, double m22, double offsetX, double offsetY, MatrixTypes type)
        {
            this._m11 = m11;
            this._m12 = m12;
            this._m21 = m21;
            this._m22 = m22;
            this._offsetX = offsetX;
            this._offsetY = offsetY;
            this._type = type;
        }

        private void DeriveMatrixType()
        {
            this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
            if ((this._m21 != 0.0) || (this._m12 != 0.0))
            {
                this._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
            }
            else
            {
                if ((this._m11 != 1.0) || (this._m22 != 1.0))
                {
                    this._type = MatrixTypes.TRANSFORM_IS_SCALING;
                }
                if ((this._offsetX != 0.0) || (this._offsetY != 0.0))
                {
                    this._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
                if ((this._type & (MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION)) == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    this._type = MatrixTypes.TRANSFORM_IS_IDENTITY;
                }
            }
        }

        [Conditional("DEBUG")]
        private void Debug_CheckType()
        {
            switch (this._type)
            {
            }
        }

        private bool IsDistinguishedIdentity =>
            (this._type == MatrixTypes.TRANSFORM_IS_IDENTITY);
        public static bool operator ==(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            return (((((matrix1.M11 == matrix2.M11) && (matrix1.M12 == matrix2.M12)) && ((matrix1.M21 == matrix2.M21) && (matrix1.M22 == matrix2.M22))) && (matrix1.OffsetX == matrix2.OffsetX)) && (matrix1.OffsetY == matrix2.OffsetY));
        }

        public static bool operator !=(Matrix matrix1, Matrix matrix2) => 
            !(matrix1 == matrix2);

        public static bool Equals(Matrix matrix1, Matrix matrix2)
        {
            if (matrix1.IsDistinguishedIdentity || matrix2.IsDistinguishedIdentity)
            {
                return (matrix1.IsIdentity == matrix2.IsIdentity);
            }
            return ((((matrix1.M11.Equals(matrix2.M11) && matrix1.M12.Equals(matrix2.M12)) && (matrix1.M21.Equals(matrix2.M21) && matrix1.M22.Equals(matrix2.M22))) && matrix1.OffsetX.Equals(matrix2.OffsetX)) && matrix1.OffsetY.Equals(matrix2.OffsetY));
        }

        public override bool Equals(object o)
        {
            if ((o == null) || !(o is Matrix))
            {
                return false;
            }
            Matrix matrix = (Matrix) o;
            return Equals(this, matrix);
        }

        public bool Equals(Matrix value) => 
            Equals(this, value);

        public override int GetHashCode()
        {
            if (this.IsDistinguishedIdentity)
            {
                return 0;
            }
            return (((((this.M11.GetHashCode() ^ this.M12.GetHashCode()) ^ this.M21.GetHashCode()) ^ this.M22.GetHashCode()) ^ this.OffsetX.GetHashCode()) ^ this.OffsetY.GetHashCode());
        }

        public static Matrix Parse(string source)
        {
            Matrix identity;
            IFormatProvider englishUSCulture = TypeConverterHelper.EnglishUSCulture;
            TokenizerHelper helper = new TokenizerHelper(source, englishUSCulture);
            string str = helper.NextTokenRequired();
            if (str == "Identity")
            {
                identity = Identity;
            }
            else
            {
                identity = new Matrix(Convert.ToDouble(str, englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture), Convert.ToDouble(helper.NextTokenRequired(), englishUSCulture));
            }
            helper.LastTokenRequired();
            return identity;
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
            return string.Format(provider, "{1:" + format + "}{0}{2:" + format + "}{0}{3:" + format + "}{0}{4:" + format + "}{0}{5:" + format + "}{0}{6:" + format + "}", new object[] { numericListSeparator, this._m11, this._m12, this._m21, this._m22, this._offsetX, this._offsetY });
        }

        static Matrix()
        {
            s_identity = CreateIdentity();
        }
    }
}

