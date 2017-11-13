namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;
    using System.Windows;

    internal class ZipIOCentralDirectoryDigitalSignature
    {
        private const long _fixedMinimalRecordSize = 6L;
        private const uint _signatureConstant = 0x5054b50;

        private ZipIOCentralDirectoryDigitalSignature()
        {
        }

        internal static ZipIOCentralDirectoryDigitalSignature ParseRecord(BinaryReader reader)
        {
            if (((reader.BaseStream.Length - reader.BaseStream.Position) >= 6L) && (reader.ReadUInt32() == 0x5054b50))
            {
                throw new NotSupportedException(System.Windows.SR.Get("ZipNotSupportedSignedArchive"));
            }
            return null;
        }
    }
}

