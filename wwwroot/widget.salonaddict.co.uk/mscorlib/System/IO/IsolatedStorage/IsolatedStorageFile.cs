namespace System.IO.IsolatedStorage
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Text;
    using System.Threading;

    [ComVisible(true)]
    public sealed class IsolatedStorageFile : System.IO.IsolatedStorage.IsolatedStorage, IDisposable
    {
        private bool m_bDisposed;
        private bool m_closed;
        private FileIOPermission m_fiop;
        private IntPtr m_handle;
        private string m_InfoFile;
        private string m_RootDir;
        private string m_SyncObjectName;
        private static string s_appDataDir;
        internal const string s_AppFiles = "AppFiles";
        internal const string s_AppInfoFile = "appinfo.dat";
        internal const string s_AssemFiles = "AssemFiles";
        private const int s_BlockSize = 0x400;
        private const int s_DirSize = 0x400;
        internal const string s_Files = "Files";
        internal const string s_IDFile = "identity.dat";
        internal const string s_InfoFile = "info.dat";
        private const string s_name = "file.store";
        private static IsolatedStorageFilePermission s_PermAdminUser;
        private static FileIOPermission s_PermMachine;
        private static FileIOPermission s_PermRoaming;
        private static FileIOPermission s_PermUser;
        private static string s_RootDirMachine;
        private static string s_RootDirRoaming;
        private static string s_RootDirUser;

        internal IsolatedStorageFile()
        {
        }

        public void Close()
        {
            if (!base.IsRoaming())
            {
                lock (this)
                {
                    if (!this.m_closed)
                    {
                        this.m_closed = true;
                        IntPtr handle = this.m_handle;
                        this.m_handle = Win32Native.NULL;
                        nClose(handle);
                        GC.nativeSuppressFinalize(this);
                    }
                }
            }
        }

        private bool ContainsUnknownFiles(string rootDir)
        {
            string[] strArray;
            string[] strArray2;
            try
            {
                strArray2 = GetFileDirectoryNames(rootDir + "*", "*", true);
                strArray = GetFileDirectoryNames(rootDir + "*", "*", false);
            }
            catch
            {
                throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectories"));
            }
            if ((strArray != null) && (strArray.Length > 0))
            {
                if (strArray.Length > 1)
                {
                    return true;
                }
                if (base.IsApp())
                {
                    if (NotAppFilesDir(strArray[0]))
                    {
                        return true;
                    }
                }
                else if (base.IsDomain())
                {
                    if (NotFilesDir(strArray[0]))
                    {
                        return true;
                    }
                }
                else if (NotAssemFilesDir(strArray[0]))
                {
                    return true;
                }
            }
            if ((strArray2 == null) || (strArray2.Length == 0))
            {
                return false;
            }
            if (base.IsRoaming())
            {
                if ((strArray2.Length <= 1) && !NotIDFile(strArray2[0]))
                {
                    return false;
                }
                return true;
            }
            if (((strArray2.Length <= 2) && (!NotIDFile(strArray2[0]) || !NotInfoFile(strArray2[0]))) && (((strArray2.Length != 2) || !NotIDFile(strArray2[1])) || !NotInfoFile(strArray2[1])))
            {
                return false;
            }
            return true;
        }

        public void CreateDirectory(string dir)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            string fullPath = this.GetFullPath(dir);
            string fullPathInternal = Path.GetFullPathInternal(fullPath);
            string[] strArray = this.DirectoriesToCreate(fullPathInternal);
            if ((strArray == null) || (strArray.Length == 0))
            {
                if (!Directory.Exists(fullPath))
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_CreateDirectory"));
                }
            }
            else
            {
                this.Reserve((ulong) (0x400L * strArray.Length));
                this.m_fiop.Assert();
                this.m_fiop.PermitOnly();
                try
                {
                    Directory.CreateDirectory(strArray[strArray.Length - 1]);
                }
                catch
                {
                    this.Unreserve((ulong) (0x400L * strArray.Length));
                    Directory.Delete(strArray[0], true);
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_CreateDirectory"));
                }
                CodeAccessPermission.RevertAll();
            }
        }

        internal void CreateIDFile(string path, IsolatedStorageScope scope)
        {
            try
            {
                using (FileStream stream = new FileStream(path + "identity.dat", FileMode.OpenOrCreate))
                {
                    MemoryStream identityStream = base.GetIdentityStream(scope);
                    byte[] buffer = identityStream.GetBuffer();
                    stream.Write(buffer, 0, (int) identityStream.Length);
                    identityStream.Close();
                }
            }
            catch
            {
            }
        }

        internal static Mutex CreateMutexNotOwned(string pathName) => 
            new Mutex(false, @"Global\" + GetStrongHashSuitableForObjectName(pathName));

        internal static string CreateRandomDirectory(string rootDir)
        {
            string str = Path.GetRandomFileName() + @"\" + Path.GetRandomFileName();
            try
            {
                Directory.CreateDirectory(rootDir + str);
            }
            catch
            {
                throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
            }
            return str;
        }

        public void DeleteDirectory(string dir)
        {
            if (dir == null)
            {
                throw new ArgumentNullException("dir");
            }
            this.m_fiop.Assert();
            this.m_fiop.PermitOnly();
            this.Lock();
            try
            {
                try
                {
                    new DirectoryInfo(this.GetFullPath(dir)).Delete(false);
                }
                catch
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectory"));
                }
                this.Unreserve(0x400L);
            }
            finally
            {
                this.Unlock();
            }
            CodeAccessPermission.RevertAll();
        }

        public void DeleteFile(string file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }
            this.m_fiop.Assert();
            this.m_fiop.PermitOnly();
            FileInfo info = new FileInfo(this.GetFullPath(file));
            long length = 0L;
            this.Lock();
            try
            {
                try
                {
                    length = info.Length;
                    info.Delete();
                }
                catch
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteFile"));
                }
                this.Unreserve(RoundToBlockSize((ulong) length));
            }
            finally
            {
                this.Unlock();
            }
            CodeAccessPermission.RevertAll();
        }

        private static void DemandAdminPermission()
        {
            if (s_PermAdminUser == null)
            {
                s_PermAdminUser = new IsolatedStorageFilePermission(IsolatedStorageContainment.AdministerIsolatedStorageByUser, 0L, false);
            }
            s_PermAdminUser.Demand();
        }

        private string[] DirectoriesToCreate(string fullPath)
        {
            ArrayList list = new ArrayList();
            int length = fullPath.Length;
            if ((length >= 2) && (fullPath[length - 1] == this.SeparatorExternal))
            {
                length--;
            }
            int rootLength = Path.GetRootLength(fullPath);
            while (rootLength < length)
            {
                rootLength++;
                while ((rootLength < length) && (fullPath[rootLength] != this.SeparatorExternal))
                {
                    rootLength++;
                }
                string path = fullPath.Substring(0, rootLength);
                if (!Directory.InternalExists(path))
                {
                    list.Add(path);
                }
            }
            if (list.Count != 0)
            {
                return (string[]) list.ToArray(typeof(string));
            }
            return null;
        }

        public void Dispose()
        {
            this.Close();
            this.m_bDisposed = true;
        }

        ~IsolatedStorageFile()
        {
            this.Dispose();
        }

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
        private static string GetDataDirectoryFromActivationContext()
        {
            if (s_appDataDir == null)
            {
                ActivationContext activationContext = AppDomain.CurrentDomain.ActivationContext;
                if (activationContext == null)
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_ApplicationMissingIdentity"));
                }
                string dataDirectory = activationContext.DataDirectory;
                if ((dataDirectory != null) && (dataDirectory[dataDirectory.Length - 1] != '\\'))
                {
                    dataDirectory = dataDirectory + @"\";
                }
                s_appDataDir = dataDirectory;
            }
            return s_appDataDir;
        }

        public string[] GetDirectoryNames(string searchPattern)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            this.m_fiop.Assert();
            this.m_fiop.PermitOnly();
            string[] strArray = GetFileDirectoryNames(this.GetFullPath(searchPattern), searchPattern, false);
            CodeAccessPermission.RevertAll();
            return strArray;
        }

        public static IEnumerator GetEnumerator(IsolatedStorageScope scope)
        {
            VerifyGlobalScope(scope);
            DemandAdminPermission();
            return new IsolatedStorageFileEnumerator(scope);
        }

        private static string[] GetFileDirectoryNames(string path, string msg, bool file)
        {
            int num;
            if (path == null)
            {
                throw new ArgumentNullException("path", Environment.GetResourceString("ArgumentNull_Path"));
            }
            bool flag = false;
            char ch = path[path.Length - 1];
            if (((ch == Path.DirectorySeparatorChar) || (ch == Path.AltDirectorySeparatorChar)) || (ch == '.'))
            {
                flag = true;
            }
            string fullPathInternal = Path.GetFullPathInternal(path);
            if (flag && (fullPathInternal[fullPathInternal.Length - 1] != ch))
            {
                fullPathInternal = fullPathInternal + @"\*";
            }
            string directoryName = Path.GetDirectoryName(fullPathInternal);
            if (directoryName != null)
            {
                directoryName = directoryName + @"\";
            }
            new FileIOPermission(FileIOPermissionAccess.Read, (directoryName == null) ? fullPathInternal : directoryName).Demand();
            string[] sourceArray = new string[10];
            int length = 0;
            Win32Native.WIN32_FIND_DATA data = new Win32Native.WIN32_FIND_DATA();
            SafeFindHandle hndFindFile = Win32Native.FindFirstFile(fullPathInternal, data);
            if (hndFindFile.IsInvalid)
            {
                num = Marshal.GetLastWin32Error();
                if (num == 2)
                {
                    return new string[0];
                }
                __Error.WinIOError(num, msg);
            }
            int num3 = 0;
            do
            {
                bool flag2;
                if (file)
                {
                    flag2 = 0 == (data.dwFileAttributes & 0x10);
                }
                else
                {
                    flag2 = 0 != (data.dwFileAttributes & 0x10);
                    if (flag2 && (data.cFileName.Equals(".") || data.cFileName.Equals("..")))
                    {
                        flag2 = false;
                    }
                }
                if (flag2)
                {
                    num3++;
                    if (length == sourceArray.Length)
                    {
                        string[] strArray2 = new string[sourceArray.Length * 2];
                        Array.Copy(sourceArray, 0, strArray2, 0, length);
                        sourceArray = strArray2;
                    }
                    sourceArray[length++] = data.cFileName;
                }
            }
            while (Win32Native.FindNextFile(hndFindFile, data));
            num = Marshal.GetLastWin32Error();
            hndFindFile.Close();
            if ((num != 0) && (num != 0x12))
            {
                __Error.WinIOError(num, msg);
            }
            if ((!file && (num3 == 1)) && ((data.dwFileAttributes & 0x10) != 0))
            {
                return new string[] { data.cFileName };
            }
            if (length == sourceArray.Length)
            {
                return sourceArray;
            }
            string[] destinationArray = new string[length];
            Array.Copy(sourceArray, 0, destinationArray, 0, length);
            return destinationArray;
        }

        public string[] GetFileNames(string searchPattern)
        {
            if (searchPattern == null)
            {
                throw new ArgumentNullException("searchPattern");
            }
            this.m_fiop.Assert();
            this.m_fiop.PermitOnly();
            string[] strArray = GetFileDirectoryNames(this.GetFullPath(searchPattern), searchPattern, true);
            CodeAccessPermission.RevertAll();
            return strArray;
        }

        internal string GetFullPath(string path)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.RootDirectory);
            if (path[0] == this.SeparatorExternal)
            {
                builder.Append(path.Substring(1));
            }
            else
            {
                builder.Append(path);
            }
            return builder.ToString();
        }

        internal static FileIOPermission GetGlobalFileIOPerm(IsolatedStorageScope scope)
        {
            if (System.IO.IsolatedStorage.IsolatedStorage.IsRoaming(scope))
            {
                if (s_PermRoaming == null)
                {
                    s_PermRoaming = new FileIOPermission(FileIOPermissionAccess.AllAccess, GetRootDir(scope));
                }
                return s_PermRoaming;
            }
            if (System.IO.IsolatedStorage.IsolatedStorage.IsMachine(scope))
            {
                if (s_PermMachine == null)
                {
                    s_PermMachine = new FileIOPermission(FileIOPermissionAccess.AllAccess, GetRootDir(scope));
                }
                return s_PermMachine;
            }
            if (s_PermUser == null)
            {
                s_PermUser = new FileIOPermission(FileIOPermissionAccess.AllAccess, GetRootDir(scope));
            }
            return s_PermUser;
        }

        internal static string GetMachineRandomDirectory(string rootDir)
        {
            string[] strArray = GetFileDirectoryNames(rootDir + "*", "*", false);
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Length == 12)
                {
                    string[] strArray2 = GetFileDirectoryNames(rootDir + strArray[i] + @"\*", "*", false);
                    for (int j = 0; j < strArray2.Length; j++)
                    {
                        if (strArray2[j].Length == 12)
                        {
                            return (strArray[i] + @"\" + strArray2[j]);
                        }
                    }
                }
            }
            return null;
        }

        public static IsolatedStorageFile GetMachineStoreForApplication() => 
            GetStore(IsolatedStorageScope.Application | IsolatedStorageScope.Machine, (Type) null);

        public static IsolatedStorageFile GetMachineStoreForAssembly() => 
            GetStore(IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly, (Type) null, (Type) null);

        public static IsolatedStorageFile GetMachineStoreForDomain() => 
            GetStore(IsolatedStorageScope.Machine | IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain, (Type) null, (Type) null);

        protected override IsolatedStoragePermission GetPermission(PermissionSet ps)
        {
            if (ps == null)
            {
                return null;
            }
            if (ps.IsUnrestricted())
            {
                return new IsolatedStorageFilePermission(PermissionState.Unrestricted);
            }
            return (IsolatedStoragePermission) ps.GetPermission(typeof(IsolatedStorageFilePermission));
        }

        internal static string GetRandomDirectory(string rootDir, out bool bMigrateNeeded, out string sOldStoreLocation)
        {
            bMigrateNeeded = false;
            sOldStoreLocation = null;
            string[] strArray = GetFileDirectoryNames(rootDir + "*", "*", false);
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray[i].Length == 12)
                {
                    string[] strArray2 = GetFileDirectoryNames(rootDir + strArray[i] + @"\*", "*", false);
                    for (int k = 0; k < strArray2.Length; k++)
                    {
                        if (strArray2[k].Length == 12)
                        {
                            return (strArray[i] + @"\" + strArray2[k]);
                        }
                    }
                }
            }
            for (int j = 0; j < strArray.Length; j++)
            {
                if (strArray[j].Length == 0x18)
                {
                    bMigrateNeeded = true;
                    sOldStoreLocation = strArray[j];
                    return null;
                }
            }
            return null;
        }

        internal static string GetRootDir(IsolatedStorageScope scope)
        {
            if (System.IO.IsolatedStorage.IsolatedStorage.IsRoaming(scope))
            {
                if (s_RootDirRoaming == null)
                {
                    s_RootDirRoaming = nGetRootDir(scope);
                }
                return s_RootDirRoaming;
            }
            if (System.IO.IsolatedStorage.IsolatedStorage.IsMachine(scope))
            {
                if (s_RootDirMachine == null)
                {
                    InitGlobalsMachine(scope);
                }
                return s_RootDirMachine;
            }
            if (s_RootDirUser == null)
            {
                InitGlobalsNonRoamingUser(scope);
            }
            return s_RootDirUser;
        }

        public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object applicationIdentity)
        {
            if (applicationIdentity == null)
            {
                throw new ArgumentNullException("applicationIdentity");
            }
            DemandAdminPermission();
            IsolatedStorageFile file = new IsolatedStorageFile();
            file.InitStore(scope, null, null, applicationIdentity);
            file.Init(scope);
            return file;
        }

        public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type applicationEvidenceType)
        {
            if (applicationEvidenceType != null)
            {
                DemandAdminPermission();
            }
            IsolatedStorageFile file = new IsolatedStorageFile();
            file.InitStore(scope, applicationEvidenceType);
            file.Init(scope);
            return file;
        }

        public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, object domainIdentity, object assemblyIdentity)
        {
            if (System.IO.IsolatedStorage.IsolatedStorage.IsDomain(scope) && (domainIdentity == null))
            {
                throw new ArgumentNullException("domainIdentity");
            }
            if (assemblyIdentity == null)
            {
                throw new ArgumentNullException("assemblyIdentity");
            }
            DemandAdminPermission();
            IsolatedStorageFile file = new IsolatedStorageFile();
            file.InitStore(scope, domainIdentity, assemblyIdentity, null);
            file.Init(scope);
            return file;
        }

        public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Type domainEvidenceType, Type assemblyEvidenceType)
        {
            if (domainEvidenceType != null)
            {
                DemandAdminPermission();
            }
            IsolatedStorageFile file = new IsolatedStorageFile();
            file.InitStore(scope, domainEvidenceType, assemblyEvidenceType);
            file.Init(scope);
            return file;
        }

        public static IsolatedStorageFile GetStore(IsolatedStorageScope scope, Evidence domainEvidence, Type domainEvidenceType, Evidence assemblyEvidence, Type assemblyEvidenceType)
        {
            if (System.IO.IsolatedStorage.IsolatedStorage.IsDomain(scope) && (domainEvidence == null))
            {
                throw new ArgumentNullException("domainEvidence");
            }
            if (assemblyEvidence == null)
            {
                throw new ArgumentNullException("assemblyEvidence");
            }
            DemandAdminPermission();
            IsolatedStorageFile file = new IsolatedStorageFile();
            file.InitStore(scope, domainEvidence, domainEvidenceType, assemblyEvidence, assemblyEvidenceType, null, null);
            file.Init(scope);
            return file;
        }

        internal static string GetStrongHashSuitableForObjectName(string name)
        {
            MemoryStream output = new MemoryStream();
            new BinaryWriter(output).Write(name.ToUpper(CultureInfo.InvariantCulture));
            output.Position = 0L;
            return System.IO.IsolatedStorage.IsolatedStorage.ToBase32StringSuitableForDirName(new SHA1CryptoServiceProvider().ComputeHash(output));
        }

        private string GetSyncObjectName()
        {
            if (this.m_SyncObjectName == null)
            {
                this.m_SyncObjectName = GetStrongHashSuitableForObjectName(this.m_InfoFile);
            }
            return this.m_SyncObjectName;
        }

        public static IsolatedStorageFile GetUserStoreForApplication() => 
            GetStore(IsolatedStorageScope.Application | IsolatedStorageScope.User, (Type) null);

        public static IsolatedStorageFile GetUserStoreForAssembly() => 
            GetStore(IsolatedStorageScope.Assembly | IsolatedStorageScope.User, (Type) null, (Type) null);

        public static IsolatedStorageFile GetUserStoreForDomain() => 
            GetStore(IsolatedStorageScope.Assembly | IsolatedStorageScope.Domain | IsolatedStorageScope.User, (Type) null, (Type) null);

        internal void Init(IsolatedStorageScope scope)
        {
            GetGlobalFileIOPerm(scope).Assert();
            StringBuilder builder = new StringBuilder();
            if (System.IO.IsolatedStorage.IsolatedStorage.IsApp(scope))
            {
                builder.Append(GetRootDir(scope));
                if (s_appDataDir == null)
                {
                    builder.Append(base.AppName);
                    builder.Append(this.SeparatorExternal);
                }
                try
                {
                    Directory.CreateDirectory(builder.ToString());
                }
                catch
                {
                }
                this.CreateIDFile(builder.ToString(), scope);
                this.m_InfoFile = builder.ToString() + "appinfo.dat";
                builder.Append("AppFiles");
            }
            else
            {
                builder.Append(GetRootDir(scope));
                if (System.IO.IsolatedStorage.IsolatedStorage.IsDomain(scope))
                {
                    builder.Append(base.DomainName);
                    builder.Append(this.SeparatorExternal);
                    try
                    {
                        Directory.CreateDirectory(builder.ToString());
                        this.CreateIDFile(builder.ToString(), scope);
                    }
                    catch
                    {
                    }
                    this.m_InfoFile = builder.ToString() + "info.dat";
                }
                builder.Append(base.AssemName);
                builder.Append(this.SeparatorExternal);
                try
                {
                    Directory.CreateDirectory(builder.ToString());
                    this.CreateIDFile(builder.ToString(), scope);
                }
                catch
                {
                }
                if (System.IO.IsolatedStorage.IsolatedStorage.IsDomain(scope))
                {
                    builder.Append("Files");
                }
                else
                {
                    this.m_InfoFile = builder.ToString() + "info.dat";
                    builder.Append("AssemFiles");
                }
            }
            builder.Append(this.SeparatorExternal);
            string path = builder.ToString();
            try
            {
                Directory.CreateDirectory(path);
            }
            catch
            {
            }
            this.m_RootDir = path;
            this.m_fiop = new FileIOPermission(FileIOPermissionAccess.AllAccess, path);
        }

        internal bool InitExistingStore(IsolatedStorageScope scope)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetRootDir(scope));
            if (System.IO.IsolatedStorage.IsolatedStorage.IsApp(scope))
            {
                builder.Append(base.AppName);
                builder.Append(this.SeparatorExternal);
                this.m_InfoFile = builder.ToString() + "appinfo.dat";
                builder.Append("AppFiles");
            }
            else
            {
                if (System.IO.IsolatedStorage.IsolatedStorage.IsDomain(scope))
                {
                    builder.Append(base.DomainName);
                    builder.Append(this.SeparatorExternal);
                    this.m_InfoFile = builder.ToString() + "info.dat";
                }
                builder.Append(base.AssemName);
                builder.Append(this.SeparatorExternal);
                if (System.IO.IsolatedStorage.IsolatedStorage.IsDomain(scope))
                {
                    builder.Append("Files");
                }
                else
                {
                    this.m_InfoFile = builder.ToString() + "info.dat";
                    builder.Append("AssemFiles");
                }
            }
            builder.Append(this.SeparatorExternal);
            FileIOPermission permission = new FileIOPermission(FileIOPermissionAccess.AllAccess, builder.ToString());
            permission.Assert();
            if (!Directory.Exists(builder.ToString()))
            {
                return false;
            }
            this.m_RootDir = builder.ToString();
            this.m_fiop = permission;
            return true;
        }

        private static void InitGlobalsMachine(IsolatedStorageScope scope)
        {
            string path = nGetRootDir(scope);
            new FileIOPermission(FileIOPermissionAccess.AllAccess, path).Assert();
            string machineRandomDirectory = GetMachineRandomDirectory(path);
            if (machineRandomDirectory == null)
            {
                Mutex mutex = CreateMutexNotOwned(path);
                if (!mutex.WaitOne())
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
                }
                try
                {
                    machineRandomDirectory = GetMachineRandomDirectory(path);
                    if (machineRandomDirectory == null)
                    {
                        string randomFileName = Path.GetRandomFileName();
                        string str4 = Path.GetRandomFileName();
                        try
                        {
                            nCreateDirectoryWithDacl(path + randomFileName);
                            nCreateDirectoryWithDacl(path + randomFileName + @"\" + str4);
                        }
                        catch
                        {
                            throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
                        }
                        machineRandomDirectory = randomFileName + @"\" + str4;
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            s_RootDirMachine = path + machineRandomDirectory + @"\";
        }

        private static void InitGlobalsNonRoamingUser(IsolatedStorageScope scope)
        {
            string path = null;
            if (scope == (IsolatedStorageScope.Application | IsolatedStorageScope.User))
            {
                path = GetDataDirectoryFromActivationContext();
                if (path != null)
                {
                    s_RootDirUser = path;
                    return;
                }
            }
            path = nGetRootDir(scope);
            new FileIOPermission(FileIOPermissionAccess.AllAccess, path).Assert();
            bool bMigrateNeeded = false;
            string sOldStoreLocation = null;
            string str3 = GetRandomDirectory(path, out bMigrateNeeded, out sOldStoreLocation);
            if (str3 == null)
            {
                Mutex mutex = CreateMutexNotOwned(path);
                if (!mutex.WaitOne())
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
                }
                try
                {
                    str3 = GetRandomDirectory(path, out bMigrateNeeded, out sOldStoreLocation);
                    if (str3 == null)
                    {
                        if (bMigrateNeeded)
                        {
                            str3 = MigrateOldIsoStoreDirectory(path, sOldStoreLocation);
                        }
                        else
                        {
                            str3 = CreateRandomDirectory(path);
                        }
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
            s_RootDirUser = path + str3 + @"\";
        }

        internal void Lock()
        {
            if (!base.IsRoaming())
            {
                lock (this)
                {
                    if (this.m_bDisposed)
                    {
                        throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_closed)
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_handle == Win32Native.NULL)
                    {
                        this.m_handle = nOpen(this.m_InfoFile, this.GetSyncObjectName());
                    }
                    nLock(this.m_handle, true);
                }
            }
        }

        internal static string MigrateOldIsoStoreDirectory(string rootDir, string oldRandomDirectory)
        {
            string randomFileName = Path.GetRandomFileName();
            string str2 = Path.GetRandomFileName();
            string path = rootDir + randomFileName;
            string destDirName = path + @"\" + str2;
            try
            {
                Directory.CreateDirectory(path);
                Directory.Move(rootDir + oldRandomDirectory, destDirName);
            }
            catch
            {
                throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_Init"));
            }
            return (randomFileName + @"\" + str2);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void nClose(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void nCreateDirectoryWithDacl(string path);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern string nGetRootDir(IsolatedStorageScope scope);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern ulong nGetUsage(IntPtr handle);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void nLock(IntPtr handle, bool fLock);
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern IntPtr nOpen(string infoFile, string syncName);
        internal static bool NotAppFilesDir(string dir) => 
            (string.Compare(dir, "AppFiles", StringComparison.Ordinal) != 0);

        internal static bool NotAssemFilesDir(string dir) => 
            (string.Compare(dir, "AssemFiles", StringComparison.Ordinal) != 0);

        private static bool NotFilesDir(string dir) => 
            (string.Compare(dir, "Files", StringComparison.Ordinal) != 0);

        private static bool NotIDFile(string file) => 
            (string.Compare(file, "identity.dat", StringComparison.Ordinal) != 0);

        private static bool NotInfoFile(string file) => 
            ((string.Compare(file, "info.dat", StringComparison.Ordinal) != 0) && (string.Compare(file, "appinfo.dat", StringComparison.Ordinal) != 0));

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern unsafe void nReserve(IntPtr handle, ulong* plQuota, ulong* plReserve, bool fFree);
        public override void Remove()
        {
            string path = null;
            this.RemoveLogicalDir();
            this.Close();
            StringBuilder builder = new StringBuilder();
            builder.Append(GetRootDir(base.Scope));
            if (base.IsApp())
            {
                builder.Append(base.AppName);
                builder.Append(this.SeparatorExternal);
            }
            else
            {
                if (base.IsDomain())
                {
                    builder.Append(base.DomainName);
                    builder.Append(this.SeparatorExternal);
                    path = builder.ToString();
                }
                builder.Append(base.AssemName);
                builder.Append(this.SeparatorExternal);
            }
            string str = builder.ToString();
            new FileIOPermission(FileIOPermissionAccess.AllAccess, str).Assert();
            if (!this.ContainsUnknownFiles(str))
            {
                try
                {
                    Directory.Delete(str, true);
                }
                catch
                {
                    return;
                }
                if (base.IsDomain())
                {
                    CodeAccessPermission.RevertAssert();
                    new FileIOPermission(FileIOPermissionAccess.AllAccess, path).Assert();
                    if (!this.ContainsUnknownFiles(path))
                    {
                        try
                        {
                            Directory.Delete(path, true);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        public static void Remove(IsolatedStorageScope scope)
        {
            VerifyGlobalScope(scope);
            DemandAdminPermission();
            string rootDir = GetRootDir(scope);
            new FileIOPermission(FileIOPermissionAccess.Write, rootDir).Assert();
            try
            {
                Directory.Delete(rootDir, true);
                Directory.CreateDirectory(rootDir);
            }
            catch
            {
                throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectories"));
            }
        }

        private void RemoveLogicalDir()
        {
            this.m_fiop.Assert();
            this.Lock();
            try
            {
                ulong lFree = base.IsRoaming() ? ((ulong) 0L) : this.CurrentSize;
                try
                {
                    Directory.Delete(this.RootDirectory, true);
                }
                catch
                {
                    throw new IsolatedStorageException(Environment.GetResourceString("IsolatedStorage_DeleteDirectories"));
                }
                this.Unreserve(lFree);
            }
            finally
            {
                this.Unlock();
            }
        }

        internal unsafe void Reserve(ulong lReserve)
        {
            if (!base.IsRoaming())
            {
                ulong maximumSize = this.MaximumSize;
                ulong plReserve = lReserve;
                lock (this)
                {
                    if (this.m_bDisposed)
                    {
                        throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_closed)
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_handle == Win32Native.NULL)
                    {
                        this.m_handle = nOpen(this.m_InfoFile, this.GetSyncObjectName());
                    }
                    nReserve(this.m_handle, &maximumSize, &plReserve, false);
                }
            }
        }

        internal void Reserve(ulong oldLen, ulong newLen)
        {
            oldLen = RoundToBlockSize(oldLen);
            if (newLen > oldLen)
            {
                this.Reserve(RoundToBlockSize(newLen - oldLen));
            }
        }

        internal void ReserveOneBlock()
        {
            this.Reserve(0x400L);
        }

        internal static ulong RoundToBlockSize(ulong num)
        {
            if (num < 0x400L)
            {
                return 0x400L;
            }
            ulong num2 = num % ((ulong) 0x400L);
            if (num2 != 0L)
            {
                num += ((ulong) 0x400L) - num2;
            }
            return num;
        }

        internal void UndoReserveOperation(ulong oldLen, ulong newLen)
        {
            oldLen = RoundToBlockSize(oldLen);
            if (newLen > oldLen)
            {
                this.Unreserve(RoundToBlockSize(newLen - oldLen));
            }
        }

        internal void Unlock()
        {
            if (!base.IsRoaming())
            {
                lock (this)
                {
                    if (this.m_bDisposed)
                    {
                        throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_closed)
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_handle == Win32Native.NULL)
                    {
                        this.m_handle = nOpen(this.m_InfoFile, this.GetSyncObjectName());
                    }
                    nLock(this.m_handle, false);
                }
            }
        }

        internal unsafe void Unreserve(ulong lFree)
        {
            if (!base.IsRoaming())
            {
                ulong maximumSize = this.MaximumSize;
                ulong plReserve = lFree;
                lock (this)
                {
                    if (this.m_bDisposed)
                    {
                        throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_closed)
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_handle == Win32Native.NULL)
                    {
                        this.m_handle = nOpen(this.m_InfoFile, this.GetSyncObjectName());
                    }
                    nReserve(this.m_handle, &maximumSize, &plReserve, true);
                }
            }
        }

        internal void UnreserveOneBlock()
        {
            this.Unreserve(0x400L);
        }

        internal static void VerifyGlobalScope(IsolatedStorageScope scope)
        {
            if (((scope != IsolatedStorageScope.User) && (scope != (IsolatedStorageScope.Roaming | IsolatedStorageScope.User))) && (scope != IsolatedStorageScope.Machine))
            {
                throw new ArgumentException(Environment.GetResourceString("IsolatedStorage_Scope_U_R_M"));
            }
        }

        [CLSCompliant(false)]
        public override ulong CurrentSize
        {
            get
            {
                if (base.IsRoaming())
                {
                    throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_CurrentSizeUndefined"));
                }
                lock (this)
                {
                    if (this.m_bDisposed)
                    {
                        throw new ObjectDisposedException(null, Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_closed)
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("IsolatedStorage_StoreNotOpen"));
                    }
                    if (this.m_handle == Win32Native.NULL)
                    {
                        this.m_handle = nOpen(this.m_InfoFile, this.GetSyncObjectName());
                    }
                    return nGetUsage(this.m_handle);
                }
            }
        }

        [CLSCompliant(false)]
        public override ulong MaximumSize
        {
            get
            {
                if (base.IsRoaming())
                {
                    return 0x7fffffffffffffffL;
                }
                return base.MaximumSize;
            }
        }

        internal string RootDirectory =>
            this.m_RootDir;
    }
}

