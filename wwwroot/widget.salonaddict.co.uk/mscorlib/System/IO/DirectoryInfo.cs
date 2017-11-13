namespace System.IO
{
    using Microsoft.Win32;
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.AccessControl;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class DirectoryInfo : FileSystemInfo
    {
        private string[] demandDir;

        public DirectoryInfo(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            if ((path.Length == 2) && (path[1] == ':'))
            {
                base.OriginalPath = ".";
            }
            else
            {
                base.OriginalPath = path;
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            this.demandDir = new string[] { Directory.GetDemandDir(fullPathInternal, true) };
            new FileIOPermission(FileIOPermissionAccess.Read, this.demandDir, false, false).Demand();
            base.FullPath = fullPathInternal;
        }

        private DirectoryInfo(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.demandDir = new string[] { Directory.GetDemandDir(base.FullPath, true) };
            new FileIOPermission(FileIOPermissionAccess.Read, this.demandDir, false, false).Demand();
        }

        internal DirectoryInfo(string fullPath, bool junk)
        {
            base.OriginalPath = Path.GetFileName(fullPath);
            base.FullPath = fullPath;
            this.demandDir = new string[] { Directory.GetDemandDir(fullPath, true) };
        }

        public void Create()
        {
            Directory.InternalCreateDirectory(base.FullPath, base.OriginalPath, null);
        }

        public void Create(DirectorySecurity directorySecurity)
        {
            Directory.InternalCreateDirectory(base.FullPath, base.OriginalPath, directorySecurity);
        }

        public DirectoryInfo CreateSubdirectory(string path) => 
            this.CreateSubdirectory(path, null);

        public DirectoryInfo CreateSubdirectory(string path, DirectorySecurity directorySecurity)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            string fullPathInternal = Path.GetFullPathInternal(Path.InternalCombine(base.FullPath, path));
            if (string.Compare(base.FullPath, 0, fullPathInternal, 0, base.FullPath.Length, StringComparison.OrdinalIgnoreCase) != 0)
            {
                string displayablePath = __Error.GetDisplayablePath(base.OriginalPath, false);
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidSubPath"), new object[] { path, displayablePath }));
            }
            string demandDir = Directory.GetDemandDir(fullPathInternal, true);
            new FileIOPermission(FileIOPermissionAccess.Write, new string[] { demandDir }, false, false).Demand();
            Directory.InternalCreateDirectory(fullPathInternal, path, directorySecurity);
            return new DirectoryInfo(fullPathInternal);
        }

        public override void Delete()
        {
            Directory.Delete(base.FullPath, base.OriginalPath, false);
        }

        public void Delete(bool recursive)
        {
            Directory.Delete(base.FullPath, base.OriginalPath, recursive);
        }

        private string FixupFileDirFullPath(string fileDirUserPath)
        {
            if (base.OriginalPath.Length == 0)
            {
                return Path.InternalCombine(base.FullPath, fileDirUserPath);
            }
            if (base.OriginalPath.EndsWith(Path.DirectorySeparatorChar) || base.OriginalPath.EndsWith(Path.AltDirectorySeparatorChar))
            {
                return Path.InternalCombine(base.FullPath, fileDirUserPath.Substring(base.OriginalPath.Length));
            }
            return Path.InternalCombine(base.FullPath, fileDirUserPath.Substring(base.OriginalPath.Length + 1));
        }

        public DirectorySecurity GetAccessControl() => 
            Directory.GetAccessControl(base.FullPath, AccessControlSections.Group | AccessControlSections.Owner | AccessControlSections.Access);

        public DirectorySecurity GetAccessControl(AccessControlSections includeSections) => 
            Directory.GetAccessControl(base.FullPath, includeSections);

        public DirectoryInfo[] GetDirectories() => 
            this.GetDirectories("*");

        public DirectoryInfo[] GetDirectories(string searchPattern) => 
            this.GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);

        public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            string[] strArray = Directory.InternalGetFileDirectoryNames(base.FullPath, base.OriginalPath, searchPattern, false, true, searchOption);
            string[] pathList = new string[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = this.FixupFileDirFullPath(strArray[i]);
                pathList[i] = strArray[i] + @"\.";
            }
            if (strArray.Length != 0)
            {
                new FileIOPermission(FileIOPermissionAccess.Read, pathList, false, false).Demand();
            }
            DirectoryInfo[] infoArray = new DirectoryInfo[strArray.Length];
            for (int j = 0; j < strArray.Length; j++)
            {
                infoArray[j] = new DirectoryInfo(strArray[j], false);
            }
            return infoArray;
        }

        public FileInfo[] GetFiles() => 
            this.GetFiles("*");

        public FileInfo[] GetFiles(string searchPattern) => 
            this.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);

        public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            string[] pathList = Directory.InternalGetFileDirectoryNames(base.FullPath, base.OriginalPath, searchPattern, true, false, searchOption);
            for (int i = 0; i < pathList.Length; i++)
            {
                pathList[i] = this.FixupFileDirFullPath(pathList[i]);
            }
            if (pathList.Length != 0)
            {
                new FileIOPermission(FileIOPermissionAccess.Read, pathList, false, false).Demand();
            }
            FileInfo[] infoArray = new FileInfo[pathList.Length];
            for (int j = 0; j < pathList.Length; j++)
            {
                infoArray[j] = new FileInfo(pathList[j], false);
            }
            return infoArray;
        }

        public FileSystemInfo[] GetFileSystemInfos() => 
            this.GetFileSystemInfos("*");

        public FileSystemInfo[] GetFileSystemInfos(string searchPattern) => 
            this.GetFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);

        private FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            string[] strArray = Directory.InternalGetFileDirectoryNames(base.FullPath, base.OriginalPath, searchPattern, false, true, searchOption);
            string[] pathList = Directory.InternalGetFileDirectoryNames(base.FullPath, base.OriginalPath, searchPattern, true, false, searchOption);
            FileSystemInfo[] infoArray = new FileSystemInfo[strArray.Length + pathList.Length];
            string[] strArray3 = new string[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = this.FixupFileDirFullPath(strArray[i]);
                strArray3[i] = strArray[i] + @"\.";
            }
            if (strArray.Length != 0)
            {
                new FileIOPermission(FileIOPermissionAccess.Read, strArray3, false, false).Demand();
            }
            for (int j = 0; j < pathList.Length; j++)
            {
                pathList[j] = this.FixupFileDirFullPath(pathList[j]);
            }
            if (pathList.Length != 0)
            {
                new FileIOPermission(FileIOPermissionAccess.Read, pathList, false, false).Demand();
            }
            int num3 = 0;
            for (int k = 0; k < strArray.Length; k++)
            {
                infoArray[num3++] = new DirectoryInfo(strArray[k], false);
            }
            for (int m = 0; m < pathList.Length; m++)
            {
                infoArray[num3++] = new FileInfo(pathList[m], false);
            }
            return infoArray;
        }

        public void MoveTo(string destDirName)
        {
            string fullPath;
            if (destDirName == null)
            {
                throw new ArgumentNullException("destDirName");
            }
            if (destDirName.Length == 0)
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destDirName");
            }
            new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, this.demandDir, false, false).Demand();
            string fullPathInternal = Path.GetFullPathInternal(destDirName);
            if (!fullPathInternal.EndsWith(Path.DirectorySeparatorChar))
            {
                fullPathInternal = fullPathInternal + Path.DirectorySeparatorChar;
            }
            string path = fullPathInternal + '.';
            new FileIOPermission(FileIOPermissionAccess.Write | FileIOPermissionAccess.Read, path).Demand();
            if (base.FullPath.EndsWith(Path.DirectorySeparatorChar))
            {
                fullPath = base.FullPath;
            }
            else
            {
                fullPath = base.FullPath + Path.DirectorySeparatorChar;
            }
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(fullPath, fullPathInternal, CompareOptions.IgnoreCase) == 0)
            {
                throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustBeDifferent"));
            }
            string pathRoot = Path.GetPathRoot(fullPath);
            string str5 = Path.GetPathRoot(fullPathInternal);
            if (CultureInfo.InvariantCulture.CompareInfo.Compare(pathRoot, str5, CompareOptions.IgnoreCase) != 0)
            {
                throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustHaveSameRoot"));
            }
            if (Environment.IsWin9X() && !Directory.InternalExists(base.FullPath))
            {
                throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("IO.PathNotFound_Path"), new object[] { destDirName }));
            }
            if (!Win32Native.MoveFile(base.FullPath, destDirName))
            {
                int errorCode = Marshal.GetLastWin32Error();
                switch (errorCode)
                {
                    case 2:
                        errorCode = 3;
                        __Error.WinIOError(errorCode, base.OriginalPath);
                        break;

                    case 5:
                        throw new IOException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("UnauthorizedAccess_IODenied_Path"), new object[] { base.OriginalPath }));
                }
                __Error.WinIOError(errorCode, string.Empty);
            }
            base.FullPath = fullPathInternal;
            base.OriginalPath = destDirName;
            this.demandDir = new string[] { Directory.GetDemandDir(base.FullPath, true) };
            base._dataInitialised = -1;
        }

        public void SetAccessControl(DirectorySecurity directorySecurity)
        {
            Directory.SetAccessControl(base.FullPath, directorySecurity);
        }

        public override string ToString() => 
            base.OriginalPath;

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
                    return ((this._data.fileAttributes != -1) && ((this._data.fileAttributes & 0x10) != 0));
                }
                catch
                {
                    return false;
                }
            }
        }

        public override string Name
        {
            get
            {
                string fullPath = base.FullPath;
                if (fullPath.Length <= 3)
                {
                    return base.FullPath;
                }
                if (fullPath.EndsWith(Path.DirectorySeparatorChar))
                {
                    fullPath = base.FullPath.Substring(0, base.FullPath.Length - 1);
                }
                return Path.GetFileName(fullPath);
            }
        }

        public DirectoryInfo Parent
        {
            get
            {
                string fullPath = base.FullPath;
                if ((fullPath.Length > 3) && fullPath.EndsWith(Path.DirectorySeparatorChar))
                {
                    fullPath = base.FullPath.Substring(0, base.FullPath.Length - 1);
                }
                string directoryName = Path.GetDirectoryName(fullPath);
                if (directoryName == null)
                {
                    return null;
                }
                DirectoryInfo info = new DirectoryInfo(directoryName, false);
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read, info.demandDir, false, false).Demand();
                return info;
            }
        }

        public DirectoryInfo Root
        {
            get
            {
                int rootLength = Path.GetRootLength(base.FullPath);
                string fullPath = base.FullPath.Substring(0, rootLength);
                string demandDir = Directory.GetDemandDir(fullPath, true);
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, new string[] { demandDir }, false, false).Demand();
                return new DirectoryInfo(fullPath);
            }
        }
    }
}

