namespace System.IO
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Text;

    [ComVisible(true)]
    public static class File
    {
        private const int ERROR_ACCESS_DENIED = 5;
        private const int ERROR_INVALID_PARAMETER = 0x57;
        private const int GetFileExInfoStandard = 0;

        public static void AppendAllText(string path, string contents)
        {
            AppendAllText(path, contents, StreamWriter.UTF8NoBOM);
        }

        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            using (StreamWriter writer = new StreamWriter(path, true, encoding))
            {
                writer.Write(contents);
            }
        }

        public static StreamWriter AppendText(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            return new StreamWriter(path, true);
        }

        public static void Copy(string sourceFileName, string destFileName)
        {
            Copy(sourceFileName, destFileName, false);
        }

        public static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            InternalCopy(sourceFileName, destFileName, overwrite);
        }

        public static FileStream Create(string path) => 
            Create(path, 0x1000, FileOptions.None);

        public static FileStream Create(string path, int bufferSize) => 
            Create(path, bufferSize, FileOptions.None);

        public static FileStream Create(string path, int bufferSize, FileOptions options) => 
            new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize, options);

        public static FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity) => 
            new FileStream(path, FileMode.Create, FileSystemRights.Read | FileSystemRights.Write, FileShare.None, bufferSize, options, fileSecurity);

        public static StreamWriter CreateText(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            return new StreamWriter(path, false);
        }

        public static void Decrypt(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (!Environment.RunningOnWinNT)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_Win9x"));
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            if (!Win32Native.DecryptFile(fullPathInternal, 0))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 5)
                {
                    DriveInfo info = new DriveInfo(Path.GetPathRoot(fullPathInternal));
                    if (!string.Equals("NTFS", info.DriveFormat))
                    {
                        throw new NotSupportedException(Environment.GetResourceString("NotSupported_EncryptionNeedsNTFS"));
                    }
                }
                __Error.WinIOError(errorCode, fullPathInternal);
            }
        }

        public static void Delete(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { fullPathInternal }, false, false).Demand();
            if (Environment.IsWin9X() && Directory.InternalExists(fullPathInternal))
            {
                throw new UnauthorizedAccessException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("UnauthorizedAccess_IODenied_Path"), new object[] { path }));
            }
            if (!Win32Native.DeleteFile(fullPathInternal))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode != 2)
                {
                    __Error.WinIOError(errorCode, fullPathInternal);
                }
            }
        }

        public static void Encrypt(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (!Environment.RunningOnWinNT)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_Win9x"));
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            if (!Win32Native.EncryptFile(fullPathInternal))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 5)
                {
                    DriveInfo info = new DriveInfo(Path.GetPathRoot(fullPathInternal));
                    if (!string.Equals("NTFS", info.DriveFormat))
                    {
                        throw new NotSupportedException(Environment.GetResourceString("NotSupported_EncryptionNeedsNTFS"));
                    }
                }
                __Error.WinIOError(errorCode, fullPathInternal);
            }
        }

        public static bool Exists(string path)
        {
            try
            {
                if (path == null)
                {
                    return false;
                }
                if (path.Length == 0)
                {
                    return false;
                }
                path = Path.GetFullPathInternal(path);
                new FileIOPermission(FileIOPermissionAccess.Read, new string[] { path }, false, false).Demand();
                return InternalExists(path);
            }
            catch (ArgumentException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return false;
        }

        internal static int FillAttributeInfo(string path, ref Win32Native.WIN32_FILE_ATTRIBUTE_DATA data, bool tryagain, bool returnErrorOnNotFound)
        {
            int num = 0;
            if ((Environment.OSInfo == Environment.OSName.Win95) || tryagain)
            {
                Win32Native.WIN32_FIND_DATA win_find_data = new Win32Native.WIN32_FIND_DATA();
                string fileName = path.TrimEnd(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
                int num2 = Win32Native.SetErrorMode(1);
                try
                {
                    bool flag = false;
                    SafeFindHandle handle = Win32Native.FindFirstFile(fileName, win_find_data);
                    try
                    {
                        if (handle.IsInvalid)
                        {
                            flag = true;
                            num = Marshal.GetLastWin32Error();
                            if ((((num == 2) || (num == 3)) || (num == 0x15)) && !returnErrorOnNotFound)
                            {
                                num = 0;
                                data.fileAttributes = -1;
                            }
                            return num;
                        }
                    }
                    finally
                    {
                        try
                        {
                            handle.Close();
                        }
                        catch
                        {
                            if (!flag)
                            {
                                __Error.WinIOError();
                            }
                        }
                    }
                }
                finally
                {
                    Win32Native.SetErrorMode(num2);
                }
                data.fileAttributes = win_find_data.dwFileAttributes;
                data.ftCreationTimeLow = (uint) win_find_data.ftCreationTime_dwLowDateTime;
                data.ftCreationTimeHigh = (uint) win_find_data.ftCreationTime_dwHighDateTime;
                data.ftLastAccessTimeLow = (uint) win_find_data.ftLastAccessTime_dwLowDateTime;
                data.ftLastAccessTimeHigh = (uint) win_find_data.ftLastAccessTime_dwHighDateTime;
                data.ftLastWriteTimeLow = (uint) win_find_data.ftLastWriteTime_dwLowDateTime;
                data.ftLastWriteTimeHigh = (uint) win_find_data.ftLastWriteTime_dwHighDateTime;
                data.fileSizeHigh = win_find_data.nFileSizeHigh;
                data.fileSizeLow = win_find_data.nFileSizeLow;
                return num;
            }
            bool flag2 = false;
            int newMode = Win32Native.SetErrorMode(1);
            try
            {
                flag2 = Win32Native.GetFileAttributesEx(path, 0, ref data);
            }
            finally
            {
                Win32Native.SetErrorMode(newMode);
            }
            if (!flag2)
            {
                num = Marshal.GetLastWin32Error();
                if (((num != 2) && (num != 3)) && (num != 0x15))
                {
                    return FillAttributeInfo(path, ref data, true, returnErrorOnNotFound);
                }
                if (!returnErrorOnNotFound)
                {
                    num = 0;
                    data.fileAttributes = -1;
                }
            }
            return num;
        }

        public static FileSecurity GetAccessControl(string path) => 
            GetAccessControl(path, AccessControlSections.Group | AccessControlSections.Owner | AccessControlSections.Access);

        public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections) => 
            new FileSecurity(path, includeSections);

        public static FileAttributes GetAttributes(string path)
        {
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            Win32Native.WIN32_FILE_ATTRIBUTE_DATA data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            int errorCode = FillAttributeInfo(fullPathInternal, ref data, false, true);
            if (errorCode != 0)
            {
                __Error.WinIOError(errorCode, fullPathInternal);
            }
            return (FileAttributes) data.fileAttributes;
        }

        public static DateTime GetCreationTime(string path) => 
            GetCreationTimeUtc(path).ToLocalTime();

        public static DateTime GetCreationTimeUtc(string path)
        {
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            Win32Native.WIN32_FILE_ATTRIBUTE_DATA data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            int errorCode = FillAttributeInfo(fullPathInternal, ref data, false, false);
            if (errorCode != 0)
            {
                __Error.WinIOError(errorCode, fullPathInternal);
            }
            long fileTime = (data.ftCreationTimeHigh << 0x20) | data.ftCreationTimeLow;
            return DateTime.FromFileTimeUtc(fileTime);
        }

        public static DateTime GetLastAccessTime(string path) => 
            GetLastAccessTimeUtc(path).ToLocalTime();

        public static DateTime GetLastAccessTimeUtc(string path)
        {
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            Win32Native.WIN32_FILE_ATTRIBUTE_DATA data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            int errorCode = FillAttributeInfo(fullPathInternal, ref data, false, false);
            if (errorCode != 0)
            {
                __Error.WinIOError(errorCode, fullPathInternal);
            }
            long fileTime = (data.ftLastAccessTimeHigh << 0x20) | data.ftLastAccessTimeLow;
            return DateTime.FromFileTimeUtc(fileTime);
        }

        public static DateTime GetLastWriteTime(string path) => 
            GetLastWriteTimeUtc(path).ToLocalTime();

        public static DateTime GetLastWriteTimeUtc(string path)
        {
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            Win32Native.WIN32_FILE_ATTRIBUTE_DATA data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            int errorCode = FillAttributeInfo(fullPathInternal, ref data, false, false);
            if (errorCode != 0)
            {
                __Error.WinIOError(errorCode, fullPathInternal);
            }
            long fileTime = (data.ftLastWriteTimeHigh << 0x20) | data.ftLastWriteTimeLow;
            return DateTime.FromFileTimeUtc(fileTime);
        }

        internal static string InternalCopy(string sourceFileName, string destFileName, bool overwrite)
        {
            if ((sourceFileName == null) || (destFileName == null))
            {
                throw new ArgumentNullException((sourceFileName == null) ? "sourceFileName" : "destFileName", Environment.GetResourceString("ArgumentNull_FileName"));
            }
            if ((sourceFileName.Length == 0) || (destFileName.Length == 0))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), (sourceFileName.Length == 0) ? "sourceFileName" : "destFileName");
            }
            string fullPathInternal = Path.GetFullPathInternal(sourceFileName);
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            string dst = Path.GetFullPathInternal(destFileName);
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { dst }, false, false).Demand();
            if (!Win32Native.CopyFile(fullPathInternal, dst, !overwrite))
            {
                int errorCode = Marshal.GetLastWin32Error();
                string maybeFullPath = destFileName;
                if (errorCode != 80)
                {
                    using (SafeFileHandle handle = Win32Native.UnsafeCreateFile(fullPathInternal, -2147483648, FileShare.Read, null, FileMode.Open, 0, IntPtr.Zero))
                    {
                        if (handle.IsInvalid)
                        {
                            maybeFullPath = sourceFileName;
                        }
                    }
                    if ((errorCode == 5) && Directory.InternalExists(dst))
                    {
                        throw new IOException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_FileIsDirectory_Name"), new object[] { destFileName }), 5, dst);
                    }
                }
                __Error.WinIOError(errorCode, maybeFullPath);
            }
            return dst;
        }

        internal static bool InternalExists(string path)
        {
            Win32Native.WIN32_FILE_ATTRIBUTE_DATA data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            return (((FillAttributeInfo(path, ref data, false, true) == 0) && (data.fileAttributes != -1)) && ((data.fileAttributes & 0x10) == 0));
        }

        public static void Move(string sourceFileName, string destFileName)
        {
            if ((sourceFileName == null) || (destFileName == null))
            {
                throw new ArgumentNullException((sourceFileName == null) ? "sourceFileName" : "destFileName", Environment.GetResourceString("ArgumentNull_FileName"));
            }
            if ((sourceFileName.Length == 0) || (destFileName.Length == 0))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), (sourceFileName.Length == 0) ? "sourceFileName" : "destFileName");
            }
            string fullPathInternal = Path.GetFullPathInternal(sourceFileName);
            new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, new string[] { fullPathInternal }, false, false).Demand();
            string dst = Path.GetFullPathInternal(destFileName);
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { dst }, false, false).Demand();
            if (!InternalExists(fullPathInternal))
            {
                __Error.WinIOError(2, fullPathInternal);
            }
            if (!Win32Native.MoveFile(fullPathInternal, dst))
            {
                __Error.WinIOError();
            }
        }

        public static FileStream Open(string path, FileMode mode) => 
            Open(path, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None);

        public static FileStream Open(string path, FileMode mode, FileAccess access) => 
            Open(path, mode, access, FileShare.None);

        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share) => 
            new FileStream(path, mode, access, share);

        private static FileStream OpenFile(string path, FileAccess access, out SafeFileHandle handle)
        {
            FileStream stream = new FileStream(path, FileMode.Open, access, FileShare.ReadWrite, 1);
            handle = stream.SafeFileHandle;
            if (handle.IsInvalid)
            {
                int errorCode = Marshal.GetLastWin32Error();
                string fullPathInternal = Path.GetFullPathInternal(path);
                if ((errorCode == 3) && fullPathInternal.Equals(Directory.GetDirectoryRoot(fullPathInternal)))
                {
                    errorCode = 5;
                }
                __Error.WinIOError(errorCode, path);
            }
            return stream;
        }

        public static FileStream OpenRead(string path) => 
            new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        public static StreamReader OpenText(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            return new StreamReader(path);
        }

        public static FileStream OpenWrite(string path) => 
            new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

        public static byte[] ReadAllBytes(string path)
        {
            byte[] buffer;
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int offset = 0;
                long length = stream.Length;
                if (length > 0x7fffffffL)
                {
                    throw new IOException(Environment.GetResourceString("IO.IO_FileTooLong2GB"));
                }
                int count = (int) length;
                buffer = new byte[count];
                while (count > 0)
                {
                    int num4 = stream.Read(buffer, offset, count);
                    if (num4 == 0)
                    {
                        __Error.EndOfFile();
                    }
                    offset += num4;
                    count -= num4;
                }
            }
            return buffer;
        }

        public static string[] ReadAllLines(string path) => 
            ReadAllLines(path, Encoding.UTF8);

        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            ArrayList list = new ArrayList();
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                string str;
                while ((str = reader.ReadLine()) != null)
                {
                    list.Add(str);
                }
            }
            return (string[]) list.ToArray(typeof(string));
        }

        public static string ReadAllText(string path) => 
            ReadAllText(path, Encoding.UTF8);

        public static string ReadAllText(string path, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(path, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            Replace(sourceFileName, destinationFileName, destinationBackupFileName, false);
        }

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            if (sourceFileName == null)
            {
                throw new ArgumentNullException("sourceFileName");
            }
            if (destinationFileName == null)
            {
                throw new ArgumentNullException("destinationFileName");
            }
            string fullPathInternal = Path.GetFullPathInternal(sourceFileName);
            string replacedFileName = Path.GetFullPathInternal(destinationFileName);
            string path = null;
            if (destinationBackupFileName != null)
            {
                path = Path.GetFullPathInternal(destinationBackupFileName);
            }
            FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, new string[] { fullPathInternal, replacedFileName });
            if (destinationBackupFileName != null)
            {
                permission.AddPathList(FileIOPermissionAccess.Write, path);
            }
            permission.Demand();
            if (Environment.OSVersion.Platform == PlatformID.Win32Windows)
            {
                throw new PlatformNotSupportedException(Environment.GetResourceString("PlatformNotSupported_Win9x"));
            }
            int dwReplaceFlags = 1;
            if (ignoreMetadataErrors)
            {
                dwReplaceFlags |= 2;
            }
            if (!Win32Native.ReplaceFile(replacedFileName, fullPathInternal, path, dwReplaceFlags, IntPtr.Zero, IntPtr.Zero))
            {
                __Error.WinIOError();
            }
        }

        public static void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            if (fileSecurity == null)
            {
                throw new ArgumentNullException("fileSecurity");
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            fileSecurity.Persist(fullPathInternal);
        }

        public static void SetAttributes(string path, FileAttributes fileAttributes)
        {
            string fullPathInternal = Path.GetFullPathInternal(path);
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { fullPathInternal }, false, false).Demand();
            if (!Win32Native.SetFileAttributes(fullPathInternal, (int) fileAttributes))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if ((errorCode == 0x57) || ((errorCode == 5) && Environment.IsWin9X()))
                {
                    throw new ArgumentException(Environment.GetResourceString("Arg_InvalidFileAttrs"));
                }
                __Error.WinIOError(errorCode, fullPathInternal);
            }
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            SetCreationTimeUtc(path, creationTime.ToUniversalTime());
        }

        public static unsafe void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            SafeFileHandle handle;
            using (OpenFile(path, FileAccess.Write, out handle))
            {
                Win32Native.FILE_TIME creationTime = new Win32Native.FILE_TIME(creationTimeUtc.ToFileTimeUtc());
                if (!Win32Native.SetFileTime(handle, &creationTime, null, null))
                {
                    __Error.WinIOError(Marshal.GetLastWin32Error(), path);
                }
            }
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
        }

        public static unsafe void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            SafeFileHandle handle;
            using (OpenFile(path, FileAccess.Write, out handle))
            {
                Win32Native.FILE_TIME lastAccessTime = new Win32Native.FILE_TIME(lastAccessTimeUtc.ToFileTimeUtc());
                if (!Win32Native.SetFileTime(handle, null, &lastAccessTime, null))
                {
                    __Error.WinIOError(Marshal.GetLastWin32Error(), path);
                }
            }
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
        }

        public static unsafe void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            SafeFileHandle handle;
            using (OpenFile(path, FileAccess.Write, out handle))
            {
                Win32Native.FILE_TIME lastWriteTime = new Win32Native.FILE_TIME(lastWriteTimeUtc.ToFileTimeUtc());
                if (!Win32Native.SetFileTime(handle, null, null, &lastWriteTime))
                {
                    __Error.WinIOError(Marshal.GetLastWin32Error(), path);
                }
            }
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public static void WriteAllLines(string path, string[] contents)
        {
            WriteAllLines(path, contents, StreamWriter.UTF8NoBOM);
        }

        public static void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            if (contents == null)
            {
                throw new ArgumentNullException("contents");
            }
            using (StreamWriter writer = new StreamWriter(path, false, encoding))
            {
                foreach (string str in contents)
                {
                    writer.WriteLine(str);
                }
            }
        }

        public static void WriteAllText(string path, string contents)
        {
            WriteAllText(path, contents, StreamWriter.UTF8NoBOM);
        }

        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            using (StreamWriter writer = new StreamWriter(path, false, encoding))
            {
                writer.Write(contents);
            }
        }
    }
}

