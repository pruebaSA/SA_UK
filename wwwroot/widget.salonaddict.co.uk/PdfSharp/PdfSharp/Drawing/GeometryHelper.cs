namespace PdfSharp.Drawing
{
    using System;
    using System.Collections.Generic;

    internal static class GeometryHelper
    {
        private static void AppendPartialArcQuadrant(List<XPoint> points, double x, double y, double width, double height, double α, double β, PathStart pathStart, XMatrix matrix)
        {
            double num7;
            double num8;
            if (β > 360.0)
            {
                β -= Math.Floor((double) (β / 360.0)) * 360.0;
            }
            double num = width / 2.0;
            double num2 = height / 2.0;
            double num3 = x + num;
            double num4 = y + num2;
            bool flag = false;
            if ((α >= 180.0) && (β >= 180.0))
            {
                α -= 180.0;
                β -= 180.0;
                flag = true;
            }
            if (width == height)
            {
                α *= 0.017453292519943295;
                β *= 0.017453292519943295;
            }
            else
            {
                α *= 0.017453292519943295;
                num7 = Math.Sin(α);
                if (Math.Abs(num7) > 1E-10)
                {
                    α = 1.5707963267948966 - Math.Atan((num2 * Math.Cos(α)) / (num * num7));
                }
                β *= 0.017453292519943295;
                num8 = Math.Sin(β);
                if (Math.Abs(num8) > 1E-10)
                {
                    β = 1.5707963267948966 - Math.Atan((num2 * Math.Cos(β)) / (num * num8));
                }
            }
            double num9 = (4.0 * (1.0 - Math.Cos((α - β) / 2.0))) / (3.0 * Math.Sin((β - α) / 2.0));
            num7 = Math.Sin(α);
            double num5 = Math.Cos(α);
            num8 = Math.Sin(β);
            double num6 = Math.Cos(β);
            if (flag)
            {
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        points.Add(matrix.Transform(new XPoint(num3 - (num * num5), num4 - (num2 * num7))));
                        break;

                    case PathStart.LineTo1st:
                        points.Add(matrix.Transform(new XPoint(num3 - (num * num5), num4 - (num2 * num7))));
                        break;
                }
            }
            else
            {
                switch (pathStart)
                {
                    case PathStart.MoveTo1st:
                        points.Add(matrix.Transform(new XPoint(num3 + (num * num5), num4 + (num2 * num7))));
                        break;

                    case PathStart.LineTo1st:
                        points.Add(matrix.Transform(new XPoint(num3 + (num * num5), num4 + (num2 * num7))));
                        break;
                }
                points.Add(matrix.Transform(new XPoint(num3 + (num * (num5 - (num9 * num7))), num4 + (num2 * (num7 + (num9 * num5))))));
                points.Add(matrix.Transform(new XPoint(num3 + (num * (num6 + (num9 * num8))), num4 + (num2 * (num8 - (num9 * num6))))));
                points.Add(matrix.Transform(new XPoint(num3 + (num * num6), num4 + (num2 * num8))));
                return;
            }
            points.Add(matrix.Transform(new XPoint(num3 - (num * (num5 - (num9 * num7))), num4 - (num2 * (num7 + (num9 * num5))))));
            points.Add(matrix.Transform(new XPoint(num3 - (num * (num6 + (num9 * num8))), num4 - (num2 * (num8 - (num9 * num6))))));
            points.Add(matrix.Transform(new XPoint(num3 - (num * num6), num4 - (num2 * num8))));
        }

        public static List<XPoint> BezierCurveFromArc(XPoint point1, XPoint point2, double rotationAngle, XSize size, bool isLargeArc, bool clockwise, PathStart pathStart)
        {
            XVector vector2;
            double width = size.Width;
            double height = size.Height;
            double num3 = height / width;
            bool flag = !clockwise;
            XMatrix matrix = new XMatrix();
            matrix.RotateAppend(-rotationAngle);
            matrix.ScaleAppend(height / width, 1.0);
            XPoint point = matrix.Transform(point1);
            XPoint point3 = matrix.Transform(point2);
            XPoint point4 = new XPoint((point.X + point3.X) / 2.0, (point.Y + point3.Y) / 2.0);
            XVector vector = (XVector) (point3 - point);
            double num4 = vector.Length / 2.0;
            if (isLargeArc == flag)
            {
                vector2 = new XVector(-vector.Y, vector.X);
            }
            else
            {
                vector2 = new XVector(vector.Y, -vector.X);
            }
            vector2.Normalize();
            double d = Math.Sqrt((height * height) - (num4 * num4));
            if (double.IsNaN(d))
            {
                d = 0.0;
            }
            XPoint point5 = point4 + ((XPoint) (d * vector2));
            double num6 = Math.Atan2(point.Y - point5.Y, point.X - point5.X);
            double num7 = Math.Atan2(point3.Y - point5.Y, point3.X - point5.X);
            if (isLargeArc == (Math.Abs((double) (num7 - num6)) < 3.1415926535897931))
            {
                if (num6 < num7)
                {
                    num6 += 6.2831853071795862;
                }
                else
                {
                    num7 += 6.2831853071795862;
                }
            }
            matrix.Invert();
            double num8 = num7 - num6;
            return BezierCurveFromArc(point5.X - (width * num3), point5.Y - height, (2.0 * width) * num3, 2.0 * height, num6 / 0.017453292519943295, num8 / 0.017453292519943295, pathStart, ref matrix);
        }

        public static List<XPoint> BezierCurveFromArc(double x, double y, double width, double height, double startAngle, double sweepAngle, PathStart pathStart, ref XMatrix matrix)
        {
            List<XPoint> points = new List<XPoint>();
            double num = startAngle;
            if (num < 0.0)
            {
                num += (1.0 + Math.Floor((double) (Math.Abs(num) / 360.0))) * 360.0;
            }
            else if (num > 360.0)
            {
                num -= Math.Floor((double) (num / 360.0)) * 360.0;
            }
            double num2 = sweepAngle;
            if (num2 < -360.0)
            {
                num2 = -360.0;
            }
            else if (num2 > 360.0)
            {
                num2 = 360.0;
            }
            if ((num == 0.0) && (num2 < 0.0))
            {
                num = 360.0;
            }
            else if ((num == 360.0) && (num2 > 0.0))
            {
                num = 0.0;
            }
            bool flag = Math.Abs(num2) <= 90.0;
            num2 = num + num2;
            if (num2 < 0.0)
            {
                num2 += (1.0 + Math.Floor((double) (Math.Abs(num2) / 360.0))) * 360.0;
            }
            bool clockwise = sweepAngle > 0.0;
            int num3 = Quatrant(num, true, clockwise);
            int num4 = Quatrant(num2, false, clockwise);
            if ((num3 == num4) && flag)
            {
                AppendPartialArcQuadrant(points, x, y, width, height, num, num2, pathStart, matrix);
                return points;
            }
            int num5 = num3;
            bool flag3 = true;
            while (true)
            {
                if ((num5 == num3) && flag3)
                {
                    double num6 = (num5 * 90) + (clockwise ? 90 : 0);
                    AppendPartialArcQuadrant(points, x, y, width, height, num, num6, pathStart, matrix);
                }
                else if (num5 == num4)
                {
                    double num7 = (num5 * 90) + (clockwise ? 0 : 90);
                    AppendPartialArcQuadrant(points, x, y, width, height, num7, num2, PathStart.Ignore1st, matrix);
                }
                else
                {
                    double num8 = (num5 * 90) + (clockwise ? 0 : 90);
                    double num9 = (num5 * 90) + (clockwise ? 90 : 0);
                    AppendPartialArcQuadrant(points, x, y, width, height, num8, num9, PathStart.Ignore1st, matrix);
                }
                if ((num5 == num4) && flag)
                {
                    return points;
                }
                flag = true;
                if (clockwise)
                {
                    num5 = (num5 == 3) ? 0 : (num5 + 1);
                }
                else
                {
                    num5 = (num5 == 0) ? 3 : (num5 - 1);
                }
                flag3 = false;
            }
        }

        private static int Quatrant(double φ, bool start, bool clockwise)
        {
            if (φ > 360.0)
            {
                φ -= Math.Floor((double) (φ / 360.0)) * 360.0;
            }
            int num = (int) (φ / 90.0);
            if ((num * 90) == φ)
            {
                if ((start && !clockwise) || (!start && clockwise))
                {
                    num = (num == 0) ? 3 : (num - 1);
                }
                return num;
            }
            return (clockwise ? (((int) Math.Floor((double) (φ / 90.0))) % 4) : ((int) Math.Floor((double) (φ / 90.0))));
        }
    }
}

