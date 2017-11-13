namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Windows;
    using System.Windows.Media;

    [FriendAccessAllowed]
    internal static class MatrixUtil
    {
        internal static void MultiplyMatrix(ref Matrix matrix1, ref Matrix matrix2)
        {
            MatrixTypes types = matrix1._type;
            MatrixTypes types2 = matrix2._type;
            if (types2 != MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                if (types == MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    matrix1 = matrix2;
                }
                else if (types2 == MatrixTypes.TRANSFORM_IS_TRANSLATION)
                {
                    matrix1._offsetX += matrix2._offsetX;
                    matrix1._offsetY += matrix2._offsetY;
                    if (types != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        matrix1._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
                else if (types == MatrixTypes.TRANSFORM_IS_TRANSLATION)
                {
                    double num = matrix1._offsetX;
                    double num2 = matrix1._offsetY;
                    matrix1 = matrix2;
                    matrix1._offsetX = ((num * matrix2._m11) + (num2 * matrix2._m21)) + matrix2._offsetX;
                    matrix1._offsetY = ((num * matrix2._m12) + (num2 * matrix2._m22)) + matrix2._offsetY;
                    if (types2 == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        matrix1._type = MatrixTypes.TRANSFORM_IS_UNKNOWN;
                    }
                    else
                    {
                        matrix1._type = MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION;
                    }
                }
                else
                {
                    switch (((((int) types) << 4) | types2))
                    {
                        case 0x22:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            return;

                        case 0x23:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            matrix1._offsetX = matrix2._offsetX;
                            matrix1._offsetY = matrix2._offsetY;
                            matrix1._type = MatrixTypes.TRANSFORM_IS_SCALING | MatrixTypes.TRANSFORM_IS_TRANSLATION;
                            return;

                        case 0x24:
                        case 0x34:
                        case 0x42:
                        case 0x43:
                        case 0x44:
                            matrix1 = new Matrix((matrix1._m11 * matrix2._m11) + (matrix1._m12 * matrix2._m21), (matrix1._m11 * matrix2._m12) + (matrix1._m12 * matrix2._m22), (matrix1._m21 * matrix2._m11) + (matrix1._m22 * matrix2._m21), (matrix1._m21 * matrix2._m12) + (matrix1._m22 * matrix2._m22), ((matrix1._offsetX * matrix2._m11) + (matrix1._offsetY * matrix2._m21)) + matrix2._offsetX, ((matrix1._offsetX * matrix2._m12) + (matrix1._offsetY * matrix2._m22)) + matrix2._offsetY);
                            return;

                        case 50:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            matrix1._offsetX *= matrix2._m11;
                            matrix1._offsetY *= matrix2._m22;
                            return;

                        case 0x33:
                            matrix1._m11 *= matrix2._m11;
                            matrix1._m22 *= matrix2._m22;
                            matrix1._offsetX = (matrix2._m11 * matrix1._offsetX) + matrix2._offsetX;
                            matrix1._offsetY = (matrix2._m22 * matrix1._offsetY) + matrix2._offsetY;
                            return;
                    }
                }
            }
        }

        internal static void PrependOffset(ref Matrix matrix, double offsetX, double offsetY)
        {
            if (matrix._type == MatrixTypes.TRANSFORM_IS_IDENTITY)
            {
                matrix = new Matrix(1.0, 0.0, 0.0, 1.0, offsetX, offsetY);
                matrix._type = MatrixTypes.TRANSFORM_IS_TRANSLATION;
            }
            else
            {
                matrix._offsetX += (matrix._m11 * offsetX) + (matrix._m21 * offsetY);
                matrix._offsetY += (matrix._m12 * offsetX) + (matrix._m22 * offsetY);
                if (matrix._type != MatrixTypes.TRANSFORM_IS_UNKNOWN)
                {
                    matrix._type |= MatrixTypes.TRANSFORM_IS_TRANSLATION;
                }
            }
        }

        internal static void TransformRect(ref Rect rect, ref Matrix matrix)
        {
            if (!rect.IsEmpty)
            {
                MatrixTypes types = matrix._type;
                if (types != MatrixTypes.TRANSFORM_IS_IDENTITY)
                {
                    if ((types & MatrixTypes.TRANSFORM_IS_SCALING) != MatrixTypes.TRANSFORM_IS_IDENTITY)
                    {
                        rect._x *= matrix._m11;
                        rect._y *= matrix._m22;
                        rect._width *= matrix._m11;
                        rect._height *= matrix._m22;
                        if (rect._width < 0.0)
                        {
                            rect._x += rect._width;
                            rect._width = -rect._width;
                        }
                        if (rect._height < 0.0)
                        {
                            rect._y += rect._height;
                            rect._height = -rect._height;
                        }
                    }
                    if ((types & MatrixTypes.TRANSFORM_IS_TRANSLATION) != MatrixTypes.TRANSFORM_IS_IDENTITY)
                    {
                        rect._x += matrix._offsetX;
                        rect._y += matrix._offsetY;
                    }
                    if (types == MatrixTypes.TRANSFORM_IS_UNKNOWN)
                    {
                        Point point = matrix.Transform(rect.TopLeft);
                        Point point2 = matrix.Transform(rect.TopRight);
                        Point point3 = matrix.Transform(rect.BottomRight);
                        Point point4 = matrix.Transform(rect.BottomLeft);
                        rect._x = Math.Min(Math.Min(point.X, point2.X), Math.Min(point3.X, point4.X));
                        rect._y = Math.Min(Math.Min(point.Y, point2.Y), Math.Min(point3.Y, point4.Y));
                        rect._width = Math.Max(Math.Max(point.X, point2.X), Math.Max(point3.X, point4.X)) - rect._x;
                        rect._height = Math.Max(Math.Max(point.Y, point2.Y), Math.Max(point3.Y, point4.Y)) - rect._y;
                    }
                }
            }
        }
    }
}

