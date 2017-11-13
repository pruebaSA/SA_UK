namespace System.IO
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Text;

    [ComVisible(true)]
    public static class Directory
    {
        private const int FILE_ATTRIBUTE_DIRECTORY = 0x10;
        private const int FILE_FLAG_BACKUP_SEMANTICS = 0x2000000;
        private const int FILE_SHARE_DELETE = 4;
        private const int FILE_SHARE_WRITE = 2;
        private const int GENERIC_WRITE = 0x40000000;
        private const int OPEN_EXISTING = 3;

        public static DirectoryInfo CreateDirectory(string path) => 
            CreateDirectory(path, null);

        public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (path.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"));
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            string demandDir = GetDemandDir(fullPathInternal, true);
            new FileIOPermission(FileIOPermissionAccess.Read, new string[] { demandDir }, false, false).Demand();
            InternalCreateDirectory(fullPathInternal, path, directorySecurity);
            return new DirectoryInfo(fullPathInternal, false);
        }

        public static void Delete(string path)
        {
            Delete(Path.GetFullPathInternal(path), path, false);
        }

        public static void Delete(string path, bool recursive)
        {
            Delete(Path.GetFullPathInternal(path), path, recursive);
        }

        internal static void Delete(string fullPath, string userPath, bool recursive)
        {
            string demandDir = GetDemandDir(fullPath, !recursive);
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { demandDir }, false, false).Demand();
            Win32Native.WIN32_FILE_ATTRIBUTE_DATA data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            int errorCode = File.FillAttributeInfo(fullPath, ref data, false, true);
            switch (errorCode)
            {
                case 0:
                    goto Label_0047;

                case 2:
                    errorCode = 3;
                    break;
            }
            __Error.WinIOError(errorCode, fullPath);
        Label_0047:
            if ((data.fileAttributes & 0x400) != 0)
            {
                recursive = false;
            }
            DeleteHelper(fullPath, userPath, recursive);
        }

        private static void DeleteHelper(string fullPath, string userPath, bool recursive)
        {
            int num;
            Exception exception = null;
            if (recursive)
            {
                Win32Native.WIN32_FIND_DATA data = new Win32Native.WIN32_FIND_DATA();
                using (SafeFindHandle handle = Win32Native.FindFirstFile(fullPath + Path.DirectorySeparatorChar + "*", data))
                {
                    if (handle.IsInvalid)
                    {
                        num = Marshal.GetLastWin32Error();
                        __Error.WinIOError(num, fullPath);
                    }
                    do
                    {
                        if (0 != (data.dwFileAttributes & 0x10))
                        {
                            if (!data.cFileName.Equals(".") && !data.cFileName.Equals(".."))
                            {
                                if (0 == (data.dwFileAttributes & 0x400))
                                {
                                    string str = Path.InternalCombine(fullPath, data.cFileName);
                                    string str2 = Path.InternalCombine(userPath, data.cFileName);
                                    try
                                    {
                                        DeleteHelper(str, str2, recursive);
                                    }
                                    catch (Exception exception2)
                                    {
                                        if (exception == null)
                                        {
                                            exception = exception2;
                                        }
                                    }
                                }
                                else
                                {
                                    if ((data.dwReserved0 == -1610612733) && !Win32Native.DeleteVolumeMountPoint(Path.InternalCombine(fullPath, data.cFileName + Path.DirectorySeparatorChar)))
                                    {
                                        num = Marshal.GetLastWin32Error();
                                        try
                                        {
                                            __Error.WinIOError(num, data.cFileName);
                                        }
                                        catch (Exception exception3)
                                        {
                                            if (exception == null)
                                            {
                                                exception = exception3;
                                            }
                                        }
                                    }
                                    if (!Win32Native.RemoveDirectory(Path.InternalCombine(fullPath, data.cFileName)))
                                    {
                                        num = Marshal.GetLastWin32Error();
                                        try
                                        {
                                            __Error.WinIOError(num, data.cFileName);
                                        }
                                        catch (Exception exception4)
                                        {
                                            if (exception == null)
                                            {
                                                exception = exception4;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (!Win32Native.DeleteFile(Path.InternalCombine(fullPath, data.cFileName)))
                        {
                            num = Marshal.GetLastWin32Error();
                            try
                            {
                                __Error.WinIOError(num, data.cFileName);
                            }
                            catch (Exception exception5)
                            {
                                if (exception == null)
                                {
                                    exception = exception5;
                                }
                            }
                        }
                    }
                    while (Win32Native.FindNextFile(handle, data));
                    num = Marshal.GetLastWin32Error();
                }
                if (exception != null)
                {
                    throw exception;
                }
                if ((num != 0) && (num != 0x12))
                {
                    __Error.WinIOError(num, userPath);
                }
            }
            if (!Win32Native.RemoveDirectory(fullPath))
            {
                num = Marshal.GetLastWin32Error();
                switch (num)
                {
                    case 2:
                        num = 3;
                        break;

                    case 5:
                        throw new IOException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("UnauthorizedAccess_IODenied_Path"), new object[] { userPath }));
                }
                __Error.WinIOError(num, fullPath);
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
                string fullPathInternal = Path.GetFullPathInternal(path);
                string demandDir = GetDemandDir(fullPathInternal, true);
                new FileIOPermission(FileIOPermissionAccess.Read, new string[] { demandDir }, false, false).Demand();
                return InternalExists(fullPathInternal);
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

        public static DirectorySecurity GetAccessControl(string path) => 
            new DirectorySecurity(path, AccessControlSections.Group | AccessControlSections.Owner | AccessControlSections.Access);

        public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections) => 
            new DirectorySecurity(path, includeSections);

        public static DateTime GetCreationTime(string path) => 
            File.GetCreationTime(path);

        public static DateTime GetCreationTimeUtc(string path) => 
            File.GetCreationTimeUtc(path);

        public static string GetCurrentDirectory()
        {
            StringBuilder lpBuffer = new StringBuilder(0x105);
            if (Win32Native.GetCurrentDirectory(lpBuffer.Capacity, lpBuffer) == 0)
            {
                __Error.WinIOError();
            }
            string path = lpBuffer.ToString();
            if (path.IndexOf('~') >= 0)
            {
                int num = Win32Native.GetLongPathName(path, lpBuffer, lpBuffer.Capacity);
                if ((num == 0) || (num >= 260))
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    if (num >= 260)
                    {
                        errorCode = 0xce;
                    }
                    if (((errorCode != 2) && (errorCode != 3)) && ((errorCode != 1) && (errorCode != 5)))
                    {
                        __Error.WinIOError(errorCode, string.Empty);
                    }
                }
                path = lpBuffer.ToString();
            }
            string demandDir = GetDemandDir(path, true);
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, new string[] { demandDir }, false, false).Demand();
            return path;
        }

        internal static string GetDemandDir(string fullPath, bool thisDirOnly)
        {
            if (thisDirOnly)
            {
                if (fullPath.EndsWith(Path.DirectorySeparatorChar) || fullPath.EndsWith(Path.AltDirectorySeparatorChar))
                {
                    return (fullPath + '.');
                }
                return (fullPath + Path.DirectorySeparatorChar + '.');
            }
            if (!fullPath.EndsWith(Path.DirectorySeparatorChar) && !fullPath.EndsWith(Path.AltDirectorySeparatorChar))
            {
                return (fullPath + Path.DirectorySeparatorChar);
            }
            return fullPath;
        }

        public static string[] GetDirectories(string path) => 
            GetDirectories(path, "*");

        public static string[] GetDirectories(string path, string searchPattern) => 
            GetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);

        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            return InternalGetFileDirectoryNames(path, path, searchPattern, false, true, searchOption);
        }

        public static string GetDirectoryRoot(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            string fullPath = fullPathInternal.Substring(0, Path.GetRootLength(fullPathInternal));
            string demandDir = GetDemandDir(fullPath, true);
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, new string[] { demandDir }, false, false).Demand();
            return fullPath;
        }

        public static string[] GetFiles(string path) => 
            GetFiles(path, "*");

        public static string[] GetFiles(string path, string searchPattern) => 
            GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            return InternalGetFileDirectoryNames(path, path, searchPattern, true, false, searchOption);
        }

        public static string[] GetFileSystemEntries(string path) => 
            GetFileSystemEntries(path, "*");

        public static string[] GetFileSystemEntries(string path, string searchPattern) => 
            GetFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);

        private static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            return InternalGetFileDirectoryNames(path, path, searchPattern, true, true, searchOption);
        }

        public static DateTime GetLastAccessTime(string path) => 
            File.GetLastAccessTime(path);

        public static DateTime GetLastAccessTimeUtc(string path) => 
            File.GetLastAccessTimeUtc(path);

        public static DateTime GetLastWriteTime(string path) => 
            File.GetLastWriteTime(path);

        public static DateTime GetLastWriteTimeUtc(string path) => 
            File.GetLastWriteTimeUtc(path);

        public static string[] GetLogicalDrives()
        {
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            int logicalDrives = Win32Native.GetLogicalDrives();
            if (logicalDrives == 0)
            {
                __Error.WinIOError();
            }
            uint num2 = (uint) logicalDrives;
            int num3 = 0;
            while (num2 != 0)
            {
                if ((num2 & 1) != 0)
                {
                    num3++;
                }
                num2 = num2 >> 1;
            }
            string[] strArray = new string[num3];
            char[] chArray = new char[] { 'A', ':', '\\' };
            num2 = (uint) logicalDrives;
            num3 = 0;
            while (num2 != 0)
            {
                if ((num2 & 1) != 0)
                {
                    strArray[num3++] = new string(chArray);
                }
                num2 = num2 >> 1;
                chArray[0] = (char) (chArray[0] + '\x0001');
            }
            return strArray;
        }

        public static DirectoryInfo GetParent(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if (path.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"), "path");
            }
            string directoryName = Path.GetDirectoryName(Path.GetFullPathInternal(path));
            if (directoryName == null)
            {
                return null;
            }
            return new DirectoryInfo(directoryName);
        }

        internal static unsafe void InternalCreateDirectory(string fullPath, string path, DirectorySecurity dirSecurity)
        {
            int length = fullPath.Length;
            if ((length >= 2) && Path.IsDirectorySeparator(fullPath[length - 1]))
            {
                length--;
            }
            int rootLength = Path.GetRootLength(fullPath);
            if ((length == 2) && Path.IsDirectorySeparator(fullPath[1]))
            {
                throw new IOException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("IO.IO_CannotCreateDirectory"), new object[] { path }));
            }
            List<string> list = new List<string>();
            bool flag = false;
            if (length > rootLength)
            {
                for (int i = length - 1; i >= rootLength; i--)
                {
                    string str = fullPath.Substring(0, i + 1);
                    if (!InternalExists(str))
                    {
                        list.Add(str);
                    }
                    else
                    {
                        flag = true;
                    }
                    while (((i > rootLength) && (fullPath[i] != Path.DirectorySeparatorChar)) && (fullPath[i] != Path.AltDirectorySeparatorChar))
                    {
                        i--;
                    }
                }
            }
            int count = list.Count;
            if (list.Count != 0)
            {
                string[] array = new string[list.Count];
                list.CopyTo(array, 0);
                for (int j = 0; j < array.Length; j++)
                {
                    string[] strArray2;
                    IntPtr ptr;
                    (strArray2 = array)[(int) (ptr = (IntPtr) j)] = strArray2[(int) ptr] + @"\.";
                }
                AccessControlActions control = (dirSecurity == null) ? AccessControlActions.None : AccessControlActions.Change;
                new FileIOPermission(FileIOPermissionAccess.Write, control, array, false, false).Demand();
            }
            Win32Native.SECURITY_ATTRIBUTES structure = null;
            if (dirSecurity != null)
            {
                structure = new Win32Native.SECURITY_ATTRIBUTES();
                structure.nLength = Marshal.SizeOf(structure);
                byte[] securityDescriptorBinaryForm = dirSecurity.GetSecurityDescriptorBinaryForm();
                byte* pDest = stackalloc byte[1 * securityDescriptorBinaryForm.Length];
                Buffer.memcpy(securityDescriptorBinaryForm, 0, pDest, 0, securityDescriptorBinaryForm.Length);
                structure.pSecurityDescriptor = pDest;
            }
            bool flag2 = true;
            int errorCode = 0;
            string maybeFullPath = path;
            while (list.Count > 0)
            {
                string str3 = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                if (str3.Length > 0xf8)
                {
                    throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
                }
                flag2 = Win32Native.CreateDirectory(str3, structure);
                if (!flag2 && (errorCode == 0))
                {
                    int num7 = Marshal.GetLastWin32Error();
                    if (num7 != 0xb7)
                    {
                        errorCode = num7;
                    }
                    else if (File.InternalExists(str3))
                    {
                        errorCode = num7;
                        try
                        {
                            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, GetDemandDir(str3, true)).Demand();
                            maybeFullPath = str3;
                            continue;
                        }
                        catch (SecurityException)
                        {
                            continue;
                        }
                    }
                }
            }
            if ((count == 0) && !flag)
            {
                if (!InternalExists(InternalGetDirectoryRoot(fullPath)))
                {
                    __Error.WinIOError(3, InternalGetDirectoryRoot(path));
                }
            }
            else if (!flag2 && (errorCode != 0))
            {
                __Error.WinIOError(errorCode, maybeFullPath);
            }
        }

        internal static bool InternalExists(string path)
        {
            Win32Native.WIN32_FILE_ATTRIBUTE_DATA data = new Win32Native.WIN32_FILE_ATTRIBUTE_DATA();
            return (((File.FillAttributeInfo(path, ref data, false, true) == 0) && (data.fileAttributes != -1)) && ((data.fileAttributes & 0x10) != 0));
        }

        internal static string InternalGetDirectoryRoot(string path) => 
            path?.Substring(0, Path.GetRootLength(path));

        internal static string[] InternalGetFileDirectoryNames(string path, string userPathOriginal, string searchPattern, bool includeFiles, bool includeDirs, SearchOption searchOption)
        {
            string str5;
            int errorCode = 0;
            if ((searchOption != SearchOption.TopDirectoryOnly) && (searchOption != SearchOption.AllDirectories))
            {
                throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
            }
            searchPattern = searchPattern.TrimEnd(new char[0]);
            if (searchPattern.Length == 0)
            {
                return new string[0];
            }
            Path.CheckSearchPattern(searchPattern);
            string fullPathInternal = Path.GetFullPathInternal(path);
            string[] pathList = new string[] { GetDemandDir(fullPathInternal, true) };
            new FileIOPermission(FileIOPermissionAccess.PathDiscovery, pathList, false, false).Demand();
            string str2 = userPathOriginal;
            string directoryName = Path.GetDirectoryName(searchPattern);
            if ((directoryName != null) && (directoryName.Length != 0))
            {
                pathList = new string[] { GetDemandDir(Path.InternalCombine(fullPathInternal, directoryName), true) };
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, pathList, false, false).Demand();
                str2 = Path.Combine(str2, directoryName);
            }
            string str4 = Path.InternalCombine(fullPathInternal, searchPattern);
            char ch = str4[str4.Length - 1];
            if (((ch == Path.DirectorySeparatorChar) || (ch == Path.AltDirectorySeparatorChar)) || (ch == Path.VolumeSeparatorChar))
            {
                str4 = str4 + '*';
            }
            fullPathInternal = Path.GetDirectoryName(str4);
            bool flag = false;
            bool flag2 = false;
            ch = fullPathInternal[fullPathInternal.Length - 1];
            if ((ch == Path.DirectorySeparatorChar) || (ch == Path.AltDirectorySeparatorChar))
            {
                str5 = str4.Substring(fullPathInternal.Length);
            }
            else
            {
                str5 = str4.Substring(fullPathInternal.Length + 1);
            }
            Win32Native.WIN32_FIND_DATA win_find_data = new Win32Native.WIN32_FIND_DATA();
            SafeFindHandle hndFindFile = null;
            SearchData data = new SearchData(fullPathInternal, str2, searchOption);
            List<SearchData> list = new List<SearchData> {
                data
            };
            List<string> list2 = new List<string>();
            int num2 = 0;
            int length = 0;
            string[] sourceArray = new string[10];
            int newMode = Win32Native.SetErrorMode(1);
            try
            {
                while (list.Count > 0)
                {
                    string str6;
                    data = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                    ch = data.fullPath[data.fullPath.Length - 1];
                    flag = (ch == Path.DirectorySeparatorChar) || (ch == Path.AltDirectorySeparatorChar);
                    if (data.userPath.Length > 0)
                    {
                        ch = data.userPath[data.userPath.Length - 1];
                        flag2 = (ch == Path.DirectorySeparatorChar) || (ch == Path.AltDirectorySeparatorChar);
                    }
                    if (data.searchOption != SearchOption.TopDirectoryOnly)
                    {
                        try
                        {
                            if (flag)
                            {
                                str6 = data.fullPath + "*";
                            }
                            else
                            {
                                str6 = data.fullPath + Path.DirectorySeparatorChar + "*";
                            }
                            hndFindFile = Win32Native.FindFirstFile(str6, win_find_data);
                            if (hndFindFile.IsInvalid)
                            {
                                errorCode = Marshal.GetLastWin32Error();
                                if (errorCode == 2)
                                {
                                    continue;
                                }
                                __Error.WinIOError(errorCode, data.fullPath);
                            }
                            do
                            {
                                if ((((win_find_data.dwFileAttributes & 0x10) != 0) && !win_find_data.cFileName.Equals(".")) && !win_find_data.cFileName.Equals(".."))
                                {
                                    SearchData item = new SearchData();
                                    StringBuilder builder = new StringBuilder(data.fullPath);
                                    if (!flag)
                                    {
                                        builder.Append(Path.DirectorySeparatorChar);
                                    }
                                    builder.Append(win_find_data.cFileName);
                                    item.fullPath = builder.ToString();
                                    builder.Length = 0;
                                    builder.Append(data.userPath);
                                    if (!flag2)
                                    {
                                        builder.Append(Path.DirectorySeparatorChar);
                                    }
                                    builder.Append(win_find_data.cFileName);
                                    item.userPath = builder.ToString();
                                    item.searchOption = data.searchOption;
                                    list.Add(item);
                                }
                            }
                            while (Win32Native.FindNextFile(hndFindFile, win_find_data));
                        }
                        finally
                        {
                            if (hndFindFile != null)
                            {
                                hndFindFile.Dispose();
                            }
                        }
                    }
                    try
                    {
                        if (flag)
                        {
                            str6 = data.fullPath + str5;
                        }
                        else
                        {
                            str6 = data.fullPath + Path.DirectorySeparatorChar + str5;
                        }
                        hndFindFile = Win32Native.FindFirstFile(str6, win_find_data);
                        if (hndFindFile.IsInvalid)
                        {
                            errorCode = Marshal.GetLastWin32Error();
                            if (errorCode == 2)
                            {
                                continue;
                            }
                            __Error.WinIOError(errorCode, data.fullPath);
                        }
                        num2 = 0;
                        do
                        {
                            bool flag3 = false;
                            if (includeFiles)
                            {
                                flag3 = 0 == (win_find_data.dwFileAttributes & 0x10);
                            }
                            if ((includeDirs && ((win_find_data.dwFileAttributes & 0x10) != 0)) && (!win_find_data.cFileName.Equals(".") && !win_find_data.cFileName.Equals("..")))
                            {
                                flag3 = true;
                            }
                            if (flag3)
                            {
                                num2++;
                                if (length == sourceArray.Length)
                                {
                                    string[] strArray3 = new string[sourceArray.Length * 2];
                                    Array.Copy(sourceArray, 0, strArray3, 0, length);
                                    sourceArray = strArray3;
                                }
                                sourceArray[length++] = Path.InternalCombine(data.userPath, win_find_data.cFileName);
                            }
                        }
                        while (Win32Native.FindNextFile(hndFindFile, win_find_data));
                        errorCode = Marshal.GetLastWin32Error();
                        if (num2 > 0)
                        {
                            list2.Add(GetDemandDir(data.fullPath, true));
                        }
                        continue;
                    }
                    finally
                    {
                        if (hndFindFile != null)
                        {
                            hndFindFile.Dispose();
                        }
                    }
                }
            }
            finally
            {
                Win32Native.SetErrorMode(newMode);
            }
            if (((errorCode != 0) && (errorCode != 0x12)) && (errorCode != 2))
            {
                __Error.WinIOError(errorCode, data.fullPath);
            }
            if (list2.Count > 0)
            {
                pathList = new string[list2.Count];
                list2.CopyTo(pathList, 0);
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, pathList, false, false).Demand();
            }
            if (length == sourceArray.Length)
            {
                return sourceArray;
            }
            string[] destinationArray = new string[length];
            Array.Copy(sourceArray, 0, destinationArray, 0, length);
            return destinationArray;
        }

        public static void Move(string sourceDirName, string destDirName)
        {
            if (sourceDirName == null)
            {
                throw new ArgumentNullException("sourceDirName");
            }
            if (sourceDirName.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "sourceDirName");
            }
            if (destDirName == null)
            {
                throw new ArgumentNullException("destDirName");
            }
            if (destDirName.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destDirName");
            }
            string fullPathInternal = Path.GetFullPathInternal(sourceDirName);
            string demandDir = GetDemandDir(fullPathInternal, false);
            if (demandDir.Length >= 0xf9)
            {
                throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
            }
            string fullPath = Path.GetFullPathInternal(destDirName);
            string str4 = GetDemandDir(fullPath, false);
            if (str4.Length >= 0xf9)
            {
                throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
            }
            new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, new string[] { demandDir }, false, false).Demand();
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { str4 }, false, false).Demand();
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(demandDir, str4, CompareOptions.IgnoreCase) == 0)
            {
                throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustBeDifferent"));
            }
            string pathRoot = Path.GetPathRoot(demandDir);
            string str6 = Path.GetPathRoot(str4);
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(pathRoot, str6, CompareOptions.IgnoreCase) != 0)
            {
                throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustHaveSameRoot"));
            }
            if (Environment.IsWin9X() && !InternalExists(Path.GetPathRoot(fullPath)))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("IO.PathNotFound_Path"), new object[] { destDirName }));
            }
            if (!Win32Native.MoveFile(sourceDirName, destDirName))
            {
                int errorCode = Marshal.GetLastWin32Error();
                switch (errorCode)
                {
                    case 2:
                        errorCode = 3;
                        __Error.WinIOError(errorCode, fullPathInternal);
                        break;

                    case 5:
                        throw new IOException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("UnauthorizedAccess_IODenied_Path"), new object[] { sourceDirName }), Win32Native.MakeHRFromErrorCode(errorCode));
                }
                __Error.WinIOError(errorCode, string.Empty);
            }
        }

        private static SafeFileHandle OpenHandle(string path)
        {
            string fullPathInternal = Path.GetFullPathInternal(path);
            string pathRoot = Path.GetPathRoot(fullPathInternal);
            if ((pathRoot == fullPathInternal) && (pathRoot[1] == Path.VolumeSeparatorChar))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_PathIsVolume"));
            }
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { GetDemandDir(fullPathInternal, true) }, false, false).Demand();
            SafeFileHandle handle = Win32Native.SafeCreateFile(fullPathInternal, 0x40000000, FileShare.Delete | FileShare.Write, null, FileMode.Open, 0x2000000, Win32Native.NULL);
            if (handle.IsInvalid)
            {
                __Error.WinIOError(Marshal.GetLastWin32Error(), fullPathInternal);
            }
            return handle;
        }

        public static void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            if (directorySecurity == null)
            {
                throw new ArgumentNullException("directorySecurity");
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            directorySecurity.Persist(fullPathInternal);
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            SetCreationTimeUtc(path, creationTime.ToUniversalTime());
        }

        public static unsafe void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            if ((Environment.OSInfo & Environment.OSName.WinNT) == Environment.OSName.WinNT)
            {
                using (SafeFileHandle handle = OpenHandle(path))
                {
                    Win32Native.FILE_TIME creationTime = new Win32Native.FILE_TIME(creationTimeUtc.ToFileTimeUtc());
                    if (!Win32Native.SetFileTime(handle, &creationTime, null, null))
                    {
                        __Error.WinIOError(Marshal.GetLastWin32Error(), path);
                    }
                }
            }
        }

        public static void SetCurrentDirectory(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("value");
            }
            if (path.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"));
            }
            if (path.Length >= 260)
            {
                throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
            }
            new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
            string fullPathInternal = Path.GetFullPathInternal(path);
            if (Environment.IsWin9X() && !InternalExists(Path.GetPathRoot(fullPathInternal)))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("IO.PathNotFound_Path"), new object[] { path }));
            }
            if (!Win32Native.SetCurrentDirectory(fullPathInternal))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 2)
                {
                    errorCode = 3;
                }
                __Error.WinIOError(errorCode, fullPathInternal);
            }
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
        }

        public static unsafe void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            if ((Environment.OSInfo & Environment.OSName.WinNT) == Environment.OSName.WinNT)
            {
                using (SafeFileHandle handle = OpenHandle(path))
                {
                    Win32Native.FILE_TIME lastAccessTime = new Win32Native.FILE_TIME(lastAccessTimeUtc.ToFileTimeUtc());
                    if (!Win32Native.SetFileTime(handle, null, &lastAccessTime, null))
                    {
                        __Error.WinIOError(Marshal.GetLastWin32Error(), path);
                    }
                }
            }
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
        }

        public static unsafe void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            if ((Environment.OSInfo & Environment.OSName.WinNT) == Environment.OSName.WinNT)
            {
                using (SafeFileHandle handle = OpenHandle(path))
                {
                    Win32Native.FILE_TIME lastWriteTime = new Win32Native.FILE_TIME(lastWriteTimeUtc.ToFileTimeUtc());
                    if (!Win32Native.SetFileTime(handle, null, null, &lastWriteTime))
                    {
                        __Error.WinIOError(Marshal.GetLastWin32Error(), path);
                    }
                }
            }
        }

        private sealed class SearchData
        {
            public string fullPath;
            public SearchOption searchOption;
            public string userPath;

            public SearchData()
            {
            }

            public SearchData(string fullPath, string userPath, SearchOption searchOption)
            {
                this.fullPath = fullPath;
                this.userPath = userPath;
                this.searchOption = searchOption;
            }
        }
    }
}

