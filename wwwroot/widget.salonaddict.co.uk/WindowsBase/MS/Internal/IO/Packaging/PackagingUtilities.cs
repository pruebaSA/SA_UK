namespace MS.Internal.IO.Packaging
{
    using Microsoft.Win32;
    using MS.Internal;
    using MS.Internal.WindowsBase;
    using System;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;
    using System.Windows;
    using System.Xml;

    [FriendAccessAllowed]
    internal static class PackagingUtilities
    {
        private static ReliableIsolatedStorageFileFolder _defaultFile;
        private const string _encodingAttribute = "encoding";
        private const string _fullProfileListKeyName = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
        private static object _isoStoreSyncObject = new object();
        private const string _profileListKeyName = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList";
        private static readonly string _webNameUnicode = Encoding.Unicode.WebName.ToUpperInvariant();
        private static readonly string _webNameUTF8 = Encoding.UTF8.WebName.ToUpperInvariant();
        internal const string ContainerFileExtension = "xps";
        internal static readonly string RelationshipNamespaceUri = "http://schemas.openxmlformats.org/package/2006/relationships";
        internal static readonly ContentType RelationshipPartContentType = new ContentType("application/vnd.openxmlformats-package.relationships+xml");
        internal const string XamlFileExtension = "xaml";
        private const string XmlNamespace = "xmlns";

        internal static void CalculateOverlap(long block1Offset, long block1Size, long block2Offset, long block2Size, out long overlapBlockOffset, out long overlapBlockSize)
        {
            overlapBlockOffset = Math.Max(block1Offset, block2Offset);
            overlapBlockSize = Math.Min((long) (block1Offset + block1Size), (long) (block2Offset + block2Size)) - overlapBlockOffset;
            if (overlapBlockSize <= 0L)
            {
                overlapBlockSize = 0L;
            }
        }

        internal static long CopyStream(Stream sourceStream, Stream targetStream, long bytesToCopy, int bufferSize)
        {
            int num2;
            Invariant.Assert(sourceStream != null);
            Invariant.Assert(targetStream != null);
            Invariant.Assert(bytesToCopy >= 0L);
            Invariant.Assert(bufferSize > 0);
            byte[] buffer = new byte[bufferSize];
            for (long i = bytesToCopy; i > 0L; i -= num2)
            {
                num2 = sourceStream.Read(buffer, 0, (int) Math.Min(i, (long) bufferSize));
                if (num2 == 0)
                {
                    targetStream.Flush();
                    return (bytesToCopy - i);
                }
                targetStream.Write(buffer, 0, num2);
            }
            targetStream.Flush();
            return bytesToCopy;
        }

        internal static Stream CreateUserScopedIsolatedStorageFileStreamWithRandomName(int retryCount, out string fileName)
        {
            if ((retryCount < 0) || (retryCount > 100))
            {
                throw new ArgumentOutOfRangeException("retryCount");
            }
            fileName = null;
        Label_0019:
            try
            {
                fileName = Path.GetRandomFileName();
                lock (IsoStoreSyncRoot)
                {
                    return GetDefaultIsolatedStorageFile().GetStream(fileName);
                }
            }
            catch (IOException)
            {
                if (--retryCount < 0)
                {
                    throw;
                }
                goto Label_0019;
            }
            return null;
        }

        private static void DeleteIsolatedStorageFile(string fileName)
        {
            lock (IsoStoreSyncRoot)
            {
                GetDefaultIsolatedStorageFile().IsoFile.DeleteFile(fileName);
            }
        }

        private static ReliableIsolatedStorageFileFolder GetDefaultIsolatedStorageFile()
        {
            if ((_defaultFile == null) || _defaultFile.IsDisposed())
            {
                _defaultFile = new ReliableIsolatedStorageFileFolder();
            }
            return _defaultFile;
        }

        internal static int GetNonXmlnsAttributeCount(XmlReader reader)
        {
            int num = 0;
            while (reader.MoveToNextAttribute())
            {
                if ((string.CompareOrdinal(reader.Name, "xmlns") != 0) && (string.CompareOrdinal(reader.Prefix, "xmlns") != 0))
                {
                    num++;
                }
            }
            reader.MoveToElement();
            return num;
        }

        internal static void PerformInitailReadAndVerifyEncoding(XmlTextReader reader)
        {
            Invariant.Assert((reader != null) && (reader.ReadState == ReadState.Initial));
            if ((reader.Read() && (reader.NodeType == XmlNodeType.XmlDeclaration)) && (reader.Depth == 0))
            {
                string attribute = reader.GetAttribute("encoding");
                if ((attribute != null) && (attribute.Length > 0))
                {
                    attribute = attribute.ToUpperInvariant();
                    if ((string.CompareOrdinal(attribute, _webNameUTF8) != 0) && (string.CompareOrdinal(attribute, _webNameUnicode) != 0))
                    {
                        throw new FileFormatException(System.Windows.SR.Get("EncodingNotSupported"));
                    }
                    return;
                }
            }
            if (!(reader.Encoding is UnicodeEncoding) && !(reader.Encoding is UTF8Encoding))
            {
                throw new FileFormatException(System.Windows.SR.Get("EncodingNotSupported"));
            }
        }

        internal static int ReliableRead(BinaryReader reader, byte[] buffer, int offset, int count) => 
            ReliableRead(reader, buffer, offset, count, count);

        internal static int ReliableRead(Stream stream, byte[] buffer, int offset, int count) => 
            ReliableRead(stream, buffer, offset, count, count);

        internal static int ReliableRead(BinaryReader reader, byte[] buffer, int offset, int requestedCount, int requiredCount)
        {
            Invariant.Assert(reader != null);
            Invariant.Assert(buffer != null);
            Invariant.Assert(buffer.Length > 0);
            Invariant.Assert(offset >= 0);
            Invariant.Assert(requestedCount >= 0);
            Invariant.Assert(requiredCount >= 0);
            Invariant.Assert((offset + requestedCount) <= buffer.Length);
            Invariant.Assert(requiredCount <= requestedCount);
            int num = 0;
            while (num < requiredCount)
            {
                int num2 = reader.Read(buffer, offset + num, requestedCount - num);
                if (num2 == 0)
                {
                    return num;
                }
                num += num2;
            }
            return num;
        }

        internal static int ReliableRead(Stream stream, byte[] buffer, int offset, int requestedCount, int requiredCount)
        {
            Invariant.Assert(stream != null);
            Invariant.Assert(buffer != null);
            Invariant.Assert(buffer.Length > 0);
            Invariant.Assert(offset >= 0);
            Invariant.Assert(requestedCount >= 0);
            Invariant.Assert(requiredCount >= 0);
            Invariant.Assert((offset + requestedCount) <= buffer.Length);
            Invariant.Assert(requiredCount <= requestedCount);
            int num = 0;
            while (num < requiredCount)
            {
                int num2 = stream.Read(buffer, offset + num, requestedCount - num);
                if (num2 == 0)
                {
                    return num;
                }
                num += num2;
            }
            return num;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static bool UserHasProfile()
        {
            PermissionSet set = new PermissionSet(PermissionState.None);
            set.AddPermission(new SecurityPermission(SecurityPermissionFlag.ControlPrincipal));
            set.AddPermission(new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList"));
            set.Assert();
            bool flag = false;
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\" + WindowsIdentity.GetCurrent().User.Value);
                flag = key != null;
            }
            finally
            {
                if (key != null)
                {
                    key.Close();
                }
                CodeAccessPermission.RevertAssert();
            }
            return flag;
        }

        internal static void VerifyStreamReadArgs(Stream s, byte[] buffer, int offset, int count)
        {
            if (!s.CanRead)
            {
                throw new NotSupportedException(System.Windows.SR.Get("ReadNotSupported"));
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", System.Windows.SR.Get("OffsetNegative"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", System.Windows.SR.Get("ReadCountNegative"));
            }
            if ((offset + count) > buffer.Length)
            {
                throw new ArgumentException(System.Windows.SR.Get("ReadBufferTooSmall"), "buffer");
            }
        }

        internal static void VerifyStreamWriteArgs(Stream s, byte[] buffer, int offset, int count)
        {
            if (!s.CanWrite)
            {
                throw new NotSupportedException(System.Windows.SR.Get("WriteNotSupported"));
            }
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException("offset", System.Windows.SR.Get("OffsetNegative"));
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", System.Windows.SR.Get("WriteCountNegative"));
            }
            if ((offset + count) > buffer.Length)
            {
                throw new ArgumentException(System.Windows.SR.Get("WriteBufferTooSmall"), "buffer");
            }
        }

        internal static object IsoStoreSyncRoot =>
            _isoStoreSyncObject;

        private class ReliableIsolatedStorageFileFolder : IDisposable
        {
            private bool _disposed;
            private static IsolatedStorageFile _file;
            private int _refCount;
            private static bool _userHasProfile = PackagingUtilities.UserHasProfile();

            internal ReliableIsolatedStorageFileFolder()
            {
                _file = this.GetCurrentStore();
            }

            internal void AddRef()
            {
                lock (PackagingUtilities.IsoStoreSyncRoot)
                {
                    this.CheckDisposed();
                    this._refCount++;
                }
            }

            private void CheckDisposed()
            {
                if (this._disposed)
                {
                    throw new ObjectDisposedException("ReliableIsolatedStorageFileFolder");
                }
            }

            internal void DecRef()
            {
                lock (PackagingUtilities.IsoStoreSyncRoot)
                {
                    this.CheckDisposed();
                    this._refCount--;
                    if (this._refCount <= 0)
                    {
                        this.Dispose();
                    }
                }
            }

            public void Dispose()
            {
                this.Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                try
                {
                    if (disposing)
                    {
                        lock (PackagingUtilities.IsoStoreSyncRoot)
                        {
                            if (!this._disposed)
                            {
                                using (_file)
                                {
                                    _file.Remove();
                                }
                                this._disposed = true;
                            }
                            _file = null;
                        }
                        GC.SuppressFinalize(this);
                    }
                    else
                    {
                        using (IsolatedStorageFile file = this.GetCurrentStore())
                        {
                            file.Remove();
                        }
                    }
                }
                catch (IsolatedStorageException)
                {
                }
            }

            ~ReliableIsolatedStorageFileFolder()
            {
                this.Dispose(false);
            }

            private IsolatedStorageFile GetCurrentStore()
            {
                if (_userHasProfile)
                {
                    return IsolatedStorageFile.GetUserStoreForDomain();
                }
                return IsolatedStorageFile.GetMachineStoreForDomain();
            }

            internal Stream GetStream(string fileName)
            {
                this.CheckDisposed();
                return new PackagingUtilities.SafeIsolatedStorageFileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None, this);
            }

            internal bool IsDisposed() => 
                this._disposed;

            internal IsolatedStorageFile IsoFile
            {
                get
                {
                    this.CheckDisposed();
                    return _file;
                }
            }
        }

        private class SafeIsolatedStorageFileStream : IsolatedStorageFileStream
        {
            private bool _disposed;
            private PackagingUtilities.ReliableIsolatedStorageFileFolder _folder;
            private string _path;

            internal SafeIsolatedStorageFileStream(string path, FileMode mode, FileAccess access, FileShare share, PackagingUtilities.ReliableIsolatedStorageFileFolder folder) : base(path, mode, access, share, folder.IsoFile)
            {
                if (path == null)
                {
                    throw new ArgumentNullException("path");
                }
                this._path = path;
                this._folder = folder;
                this._folder.AddRef();
            }

            protected override void Dispose(bool disposing)
            {
                if (!this._disposed)
                {
                    if (disposing)
                    {
                        base.Dispose(disposing);
                        if (this._path != null)
                        {
                            PackagingUtilities.DeleteIsolatedStorageFile(this._path);
                            this._path = null;
                        }
                        this._folder.DecRef();
                        this._folder = null;
                        GC.SuppressFinalize(this);
                    }
                    this._disposed = true;
                }
            }
        }
    }
}

