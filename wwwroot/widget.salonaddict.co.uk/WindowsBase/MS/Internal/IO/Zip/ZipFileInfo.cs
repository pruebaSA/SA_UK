namespace MS.Internal.IO.Zip
{
    using System;
    using System.IO;

    internal sealed class ZipFileInfo
    {
        private ZipIOLocalFileBlock _fileBlock;
        private MS.Internal.IO.Zip.ZipArchive _zipArchive;

        internal ZipFileInfo(MS.Internal.IO.Zip.ZipArchive zipArchive, ZipIOLocalFileBlock fileBlock)
        {
            this._fileBlock = fileBlock;
            this._zipArchive = zipArchive;
        }

        private void CheckDisposed()
        {
            this._fileBlock.CheckDisposed();
        }

        internal Stream GetStream(FileMode mode, FileAccess access)
        {
            this.CheckDisposed();
            return this._fileBlock.GetStream(mode, access);
        }

        internal CompressionMethodEnum CompressionMethod
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.CompressionMethod;
            }
        }

        internal DeflateOptionEnum DeflateOption
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.DeflateOption;
            }
        }

        internal bool FolderFlag
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.FolderFlag;
            }
        }

        internal DateTime LastModFileDateTime
        {
            get
            {
                this.CheckDisposed();
                return ZipIOBlockManager.FromMsDosDateTime(this._fileBlock.LastModFileDateTime);
            }
        }

        internal ZipIOLocalFileBlock LocalFileBlock =>
            this._fileBlock;

        internal string Name
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.FileName;
            }
        }

        internal bool VolumeLabelFlag
        {
            get
            {
                this.CheckDisposed();
                return this._fileBlock.VolumeLabelFlag;
            }
        }

        internal MS.Internal.IO.Zip.ZipArchive ZipArchive
        {
            get
            {
                this.CheckDisposed();
                return this._zipArchive;
            }
        }
    }
}

