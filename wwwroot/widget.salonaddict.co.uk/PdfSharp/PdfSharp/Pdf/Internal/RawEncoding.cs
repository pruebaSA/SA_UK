namespace PdfSharp.Pdf.Internal
{
    using System;
    using System.Text;

    internal sealed class RawEncoding : Encoding
    {
        public override int GetByteCount(char[] chars, int index, int count) => 
            count;

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = charCount; i > 0; i--)
            {
                bytes[byteIndex] = (byte) chars[charIndex];
                charIndex++;
                byteIndex++;
            }
            return charCount;
        }

        public override int GetCharCount(byte[] bytes, int index, int count) => 
            count;

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = byteCount; i > 0; i--)
            {
                chars[charIndex] = (char) bytes[byteIndex];
                byteIndex++;
                charIndex++;
            }
            return byteCount;
        }

        public override int GetMaxByteCount(int charCount) => 
            charCount;

        public override int GetMaxCharCount(int byteCount) => 
            byteCount;
    }
}

