namespace MS.Internal.IO.Zip
{
    using MS.Internal;
    using System;
    using System.IO;
    using System.Windows;

    internal class ZipIOLocalFileDataDescriptor
    {
        private long _compressedSize;
        private uint _crc32;
        private const int _fixedMinimalRecordSizeWithoutSignature = 12;
        private const int _fixedMinimalRecordSizeWithoutSignatureZip64 = 20;
        private const int _fixedMinimalRecordSizeWithSignature = 0x10;
        private const int _fixedMinimalRecordSizeWithSignatureZip64 = 0x18;
        private uint _signature = 0x8074b50;
        private const uint _signatureConstant = 0x8074b50;
        private int _size;
        private long _uncompressedSize;

        internal static ZipIOLocalFileDataDescriptor CreateNew() => 
            new ZipIOLocalFileDataDescriptor { _size = 0x18 };

        internal static ZipIOLocalFileDataDescriptor ParseRecord(BinaryReader reader, long compressedSizeFromCentralDir, long uncompressedSizeFromCentralDir, uint crc32FromCentralDir, ushort versionNeededToExtract)
        {
            ZipIOLocalFileDataDescriptor descriptor = new ZipIOLocalFileDataDescriptor();
            uint[] numArray = new uint[6];
            numArray[0] = reader.ReadUInt32();
            numArray[1] = reader.ReadUInt32();
            numArray[2] = reader.ReadUInt32();
            if (descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, 0x8074b50, numArray[0], (ulong) numArray[1], (ulong) numArray[2]))
            {
                descriptor._size = 12;
                return descriptor;
            }
            numArray[3] = reader.ReadUInt32();
            if (descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, numArray[0], numArray[1], (ulong) numArray[2], (ulong) numArray[3]))
            {
                descriptor._size = 0x10;
                return descriptor;
            }
            if (versionNeededToExtract < 0x2d)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            numArray[4] = reader.ReadUInt32();
            if (descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, 0x8074b50, numArray[0], ZipIOBlockManager.ConvertToUInt64(numArray[1], numArray[2]), ZipIOBlockManager.ConvertToUInt64(numArray[3], numArray[4])))
            {
                descriptor._size = 20;
                return descriptor;
            }
            numArray[5] = reader.ReadUInt32();
            if (!descriptor.TestMatch(0x8074b50, crc32FromCentralDir, compressedSizeFromCentralDir, uncompressedSizeFromCentralDir, numArray[0], numArray[1], ZipIOBlockManager.ConvertToUInt64(numArray[2], numArray[3]), ZipIOBlockManager.ConvertToUInt64(numArray[4], numArray[5])))
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            descriptor._size = 0x18;
            return descriptor;
        }

        internal void Save(BinaryWriter writer)
        {
            Invariant.Assert(this._size == 0x18);
            writer.Write((uint) 0x8074b50);
            writer.Write(this._crc32);
            writer.Write((ulong) this._compressedSize);
            writer.Write((ulong) this._uncompressedSize);
        }

        private bool TestMatch(uint signature, uint crc32FromCentralDir, long compressedSizeFromCentralDir, long uncompressedSizeFromCentralDir, uint suspectSignature, uint suspectCrc32, ulong suspectCompressedSize, ulong suspectUncompressedSize)
        {
            if (((signature == suspectSignature) && (((ulong) compressedSizeFromCentralDir) == suspectCompressedSize)) && ((((ulong) uncompressedSizeFromCentralDir) == suspectUncompressedSize) && (crc32FromCentralDir == suspectCrc32)))
            {
                this._signature = suspectSignature;
                this._compressedSize = (long) suspectCompressedSize;
                this._uncompressedSize = (long) suspectUncompressedSize;
                this._crc32 = suspectCrc32;
                return true;
            }
            return false;
        }

        internal long CompressedSize
        {
            get => 
                this._compressedSize;
            set
            {
                Invariant.Assert(value >= 0L, "CompressedSize must be non-negative");
                this._compressedSize = value;
            }
        }

        internal uint Crc32
        {
            get => 
                this._crc32;
            set
            {
                this._crc32 = value;
            }
        }

        internal long Size =>
            ((long) this._size);

        internal long UncompressedSize
        {
            get => 
                this._uncompressedSize;
            set
            {
                Invariant.Assert(value >= 0L, "UncompressedSize must be non-negative");
                this._uncompressedSize = value;
            }
        }
    }
}

