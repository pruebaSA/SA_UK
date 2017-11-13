namespace MigraDoc.Rendering
{
    using PdfSharp.Charting;

    internal class ChartFormatInfo : ShapeFormatInfo
    {
        internal ChartFrame chartFrame;
        internal FormattedTextArea formattedBottom;
        internal FormattedTextArea formattedFooter;
        internal FormattedTextArea formattedHeader;
        internal FormattedTextArea formattedLeft;
        internal FormattedTextArea formattedRight;
        internal FormattedTextArea formattedTop;
    }
}

