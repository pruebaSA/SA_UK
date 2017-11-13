namespace PdfSharp.SharpZipLib.Checksums
{
    using System;

    internal interface IChecksum
    {
        void Reset();
        void Update(int bval);
        void Update(byte[] buffer);
        void Update(byte[] buf, int off, int len);

        long Value { get; }
    }
}

