namespace System.Xml
{
    using System;
    using System.Text;

    internal class Ucs4Encoding : Encoding
    {
        internal Ucs4Decoder ucs4Decoder;

        public override int GetByteCount(char[] chars) => 
            (chars.Length * 4);

        public override int GetByteCount(char[] chars, int index, int count) => 
            (count * 4);

        public override byte[] GetBytes(string s) => 
            null;

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) => 
            0;

        public override int GetCharCount(byte[] bytes) => 
            (bytes.Length / 4);

        public override int GetCharCount(byte[] bytes, int index, int count) => 
            this.ucs4Decoder.GetCharCount(bytes, index, count);

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) => 
            this.ucs4Decoder.GetChars(bytes, byteIndex, byteCount, chars, charIndex);

        public override System.Text.Decoder GetDecoder() => 
            this.ucs4Decoder;

        public override System.Text.Encoder GetEncoder() => 
            null;

        public override int GetMaxByteCount(int charCount) => 
            0;

        public override int GetMaxCharCount(int byteCount) => 
            ((byteCount + 3) / 4);

        public override int CodePage =>
            0;

        internal static Encoding UCS4_2143 =>
            new Ucs4Encoding2143();

        internal static Encoding UCS4_3412 =>
            new Ucs4Encoding3412();

        internal static Encoding UCS4_Bigendian =>
            new Ucs4Encoding1234();

        internal static Encoding UCS4_Littleendian =>
            new Ucs4Encoding4321();
    }
}

