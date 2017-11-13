namespace System.Xml
{
    using System;

    internal class IncrementalReadDummyDecoder : IncrementalReadDecoder
    {
        internal override int Decode(char[] chars, int startPos, int len) => 
            len;

        internal override int Decode(string str, int startPos, int len) => 
            len;

        internal override void Reset()
        {
        }

        internal override void SetNextOutputBuffer(Array array, int offset, int len)
        {
        }

        internal override int DecodedCount =>
            -1;

        internal override bool IsFull =>
            false;
    }
}

