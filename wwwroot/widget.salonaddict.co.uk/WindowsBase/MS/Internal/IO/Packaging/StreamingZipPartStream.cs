namespace MS.Internal.IO.Packaging
{
    using MS.Internal.IO.Zip;
    using System;
    using System.IO;
    using System.IO.Packaging;
    using System.Windows;

    internal class StreamingZipPartStream : Stream
    {
        private FileAccess _access;
        private ZipArchive _archive;
        private bool _closed;
        private CompressionMethodEnum _compressionMethod;
        private int _currentPieceNumber;
        private DeflateOptionEnum _deflateOption;
        private FileMode _mode;
        private string _partName;
        private Stream _pieceStream;

        internal StreamingZipPartStream(string partName, ZipArchive zipArchive, CompressionMethodEnum compressionMethod, DeflateOptionEnum deflateOption, FileMode mode, FileAccess access)
        {
            if (((mode != FileMode.Create) && (mode != FileMode.CreateNew)) || (access != FileAccess.Write))
            {
                throw new NotSupportedException(System.Windows.SR.Get("OnlyStreamingProductionIsSupported"));
            }
            this._partName = partName;
            this._archive = zipArchive;
            this._compressionMethod = compressionMethod;
            this._deflateOption = deflateOption;
            this._mode = mode;
            this._access = access;
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
                    this.Flush();
                    this.EnsurePieceStream(true);
                    this._pieceStream.Close();
                }
            }
            finally
            {
                this._closed = true;
                base.Dispose(disposing);
            }
        }

        private void EnsurePieceStream(bool isLastPiece)
        {
            if (this._pieceStream != null)
            {
                if (isLastPiece)
                {
                    this._pieceStream.Close();
                }
                if (!this._pieceStream.CanWrite)
                {
                    this._currentPieceNumber++;
                    this._pieceStream = null;
                }
            }
            if (this._pieceStream == null)
            {
                string zipItemNameFromOpcName = ZipPackage.GetZipItemNameFromOpcName(PieceNameHelper.CreatePieceName(this._partName, this._currentPieceNumber, isLastPiece));
                this._pieceStream = this._archive.AddFile(zipItemNameFromOpcName, this._compressionMethod, this._deflateOption).GetStream(FileMode.Create, this._access);
            }
        }

        public override void Flush()
        {
            this.CheckClosed();
            if ((this._pieceStream != null) && this._pieceStream.CanWrite)
            {
                this._pieceStream.Flush();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException(System.Windows.SR.Get("OnlyWriteOperationsAreSupportedInStreamingCreation"));
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException(System.Windows.SR.Get("OnlyWriteOperationsAreSupportedInStreamingCreation"));
        }

        public override void SetLength(long newLength)
        {
            throw new InvalidOperationException(System.Windows.SR.Get("OperationViolatesWriteOnceSemantics", new object[] { "SetLength" }));
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.CheckClosed();
            this.EnsurePieceStream(false);
            this._pieceStream.Write(buffer, offset, count);
        }

        public override bool CanRead =>
            false;

        public override bool CanSeek =>
            false;

        public override bool CanWrite =>
            !this._closed;

        public override long Length =>
            -1L;

        public override long Position
        {
            get => 
                -1L;
            set
            {
                throw new InvalidOperationException(System.Windows.SR.Get("OperationViolatesWriteOnceSemantics", new object[] { "set_Position" }));
            }
        }
    }
}

