namespace System.Deployment.Application
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    internal static class NativeMethods
    {
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public const ushort PROCESSOR_ARCHITECTURE_AMD64 = 9;
        public const ushort PROCESSOR_ARCHITECTURE_IA64 = 6;
        public const ushort PROCESSOR_ARCHITECTURE_INTEL = 0;

        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        internal static extern bool CloseHandle(HandleRef handle);
        [DllImport("Ole32.dll")]
        public static extern uint CoCreateInstance([In] ref Guid clsid, [MarshalAs(UnmanagedType.IUnknown)] object punkOuter, int context, [In] ref Guid iid, [MarshalAs(UnmanagedType.IUnknown)] out object o);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("wininet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CommitUrlCacheEntry([In] string lpszUrlName, [In] string lpszLocalFileName, [In] long ExpireTime, [In] long LastModifiedTime, [In] uint CacheEntryType, [In] string lpHeaderInfo, [In] int dwHeaderSize, [In] string lpszFileExtension, [In] string lpszOriginalUrl);
        [DllImport("mscorwks.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig=false)]
        internal static extern void CorLaunchApplication(uint hostType, string applicationFullName, int manifestPathsCount, string[] manifestPaths, int activationDataCount, string[] activationData, PROCESS_INFORMATION processInformation);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        internal static extern IntPtr CreateActCtxW([In] ACTCTXW actCtx);
        [DllImport("mscorwks.dll", PreserveSig=false)]
        internal static extern void CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);
        [DllImport("mscorwks.dll", CharSet=CharSet.Auto, PreserveSig=false)]
        internal static extern void CreateAssemblyEnum(out IAssemblyEnum ppEnum, IApplicationContext pAppCtx, IAssemblyName pName, uint dwFlags, IntPtr pvReserved);
        [DllImport("mscorwks.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
        internal static extern void CreateAssemblyNameObject(out IAssemblyName ppEnum, string szAssemblyName, uint dwFlags, IntPtr pvReserved);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("wininet.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool CreateUrlCacheEntry([In] string urlName, [In] int expectedFileSize, [In] string fileExtension, [Out] StringBuilder fileName, [In] int dwReserved);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr FindResource(IntPtr hModule, string lpName, string lpType);
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool FreeLibrary(IntPtr hModule);
        [return: MarshalAs(UnmanagedType.IUnknown)]
        [DllImport("mscorwks.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig=false)]
        internal static extern object GetAssemblyIdentityFromFile([In, MarshalAs(UnmanagedType.LPWStr)] string filePath, [In] ref Guid riid);
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern uint GetCurrentThreadId();
        [DllImport("mscoree.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig=false)]
        public static extern void GetFileVersion(string szFileName, StringBuilder szBuffer, uint cchBuffer, out uint dwLength);
        internal static string GetLoadedModulePath(string moduleName)
        {
            string str = null;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            if (moduleHandle != IntPtr.Zero)
            {
                StringBuilder fileName = new StringBuilder(260);
                if (GetModuleFileName(moduleHandle, fileName, fileName.Capacity) > 0)
                {
                    str = fileName.ToString();
                }
            }
            return str;
        }

        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int GetModuleFileName(IntPtr module, [Out] StringBuilder fileName, int size);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr GetModuleHandle(string moduleName);
        [DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
        public static extern void GetNativeSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO sysInfo);
        [DllImport("kernel32.dll", CharSet=CharSet.Ansi, SetLastError=true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
        [DllImport("mscoree.dll", CharSet=CharSet.Unicode, ExactSpelling=true, PreserveSig=false)]
        public static extern void GetRequestedRuntimeInfo(string pExe, string pwszVersion, string pConfigurationFile, uint startupFlags, uint runtimeInfoFlags, StringBuilder pDirectory, uint dwDirectory, out uint dwDirectoryLength, StringBuilder pVersion, uint cchBuffer, out uint dwLength);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern int GetShortPathName(string LongPath, [Out] StringBuilder ShortPath, int BufferSize);
        [DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
        public static extern void GetSystemInfo([MarshalAs(UnmanagedType.Struct)] ref SYSTEM_INFO sysInfo);
        [DllImport("wininet.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern bool InternetGetCookieW([In] string url, [In] string cookieName, [Out] StringBuilder cookieData, [In, Out] ref uint bytes);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr LoadLibrary(string lpModuleName);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr LoadLibraryEx(string lpModuleName, IntPtr hFile, uint dwFlags);
        [DllImport("mscoree.dll", PreserveSig=false)]
        private static extern IntPtr LoadLibraryShim([MarshalAs(UnmanagedType.LPWStr)] string dllName, [MarshalAs(UnmanagedType.LPWStr)] string szVersion, IntPtr reserved);
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern IntPtr LoadResource(IntPtr hModule, IntPtr handle);
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern IntPtr LockResource(IntPtr hglobal);
        [DllImport("kernel32.dll", ExactSpelling=true)]
        internal static extern void ReleaseActCtx([In] IntPtr hActCtx);
        [DllImport("shell32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern void SHChangeNotify(int eventID, uint flags, IntPtr item1, IntPtr item2);
        [DllImport("shell32.dll", CharSet=CharSet.Unicode)]
        public static extern uint SHCreateItemFromParsingName([In, MarshalAs(UnmanagedType.LPWStr)] string pszPath, [In] IntPtr pbc, [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.Interface)] out object ppv);
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern uint SizeofResource(IntPtr hModule, IntPtr handle);
        [DllImport("mscoree.dll", ExactSpelling=true, PreserveSig=false)]
        internal static extern void StrongNameFreeBuffer(IntPtr buffer);
        [DllImport("mscoree.dll")]
        internal static extern byte StrongNameSignatureVerificationEx([MarshalAs(UnmanagedType.LPWStr)] string filePath, byte forceVerification, out byte wasVerified);
        [DllImport("mscoree.dll", ExactSpelling=true, PreserveSig=false)]
        internal static extern void StrongNameTokenFromPublicKey(byte[] publicKeyBlob, uint publicKeyBlobCount, ref IntPtr strongNameTokenArray, ref uint strongNameTokenCount);
        [DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool VerifyVersionInfo([In, Out] OSVersionInfoEx osvi, [In] uint dwTypeMask, [In] ulong dwConditionMask);
        [DllImport("kernel32.dll")]
        public static extern ulong VerSetConditionMask([In] ulong ConditionMask, [In] uint TypeMask, [In] byte Condition);

        [StructLayout(LayoutKind.Explicit)]
        public struct _PROCESSOR_INFO_UNION
        {
            [FieldOffset(0)]
            internal uint dwOemId;
            [FieldOffset(0)]
            internal ushort wProcessorArchitecture;
            [FieldOffset(2)]
            internal ushort wReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class ACTCTXW
        {
            public uint cbSize = ((uint) Marshal.SizeOf(typeof(NativeMethods.ACTCTXW)));
            public uint dwFlags = 0;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpSource;
            public ushort wProcessorArchitecture;
            public ushort wLangId;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpAssemblyDirectory;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpResourceName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpApplicationName;
            public IntPtr hModule;
            public ACTCTXW(string manifestPath)
            {
                this.lpSource = manifestPath;
            }
        }

        internal enum ASM_CACHE : uint
        {
            DOWNLOAD = 4,
            GAC = 2,
            ZAP = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AssemblyInfoInternal
        {
            internal const int MaxPath = 0x400;
            internal int cbAssemblyInfo;
            internal int assemblyFlags;
            internal long assemblySizeInKB;
            internal IntPtr currentAssemblyPathBuf;
            internal int cchBuf;
        }

        public enum CacheEntryFlags : uint
        {
            Cookie = 0x100000,
            Edited = 8,
            Normal = 1,
            Sparse = 0x10000,
            Sticky = 4,
            TrackOffline = 0x10,
            TrackOnline = 0x20,
            UrlHistory = 0x200000
        }

        internal enum CreateAssemblyNameObjectFlags : uint
        {
            CANOF_DEFAULT = 0,
            CANOF_PARSE_DISPLAY_NAME = 1
        }

        internal enum CreationDisposition : uint
        {
            CREATE_ALWAYS = 2,
            CREATE_NEW = 1,
            OPEN_ALWAYS = 4,
            OPEN_EXISTING = 3,
            TRUNCATE_EXISTING = 5
        }

        [Flags]
        internal enum FlagsAndAttributes : uint
        {
            FILE_ATTRIBUTE_ARCHIVE = 0x20,
            FILE_ATTRIBUTE_COMPRESSED = 0x800,
            FILE_ATTRIBUTE_DEVICE = 0x40,
            FILE_ATTRIBUTE_DIRECTORY = 0x10,
            FILE_ATTRIBUTE_ENCRYPTED = 0x4000,
            FILE_ATTRIBUTE_HIDDEN = 2,
            FILE_ATTRIBUTE_NORMAL = 0x80,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x2000,
            FILE_ATTRIBUTE_OFFLINE = 0x1000,
            FILE_ATTRIBUTE_READONLY = 1,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x400,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x200,
            FILE_ATTRIBUTE_SYSTEM = 4,
            FILE_ATTRIBUTE_TEMPORARY = 0x100,
            FILE_FLAG_BACKUP_SEMANTICS = 0x2000000,
            FILE_FLAG_DELETE_ON_CLOSE = 0x4000000,
            FILE_FLAG_FIRST_PIPE_INSTANCE = 0x80000,
            FILE_FLAG_NO_BUFFERING = 0x20000000,
            FILE_FLAG_OPEN_NO_RECALL = 0x100000,
            FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
            FILE_FLAG_OVERLAPPED = 0x40000000,
            FILE_FLAG_POSIX_SEMANTICS = 0x1000000,
            FILE_FLAG_RANDOM_ACCESS = 0x10000000,
            FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000,
            FILE_FLAG_WRITE_THROUGH = 0x80000000
        }

        [Flags]
        internal enum GenericAccess : uint
        {
            GENERIC_ALL = 0x10000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000
        }

        internal enum HResults
        {
            HRESULT_ERROR_REVISION_MISMATCH = -2147023590
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("7c23ff90-33af-11d3-95da-00a024a85b51")]
        internal interface IApplicationContext
        {
            void SetContextNameObject(NativeMethods.IAssemblyName pName);
            void GetContextNameObject(out NativeMethods.IAssemblyName ppName);
            void Set([MarshalAs(UnmanagedType.LPWStr)] string szName, int pvValue, uint cbValue, uint dwFlags);
            void Get([MarshalAs(UnmanagedType.LPWStr)] string szName, out int pvValue, ref uint pcbValue, uint dwFlags);
            void GetDynamicDirectory(out int wzDynamicDir, ref uint pdwSize);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
        internal interface IAssemblyCache
        {
            void UninstallAssembly();
            void QueryAssemblyInfo(int flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, ref NativeMethods.AssemblyInfoInternal assemblyInfo);
            void CreateAssemblyCacheItem();
            void CreateAssemblyScavenger();
            void InstallAssembly();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
        internal interface IAssemblyEnum
        {
            [PreserveSig]
            int GetNextAssembly(NativeMethods.IApplicationContext ppAppCtx, out NativeMethods.IAssemblyName ppName, uint dwFlags);
            [PreserveSig]
            int Reset();
            [PreserveSig]
            int Clone(out NativeMethods.IAssemblyEnum ppEnum);
        }

        [ComImport, Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IAssemblyName
        {
            [PreserveSig]
            int SetProperty(uint PropertyId, IntPtr pvProperty, uint cbProperty);
            [PreserveSig]
            int GetProperty(uint PropertyId, IntPtr pvProperty, ref uint pcbProperty);
            [PreserveSig]
            int Finalize();
            [PreserveSig]
            int GetDisplayName(IntPtr szDisplayName, ref uint pccDisplayName, uint dwDisplayFlags);
            [PreserveSig]
            int BindToObject(object refIID, object pAsmBindSink, NativeMethods.IApplicationContext pApplicationContext, [MarshalAs(UnmanagedType.LPWStr)] string szCodeBase, long llFlags, int pvReserved, uint cbReserved, out int ppv);
            [PreserveSig]
            int GetName(out uint lpcwBuffer, out int pwzName);
            [PreserveSig]
            int GetVersion(out uint pdwVersionHi, out uint pdwVersionLow);
            [PreserveSig]
            int IsEqual(NativeMethods.IAssemblyName pName, uint dwCmpFlags);
            [PreserveSig]
            int Clone(out NativeMethods.IAssemblyName pName);
        }

        [ComImport, Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IShellItem
        {
            void BindToHandler(IntPtr pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid bhid, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);
            void GetParent(out NativeMethods.IShellItem ppsi);
            void GetDisplayName(NativeMethods.SIGDN sigdnName, out IntPtr ppszName);
            void GetAttributes(uint sfgaoMask, out uint psfgaoAttribs);
            void Compare(NativeMethods.IShellItem psi, uint hint, out int piOrder);
        }

        [ComImport, Guid("4CD19ADA-25A5-4A32-B3B7-347BEE5BE36B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IStartMenuPinnedList
        {
            void RemoveFromList(NativeMethods.IShellItem psi);
        }

        [StructLayout(LayoutKind.Sequential)]
        public class OSVersionInfoEx
        {
            public uint dwOSVersionInfoSize;
            public uint dwMajorVersion;
            public uint dwMinorVersion;
            public uint dwBuildNumber;
            public uint dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x80)]
            public string szCSDVersion;
            public ushort wServicePackMajor;
            public ushort wServicePackMinor;
            public ushort wSuiteMask;
            public byte bProductType;
            public byte bReserved;
        }

        [StructLayout(LayoutKind.Sequential), SuppressUnmanagedCodeSecurity]
        internal class PROCESS_INFORMATION
        {
            public IntPtr hProcess = IntPtr.Zero;
            public IntPtr hThread = IntPtr.Zero;
            public int dwProcessId;
            public int dwThreadId;
            ~PROCESS_INFORMATION()
            {
                this.Close();
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            internal void Close()
            {
                if ((this.hProcess != IntPtr.Zero) && (this.hProcess != NativeMethods.INVALID_HANDLE_VALUE))
                {
                    NativeMethods.CloseHandle(new HandleRef(this, this.hProcess));
                    this.hProcess = NativeMethods.INVALID_HANDLE_VALUE;
                }
                if ((this.hThread != IntPtr.Zero) && (this.hThread != NativeMethods.INVALID_HANDLE_VALUE))
                {
                    NativeMethods.CloseHandle(new HandleRef(this, this.hThread));
                    this.hThread = NativeMethods.INVALID_HANDLE_VALUE;
                }
            }
        }

        [Flags]
        internal enum ShareMode : uint
        {
            FILE_SHARE_DELETE = 4,
            FILE_SHARE_NONE = 0,
            FILE_SHARE_READ = 1,
            FILE_SHARE_WRITE = 2
        }

        public enum SHChangeNotifyEventID
        {
            SHCNE_ASSOCCHANGED = 0x8000000
        }

        public enum SHChangeNotifyFlags : uint
        {
            SHCNF_IDLIST = 0
        }

        internal enum SIGDN : uint
        {
            DESKTOPABSOLUTEEDITING = 0x8004c000,
            DESKTOPABSOLUTEPARSING = 0x80028000,
            FILESYSPATH = 0x80058000,
            NORMALDISPLAY = 0,
            PARENTRELATIVE = 0x80080001,
            PARENTRELATIVEEDITING = 0x80031001,
            PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
            PARENTRELATIVEPARSING = 0x80018001,
            URL = 0x80068000
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            internal NativeMethods._PROCESSOR_INFO_UNION uProcessorInfo;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        internal enum Win32Error
        {
            ERROR_ACCESS_DENIED = 5,
            ERROR_ALREADY_EXISTS = 0xb7,
            ERROR_CALL_NOT_IMPLEMENTED = 120,
            ERROR_FILE_EXISTS = 80,
            ERROR_FILE_NOT_FOUND = 2,
            ERROR_FILENAME_EXCED_RANGE = 0xce,
            ERROR_INVALID_FUNCTION = 1,
            ERROR_INVALID_HANDLE = 6,
            ERROR_INVALID_PARAMETER = 0x57,
            ERROR_NO_MORE_FILES = 0x12,
            ERROR_NOT_READY = 0x15,
            ERROR_PATH_NOT_FOUND = 3,
            ERROR_SHARING_VIOLATION = 0x20,
            ERROR_SUCCESS = 0,
            ERROR_TOO_MANY_OPEN_FILES = 4
        }
    }
}

