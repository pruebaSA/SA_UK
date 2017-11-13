namespace MS.Internal.IO.Packaging
{
    using System;
    using System.IO;

    internal interface IDeflateTransform
    {
        void Compress(Stream source, Stream sink);
        void Decompress(Stream source, Stream sink);
    }
}

