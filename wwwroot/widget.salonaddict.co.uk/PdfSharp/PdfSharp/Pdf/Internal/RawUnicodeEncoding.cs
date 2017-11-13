namespace PdfSharp.Pdf.Internal
{
    using System;
    using System.Text;

    internal sealed class RawUnicodeEncoding : Encoding
    {
        public override int GetByteCount(char[] chars, int index, int count) => 
            (2 * count);

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            for (int i = charCount; i > 0; i--)
            {
                char ch = chars[charIndex];
                bytes[byteIndex++] = (byte) (ch >> 8);
                bytes[byteIndex++] = (byte) ch;
                charIndex++;
            }
            return (charCount * 2);
        }

        public override int GetCharCount(byte[] bytes, int index, int count) => 
            (count / 2);

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            for (int i = byteCount; i > 0; i--)
            {
                chars[charIndex] = (char) (bytes[byteIndex] << (8 + bytes[byteIndex + 1]));
                byteIndex += 2;
                charIndex++;
            }
            return byteCount;
        }

        public override int GetMaxByteCount(int charCount) => 
            (charCount * 2);

        public override int GetMaxCharCount(int byteCount) => 
            (byteCount / 2);
    }
}

