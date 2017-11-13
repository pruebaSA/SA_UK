namespace System.Text
{
    using System;

    [Serializable]
    internal class InternalEncoderBestFitFallback : EncoderFallback
    {
        internal char[] arrayBestFit;
        internal Encoding encoding;

        internal InternalEncoderBestFitFallback(Encoding encoding)
        {
            this.encoding = encoding;
            base.bIsMicrosoftBestFitFallback = true;
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer() => 
            new InternalEncoderBestFitFallbackBuffer(this);

        public override bool Equals(object value)
        {
            InternalEncoderBestFitFallback fallback = value as InternalEncoderBestFitFallback;
            return ((fallback != null) && (this.encoding.CodePage == fallback.encoding.CodePage));
        }

        public override int GetHashCode() => 
            this.encoding.CodePage;

        public override int MaxCharCount =>
            1;
    }
}

