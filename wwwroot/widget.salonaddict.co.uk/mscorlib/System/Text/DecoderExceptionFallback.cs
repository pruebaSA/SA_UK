namespace System.Text
{
    using System;

    [Serializable]
    public sealed class DecoderExceptionFallback : DecoderFallback
    {
        public override DecoderFallbackBuffer CreateFallbackBuffer() => 
            new DecoderExceptionFallbackBuffer();

        public override bool Equals(object value) => 
            (value is DecoderExceptionFallback);

        public override int GetHashCode() => 
            0x36f;

        public override int MaxCharCount =>
            0;
    }
}

