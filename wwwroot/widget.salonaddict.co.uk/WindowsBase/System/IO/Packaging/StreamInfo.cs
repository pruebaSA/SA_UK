namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows;

    public class StreamInfo
    {
        private System.IO.Packaging.CompressionOption _compressionOption;
        private System.IO.Packaging.EncryptionOption _encryptionOption;
        private bool _needToGetTransformInfo;
        private CompoundFileStreamReference _streamReference;
        private StreamInfoCore core;
        private const string defaultDataSpace = null;
        private const FileMode defaultFileCreateMode = FileMode.Create;
        private const FileMode defaultFileOpenMode = FileMode.OpenOrCreate;
        private FileAccess openFileAccess;
        private StorageInfo parentStorage;

        internal StreamInfo(StorageInfo parent, string streamName) : this(parent, streamName, System.IO.Packaging.CompressionOption.NotCompressed, System.IO.Packaging.EncryptionOption.None)
        {
        }

        private StreamInfo(StorageRoot root, string streamPath) : this((StorageInfo) root, streamPath)
        {
        }

        internal StreamInfo(StorageInfo parent, string streamName, System.IO.Packaging.CompressionOption compressionOption, System.IO.Packaging.EncryptionOption encryptionOption)
        {
            this._needToGetTransformInfo = true;
            ContainerUtilities.CheckAgainstNull(parent, "parent");
            ContainerUtilities.CheckStringAgainstNullAndEmpty(streamName, "streamName");
            this.BuildStreamInfoRelativeToStorage(parent, streamName);
            this._compressionOption = compressionOption;
            this._encryptionOption = encryptionOption;
            this._streamReference = new CompoundFileStreamReference(this.parentStorage.FullNameInternal, this.core.streamName);
        }

        private void BuildStreamInfoRelativeToStorage(StorageInfo parent, string path)
        {
            this.parentStorage = parent;
            this.core = this.parentStorage.CoreForChildStream(path);
        }

        private Stream BuildStreamOnUnderlyingIStream(MS.Internal.IO.Packaging.CompoundFile.IStream underlyingIStream, FileAccess access, StreamInfo parent)
        {
            Stream stream = new CFStream(underlyingIStream, access, parent);
            if (this.core.dataSpaceLabel == null)
            {
                return new BufferedStream(stream);
            }
            return this.parentStorage.Root.GetDataSpaceManager().CreateDataSpaceStream(this.StreamReference, stream);
        }

        private Stream CFStreamOfClone(FileAccess access)
        {
            long plibNewPosition = 0L;
            MS.Internal.IO.Packaging.CompoundFile.IStream ppstm = null;
            this.core.safeIStream.Clone(out ppstm);
            ppstm.Seek(0L, 0, out plibNewPosition);
            Stream stream2 = this.BuildStreamOnUnderlyingIStream(ppstm, access, this);
            this.core.exposedStream = stream2;
            return stream2;
        }

        internal void CheckAccessMode(int grfMode)
        {
            if ((this.core.safeIStream != null) && (this.core.exposedStream == null))
            {
                System.Runtime.InteropServices.ComTypes.STATSTG statstg;
                this.core.safeIStream.Stat(out statstg, 1);
                if (grfMode != statstg.grfMode)
                {
                    ((IDisposable) this.core.safeIStream).Dispose();
                    this.core.safeIStream = null;
                }
            }
        }

        internal void CheckDisposedStatus()
        {
            if (this.StreamInfoDisposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamInfoDisposed"));
            }
        }

        internal Stream Create() => 
            this.Create(FileMode.Create, this.parentStorage.Root.OpenAccess, null);

        private Stream Create(FileMode mode) => 
            this.Create(mode, this.parentStorage.Root.OpenAccess, null);

        internal Stream Create(string dataSpaceLabel) => 
            this.Create(FileMode.Create, this.parentStorage.Root.OpenAccess, dataSpaceLabel);

        private Stream Create(FileMode mode, FileAccess access) => 
            this.Create(mode, access, null);

        internal Stream Create(FileMode mode, FileAccess access, string dataSpace)
        {
            this.CheckDisposedStatus();
            int grfMode = 0;
            MS.Internal.IO.Packaging.CompoundFile.IStream stream = null;
            DataSpaceManager dataSpaceManager = null;
            this.CreateTimeReadOnlyCheck(access);
            if (dataSpace != null)
            {
                if (dataSpace.Length == 0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("DataSpaceLabelInvalidEmpty"));
                }
                dataSpaceManager = this.parentStorage.Root.GetDataSpaceManager();
                if (!dataSpaceManager.DataSpaceIsDefined(dataSpace))
                {
                    throw new ArgumentException(System.Windows.SR.Get("DataSpaceLabelUndefined"));
                }
            }
            this.openFileAccess = access;
            if (this.parentStorage.Root.OpenAccess == FileAccess.ReadWrite)
            {
                access = FileAccess.ReadWrite;
            }
            SafeNativeCompoundFileMethods.UpdateModeFlagFromFileAccess(access, ref grfMode);
            grfMode |= 0x10;
            this.CheckAccessMode(grfMode);
            switch (mode)
            {
                case FileMode.CreateNew:
                    if (this.core.safeIStream != null)
                    {
                        throw new IOException(System.Windows.SR.Get("StreamAlreadyExist"));
                    }
                    stream = this.CreateStreamOnParentIStorage(this.core.streamName, grfMode);
                    break;

                case FileMode.Create:
                    if (this.core.exposedStream != null)
                    {
                        ((Stream) this.core.exposedStream).Close();
                    }
                    this.core.exposedStream = null;
                    if (this.core.safeIStream != null)
                    {
                        ((IDisposable) this.core.safeIStream).Dispose();
                        this.core.safeIStream = null;
                    }
                    grfMode |= 0x1000;
                    stream = this.CreateStreamOnParentIStorage(this.core.streamName, grfMode);
                    break;

                default:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeInvalid"));
            }
            this.core.safeIStream = stream;
            this.core.dataSpaceLabel = dataSpace;
            if (dataSpace != null)
            {
                dataSpaceManager.CreateDataSpaceMapping(new CompoundFileStreamReference(this.parentStorage.FullNameInternal, this.core.streamName), this.core.dataSpaceLabel);
            }
            Stream stream2 = this.BuildStreamOnUnderlyingIStream(this.core.safeIStream, this.openFileAccess, this);
            this._needToGetTransformInfo = false;
            this.core.exposedStream = stream2;
            return stream2;
        }

        private MS.Internal.IO.Packaging.CompoundFile.IStream CreateStreamOnParentIStorage(string name, int mode)
        {
            MS.Internal.IO.Packaging.CompoundFile.IStream ppstm = null;
            int errorCode = 0;
            if (!this.parentStorage.Exists)
            {
                this.parentStorage.Create();
            }
            errorCode = this.parentStorage.SafeIStorage.CreateStream(name, mode, 0, 0, out ppstm);
            if (-2147286785 == errorCode)
            {
                throw new ArgumentException(System.Windows.SR.Get("StorageFlagsUnsupported"));
            }
            if (errorCode != 0)
            {
                throw new IOException(System.Windows.SR.Get("UnableToCreateStream"), new COMException(System.Windows.SR.Get("NamedAPIFailure", new object[] { "IStorage.CreateStream" }), errorCode));
            }
            this.parentStorage.InvalidateEnumerators();
            return ppstm;
        }

        private void CreateTimeReadOnlyCheck(FileAccess access)
        {
            if (FileAccess.Read == this.parentStorage.Root.OpenAccess)
            {
                throw new IOException(System.Windows.SR.Get("CanNotCreateInReadOnly"));
            }
            if (access == FileAccess.Read)
            {
                throw new ArgumentException(System.Windows.SR.Get("CanNotCreateAsReadOnly"));
            }
        }

        internal void Delete()
        {
            this.CheckDisposedStatus();
            if (this.InternalExists())
            {
                if (this.core.safeIStream != null)
                {
                    ((IDisposable) this.core.safeIStream).Dispose();
                    this.core.safeIStream = null;
                }
                this.parentStorage.DestroyElement(this.core.streamName);
                this.parentStorage.InvalidateEnumerators();
            }
        }

        private void EnsureTransformInformation()
        {
            if (this._needToGetTransformInfo && this.InternalExists())
            {
                this._encryptionOption = System.IO.Packaging.EncryptionOption.None;
                this._compressionOption = System.IO.Packaging.CompressionOption.NotCompressed;
                foreach (IDataTransform transform in this.parentStorage.Root.GetDataSpaceManager().GetTransformsForStreamInfo(this))
                {
                    string transformIdentifier = transform.TransformIdentifier as string;
                    if (transformIdentifier != null)
                    {
                        transformIdentifier = transformIdentifier.ToUpperInvariant();
                        if ((string.CompareOrdinal(transformIdentifier, RightsManagementEncryptionTransform.ClassTransformIdentifier.ToUpperInvariant()) == 0) && (transform is RightsManagementEncryptionTransform))
                        {
                            this._encryptionOption = System.IO.Packaging.EncryptionOption.RightsManagement;
                        }
                        else if ((string.CompareOrdinal(transformIdentifier, CompressionTransform.ClassTransformIdentifier.ToUpperInvariant()) == 0) && (transform is CompressionTransform))
                        {
                            this._compressionOption = System.IO.Packaging.CompressionOption.Maximum;
                        }
                    }
                }
                this._needToGetTransformInfo = false;
            }
        }

        public Stream GetStream() => 
            this.GetStream(FileMode.OpenOrCreate, this.parentStorage.Root.OpenAccess);

        public Stream GetStream(FileMode mode) => 
            this.GetStream(mode, this.parentStorage.Root.OpenAccess);

        public Stream GetStream(FileMode mode, FileAccess access)
        {
            this.CheckDisposedStatus();
            int grfMode = 0;
            MS.Internal.IO.Packaging.CompoundFile.IStream ppstm = null;
            this.openFileAccess = access;
            if (this.parentStorage.Root.OpenAccess == FileAccess.ReadWrite)
            {
                access = FileAccess.ReadWrite;
            }
            SafeNativeCompoundFileMethods.UpdateModeFlagFromFileAccess(access, ref grfMode);
            grfMode |= 0x10;
            this.CheckAccessMode(grfMode);
            switch (mode)
            {
                case FileMode.CreateNew:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported"));

                case FileMode.Create:
                    this.CreateTimeReadOnlyCheck(this.openFileAccess);
                    if (this.core.exposedStream != null)
                    {
                        ((Stream) this.core.exposedStream).Close();
                    }
                    this.core.exposedStream = null;
                    if (this.core.safeIStream != null)
                    {
                        ((IDisposable) this.core.safeIStream).Dispose();
                        this.core.safeIStream = null;
                    }
                    grfMode |= 0x1000;
                    ppstm = this.CreateStreamOnParentIStorage(this.core.streamName, grfMode);
                    break;

                case FileMode.Open:
                    if (this.core.safeIStream != null)
                    {
                        return this.CFStreamOfClone(this.openFileAccess);
                    }
                    ppstm = this.OpenStreamOnParentIStorage(this.core.streamName, grfMode);
                    break;

                case FileMode.OpenOrCreate:
                    if (this.core.safeIStream == null)
                    {
                        if ((FileAccess.Read != this.parentStorage.Root.OpenAccess) && (FileAccess.Read != this.openFileAccess))
                        {
                            if (!this.parentStorage.Exists)
                            {
                                this.parentStorage.Create();
                            }
                            int errorCode = this.parentStorage.SafeIStorage.CreateStream(this.core.streamName, grfMode, 0, 0, out ppstm);
                            if ((errorCode != 0) && (-2147286960 != errorCode))
                            {
                                throw new IOException(System.Windows.SR.Get("UnableToCreateStream"), new COMException(System.Windows.SR.Get("NamedAPIFailure", new object[] { "IStorage.CreateStream" }), errorCode));
                            }
                            this.parentStorage.InvalidateEnumerators();
                        }
                        if (ppstm == null)
                        {
                            ppstm = this.OpenStreamOnParentIStorage(this.core.streamName, grfMode);
                        }
                        break;
                    }
                    return this.CFStreamOfClone(this.openFileAccess);

                case FileMode.Truncate:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported"));

                case FileMode.Append:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported"));

                default:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeInvalid"));
            }
            this.core.safeIStream = ppstm;
            Stream stream2 = this.BuildStreamOnUnderlyingIStream(this.core.safeIStream, this.openFileAccess, this);
            this.core.exposedStream = stream2;
            return stream2;
        }

        internal bool InternalExists()
        {
            if (this.core.safeIStream != null)
            {
                return true;
            }
            if (!this.parentStorage.Exists)
            {
                return false;
            }
            return (0 == this.parentStorage.SafeIStorage.OpenStream(this.core.streamName, 0, 0x10, 0, out this.core.safeIStream));
        }

        private MS.Internal.IO.Packaging.CompoundFile.IStream OpenStreamOnParentIStorage(string name, int mode)
        {
            MS.Internal.IO.Packaging.CompoundFile.IStream ppstm = null;
            int errorCode = 0;
            errorCode = this.parentStorage.SafeIStorage.OpenStream(name, 0, mode, 0, out ppstm);
            if (errorCode != 0)
            {
                throw new IOException(System.Windows.SR.Get("UnableToOpenStream"), new COMException(System.Windows.SR.Get("NamedAPIFailure", new object[] { "IStorage.OpenStream" }), errorCode));
            }
            return ppstm;
        }

        private void VerifyExists()
        {
            if (!this.InternalExists())
            {
                throw new IOException(System.Windows.SR.Get("StreamNotExist"));
            }
        }

        public System.IO.Packaging.CompressionOption CompressionOption
        {
            get
            {
                if (this.StreamInfoDisposed)
                {
                    return System.IO.Packaging.CompressionOption.NotCompressed;
                }
                this.EnsureTransformInformation();
                return this._compressionOption;
            }
        }

        public System.IO.Packaging.EncryptionOption EncryptionOption
        {
            get
            {
                if (this.StreamInfoDisposed)
                {
                    return System.IO.Packaging.EncryptionOption.None;
                }
                this.EnsureTransformInformation();
                return this._encryptionOption;
            }
        }

        public string Name
        {
            get
            {
                if (this.StreamInfoDisposed)
                {
                    return "";
                }
                return this.core.streamName;
            }
        }

        internal bool StreamInfoDisposed
        {
            get
            {
                if (this.core.streamName != null)
                {
                    return this.parentStorage.StorageDisposed;
                }
                return true;
            }
        }

        internal CompoundFileStreamReference StreamReference =>
            this._streamReference;
    }
}

