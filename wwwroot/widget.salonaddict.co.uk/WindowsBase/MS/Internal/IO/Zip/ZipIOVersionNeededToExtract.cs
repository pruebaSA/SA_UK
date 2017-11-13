namespace MS.Internal.IO.Zip
{
    using System;

    internal enum ZipIOVersionNeededToExtract : ushort
    {
        DeflatedData = 20,
        StoredData = 10,
        VolumeLabel = 11,
        Zip64FileFormat = 0x2d
    }
}

