namespace PdfSharp.Pdf.Internal
{
    using PdfSharp.Drawing;
    using PdfSharp.Pdf;
    using PdfSharp.Pdf.Security;
    using System;
    using System.Globalization;
    using System.Text;

    internal static class PdfEncoders
    {
        private static byte[] docencode_______ = new byte[] { 
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15,
            0x10, 0x11, 0x12, 0x13, 20, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1a, 0x1b, 0x1c, 0x1d, 30, 0x1f,
            0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 40, 0x29, 0x2a, 0x2b, 0x2c, 0x2d, 0x2e, 0x2f,
            0x30, 0x31, 50, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3a, 0x3b, 60, 0x3d, 0x3e, 0x3f,
            0x40, 0x41, 0x42, 0x43, 0x44, 0x45, 70, 0x47, 0x48, 0x49, 0x4a, 0x4b, 0x4c, 0x4d, 0x4e, 0x4f,
            80, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 90, 0x5b, 0x5c, 0x5d, 0x5e, 0x5f,
            0x60, 0x61, 0x62, 0x63, 100, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6a, 0x6b, 0x6c, 0x6d, 110, 0x6f,
            0x70, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 120, 0x79, 0x7a, 0x7b, 0x7c, 0x7d, 0x7e, 0x7f,
            160, 0x7f, 130, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8a, 0x8b, 140, 0x8d, 0x8e, 0x8f,
            0x90, 0x91, 0x92, 0x93, 0x94, 0x95, 0x8a, 140, 0x98, 0x99, 0x9a, 0x9b, 0x9c, 0x9d, 0x9e, 0x9f,
            160, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 170, 0xab, 0xac, 0xad, 0xae, 0xaf,
            0xb0, 0xb1, 0xb2, 0xb3, 180, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 190, 0xbf,
            0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 200, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
            0xd0, 0xd1, 210, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 220, 0xdd, 0xde, 0xdf,
            0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 230, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
            240, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 250, 0xfb, 0xfc, 0xfd, 0xfe, 0xff
        };
        private static Encoding docEncoding;
        private static Encoding rawEncoding;
        private static Encoding rawUnicodeEncoding;
        private static Encoding unicodeEncoding;
        private static Encoding winAnsiEncoding;

        public static string Format(string format, params object[] args) => 
            string.Format(CultureInfo.InvariantCulture, format, args);

        public static byte[] FormatStringLiteral(byte[] bytes, bool unicode, bool prefix, bool hex, PdfStandardSecurityHandler securityHandler)
        {
            if ((bytes == null) || (bytes.Length == 0))
            {
                if (!hex)
                {
                    return new byte[] { 40, 0x29 };
                }
                return new byte[] { 60, 0x3e };
            }
            bool flag = false;
            if (securityHandler != null)
            {
                bytes = (byte[]) bytes.Clone();
                bytes = securityHandler.EncryptBytes(bytes);
                flag = true;
            }
            int length = bytes.Length;
            StringBuilder builder = new StringBuilder();
            if (!unicode)
            {
                if (!hex)
                {
                    builder.Append("(");
                    for (int i = 0; i < length; i++)
                    {
                        char ch = (char) bytes[i];
                        if (ch < ' ')
                        {
                            switch (ch)
                            {
                                case '\b':
                                {
                                    builder.Append(@"\b");
                                    continue;
                                }
                                case '\t':
                                {
                                    builder.Append(@"\t");
                                    continue;
                                }
                                case '\n':
                                {
                                    builder.Append(@"\n");
                                    continue;
                                }
                                case '\r':
                                {
                                    builder.Append(@"\r");
                                    continue;
                                }
                            }
                            if (!true)
                            {
                                builder.Append(@"\0");
                                builder.Append((char) ((ch % '\b') + 0x30));
                                builder.Append((char) ((ch / '\b') + 0x30));
                            }
                            else
                            {
                                builder.Append(ch);
                            }
                            continue;
                        }
                        switch (ch)
                        {
                            case '(':
                                builder.Append(@"\(");
                                break;

                            case ')':
                                builder.Append(@"\)");
                                break;

                            case '\\':
                                builder.Append(@"\\");
                                break;

                            default:
                                builder.Append(ch);
                                break;
                        }
                    }
                    builder.Append(')');
                }
                else
                {
                    builder.Append('<');
                    for (int j = 0; j < length; j++)
                    {
                        builder.AppendFormat("{0:X2}", bytes[j]);
                    }
                    builder.Append('>');
                }
            }
            else
            {
                while (true)
                {
                    if (hex)
                    {
                        if (prefix)
                        {
                            builder.Append("<FEFF");
                        }
                        else
                        {
                            builder.Append("<");
                        }
                        for (int k = 0; k < length; k += 2)
                        {
                            builder.AppendFormat("{0:X2}{1:X2}", bytes[k], bytes[k + 1]);
                            if ((k != 0) && ((k % 0x30) == 0))
                            {
                                builder.Append("\n");
                            }
                        }
                        builder.Append(">");
                        break;
                    }
                    hex = true;
                }
            }
            return RawEncoding.GetBytes(builder.ToString());
        }

