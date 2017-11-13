namespace PdfSharp.Pdf.Internal
{
    using System;
    using System.Text;

    internal sealed class DocEncoding : Encoding
    {
        private static byte[] AnsiToDoc = new byte[] { 
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
            0x20, 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xa9, 170, 0xab, 0xac, 0xad, 0xae, 0xaf,
            0xb0, 0xb1, 0xb2, 0xb3, 180, 0xb5, 0xb6, 0xb7, 0xb8, 0xb9, 0xba, 0xbb, 0xbc, 0xbd, 190, 0xbf,
            0xc0, 0xc1, 0xc2, 0xc3, 0xc4, 0xc5, 0xc6, 0xc7, 200, 0xc9, 0xca, 0xcb, 0xcc, 0xcd, 0xce, 0xcf,
            0xd0, 0xd1, 210, 0xd3, 0xd4, 0xd5, 0xd6, 0xd7, 0xd8, 0xd9, 0xda, 0xdb, 220, 0xdd, 0xde, 0xdf,
            0xe0, 0xe1, 0xe2, 0xe3, 0xe4, 0xe5, 230, 0xe7, 0xe8, 0xe9, 0xea, 0xeb, 0xec, 0xed, 0xee, 0xef,
            240, 0xf1, 0xf2, 0xf3, 0xf4, 0xf5, 0xf6, 0xf7, 0xf8, 0xf9, 250, 0xfb, 0xfc, 0xfd, 0xfe, 0xff
        };
        private static char[] PdfDocToUnicode = new char[] { 
            '\0', '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\x000e', '\x000f',
            '\x0010', '\x0011', '\x0012', '\x0013', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019', '\x001a', '\x001b', '\x001c', '\x001d', '\x001e', '\x001f',
            ' ', '!', '"', '#', '$', '%', '&', '\'', '(', ')', '*', '+', ',', '-', '.', '/',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':', ';', '<', '=', '>', '?',
            '@', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O',
            'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '[', '\\', ']', '^', '_',
            '`', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '{', '|', '}', '~', '\x007f',
            '•', '†', '‡', '…', '—', '–', 'ƒ', '⁄', '‹', '›', '−', '‰', '„', '“', '”', '‘',
            '’', '‚', '™', 'ﬁ', 'ﬂ', 'Ł', 'Œ', 'Š', 'Ÿ', 'Ž', 'ı', 'ł', 'œ', 'š', 'ž', '�',
            '€', '\x00a1', '\x00a2', '\x00a3', '\x00a4', '\x00a5', '\x00a6', '\x00a7', '\x00a8', '\x00a9', '\x00aa', '\x00ab', '\x00ac', '\x00ad', '\x00ae', '\x00af',
            '\x00b0', '\x00b1', '\x00b2', '\x00b3', '\x00b4', '\x00b5', '\x00b6', '\x00b7', '\x00b8', '\x00b9', '\x00ba', '\x00bb', '\x00bc', '\x00bd', '\x00be', '\x00bf',
            '\x00c0', '\x00c1', '\x00c2', '\x00c3', '\x00c4', '\x00c5', '\x00c6', '\x00c7', '\x00c8', '\x00c9', '\x00ca', '\x00cb', '\x00cc', '\x00cd', '\x00ce', '\x00cf',
            '\x00d0', '\x00d1', '\x00d2', '\x00d3', '\x00d4', '\x00d5', '\x00d6', '\x00d7', '\x00d8', '\x00d9', '\x00da', '\x00db', '\x00dc', '\x00dd', '\x00de', '\x00df',
            '\x00e0', '\x00e1', '\x00e2', '\x00e3', '\x00e4', '\x00e5', '\x00e6', '\x00e7', '\x00e8', '\x00e9', '\x00ea', '\x00eb', '\x00ec', '\x00ed', '\x00ee', '\x00ef',
            '\x00f0', '\x00f1', '\x00f2', '\x00f3', '\x00f4', '\x00f5', '\x00f6', '\x00f7', '\x00f8', '\x00f9', '\x00fa', '\x00fb', '\x00fc', '\x00fd', '\x00fe', '\x00ff'
        };

        public override int GetByteCount(char[] chars, int index, int count) => 
            PdfEncoders.WinAnsiEncoding.GetByteCount(chars, index, count);

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            byte[] buffer = PdfEncoders.WinAnsiEncoding.GetBytes(chars, charIndex, charCount);
            int index = 0;
            for (int i = buffer.Length; i > 0; i--)
            {
                bytes[byteIndex] = AnsiToDoc[buffer[index]];
                index++;
                byteIndex++;
            }
            return buffer.Length;
        }

        public override int GetCharCount(byte[] bytes, int index, int count) => 
            count;

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            throw new NotImplementedException("GetChars");
        }

        public override int GetMaxByteCount(int charCount) => 
            charCount;

        public override int GetMaxCharCount(int byteCount) => 
            byteCount;
    }
}

