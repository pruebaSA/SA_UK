namespace System.IO.IsolatedStorage
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [ComVisible(true)]
    public class IsolatedStorageFileStream : FileStream
    {
        private FileStream m_fs;
        private string m_FullPath;
        private string m_GivenPath;
        private IsolatedStorageFile m_isf;
        private bool m_OwnedStore;
        private const string s_BackSlash = @"\";
        private const int s_BlockSize = 0x400;

        private IsolatedStorageFileStream()
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode) : this(path, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None, (IsolatedStorageFile) null)
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access) : this(path, mode, access, (access == FileAccess.Read) ? FileShare.Read : FileShare.None, 0x1000, null)
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode, IsolatedStorageFile isf) : this(path, mode, FileAccess.ReadWrite, FileShare.None, isf)
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share) : this(path, mode, access, share, 0x1000, null)
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, IsolatedStorageFile isf) : this(path, mode, access, (access == FileAccess.Read) ? FileShare.Read : FileShare.None, 0x1000, isf)
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : this(path, mode, access, share, bufferSize, null)
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, IsolatedStorageFile isf) : this(path, mode, access, share, 0x1000, isf)
        {
        }

        public IsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, IsolatedStorageFile isf)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if ((path.Length == 0) || path.Equals(@"\"))
            {
                throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_Path"));
            }
            ulong num = 0L;
            bool flag = false;
            bool flag2 = false;
            if (isf == null)
            {
                this.m_OwnedStore = true;
                isf = IsolatedStorageFile.GetUserStoreForDomain();
            }
            this.m_isf = isf;
            FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, this.m_isf.RootDirectory);
            permission.Assert();
            permission.PermitOnly();
            this.m_GivenPath = path;
            this.m_FullPath = this.m_isf.GetFullPath(this.m_GivenPath);
            try
            {
                switch (mode)
                {
                    case FileMode.CreateNew:
                        flag = true;
                        break;

                    case FileMode.Create:
                    case FileMode.OpenOrCreate:
                    case FileMode.Truncate:
                    case FileMode.Append:
                        this.m_isf.Lock();
                        flag2 = true;
                        try
                        {
                            FileInfo info = new FileInfo(this.m_FullPath);
                            num = IsolatedStorageFile.RoundToBlockSize((ulong) info.Length);
                        }
                        catch (FileNotFoundException)
                        {
                            flag = true;
                        }
                        catch
                        {
                        }
                        break;

                    case FileMode.Open:
                        break;

                    default:
                        throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_FileOpenMode"));
                }
                if (flag)
                {
                    this.m_isf.ReserveOneBlock();
                }
                try
                {
                    this.m_fs = new FileStream(this.m_FullPath, mode, access, share, bufferSize, FileOptions.None, this.m_GivenPath, true);
                }
                catch
                {
                    if (flag)
                    {
                        this.m_isf.UnreserveOneBlock();
                    }
                    throw;
                }
                if (!flag && ((mode == FileMode.Truncate) || (mode == FileMode.Create)))
                {
                    ulong num2 = IsolatedStorageFile.RoundToBlockSize((ulong) this.m_fs.Length);
                    if (num > num2)
                    {
                        this.m_isf.Unreserve(num - num2);
                    }
                    else if (num2 > num)
                    {
                        this.m_isf.Reserve(num2 - num);
                    }
                }
            }
            finally
            {
                if (flag2)
                {
                    this.m_isf.Unlock();
                }
            }
            CodeAccessPermission.RevertAll();
        }

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int numBytes, AsyncCallback userCallback, object stateObject) => 
            this.m_fs.BeginRead(buffer, offset, numBytes, userCallback, stateObject);

        [HostProtection(SecurityAction.LinkDemand, ExternalThreading=true)]
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
        {
            IAsyncResult result;
            this.m_isf.Lock();
            try
            {
                ulong length = (ulong) this.m_fs.Length;
                ulong newLen = (ulong) (this.m_fs.Position + numBytes);
                this.m_isf.Reserve(length, newLen);
                try
                {
                    result = this.m_fs.BeginWrite(buffer, offset, numBytes, userCallback, stateObject);
                }
                catch
                {
                    this.m_isf.UndoReserveOperation(length, newLen);
                    throw;
                }
            }
            finally
            {
                this.m_isf.Unlock();
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.m_fs != null)
                {
                    this.m_fs.Close();
                }
                if (this.m_OwnedStore && (this.m_isf != null))
                {
                    this.m_isf.Close();
                }
            }
            base.Dispose(disposing);
        }

        public override int EndRead(IAsyncResult asyncResult) => 
            this.m_fs.EndRead(asyncResult);

        public override void EndWrite(IAsyncResult asyncResult)
        {
            this.m_fs.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            this.m_fs.Flush();
        }

        internal void NotPermittedError()
        {
            this.NotPermittedError(Environment.GetResourceString("IsolatedStorage_Operation"));
        }

        internal void NotPermittedError(string str)
        {
            throw new IsolatedStorageException(str);
        }

        public override int Read(byte[] buffer, int offset, int count) => 
            this.m_fs.Read(buffer, offset, count);

        public override int ReadByte() => 
            this.m_fs.ReadByte();

        public override long Seek(long offset, SeekOrigin origin)
        {
            long num;
            this.m_isf.Lock();
            try
            {
                ulong num3;
                ulong length = (ulong) this.m_fs.Length;
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        num3 = (offset < 0L) ? ((ulong) 0L) : ((ulong) offset);
                        break;

                    case SeekOrigin.Current:
                        num3 = ((this.m_fs.Position + offset) < 0L) ? ((ulong) 0L) : ((ulong) (this.m_fs.Position + offset));
                        break;

                    case SeekOrigin.End:
                        num3 = ((this.m_fs.Length + offset) < 0L) ? ((ulong) 0L) : ((ulong) (this.m_fs.Length + offset));
                        break;

                    default:
                        throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_SeekOrigin"));
                }
                this.m_isf.Reserve(length, num3);
                try
                {
                    this.ZeroInit(length, num3);
                    num = this.m_fs.Seek(offset, origin);
                }
                catch
                {
                    this.m_isf.UndoReserveOperation(length, num3);
                    throw;
                }
            }
            finally
            {
                this.m_isf.Unlock();
            }
            return num;
        }

        public override void SetLength(long value)
        {
            if (value < 0L)
            {
                throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
            }
            this.m_isf.Lock();
            try
            {
                ulong length = (ulong) this.m_fs.Length;
                ulong newLen = (ulong) value;
                this.m_isf.Reserve(length, newLen);
                try
                {
                    this.ZeroInit(length, newLen);
                    this.m_fs.SetLength(value);
                }
                catch
                {
                    this.m_isf.UndoReserveOperation(length, newLen);
                    throw;
                }
                if (length > newLen)
                {
                    this.m_isf.UndoReserveOperation(newLen, length);
                }
            }
            finally
            {
                this.m_isf.Unlock();
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.m_isf.Lock();
            try
            {
                ulong length = (ulong) this.m_fs.Length;
                ulong newLen = (ulong) (this.m_fs.Position + count);
                this.m_isf.Reserve(length, newLen);
                try
                {
                    this.m_fs.Write(buffer, offset, count);
                }
                catch
                {
                    this.m_isf.UndoReserveOperation(length, newLen);
                    throw;
                }
            }
            finally
            {
                this.m_isf.Unlock();
            }
        }

        public override void WriteByte(byte value)
        {
            this.m_isf.Lock();
            try
            {
                ulong length = (ulong) this.m_fs.Length;
                ulong newLen = (ulong) (this.m_fs.Position + 1L);
                this.m_isf.Reserve(length, newLen);
                try
                {
                    this.m_fs.WriteByte(value);
                }
                catch
                {
                    this.m_isf.UndoReserveOperation(length, newLen);
                    throw;
                }
            }
            finally
            {
                this.m_isf.Unlock();
            }
        }

        private void ZeroInit(ulong oldLen, ulong newLen)
        {
            if (oldLen < newLen)
            {
                ulong num = newLen - oldLen;
                byte[] buffer = new byte[0x400];
                long position = this.m_fs.Position;
                this.m_fs.Seek((long) oldLen, SeekOrigin.Begin);
                if (num <= 0x400L)
                {
                    this.m_fs.Write(buffer, 0, (int) num);
                    this.m_fs.Position = position;
                }
                else
                {
                    int count = 0x400 - ((int) (oldLen & ((ulong) 0x3ffL)));
                    this.m_fs.Write(buffer, 0, count);
                    num -= count;
                    int num4 = (int) (num / ((ulong) 0x400L));
                    for (int i = 0; i < num4; i++)
                    {
                        this.m_fs.Write(buffer, 0, 0x400);
                    }
                    this.m_fs.Write(buffer, 0, (int) (num & ((ulong) 0x3ffL)));
                    this.m_fs.Position = position;
                }
            }
        }

        public override bool CanRead =>
            this.m_fs.CanRead;

        public override bool CanSeek =>
            this.m_fs.CanSeek;

        public override bool CanWrite =>
            this.m_fs.CanWrite;

        [Obsolete("This property has been deprecated.  Please use IsolatedStorageFileStream's SafeFileHandle property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public override IntPtr Handle
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                this.NotPermittedError();
                return Win32Native.INVALID_HANDLE_VALUE;
            }
        }

        public override bool IsAsync =>
            this.m_fs.IsAsync;

        public override long Length =>
            this.m_fs.Length;

        public override long Position
        {
            get => 
                this.m_fs.Position;
            set
            {
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
                }
                this.Seek(value, SeekOrigin.Begin);
            }
        }

        public override Microsoft.Win32.SafeHandles.SafeFileHandle SafeFileHandle
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                this.NotPermittedError();
                return null;
            }
        }
    }
}

