namespace MS.Internal.IO.Zip
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Windows;

    internal class ZipIOExtraField
    {
        private ArrayList _extraFieldElements;
        private ZipIOExtraFieldPaddingElement _paddingElement;
        private ZipIOExtraFieldZip64Element _zip64Element;

        private ZipIOExtraField()
        {
        }

        internal static ZipIOExtraField CreateNew(bool createPadding)
        {
            ZipIOExtraField field = new ZipIOExtraField {
                _zip64Element = ZipIOExtraFieldZip64Element.CreateNew()
            };
            if (createPadding)
            {
                field._paddingElement = ZipIOExtraFieldPaddingElement.CreateNew();
            }
            return field;
        }

        internal static ZipIOExtraField ParseRecord(BinaryReader reader, ZipIOZip64ExtraFieldUsage zip64extraFieldUsage, ushort expectedExtraFieldSize)
        {
            if (expectedExtraFieldSize == 0)
            {
                if (zip64extraFieldUsage != ZipIOZip64ExtraFieldUsage.None)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                return CreateNew(false);
            }
            ZipIOExtraField field = new ZipIOExtraField();
            while (expectedExtraFieldSize > 0)
            {
                if (expectedExtraFieldSize < ZipIOExtraFieldElement.MinimumSize)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                ZipIOExtraFieldElement element = ZipIOExtraFieldElement.Parse(reader, zip64extraFieldUsage);
                ZipIOExtraFieldZip64Element element2 = element as ZipIOExtraFieldZip64Element;
                ZipIOExtraFieldPaddingElement element3 = element as ZipIOExtraFieldPaddingElement;
                if (element2 != null)
                {
                    if (field._zip64Element != null)
                    {
                        throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                    }
                    field._zip64Element = element2;
                }
                else if (element3 != null)
                {
                    if (field._paddingElement != null)
                    {
                        throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                    }
                    field._paddingElement = element3;
                }
                else
                {
                    if (field._extraFieldElements == null)
                    {
                        field._extraFieldElements = new ArrayList(3);
                    }
                    field._extraFieldElements.Add(element);
                }
                expectedExtraFieldSize = (ushort) (expectedExtraFieldSize - element.Size);
            }
            if (expectedExtraFieldSize != 0)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if (field._zip64Element == null)
            {
                field._zip64Element = ZipIOExtraFieldZip64Element.CreateNew();
            }
            return field;
        }

        internal void Save(BinaryWriter writer)
        {
            if (this._zip64Element.SizeField > 0)
            {
                this._zip64Element.Save(writer);
            }
            if (this._paddingElement != null)
            {
                this._paddingElement.Save(writer);
            }
            if (this._extraFieldElements != null)
            {
                foreach (ZipIOExtraFieldElement element in this._extraFieldElements)
                {
                    element.Save(writer);
                }
            }
        }

        internal void UpdatePadding(long size)
        {
            if (Math.Abs(size) <= 0xffffL)
            {
                if ((size > 0L) && (this._paddingElement != null))
                {
                    if (this._paddingElement.PaddingSize >= size)
                    {
                        this._paddingElement.PaddingSize = (ushort) (this._paddingElement.PaddingSize - ((ushort) size));
                    }
                    else if (this._paddingElement.Size == size)
                    {
                        this._paddingElement = null;
                    }
                }
                else if (size < 0L)
                {
                    if (this._paddingElement == null)
                    {
                        size += ZipIOExtraFieldPaddingElement.MinimumFieldDataSize + ZipIOExtraFieldElement.MinimumSize;
                        if (size >= 0L)
                        {
                            this._paddingElement = new ZipIOExtraFieldPaddingElement();
                            this._paddingElement.PaddingSize = (ushort) size;
                        }
                    }
                    else if ((this._paddingElement.PaddingSize - size) <= 0xffffL)
                    {
                        this._paddingElement.PaddingSize = (ushort) (this._paddingElement.PaddingSize - size);
                    }
                }
            }
        }

        internal long CompressedSize
        {
            get => 
                this._zip64Element.CompressedSize;
            set
            {
                this._zip64Element.CompressedSize = value;
            }
        }

        internal uint DiskNumberOfFileStart =>
            this._zip64Element.DiskNumber;

        internal long OffsetOfLocalHeader
        {
            get => 
                this._zip64Element.OffsetOfLocalHeader;
            set
            {
                this._zip64Element.OffsetOfLocalHeader = value;
            }
        }

        internal ushort Size
        {
            get
            {
                ushort num = 0;
                if (this._extraFieldElements != null)
                {
                    foreach (ZipIOExtraFieldElement element in this._extraFieldElements)
                    {
                        num = (ushort) (num + element.Size);
                    }
                }
                num = (ushort) (num + this._zip64Element.Size);
                if (this._paddingElement != null)
                {
                    num = (ushort) (num + this._paddingElement.Size);
                }
                return num;
            }
        }

        internal long UncompressedSize
        {
            get => 
                this._zip64Element.UncompressedSize;
            set
            {
                this._zip64Element.UncompressedSize = value;
            }
        }

        internal ZipIOZip64ExtraFieldUsage Zip64ExtraFieldUsage
        {
            get => 
                this._zip64Element.Zip64ExtraFieldUsage;
            set
            {
                this._zip64Element.Zip64ExtraFieldUsage = value;
            }
        }
    }
}

