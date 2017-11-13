namespace System.IO.Compression
{
    using System;

    internal class Deflater
    {
        private FastEncoder encoder;

        public Deflater(bool doGZip)
        {
            this.encoder = new FastEncoder(doGZip);
        }

        public int Finish(byte[] output) => 
            this.encoder.Finish(output);

        public int GetDeflateOutput(byte[] output) => 
            this.encoder.GetCompressedOutput(output);

        public bool NeedsInput() => 
            this.encoder.NeedsInput();

        public void SetInput(byte[] input, int startIndex, int count)
        {
            this.encoder.SetInput(input, startIndex, count);
        }
    }
}

