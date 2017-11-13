namespace System.Xml
{
    using System;

    internal class Ucs4Decoder4321 : Ucs4Decoder
    {
        internal override int GetFullChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            byteCount += byteIndex;
            int index = byteIndex;
            int num3 = charIndex;
            while ((index + 3) < byteCount)
            {
                uint code = (uint) ((((bytes[index + 3] << 0x18) | (bytes[index + 2] << 0x10)) | (bytes[index + 1] << 8)) | bytes[index]);
                if (code > 0x10ffff)
                {
                    throw new ArgumentException(Res.GetString("Enc_InvalidByteInEncoding", new object[] { index }), null);
                }
                if (code > 0xffff)
                {
                    chars[num3] = base.UnicodeToUTF16(code);
                    num3++;
                }
                else
                {
                    if ((code >= 0xd800) && (code <= 0xdfff))
                    {
                        throw new XmlException("Xml_InvalidCharInThisEncoding", string.Empty);
                    }
                    chars[num3] = (char) code;
                }
                num3++;
                index += 4;
            }
            return (num3 - charIndex);
        }
    }
}

