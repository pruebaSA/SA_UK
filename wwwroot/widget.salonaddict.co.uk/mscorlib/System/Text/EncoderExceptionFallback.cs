namespace System.Text
{
    using System;

    [Serializable]
    public sealed class EncoderExceptionFallback : EncoderFallback
    {
        public override EncoderFallbackBuffer CreateFallbackBuffer() => 
            new EncoderExceptionFallbackBuffer();

        public override bool Equals(object value) => 
            (value is EncoderExceptionFallback);

        public override int GetHashCode() => 
            0x28e;

        public override int MaxCharCount =>
            0;
    }
}

