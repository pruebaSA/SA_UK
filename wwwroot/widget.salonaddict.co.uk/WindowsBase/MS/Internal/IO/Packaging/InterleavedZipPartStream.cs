namespace MS.Internal.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Zip;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Packaging;
    using System.Windows;

    internal class InterleavedZipPartStream : Stream
    {
        private bool _closed;
        private long _currentOffset;
        private int _currentPieceNumber;
        private PieceDirectory _dir;
        private long? _offsetForCurrentPieceNumber;

        internal InterleavedZipPartStream(ZipPackagePart owningPart, FileMode mode, FileAccess access) : this(PackUriHelper.GetStringForPartUri(owningPart.Uri), owningPart.PieceDescriptors, mode, access)
        {
        }

        internal InterleavedZipPartStream(string partName, List<PieceInfo> sortedPieceInfoList, FileMode mode, FileAccess access)
        {
            this._dir = new PieceDirectory(sortedPieceInfoList, mode, access);
            Invariant.Assert(this._dir.GetStartOffset(this.GetCurrentPieceNumber()) == 0L);
        }

        private void CheckClosed()
        {
            if (this._closed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && !this._closed)
                {
                    this._dir.Close();
                }
            }
            finally
            {
                this._closed = true;
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.CheckClosed();
            this._dir.Flush();
        }

        private int GetCurrentPieceNumber()
        {
            long? nullable = this._offsetForCurrentPieceNumber;
            long num = this._currentOffset;
            if ((nullable.GetValueOrDefault() != num) || !nullable.HasValue)
            {
                this._currentPieceNumber = this._dir.GetPieceNumberFromOffset(this._currentOffset);
                this._offsetForCurrentPieceNumber = new long?(this._currentOffset);
            }
            return this._currentPieceNumber;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.CheckClosed();
            PackagingUtilities.VerifyStreamReadArgs(this, buffer, offset, count);
            if (count == 0)
            {
                return 0;
            }
            int currentPieceNumber = this.GetCurrentPieceNumber();
            int num2 = 0;
            Stream stream = this._dir.GetStream(currentPieceNumber);
            stream.Seek(this._currentOffset - this._dir.GetStartOffset(currentPieceNumber), SeekOrigin.Begin);
            while (num2 < count)
            {
                int num3 = stream.Read(buffer, offset + num2, count - num2);
                if (num3 == 0)
                {
                    if (this._dir.IsLastPiece(currentPieceNumber))
                    {
                        break;
                    }
                    currentPieceNumber++;
                    Invariant.Assert(this._dir.GetStartOffset(currentPieceNumber) == (this._currentOffset + num2));
                    this._dir.GetStream(currentPieceNumber).Seek(0L, SeekOrigin.Begin);
                }
                num2 += num3;
            }
            this._currentOffset += num2;
            return num2;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.CheckClosed();
            if (!this.CanSeek)
            {
                throw new NotSupportedException(System.Windows.SR.Get("SeekNotSupported"));
            }
            switch (origin)
            {
                case SeekOrigin.Begin:
                    break;

                case SeekOrigin.Current:
                    offset += this._currentOffset;
                    break;

                case SeekOrigin.End:
                    offset += this.Length;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("origin");
            }
            if (offset < 0L)
            {
                throw new ArgumentException(System.Windows.SR.Get("SeekNegative"));
            }
            this._currentOffset = offset;
            return this._currentOffset;
        }

        public override void SetLength(long newLength)
        {
            int pieceNumberFromOffset;
            this.CheckClosed();
            if (newLength < 0L)
            {
                throw new ArgumentOutOfRangeException("newLength");
            }
            if (!this.CanWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("StreamDoesNotSupportWrite"));
            }
            if (!this.CanSeek)
            {
                throw new NotSupportedException(System.Windows.SR.Get("SeekNotSupported"));
            }
            if (newLength == 0L)
            {
                pieceNumberFromOffset = 0;
            }
            else
            {
                pieceNumberFromOffset = this._dir.GetPieceNumberFromOffset(newLength - 1L);
            }
            this._dir.SetLogicalLastPiece(pieceNumberFromOffset);
            Stream stream = this._dir.GetStream(pieceNumberFromOffset);
            long num2 = newLength - this._dir.GetStartOffset(pieceNumberFromOffset);
            stream.SetLength(num2);
            if (this._currentOffset > newLength)
            {
                this._currentOffset = newLength;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckClosed();
            PackagingUtilities.VerifyStreamWriteArgs(this, buffer, offset, count);
            if (count != 0)
            {
                int num = 0;
                int currentPieceNumber = this.GetCurrentPieceNumber();
                Stream stream = this._dir.GetStream(currentPieceNumber);
                stream.Seek(this._currentOffset - this._dir.GetStartOffset(currentPieceNumber), SeekOrigin.Begin);
                while (num < count)
                {
                    int num3 = count - num;
                    if (!this._dir.IsLastPiece(currentPieceNumber))
                    {
                        long num4 = this._currentOffset + num;
                        long num5 = this._dir.GetStartOffset(currentPieceNumber + 1) - 1L;
                        if (num3 > ((num5 - num4) + 1L))
                        {
                            num3 = (int) ((num5 - num4) + 1L);
                        }
                    }
                    stream.Write(buffer, offset + num, num3);
                    num += num3;
                    if (!this._dir.IsLastPiece(currentPieceNumber) && (num < count))
                    {
                        currentPieceNumber++;
                        this._dir.GetStream(currentPieceNumber).Seek(0L, SeekOrigin.Begin);
                    }
                }
                Invariant.Assert(num == count);
                this._currentOffset += num;
            }
        }

        public override bool CanRead =>
            (!this._closed && this._dir.GetStream(0).CanRead);

        public override bool CanSeek =>
            (!this._closed && this._dir.GetStream(0).CanSeek);

        public override bool CanWrite =>
            (!this._closed && this._dir.GetStream(0).CanWrite);

        public override long Length
        {
            get
            {
                this.CheckClosed();
                Invariant.Assert(this.CanSeek);
                long num = 0L;
                for (int i = 0; i < this._dir.GetNumberOfPieces(); i++)
                {
                    num += this._dir.GetStream(i).Length;
                }
                return num;
            }
        }

        public override long Position
        {
            get
            {
                this.CheckClosed();
                return this._currentOffset;
            }
            set
            {
                this.CheckClosed();
                this.Seek(value, SeekOrigin.Begin);
            }
        }

        private class PieceDirectory
        {
            private FileAccess _fileAccess;
            private FileMode _fileMode;
            private int _indexOfLastPieceStreamInfoAccessed;
            private int _lastPieceIndex;
            private bool _logicalEndPrecedesPhysicalEnd;
            private List<PieceStreamInfo> _pieceStreamInfoList;
            private List<PieceInfo> _sortedPieceInfoList;
            private Stream _temporaryMemoryStream = new MemoryStream(0);
            private ZipArchive _zipArchive;

            internal PieceDirectory(List<PieceInfo> sortedPieceInfoList, FileMode mode, FileAccess access)
            {
                if (sortedPieceInfoList == null)
                {
                    throw new ArgumentNullException("pieceDescriptors");
                }
                Invariant.Assert(sortedPieceInfoList.Count > 0);
                this._pieceStreamInfoList = new List<PieceStreamInfo>(sortedPieceInfoList.Count);
                this._pieceStreamInfoList.Add(new PieceStreamInfo(sortedPieceInfoList[0].ZipFileInfo.GetStream(mode, access), 0L));
                this._indexOfLastPieceStreamInfoAccessed = 0;
                this._lastPieceIndex = sortedPieceInfoList.Count - 1;
                this._fileMode = mode;
                this._fileAccess = access;
                this._sortedPieceInfoList = sortedPieceInfoList;
                this._zipArchive = sortedPieceInfoList[0].ZipFileInfo.ZipArchive;
            }

            internal void Close()
            {
                this.UpdatePhysicalEndIfNecessary();
                for (int i = 0; i <= this._indexOfLastPieceStreamInfoAccessed; i++)
                {
                    this._pieceStreamInfoList[i].Stream.Close();
                }
            }

            internal void Flush()
            {
                this.UpdatePhysicalEndIfNecessary();
                for (int i = 0; i <= this._indexOfLastPieceStreamInfoAccessed; i++)
                {
                    this._pieceStreamInfoList[i].Stream.Flush();
                }
            }

            internal int GetNumberOfPieces() => 
                (this._lastPieceIndex + 1);

            internal int GetPieceNumberFromOffset(long offset)
            {
                PieceStreamInfo item = new PieceStreamInfo(this._temporaryMemoryStream, offset);
                int pieceNumber = this._pieceStreamInfoList.BinarySearch(item);
                if (pieceNumber < 0)
                {
                    pieceNumber = ~pieceNumber - 1;
                    if (pieceNumber >= this._indexOfLastPieceStreamInfoAccessed)
                    {
                        while (pieceNumber < this._lastPieceIndex)
                        {
                            PieceStreamInfo info2 = this.RetrievePiece(pieceNumber);
                            if (offset < (info2.StartOffset + info2.Stream.Length))
                            {
                                break;
                            }
                            pieceNumber++;
                        }
                        Invariant.Assert(pieceNumber <= this._lastPieceIndex, "We should have found the valid pieceNumber earlier");
                        return pieceNumber;
                    }
                }
                return pieceNumber;
            }

            internal long GetStartOffset(int pieceNumber) => 
                this.RetrievePiece(pieceNumber).StartOffset;

            internal Stream GetStream(int pieceNumber) => 
                this.RetrievePiece(pieceNumber).Stream;

            internal bool IsLastPiece(int pieceNumber) => 
                (this._lastPieceIndex == pieceNumber);

            private PieceStreamInfo RetrievePiece(int pieceNumber)
            {
                if (pieceNumber > this._lastPieceIndex)
                {
                    throw new ArgumentException(System.Windows.SR.Get("PieceDoesNotExist"));
                }
                if (this._indexOfLastPieceStreamInfoAccessed < pieceNumber)
                {
                    PieceStreamInfo item = this._pieceStreamInfoList[this._indexOfLastPieceStreamInfoAccessed];
                    for (int i = this._indexOfLastPieceStreamInfoAccessed + 1; i <= pieceNumber; i++)
                    {
                        long pieceStart = item.StartOffset + item.Stream.Length;
                        Stream stream = this._sortedPieceInfoList[pieceNumber].ZipFileInfo.GetStream(this._fileMode, this._fileAccess);
                        this._indexOfLastPieceStreamInfoAccessed = i;
                        item = new PieceStreamInfo(stream, pieceStart);
                        this._pieceStreamInfoList.Add(item);
                    }
                }
                return this._pieceStreamInfoList[pieceNumber];
            }

            internal void SetLogicalLastPiece(int pieceNumber)
            {
                Invariant.Assert(pieceNumber <= this._lastPieceIndex);
                this.RetrievePiece(pieceNumber);
                if (this._lastPieceIndex > pieceNumber)
                {
                    this._logicalEndPrecedesPhysicalEnd = true;
                    this._lastPieceIndex = pieceNumber;
                    this._indexOfLastPieceStreamInfoAccessed = this._lastPieceIndex;
                    this._pieceStreamInfoList.RemoveRange(this._indexOfLastPieceStreamInfoAccessed + 1, this._pieceStreamInfoList.Count - (this._indexOfLastPieceStreamInfoAccessed + 1));
                }
            }

            private void UpdatePhysicalEndIfNecessary()
            {
                if (this._logicalEndPrecedesPhysicalEnd)
                {
                    for (int i = this._lastPieceIndex + 1; i < this._sortedPieceInfoList.Count; i++)
                    {
                        this._zipArchive.DeleteFile(this._sortedPieceInfoList[i].ZipFileInfo.Name);
                    }
                    this._sortedPieceInfoList.RemoveRange(this._lastPieceIndex + 1, this._sortedPieceInfoList.Count - (this._lastPieceIndex + 1));
                    int pieceNumber = this._lastPieceIndex + 1;
                    ZipFileInfo zipFileInfo = this._sortedPieceInfoList[0].ZipFileInfo;
                    CompressionMethodEnum compressionMethod = zipFileInfo.CompressionMethod;
                    DeflateOptionEnum deflateOption = zipFileInfo.DeflateOption;
                    if ((this._lastPieceIndex == 0) && (this._pieceStreamInfoList[0].Stream.Length == 0L))
                    {
                        this._zipArchive.DeleteFile(zipFileInfo.Name);
                        this._indexOfLastPieceStreamInfoAccessed = -1;
                        this._pieceStreamInfoList.Clear();
                        pieceNumber = 0;
                    }
                    string zipFileName = PieceNameHelper.CreatePieceName(this._sortedPieceInfoList[0].PrefixName, pieceNumber, true);
                    ZipFileInfo info2 = this._zipArchive.AddFile(zipFileName, compressionMethod, deflateOption);
                    this._lastPieceIndex = pieceNumber;
                    this._sortedPieceInfoList.Add(new PieceInfo(info2, this._sortedPieceInfoList[0].PartUri, this._sortedPieceInfoList[0].PrefixName, this._lastPieceIndex, true));
                    if (pieceNumber == 0)
                    {
                        Stream stream = info2.GetStream(this._fileMode, this._fileAccess);
                        this._indexOfLastPieceStreamInfoAccessed = 0;
                        Invariant.Assert(this._pieceStreamInfoList.Count == 0);
                        this._pieceStreamInfoList.Add(new PieceStreamInfo(stream, 0L));
                    }
                    this._logicalEndPrecedesPhysicalEnd = false;
                }
            }

            private sealed class PieceStreamInfo : IComparable<InterleavedZipPartStream.PieceDirectory.PieceStreamInfo>
            {
                private long _startOffset;
                private System.IO.Stream _stream;

                internal PieceStreamInfo(System.IO.Stream stream, long pieceStart)
                {
                    Invariant.Assert(stream != null);
                    Invariant.Assert(pieceStart >= 0L);
                    this._stream = stream;
                    this._startOffset = pieceStart;
                }

                private int Compare(InterleavedZipPartStream.PieceDirectory.PieceStreamInfo pieceStreamInfo)
                {
                    if (pieceStreamInfo != null)
                    {
                        if (this._startOffset == pieceStreamInfo.StartOffset)
                        {
                            return 0;
                        }
                        if (this._startOffset < pieceStreamInfo.StartOffset)
                        {
                            return -1;
                        }
                    }
                    return 1;
                }

                int IComparable<InterleavedZipPartStream.PieceDirectory.PieceStreamInfo>.CompareTo(InterleavedZipPartStream.PieceDirectory.PieceStreamInfo pieceStreamInfo) => 
                    this.Compare(pieceStreamInfo);

                internal long StartOffset =>
                    this._startOffset;

                internal System.IO.Stream Stream =>
                    this._stream;
            }
        }
    }
}

