namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;

    internal class ZipIOLocalFileHeader
    {
        private uint _compressedSize;
        private ushort _compressionMethod;
        private uint _crc32;
        private ZipIOExtraField _extraField;
        private ushort _extraFieldLength;
        private byte[] _fileName;
        private ushort _fileNameLength;
        private const int _fixedMinimalRecordSize = 30;
        private ushort _generalPurposeBitFlag;
        private uint _lastModFileDateTime;
        private uint _signature = 0x4034b50;
        private const uint _signatureConstant = 0x4034b50;
        private string _stringFileName;
        private uint _uncompressedSize;
        private ushort _versionNeededToExtract;

        private ZipIOLocalFileHeader()
        {
        }

        internal static ZipIOLocalFileHeader CreateNew(string fileName, Encoding encoding, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption, bool streaming)
        {
            byte[] bytes = encoding.GetBytes(fileName);
            if (bytes.Length > ZipIOBlockManager.MaxFileNameSize)
            {
                throw new ArgumentOutOfRangeException("fileName");
            }
            ZipIOLocalFileHeader header = new ZipIOLocalFileHeader {
                _signature = 0x4034b50,
                _compressionMethod = (ushort) compressionMethod
            };
            if (streaming)
            {
                header._versionNeededToExtract = 0x2d;
            }
            else
            {
                header._versionNeededToExtract = (ushort) ZipIOBlockManager.CalcVersionNeededToExtractFromCompression(compressionMethod);
            }
            if (compressionMethod != CompressionMethodEnum.Stored)
            {
                header.DeflateOption = deflateOption;
            }
            if (streaming)
            {
                header.StreamingCreationFlag = true;
            }
            header._lastModFileDateTime = ZipIOBlockManager.ToMsDosDateTime(DateTime.Now);
            header._fileNameLength = (ushort) bytes.Length;
            header._fileName = bytes;
            header._extraField = ZipIOExtraField.CreateNew(!streaming);
            header._extraFieldLength = header._extraField.Size;
            header._stringFileName = fileName;
            return header;
        }

        internal static ZipIOLocalFileHeader ParseRecord(BinaryReader reader, Encoding encoding)
        {
            ZipIOLocalFileHeader header = new ZipIOLocalFileHeader {
                _signature = reader.ReadUInt32(),
                _versionNeededToExtract = reader.ReadUInt16(),
                _generalPurposeBitFlag = reader.ReadUInt16(),
                _compressionMethod = reader.ReadUInt16(),
                _lastModFileDateTime = reader.ReadUInt32(),
                _crc32 = reader.ReadUInt32(),
                _compressedSize = reader.ReadUInt32(),
                _uncompressedSize = reader.ReadUInt32(),
                _fileNameLength = reader.ReadUInt16(),
                _extraFieldLength = reader.ReadUInt16()
            };
            header._fileName = reader.ReadBytes(header._fileNameLength);
            ZipIOZip64ExtraFieldUsage none = ZipIOZip64ExtraFieldUsage.None;
            if (header._versionNeededToExtract >= 0x2d)
            {
                if (header._compressedSize == uint.MaxValue)
                {
                    none |= ZipIOZip64ExtraFieldUsage.CompressedSize;
                }
                if (header._uncompressedSize == uint.MaxValue)
                {
                    none |= ZipIOZip64ExtraFieldUsage.UncompressedSize;
                }
            }
            header._extraField = ZipIOExtraField.ParseRecord(reader, none, header._extraFieldLength);
            header._stringFileName = ZipIOBlockManager.ValidateNormalizeFileName(encoding.GetString(header._fileName));
            header.Validate();
            return header;
        }

        internal void Save(BinaryWriter writer)
        {
            writer.Write((uint) 0x4034b50);
            writer.Write(this._versionNeededToExtract);
            writer.Write(this._generalPurposeBitFlag);
            writer.Write(this._compressionMethod);
            writer.Write(this._lastModFileDateTime);
            writer.Write(this._crc32);
            writer.Write(this._compressedSize);
            writer.Write(this._uncompressedSize);
            writer.Write(this._fileNameLength);
            writer.Write(this._extraField.Size);
            if (this._fileNameLength > 0)
            {
                writer.Write(this._fileName, 0, this._fileNameLength);
            }
            this._extraField.Save(writer);
            writer.Flush();
        }

        internal void UpdatePadding(long headerSizeChange)
        {
            this._extraField.UpdatePadding(headerSizeChange);
        }

        internal void UpdateZip64Structures(long compressedSize, long uncompressedSize, long offset)
        {
            if (((compressedSize >= 0xffffffffL) || (uncompressedSize >= 0xffffffffL)) || (offset >= 0xffffffffL))
            {
                this._extraField.CompressedSize = compressedSize;
                this._extraField.UncompressedSize = uncompressedSize;
                this._compressedSize = uint.MaxValue;
                this._uncompressedSize = uint.MaxValue;
                this._versionNeededToExtract = 0x2d;
            }
            else
            {
                this._compressedSize = (uint) compressedSize;
                this._uncompressedSize = (uint) uncompressedSize;
                this._extraField.Zip64ExtraFieldUsage = ZipIOZip64ExtraFieldUsage.None;
                this._versionNeededToExtract = (ushort) ZipIOBlockManager.CalcVersionNeededToExtractFromCompression(this.CompressionMethod);
            }
        }

        private void Validate()
        {
            if (this._signature != 0x4034b50)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if (this._fileNameLength != this._fileName.Length)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            ZipArchive.VerifyVersionNeededToExtract(this._versionNeededToExtract);
            if (this.EncryptedFlag)
            {
                throw new NotSupportedException(System.Windows.SR.Get("ZipNotSupportedEncryptedArchive"));
            }
            if ((this._versionNeededToExtract < 0x2d) && (this._extraField.Zip64ExtraFieldUsage != ZipIOZip64ExtraFieldUsage.None))
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if ((this._extraField.Zip64ExtraFieldUsage != ZipIOZip64ExtraFieldUsage.None) && (this._extraField.Zip64ExtraFieldUsage != (ZipIOZip64ExtraFieldUsage.CompressedSize | ZipIOZip64ExtraFieldUsage.UncompressedSize)))
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if (this._extraFieldLength != this._extraField.Size)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if ((this.CompressionMethod != CompressionMethodEnum.Stored) && (this.CompressionMethod != CompressionMethodEnum.Deflated))
            {
                throw new NotSupportedException(System.Windows.SR.Get("ZipNotSupportedCompressionMethod"));
            }
        }

        internal long CompressedSize
        {
            get
            {
                if ((this._extraField.Zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    return this._extraField.CompressedSize;
                }
                return (long) this._compressedSize;
            }
        }

        internal CompressionMethodEnum CompressionMethod =>
            ((CompressionMethodEnum) this._compressionMethod);

        internal uint Crc32
        {
            get => 
                this._crc32;
            set
            {
                this._crc32 = value;
            }
        }

        internal DeflateOptionEnum DeflateOption
        {
            get
            {
                if (this.CompressionMethod == CompressionMethodEnum.Deflated)
                {
                    return (DeflateOptionEnum) ((byte) (this._generalPurposeBitFlag & 6));
                }
                return DeflateOptionEnum.None;
            }
            set
            {
                this._generalPurposeBitFlag = (ushort) (this._generalPurposeBitFlag & 0xfff9);
                this._generalPurposeBitFlag = (ushort) (this._generalPurposeBitFlag | ((ushort) value));
            }
        }

        private bool EncryptedFlag =>
            ((this._generalPurposeBitFlag & 1) == 1);

        internal string FileName =>
            this._stringFileName;

        internal static int FixedMinimalRecordSize =>
            30;

        internal ushort GeneralPurposeBitFlag =>
            this._generalPurposeBitFlag;

        internal uint LastModFileDateTime
        {
            get => 
                this._lastModFileDateTime;
            set
            {
                this._lastModFileDateTime = value;
            }
        }

        internal long Size =>
            ((long) ((30 + this._fileNameLength) + this._extraField.Size));

        internal bool StreamingCreationFlag
        {
            get => 
                ((this._generalPurposeBitFlag & 8) != 0);
            set
            {
                if (value)
                {
                    this._generalPurposeBitFlag = (ushort) (this._generalPurposeBitFlag | 8);
                }
                else
                {
                    this._generalPurposeBitFlag = (ushort) (this._generalPurposeBitFlag & 0xfff7);
                }
            }
        }

        internal long UncompressedSize
        {
            get
            {
                if ((this._extraField.Zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    return this._extraField.UncompressedSize;
                }
                return (long) this._uncompressedSize;
            }
        }

        internal ushort VersionNeededToExtract =>
            this._versionNeededToExtract;
    }
}

