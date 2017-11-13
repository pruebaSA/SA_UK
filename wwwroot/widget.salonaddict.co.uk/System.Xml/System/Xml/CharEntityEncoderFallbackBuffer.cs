namespace System.Xml
{
    using System;
    using System.Globalization;
    using System.Text;

    internal class CharEntityEncoderFallbackBuffer : EncoderFallbackBuffer
    {
        private string charEntity = string.Empty;
        private int charEntityIndex = -1;
        private CharEntityEncoderFallback parent;

        internal CharEntityEncoderFallbackBuffer(CharEntityEncoderFallback parent)
        {
            this.parent = parent;
        }

        public override bool Fallback(char charUnknown, int index)
        {
            if (this.charEntityIndex >= 0)
            {
                new EncoderExceptionFallbackBuffer().Fallback(charUnknown, index);
            }
            if (this.parent.CanReplaceAt(index))
            {
                this.charEntity = string.Format(CultureInfo.InvariantCulture, "&#x{0:X};", new object[] { (int) charUnknown });
                this.charEntityIndex = 0;
                return true;
            }
            new EncoderExceptionFallback().CreateFallbackBuffer().Fallback(charUnknown, index);
            return false;
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            if (!char.IsSurrogatePair(charUnknownHigh, charUnknownLow))
            {
                throw XmlConvert.CreateInvalidSurrogatePairException(charUnknownHigh, charUnknownLow);
            }
            if (this.charEntityIndex >= 0)
            {
                new EncoderExceptionFallbackBuffer().Fallback(charUnknownHigh, charUnknownLow, index);
            }
            if (this.parent.CanReplaceAt(index))
            {
                this.charEntity = string.Format(CultureInfo.InvariantCulture, "&#x{0:X};", new object[] { char.ConvertToUtf32(charUnknownHigh, charUnknownLow) });
                this.charEntityIndex = 0;
                return true;
            }
            new EncoderExceptionFallback().CreateFallbackBuffer().Fallback(charUnknownHigh, charUnknownLow, index);
            return false;
        }

        public override char GetNextChar()
        {
            if (this.charEntityIndex == -1)
            {
                return '\0';
            }
            char ch = this.charEntity[this.charEntityIndex++];
            if (this.charEntityIndex == this.charEntity.Length)
            {
                this.charEntityIndex = -1;
            }
            return ch;
        }

        public override bool MovePrevious()
        {
            if ((this.charEntityIndex != -1) && (this.charEntityIndex > 0))
            {
                this.charEntityIndex--;
                return true;
            }
            return false;
        }

        public override void Reset()
        {
            this.charEntityIndex = -1;
        }

        public override int Remaining
        {
            get
            {
                if (this.charEntityIndex == -1)
                {
                    return 0;
                }
                return (this.charEntity.Length - this.charEntityIndex);
            }
        }
    }
}

