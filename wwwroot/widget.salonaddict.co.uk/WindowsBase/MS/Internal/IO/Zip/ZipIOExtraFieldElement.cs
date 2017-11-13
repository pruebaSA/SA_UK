namespace MS.Internal.IO.Zip
{
    using MS.Internal.IO.Packaging;
    using System;
    using System.IO;
    using System.Windows;

    internal class ZipIOExtraFieldElement
    {
        private byte[] _data;
        private ushort _id;
        private static readonly ushort _minimumSize = 4;
        private ushort _size;

        internal ZipIOExtraFieldElement(ushort id)
        {
            this._id = id;
        }

        private ZipIOExtraFieldElement(ushort id, byte[] data)
        {
            this._id = id;
            this._data = data;
            this._size = (ushort) data.Length;
        }

        internal static ZipIOExtraFieldElement Parse(BinaryReader reader, ZipIOZip64ExtraFieldUsage zip64extraFieldUsage)
        {
            ZipIOExtraFieldElement element;
            ushort id = reader.ReadUInt16();
            ushort size = reader.ReadUInt16();
            if (id == ZipIOExtraFieldZip64Element.ConstantFieldId)
            {
                element = new ZipIOExtraFieldZip64Element();
                ((ZipIOExtraFieldZip64Element) element).Zip64ExtraFieldUsage = zip64extraFieldUsage;
            }
            else if (id == ZipIOExtraFieldPaddingElement.ConstantFieldId)
            {
                if (size < ZipIOExtraFieldPaddingElement.MinimumFieldDataSize)
                {
                    element = new ZipIOExtraFieldElement(id);
                }
                else
                {
                    byte[] sniffiedBytes = reader.ReadBytes(ZipIOExtraFieldPaddingElement.SignatureSize);
                    if (ZipIOExtraFieldPaddingElement.MatchesPaddingSignature(sniffiedBytes))
                    {
                        element = new ZipIOExtraFieldPaddingElement();
                    }
                    else
                    {
                        element = new ZipIOExtraFieldElement(id, sniffiedBytes);
                    }
                }
            }
            else
            {
                element = new ZipIOExtraFieldElement(id);
            }
            element.ParseDataField(reader, size);
            return element;
        }

        internal virtual void ParseDataField(BinaryReader reader, ushort size)
        {
            if (this._data == null)
            {
                this._data = reader.ReadBytes(size);
                if (this._data.Length != size)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
            }
            else
            {
                byte[] sourceArray = this._data;
                this._data = new byte[size];
                Array.Copy(sourceArray, this._data, (int) this._size);
                if ((PackagingUtilities.ReliableRead(reader, this._data, this._size, size - this._size) + this._size) != size)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
            }
            this._size = size;
        }

        internal virtual void Save(BinaryWriter writer)
        {
            writer.Write(this._id);
            writer.Write(this._size);
            writer.Write(this._data);
        }

        internal virtual byte[] DataField =>
            this._data;

        internal static ushort MinimumSize =>
            _minimumSize;

        internal virtual ushort Size =>
            ((ushort) (this._size + _minimumSize));

        internal virtual ushort SizeField =>
            this._size;
    }
}

