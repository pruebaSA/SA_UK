namespace System.EnterpriseServices
{
    using System;
    using System.EnterpriseServices.Thunk;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;

    internal class Util
    {
        internal static readonly int CLSCTX_SERVER = 0x15;
        internal static readonly int COMADMIN_E_OBJECTERRORS = -2146368511;
        internal static readonly int CONTEXT_E_ABORTED = -2147164158;
        internal static readonly int CONTEXT_E_ABORTING = -2147164157;
        internal static readonly int CONTEXT_E_NOCONTEXT = -2147164156;
        internal static readonly int CONTEXT_E_TMNOTAVAILABLE = -2147164145;
        internal static readonly int DISP_E_UNKNOWNNAME = -2147352570;
        internal static readonly int E_ACCESSDENIED = -2147024891;
        internal static readonly int E_FAIL = -2147467259;
        internal static readonly int E_NOINTERFACE = -2147467262;
        internal static readonly int E_UNEXPECTED = -2147418113;
        internal static readonly int ERROR_NO_TOKEN = 0x3f0;
        internal static readonly int ERROR_SUCCESS = 0;
        internal static readonly int FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x2000;
        internal static readonly int FORMAT_MESSAGE_FROM_SYSTEM = 0x1000;
        internal static readonly int FORMAT_MESSAGE_IGNORE_INSERTS = 0x200;
        internal static readonly Guid GUID_NULL = new Guid("00000000-0000-0000-0000-000000000000");
        internal static readonly Guid IID_IObjectContext = new Guid("51372AE0-CAE7-11CF-BE81-00AA00A2FA25");
        internal static readonly Guid IID_ISecurityCallContext = new Guid("CAFC823E-B441-11D1-B82B-0000F8757E2A");
        internal static readonly Guid IID_IUnknown = new Guid("00000000-0000-0000-C000-000000000046");
        internal static readonly int MB_ABORTRETRYIGNORE = 2;
        internal static readonly int MB_ICONEXCLAMATION = 0x30;
        internal static readonly int REGDB_E_CLASSNOTREG = -2147221164;
        internal static readonly int SECURITY_CREATOR_SID_AUTHORITY = 3;
        internal static readonly int SECURITY_LOCAL_SID_AUTHORITY = 2;
        internal static readonly int SECURITY_NT_SID_AUTHORITY = 5;
        internal static readonly int SECURITY_NULL_SID_AUTHORITY = 0;
        internal static readonly int SECURITY_WORLD_SID_AUTHORITY = 1;
        internal static readonly int XACT_E_INDOUBT = -2147168234;

        [DllImport("ole32.dll", PreserveSig=false)]
        internal static extern void CoGetCallContext([MarshalAs(UnmanagedType.LPStruct)] Guid riid, [MarshalAs(UnmanagedType.Interface)] out ISecurityCallContext iface);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        internal static extern int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId, int dwLanguageId, StringBuilder lpBuffer, int nSize, int arguments);
        internal static string GetErrorString(int hr)
        {
            StringBuilder lpBuffer = new StringBuilder(0x400);
            if (FormatMessage((FORMAT_MESSAGE_IGNORE_INSERTS | FORMAT_MESSAGE_FROM_SYSTEM) | FORMAT_MESSAGE_ARGUMENT_ARRAY, IntPtr.Zero, hr, 0, lpBuffer, lpBuffer.Capacity + 1, 0) == 0)
            {
                return null;
            }
            int length = lpBuffer.Length;
            while (length > 0)
            {
                char ch = lpBuffer[length - 1];
                if ((ch > ' ') && (ch != '.'))
                {
                    break;
                }
                length--;
            }
            return lpBuffer.ToString(0, length);
        }

        [DllImport("mtxex.dll", CallingConvention=CallingConvention.Cdecl)]
        internal static extern int GetObjectContext([MarshalAs(UnmanagedType.Interface)] out System.EnterpriseServices.IObjectContext pCtx);
        [DllImport("KERNEL32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool GetVersionEx([In, Out] OSVERSIONINFOEX ver);
        [DllImport("oleaut32.dll")]
        internal static extern int LoadRegTypeLib([In, MarshalAs(UnmanagedType.LPStruct)] Guid lidID, short wVerMajor, short wVerMinor, int lcid, [MarshalAs(UnmanagedType.Interface)] out object pptlib);
        [DllImport("oleaut32.dll")]
        internal static extern int LoadTypeLibEx([In, MarshalAs(UnmanagedType.LPWStr)] string str, int regKind, out IntPtr pptlib);
        [DllImport("user32.dll")]
        internal static extern int MessageBox(int hWnd, string lpText, string lpCaption, int type);
        [DllImport("kernel32.dll")]
        internal static extern void OutputDebugString(string msg);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        internal static extern bool QueryPerformanceCounter(out long count);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll")]
        internal static extern bool QueryPerformanceFrequency(out long count);
        [DllImport("oleaut32.dll")]
        internal static extern int RegisterTypeLib(IntPtr pptlib, [In, MarshalAs(UnmanagedType.LPWStr)] string str, [In, MarshalAs(UnmanagedType.LPWStr)] string help);
        [DllImport("oleaut32.dll", PreserveSig=false)]
        internal static extern void UnRegisterTypeLib([In, MarshalAs(UnmanagedType.LPStruct)] Guid libID, short wVerMajor, short wVerMinor, int lcid, System.Runtime.InteropServices.ComTypes.SYSKIND syskind);

        internal static bool ExtendedLifetime =>
            ((Proxy.GetManagedExts() & 1) != 0);

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        internal class OSVERSIONINFOEX
        {
            internal int OSVersionInfoSize;
            internal int MajorVersion;
            internal int MinorVersion;
            internal int BuildNumber;
            internal int PlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x80)]
            internal string CSDVersion;
            internal short ServicePackMajor;
            internal short ServicePackMinor;
            internal short SuiteMask;
            internal byte ProductType;
            internal byte Reserved;
            public OSVERSIONINFOEX()
            {
                this.OSVersionInfoSize = Marshal.SizeOf(this);
            }
        }
    }
}

