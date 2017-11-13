namespace System.Text
{
    using System;

    [Serializable]
    internal sealed class InternalDecoderBestFitFallback : DecoderFallback
    {
        internal char[] arrayBestFit;
        internal char cReplacement = '?';
        internal Encoding encoding;

        internal InternalDecoderBestFitFallback(Encoding encoding)
        {
            this.encoding = encoding;
            base.bIsMicrosoftBestFitFallback = true;
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer() => 
            new InternalDecoderBestFitFallbackBuffer(this);

        public override bool Equals(object value)
        {
            InternalDecoderBestFitFallback fallback = value as InternalDecoderBestFitFallback;
            return ((fallback != null) && (this.encoding.CodePage == fallback.encoding.CodePage));
        }

        public override int GetHashCode() => 
            this.encoding.CodePage;

        public override int MaxCharCount =>
            1;
    }
}

