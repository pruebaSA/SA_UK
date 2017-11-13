namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;
    using System.Windows;

    internal class ZipIOExtraFieldZip64Element : ZipIOExtraFieldElement
    {
        private ulong _compressedSize;
        private const ushort _constantFieldId = 1;
        private uint _diskNumber;
        private ulong _offsetOfLocalHeader;
        private ulong _uncompressedSize;
        private ZipIOZip64ExtraFieldUsage _zip64ExtraFieldUsage;

        internal ZipIOExtraFieldZip64Element() : base(1)
        {
            this._zip64ExtraFieldUsage = ZipIOZip64ExtraFieldUsage.None;
        }

        internal static ZipIOExtraFieldZip64Element CreateNew() => 
            new ZipIOExtraFieldZip64Element();

        internal override void ParseDataField(BinaryReader reader, ushort size)
        {
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._uncompressedSize = reader.ReadUInt64();
                if (size < 8)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                size = (ushort) (size - 8);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._compressedSize = reader.ReadUInt64();
                if (size < 8)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                size = (ushort) (size - 8);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._offsetOfLocalHeader = reader.ReadUInt64();
                if (size < 8)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                size = (ushort) (size - 8);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.DiskNumber) != ZipIOZip64ExtraFieldUsage.None)
            {
                this._diskNumber = reader.ReadUInt32();
                if (size < 4)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                size = (ushort) (size - 4);
            }
            if (size != 0)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            this.Validate();
        }

        internal override void Save(BinaryWriter writer)
        {
            writer.Write((ushort) 1);
            writer.Write(this.SizeField);
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._uncompressedSize);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._compressedSize);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._offsetOfLocalHeader);
            }
            if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.DiskNumber) != ZipIOZip64ExtraFieldUsage.None)
            {
                writer.Write(this._diskNumber);
            }
        }

        private void Validate()
        {
            if (((this._compressedSize >= 0x7fffffffffffffffL) || (this._uncompressedSize >= 0x7fffffffffffffffL)) || (this._offsetOfLocalHeader >= 0x7fffffffffffffffL))
            {
                throw new NotSupportedException(System.Windows.SR.Get("Zip64StructuresTooLarge"));
            }
            if (this._diskNumber != 0)
            {
                throw new NotSupportedException(System.Windows.SR.Get("NotSupportedMultiDisk"));
            }
        }

        internal long CompressedSize
        {
            get => 
                ((long) this._compressedSize);
            set
            {
                this._zip64ExtraFieldUsage |= ZipIOZip64ExtraFieldUsage.CompressedSize;
                this._compressedSize = (ulong) value;
            }
        }

        internal static ushort ConstantFieldId =>
            1;

        internal uint DiskNumber =>
            this._diskNumber;

        internal long OffsetOfLocalHeader
        {
            get => 
                ((long) this._offsetOfLocalHeader);
            set
            {
                this._zip64ExtraFieldUsage |= ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader;
                this._offsetOfLocalHeader = (ulong) value;
            }
        }

        internal override ushort Size
        {
            get
            {
                if (this.SizeField == 0)
                {
                    return 0;
                }
                return (ushort) (this.SizeField + ZipIOExtraFieldElement.MinimumSize);
            }
        }

        internal override ushort SizeField
        {
            get
            {
                ushort num = 0;
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.UncompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort) (num + 8);
                }
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.CompressedSize) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort) (num + 8);
                }
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.OffsetOfLocalHeader) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort) (num + 8);
                }
                if ((this._zip64ExtraFieldUsage & ZipIOZip64ExtraFieldUsage.DiskNumber) != ZipIOZip64ExtraFieldUsage.None)
                {
                    num = (ushort) (num + 4);
                }
                return num;
            }
        }

        internal long UncompressedSize
        {
            get => 
                ((long) this._uncompressedSize);
            set
            {
                this._zip64ExtraFieldUsage |= ZipIOZip64ExtraFieldUsage.UncompressedSize;
                this._uncompressedSize = (ulong) value;
            }
        }

        internal ZipIOZip64ExtraFieldUsage Zip64ExtraFieldUsage
        {
            get => 
                this._zip64ExtraFieldUsage;
            set
            {
                this._zip64ExtraFieldUsage = value;
            }
        }
    }
}

