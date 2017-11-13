namespace MS.Internal.IO.Zip
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.IO;
    using System.Windows;

    internal class ZipIOCentralDirectoryBlock : IZipIOBlock
    {
        private ZipIOBlockManager _blockManager;
        private IDictionary _centralDirectoryDictionary;
        private const int _centralDirectoryDictionaryInitialSize = 50;
        private ZipIOCentralDirectoryDigitalSignature _centralDirectoryDigitalSignature;
        private bool _dirtyFlag;
        private static IComparer _headerOffsetComparer = new HeaderFileOffsetComparer();
        private long _offset;

        private ZipIOCentralDirectoryBlock(ZipIOBlockManager blockManager)
        {
            this._blockManager = blockManager;
        }

        internal void AddFileBlock(ZipIOLocalFileBlock fileBlock)
        {
            this._dirtyFlag = true;
            ZipIOCentralDirectoryFileHeader header = ZipIOCentralDirectoryFileHeader.CreateNew(this._blockManager.Encoding, fileBlock);
            this.CentralDirectoryDictionary.Add(header.FileName, header);
        }

        internal static ZipIOCentralDirectoryBlock CreateNew(ZipIOBlockManager blockManager) => 
            new ZipIOCentralDirectoryBlock(blockManager) { 
                _offset = 0L,
                _dirtyFlag = true,
                _centralDirectoryDigitalSignature = null
            };

        internal bool FileExists(string fileName) => 
            this.CentralDirectoryDictionary.Contains(fileName);

        internal ZipIOCentralDirectoryFileHeader GetCentralDirectoryFileHeader(string fileName) => 
            ((ZipIOCentralDirectoryFileHeader) this.CentralDirectoryDictionary[fileName]);

        public bool GetDirtyFlag(bool closingFlag) => 
            this._dirtyFlag;

        internal ICollection GetFileNamesCollection() => 
            this.CentralDirectoryDictionary.Keys;

        public void Move(long shiftSize)
        {
            if (shiftSize != 0L)
            {
                this._offset += shiftSize;
                this._dirtyFlag = true;
            }
        }

        private void ParseRecord(BinaryReader reader, long centralDirectoryOffset, int centralDirectoryCount, long expectedCentralDirectorySize)
        {
            if (centralDirectoryCount > 0)
            {
                SortedList list = new SortedList(centralDirectoryCount);
                for (int i = 0; i < centralDirectoryCount; i++)
                {
                    ZipIOCentralDirectoryFileHeader header = ZipIOCentralDirectoryFileHeader.ParseRecord(reader, this._blockManager.Encoding);
                    list.Add(header.OffsetOfLocalHeader, header);
                }
                if ((reader.BaseStream.Position - centralDirectoryOffset) > expectedCentralDirectorySize)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                foreach (ZipIOCentralDirectoryFileHeader header2 in list.Values)
                {
                    this.CentralDirectoryDictionary.Add(header2.FileName, header2);
                }
                this._centralDirectoryDigitalSignature = ZipIOCentralDirectoryDigitalSignature.ParseRecord(reader);
            }
            this._offset = centralDirectoryOffset;
            this._dirtyFlag = false;
            this.Validate(expectedCentralDirectorySize);
        }

        public PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size) => 
            PreSaveNotificationScanControlInstruction.Stop;

        internal void RemoveFileBlock(string fileName)
        {
            this._dirtyFlag = true;
            this.CentralDirectoryDictionary.Remove(fileName);
            if (this.CentralDirectoryDictionary.Count == 0)
            {
                this._centralDirectoryDigitalSignature = null;
            }
        }

        public void Save()
        {
            if (this._dirtyFlag)
            {
                if (this.CentralDirectoryDictionary.Count > 0)
                {
                    BinaryWriter binaryWriter = this._blockManager.BinaryWriter;
                    if (this._blockManager.Streaming)
                    {
                        SortedList list = new SortedList(this.CentralDirectoryDictionary.Count);
                        foreach (ZipIOCentralDirectoryFileHeader header in this.CentralDirectoryDictionary.Values)
                        {
                            list.Add(header.OffsetOfLocalHeader, header);
                        }
                        foreach (ZipIOCentralDirectoryFileHeader header2 in list.Values)
                        {
                            header2.Save(binaryWriter);
                        }
                    }
                    else
                    {
                        if (this._blockManager.Stream.Position != this._offset)
                        {
                            this._blockManager.Stream.Seek(this._offset, SeekOrigin.Begin);
                        }
                        foreach (ZipIOCentralDirectoryFileHeader header3 in this.CentralDirectoryDictionary.Values)
                        {
                            header3.Save(binaryWriter);
                        }
                    }
                    binaryWriter.Flush();
                }
                this._dirtyFlag = false;
            }
        }

        internal static ZipIOCentralDirectoryBlock SeekableLoad(ZipIOBlockManager blockManager)
        {
            ZipIOZip64EndOfCentralDirectoryBlock block = blockManager.Zip64EndOfCentralDirectoryBlock;
            blockManager.Stream.Seek(block.OffsetOfStartOfCentralDirectory, SeekOrigin.Begin);
            ZipIOCentralDirectoryBlock block2 = new ZipIOCentralDirectoryBlock(blockManager);
            block2.ParseRecord(blockManager.BinaryReader, block.OffsetOfStartOfCentralDirectory, block.TotalNumberOfEntriesInTheCentralDirectory, block.SizeOfCentralDirectory);
            return block2;
        }

        public void UpdateReferences(bool closingFlag)
        {
            foreach (IZipIOBlock block in (IEnumerable) this._blockManager)
            {
                ZipIOLocalFileBlock fileBlock = block as ZipIOLocalFileBlock;
                ZipIORawDataFileBlock block3 = block as ZipIORawDataFileBlock;
                if (fileBlock != null)
                {
                    ZipIOCentralDirectoryFileHeader header = (ZipIOCentralDirectoryFileHeader) this.CentralDirectoryDictionary[fileBlock.FileName];
                    if (header.UpdateIfNeeded(fileBlock))
                    {
                        this._dirtyFlag = true;
                    }
                }
                else if (block3 != null)
                {
                    long diskImageShift = block3.DiskImageShift;
                    if (diskImageShift != 0L)
                    {
                        foreach (ZipIOCentralDirectoryFileHeader header2 in this.CentralDirectoryDictionary.Values)
                        {
                            if (block3.DiskImageContains(header2.OffsetOfLocalHeader))
                            {
                                header2.MoveReference(diskImageShift);
                                this._dirtyFlag = true;
                            }
                        }
                    }
                }
            }
        }

        private void Validate(long expectedCentralDirectorySize)
        {
            long num = 0L;
            foreach (ZipIOCentralDirectoryFileHeader header in this.CentralDirectoryDictionary.Values)
            {
                if ((num == 0L) && (header.OffsetOfLocalHeader != 0L))
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                if (header.OffsetOfLocalHeader < num)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
                num += (header.CompressedSize + ZipIOLocalFileHeader.FixedMinimalRecordSize) + header.FileName.Length;
            }
            if (this._offset < num)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if (this.Size != expectedCentralDirectorySize)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if (this._blockManager.Zip64EndOfCentralDirectoryBlock.Size == 0L)
            {
                if ((this._offset + expectedCentralDirectorySize) != this._blockManager.EndOfCentralDirectoryBlock.Offset)
                {
                    throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
                }
            }
            else if ((((this._offset + expectedCentralDirectorySize) != this._blockManager.Zip64EndOfCentralDirectoryBlock.Offset) || ((this._blockManager.Zip64EndOfCentralDirectoryBlock.Offset + this._blockManager.Zip64EndOfCentralDirectoryBlock.Size) != this._blockManager.Zip64EndOfCentralDirectoryLocatorBlock.Offset)) || ((this._blockManager.Zip64EndOfCentralDirectoryLocatorBlock.Offset + this._blockManager.Zip64EndOfCentralDirectoryLocatorBlock.Size) != this._blockManager.EndOfCentralDirectoryBlock.Offset))
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
        }

        private IDictionary CentralDirectoryDictionary
        {
            get
            {
                if (this._centralDirectoryDictionary == null)
                {
                    if (this._blockManager.Streaming)
                    {
                        this._centralDirectoryDictionary = new Hashtable(50, StringComparer.Ordinal);
                    }
                    else
                    {
                        this._centralDirectoryDictionary = new OrderedDictionary(50, StringComparer.Ordinal);
                    }
                }
                return this._centralDirectoryDictionary;
            }
        }

        internal int Count =>
            this.CentralDirectoryDictionary.Count;

        internal bool IsZip64BitRequiredForStoring
        {
            get
            {
                if ((this.Count < 0xffff) && (this.Offset < 0xffffffffL))
                {
                    return (this.Size >= 0xffffffffL);
                }
                return true;
            }
        }

        public long Offset =>
            this._offset;

        public long Size
        {
            get
            {
                long num = 0L;
                if (this.CentralDirectoryDictionary.Count > 0)
                {
                    foreach (ZipIOCentralDirectoryFileHeader header in this.CentralDirectoryDictionary.Values)
                    {
                        num += header.Size;
                    }
                }
                return num;
            }
        }

        private class HeaderFileOffsetComparer : IComparer
        {
            int IComparer.Compare(object o1, object o2)
            {
                ZipIOCentralDirectoryFileHeader header = o1 as ZipIOCentralDirectoryFileHeader;
                ZipIOCentralDirectoryFileHeader header2 = o2 as ZipIOCentralDirectoryFileHeader;
                if (header.OffsetOfLocalHeader > header2.OffsetOfLocalHeader)
                {
                    return 1;
                }
                if (header.OffsetOfLocalHeader < header2.OffsetOfLocalHeader)
                {
                    return -1;
                }
                return 0;
            }
        }
    }
}

