namespace PdfSharp
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Resources;

    internal static class PSSR
    {
        private static ResourceManager resmngr;

        public static string CannotChangeImmutableObject(string typename) => 
            $"You cannot change this immutable {typename} object.";

        public static string CannotGetGlyphTypeface(string fontName) => 
            Format("Cannot get a matching glyph typeface for font '{0}'.", new object[] { fontName });

        public static string FileNotFound(string path) => 
            Format("The file '{0}' does not exist.", new object[] { path });

        public static string Format(PSMsgID id, params object[] args)
        {
            string str;
            try
            {
                str = GetString(id);
                return ((str != null) ? Format(str, args) : "INTERNAL ERROR: Message not found in resources.");
            }
            catch (Exception exception)
            {
                str = $"UNEXPECTED ERROR while formatting message with ID {id.ToString()}: {exception.ToString()}";
            }
            return str;
        }

        public static string Format(string format, params object[] args)
        {
            if (format == null)
            {
                throw new ArgumentNullException("format");
            }
            try
            {
                return string.Format(format, args);
            }
            catch (Exception exception)
            {
                return $"UNEXPECTED ERROR while formatting message '{format}': {exception}";
            }
        }

        public static string GetString(PSMsgID id) => 
            ResMngr.GetString(id.ToString());

        public static string ImportPageNumberOutOfRange(int pageNumber, int maxPage, string path) => 
            string.Format("The page cannot be imported from document '{2}', because the page number is out of range. The specified page number is {0}, but it must be in the range from 1 to {1}.", pageNumber, maxPage, path);

        public static string InappropriateColorSpace(PdfColorMode colorMode, XColorSpace colorSpace)
        {
            string str;
            string str2;
            switch (colorMode)
            {
                case PdfColorMode.Rgb:
                    str = "RGB";
                    break;

                case PdfColorMode.Cmyk:
                    str = "CMYK";
                    break;

                default:
                    str = "(undefined)";
                    break;
            }
            switch (colorSpace)
            {
                case XColorSpace.Rgb:
                    str2 = "RGB";
                    break;

                case XColorSpace.Cmyk:
                    str2 = "CMYK";
                    break;

                case XColorSpace.GrayScale:
                    str2 = "grayscale";
                    break;

                default:
                    str2 = "(undefined)";
                    break;
            }
            return $"The document requires color mode {str}, but a color is defined using {str2}. Use only colors that match the color mode of the PDF document";
        }

        public static string InvalidValue(int val, string name, int min, int max) => 
            Format("{0} is not a valid value for {1}. {1} should be greater than or equal to {2} and less than or equal to {3}.", new object[] { val, name, min, max });

        public static string PointArrayAtLeast(int count) => 
            Format("The point array must contain {0} or more points.", new object[] { count });

        [Conditional("DEBUG")]
        public static void TestResourceMessages()
        {
            foreach (string str in Enum.GetNames(typeof(PSMsgID)))
            {
                $"{str}: '{ResMngr.GetString(str)}'";
            }
        }

        public static string UnexpectedToken(string token) => 
            Format(PSMsgID.UnexpectedToken, new object[] { token });

        public static string CannotHandleXRefStreams =>
            "Cannot handle iref streams. The current implementation of PDFsharp cannot handle this PDF feature introduced with Acrobat 6.";

        public static string CannotModify =>
            "The document cannot be modified.";

        public static string ErrorReadingFontData =>
            "Error while parsing an OpenType font.";

        public static string FontDataReadOnly =>
            "Font data is read-only.";

        public static string IndexOutOfRange =>
            "The index is out of range.";

        public static string InvalidPassword =>
            "The specified password is invalid.";

        public static string InvalidPdf =>
            "The file is not a valid PDF document.";

        public static string InvalidVersionNumber =>
            "Invalid version number. Valid values are 12, 13, and 14.";

        public static string ListEnumCurrentOutOfRange =>
            "Enumeration out of range.";

        public static string MultiplePageInsert =>
            "The page cannot be added to this document because the document already owned this page.";

        public static string NameMustStartWithSlash =>
            "A PDF name must start with a slash (/).";

        public static string NeedPenOrBrush =>
            "XPen or XBrush or both must not be null.";

        public static string ObsoleteFunktionCalled =>
            "The function is obsolete and must not be called.";

        public static string OutlineIndexOutOfRange =>
            "The index of an outline is out of range.";

        public static string OwnerPasswordRequired =>
            "To modify the document the owner password is required";

        public static string OwningDocumentRequired =>
            "The PDF object must belong to a PdfDocument, but property Document is null.";

        public static string PageIndexOutOfRange =>
            "The index of a page is out of range.";

        public static string PasswordRequired =>
            "A password is required to open the PDF document.";

        public static string PointArrayEmpty =>
            "The PointF array must not be empty.";

        public static ResourceManager ResMngr
        {
            get
            {
                if (resmngr == null)
                {
                    resmngr = new ResourceManager("PdfSharp.Resources.Messages", Assembly.GetExecutingAssembly());
                }
                return resmngr;
            }
        }

        public static string UnexpectedTokenInPdfFile =>
            "Unexpected token in PDF file. The PDF file may be corrupt. If it is not, please send us the file for service.";

        public static string UnknownEncryption =>
            GetString(PSMsgID.UnknownEncryption);

        public static string UserOrOwnerPasswordRequired =>
            GetString(PSMsgID.UserOrOwnerPasswordRequired);
    }
}

