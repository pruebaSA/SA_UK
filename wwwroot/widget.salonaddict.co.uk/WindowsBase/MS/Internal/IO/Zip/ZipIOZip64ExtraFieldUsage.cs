namespace MS.Internal.IO.Zip
{
    using System;

    internal enum ZipIOZip64ExtraFieldUsage
    {
        CompressedSize = 2,
        DiskNumber = 8,
        None = 0,
        OffsetOfLocalHeader = 4,
        UncompressedSize = 1
    }
}

