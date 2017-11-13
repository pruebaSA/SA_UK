namespace PdfSharp.Pdf.Filters
{
    using PdfSharp.Pdf;
    using System;

    public static class Filtering
    {
        private static PdfSharp.Pdf.Filters.ASCII85Decode ascii85Decode;
        private static PdfSharp.Pdf.Filters.ASCIIHexDecode asciiHexDecode;
        private static PdfSharp.Pdf.Filters.FlateDecode flateDecode;
        private static PdfSharp.Pdf.Filters.LzwDecode lzwDecode;

        public static byte[] Decode(byte[] data, PdfItem filterItem)
        {
            byte[] buffer = null;
            if (filterItem is PdfName)
            {
                PdfSharp.Pdf.Filters.Filter filter = GetFilter(filterItem.ToString());
                if (filter != null)
                {
                    buffer = filter.Decode(data);
                }
                return buffer;
            }
            if (!(filterItem is PdfArray))
            {
                return buffer;
            }
            PdfArray array = (PdfArray) filterItem;
            foreach (PdfItem item in array)
            {
                data = Decode(data, item);
            }
            return data;
        }

        public static byte[] Decode(byte[] data, string filterName)
        {
            PdfSharp.Pdf.Filters.Filter filter = GetFilter(filterName);
            if (filter != null)
            {
                return filter.Decode(data, null);
            }
            return null;
        }

        public static byte[] Decode(byte[] data, string filterName, FilterParms parms)
        {
            PdfSharp.Pdf.Filters.Filter filter = GetFilter(filterName);
            if (filter != null)
            {
                return filter.Decode(data, parms);
            }
            return null;
        }

        public static string DecodeToString(byte[] data, string filterName)
        {
            PdfSharp.Pdf.Filters.Filter filter = GetFilter(filterName);
            if (filter != null)
            {
                return filter.DecodeToString(data, null);
            }
            return null;
        }

        public static string DecodeToString(byte[] data, string filterName, FilterParms parms)
        {
            PdfSharp.Pdf.Filters.Filter filter = GetFilter(filterName);
            if (filter != null)
            {
                return filter.DecodeToString(data, parms);
            }
            return null;
        }

        public static byte[] Encode(byte[] data, string filterName)
        {
            PdfSharp.Pdf.Filters.Filter filter = GetFilter(filterName);
            if (filter != null)
            {
                return filter.Encode(data);
            }
            return null;
        }

        public static byte[] Encode(string rawString, string filterName)
        {
            PdfSharp.Pdf.Filters.Filter filter = GetFilter(filterName);
            if (filter != null)
            {
                return filter.Encode(rawString);
            }
            return null;
        }

        public static PdfSharp.Pdf.Filters.Filter GetFilter(string filterName)
        {
            if (filterName.StartsWith("/"))
            {
                filterName = filterName.Substring(1);
            }
            switch (filterName)
            {
                case "ASCIIHexDecode":
                case "AHx":
                    if (asciiHexDecode == null)
                    {
                        asciiHexDecode = new PdfSharp.Pdf.Filters.ASCIIHexDecode();
                    }
                    return asciiHexDecode;

                case "ASCII85Decode":
                case "A85":
                    if (ascii85Decode == null)
                    {
                        ascii85Decode = new PdfSharp.Pdf.Filters.ASCII85Decode();
                    }
                    return ascii85Decode;

                case "LZWDecode":
                case "LZW":
                    if (lzwDecode == null)
                    {
                        lzwDecode = new PdfSharp.Pdf.Filters.LzwDecode();
                    }
                    return lzwDecode;

                case "FlateDecode":
                case "Fl":
                    if (flateDecode == null)
                    {
                        flateDecode = new PdfSharp.Pdf.Filters.FlateDecode();
                    }
                    return flateDecode;

                case "RunLengthDecode":
                case "CCITTFaxDecode":
                case "JBIG2Decode":
                case "DCTDecode":
                case "JPXDecode":
                case "Crypt":
                    return null;
            }
            throw new NotImplementedException("Unknown filter: " + filterName);
        }

        public static PdfSharp.Pdf.Filters.ASCII85Decode ASCII85Decode
        {
            get
            {
                if (ascii85Decode == null)
                {
                    ascii85Decode = new PdfSharp.Pdf.Filters.ASCII85Decode();
                }
                return ascii85Decode;
            }
        }

        public static PdfSharp.Pdf.Filters.ASCIIHexDecode ASCIIHexDecode
        {
            get
            {
                if (asciiHexDecode == null)
                {
                    asciiHexDecode = new PdfSharp.Pdf.Filters.ASCIIHexDecode();
                }
                return asciiHexDecode;
            }
        }

        public static PdfSharp.Pdf.Filters.FlateDecode FlateDecode
        {
            get
            {
                if (flateDecode == null)
                {
                    flateDecode = new PdfSharp.Pdf.Filters.FlateDecode();
                }
                return flateDecode;
            }
        }

        public static PdfSharp.Pdf.Filters.LzwDecode LzwDecode
        {
            get
            {
                if (lzwDecode == null)
                {
                    lzwDecode = new PdfSharp.Pdf.Filters.LzwDecode();
                }
                return lzwDecode;
            }
        }
    }
}

