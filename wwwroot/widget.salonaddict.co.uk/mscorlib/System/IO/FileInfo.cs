namespace System.IO
{
    using Microsoft.Win32;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Text;

    [Serializable, ComVisible(true)]
    public sealed class FileInfo : FileSystemInfo
    {
        private string _name;

        public FileInfo(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }
            base.OriginalPath = fileName;
            string fullPathInternal = Path.GetFullPathInternal(fileName);
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            this._name = Path.GetFileName(fileName);
            base.FullPath = fullPathInternal;
        }

        private FileInfo(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { base.FullPath }, false, false).Demand();
            this._name = Path.GetFileName(base.OriginalPath);
        }

        internal FileInfo(string fullPath, bool ignoreThis)
        {
            this._name = Path.GetFileName(fullPath);
            base.OriginalPath = this._name;
            base.FullPath = fullPath;
        }

        public StreamWriter AppendText() => 
            new StreamWriter(base.FullPath, true);

        public FileInfo CopyTo(string destFileName) => 
            this.CopyTo(destFileName, false);

        public FileInfo CopyTo(string destFileName, bool overwrite)
        {
            destFileName = File.InternalCopy(base.FullPath, destFileName, overwrite);
            return new FileInfo(destFileName, false);
        }

        public FileStream Create() => 
            File.Create(base.FullPath);

        public StreamWriter CreateText() => 
            new StreamWriter(base.FullPath, false);

        [ComVisible(false)]
        public void Decrypt()
        {
            File.Decrypt(base.FullPath);
        }

        public override void Delete()
        {
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { base.FullPath }, false, false).Demand();
            if (Environment.IsWin9X() && System.IO.Directory.InternalExists(base.FullPath))
            {
                throw new UnauthorizedAccessException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("UnauthorizedAccess_IODenied_Path"), new object[] { base.OriginalPath }));
            }
            if (!Win32Native.DeleteFile(base.FullPath))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 2)
                {
                    __Error.WinIOError(errorCode, base.OriginalPath);
                }
            }
        }

        [ComVisible(false)]
        public void Encrypt()
        {
            File.Encrypt(base.FullPath);
        }

        public FileSecurity GetAccessControl() => 
            File.GetAccessControl(base.FullPath, AccessControlSections.Group | AccessControlSections.Owner | AccessControlSections.Access);

        public FileSecurity GetAccessControl(AccessControlSections includeSections) => 
            File.GetAccessControl(base.FullPath, includeSections);

        public void MoveTo(string destFileName)
        {
            if (destFileName == null)
            {
                throw new ArgumentNullException("destFileName");
            }
            if (destFileName.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destFileName");
            }
            new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, new string[] { base.FullPath }, false, false).Demand();
            string fullPathInternal = Path.GetFullPathInternal(destFileName);
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { fullPathInternal }, false, false).Demand();
            if (!Win32Native.MoveFile(base.FullPath, fullPathInternal))
            {
                __Error.WinIOError();
            }
            base.FullPath = fullPathInternal;
            base.OriginalPath = destFileName;
            this._name = Path.GetFileName(fullPathInternal);
            base._dataInitialised = -1;
        }

        public FileStream Open(FileMode mode) => 
            this.Open(mode, FileAccess.ReadWrite, FileShare.None);

        public FileStream Open(FileMode mode, FileAccess access) => 
            this.Open(mode, access, FileShare.None);

        public FileStream Open(FileMode mode, FileAccess access, FileShare share) => 
            new FileStream(base.FullPath, mode, access, share);

        public FileStream OpenRead() => 
            new FileStream(base.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read);

        public StreamReader OpenText() => 
            new StreamReader(base.FullPath, Encoding.UTF8, true, 0x400);

        public FileStream OpenWrite() => 
            new FileStream(base.FullPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

        [ComVisible(false)]
        public FileInfo Replace(string destinationFileName, string destinationBackupFileName) => 
            this.Replace(destinationFileName, destinationBackupFileName, false);

        [ComVisible(false)]
        public FileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            File.Replace(base.FullPath, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
            return new FileInfo(destinationFileName);
        }

        public void SetAccessControl(FileSecurity fileSecurity)
        {
            File.SetAccessControl(base.FullPath, fileSecurity);
        }

        public override string ToString() => 
            base.OriginalPath;

        public DirectoryInfo Directory
        {
            get
            {
                string directoryName = this.DirectoryName;
                if (directoryName == null)
                {
                    return null;
                }
                return new DirectoryInfo(directoryName);
            }
        }

        public string DirectoryName
        {
            get
            {
                string directoryName = Path.GetDirectoryName(base.FullPath);
                if (directoryName != null)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, new string[] { directoryName }, false, false).Demand();
                }
                return directoryName;
            }
        }

        public override bool Exists
        {
            get
            {
                try
                {
                    if (base._dataInitialised == -1)
                    {
                        base.Refresh();
                    }
                    if (base._dataInitialised != 0)
                    {
                        return false;
                    }
                    return ((this._data.fileAttributes & 0x10) == 0);
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsReadOnly
        {
            get => 
                ((base.Attributes & FileAttributes.ReadOnly) != 0);
            set
            {
                if (value)
                {
                    base.Attributes |= FileAttributes.ReadOnly;
                }
                else
                {
                    base.Attributes &= ~FileAttributes.ReadOnly;
                }
            }
        }

        public long Length
        {
            get
            {
                if (base._dataInitialised == -1)
                {
                    base.Refresh();
                }
                if (base._dataInitialised != 0)
                {
                    __Error.WinIOError(base._dataInitialised, base.OriginalPath);
                }
                if ((this._data.fileAttributes & 0x10) != 0)
                {
                    __Error.WinIOError(2, base.OriginalPath);
                }
                return ((this._data.fileSizeHigh << 0x20) | (this._data.fileSizeLow & ((long) 0xffffffffL)));
            }
        }

        public override string Name =>
            this._name;
    }
}

