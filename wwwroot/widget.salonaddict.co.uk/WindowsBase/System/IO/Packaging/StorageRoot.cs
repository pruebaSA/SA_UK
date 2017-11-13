namespace System.IO.Packaging
{
    using MS.Internal;
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Windows;

    internal class StorageRoot : StorageInfo
    {
        private bool containerIsReadOnly;
        private DataSpaceManager dataSpaceManager;
        private bool dataSpaceManagerInitializationInProgress;
        private const FileAccess defaultFileAccess = FileAccess.ReadWrite;
        private const FileMode defaultFileMode = FileMode.OpenOrCreate;
        private const FileShare defaultFileShare = FileShare.None;
        private const int defaultSectorSize = 0x200;
        private IStorage rootIStorage;
        private const int stgFormatDocFile = 5;

        private StorageRoot(IStorage root, bool readOnly) : base(root)
        {
            this.rootIStorage = root;
            this.containerIsReadOnly = readOnly;
            this.dataSpaceManagerInitializationInProgress = false;
        }

        internal void CheckRootDisposedStatus()
        {
            if (this.RootDisposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("StorageRootDisposed"));
            }
        }

        internal void Close()
        {
            if (this.rootIStorage != null)
            {
                if (this.dataSpaceManager != null)
                {
                    this.dataSpaceManager.Dispose();
                    this.dataSpaceManager = null;
                }
                try
                {
                    if (!this.containerIsReadOnly)
                    {
                        this.rootIStorage.Commit(0);
                    }
                }
                finally
                {
                    StorageInfo.RecursiveStorageInfoCoreRelease(base.core);
                    this.rootIStorage = null;
                }
            }
        }

        private static StorageRoot CreateOnIStorage(IStorage root)
        {
            System.Runtime.InteropServices.ComTypes.STATSTG statstg;
            Invariant.Assert(root != null);
            root.Stat(out statstg, 1);
            return new StorageRoot(root, (1 != (statstg.grfMode & 1)) && (2 != (statstg.grfMode & 2)));
        }

        internal static StorageRoot CreateOnStream(Stream baseStream)
        {
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }
            if (0L == baseStream.Length)
            {
                return CreateOnStream(baseStream, FileMode.Create);
            }
            return CreateOnStream(baseStream, FileMode.Open);
        }

        internal static StorageRoot CreateOnStream(Stream baseStream, FileMode mode)
        {
            IStorage storage;
            int num;
            if (baseStream == null)
            {
                throw new ArgumentNullException("baseStream");
            }
            int grfMode = 0x10;
            if (!baseStream.CanRead)
            {
                throw new ArgumentException(System.Windows.SR.Get("CanNotCreateStorageRootOnNonReadableStream"));
            }
            if (baseStream.CanWrite)
            {
                grfMode |= 2;
            }
            else if (FileMode.Create == mode)
            {
                throw new ArgumentException(System.Windows.SR.Get("CanNotCreateContainerOnReadOnlyStream"));
            }
            if (FileMode.Create == mode)
            {
                num = SafeNativeCompoundFileMethods.SafeStgCreateDocfileOnStream(baseStream, grfMode | 0x1000, out storage);
            }
            else
            {
                if (FileMode.Open != mode)
                {
                    throw new ArgumentException(System.Windows.SR.Get("CreateModeMustBeCreateOrOpen"));
                }
                num = SafeNativeCompoundFileMethods.SafeStgOpenStorageOnStream(baseStream, grfMode, out storage);
            }
            uint num3 = (uint) num;
            if (num3 != 0)
            {
                throw new IOException(System.Windows.SR.Get("UnableToCreateOnStream"), new COMException(System.Windows.SR.Get("CFAPIFailure"), num));
            }
            return CreateOnIStorage(storage);
        }

        internal void Flush()
        {
            this.CheckRootDisposedStatus();
            if (!this.containerIsReadOnly)
            {
                this.rootIStorage.Commit(0);
            }
        }

        internal DataSpaceManager GetDataSpaceManager()
        {
            this.CheckRootDisposedStatus();
            if (this.dataSpaceManager == null)
            {
                if (this.dataSpaceManagerInitializationInProgress)
                {
                    return null;
                }
                this.dataSpaceManagerInitializationInProgress = true;
                this.dataSpaceManager = new DataSpaceManager(this);
                this.dataSpaceManagerInitializationInProgress = false;
            }
            return this.dataSpaceManager;
        }

        internal IStorage GetRootIStorage() => 
            this.rootIStorage;

        internal static StorageRoot Open(string path) => 
            Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 0x200);

        internal static StorageRoot Open(string path, FileMode mode) => 
            Open(path, mode, FileAccess.ReadWrite, FileShare.None, 0x200);

        internal static StorageRoot Open(string path, FileMode mode, FileAccess access) => 
            Open(path, mode, access, FileShare.None, 0x200);

        internal static StorageRoot Open(string path, FileMode mode, FileAccess access, FileShare share) => 
            Open(path, mode, access, share, 0x200);

        internal static StorageRoot Open(string path, FileMode mode, FileAccess access, FileShare share, int sectorSize)
        {
            IStorage storage;
            int grfMode = 0;
            int errorCode = 0;
            ContainerUtilities.CheckStringAgainstNullAndEmpty(path, "Path");
            Guid riid = new Guid(11, 0, 0, 0xc0, 0, 0, 0, 0, 0, 0, 70);
            switch (mode)
            {
                case FileMode.CreateNew:
                {
                    FileInfo info = new FileInfo(path);
                    if (info.Exists)
                    {
                        throw new IOException(System.Windows.SR.Get("FileAlreadyExists"));
                    }
                    break;
                }
                case FileMode.Create:
                    break;

                case FileMode.Open:
                    goto Label_00C0;

                case FileMode.OpenOrCreate:
                {
                    FileInfo info2 = new FileInfo(path);
                    if (!info2.Exists)
                    {
                        break;
                    }
                    goto Label_00C0;
                }
                case FileMode.Truncate:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported"));

                case FileMode.Append:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeUnsupported"));

                default:
                    throw new ArgumentException(System.Windows.SR.Get("FileModeInvalid"));
            }
            grfMode |= 0x1000;
        Label_00C0:
            SafeNativeCompoundFileMethods.UpdateModeFlagFromFileAccess(access, ref grfMode);
            if ((share & FileShare.Inheritable) != FileShare.None)
            {
                throw new ArgumentException(System.Windows.SR.Get("FileShareUnsupported"));
            }
            if (share == FileShare.None)
            {
                grfMode |= 0x10;
            }
            else if (share == FileShare.Read)
            {
                grfMode |= 0x20;
            }
            else if (share == FileShare.Write)
            {
                grfMode |= 0x30;
            }
            else
            {
                if (share != FileShare.ReadWrite)
                {
                    throw new ArgumentException(System.Windows.SR.Get("FileShareInvalid"));
                }
                grfMode |= 0x40;
            }
            if ((grfMode & 0x1000) != 0)
            {
                errorCode = SafeNativeCompoundFileMethods.SafeStgCreateStorageEx(path, grfMode, 5, 0, IntPtr.Zero, IntPtr.Zero, ref riid, out storage);
            }
            else
            {
                errorCode = SafeNativeCompoundFileMethods.SafeStgOpenStorageEx(path, grfMode, 5, 0, IntPtr.Zero, IntPtr.Zero, ref riid, out storage);
            }
            switch (errorCode)
            {
                case -2147287038:
                    throw new FileNotFoundException(System.Windows.SR.Get("ContainerNotFound"));

                case -2147286785:
                    throw new ArgumentException(System.Windows.SR.Get("StorageFlagsUnsupported"), new COMException(System.Windows.SR.Get("CFAPIFailure"), errorCode));

                case 0:
                    return CreateOnIStorage(storage);
            }
            throw new IOException(System.Windows.SR.Get("ContainerCanNotOpen"), new COMException(System.Windows.SR.Get("CFAPIFailure"), errorCode));
        }

        internal FileAccess OpenAccess
        {
            get
            {
                this.CheckRootDisposedStatus();
                if (this.containerIsReadOnly)
                {
                    return FileAccess.Read;
                }
                return FileAccess.ReadWrite;
            }
        }

        internal bool RootDisposed =>
            (null == this.rootIStorage);
    }
}

