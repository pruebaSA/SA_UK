namespace MS.Internal.IO.Zip
{
    using MS.Internal;
    using MS.Internal.IO.Packaging;
    using System;
    using System.Collections;
    using System.IO;
    using System.Windows;

    internal class ZipIOLocalFileBlock : IZipIOBlock, IDisposable
    {
        private ZipIOBlockManager _blockManager;
        private ProgressiveCrcCalculatingStream _crcCalculatingStream;
        private Stream _deflateStream;
        private bool _dirtyFlag;
        private bool _disposedFlag;
        private ArrayList _exposedPublicStreams;
        private ZipIOFileItemStream _fileItemStream;
        private bool _folderFlag;
        private const int _initialExposedPublicStreamsCollectionSize = 5;
        private ZipIOLocalFileDataDescriptor _localFileDataDescriptor;
        private ZipIOLocalFileHeader _localFileHeader;
        private bool _localFileHeaderSaved;
        private long _offset;
        private bool _volumeLabelFlag;

        private ZipIOLocalFileBlock(ZipIOBlockManager blockManager, bool folderFlag, bool volumeLabelFlag)
        {
            this._blockManager = blockManager;
            this._folderFlag = folderFlag;
            this._volumeLabelFlag = volumeLabelFlag;
        }

        internal void CheckDisposed()
        {
            if (this._disposedFlag)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("ZipFileItemDisposed"));
            }
        }

        private static void CheckFileAccessParameter(Stream stream, FileAccess access)
        {
            switch (access)
            {
                case FileAccess.Read:
                    if (!stream.CanRead)
                    {
                        throw new ArgumentException(System.Windows.SR.Get("CanNotReadInWriteOnlyMode"));
                    }
                    break;

                case FileAccess.Write:
                    if (!stream.CanWrite)
                    {
                        throw new ArgumentException(System.Windows.SR.Get("CanNotWriteInReadOnlyMode"));
                    }
                    break;

                case FileAccess.ReadWrite:
                    if (!stream.CanRead || !stream.CanWrite)
                    {
                        throw new ArgumentException(System.Windows.SR.Get("CanNotReadWriteInReadOnlyWriteOnlyMode"));
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("access");
            }
        }

        private void CloseExposedStreams()
        {
            if (this._exposedPublicStreams != null)
            {
                for (int i = this._exposedPublicStreams.Count - 1; i >= 0; i--)
                {
                    ((ZipIOModeEnforcingStream) this._exposedPublicStreams[i]).Close();
                }
            }
        }

        internal static ZipIOLocalFileBlock CreateNew(ZipIOBlockManager blockManager, string fileName, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption)
        {
            ZipIOLocalFileBlock block = new ZipIOLocalFileBlock(blockManager, false, false) {
                _localFileHeader = ZipIOLocalFileHeader.CreateNew(fileName, blockManager.Encoding, compressionMethod, deflateOption, blockManager.Streaming)
            };
            if (blockManager.Streaming)
            {
                block._localFileDataDescriptor = ZipIOLocalFileDataDescriptor.CreateNew();
            }
            block._offset = 0L;
            block._dirtyFlag = true;
            block._fileItemStream = new ZipIOFileItemStream(blockManager, block, block._offset + block._localFileHeader.Size, 0L);
            if (compressionMethod == CompressionMethodEnum.Deflated)
            {
                block._deflateStream = new CompressStream(block._fileItemStream, 0L, true);
                block._crcCalculatingStream = new ProgressiveCrcCalculatingStream(blockManager, block._deflateStream);
                return block;
            }
            block._crcCalculatingStream = new ProgressiveCrcCalculatingStream(blockManager, block._fileItemStream);
            return block;
        }

        internal void DeregisterExposedStream(Stream exposedStream)
        {
            this._exposedPublicStreams.Remove(exposedStream);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this._disposedFlag)
            {
                try
                {
                    this.CloseExposedStreams();
                    this._crcCalculatingStream.Close();
                    if (this._deflateStream != null)
                    {
                        this._deflateStream.Close();
                    }
                    this._fileItemStream.Close();
                }
                finally
                {
                    this._disposedFlag = true;
                    this._crcCalculatingStream = null;
                    this._deflateStream = null;
                    this._fileItemStream = null;
                }
            }
        }

        private void FlushExposedStreams()
        {
            this._crcCalculatingStream.Flush();
            if (((this._deflateStream != null) && !this._localFileHeader.StreamingCreationFlag) && ((this._exposedPublicStreams == null) || (this._exposedPublicStreams.Count == 0)))
            {
                ((CompressStream) this._deflateStream).Reset();
            }
        }

        public bool GetDirtyFlag(bool closingFlag)
        {
            this.CheckDisposed();
            bool flag = false;
            if (this._deflateStream != null)
            {
                flag = ((CompressStream) this._deflateStream).IsDirty(closingFlag);
            }
            if (!this._dirtyFlag && !this._fileItemStream.DirtyFlag)
            {
                return flag;
            }
            return true;
        }

        internal Stream GetStream(FileMode mode, FileAccess access)
        {
            this.CheckDisposed();
            CheckFileAccessParameter(this._blockManager.Stream, access);
            switch (mode)
            {
                case FileMode.CreateNew:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported", new object[] { "CreateNew" }));

                case FileMode.Create:
                    if (!this._blockManager.Stream.CanWrite)
                    {
                        throw new InvalidOperationException(System.Windows.SR.Get("CanNotWriteInReadOnlyMode"));
                    }
                    if ((this._crcCalculatingStream != null) && !this._blockManager.Streaming)
                    {
                        this._crcCalculatingStream.SetLength(0L);
                    }
                    break;

                case FileMode.Open:
                case FileMode.OpenOrCreate:
                    break;

                case FileMode.Truncate:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported", new object[] { "Truncate" }));

                case FileMode.Append:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported", new object[] { "Append" }));

                default:
                    throw new ArgumentOutOfRangeException("mode");
            }
            if ((this._blockManager.Streaming && (this._exposedPublicStreams != null)) && (this._exposedPublicStreams.Count > 0))
            {
                return (Stream) this._exposedPublicStreams[0];
            }
            Stream exposedStream = new ZipIOModeEnforcingStream(this._crcCalculatingStream, access, this._blockManager, this);
            this.RegisterExposedStream(exposedStream);
            return exposedStream;
        }

        public void Move(long shiftSize)
        {
            this.CheckDisposed();
            if (shiftSize != 0L)
            {
                this._offset += shiftSize;
                this._fileItemStream.Move(shiftSize);
                this._dirtyFlag = true;
            }
        }

        private void ParseRecord(BinaryReader reader, string fileName, long position, ZipIOCentralDirectoryBlock centralDir, ZipIOCentralDirectoryFileHeader centralDirFileHeader)
        {
            this.CheckDisposed();
            this._localFileHeader = ZipIOLocalFileHeader.ParseRecord(reader, this._blockManager.Encoding);
            if (this._localFileHeader.StreamingCreationFlag)
            {
                this._blockManager.Stream.Seek(centralDirFileHeader.CompressedSize, SeekOrigin.Current);
                this._localFileDataDescriptor = ZipIOLocalFileDataDescriptor.ParseRecord(reader, centralDirFileHeader.CompressedSize, centralDirFileHeader.UncompressedSize, centralDirFileHeader.Crc32, this._localFileHeader.VersionNeededToExtract);
            }
            else
            {
                this._localFileDataDescriptor = null;
            }
            this._offset = position;
            this._dirtyFlag = false;
            this._fileItemStream = new ZipIOFileItemStream(this._blockManager, this, position + this._localFileHeader.Size, centralDirFileHeader.CompressedSize);
            if (this._localFileHeader.CompressionMethod == CompressionMethodEnum.Deflated)
            {
                this._deflateStream = new CompressStream(this._fileItemStream, centralDirFileHeader.UncompressedSize);
                this._crcCalculatingStream = new ProgressiveCrcCalculatingStream(this._blockManager, this._deflateStream, this.Crc32);
            }
            else
            {
                if (this._localFileHeader.CompressionMethod != CompressionMethodEnum.Stored)
                {
                    throw new NotSupportedException(System.Windows.SR.Get("ZipNotSupportedCompressionMethod"));
                }
                this._crcCalculatingStream = new ProgressiveCrcCalculatingStream(this._blockManager, this._fileItemStream, this.Crc32);
            }
            this.Validate(fileName, centralDir, centralDirFileHeader);
        }

        public PreSaveNotificationScanControlInstruction PreSaveNotification(long offset, long size)
        {
            this.CheckDisposed();
            return this._fileItemStream.PreSaveNotification(offset, size);
        }

        private void RegisterExposedStream(Stream exposedStream)
        {
            if (this._exposedPublicStreams == null)
            {
                this._exposedPublicStreams = new ArrayList(5);
            }
            this._exposedPublicStreams.Add(exposedStream);
        }

        public void Save()
        {
            this.CheckDisposed();
            if (this.GetDirtyFlag(true))
            {
                this._fileItemStream.PreSaveNotification(this._offset, this._localFileHeader.Size);
                BinaryWriter binaryWriter = this._blockManager.BinaryWriter;
                if (this._blockManager.Stream.Position != this._offset)
                {
                    this._blockManager.Stream.Seek(this._offset, SeekOrigin.Begin);
                }
                this._localFileHeader.Save(binaryWriter);
                this._fileItemStream.Save();
                this._dirtyFlag = false;
            }
        }

        internal void SaveStreaming(bool closingFlag)
        {
            this.CheckDisposed();
            if (this.GetDirtyFlag(closingFlag))
            {
                BinaryWriter binaryWriter = this._blockManager.BinaryWriter;
                if (!this._localFileHeaderSaved)
                {
                    this._offset = this._blockManager.Stream.Position;
                    this._localFileHeader.Save(binaryWriter);
                    this._localFileHeaderSaved = true;
                }
                this.FlushExposedStreams();
                this._fileItemStream.SaveStreaming();
                if (closingFlag)
                {
                    this._localFileDataDescriptor.UncompressedSize = this._crcCalculatingStream.Length;
                    this._localFileDataDescriptor.Crc32 = this._crcCalculatingStream.CalculateCrc();
                    this.CloseExposedStreams();
                    if (this._deflateStream != null)
                    {
                        this._deflateStream.Close();
                        this._fileItemStream.SaveStreaming();
                    }
                    this._localFileDataDescriptor.CompressedSize = this._fileItemStream.Length;
                    this._localFileDataDescriptor.Save(binaryWriter);
                    this._dirtyFlag = false;
                }
            }
        }

        internal static ZipIOLocalFileBlock SeekableLoad(ZipIOBlockManager blockManager, string fileName)
        {
            ZipIOCentralDirectoryBlock centralDirectoryBlock = blockManager.CentralDirectoryBlock;
            ZipIOCentralDirectoryFileHeader centralDirectoryFileHeader = centralDirectoryBlock.GetCentralDirectoryFileHeader(fileName);
            long offsetOfLocalHeader = centralDirectoryFileHeader.OffsetOfLocalHeader;
            bool folderFlag = centralDirectoryFileHeader.FolderFlag;
            bool volumeLabelFlag = centralDirectoryFileHeader.VolumeLabelFlag;
            blockManager.Stream.Seek(offsetOfLocalHeader, SeekOrigin.Begin);
            ZipIOLocalFileBlock block2 = new ZipIOLocalFileBlock(blockManager, folderFlag, volumeLabelFlag);
            block2.ParseRecord(blockManager.BinaryReader, fileName, offsetOfLocalHeader, centralDirectoryBlock, centralDirectoryFileHeader);
            return block2;
        }

        public void UpdateReferences(bool closingFlag)
        {
            Invariant.Assert(!this._blockManager.Streaming);
            this.CheckDisposed();
            if (closingFlag)
            {
                this.CloseExposedStreams();
            }
            else
            {
                this.FlushExposedStreams();
            }
            if (this.GetDirtyFlag(closingFlag))
            {
                long size = this._localFileHeader.Size;
                long length = this._crcCalculatingStream.Length;
                this._localFileHeader.Crc32 = this._crcCalculatingStream.CalculateCrc();
                if (closingFlag)
                {
                    this._crcCalculatingStream.Close();
                    if (this._deflateStream != null)
                    {
                        this._deflateStream.Close();
                    }
                }
                if (this._fileItemStream.DataChanged)
                {
                    this._localFileHeader.LastModFileDateTime = ZipIOBlockManager.ToMsDosDateTime(DateTime.Now);
                }
                long compressedSize = this._fileItemStream.Length;
                this._localFileHeader.UpdateZip64Structures(compressedSize, length, this.Offset);
                this._localFileHeader.UpdatePadding(this._localFileHeader.Size - size);
                this._localFileHeader.StreamingCreationFlag = false;
                this._localFileDataDescriptor = null;
                this._fileItemStream.Move((this.Offset + this._localFileHeader.Size) - this._fileItemStream.Offset);
                this._dirtyFlag = true;
            }
        }

        private void Validate(string fileName, ZipIOCentralDirectoryBlock centralDir, ZipIOCentralDirectoryFileHeader centralDirFileHeader)
        {
            if (string.CompareOrdinal(this._localFileHeader.FileName, fileName) != 0)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if ((((this.VersionNeededToExtract != centralDirFileHeader.VersionNeededToExtract) || (this.GeneralPurposeBitFlag != centralDirFileHeader.GeneralPurposeBitFlag)) || ((this.CompressedSize != centralDirFileHeader.CompressedSize) || (this.UncompressedSize != centralDirFileHeader.UncompressedSize))) || ((this.CompressionMethod != centralDirFileHeader.CompressionMethod) || (this.Crc32 != centralDirFileHeader.Crc32)))
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
            if ((this.Offset + this.Size) > centralDir.Offset)
            {
                throw new FileFormatException(System.Windows.SR.Get("CorruptedData"));
            }
        }

        internal long CompressedSize
        {
            get
            {
                this.CheckDisposed();
                if (this._localFileHeader.StreamingCreationFlag)
                {
                    Invariant.Assert(this._localFileDataDescriptor != null);
                    return this._localFileDataDescriptor.CompressedSize;
                }
                return this._localFileHeader.CompressedSize;
            }
        }

        internal CompressionMethodEnum CompressionMethod
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.CompressionMethod;
            }
        }

        internal uint Crc32
        {
            get
            {
                this.CheckDisposed();
                if (this._localFileHeader.StreamingCreationFlag)
                {
                    Invariant.Assert(this._localFileDataDescriptor != null);
                    return this._localFileDataDescriptor.Crc32;
                }
                return this._localFileHeader.Crc32;
            }
        }

        internal DeflateOptionEnum DeflateOption
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.DeflateOption;
            }
        }

        internal string FileName
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.FileName;
            }
        }

        internal bool FolderFlag
        {
            get
            {
                this.CheckDisposed();
                return this._folderFlag;
            }
        }

        internal ushort GeneralPurposeBitFlag
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.GeneralPurposeBitFlag;
            }
        }

        internal uint LastModFileDateTime
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.LastModFileDateTime;
            }
        }

        public long Offset
        {
            get
            {
                this.CheckDisposed();
                return this._offset;
            }
        }

        public long Size
        {
            get
            {
                this.CheckDisposed();
                long num = this._localFileHeader.Size + this._fileItemStream.Length;
                if (this._localFileDataDescriptor != null)
                {
                    num += this._localFileDataDescriptor.Size;
                }
                return num;
            }
        }

        internal long UncompressedSize
        {
            get
            {
                this.CheckDisposed();
                if (this._localFileHeader.StreamingCreationFlag)
                {
                    Invariant.Assert(this._localFileDataDescriptor != null);
                    return this._localFileDataDescriptor.UncompressedSize;
                }
                return this._localFileHeader.UncompressedSize;
            }
        }

        internal ushort VersionNeededToExtract
        {
            get
            {
                this.CheckDisposed();
                return this._localFileHeader.VersionNeededToExtract;
            }
        }

        internal bool VolumeLabelFlag
        {
            get
            {
                this.CheckDisposed();
                return this._volumeLabelFlag;
            }
        }
    }
}

