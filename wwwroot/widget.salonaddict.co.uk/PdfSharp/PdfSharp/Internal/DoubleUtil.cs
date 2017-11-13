namespace PdfSharp.Internal
{
    using PdfSharp.Drawing;
    using System;
    using System.Runtime.InteropServices;

    internal static class DoubleUtil
    {
        private static double[] decs = new double[] { 
            1.0, 0.1, 0.01, 0.001, 0.0001, 1E-05, 1E-06, 1E-07, 1E-08, 1E-09, 1E-10, 1E-11, 1E-12, 1E-13, 1E-14, 1E-15,
            1E-16
        };
        internal const double Epsilon = 2.2204460492503131E-16;
        internal const float FloatMinimum = 1.175494E-38f;

        public static bool AreClose(XPoint point1, XPoint point2) => 
            (AreClose(point1.X, point2.X) && AreClose(point1.Y, point2.Y));

        public static bool AreClose(XRect rect1, XRect rect2)
        {
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }
            return (((!rect2.IsEmpty && AreClose(rect1.X, rect2.X)) && (AreClose(rect1.Y, rect2.Y) && AreClose(rect1.Height, rect2.Height))) && AreClose(rect1.Width, rect2.Width));
        }

        public static bool AreClose(XSize size1, XSize size2) => 
            (AreClose(size1.Width, size2.Width) && AreClose(size1.Height, size2.Height));

        public static bool AreClose(XVector vector1, XVector vector2) => 
            (AreClose(vector1.X, vector2.X) && AreClose(vector1.Y, vector2.Y));

        public static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }
            double num = ((Math.Abs(value1) + Math.Abs(value2)) + 10.0) * 2.2204460492503131E-16;
            double num2 = value1 - value2;
            return ((-num < num2) && (num > num2));
        }

        public static bool AreRoughlyEqual(double value1, double value2, int decimalPlace) => 
            ((value1 == value2) || (Math.Abs((double) (value1 - value2)) < decs[decimalPlace]));

        public static int DoubleToInt(double value)
        {
            if (0.0 >= value)
            {
                return (int) (value - 0.5);
            }
            return (int) (value + 0.5);
        }

        public static bool GreaterThan(double value1, double value2) => 
            ((value1 > value2) && !AreClose(value1, value2));

        public static bool GreaterThanOrClose(double value1, double value2)
        {
            if (value1 <= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool IsBetweenZeroAndOne(double value) => 
            (GreaterThanOrClose(value, 0.0) && LessThanOrClose(value, 1.0));

        public static bool IsNaN(double value)
        {
            NanUnion union = new NanUnion {
                DoubleValue = value
            };
            ulong num = union.UintValue & 18442240474082181120L;
            ulong num2 = union.UintValue & ((ulong) 0xfffffffffffffL);
            if ((num != 0x7ff0000000000000L) && (num != 18442240474082181120L))
            {
                return false;
            }
            return (num2 != 0L);
        }

        public static bool IsOne(double value) => 
            (Math.Abs((double) (value - 1.0)) < 2.2204460492503131E-15);

        public static bool IsZero(double value) => 
            (Math.Abs(value) < 2.2204460492503131E-15);

        public static bool LessThan(double value1, double value2) => 
            ((value1 < value2) && !AreClose(value1, value2));

        public static bool LessThanOrClose(double value1, double value2)
        {
            if (value1 >= value2)
            {
                return AreClose(value1, value2);
            }
            return true;
        }

        public static bool RectHasNaN(XRect r)
        {
            if ((!IsNaN(r.x) && !IsNaN(r.y)) && !IsNaN(r.height))
            {
                return IsNaN(r.width);
            }
            return true;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal ulong UintValue;
        }
    }
}

