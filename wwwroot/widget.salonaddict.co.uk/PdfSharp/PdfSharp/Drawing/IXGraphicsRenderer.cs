namespace PdfSharp.Drawing
{
    using System;

    internal interface IXGraphicsRenderer
    {
        void BeginContainer(XGraphicsContainer container, XRect dstrect, XRect srcrect, XGraphicsUnit unit);
        void Clear(XColor color);
        void Close();
        void DrawArc(XPen pen, double x, double y, double width, double height, double startAngle, double sweepAngle);
        void DrawBezier(XPen pen, double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4);
        void DrawBeziers(XPen pen, XPoint[] points);
        void DrawClosedCurve(XPen pen, XBrush brush, XPoint[] points, double tension, XFillMode fillmode);
        void DrawCurve(XPen pen, XPoint[] points, double tension);
        void DrawEllipse(XPen pen, XBrush brush, double x, double y, double width, double height);
        void DrawImage(XImage image, XRect destRect, XRect srcRect, XGraphicsUnit srcUnit);
        void DrawImage(XImage image, double x, double y, double width, double height);
        void DrawLine(XPen pen, double x1, double y1, double x2, double y2);
        void DrawLines(XPen pen, XPoint[] points);
        void DrawPath(XPen pen, XBrush brush, XGraphicsPath path);
        void DrawPie(XPen pen, XBrush brush, double x, double y, double width, double height, double startAngle, double sweepAngle);
        void DrawPolygon(XPen pen, XBrush brush, XPoint[] points, XFillMode fillmode);
        void DrawRectangle(XPen pen, XBrush brush, double x, double y, double width, double height);
        void DrawRectangles(XPen pen, XBrush brush, XRect[] rects);
        void DrawRoundedRectangle(XPen pen, XBrush brush, double x, double y, double width, double height, double ellipseWidth, double ellipseHeight);
        void DrawString(string s, XFont font, XBrush brush, XRect layoutRectangle, XStringFormat format);
        void EndContainer(XGraphicsContainer container);
        void ResetClip();
        void Restore(XGraphicsState state);
        void Save(XGraphicsState state);
        void SetClip(XGraphicsPath path, XCombineMode combineMode);
        void SetPageTransform(XPageDirection direction, XPoint origion, XGraphicsUnit unit);
        void WriteComment(string comment);

        XMatrix Transform { set; }
    }
}

