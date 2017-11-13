namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;

    internal class ZipIOExtraFieldPaddingElement : ZipIOExtraFieldElement
    {
        private const ushort _constantFieldId = 0xa220;
        private ushort _initialRequestedPaddingSize;
        private static readonly ushort _minimumFieldDataSize = 4;
        private const ushort _newInitialPaddingSize = 20;
        private ushort _paddingSize;
        private const ushort _signature = 0xa028;
        private static readonly ushort _signatureSize = 2;

        internal ZipIOExtraFieldPaddingElement() : base(0xa220)
        {
            this._initialRequestedPaddingSize = 20;
            this._paddingSize = this._initialRequestedPaddingSize;
        }

        internal static ZipIOExtraFieldPaddingElement CreateNew() => 
            new ZipIOExtraFieldPaddingElement();

        internal static bool MatchesPaddingSignature(byte[] sniffiedBytes)
        {
            if (sniffiedBytes.Length < _signatureSize)
            {
                return false;
            }
            if (BitConverter.ToUInt16(sniffiedBytes, 0) != 0xa028)
            {
                return false;
            }
            return true;
        }

        internal override void ParseDataField(BinaryReader reader, ushort size)
        {
            this._initialRequestedPaddingSize = reader.ReadUInt16();
            size = (ushort) (size - _minimumFieldDataSize);
            this._paddingSize = size;
            if (this._paddingSize != 0)
            {
                reader.BaseStream.Seek((long) size, SeekOrigin.Current);
            }
        }

        internal override void Save(BinaryWriter writer)
        {
            writer.Write((ushort) 0xa220);
            writer.Write(this.SizeField);
            writer.Write((ushort) 0xa028);
            writer.Write(this._initialRequestedPaddingSize);
            for (int i = 0; i < this._paddingSize; i++)
            {
                writer.Write((byte) 0);
            }
        }

        internal static ushort ConstantFieldId =>
            0xa220;

        internal static ushort MinimumFieldDataSize =>
            _minimumFieldDataSize;

        internal ushort PaddingSize
        {
            get => 
                this._paddingSize;
            set
            {
                this._paddingSize = value;
            }
        }

        internal static ushort SignatureSize =>
            _signatureSize;

        internal override ushort Size =>
            ((ushort) (this.SizeField + ZipIOExtraFieldElement.MinimumSize));

        internal override ushort SizeField =>
            ((ushort) (_minimumFieldDataSize + this._paddingSize));
    }
}

