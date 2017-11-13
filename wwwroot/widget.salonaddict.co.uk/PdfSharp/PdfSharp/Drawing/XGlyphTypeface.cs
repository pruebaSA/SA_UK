namespace PdfSharp.Drawing
{
    using PdfSharp.Fonts.OpenType;
    using System;
    using System.IO;

    public class XGlyphTypeface
    {
        private PdfSharp.Fonts.OpenType.FontData fontData;

        public XGlyphTypeface(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }
            FileStream stream = null;
            try
            {
                stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                int length = (int) stream.Length;
                byte[] buffer = new byte[length];
                stream.Read(buffer, 0, length);
                this.Initialize(buffer);
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public XGlyphTypeface(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            this.Initialize(data);
        }

        private void Initialize(byte[] data)
        {
            this.fontData = FontDataStock.Global.RegisterFontData(data);
        }

        public string FamilyName =>
            "Times";

        internal PdfSharp.Fonts.OpenType.FontData FontData =>
            this.fontData;

        public bool IsBold =>
            false;

        public bool IsItalic =>
            false;
    }
}

