namespace PdfSharp.Pdf.Advanced
{
    using PdfSharp.Drawing;
    using System;
    using System.Runtime.InteropServices;

    internal interface IContentStream
    {
        string GetFontName(XFont font, out PdfFont pdfFont);
        string GetFontName(string idName, byte[] fontData, out PdfFont pdfFont);
        string GetFormName(XForm form);
        string GetImageName(XImage image);

        PdfResources Resources { get; }
    }
}