        public static string ToHexStringLiteral(string text, PdfStringEncoding encoding, PdfStandardSecurityHandler securityHandler)
        {
            byte[] buffer;
            if (string.IsNullOrEmpty(text))
            {
                return "<>";
            }
            switch (encoding)
            {
                case PdfStringEncoding.RawEncoding:
                    buffer = RawEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.PDFDocEncoding:
                    buffer = DocEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.WinAnsiEncoding:
                    buffer = WinAnsiEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.Unicode:
                    buffer = UnicodeEncoding.GetBytes(text);
                    break;

                default:
                    throw new NotImplementedException(encoding.ToString());
            }
            byte[] bytes = FormatStringLiteral(buffer, encoding == PdfStringEncoding.Unicode, true, true, securityHandler);
            return RawEncoding.GetString(bytes, 0, bytes.Length);
        }

        public static string ToHexStringLiteral(byte[] bytes, bool unicode, PdfStandardSecurityHandler securityHandler)
        {
            if ((bytes == null) || (bytes.Length == 0))
            {
                return "<>";
            }
            byte[] buffer = FormatStringLiteral(bytes, unicode, true, true, securityHandler);
            return RawEncoding.GetString(buffer, 0, buffer.Length);
        }

        public static string ToString(XMatrix matrix) => 
            string.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###} {3:0.###} {4:0.###} {5:0.###}", new object[] { matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY });

        public static string ToString(double val) => 
            val.ToString("0.###", CultureInfo.InvariantCulture);

        public static string ToString(XColor color, PdfColorMode colorMode)
        {
            if (colorMode == PdfColorMode.Undefined)
            {
                colorMode = (color.ColorSpace == XColorSpace.Cmyk) ? PdfColorMode.Cmyk : PdfColorMode.Rgb;
            }
            if (colorMode == PdfColorMode.Cmyk)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###} {3:0.###}", new object[] { color.C, color.M, color.Y, color.K });
            }
            return string.Format(CultureInfo.InvariantCulture, "{0:0.###} {1:0.###} {2:0.###}", new object[] { ((double) color.R) / 255.0, ((double) color.G) / 255.0, ((double) color.B) / 255.0 });
        }

        public static string ToStringLiteral(string text, PdfStringEncoding encoding, PdfStandardSecurityHandler securityHandler)
        {
            byte[] buffer;
            if (string.IsNullOrEmpty(text))
            {
                return "()";
            }
            switch (encoding)
            {
                case PdfStringEncoding.RawEncoding:
                    buffer = RawEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.PDFDocEncoding:
                    buffer = DocEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.WinAnsiEncoding:
                    buffer = WinAnsiEncoding.GetBytes(text);
                    break;

                case PdfStringEncoding.Unicode:
                    buffer = UnicodeEncoding.GetBytes(text);
                    break;

                default:
                    throw new NotImplementedException(encoding.ToString());
            }
            byte[] bytes = FormatStringLiteral(buffer, encoding == PdfStringEncoding.Unicode, true, false, securityHandler);
            return RawEncoding.GetString(bytes, 0, bytes.Length);
        }

        public static string ToStringLiteral(byte[] bytes, bool unicode, PdfStandardSecurityHandler securityHandler)
        {
            if ((bytes == null) || (bytes.Length == 0))
            {
                return "()";
            }
            byte[] buffer = FormatStringLiteral(bytes, unicode, true, false, securityHandler);
            return RawEncoding.GetString(buffer, 0, buffer.Length);
        }

        public static Encoding DocEncoding
        {
            get
            {
                if (docEncoding == null)
                {
                    docEncoding = new PdfSharp.Pdf.Internal.DocEncoding();
                }
                return docEncoding;
            }
        }

        public static Encoding RawEncoding
        {
            get
            {
                if (rawEncoding == null)
                {
                    rawEncoding = new PdfSharp.Pdf.Internal.RawEncoding();
                }
                return rawEncoding;
            }
        }

        public static Encoding RawUnicodeEncoding
        {
            get
            {
                if (rawUnicodeEncoding == null)
                {
                    rawUnicodeEncoding = new PdfSharp.Pdf.Internal.RawUnicodeEncoding();
                }
                return rawUnicodeEncoding;
            }
        }

        public static Encoding UnicodeEncoding
        {
            get
            {
                if (unicodeEncoding == null)
                {
                    unicodeEncoding = Encoding.Unicode;
                }
                return unicodeEncoding;
            }
        }

        public static Encoding WinAnsiEncoding
        {
            get
            {
                if (winAnsiEncoding == null)
                {
                    winAnsiEncoding = Encoding.GetEncoding(0x4e4);
                }
                return winAnsiEncoding;
            }
        }
    }
}

