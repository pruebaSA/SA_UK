namespace MS.Internal.IO.Packaging
{
    using System;
    using System.IO;
    using System.IO.Compression;

    internal class DeflateEmulationTransform : IDeflateTransform
    {
        private byte[] _buffer;

        public void Compress(Stream source, Stream sink)
        {
            using (DeflateStream stream = new DeflateStream(sink, CompressionMode.Compress, true))
            {
                int count = 0;
                do
                {
                    count = source.Read(this.Buffer, 0, this.Buffer.Length);
                    if (count > 0)
                    {
                        stream.Write(this.Buffer, 0, count);
                    }
                }
                while (count > 0);
            }
            if (sink.CanSeek)
            {
                sink.SetLength(sink.Position);
            }
        }

        public void Decompress(Stream source, Stream sink)
        {
            using (DeflateStream stream = new DeflateStream(source, CompressionMode.Decompress, true))
            {
                int count = 0;
                do
                {
                    count = stream.Read(this.Buffer, 0, this.Buffer.Length);
                    if (count > 0)
                    {
                        sink.Write(this.Buffer, 0, count);
                    }
                }
                while (count > 0);
            }
        }

        private byte[] Buffer
        {
            get
            {
                if (this._buffer == null)
                {
                    this._buffer = new byte[0x1000];
                }
                return this._buffer;
            }
        }
    }
}

