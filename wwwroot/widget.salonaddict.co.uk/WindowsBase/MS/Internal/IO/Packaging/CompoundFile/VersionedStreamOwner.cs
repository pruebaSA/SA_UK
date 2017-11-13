namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Windows;

    internal class VersionedStreamOwner : VersionedStream
    {
        private FormatVersion _codeVersion;
        private long _dataOffset;
        private FormatVersion _fileVersion;
        private bool _readOccurred;
        private bool _writeOccurred;

        internal VersionedStreamOwner(Stream baseStream, FormatVersion codeVersion) : base(baseStream)
        {
            this._codeVersion = codeVersion;
        }

        private void EnsureParsed()
        {
            if ((this._fileVersion == null) && (base.BaseStream.Length > 0L))
            {
                if (!base.BaseStream.CanRead)
                {
                    throw new NotSupportedException(System.Windows.SR.Get("ReadNotSupported"));
                }
                base.BaseStream.Seek(0L, SeekOrigin.Begin);
                this._fileVersion = FormatVersion.LoadFromStream(base.BaseStream);
                if (string.CompareOrdinal(this._fileVersion.FeatureIdentifier.ToUpper(CultureInfo.InvariantCulture), this._codeVersion.FeatureIdentifier.ToUpper(CultureInfo.InvariantCulture)) != 0)
                {
                    throw new FileFormatException(System.Windows.SR.Get("InvalidTransformFeatureName", new object[] { this._fileVersion.FeatureIdentifier, this._codeVersion.FeatureIdentifier }));
                }
                this._dataOffset = base.BaseStream.Position;
            }
        }

        public override void Flush()
        {
            base.CheckDisposed();
            base.BaseStream.Flush();
        }

        private void PersistVersion(FormatVersion version)
        {
            if (!base.BaseStream.CanWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("WriteNotSupported"));
            }
            long num = base.BaseStream.Position - this._dataOffset;
            base.BaseStream.Seek(0L, SeekOrigin.Begin);
            long num2 = version.SaveToStream(base.BaseStream);
            this._fileVersion = version;
            if ((this._dataOffset != 0L) && (num2 != this._dataOffset))
            {
                throw new FileFormatException(System.Windows.SR.Get("VersionUpdateFailure"));
            }
            this._dataOffset = num2;
            base.BaseStream.Position = num + this._dataOffset;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            this.ReadAttempt(true);
            return base.BaseStream.Read(buffer, offset, count);
        }

        internal void ReadAttempt()
        {
            this.ReadAttempt(false);
        }

        internal void ReadAttempt(bool throwIfEmpty)
        {
            base.CheckDisposed();
            if (!this._readOccurred)
            {
                this.EnsureParsed();
                if (throwIfEmpty || (base.BaseStream.Length > 0L))
                {
                    if (this._fileVersion == null)
                    {
                        throw new FileFormatException(System.Windows.SR.Get("VersionStreamMissing"));
                    }
                    if (!this._fileVersion.IsReadableBy(this._codeVersion.ReaderVersion))
                    {
                        throw new FileFormatException(System.Windows.SR.Get("ReaderVersionError", new object[] { this._fileVersion.ReaderVersion, this._codeVersion }));
                    }
                }
                this._readOccurred = true;
            }
        }

        public override int ReadByte()
        {
            this.ReadAttempt(true);
            return base.BaseStream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            this.ReadAttempt();
            long num = -1L;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    num = offset;
                    break;

                case SeekOrigin.Current:
                    num = this.Position + offset;
                    break;

                case SeekOrigin.End:
                    num = this.Length + offset;
                    break;
            }
            if (num < 0L)
            {
                throw new ArgumentException(System.Windows.SR.Get("SeekNegative"));
            }
            base.BaseStream.Position = num + this._dataOffset;
            return num;
        }

        public override void SetLength(long newLength)
        {
            if (newLength < 0L)
            {
                throw new ArgumentOutOfRangeException("newLength");
            }
            this.WriteAttempt();
            base.BaseStream.SetLength(newLength + this._dataOffset);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.WriteAttempt();
            base.BaseStream.Write(buffer, offset, count);
        }

        internal void WriteAttempt()
        {
            base.CheckDisposed();
            if (!this._writeOccurred)
            {
                this.EnsureParsed();
                if (this._fileVersion == null)
                {
                    this.PersistVersion(this._codeVersion);
                }
                else
                {
                    if (!this._fileVersion.IsUpdatableBy(this._codeVersion.UpdaterVersion))
                    {
                        throw new FileFormatException(System.Windows.SR.Get("UpdaterVersionError", new object[] { this._fileVersion.UpdaterVersion, this._codeVersion }));
                    }
                    if (this._codeVersion.UpdaterVersion != this._fileVersion.UpdaterVersion)
                    {
                        this._fileVersion.UpdaterVersion = this._codeVersion.UpdaterVersion;
                        this.PersistVersion(this._fileVersion);
                    }
                }
                this._writeOccurred = true;
            }
        }

        public override void WriteByte(byte b)
        {
            this.WriteAttempt();
            base.BaseStream.WriteByte(b);
        }

        public override bool CanRead =>
            (((base.BaseStream != null) && base.BaseStream.CanRead) && this.IsReadable);

        public override bool CanSeek =>
            (((base.BaseStream != null) && base.BaseStream.CanSeek) && this.IsReadable);

        public override bool CanWrite =>
            (((base.BaseStream != null) && base.BaseStream.CanWrite) && this.IsUpdatable);

        internal bool IsReadable
        {
            get
            {
                base.CheckDisposed();
                this.EnsureParsed();
                if (this._fileVersion != null)
                {
                    return this._fileVersion.IsReadableBy(this._codeVersion.ReaderVersion);
                }
                return true;
            }
        }

        internal bool IsUpdatable
        {
            get
            {
                base.CheckDisposed();
                this.EnsureParsed();
                if (this._fileVersion != null)
                {
                    return this._fileVersion.IsUpdatableBy(this._codeVersion.UpdaterVersion);
                }
                return true;
            }
        }

        public override long Length
        {
            get
            {
                this.ReadAttempt();
                long num = base.BaseStream.Length - this._dataOffset;
                Invariant.Assert(num >= 0L);
                return num;
            }
        }

        public override long Position
        {
            get
            {
                this.ReadAttempt();
                return (base.BaseStream.Position - this._dataOffset);
            }
            set
            {
                this.Seek(value, SeekOrigin.Begin);
            }
        }
    }
}

