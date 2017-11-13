namespace MS.Win32
{
    using Accessibility;
    using Microsoft.Win32.SafeHandles;
    using MS.Internal.WindowsBase;
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Windows;
    using System.Windows.Interop;

    [FriendAccessAllowed]
    internal sealed class UnsafeNativeMethods
    {
        public static readonly Guid Clsid_SpeechTip = new Guid(0xdcbd6fa8, 0x32f, 0x11d3, 0xb5, 0xb1, 0, 0xc0, 0x4f, 0xc3, 0x24, 0xa1);
        internal const uint COOKIE_STATE_REJECT = 5;
        public const int DUPLICATE_CLOSE_SOURCE = 1;
        public const int DUPLICATE_SAME_ACCESS = 2;
        public const int EventObjectUIFragmentCreate = 0x6fffffff;
        internal const int FILE_MAP_ALL_ACCESS = 0xf001f;
        internal const int FILE_MAP_COPY = 1;
        internal const int FILE_MAP_READ = 4;
        internal const int FILE_MAP_WRITE = 2;
        public static readonly Guid GUID_COMPARTMENT_HANDWRITING_OPENCLOSE = new Guid(0xf9ae2c6b, 0x1866, 0x4361, 0xaf, 0x72, 0x7a, 0xa3, 9, 0x48, 0x89, 14);
        public static readonly Guid GUID_COMPARTMENT_KEYBOARD_DISABLED = new Guid(0x71a5b253, 0x1951, 0x466b, 0x9f, 0xbc, 0x9c, 0x88, 8, 250, 0x84, 0xf2);
        public static readonly Guid GUID_COMPARTMENT_KEYBOARD_INPUTMODE_CONVERSION = new Guid(0xccf05dd8, 0x4a87, 0x11d7, 0xa6, 0xe2, 0, 6, 0x5b, 0x84, 0x43, 0x5c);
        public static readonly Guid GUID_COMPARTMENT_KEYBOARD_INPUTMODE_SENTENCE = new Guid(0xccf05dd9, 0x4a87, 0x11d7, 0xa6, 0xe2, 0, 6, 0x5b, 0x84, 0x43, 0x5c);
        public static Guid GUID_COMPARTMENT_KEYBOARD_OPENCLOSE = new Guid(0x58273aad, 0x1bb, 0x4164, 0x95, 0xc6, 0x75, 0x5b, 160, 0xb5, 0x16, 0x2d);
        public static readonly Guid GUID_COMPARTMENT_SPEECH_DISABLED = new Guid(0x56c5c607, 0x703, 0x4e59, 0x8e, 0x52, 0xcb, 200, 0x4e, 0x8b, 190, 0x35);
        public static readonly Guid GUID_COMPARTMENT_SPEECH_GLOBALSTATE = new Guid(0x2a54fe8e, 0xd08, 0x460c, 0xa7, 0x5d, 0x87, 3, 0x5f, 0xf4, 0x36, 0xc5);
        public static readonly Guid GUID_COMPARTMENT_SPEECH_OPENCLOSE = new Guid(0x544d6a63, 0xe2e8, 0x4752, 0xbb, 0xd1, 0, 9, 0x60, 0xbc, 160, 0x83);
        public static readonly Guid GUID_COMPARTMENT_TRANSITORYEXTENSION = new Guid(0x8be347f5, 0xc7a0, 0x11d7, 180, 8, 0, 6, 0x5b, 0x84, 0x43, 0x5c);
        public static readonly Guid GUID_COMPARTMENT_TRANSITORYEXTENSION_DOCUMENTMANAGER = new Guid(0x8be347f7, 0xc7a0, 0x11d7, 180, 8, 0, 6, 0x5b, 0x84, 0x43, 0x5c);
        public static readonly Guid GUID_COMPARTMENT_TRANSITORYEXTENSION_PARENT = new Guid(0x8be347f8, 0xc7a0, 0x11d7, 180, 8, 0, 6, 0x5b, 0x84, 0x43, 0x5c);
        public static readonly Guid Guid_Null = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        public static readonly Guid GUID_PROP_ATTRIBUTE = new Guid(0x34b45670, 0x7526, 0x11d2, 0xa1, 0x47, 0, 0x10, 90, 0x27, 0x99, 0xb5);
        public static readonly Guid GUID_PROP_INPUTSCOPE = new Guid(0x1713dd5a, 0x68e7, 0x4a5b, 0x9a, 0xf6, 0x59, 0x2a, 0x59, 0x5c, 0x77, 0x8d);
        public static readonly Guid GUID_PROP_LANGID = new Guid(0x3280ce20, 0x8032, 0x11d2, 0xb6, 3, 0, 0x10, 90, 0x27, 0x99, 0xb5);
        public static readonly Guid GUID_PROP_READING = new Guid(0x5463f7c0, 0x8e31, 0x11d2, 0xbf, 70, 0, 0x10, 90, 0x27, 0x99, 0xb5);
        public static readonly Guid GUID_SYSTEM_FUNCTIONPROVIDER = new Guid("9a698bb0-0f21-11d3-8df1-00105a2799b5");
        public static readonly Guid GUID_TFCAT_TIP_KEYBOARD = new Guid(0x34745c63, 0xb2f0, 0x4784, 0x8b, 0x67, 0x5e, 0x12, 200, 0x70, 0x1a, 0x31);
        internal static Guid IID_IAccessible = new Guid(0x618736e0, 0x3c3d, 0x11cf, 0x81, 12, 0, 170, 0, 0x38, 0x9b, 0x71);
        public static readonly Guid IID_ITextStoreACPSink = new Guid(0x22d44c94, 0xa419, 0x4542, 0xa2, 0x72, 0xae, 0x26, 9, 0x3e, 0xce, 0xcf);
        public static readonly Guid IID_ITfCompartmentEventSink = new Guid(0x743abd5f, 0xf26d, 0x48df, 140, 0xc5, 0x23, 0x84, 0x92, 0x41, 0x9b, 100);
        public static readonly Guid IID_ITfFnConfigure = new Guid(0x88f567c6, 0x1757, 0x49f8, 0xa1, 0xb2, 0x89, 0x23, 0x4c, 30, 0xef, 0xf9);
        public static readonly Guid IID_ITfFnConfigureRegisterWord = new Guid(0xbb95808a, 0x6d8f, 0x4bca, 0x84, 0, 0x53, 0x90, 0xb5, 0x86, 0xae, 0xdf);
        public static readonly Guid IID_ITfFnCustomSpeechCommand = new Guid(0xfca6c349, 0xa12f, 0x43a3, 0x8d, 0xd6, 90, 90, 0x42, 130, 0x57, 0x7b);
        public static readonly Guid IID_ITfFnReconversion = new Guid("4cea93c0-0a58-11d3-8df0-00105a2799b5");
        public static readonly Guid IID_ITfLanguageProfileNotifySink = new Guid(0x43c9fe15, 0xf494, 0x4c17, 0x9d, 0xe2, 0xb8, 0xa4, 0xac, 0x35, 10, 0xa8);
        public static readonly Guid IID_ITfTextEditSink = new Guid(0x8127d409, 0xccd3, 0x4683, 150, 0x7a, 180, 0x3d, 0x5b, 0x48, 0x2b, 0xf7);
        public static readonly Guid IID_ITfThreadFocusSink = new Guid(0xc0f1db0c, 0x3a20, 0x405c, 0xa3, 3, 150, 0xb6, 1, 10, 0x88, 0x5f);
        public static readonly Guid IID_ITfTransitoryExtensionSink = new Guid(0xa615096f, 0x1c57, 0x4813, 0x8a, 0x15, 0x55, 0xee, 110, 90, 0x83, 0x9c);
        internal const uint INTERNET_COOKIE_EVALUATE_P3P = 0x40;
        internal const uint INTERNET_COOKIE_IS_RESTRICTED = 0x200;
        internal const uint INTERNET_COOKIE_THIRD_PARTY = 0x10;
        internal static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
        public const int MB_COMPOSITE = 2;
        public const int MB_ERR_INVALID_CHARS = 8;
        public const int MB_PRECOMPOSED = 1;
        public const int MB_USEGLYPHCHARS = 4;
        internal const int MEM_4MB_PAGES = -2147483648;
        internal const int MEM_COMMIT = 0x1000;
        internal const int MEM_DECOMMIT = 0x4000;
        internal const int MEM_FREE = 0x10000;
        internal const int MEM_IMAGE = 0x1000000;
        internal const int MEM_MAPPED = 0x40000;
        internal const int MEM_PHYSICAL = 0x400000;
        internal const int MEM_PRIVATE = 0x20000;
        internal const int MEM_RELEASE = 0x8000;
        internal const int MEM_RESERVE = 0x2000;
        internal const int MEM_RESET = 0x80000;
        internal const int MEM_TOP_DOWN = 0x100000;
        internal const int MEM_WRITE_WATCH = 0x200000;
        internal const int PAGE_EXECUTE = 0x10;
        internal const int PAGE_EXECUTE_READ = 0x20;
        internal const int PAGE_EXECUTE_READWRITE = 0x40;
        internal const int PAGE_EXECUTE_WRITECOPY = 0x80;
        internal const int PAGE_GUARD = 0x100;
        internal const int PAGE_NOACCESS = 1;
        internal const int PAGE_NOCACHE = 0x200;
        internal const int PAGE_READONLY = 2;
        internal const int PAGE_READWRITE = 4;
        internal const int PAGE_WRITECOMBINE = 0x400;
        internal const int PAGE_WRITECOPY = 8;
        public const int PROCESS_QUERY_INFORMATION = 0x400;
        public const int PROCESS_VM_READ = 0x10;
        internal const int SDDL_REVISION = 1;
        internal const int SDDL_REVISION_1 = 1;
        internal const int SEC_COMMIT = 0x8000000;
        internal const int SEC_FILE = 0x800000;
        internal const int SEC_IMAGE = 0x1000000;
        internal const int SEC_NOCACHE = 0x10000000;
        internal const int SEC_RESERVE = 0x4000000;
        internal const int SECTION_ALL_ACCESS = 0xf001f;
        internal const int SECTION_EXTEND_SIZE = 0x10;
        internal const int SECTION_MAP_EXECUTE = 8;
        internal const int SECTION_MAP_READ = 4;
        internal const int SECTION_MAP_WRITE = 2;
        internal const int SECTION_QUERY = 1;
        internal const int STANDARD_RIGHTS_REQUIRED = 0xf0000;
        internal const int STATUS_BUFFER_TOO_SMALL = -1073741789;
        internal const int STATUS_SUCCESS = 0;
        internal const int STATUS_TIMEOUT = 0x102;
        public const int TF_CLIENTID_NULL = 0;
        public const int TF_COMMANDING_ON = 8;
        public const int TF_DICTATION_ON = 1;
        public const int TF_INVALID_COOKIE = -1;
        public const char TS_CHAR_EMBEDDED = '￼';
        public const char TS_CHAR_REGION = '\0';
        public const char TS_CHAR_REPLACEMENT = '�';
        public const int TS_DEFAULT_SELECTION = -1;
        public const int TS_E_FORMAT = -2147220982;
        public const int TS_E_INVALIDPOINT = -2147220985;
        public const int TS_E_NOLAYOUT = -2147220986;
        public const int TS_E_NOSELECTION = -2147220987;
        public const int TS_E_READONLY = -2147220983;
        public const int TS_E_SYNCHRONOUS = -2147220984;
        public const int TS_S_ASYNC = 0x40300;
        public static readonly Guid TSATTRID_Font_FaceName = new Guid(0xb536aeb6, 0x53b, 0x4eb8, 0xb6, 90, 80, 0xda, 30, 0x81, 0xe7, 0x2e);
        public static readonly Guid TSATTRID_Font_SizePts = new Guid(0xc8493302, 0xa5e9, 0x456d, 0xaf, 4, 0x80, 5, 0xe4, 0x13, 15, 3);
        public static readonly Guid TSATTRID_Font_Style_Height = new Guid(0x7e937477, 0x12e6, 0x458b, 0x92, 0x6a, 0x1f, 0xa4, 0x4e, 0xe8, 0xf3, 0x91);
        public static readonly Guid TSATTRID_Text_Orientation = new Guid(0x6bab707f, 0x8785, 0x4c39, 0x8b, 0x52, 150, 0xf8, 120, 0x30, 0x3f, 0xfb);
        public static readonly Guid TSATTRID_Text_ReadOnly = new Guid(0x85836617, 0xde32, 0x4afd, 0xa5, 15, 0xa2, 0xdb, 0x11, 14, 110, 0x4d);
        public static readonly Guid TSATTRID_Text_VerticalWriting = new Guid(0x6bba8195, 0x46f, 0x4ea9, 0xb3, 0x11, 0x97, 0xfd, 0x66, 0xc4, 0x27, 0x4b);
        public const int WAIT_FAILED = -1;
        internal const int WRITE_WATCH_FLAG_RESET = 1;

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", EntryPoint="GetTempFileName", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern uint _GetTempFileName(string tmpPath, string prefix, uint uniqueIdOrZero, StringBuilder tmpFileName);
        [SecurityCritical]
        public static IntPtr BeginPaint(HandleRef hWnd, [In, Out, MarshalAs(UnmanagedType.LPStruct)] ref NativeMethods.PAINTSTRUCT lpPaint) => 
            MS.Win32.HandleCollector.Add(IntBeginPaint(hWnd, ref lpPaint), NativeMethods.CommonHandles.HDC);

        [DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool BitBlt(HandleRef hDC, int x, int y, int nWidth, int nHeight, HandleRef hSrcDC, int xSrc, int ySrc, int dwRop);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [SecurityCritical]
        public static void ClientToScreen(HandleRef hWnd, [In, Out] NativeMethods.POINT pt)
        {
            if (IntClientToScreen(hWnd, pt) == 0)
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical]
        public static bool CloseHandleNoThrow(HandleRef handle)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) handle, NativeMethods.CommonHandles.Kernel);
            bool flag = IntCloseHandle(handle);
            Marshal.GetLastWin32Error();
            return flag;
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("ole32.dll", ExactSpelling=true, PreserveSig=false)]
        public static extern object CoCreateInstance([In] ref Guid clsid, [MarshalAs(UnmanagedType.Interface)] object punkOuter, int context, [In] ref Guid iid);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("urlmon.dll", ExactSpelling=true)]
        internal static extern int CoInternetCreateSecurityManager([MarshalAs(UnmanagedType.Interface)] object pIServiceProvider, [MarshalAs(UnmanagedType.Interface)] out object ppISecurityManager, int dwReserved);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("urlmon.dll", ExactSpelling=true)]
        internal static extern int CoInternetIsFeatureEnabled(int featureEntry, int dwFlags);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("urlmon.dll", ExactSpelling=true)]
        internal static extern int CoInternetIsFeatureZoneElevationEnabled([MarshalAs(UnmanagedType.LPWStr)] string szFromURL, [MarshalAs(UnmanagedType.LPWStr)] string szToURL, IInternetSecurityManager secMgr, int dwFlags);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("urlmon.dll", ExactSpelling=true)]
        internal static extern int CoInternetSetFeatureEnabled(int featureEntry, int dwFlags, bool fEnable);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        internal static extern int CommDlgExtendedError();
        [DllImport("advapi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor(string stringSecurityDescriptor, int stringSDRevision, ref IntPtr securityDescriptor, IntPtr securityDescriptorSize);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", EntryPoint="RtlMoveMemory", ExactSpelling=true)]
        public static extern void CopyMemory(IntPtr pdst, byte[] psrc, int cb);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", EntryPoint="RtlMoveMemory", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern void CopyMemoryW(IntPtr pdst, string psrc, int cb);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", EntryPoint="RtlMoveMemory", CharSet=CharSet.Unicode, ExactSpelling=true)]
        public static extern void CopyMemoryW(IntPtr pdst, char[] psrc, int cb);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("ole32.dll")]
        public static extern int CoRegisterPSClsid(ref Guid riid, ref Guid rclsid);
        [SecurityCritical]
        internal static NativeMethods.BitmapHandle CreateBitmap(int width, int height, int planes, int bitsPerPixel, byte[] lpvBits)
        {
            NativeMethods.BitmapHandle handle = PrivateCreateBitmap(width, height, planes, bitsPerPixel, lpvBits);
            Marshal.GetLastWin32Error();
            bool isInvalid = handle.IsInvalid;
            return handle;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool CreateCaret(HandleRef hwnd, NativeMethods.BitmapHandle hbitmap, int width, int height);
        [DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr CreateCompatibleBitmap(HandleRef hDC, int width, int height);
        [SecurityTreatAsSafe, SecurityCritical]
        public static IntPtr CreateCompatibleDC(HandleRef hDC)
        {
            IntPtr handle = IntCreateCompatibleDC(hDC);
            if (handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            return MS.Win32.HandleCollector.Add(handle, NativeMethods.CommonHandles.HDC);
        }

        [SecurityCritical]
        internal static NativeMethods.BitmapHandle CreateDIBSection(HandleRef hdc, ref NativeMethods.BITMAPINFO bitmapInfo, int iUsage, ref IntPtr ppvBits, IntPtr hSection, int dwOffset)
        {
            NativeMethods.BitmapHandle handle = PrivateCreateDIBSection(hdc, ref bitmapInfo, iUsage, ref ppvBits, hSection, dwOffset);
            Marshal.GetLastWin32Error();
            bool isInvalid = handle.IsInvalid;
            return handle;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, [In] NativeMethods.SECURITY_ATTRIBUTES lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern SafeFileMappingHandle CreateFileMapping(SafeFileHandle hFile, NativeMethods.SECURITY_ATTRIBUTES lpFileMappingAttributes, int flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern NativeMethods.IconHandle CreateIcon(IntPtr hInstance, int nWidth, int nHeight, byte cPlanes, byte cBitsPixel, byte[] lpbANDbits, byte[] lpbXORbits);
        [SecurityCritical]
        internal static NativeMethods.IconHandle CreateIconIndirect([In, MarshalAs(UnmanagedType.LPStruct)] NativeMethods.ICONINFO iconInfo)
        {
            NativeMethods.IconHandle handle = PrivateCreateIconIndirect(iconInfo);
            Marshal.GetLastWin32Error();
            bool isInvalid = handle.IsInvalid;
            return handle;
        }

        [SecurityCritical, DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, ref System.Runtime.InteropServices.ComTypes.IStream istream);
        [SecurityCritical]
        public static IntPtr CreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName, int style, int x, int y, int width, int height, HandleRef hWndParent, HandleRef hMenu, HandleRef hInst, [MarshalAs(UnmanagedType.AsAny)] object pvParam)
        {
            IntPtr ptr = IntCreateWindowEx(dwExStyle, lpszClassName, lpszWindowName, style, x, y, width, height, hWndParent, hMenu, hInst, pvParam);
            if (ptr == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            return ptr;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("gdi32.dll", EntryPoint="CreateCompatibleBitmap", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr CriticalCreateCompatibleBitmap(HandleRef hDC, int width, int height);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("gdi32.dll", EntryPoint="CreateCompatibleDC", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr CriticalCreateCompatibleDC(HandleRef hDC);
        [SecurityCritical]
        public static void CriticalDeleteDC(HandleRef hDC)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) hDC, NativeMethods.CommonHandles.HDC);
            if (!IntCriticalDeleteDC(hDC))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="FillRect", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int CriticalFillRect(IntPtr hdc, ref NativeMethods.RECT rcFill, IntPtr brush);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("gdi32.dll", EntryPoint="GetStockObject", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr CriticalGetStockObject(int stockObject);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="PrintWindow", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool CriticalPrintWindow(HandleRef hWnd, HandleRef hDC, int flags);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="RedrawWindow", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool CriticalRedrawWindow(HandleRef hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, int flags);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("gdi32.dll", EntryPoint="SelectObject", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr CriticalSelectObject(HandleRef hdc, IntPtr obj);
        [SecurityCritical]
        internal static IntPtr CriticalSetWindowLong(HandleRef hWnd, int nIndex, NativeMethods.WndProc dwNewLong)
        {
            int num;
            IntPtr ptr;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                int num2 = IntCriticalSetWindowLong(hWnd, nIndex, dwNewLong);
                num = Marshal.GetLastWin32Error();
                ptr = new IntPtr(num2);
            }
            else
            {
                ptr = IntCriticalSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                num = Marshal.GetLastWin32Error();
            }
            if ((ptr == IntPtr.Zero) && (num != 0))
            {
                throw new Win32Exception(num);
            }
            return ptr;
        }

        [SecurityCritical]
        internal static IntPtr CriticalSetWindowLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(IntCriticalSetWindowLong(hWnd, nIndex, NativeMethods.IntPtrToInt32(dwNewLong)));
            }
            return IntCriticalSetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("uxtheme.dll", EntryPoint="SetWindowTheme", CharSet=CharSet.Auto)]
        public static extern int CriticalSetWindowTheme(HandleRef hWnd, string subAppName, string subIdList);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="DefWindowProcW", CharSet=CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [SecurityTreatAsSafe, SecurityCritical]
        public static void DeleteDC(HandleRef hDC)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) hDC, NativeMethods.CommonHandles.HDC);
            if (!IntDeleteDC(hDC))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical]
        public static bool DeleteObject(IntPtr hObject)
        {
            bool flag = IntDeleteObject(hObject);
            Marshal.GetLastWin32Error();
            return flag;
        }

        [SecurityCritical]
        public static void DeleteObject(HandleRef hObject)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) hObject, NativeMethods.CommonHandles.GDI);
            if (!IntDeleteObject(hObject))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical]
        public static bool DeleteObjectNoThrow(HandleRef hObject)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) hObject, NativeMethods.CommonHandles.GDI);
            bool flag = IntDeleteObject(hObject);
            Marshal.GetLastWin32Error();
            return flag;
        }

        [SecurityCritical]
        public static bool DestroyCursor(IntPtr hCurs) => 
            IntDestroyCursor(hCurs);

        [SecurityCritical]
        public static bool DestroyIcon(IntPtr hIcon)
        {
            bool flag = IntDestroyIcon(hIcon);
            Marshal.GetLastWin32Error();
            return flag;
        }

        [SecurityCritical]
        internal static bool DestroyIcon(HandleRef handle)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) handle, NativeMethods.CommonHandles.Icon);
            bool flag = PrivateDestroyIcon(handle);
            Marshal.GetLastWin32Error();
            return flag;
        }

        [SecurityCritical]
        public static void DestroyWindow(HandleRef hWnd)
        {
            if (!IntDestroyWindow(hWnd))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr DispatchMessage([In] ref MSG msg);
        [DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true, PreserveSig=false)]
        public static extern void DoDragDrop(IDataObject dataObject, IOleDropSource dropSource, int allowedEffects, int[] finalEffect);
        [DllImport("shell32.dll", CharSet=CharSet.Auto)]
        public static extern int DragQueryFile(HandleRef hDrop, int iFile, StringBuilder lpszFile, int cch);
        [SecurityCritical, DllImport("kernel32.dll", SetLastError=true)]
        public static extern bool DuplicateHandle(IntPtr hSourceProcess, SafeWaitHandle hSourceHandle, IntPtr hTargetProcessHandle, out IntPtr hTargetHandle, uint dwDesiredAccess, bool fInheritHandle, uint dwOptions);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("dwmapi.dll")]
        public static extern int DwmIsCompositionEnabled(out int enabled);
        [SecurityCritical]
        public static bool EnableWindow(HandleRef hWnd, bool enable)
        {
            SetLastError(0);
            bool flag = IntEnableWindow(hWnd, enable);
            if (!flag)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return flag;
        }

        [SecurityCritical]
        public static bool EnableWindowNoThrow(HandleRef hWnd, bool enable) => 
            IntEnableWindowNoThrow(hWnd, enable);

        [SecurityCritical, SuppressUnmanagedCodeSecurity]
        public static bool EndPaint(HandleRef hWnd, [In, MarshalAs(UnmanagedType.LPStruct)] ref NativeMethods.PAINTSTRUCT lpPaint)
        {
            MS.Win32.HandleCollector.Remove(lpPaint.hdc, NativeMethods.CommonHandles.HDC);
            return IntEndPaint(hWnd, ref lpPaint);
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool EnumThreadWindows(int dwThreadId, NativeMethods.EnumThreadWindowsCallback lpfn, HandleRef lParam);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("shell32.dll", CharSet=CharSet.Auto)]
        internal static extern int ExtractIconEx(string stExeFileName, int nIconIndex, ref NativeMethods.IconHandle phiconLarge, ref NativeMethods.IconHandle phiconSmall, int nIcons);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
        internal static extern int FindNLSString(int locale, uint flags, [MarshalAs(UnmanagedType.LPWStr)] string sourceString, int sourceCount, [MarshalAs(UnmanagedType.LPWStr)] string findString, int findCount, out int found);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern IntPtr GetActiveWindow();
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern IntPtr GetAncestor(HandleRef hWnd, int flags);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern int GetBitmapBits(HandleRef hbmp, int cbBuffer, byte[] lpvBits);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int GetClassName(HandleRef hwnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int GetClipboardFormatName(int format, StringBuilder lpString, int cchMax);
        [SecurityCritical, DllImport("kernel32.dll", SetLastError=true)]
        public static extern IntPtr GetCurrentProcess();
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("uxtheme.dll", CharSet=CharSet.Auto)]
        public static extern int GetCurrentThemeName(StringBuilder pszThemeFileName, int dwMaxNameChars, StringBuilder pszColorBuff, int dwMaxColorChars, StringBuilder pszSizeBuff, int cchMaxSizeChars);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern IntPtr GetCurrentThread();
        [SecurityCritical]
        internal static bool GetCursorPos([In, Out] NativeMethods.POINT pt)
        {
            bool flag = IntGetCursorPos(pt);
            if (!flag)
            {
                throw new Win32Exception();
            }
            return flag;
        }

        [SecurityCritical]
        public static IntPtr GetDC(HandleRef hWnd)
        {
            IntPtr handle = IntGetDC(hWnd);
            if (handle == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            return MS.Win32.HandleCollector.Add(handle, NativeMethods.CommonHandles.HDC);
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern IntPtr GetDesktopWindow();
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern int GetDeviceCaps(HandleRef hDC, int nIndex);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", SetLastError=true)]
        internal static extern bool GetFileSizeEx(SafeFileHandle hFile, ref LARGE_INTEGER lpFileSize);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern IntPtr GetFocus();
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern IntPtr GetForegroundWindow();
        [SecurityCritical]
        internal static void GetIconInfo(HandleRef hIcon, out NativeMethods.ICONINFO piconinfo)
        {
            bool iconInfoImpl = false;
            piconinfo = new NativeMethods.ICONINFO();
            ICONINFO_IMPL iconinfo_impl = new ICONINFO_IMPL();
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                iconInfoImpl = GetIconInfoImpl(hIcon, iconinfo_impl);
                Marshal.GetLastWin32Error();
                if (iconInfoImpl)
                {
                    piconinfo.hbmMask = new NativeMethods.BitmapHandle(iconinfo_impl.hbmMask, true);
                    piconinfo.hbmColor = new NativeMethods.BitmapHandle(iconinfo_impl.hbmColor, true);
                    piconinfo.fIcon = iconinfo_impl.fIcon;
                    piconinfo.xHotspot = iconinfo_impl.xHotspot;
                    piconinfo.yHotspot = iconinfo_impl.yHotspot;
                }
            }
            if (!iconInfoImpl)
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("user32.dll", EntryPoint="GetIconInfo", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool GetIconInfoImpl(HandleRef hIcon, [Out] ICONINFO_IMPL piconinfo);
        [SecurityCritical]
        public static void GetKeyboardState(byte[] keystate)
        {
            if (IntGetKeyboardState(keystate) == 0)
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern short GetKeyState(int keyCode);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll")]
        public static extern bool GetLayeredWindowAttributes(HandleRef hwnd, IntPtr pcrKey, IntPtr pbAlpha, IntPtr pdwFlags);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        internal static extern int GetLocaleInfoW(int locale, int type, string data, int dataSize);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto)]
        internal static extern IntPtr GetMessageExtraInfo();
        [SecurityCritical]
        public static bool GetMessageW([In, Out] ref MSG msg, HandleRef hWnd, int uMsgFilterMin, int uMsgFilterMax)
        {
            switch (IntGetMessageW(ref msg, hWnd, uMsgFilterMin, uMsgFilterMax))
            {
                case -1:
                    throw new Win32Exception();

                case 0:
                    return false;
            }
            return true;
        }

        [SecurityCritical]
        internal static int GetModuleFileName(HandleRef hModule, StringBuilder buffer, int length)
        {
            int num = IntGetModuleFileName(hModule, buffer, length);
            if (num == 0)
            {
                throw new Win32Exception();
            }
            return num;
        }

        [SecurityCritical]
        internal static IntPtr GetModuleHandle(string modName)
        {
            IntPtr ptr = IntGetModuleHandle(modName);
            if (ptr == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            return ptr;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        internal static extern int GetMouseMovePointsEx(uint cbSize, [In] ref NativeMethods.MOUSEMOVEPOINT pointsIn, [Out] NativeMethods.MOUSEMOVEPOINT[] pointsBufferOut, int nBufPoints, uint resolution);
        [DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int GetObject(HandleRef hObject, int nSize, [In, Out] NativeMethods.BITMAP bm);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        internal static extern int GetOEMCP();
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("comdlg32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern bool GetOpenFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);
        [SecurityCritical]
        internal static IntPtr GetParent(HandleRef hWnd)
        {
            SetLastError(0);
            IntPtr ptr = IntGetParent(hWnd);
            int error = Marshal.GetLastWin32Error();
            if ((ptr == IntPtr.Zero) && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return ptr;
        }

        [SecurityCritical]
        public static IntPtr GetProcAddress(HandleRef hModule, string lpProcName)
        {
            IntPtr ptr = IntGetProcAddress(hModule, lpProcName);
            if (ptr == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            return ptr;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", EntryPoint="GetProcAddress", CharSet=CharSet.Ansi)]
        public static extern IntPtr GetProcAddressNoThrow(HandleRef hModule, string lpProcName);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint command, [In] ref NativeMethods.RID_DEVICE_INFO ridInfo, ref uint sizeInBytes);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern uint GetRawInputDeviceList([In, Out] NativeMethods.RAWINPUTDEVICELIST[] ridl, [In, Out] ref uint numDevices, uint sizeInBytes);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("comdlg32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern bool GetSaveFileName([In, Out] NativeMethods.OPENFILENAME_I ofn);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int GetSystemMetrics(int nIndex);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool GetSystemPowerStatus(ref NativeMethods.SYSTEM_POWER_STATUS systemPowerStatus);
        [SecurityCritical]
        internal static uint GetTempFileName(string tmpPath, string prefix, uint uniqueIdOrZero, StringBuilder tmpFileName)
        {
            uint num = _GetTempFileName(tmpPath, prefix, uniqueIdOrZero, tmpFileName);
            if (num == 0)
            {
                throw new Win32Exception();
            }
            return num;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("Wininet.dll", EntryPoint="GetUrlCacheConfigInfoW", SetLastError=true)]
        internal static extern bool GetUrlCacheConfigInfo(ref NativeMethods.InternetCacheConfigInfo pInternetCacheConfigInfo, ref uint cbCacheConfigInfo, uint fieldControl);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", CharSet=CharSet.Unicode)]
        internal static extern bool GetVersionEx([In, Out] NativeMethods.OSVERSIONINFOEX ver);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);
        [SecurityCritical]
        internal static int GetWindowLong(HandleRef hWnd, int nIndex)
        {
            int num = 0;
            IntPtr zero = IntPtr.Zero;
            int num2 = 0;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                num = IntGetWindowLong(hWnd, nIndex);
                num2 = Marshal.GetLastWin32Error();
                zero = new IntPtr(num);
            }
            else
            {
                zero = IntGetWindowLongPtr(hWnd, nIndex);
                num2 = Marshal.GetLastWin32Error();
                num = NativeMethods.IntPtrToInt32(zero);
            }
            if (zero == IntPtr.Zero)
            {
            }
            return num;
        }

        [SecurityCritical]
        internal static IntPtr GetWindowLongPtr(HandleRef hWnd, int nIndex)
        {
            IntPtr zero = IntPtr.Zero;
            int num = 0;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                int num2 = IntGetWindowLong(hWnd, nIndex);
                num = Marshal.GetLastWin32Error();
                zero = new IntPtr(num2);
            }
            else
            {
                zero = IntGetWindowLongPtr(hWnd, nIndex);
                num = Marshal.GetLastWin32Error();
            }
            if (zero == IntPtr.Zero)
            {
            }
            return zero;
        }

        [SecurityCritical]
        internal static NativeMethods.WndProc GetWindowLongWndProc(HandleRef hWnd)
        {
            NativeMethods.WndProc proc = null;
            int error = 0;
            SetLastError(0);
            if (IntPtr.Size == 4)
            {
                proc = IntGetWindowLongWndProc(hWnd, -4);
                error = Marshal.GetLastWin32Error();
            }
            else
            {
                proc = IntGetWindowLongWndProcPtr(hWnd, -4);
                error = Marshal.GetLastWin32Error();
            }
            if (proc == null)
            {
                throw new Win32Exception(error);
            }
            return proc;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static void GetWindowPlacement(HandleRef hWnd, ref NativeMethods.WINDOWPLACEMENT placement)
        {
            if (!IntGetWindowPlacement(hWnd, ref placement))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical]
        internal static int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount)
        {
            SetLastError(0);
            int num = IntGetWindowText(hWnd, lpString, nMaxCount);
            if (num == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return num;
        }

        [SecurityCritical]
        internal static int GetWindowTextLength(HandleRef hWnd)
        {
            SetLastError(0);
            int num = IntGetWindowTextLength(hWnd);
            if (num == 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 0)
                {
                    throw new Win32Exception(error);
                }
            }
            return num;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int GetWindowThreadProcessId(HandleRef hWnd, out int lpdwProcessId);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr GlobalAlloc(int uFlags, IntPtr dwBytes);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr GlobalFree(HandleRef handle);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr GlobalLock(HandleRef handle);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr GlobalReAlloc(HandleRef handle, IntPtr bytes, int flags);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr GlobalSize(HandleRef handle);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool GlobalUnlock(HandleRef handle);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr ImmAssociateContext(HandleRef hWnd, HandleRef hIMC);
        [DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmConfigureIME(HandleRef hkl, HandleRef hwnd, int dwData, IntPtr pvoid);
        [DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmConfigureIME(HandleRef hkl, HandleRef hwnd, int dwData, [In] ref NativeMethods.REGISTERWORD registerWord);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmGetCompositionString(HandleRef hIMC, int dwIndex, byte[] lpBuf, int dwBufLen);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmGetCompositionString(HandleRef hIMC, int dwIndex, char[] lpBuf, int dwBufLen);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmGetCompositionString(HandleRef hIMC, int dwIndex, int[] lpBuf, int dwBufLen);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmGetCompositionString(HandleRef hIMC, int dwIndex, IntPtr lpBuf, int dwBufLen);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr ImmGetContext(HandleRef hWnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern bool ImmGetConversionStatus(HandleRef hIMC, ref int conversion, ref int sentence);
        [DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr ImmGetDefaultIMEWnd(HandleRef hwnd);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern bool ImmGetOpenStatus(HandleRef hIMC);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmGetProperty(HandleRef hkl, int flags);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern bool ImmNotifyIME(HandleRef hIMC, int dwAction, int dwIndex, int dwValue);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern bool ImmReleaseContext(HandleRef hWnd, HandleRef hIMC);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmSetCandidateWindow(HandleRef hIMC, [In, Out] ref NativeMethods.CANDIDATEFORM candform);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern int ImmSetCompositionWindow(HandleRef hIMC, [In, Out] ref NativeMethods.COMPOSITIONFORM compform);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern bool ImmSetConversionStatus(HandleRef hIMC, int conversion, int sentence);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("imm32.dll", CharSet=CharSet.Auto)]
        public static extern bool ImmSetOpenStatus(HandleRef hIMC, bool open);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="BeginPaint", CharSet=CharSet.Auto, ExactSpelling=true)]
        private static extern IntPtr IntBeginPaint(HandleRef hWnd, [In, Out] ref NativeMethods.PAINTSTRUCT lpPaint);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="ClientToScreen", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern int IntClientToScreen(HandleRef hWnd, [In, Out] NativeMethods.POINT pt);
        [DllImport("kernel32.dll", EntryPoint="CloseHandle", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool IntCloseHandle(HandleRef handle);
        [DllImport("gdi32.dll", EntryPoint="CreateCompatibleDC", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr IntCreateCompatibleDC(HandleRef hDC);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="CreateWindowEx", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern IntPtr IntCreateWindowEx(int dwExStyle, string lpszClassName, string lpszWindowName, int style, int x, int y, int width, int height, HandleRef hWndParent, HandleRef hMenu, HandleRef hInst, [MarshalAs(UnmanagedType.AsAny)] object pvParam);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("gdi32.dll", EntryPoint="DeleteDC", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern bool IntCriticalDeleteDC(HandleRef hDC);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="SetWindowLong", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntCriticalSetWindowLong(HandleRef hWnd, int nIndex, NativeMethods.WndProc dwNewLong);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="SetWindowLong", CharSet=CharSet.Auto)]
        private static extern int IntCriticalSetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="SetWindowLongPtr", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr IntCriticalSetWindowLongPtr(HandleRef hWnd, int nIndex, NativeMethods.WndProc dwNewLong);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="SetWindowLongPtr", CharSet=CharSet.Auto)]
        private static extern IntPtr IntCriticalSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("gdi32.dll", EntryPoint="DeleteDC", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern bool IntDeleteDC(HandleRef hDC);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("gdi32.dll", EntryPoint="DeleteObject", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool IntDeleteObject(IntPtr hObject);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("gdi32.dll", EntryPoint="DeleteObject", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool IntDeleteObject(HandleRef hObject);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="DestroyCursor", CharSet=CharSet.Auto, ExactSpelling=true)]
        private static extern bool IntDestroyCursor(IntPtr hCurs);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="DestroyIcon", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool IntDestroyIcon(IntPtr hIcon);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="DestroyWindow", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool IntDestroyWindow(HandleRef hWnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="EnableWindow", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool IntEnableWindow(HandleRef hWnd, bool enable);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="EnableWindow", CharSet=CharSet.Auto)]
        public static extern bool IntEnableWindowNoThrow(HandleRef hWnd, bool enable);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="EndPaint", CharSet=CharSet.Auto, ExactSpelling=true)]
        private static extern bool IntEndPaint(HandleRef hWnd, ref NativeMethods.PAINTSTRUCT lpPaint);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("wininet.dll", EntryPoint="InternetGetCookieExW", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
        internal static extern bool InternetGetCookieEx([In] string Url, [In] string cookieName, [Out] StringBuilder cookieData, [In, Out] ref uint pchCookieData, uint flags, IntPtr reserved);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("wininet.dll", EntryPoint="InternetSetCookieExW", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
        internal static extern uint InternetSetCookieEx([In] string Url, [In] string CookieName, [In] string cookieData, uint flags, [In] string p3pHeader);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="GetCursorPos", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern bool IntGetCursorPos([In, Out] NativeMethods.POINT pt);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="GetDC", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern IntPtr IntGetDC(HandleRef hWnd);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="GetKeyboardState", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntGetKeyboardState(byte[] keystate);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="GetMessageW", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
        private static extern int IntGetMessageW([In, Out] ref MSG msg, HandleRef hWnd, int uMsgFilterMin, int uMsgFilterMax);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", EntryPoint="GetModuleFileName", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntGetModuleFileName(HandleRef hModule, StringBuilder buffer, int length);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", EntryPoint="GetModuleHandle", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr IntGetModuleHandle(string modName);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="GetParent", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern IntPtr IntGetParent(HandleRef hWnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", EntryPoint="GetProcAddress", CharSet=CharSet.Ansi, SetLastError=true)]
        public static extern IntPtr IntGetProcAddress(HandleRef hModule, string lpProcName);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="GetWindowLong", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntGetWindowLong(HandleRef hWnd, int nIndex);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="GetWindowLongPtr", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr IntGetWindowLongPtr(HandleRef hWnd, int nIndex);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="GetWindowLong", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern NativeMethods.WndProc IntGetWindowLongWndProc(HandleRef hWnd, int nIndex);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="GetWindowLongPtr", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern NativeMethods.WndProc IntGetWindowLongWndProcPtr(HandleRef hWnd, int nIndex);
        [DllImport("user32.dll", EntryPoint="GetWindowPlacement", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern bool IntGetWindowPlacement(HandleRef hWnd, ref NativeMethods.WINDOWPLACEMENT placement);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="GetWindowText", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntGetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="GetWindowTextLength", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntGetWindowTextLength(HandleRef hWnd);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="MsgWaitForMultipleObjectsEx", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern int IntMsgWaitForMultipleObjectsEx(int nCount, IntPtr[] pHandles, int dwMilliseconds, int dwWakeMask, int dwFlags);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("ole32.dll", EntryPoint="OleInitialize")]
        private static extern int IntOleInitialize(IntPtr val);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="PostMessage", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool IntPostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="PostThreadMessage", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntPostThreadMessage(int id, int msg, IntPtr wparam, IntPtr lparam);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="RegisterClassEx", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern ushort IntRegisterClassEx(NativeMethods.WNDCLASSEX_D wc_d);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="ReleaseDC", CharSet=CharSet.Auto, ExactSpelling=true)]
        private static extern int IntReleaseDC(HandleRef hWnd, HandleRef hDC);
        [DllImport("user32.dll", EntryPoint="SetActiveWindow", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern IntPtr IntSetActiveWindow(HandleRef hWnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="SetFocus", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern IntPtr IntSetFocus(HandleRef hWnd);
        [DllImport("user32.dll", EntryPoint="SetWindowLong", CharSet=CharSet.Auto)]
        private static extern int IntSetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint="SetWindowLongPtr", CharSet=CharSet.Auto)]
        private static extern IntPtr IntSetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll", EntryPoint="SetWindowPlacement", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern bool IntSetWindowPlacement(HandleRef hWnd, [In] ref NativeMethods.WINDOWPLACEMENT placement);
        [DllImport("user32.dll", EntryPoint="SetWindowText", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool IntSetWindowText(HandleRef hWnd, string text);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="GetCursorPos", CharSet=CharSet.Auto, ExactSpelling=true)]
        private static extern bool IntTryGetCursorPos([In, Out] NativeMethods.POINT pt);
        [DllImport("kernel32.dll", EntryPoint="UnmapViewOfFile", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern bool IntUnmapViewOfFile(HandleRef pvBaseAddress);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="UnregisterClass", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern int IntUnregisterClass(IntPtr atomString, IntPtr hInstance);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", EntryPoint="WaitForMultipleObjectsEx", CharSet=CharSet.Auto, SetLastError=true)]
        private static extern int IntWaitForMultipleObjectsEx(int nCount, IntPtr[] pHandles, bool bWaitAll, int dwMilliseconds, bool bAlertable);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="WindowFromPoint", CharSet=CharSet.Auto, ExactSpelling=true)]
        private static extern IntPtr IntWindowFromPoint(POINTSTRUCT pt);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool InvalidateRect(HandleRef hWnd, IntPtr rect, bool erase);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool IsChild(HandleRef hWndParent, HandleRef hwnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool IsWindow(HandleRef hWnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll")]
        internal static extern bool IsWinEventHookInstalled(int winevent);
        [DllImport("user32.dll", EntryPoint="LoadImage", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern NativeMethods.CursorHandle LoadImageCursor(IntPtr hinst, string stName, int nType, int cxDesired, int cyDesired, int nFlags);
        [DllImport("user32.dll", EntryPoint="LoadImage", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern NativeMethods.IconHandle LoadImageIcon(IntPtr hinst, string stName, int nType, int cxDesired, int cyDesired, int nFlags);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", CharSet=CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", SetLastError=true)]
        internal static extern IntPtr LocalFree(IntPtr hMem);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", SetLastError=true)]
        internal static extern SafeViewOfFileHandle MapViewOfFileEx(SafeFileMappingHandle hFileMappingObject, int dwDesiredAccess, int dwFileOffsetHigh, int dwFileOffsetLow, IntPtr dwNumberOfBytesToMap, IntPtr lpBaseAddress);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int MessageBox(HandleRef hWnd, string text, string caption, int type);
        [SecurityCritical]
        internal static int MsgWaitForMultipleObjectsEx(int nCount, IntPtr[] pHandles, int dwMilliseconds, int dwWakeMask, int dwFlags)
        {
            int num = IntMsgWaitForMultipleObjectsEx(nCount, pHandles, dwMilliseconds, dwWakeMask, dwFlags);
            if (num == -1)
            {
                throw new Win32Exception();
            }
            return num;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
        public static extern int MultiByteToWideChar(int CodePage, int dwFlags, byte[] lpMultiByteStr, int cchMultiByte, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder lpWideCharStr, int cchWideChar);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("naturallanguage6.dll", EntryPoint="DllGetClassObject", PreserveSig=false)]
        internal static extern void NaturalLanguageDllGetClassObject(ref Guid clsid, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out object classObject);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern void NotifyWinEvent(int winEvent, HandleRef hwnd, int objType, int objID);
        [SecurityTreatAsSafe, SecurityCritical]
        internal static void NtCheck(int err)
        {
            if (!NtSuccess(err))
            {
                throw new Win32Exception(RtlNtStatusToDosError(err));
            }
        }

        internal static bool NtSuccess(int err) => 
            (err >= 0);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("oleacc.dll")]
        internal static extern int ObjectFromLresult(IntPtr lResult, ref Guid iid, IntPtr wParam, [In, Out] ref IAccessible ppvObject);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("urlmon.dll", CharSet=CharSet.Ansi, ExactSpelling=true)]
        internal static extern int ObtainUserAgentString(int dwOption, StringBuilder userAgent, ref int length);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int OleFlushClipboard();
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int OleGetClipboard(ref IDataObject data);
        [SecurityCritical]
        public static int OleInitialize() => 
            IntOleInitialize(IntPtr.Zero);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int OleIsCurrentClipboard(IDataObject pDataObj);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int OleSetClipboard(IDataObject pDataObj);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("ole32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern int OleUninitialize();
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern SafeFileMappingHandle OpenFileMapping(int dwDesiredAccess, bool bInheritHandle, string lpName);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern bool PeekMessage([In, Out] ref MSG msg, HandleRef hwnd, int msgMin, int msgMax, int remove);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("winmm.dll", CharSet=CharSet.Unicode)]
        internal static extern bool PlaySound([In] string soundName, IntPtr hmod, SafeNativeMethods.PlaySoundFlags soundFlags);
        [SecurityCritical]
        internal static void PostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (!IntPostMessage(hwnd, msg, wparam, lparam))
            {
                throw new Win32Exception();
            }
        }

        [SecurityCritical]
        public static void PostThreadMessage(int id, int msg, IntPtr wparam, IntPtr lparam)
        {
            if (IntPostThreadMessage(id, msg, wparam, lparam) == 0)
            {
                throw new Win32Exception();
            }
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("gdi32.dll", EntryPoint="CreateBitmap", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern NativeMethods.BitmapHandle PrivateCreateBitmap(int width, int height, int planes, int bitsPerPixel, byte[] lpvBits);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("gdi32.dll", EntryPoint="CreateDIBSection", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern NativeMethods.BitmapHandle PrivateCreateDIBSection(HandleRef hdc, ref NativeMethods.BITMAPINFO bitmapInfo, int iUsage, ref IntPtr ppvBits, IntPtr hSection, int dwOffset);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="CreateIconIndirect", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern NativeMethods.IconHandle PrivateCreateIconIndirect([In, MarshalAs(UnmanagedType.LPStruct)] NativeMethods.ICONINFO iconInfo);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="DestroyIcon", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        private static extern bool PrivateDestroyIcon(HandleRef handle);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("PresentationHostDll.dll", EntryPoint="ProcessUnhandledException")]
        internal static extern void ProcessUnhandledException_DLL([MarshalAs(UnmanagedType.BStr)] string errMsg);
        [SecurityCritical]
        public static object PtrToStructure(IntPtr lparam, Type cls) => 
            Marshal.PtrToStructure(lparam, cls);

        [SecurityCritical]
        internal static ushort RegisterClassEx(NativeMethods.WNDCLASSEX_D wc_d)
        {
            ushort num = IntRegisterClassEx(wc_d);
            if (num == 0)
            {
                throw new Win32Exception();
            }
            return num;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern int RegisterClipboardFormat(string format);
        [DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int RegisterDragDrop(HandleRef hwnd, IOleDropTarget target);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll")]
        internal static extern unsafe IntPtr RegisterPowerSettingNotification(IntPtr hRecipient, Guid* pGuid, int Flags);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int RegisterWindowMessage(string msg);
        [SecurityCritical]
        public static int ReleaseDC(HandleRef hWnd, HandleRef hDC)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) hDC, NativeMethods.CommonHandles.HDC);
            return IntReleaseDC(hWnd, hDC);
        }

        [DllImport("ole32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern int RevokeDragDrop(HandleRef hwnd);
        [DllImport("ntdll.dll")]
        internal static extern int RtlNtStatusToDosError(int Status);
        [SecurityCritical]
        internal static int SafeReleaseComObject(object o)
        {
            int num = 0;
            if ((o != null) && Marshal.IsComObject(o))
            {
                num = Marshal.ReleaseComObject(o);
            }
            return num;
        }

        [DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern IntPtr SelectObject(HandleRef hdc, IntPtr obj);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        internal static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, NativeMethods.IconHandle iconHandle);
        [SecurityTreatAsSafe, SecurityCritical]
        internal static IntPtr SetActiveWindow(HandleRef hWnd)
        {
            IntPtr ptr = IntSetActiveWindow(hWnd);
            if (ptr == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
            return ptr;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern int SetEvent([In] SafeWaitHandle hHandle);
        [DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool SetEvent(IntPtr hEvent);
        [SecurityCritical]
        internal static IntPtr SetFocus(HandleRef hWnd)
        {
            SetLastError(0);
            IntPtr ptr = IntSetFocus(hWnd);
            int error = Marshal.GetLastWin32Error();
            if ((ptr == IntPtr.Zero) && (error != 0))
            {
                throw new Win32Exception(error);
            }
            return ptr;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool SetForegroundWindow(HandleRef hWnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        internal static extern void SetLastError(int dwErrorCode);
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool SetLayeredWindowAttributes(HandleRef hwnd, int crKey, byte bAlpha, int dwFlags);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern bool SetProp(HandleRef hWnd, string propName, HandleRef data);
        [SecurityTreatAsSafe, SecurityCritical]
        internal static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return new IntPtr(IntSetWindowLong(hWnd, nIndex, NativeMethods.IntPtrToInt32(dwNewLong)));
            }
            return IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal static void SetWindowPlacement(HandleRef hWnd, [In] ref NativeMethods.WINDOWPLACEMENT placement)
        {
            if (!IntSetWindowPlacement(hWnd, ref placement))
            {
                throw new Win32Exception();
            }
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool SetWindowPos(HandleRef hWnd, HandleRef hWndInsertAfter, int x, int y, int cx, int cy, int flags);
        [SecurityTreatAsSafe, SecurityCritical]
        internal static void SetWindowText(HandleRef hWnd, string text)
        {
            if (!IntSetWindowText(hWnd, text))
            {
                throw new Win32Exception();
            }
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll")]
        internal static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, NativeMethods.WinEventProcDef WinEventReentrancyFilter, uint idProcess, uint idThread, int dwFlags);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("shell32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr ShellExecute(HandleRef hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("shell32.dll", CharSet=CharSet.Unicode, SetLastError=true)]
        internal static extern bool ShellExecuteEx([In, Out] ShellExecuteInfo lpExecInfo);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool ShowCaret(HandleRef hwnd);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool ShowWindow(HandleRef hWnd, int nCmdShow);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [SecurityCritical]
        public static void StructureToPtr(object structure, IntPtr ptr, bool fDeleteOld)
        {
            Marshal.StructureToPtr(structure, ptr, fDeleteOld);
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref NativeMethods.HIGHCONTRAST_I rc, int nUpdate);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref NativeMethods.RECT rc, int nUpdate);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref bool value, int ignore);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, ref int value, int ignore);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto)]
        internal static extern bool SystemParametersInfo(int nAction, int nParam, [In, Out] NativeMethods.ANIMATIONINFO anim, int nUpdate);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto)]
        internal static extern bool SystemParametersInfo(int nAction, int nParam, [In, Out] NativeMethods.ICONMETRICS metrics, int nUpdate);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool SystemParametersInfo(int nAction, int nParam, [In, Out] NativeMethods.NONCLIENTMETRICS metrics, int nUpdate);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("msctf.dll")]
        public static extern int TF_CreateCategoryMgr(out ITfCategoryMgr catmgr);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("msctf.dll")]
        public static extern int TF_CreateDisplayAttributeMgr(out ITfDisplayAttributeMgr dam);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("msctf.dll")]
        public static extern int TF_CreateInputProcessorProfiles(out ITfInputProcessorProfiles profiles);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("msctf.dll")]
        internal static extern int TF_CreateThreadMgr(out ITfThreadMgr threadManager);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
        public static extern bool TranslateMessage([In, Out] ref MSG msg);
        [SecurityCritical]
        internal static bool TryGetCursorPos([In, Out] NativeMethods.POINT pt)
        {
            bool flag = IntTryGetCursorPos(pt);
            if (!flag)
            {
                pt.x = 0;
                pt.y = 0;
            }
            return flag;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", EntryPoint="PostMessage", CharSet=CharSet.Auto)]
        internal static extern bool TryPostMessage(HandleRef hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [SecurityCritical]
        internal static bool TrySetFocus(HandleRef hWnd, ref IntPtr result)
        {
            SetLastError(0);
            result = IntSetFocus(hWnd);
            int num = Marshal.GetLastWin32Error();
            if ((result == IntPtr.Zero) && (num != 0))
            {
                return false;
            }
            return true;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
        internal static extern bool UnhookWinEvent(IntPtr winEventHook);
        [SecurityCritical]
        public static bool UnmapViewOfFileNoThrow(HandleRef pvBaseAddress)
        {
            MS.Win32.HandleCollector.Remove((IntPtr) pvBaseAddress, NativeMethods.CommonHandles.Kernel);
            bool flag = IntUnmapViewOfFile(pvBaseAddress);
            Marshal.GetLastWin32Error();
            return flag;
        }

        [SecurityCritical]
        internal static void UnregisterClass(IntPtr atomString, IntPtr hInstance)
        {
            if (IntUnregisterClass(atomString, hInstance) == 0)
            {
                throw new Win32Exception();
            }
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        internal static extern IntPtr UnsafeSendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, NativeMethods.POINT pptDst, NativeMethods.POINT pSizeDst, IntPtr hdcSrc, NativeMethods.POINT pptSrc, int crKey, ref NativeMethods.BLENDFUNCTION pBlend, int dwFlags);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("oleaut32.dll")]
        private static extern int VariantClear(IntPtr pObject);
        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", SetLastError=true)]
        internal static extern IntPtr VirtualAlloc(IntPtr lpAddress, UIntPtr dwSize, int flAllocationType, int flProtect);
        [SecurityCritical]
        internal static int WaitForMultipleObjectsEx(int nCount, IntPtr[] pHandles, bool bWaitAll, int dwMilliseconds, bool bAlertable)
        {
            int num = IntWaitForMultipleObjectsEx(nCount, pHandles, bWaitAll, dwMilliseconds, bAlertable);
            if (num == -1)
            {
                throw new Win32Exception();
            }
            return num;
        }

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
        public static extern int WaitForSingleObject([In] SafeWaitHandle hHandle, [In] int dwMilliseconds);
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll", CharSet=CharSet.Unicode, SetLastError=true, ExactSpelling=true)]
        public static extern int WideCharToMultiByte(int codePage, int flags, [MarshalAs(UnmanagedType.LPWStr)] string wideStr, int chars, [In, Out] byte[] pOutBytes, int bufferBytes, IntPtr defaultChar, IntPtr pDefaultUsed);
        [SecurityCritical]
        public static IntPtr WindowFromPoint(int x, int y)
        {
            POINTSTRUCT pt = new POINTSTRUCT(x, y);
            return IntWindowFromPoint(pt);
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("WtsApi32.dll")]
        public static extern bool WTSRegisterSessionNotification(IntPtr hwnd, uint dwFlags);
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("WtsApi32.dll")]
        public static extern bool WTSUnRegisterSessionNotification(IntPtr hwnd);

        [Flags]
        public enum AdviseFlags
        {
            TS_AS_ATTR_CHANGE = 8,
            TS_AS_LAYOUT_CHANGE = 4,
            TS_AS_SEL_CHANGE = 2,
            TS_AS_STATUS_CHANGE = 0x10,
            TS_AS_TEXT_CHANGE = 1
        }

        internal static class ArrayToVARIANTHelper
        {
            private static readonly int VariantSize = ((int) Marshal.OffsetOf(typeof(FindSizeOfVariant), "b"));

            [SecurityCritical]
            public static unsafe IntPtr ArrayToVARIANTVector(object[] args)
            {
                IntPtr zero = IntPtr.Zero;
                int index = 0;
                try
                {
                    int length = args.Length;
                    zero = Marshal.AllocCoTaskMem(length * VariantSize);
                    byte* numPtr = (byte*) zero;
                    index = 0;
                    while (index < length)
                    {
                        Marshal.GetNativeVariantForObject(args[index], (IntPtr) (numPtr + (VariantSize * index)));
                        index++;
                    }
                }
                catch
                {
                    if (zero != IntPtr.Zero)
                    {
                        FreeVARIANTVector(zero, index);
                    }
                    throw;
                }
                return zero;
            }

            [SecurityCritical]
            public static unsafe void FreeVARIANTVector(IntPtr mem, int len)
            {
                int hr = 0;
                byte* numPtr = (byte*) mem;
                for (int i = 0; i < len; i++)
                {
                    int num3 = 0;
                    num3 = MS.Win32.UnsafeNativeMethods.VariantClear((IntPtr) (numPtr + (VariantSize * i)));
                    if (NativeMethods.Succeeded(hr) && NativeMethods.Failed(num3))
                    {
                        hr = num3;
                    }
                }
                Marshal.FreeCoTaskMem(mem);
                if (NativeMethods.Failed(hr))
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }

            [StructLayout(LayoutKind.Sequential, Pack=1)]
            private struct FindSizeOfVariant
            {
                [MarshalAs(UnmanagedType.Struct)]
                public object var;
                public byte b;
            }
        }

        [Flags]
        public enum AttributeFlags
        {
            TS_ATTR_FIND_BACKWARDS = 1,
            TS_ATTR_FIND_HIDDEN = 0x20,
            TS_ATTR_FIND_UPDATESTART = 4,
            TS_ATTR_FIND_WANT_END = 0x10,
            TS_ATTR_FIND_WANT_OFFSET = 2,
            TS_ATTR_FIND_WANT_VALUE = 8
        }

        [Flags]
        internal enum BrowserNavConstants : uint
        {
            AllowAutosearch = 0x10,
            BrowserBar = 0x20,
            EnforceRestricted = 0x80,
            Hyperlink = 0x40,
            KeepWordWheelText = 0x2000,
            NewWindowsManaged = 0x100,
            NoHistory = 2,
            NoReadFromCache = 4,
            NoWriteToCache = 8,
            OpenInBackgroundTab = 0x1000,
            OpenInNewTab = 0x800,
            OpenInNewWindow = 1,
            TrustedForActiveX = 0x400,
            UntrustedForDownload = 0x200
        }

        [Flags]
        public enum ConversionModeFlags
        {
            TF_CONVERSIONMODE_ALPHANUMERIC = 0,
            TF_CONVERSIONMODE_CHARCODE = 0x20,
            TF_CONVERSIONMODE_EUDC = 0x200,
            TF_CONVERSIONMODE_FIXED = 0x800,
            TF_CONVERSIONMODE_FULLSHAPE = 8,
            TF_CONVERSIONMODE_KATAKANA = 2,
            TF_CONVERSIONMODE_NATIVE = 1,
            TF_CONVERSIONMODE_NOCONVERSION = 0x100,
            TF_CONVERSIONMODE_ROMAN = 0x10,
            TF_CONVERSIONMODE_SYMBOL = 0x400
        }

        [Flags]
        public enum CreateContextFlags
        {
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIDispatch), TypeLibType(TypeLibTypeFlags.FHidden), Guid("34A715A0-6587-11D0-924A-0020AFC7AC4D")]
        public interface DWebBrowserEvents2
        {
            [DispId(0x66)]
            void StatusTextChange([In] string text);
            [DispId(0x6c)]
            void ProgressChange([In] int progress, [In] int progressMax);
            [DispId(0x69)]
            void CommandStateChange([In] long command, [In] bool enable);
            [DispId(0x6a)]
            void DownloadBegin();
            [DispId(0x68)]
            void DownloadComplete();
            [DispId(0x71)]
            void TitleChange([In] string text);
            [DispId(0x70)]
            void PropertyChange([In] string szProperty);
            [DispId(0xe1)]
            void PrintTemplateInstantiation([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
            [DispId(0xe2)]
            void PrintTemplateTeardown([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp);
            [DispId(0xe3)]
            void UpdatePageStatus([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object nPage, [In] ref object fDone);
            [DispId(250)]
            void BeforeNavigate2([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers, [In, Out] ref bool cancel);
            [DispId(0xfb)]
            void NewWindow2([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object pDisp, [In, Out] ref bool cancel);
            [DispId(0xfc)]
            void NavigateComplete2([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);
            [DispId(0x103)]
            void DocumentComplete([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL);
            [DispId(0xfd)]
            void OnQuit();
            [DispId(0xfe)]
            void OnVisible([In] bool visible);
            [DispId(0xff)]
            void OnToolBar([In] bool toolBar);
            [DispId(0x100)]
            void OnMenuBar([In] bool menuBar);
            [DispId(0x101)]
            void OnStatusBar([In] bool statusBar);
            [DispId(0x102)]
            void OnFullScreen([In] bool fullScreen);
            [DispId(260)]
            void OnTheaterMode([In] bool theaterMode);
            [DispId(0x106)]
            void WindowSetResizable([In] bool resizable);
            [DispId(0x108)]
            void WindowSetLeft([In] int left);
            [DispId(0x109)]
            void WindowSetTop([In] int top);
            [DispId(0x10a)]
            void WindowSetWidth([In] int width);
            [DispId(0x10b)]
            void WindowSetHeight([In] int height);
            [DispId(0x107)]
            void WindowClosing([In] bool isChildWindow, [In, Out] ref bool cancel);
            [DispId(0x10c)]
            void ClientToHostWindow([In, Out] ref long cx, [In, Out] ref long cy);
            [DispId(0x10d)]
            void SetSecureLockIcon([In] int secureLockIcon);
            [DispId(270)]
            void FileDownload([In, Out] ref bool ActiveDocument, [In, Out] ref bool cancel);
            [DispId(0x10f)]
            void NavigateError([In, MarshalAs(UnmanagedType.IDispatch)] object pDisp, [In] ref object URL, [In] ref object frame, [In] ref object statusCode, [In, Out] ref bool cancel);
            [DispId(0x110)]
            void PrivacyImpactedStateChange([In] bool bImpacted);
            [DispId(0x11a)]
            void SetPhishingFilterStatus(uint phishingFilterStatus);
            [DispId(0x11b)]
            void WindowStateChanged(uint dwFlags, uint dwValidFlagsMask);
        }

        [Flags]
        public enum DynamicStatusFlags
        {
            TS_SD_LOADING = 2,
            TS_SD_READONLY = 1
        }

        [SecurityCritical(SecurityCriticalScope.Everything), SuppressUnmanagedCodeSecurity]
        internal sealed class EtwTrace
        {
            private EtwTrace()
            {
            }

            [DllImport("advapi32", CharSet=CharSet.Unicode, ExactSpelling=true)]
            internal static extern int GetTraceEnableFlags(ulong traceHandle);
            [DllImport("advapi32", CharSet=CharSet.Unicode, ExactSpelling=true)]
            internal static extern byte GetTraceEnableLevel(ulong traceHandle);
            [DllImport("advapi32", EntryPoint="RegisterTraceGuidsW", CharSet=CharSet.Unicode, ExactSpelling=true)]
            internal static extern unsafe uint RegisterTraceGuids([In] EtwProc cbFunc, [In] void* context, [In] ref Guid controlGuid, [In] uint guidCount, ref TraceGuidRegistration guidReg, [In] string mofImagePath, [In] string mofResourceName, out ulong regHandle);
            [DllImport("advapi32", CharSet=CharSet.Unicode, ExactSpelling=true)]
            internal static extern unsafe uint TraceEvent(ulong traceHandle, char* header);
            [DllImport("advapi32", CharSet=CharSet.Unicode, ExactSpelling=true)]
            internal static extern int UnregisterTraceGuids(ulong regHandle);

            [StructLayout(LayoutKind.Sequential)]
            internal struct CSTRACE_GUID_REGISTRATION
            {
                internal unsafe System.Guid* Guid;
                internal uint RegHandle;
            }

            internal unsafe delegate uint EtwProc(uint requestCode, IntPtr requestContext, IntPtr bufferSize, byte* buffer);

            [StructLayout(LayoutKind.Sequential)]
            internal struct TraceGuidRegistration
            {
                internal unsafe System.Guid* Guid;
                internal unsafe void* RegHandle;
            }
        }

        public enum EXTENDED_NAME_FORMAT
        {
            NameCanonical = 7,
            NameCanonicalEx = 9,
            NameDisplay = 3,
            NameFullyQualifiedDN = 1,
            NameSamCompatible = 2,
            NameServicePrincipal = 10,
            NameUniqueId = 6,
            NameUnknown = 0,
            NameUserPrincipal = 8
        }

        [Flags]
        public enum FindRenderingMarkupFlags
        {
            TF_FRM_BACKWARD = 2,
            TF_FRM_INCLUDE_PROPERTY = 1,
            TF_FRM_NO_CONTAINED = 4,
            TF_FRM_NO_RANGE = 8
        }

        [Flags]
        public enum GetPositionFromPointFlags
        {
            GXFPF_NEAREST = 2,
            GXFPF_ROUND_NEAREST = 1
        }

        [Flags]
        public enum GetRenderingMarkupFlags
        {
            TF_GRM_INCLUDE_PROPERTY = 1
        }

        internal class HRESULT
        {
            public static void Check(int hr)
            {
                if (hr < 0)
                {
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class ICONINFO_IMPL
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask = IntPtr.Zero;
            public IntPtr hbmColor = IntPtr.Zero;
        }

        [ComImport, SecurityCritical(SecurityCriticalScope.Everything), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("B196B286-BAB4-101A-B69C-00AA00341D07")]
        public interface IConnectionPoint
        {
            [PreserveSig]
            int GetConnectionInterface(out Guid iid);
            [PreserveSig]
            int GetConnectionPointContainer([MarshalAs(UnmanagedType.Interface)] ref MS.Win32.UnsafeNativeMethods.IConnectionPointContainer pContainer);
            [PreserveSig]
            int Advise([In, MarshalAs(UnmanagedType.Interface)] object pUnkSink, ref int cookie);
            [PreserveSig]
            int Unadvise(int cookie);
            [PreserveSig]
            int EnumConnections(out object pEnum);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SecurityCritical(SecurityCriticalScope.Everything), SuppressUnmanagedCodeSecurity, Guid("B196B284-BAB4-101A-B69C-00AA00341D07")]
        public interface IConnectionPointContainer
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object EnumConnectionPoints();
            [PreserveSig]
            int FindConnectionPoint([In] ref Guid guid, [MarshalAs(UnmanagedType.Interface)] out MS.Win32.UnsafeNativeMethods.IConnectionPoint ppCP);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00020400-0000-0000-C000-000000000046")]
        public interface IDispatch
        {
            int GetTypeInfoCount();
            [return: MarshalAs(UnmanagedType.Interface)]
            ITypeInfo GetTypeInfo([In, MarshalAs(UnmanagedType.U4)] int iTInfo, [In, MarshalAs(UnmanagedType.U4)] int lcid);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetIDsOfNames([In] ref Guid riid, [In, MarshalAs(UnmanagedType.LPArray)] string[] rgszNames, [In, MarshalAs(UnmanagedType.U4)] int cNames, [In, MarshalAs(UnmanagedType.U4)] int lcid, [Out, MarshalAs(UnmanagedType.LPArray)] int[] rgDispId);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void Invoke(int dispIdMember, [In] ref Guid riid, [In, MarshalAs(UnmanagedType.U4)] int lcid, [In, MarshalAs(UnmanagedType.U4)] int dwFlags, [In, Out] NativeMethods.tagDISPPARAMS pDispParams, [Out, MarshalAs(UnmanagedType.LPArray)] object[] pVarResult, [In, Out] NativeMethods.tagEXCEPINFO pExcepInfo, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] pArgErr);
        }

        [ComImport, Guid("BD3F23C0-D43E-11CF-893B-00AA00BDCE1A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDocHostUIHandler
        {
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ShowContextMenu([In, MarshalAs(UnmanagedType.U4)] int dwID, [In] NativeMethods.POINT pt, [In, MarshalAs(UnmanagedType.Interface)] object pcmdtReserved, [In, MarshalAs(UnmanagedType.Interface)] object pdispReserved);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetHostInfo([In, Out] NativeMethods.DOCHOSTUIINFO info);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ShowUI([In, MarshalAs(UnmanagedType.I4)] int dwID, [In] MS.Win32.UnsafeNativeMethods.IOleInPlaceActiveObject activeObject, [In] NativeMethods.IOleCommandTarget commandTarget, [In] MS.Win32.UnsafeNativeMethods.IOleInPlaceFrame frame, [In] MS.Win32.UnsafeNativeMethods.IOleInPlaceUIWindow doc);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int HideUI();
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int UpdateUI();
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int EnableModeless([In, MarshalAs(UnmanagedType.Bool)] bool fEnable);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnDocWindowActivate([In, MarshalAs(UnmanagedType.Bool)] bool fActivate);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int OnFrameWindowActivate([In, MarshalAs(UnmanagedType.Bool)] bool fActivate);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int ResizeBorder([In] NativeMethods.COMRECT rect, [In] MS.Win32.UnsafeNativeMethods.IOleInPlaceUIWindow doc, bool fFrameWindow);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateAccelerator([In] ref MSG msg, [In] ref Guid group, [In, MarshalAs(UnmanagedType.I4)] int nCmdID);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetOptionKeyPath([Out, MarshalAs(UnmanagedType.LPArray)] string[] pbstrKey, [In, MarshalAs(UnmanagedType.U4)] int dw);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetDropTarget([In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IOleDropTarget pDropTarget, [MarshalAs(UnmanagedType.Interface)] out MS.Win32.UnsafeNativeMethods.IOleDropTarget ppDropTarget);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int GetExternal([MarshalAs(UnmanagedType.IDispatch)] out object ppDispatch);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int TranslateUrl([In, MarshalAs(UnmanagedType.U4)] int dwTranslate, [In, MarshalAs(UnmanagedType.LPWStr)] string strURLIn, [MarshalAs(UnmanagedType.LPWStr)] out string pstrURLOut);
            [return: MarshalAs(UnmanagedType.I4)]
            [PreserveSig]
            int FilterDataObject(IDataObject pDO, out IDataObject ppDORet);
        }

        [ComImport, Guid("B196B285-BAB4-101A-B69C-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumConnectionPoints
        {
            [PreserveSig]
            int Next(int cConnections, out MS.Win32.UnsafeNativeMethods.IConnectionPoint pCp, out int pcFetched);
            [PreserveSig]
            int Skip(int cSkip);
            void Reset();
            MS.Win32.UnsafeNativeMethods.IEnumConnectionPoints Clone();
        }

        [ComImport, Guid("5EFD22BA-7838-46CB-88E2-CADB14124F8F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IEnumITfCompositionView
        {
            void Clone(out MS.Win32.UnsafeNativeMethods.IEnumTfRanges ranges);
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int Next(int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] MS.Win32.UnsafeNativeMethods.ITfCompositionView[] compositionview, out int fetched);
            void Reset();
            [PreserveSig]
            int Skip(int count);
        }

        [ComImport, Guid("00000104-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumOLEVERB
        {
            [PreserveSig]
            int Next([MarshalAs(UnmanagedType.U4)] int celt, [Out] NativeMethods.tagOLEVERB rgelt, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);
            [PreserveSig]
            int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);
            void Reset();
            void Clone(out MS.Win32.UnsafeNativeMethods.IEnumOLEVERB ppenum);
        }

        [ComImport, Guid("aa80e808-2021-11d2-93e0-0060b067b86e"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumTfDocumentMgrs
        {
        }

        [ComImport, Guid("e4b24db0-0990-11d3-8df0-00105a2799b5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumTfFunctionProviders
        {
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3d61bf11-ac5f-42c8-a4cb-931bcc28c744")]
        internal interface IEnumTfLanguageProfiles
        {
            void Clone(out MS.Win32.UnsafeNativeMethods.IEnumTfLanguageProfiles enumIPP);
            [PreserveSig]
            int Next(int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] MS.Win32.UnsafeNativeMethods.TF_LANGUAGEPROFILE[] profiles, out int fetched);
            void Reset();
            void Skip(int count);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, SecurityCritical, Guid("f99d3f40-8e32-11d2-bf46-00105a2799b5")]
        public interface IEnumTfRanges
        {
            [SecurityCritical]
            void Clone(out MS.Win32.UnsafeNativeMethods.IEnumTfRanges ranges);
            [PreserveSig, SecurityCritical]
            int Next(int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] MS.Win32.UnsafeNativeMethods.ITfRange[] ranges, out int fetched);
            [SecurityCritical]
            void Reset();
            [PreserveSig, SecurityCritical]
            int Skip(int count);
        }

        [ComImport, Guid("8c03d21b-95a7-4ba0-ae1b-7fce12a72930"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumTfRenderingMarkup
        {
            void Clone(out MS.Win32.UnsafeNativeMethods.IEnumTfRenderingMarkup clone);
            [PreserveSig]
            int Next(int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] MS.Win32.UnsafeNativeMethods.TF_RENDERINGMARKUP[] markup, out int fetched);
            void Reset();
            [PreserveSig]
            int Skip(int count);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000100-0000-0000-C000-000000000046")]
        public interface IEnumUnknown
        {
            [PreserveSig]
            int Next([In, MarshalAs(UnmanagedType.U4)] int celt, [Out] IntPtr rgelt, IntPtr pceltFetched);
            [PreserveSig]
            int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);
            void Reset();
            void Clone(out MS.Win32.UnsafeNativeMethods.IEnumUnknown ppenum);
        }

        [ComImport, Guid("00020404-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumVariant
        {
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int Next([In, MarshalAs(UnmanagedType.U4)] int celt, [In, Out] IntPtr rgvar, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pceltFetched);
            void Skip([In, MarshalAs(UnmanagedType.U4)] int celt);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void Reset();
            void Clone([Out, MarshalAs(UnmanagedType.LPArray)] MS.Win32.UnsafeNativeMethods.IEnumVariant[] ppenum);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsDual), Guid("626FC520-A41E-11CF-A731-00A0C9082637")]
        internal interface IHTMLDocument
        {
            [return: MarshalAs(UnmanagedType.IDispatch)]
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            object GetScript();
        }

        [ComImport, SecurityCritical(SecurityCriticalScope.Everything), InterfaceType(ComInterfaceType.InterfaceIsDual), SuppressUnmanagedCodeSecurity, Guid("332C4425-26CB-11D0-B483-00C04FD90119")]
        internal interface IHTMLDocument2 : MS.Win32.UnsafeNativeMethods.IHTMLDocument
        {
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetScript();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetAll();
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetBody();
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetActiveElement();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetImages();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetApplets();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetLinks();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetForms();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetAnchors();
            void SetTitle(string p);
            string GetTitle();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetScripts();
            void SetDesignMode(string p);
            string GetDesignMode();
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetSelection();
            string GetReadyState();
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetFrames();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetEmbeds();
            MS.Win32.UnsafeNativeMethods.IHTMLElementCollection GetPlugins();
            void SetAlinkColor(object c);
            object GetAlinkColor();
            void SetBgColor(object c);
            object GetBgColor();
            void SetFgColor(object c);
            object GetFgColor();
            void SetLinkColor(object c);
            object GetLinkColor();
            void SetVlinkColor(object c);
            object GetVlinkColor();
            string GetReferrer();
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetLocation();
            string GetLastModified();
            void SetUrl(string p);
            string GetUrl();
            void SetDomain(string p);
            string GetDomain();
            void SetCookie(string p);
            string GetCookie();
            void SetExpando(bool p);
            bool GetExpando();
            void SetCharset(string p);
            string GetCharset();
            void SetDefaultCharset(string p);
            string GetDefaultCharset();
            string GetMimeType();
            string GetFileSize();
            string GetFileCreatedDate();
            string GetFileModifiedDate();
            string GetFileUpdatedDate();
            string GetSecurity();
            string GetProtocol();
            string GetNameProp();
            int Write([In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_EMPTY)] object[] psarray);
            int WriteLine([In, MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_EMPTY)] object[] psarray);
            [return: MarshalAs(UnmanagedType.Interface)]
            object Open(string mimeExtension, object name, object features, object replace);
            void Close();
            void Clear();
            bool QueryCommandSupported(string cmdID);
            bool QueryCommandEnabled(string cmdID);
            bool QueryCommandState(string cmdID);
            bool QueryCommandIndeterm(string cmdID);
            string QueryCommandText(string cmdID);
            object QueryCommandValue(string cmdID);
            bool ExecCommand(string cmdID, bool showUI, object value);
            bool ExecCommandShowHelp(string cmdID);
            [return: MarshalAs(UnmanagedType.Interface)]
            object CreateElement(string eTag);
            void SetOnhelp(object p);
            object GetOnhelp();
            void SetOnclick(object p);
            object GetOnclick();
            void SetOndblclick(object p);
            object GetOndblclick();
            void SetOnkeyup(object p);
            object GetOnkeyup();
            void SetOnkeydown(object p);
            object GetOnkeydown();
            void SetOnkeypress(object p);
            object GetOnkeypress();
            void SetOnmouseup(object p);
            object GetOnmouseup();
            void SetOnmousedown(object p);
            object GetOnmousedown();
            void SetOnmousemove(object p);
            object GetOnmousemove();
            void SetOnmouseout(object p);
            object GetOnmouseout();
            void SetOnmouseover(object p);
            object GetOnmouseover();
            void SetOnreadystatechange(object p);
            object GetOnreadystatechange();
            void SetOnafterupdate(object p);
            object GetOnafterupdate();
            void SetOnrowexit(object p);
            object GetOnrowexit();
            void SetOnrowenter(object p);
            object GetOnrowenter();
            void SetOndragstart(object p);
            object GetOndragstart();
            void SetOnselectstart(object p);
            object GetOnselectstart();
            [return: MarshalAs(UnmanagedType.Interface)]
            object ElementFromPoint(int x, int y);
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetParentWindow();
            [return: MarshalAs(UnmanagedType.Interface)]
            object GetStyleSheets();
            void SetOnbeforeupdate(object p);
            object GetOnbeforeupdate();
            void SetOnerrorupdate(object p);
            object GetOnerrorupdate();
            string toString();
            [return: MarshalAs(UnmanagedType.Interface)]
            object CreateStyleSheet(string bstrHref, int lIndex);
        }

        [ComImport, Guid("3050F21F-98B5-11CF-BB82-00AA00BDCE0B"), InterfaceType(ComInterfaceType.InterfaceIsDual), SuppressUnmanagedCodeSecurity, SecurityCritical(SecurityCriticalScope.Everything)]
        internal interface IHTMLElementCollection
        {
            string toString();
            void SetLength(int p);
            int GetLength();
            [return: MarshalAs(UnmanagedType.Interface)]
            object Get_newEnum();
            [return: MarshalAs(UnmanagedType.IDispatch)]
            object Item(object idOrName, object index);
            [return: MarshalAs(UnmanagedType.Interface)]
            object Tags(object tagName);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("79eac9ee-baf9-11ce-8c82-00aa004ba90b"), ComVisible(false)]
        internal interface IInternetSecurityManager
        {
            void SetSecuritySite(NativeMethods.IInternetSecurityMgrSite pSite);
            unsafe void GetSecuritySite(void** ppSite);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void MapUrlToZone([In, MarshalAs(UnmanagedType.BStr)] string pwszUrl, out int pdwZone, [In] int dwFlags);
            unsafe void GetSecurityId(string pwszUrl, byte* pbSecurityId, int* pcbSecurityId, int dwReserved);
            unsafe void ProcessUrlAction(string pwszUrl, int dwAction, byte* pPolicy, int cbPolicy, byte* pContext, int cbContext, int dwFlags, int dwReserved);
            unsafe void QueryCustomPolicy(string pwszUrl, void* guidKey, byte** ppPolicy, int* pcbPolicy, byte* pContext, int cbContext, int dwReserved);
            void SetZoneMapping(int dwZone, string lpszPattern, int dwFlags);
            unsafe void GetZoneMappings(int dwZone, void** ppenumString, int dwFlags);
        }

        [Flags]
        public enum InsertAtSelectionFlags
        {
            TS_IAS_NOQUERY = 1,
            TS_IAS_QUERYONLY = 2
        }

        [Flags]
        public enum InsertEmbeddedFlags
        {
            TS_IE_CORRECTION = 1
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000118-0000-0000-C000-000000000046")]
        public interface IOleClientSite
        {
            [PreserveSig]
            int SaveObject();
            [PreserveSig]
            int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object moniker);
            [PreserveSig]
            int GetContainer(out MS.Win32.UnsafeNativeMethods.IOleContainer container);
            [PreserveSig]
            int ShowObject();
            [PreserveSig]
            int OnShowWindow(int fShow);
            [PreserveSig]
            int RequestNewObjectLayout();
        }

        [ComImport, Guid("0000011B-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleContainer
        {
            [PreserveSig]
            int ParseDisplayName([In, MarshalAs(UnmanagedType.Interface)] object pbc, [In, MarshalAs(UnmanagedType.BStr)] string pszDisplayName, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pchEaten, [Out, MarshalAs(UnmanagedType.LPArray)] object[] ppmkOut);
            [PreserveSig]
            int EnumObjects([In, MarshalAs(UnmanagedType.U4)] int grfFlags, out MS.Win32.UnsafeNativeMethods.IEnumUnknown ppenum);
            [PreserveSig]
            int LockContainer(bool fLock);
        }

        [ComImport, SuppressUnmanagedCodeSecurity, SecurityCritical(SecurityCriticalScope.Everything), Guid("B196B288-BAB4-101A-B69C-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleControl
        {
            [PreserveSig]
            int GetControlInfo([Out] NativeMethods.tagCONTROLINFO pCI);
            [PreserveSig]
            int OnMnemonic([In] ref MSG pMsg);
            [PreserveSig]
            int OnAmbientPropertyChange(int dispID);
            [PreserveSig]
            int FreezeEvents(int bFreeze);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("B196B289-BAB4-101A-B69C-00AA00341D07")]
        public interface IOleControlSite
        {
            [PreserveSig]
            int OnControlInfoChanged();
            [PreserveSig]
            int LockInPlaceActive(int fLock);
            [PreserveSig]
            int GetExtendedControl([MarshalAs(UnmanagedType.IDispatch)] out object ppDisp);
            [PreserveSig]
            int TransformCoords([In, Out] NativeMethods.POINT pPtlHimetric, [In, Out] NativeMethods.POINTF pPtfContainer, [In, MarshalAs(UnmanagedType.U4)] int dwFlags);
            [PreserveSig]
            int TranslateAccelerator([In] ref MSG pMsg, [In, MarshalAs(UnmanagedType.U4)] int grfModifiers);
            [PreserveSig]
            int OnFocus(int fGotFocus);
            [PreserveSig]
            int ShowPropertyFrame();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000121-0000-0000-C000-000000000046")]
        public interface IOleDropSource
        {
            [PreserveSig]
            int OleQueryContinueDrag(int fEscapePressed, [In, MarshalAs(UnmanagedType.U4)] int grfKeyState);
            [PreserveSig]
            int OleGiveFeedback([In, MarshalAs(UnmanagedType.U4)] int dwEffect);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000122-0000-0000-C000-000000000046")]
        public interface IOleDropTarget
        {
            [PreserveSig]
            int OleDragEnter([In, MarshalAs(UnmanagedType.Interface)] object pDataObj, [In, MarshalAs(UnmanagedType.U4)] int grfKeyState, [In, MarshalAs(UnmanagedType.U8)] long pt, [In, Out] ref int pdwEffect);
            [PreserveSig]
            int OleDragOver([In, MarshalAs(UnmanagedType.U4)] int grfKeyState, [In, MarshalAs(UnmanagedType.U8)] long pt, [In, Out] ref int pdwEffect);
            [PreserveSig]
            int OleDragLeave();
            [PreserveSig]
            int OleDrop([In, MarshalAs(UnmanagedType.Interface)] object pDataObj, [In, MarshalAs(UnmanagedType.U4)] int grfKeyState, [In, MarshalAs(UnmanagedType.U8)] long pt, [In, Out] ref int pdwEffect);
        }

        [ComImport, Guid("00000117-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceActiveObject
        {
            [PreserveSig, SecurityCritical, SuppressUnmanagedCodeSecurity]
            int GetWindow(out IntPtr hwnd);
            void ContextSensitiveHelp(int fEnterMode);
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int TranslateAccelerator([In] ref MSG lpmsg);
            void OnFrameWindowActivate(int fActivate);
            void OnDocWindowActivate(int fActivate);
            void ResizeBorder([In] NativeMethods.RECT prcBorder, [In] MS.Win32.UnsafeNativeMethods.IOleInPlaceUIWindow pUIWindow, bool fFrameWindow);
            void EnableModeless(int fEnable);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000116-0000-0000-C000-000000000046")]
        public interface IOleInPlaceFrame
        {
            IntPtr GetWindow();
            [PreserveSig]
            int ContextSensitiveHelp(int fEnterMode);
            [PreserveSig]
            int GetBorder([Out] NativeMethods.COMRECT lprectBorder);
            [PreserveSig]
            int RequestBorderSpace([In] NativeMethods.COMRECT pborderwidths);
            [PreserveSig]
            int SetBorderSpace([In] NativeMethods.COMRECT pborderwidths);
            [PreserveSig]
            int SetActiveObject([In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IOleInPlaceActiveObject pActiveObject, [In, MarshalAs(UnmanagedType.LPWStr)] string pszObjName);
            [PreserveSig]
            int InsertMenus([In] IntPtr hmenuShared, [In, Out] NativeMethods.tagOleMenuGroupWidths lpMenuWidths);
            [PreserveSig]
            int SetMenu([In] IntPtr hmenuShared, [In] IntPtr holemenu, [In] IntPtr hwndActiveObject);
            [PreserveSig]
            int RemoveMenus([In] IntPtr hmenuShared);
            [PreserveSig]
            int SetStatusText([In, MarshalAs(UnmanagedType.LPWStr)] string pszStatusText);
            [PreserveSig]
            int EnableModeless(bool fEnable);
            [PreserveSig]
            int TranslateAccelerator([In] ref MSG lpmsg, [In, MarshalAs(UnmanagedType.U2)] short wID);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SecurityCritical(SecurityCriticalScope.Everything), SuppressUnmanagedCodeSecurity, Guid("00000113-0000-0000-C000-000000000046")]
        public interface IOleInPlaceObject
        {
            [PreserveSig]
            int GetWindow(out IntPtr hwnd);
            void ContextSensitiveHelp(int fEnterMode);
            void InPlaceDeactivate();
            [PreserveSig]
            int UIDeactivate();
            void SetObjectRects([In] NativeMethods.COMRECT lprcPosRect, [In] NativeMethods.COMRECT lprcClipRect);
            void ReactivateAndUndo();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("1C2056CC-5EF4-101B-8BC8-00AA003E3B29")]
        public interface IOleInPlaceObjectWindowless
        {
            [PreserveSig]
            int SetClientSite([In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IOleClientSite pClientSite);
            [PreserveSig]
            int GetClientSite(out MS.Win32.UnsafeNativeMethods.IOleClientSite site);
            [PreserveSig]
            int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] string szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);
            [PreserveSig]
            int Close(int dwSaveOption);
            [PreserveSig]
            int SetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [In, MarshalAs(UnmanagedType.Interface)] object pmk);
            [PreserveSig]
            int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object moniker);
            [PreserveSig]
            int InitFromData([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObject, int fCreation, [In, MarshalAs(UnmanagedType.U4)] int dwReserved);
            [PreserveSig]
            int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] int dwReserved, out IDataObject data);
            [PreserveSig]
            int DoVerb(int iVerb, [In] IntPtr lpmsg, [In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, [In] NativeMethods.RECT lprcPosRect);
            [PreserveSig]
            int EnumVerbs(out MS.Win32.UnsafeNativeMethods.IEnumOLEVERB e);
            [PreserveSig]
            int OleUpdate();
            [PreserveSig]
            int IsUpToDate();
            [PreserveSig]
            int GetUserClassID([In, Out] ref Guid pClsid);
            [PreserveSig]
            int GetUserType([In, MarshalAs(UnmanagedType.U4)] int dwFormOfType, [MarshalAs(UnmanagedType.LPWStr)] out string userType);
            [PreserveSig]
            int SetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [In] NativeMethods.SIZE pSizel);
            [PreserveSig]
            int GetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [Out] NativeMethods.SIZE pSizel);
            [PreserveSig]
            int Advise([In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink, out int cookie);
            [PreserveSig]
            int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);
            [PreserveSig]
            int EnumAdvise(out IEnumSTATDATA e);
            [PreserveSig]
            int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);
            [PreserveSig]
            int SetColorScheme([In] NativeMethods.tagLOGPALETTE pLogpal);
            [PreserveSig]
            int OnWindowMessage([In, MarshalAs(UnmanagedType.U4)] int msg, [In, MarshalAs(UnmanagedType.U4)] int wParam, [In, MarshalAs(UnmanagedType.U4)] int lParam, [Out, MarshalAs(UnmanagedType.U4)] int plResult);
            [PreserveSig]
            int GetDropTarget([Out, MarshalAs(UnmanagedType.Interface)] object ppDropTarget);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000119-0000-0000-C000-000000000046")]
        public interface IOleInPlaceSite
        {
            IntPtr GetWindow();
            [PreserveSig]
            int ContextSensitiveHelp(int fEnterMode);
            [PreserveSig]
            int CanInPlaceActivate();
            [PreserveSig]
            int OnInPlaceActivate();
            [PreserveSig]
            int OnUIActivate();
            [PreserveSig]
            int GetWindowContext([MarshalAs(UnmanagedType.Interface)] out MS.Win32.UnsafeNativeMethods.IOleInPlaceFrame ppFrame, [MarshalAs(UnmanagedType.Interface)] out MS.Win32.UnsafeNativeMethods.IOleInPlaceUIWindow ppDoc, [Out] NativeMethods.COMRECT lprcPosRect, [Out] NativeMethods.COMRECT lprcClipRect, [In, Out] NativeMethods.OLEINPLACEFRAMEINFO lpFrameInfo);
            [PreserveSig]
            int Scroll(NativeMethods.SIZE scrollExtant);
            [PreserveSig]
            int OnUIDeactivate(int fUndoable);
            [PreserveSig]
            int OnInPlaceDeactivate();
            [PreserveSig]
            int DiscardUndoState();
            [PreserveSig]
            int DeactivateAndUndo();
            [PreserveSig]
            int OnPosRectChange([In] NativeMethods.COMRECT lprcPosRect);
        }

        [ComImport, Guid("00000115-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleInPlaceUIWindow
        {
            IntPtr GetWindow();
            [PreserveSig]
            int ContextSensitiveHelp(int fEnterMode);
            [PreserveSig]
            int GetBorder([Out] NativeMethods.RECT lprectBorder);
            [PreserveSig]
            int RequestBorderSpace([In] NativeMethods.RECT pborderwidths);
            [PreserveSig]
            int SetBorderSpace([In] NativeMethods.RECT pborderwidths);
            void SetActiveObject([In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IOleInPlaceActiveObject pActiveObject, [In, MarshalAs(UnmanagedType.LPWStr)] string pszObjName);
        }

        [ComImport, Guid("00000112-0000-0000-C000-000000000046"), SuppressUnmanagedCodeSecurity, SecurityCritical(SecurityCriticalScope.Everything), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IOleObject
        {
            [PreserveSig]
            int SetClientSite([In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IOleClientSite pClientSite);
            MS.Win32.UnsafeNativeMethods.IOleClientSite GetClientSite();
            [PreserveSig]
            int SetHostNames([In, MarshalAs(UnmanagedType.LPWStr)] string szContainerApp, [In, MarshalAs(UnmanagedType.LPWStr)] string szContainerObj);
            [PreserveSig]
            int Close(int dwSaveOption);
            [PreserveSig]
            int SetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [In, MarshalAs(UnmanagedType.Interface)] object pmk);
            [PreserveSig]
            int GetMoniker([In, MarshalAs(UnmanagedType.U4)] int dwAssign, [In, MarshalAs(UnmanagedType.U4)] int dwWhichMoniker, [MarshalAs(UnmanagedType.Interface)] out object moniker);
            [PreserveSig]
            int InitFromData([In, MarshalAs(UnmanagedType.Interface)] IDataObject pDataObject, int fCreation, [In, MarshalAs(UnmanagedType.U4)] int dwReserved);
            [PreserveSig]
            int GetClipboardData([In, MarshalAs(UnmanagedType.U4)] int dwReserved, out IDataObject data);
            [PreserveSig]
            int DoVerb(int iVerb, [In] IntPtr lpmsg, [In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, [In] NativeMethods.COMRECT lprcPosRect);
            [PreserveSig]
            int EnumVerbs(out MS.Win32.UnsafeNativeMethods.IEnumOLEVERB e);
            [PreserveSig]
            int OleUpdate();
            [PreserveSig]
            int IsUpToDate();
            [PreserveSig]
            int GetUserClassID([In, Out] ref Guid pClsid);
            [PreserveSig]
            int GetUserType([In, MarshalAs(UnmanagedType.U4)] int dwFormOfType, [MarshalAs(UnmanagedType.LPWStr)] out string userType);
            [PreserveSig]
            int SetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [In] NativeMethods.SIZE pSizel);
            [PreserveSig]
            int GetExtent([In, MarshalAs(UnmanagedType.U4)] int dwDrawAspect, [Out] NativeMethods.SIZE pSizel);
            [PreserveSig]
            int Advise(IAdviseSink pAdvSink, out int cookie);
            [PreserveSig]
            int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);
            [PreserveSig]
            int EnumAdvise(out IEnumSTATDATA e);
            [PreserveSig]
            int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);
            [PreserveSig]
            int SetColorScheme([In] NativeMethods.tagLOGPALETTE pLogpal);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("00000114-0000-0000-C000-000000000046")]
        public interface IOleWindow
        {
            [PreserveSig]
            int GetWindow(out IntPtr hwnd);
            void ContextSensitiveHelp(int fEnterMode);
        }

        [ComImport, Guid("7FD52380-4E07-101B-AE2D-08002B2EC713"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IPersistStreamInit
        {
            void GetClassID(out Guid pClassID);
            [PreserveSig]
            int IsDirty();
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void Load([In, MarshalAs(UnmanagedType.Interface)] System.Runtime.InteropServices.ComTypes.IStream pstm);
            void Save([In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IStream pstm, [In, MarshalAs(UnmanagedType.Bool)] bool fClearDirty);
            void GetSizeMax([Out, MarshalAs(UnmanagedType.LPArray)] long pcbSize);
            void InitNew();
        }

        [ComImport, Guid("9BFBBC02-EFF1-101A-84ED-00AA00341D07"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyNotifySink
        {
            void OnChanged(int dispID);
            [PreserveSig]
            int OnRequestEdit(int dispID);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000C-0000-0000-C000-000000000046")]
        public interface IStream
        {
            int Read(IntPtr buf, int len);
            int Write(IntPtr buf, int len);
            [return: MarshalAs(UnmanagedType.I8)]
            long Seek([In, MarshalAs(UnmanagedType.I8)] long dlibMove, int dwOrigin);
            void SetSize([In, MarshalAs(UnmanagedType.I8)] long libNewSize);
            [return: MarshalAs(UnmanagedType.I8)]
            long CopyTo([In, MarshalAs(UnmanagedType.Interface)] MS.Win32.UnsafeNativeMethods.IStream pstm, [In, MarshalAs(UnmanagedType.I8)] long cb, [Out, MarshalAs(UnmanagedType.LPArray)] long[] pcbRead);
            void Commit(int grfCommitFlags);
            void Revert();
            void LockRegion([In, MarshalAs(UnmanagedType.I8)] long libOffset, [In, MarshalAs(UnmanagedType.I8)] long cb, int dwLockType);
            void UnlockRegion([In, MarshalAs(UnmanagedType.I8)] long libOffset, [In, MarshalAs(UnmanagedType.I8)] long cb, int dwLockType);
            void Stat([Out] NativeMethods.STATSTG pStatstg, int grfStatFlag);
            [return: MarshalAs(UnmanagedType.Interface)]
            MS.Win32.UnsafeNativeMethods.IStream Clone();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("28888fe3-c2a0-483a-a3ea-8cb1ce51ff3d")]
        public interface ITextStoreACP
        {
            void AdviseSink(ref Guid riid, [MarshalAs(UnmanagedType.Interface)] object obj, MS.Win32.UnsafeNativeMethods.AdviseFlags flags);
            void UnadviseSink([MarshalAs(UnmanagedType.Interface)] object obj);
            void RequestLock(MS.Win32.UnsafeNativeMethods.LockFlags flags, out int hrSession);
            void GetStatus(out MS.Win32.UnsafeNativeMethods.TS_STATUS status);
            void QueryInsert(int start, int end, int cch, out int startResult, out int endResult);
            void GetSelection(int index, int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] MS.Win32.UnsafeNativeMethods.TS_SELECTION_ACP[] selection, out int fetched);
            void SetSelection(int count, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] MS.Win32.UnsafeNativeMethods.TS_SELECTION_ACP[] selection);
            void GetText(int start, int end, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] char[] text, int cchReq, out int charsCopied, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=6)] MS.Win32.UnsafeNativeMethods.TS_RUNINFO[] runInfo, int cRunInfoReq, out int cRunInfoRcv, out int nextCp);
            void SetText(MS.Win32.UnsafeNativeMethods.SetTextFlags flags, int start, int end, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] char[] text, int cch, out MS.Win32.UnsafeNativeMethods.TS_TEXTCHANGE change);
            void GetFormattedText(int start, int end, [MarshalAs(UnmanagedType.Interface)] out object obj);
            void GetEmbedded(int position, ref Guid guidService, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object obj);
            void QueryInsertEmbedded(ref Guid guidService, int formatEtc, [MarshalAs(UnmanagedType.Bool)] out bool insertable);
            void InsertEmbedded(MS.Win32.UnsafeNativeMethods.InsertEmbeddedFlags flags, int start, int end, [MarshalAs(UnmanagedType.Interface)] object obj, out MS.Win32.UnsafeNativeMethods.TS_TEXTCHANGE change);
            void InsertTextAtSelection(MS.Win32.UnsafeNativeMethods.InsertAtSelectionFlags flags, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] char[] text, int cch, out int start, out int end, out MS.Win32.UnsafeNativeMethods.TS_TEXTCHANGE change);
            void InsertEmbeddedAtSelection(MS.Win32.UnsafeNativeMethods.InsertAtSelectionFlags flags, [MarshalAs(UnmanagedType.Interface)] object obj, out int start, out int end, out MS.Win32.UnsafeNativeMethods.TS_TEXTCHANGE change);
            [PreserveSig]
            int RequestSupportedAttrs(MS.Win32.UnsafeNativeMethods.AttributeFlags flags, int count, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] Guid[] filterAttributes);
            [PreserveSig]
            int RequestAttrsAtPosition(int position, int count, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] Guid[] filterAttributes, MS.Win32.UnsafeNativeMethods.AttributeFlags flags);
            void RequestAttrsTransitioningAtPosition(int position, int count, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] Guid[] filterAttributes, MS.Win32.UnsafeNativeMethods.AttributeFlags flags);
            void FindNextAttrTransition(int start, int halt, int count, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] Guid[] filterAttributes, MS.Win32.UnsafeNativeMethods.AttributeFlags flags, out int acpNext, [MarshalAs(UnmanagedType.Bool)] out bool found, out int foundOffset);
            void RetrieveRequestedAttrs(int count, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] MS.Win32.UnsafeNativeMethods.TS_ATTRVAL[] attributeVals, out int countFetched);
            void GetEnd(out int end);
            void GetActiveView(out int viewCookie);
            void GetACPFromPoint(int viewCookie, ref MS.Win32.UnsafeNativeMethods.POINT point, MS.Win32.UnsafeNativeMethods.GetPositionFromPointFlags flags, out int position);
            void GetTextExt(int viewCookie, int start, int end, out MS.Win32.UnsafeNativeMethods.RECT rect, [MarshalAs(UnmanagedType.Bool)] out bool clipped);
            void GetScreenExt(int viewCookie, out MS.Win32.UnsafeNativeMethods.RECT rect);
            void GetWnd(int viewCookie, out IntPtr hwnd);
        }

        [ComImport, Guid("22d44c94-a419-4542-a272-ae26093ececf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITextStoreACPSink
        {
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void OnTextChange(MS.Win32.UnsafeNativeMethods.OnTextChangeFlags flags, ref MS.Win32.UnsafeNativeMethods.TS_TEXTCHANGE change);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void OnSelectionChange();
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void OnLayoutChange(MS.Win32.UnsafeNativeMethods.TsLayoutCode lcode, int viewCookie);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void OnStatusChange(MS.Win32.UnsafeNativeMethods.DynamicStatusFlags flags);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void OnAttrsChange(int start, int end, int count, Guid[] attributes);
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int OnLockGranted(MS.Win32.UnsafeNativeMethods.LockFlags flags);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void OnStartEditTransaction();
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void OnEndEditTransaction();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a3ad50fb-9bdb-49e3-a843-6c76520fbf5d")]
        public interface ITfCandidateList
        {
            void EnumCandidates(out object enumCand);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetCandidate(int nIndex, out MS.Win32.UnsafeNativeMethods.ITfCandidateString candstring);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetCandidateNum(out int nCount);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void SetResult(int nIndex, MS.Win32.UnsafeNativeMethods.TfCandidateResult result);
        }

        [ComImport, Guid("581f317e-fd9d-443f-b972-ed00467c5d40"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfCandidateString
        {
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void GetString([MarshalAs(UnmanagedType.BStr)] out string funcName);
            void GetIndex(out int nIndex);
        }

        [ComImport, SuppressUnmanagedCodeSecurity, SecurityCritical, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("c3acefb5-f69d-4905-938f-fcadcf4be830")]
        public interface ITfCategoryMgr
        {
            [SecurityCritical]
            void stub_RegisterCategory();
            [SecurityCritical]
            void stub_UnregisterCategory();
            [SecurityCritical]
            void stub_EnumCategoriesInItem();
            [SecurityCritical]
            void stub_EnumItemsInCategory();
            [SecurityCritical]
            void stub_FindClosestCategory();
            [SecurityCritical]
            void stub_RegisterGUIDDescription();
            [SecurityCritical]
            void stub_UnregisterGUIDDescription();
            [SecurityCritical]
            void stub_GetGUIDDescription();
            [SecurityCritical]
            void stub_RegisterGUIDDWORD();
            [SecurityCritical]
            void stub_UnregisterGUIDDWORD();
            [SecurityCritical]
            void stub_GetGUIDDWORD();
            [SecurityCritical]
            void stub_RegisterGUID();
            [PreserveSig, SecurityCritical]
            int GetGUID(int guidatom, out Guid guid);
            [SecurityCritical]
            void stub_IsEqualTfGuidAtom();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("bb08f7a9-607a-4384-8623-056892b64371"), SecurityCritical, SuppressUnmanagedCodeSecurity]
        public interface ITfCompartment
        {
            [PreserveSig, SecurityCritical]
            int SetValue(int tid, ref object varValue);
            [SecurityCritical]
            void GetValue(out object varValue);
        }

        [ComImport, Guid("743abd5f-f26d-48df-8cc5-238492419b64"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfCompartmentEventSink
        {
            void OnChange(ref Guid rguid);
        }

        [ComImport, Guid("7dcf57ac-18ad-438b-824d-979bffb74b7c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfCompartmentMgr
        {
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetCompartment(ref Guid guid, out MS.Win32.UnsafeNativeMethods.ITfCompartment comp);
            void ClearCompartment(int tid, Guid guid);
            void EnumCompartments(out object enumGuid);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("D7540241-F9A1-4364-BEFC-DBCD2C4395B7"), SecurityCritical]
        public interface ITfCompositionView
        {
            [SecurityCritical]
            void GetOwnerClsid(out Guid clsid);
            [SecurityCritical]
            void GetRange(out MS.Win32.UnsafeNativeMethods.ITfRange range);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("aa80e7fd-2021-11d2-93e0-0060b067b86e")]
        public interface ITfContext
        {
            int stub_RequestEditSession();
            void InWriteSession(int clientId, [MarshalAs(UnmanagedType.Bool)] out bool inWriteSession);
            void stub_GetSelection();
            void stub_SetSelection();
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetStart(int ec, out MS.Win32.UnsafeNativeMethods.ITfRange range);
            void stub_GetEnd();
            void stub_GetActiveView();
            void stub_EnumViews();
            void stub_GetStatus();
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetProperty(ref Guid guid, out MS.Win32.UnsafeNativeMethods.ITfProperty property);
            void stub_GetAppProperty();
            void stub_TrackProperties();
            void stub_EnumProperties();
            void stub_GetDocumentMgr();
            void stub_CreateRangeBackup();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("D40C8AAE-AC92-4FC7-9A11-0EE0E23AA39B")]
        public interface ITfContextComposition
        {
            void StartComposition(int ecWrite, MS.Win32.UnsafeNativeMethods.ITfRange range, [MarshalAs(UnmanagedType.Interface)] object sink, [MarshalAs(UnmanagedType.Interface)] out object composition);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void EnumCompositions([MarshalAs(UnmanagedType.Interface)] out MS.Win32.UnsafeNativeMethods.IEnumITfCompositionView enumView);
            void FindComposition(int ecRead, MS.Win32.UnsafeNativeMethods.ITfRange testRange, [MarshalAs(UnmanagedType.Interface)] out object enumView);
            void TakeOwnership(int ecWrite, MS.Win32.UnsafeNativeMethods.ITfCompositionView view, [MarshalAs(UnmanagedType.Interface)] object sink, [MarshalAs(UnmanagedType.Interface)] out object composition);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("aa80e80c-2021-11d2-93e0-0060b067b86e")]
        public interface ITfContextOwner
        {
            void GetACPFromPoint(ref MS.Win32.UnsafeNativeMethods.POINT point, MS.Win32.UnsafeNativeMethods.GetPositionFromPointFlags flags, out int position);
            void GetTextExt(int start, int end, out MS.Win32.UnsafeNativeMethods.RECT rect, [MarshalAs(UnmanagedType.Bool)] out bool clipped);
            void GetScreenExt(out MS.Win32.UnsafeNativeMethods.RECT rect);
            void GetStatus(out MS.Win32.UnsafeNativeMethods.TS_STATUS status);
            void GetWnd(out IntPtr hwnd);
            void GetValue(ref Guid guidAttribute, out object varValue);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("86462810-593B-4916-9764-19C08E9CE110")]
        public interface ITfContextOwnerCompositionServices
        {
            void StartComposition(int ecWrite, MS.Win32.UnsafeNativeMethods.ITfRange range, [MarshalAs(UnmanagedType.Interface)] object sink, [MarshalAs(UnmanagedType.Interface)] out object composition);
            void EnumCompositions([MarshalAs(UnmanagedType.Interface)] out object enumView);
            void FindComposition(int ecRead, MS.Win32.UnsafeNativeMethods.ITfRange testRange, [MarshalAs(UnmanagedType.Interface)] out object enumView);
            void TakeOwnership(int ecWrite, MS.Win32.UnsafeNativeMethods.ITfCompositionView view, [MarshalAs(UnmanagedType.Interface)] object sink, [MarshalAs(UnmanagedType.Interface)] out object composition);
            [PreserveSig, SecurityCritical, SuppressUnmanagedCodeSecurity]
            int TerminateComposition(MS.Win32.UnsafeNativeMethods.ITfCompositionView view);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("5F20AA40-B57A-4F34-96AB-3576F377CC79")]
        public interface ITfContextOwnerCompositionSink
        {
            void OnStartComposition(MS.Win32.UnsafeNativeMethods.ITfCompositionView view, [MarshalAs(UnmanagedType.Bool)] out bool ok);
            void OnUpdateComposition(MS.Win32.UnsafeNativeMethods.ITfCompositionView view, MS.Win32.UnsafeNativeMethods.ITfRange rangeNew);
            void OnEndComposition(MS.Win32.UnsafeNativeMethods.ITfCompositionView view);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("b23eb630-3e1c-11d3-a745-0050040ab407")]
        public interface ITfContextOwnerServices
        {
            void stub_OnLayoutChange();
            void stub_OnStatusChange();
            void stub_OnAttributeChange();
            void stub_Serialize();
            void stub_Unserialize();
            void stub_ForceLoadProperty();
            void CreateRange(int acpStart, int acpEnd, out MS.Win32.UnsafeNativeMethods.ITfRangeACP range);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a305b1c0-c776-4523-bda0-7c5a2e0fef10")]
        public interface ITfContextRenderingMarkup
        {
            void GetRenderingMarkup(int editCookie, MS.Win32.UnsafeNativeMethods.GetRenderingMarkupFlags flags, MS.Win32.UnsafeNativeMethods.ITfRange range, out MS.Win32.UnsafeNativeMethods.IEnumTfRenderingMarkup enumMarkup);
            void FindNextRenderingMarkup(int editCookie, MS.Win32.UnsafeNativeMethods.FindRenderingMarkupFlags flags, MS.Win32.UnsafeNativeMethods.ITfRange queryRange, MS.Win32.UnsafeNativeMethods.TfAnchor queryAnchor, out MS.Win32.UnsafeNativeMethods.ITfRange foundRange, out MS.Win32.UnsafeNativeMethods.TF_RENDERINGMARKUP foundMarkup);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("70528852-2f26-4aea-8c96-215150578932"), SecurityCritical]
        public interface ITfDisplayAttributeInfo
        {
            [SecurityCritical]
            void stub_GetGUID();
            [SecurityCritical]
            void stub_GetDescription();
            [SecurityCritical]
            void GetAttributeInfo(out MS.Win32.UnsafeNativeMethods.TF_DISPLAYATTRIBUTE attr);
            [SecurityCritical]
            void stub_SetAttributeInfo();
            [SecurityCritical]
            void stub_Reset();
        }

        [ComImport, SuppressUnmanagedCodeSecurity, SecurityCritical, Guid("8ded7393-5db1-475c-9e71-a39111b0ff67"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfDisplayAttributeMgr
        {
            [SecurityCritical]
            void OnUpdateInfo();
            [SecurityCritical]
            void stub_EnumDisplayAttributeInfo();
            [SecurityCritical]
            void GetDisplayAttributeInfo(ref Guid guid, out MS.Win32.UnsafeNativeMethods.ITfDisplayAttributeInfo info, out Guid clsid);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("aa80e7f4-2021-11d2-93e0-0060b067b86e")]
        public interface ITfDocumentMgr
        {
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void CreateContext(int clientId, MS.Win32.UnsafeNativeMethods.CreateContextFlags flags, [MarshalAs(UnmanagedType.Interface)] object obj, out MS.Win32.UnsafeNativeMethods.ITfContext context, out int editCookie);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void Push(MS.Win32.UnsafeNativeMethods.ITfContext context);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void Pop(MS.Win32.UnsafeNativeMethods.PopFlags flags);
            void GetTop(out MS.Win32.UnsafeNativeMethods.ITfContext context);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetBase(out MS.Win32.UnsafeNativeMethods.ITfContext context);
            void EnumContexts([MarshalAs(UnmanagedType.Interface)] out object enumContexts);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("42d4d099-7c1a-4a89-b836-6c6f22160df0")]
        public interface ITfEditRecord
        {
            void GetSelectionStatus([MarshalAs(UnmanagedType.Bool)] out bool selectionChanged);
            void GetTextAndPropertyUpdates(int flags, ref IntPtr properties, int count, out MS.Win32.UnsafeNativeMethods.IEnumTfRanges ranges);
        }

        [ComImport, Guid("88f567c6-1757-49f8-a1b2-89234c1eeff9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfFnConfigure
        {
            void GetDisplayName([MarshalAs(UnmanagedType.BStr)] out string funcName);
            [PreserveSig]
            int Show(IntPtr hwndParent, short langid, ref Guid guidProfile);
        }

        [ComImport, Guid("bb95808a-6d8f-4bca-8400-5390b586aedf"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfFnConfigureRegisterWord
        {
            void GetDisplayName([MarshalAs(UnmanagedType.BStr)] out string funcName);
            [PreserveSig]
            int Show(IntPtr hwndParent, short langid, ref Guid guidProfile, [MarshalAs(UnmanagedType.BStr)] string bstrRegistered);
        }

        [ComImport, Guid("4cea93c0-0a58-11d3-8df0-00105a2799b5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfFnReconversion
        {
            void GetDisplayName([MarshalAs(UnmanagedType.BStr)] out string funcName);
            [PreserveSig, SecurityCritical, SuppressUnmanagedCodeSecurity]
            int QueryRange(MS.Win32.UnsafeNativeMethods.ITfRange range, out MS.Win32.UnsafeNativeMethods.ITfRange newRange, [MarshalAs(UnmanagedType.Bool)] out bool isConvertable);
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int GetReconversion(MS.Win32.UnsafeNativeMethods.ITfRange range, out MS.Win32.UnsafeNativeMethods.ITfCandidateList candList);
            [PreserveSig, SecurityCritical, SuppressUnmanagedCodeSecurity]
            int Reconvert(MS.Win32.UnsafeNativeMethods.ITfRange range);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("db593490-098f-11d3-8df0-00105a2799b5")]
        public interface ITfFunction
        {
            void GetDisplayName([MarshalAs(UnmanagedType.BStr)] out string funcName);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("101d6610-0990-11d3-8df0-00105a2799b5")]
        public interface ITfFunctionProvider
        {
            void GetType(out Guid guid);
            void GetDescription([MarshalAs(UnmanagedType.BStr)] out string desc);
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int GetFunction(ref Guid guid, ref Guid riid, [MarshalAs(UnmanagedType.Interface)] out object obj);
        }

        [ComImport, SecurityCritical, SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("1F02B6C5-7842-4EE6-8A0B-9A24183A95CA")]
        public interface ITfInputProcessorProfiles
        {
            [SecurityCritical]
            void stub_Register();
            [SecurityCritical]
            void stub_Unregister();
            [SecurityCritical]
            void stub_AddLanguageProfile();
            [SecurityCritical]
            void stub_RemoveLanguageProfile();
            [SecurityCritical]
            void stub_EnumInputProcessorInfo();
            [SecurityCritical]
            void stub_GetDefaultLanguageProfile();
            [SecurityCritical]
            void stub_SetDefaultLanguageProfile();
            [SecurityCritical]
            void ActivateLanguageProfile(ref Guid clsid, short langid, ref Guid guidProfile);
            [PreserveSig, SecurityCritical]
            int GetActiveLanguageProfile(ref Guid clsid, out short langid, out Guid profile);
            [SecurityCritical]
            void stub_GetLanguageProfileDescription();
            [SecurityCritical]
            void GetCurrentLanguage(out short langid);
            [PreserveSig, SecurityCritical]
            int ChangeCurrentLanguage(short langid);
            [PreserveSig, SecurityCritical]
            int GetLanguageList(out IntPtr langids, out int count);
            [SecurityCritical]
            void EnumLanguageProfiles(short langid, out MS.Win32.UnsafeNativeMethods.IEnumTfLanguageProfiles enumIPP);
            [SecurityCritical]
            void stub_EnableLanguageProfile();
            [SecurityCritical]
            void stub_IsEnabledLanguageProfile();
            [SecurityCritical]
            void stub_EnableLanguageProfileByDefault();
            [SecurityCritical]
            void stub_SubstituteKeyboardLayout();
        }

        [ComImport, Guid("fde1eaee-6924-4cdf-91e7-da38cff5559d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ITfInputScope
        {
            void GetInputScopes(out IntPtr ppinputscopes, out int count);
            [PreserveSig]
            int GetPhrase(out IntPtr ppbstrPhrases, out int count);
            [PreserveSig]
            int GetRegularExpression([MarshalAs(UnmanagedType.BStr)] out string desc);
            [PreserveSig]
            int GetSRGC([MarshalAs(UnmanagedType.BStr)] out string desc);
            [PreserveSig]
            int GetXML([MarshalAs(UnmanagedType.BStr)] out string desc);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("aa80e7f0-2021-11d2-93e0-0060b067b86e")]
        public interface ITfKeystrokeMgr
        {
            void AdviseKeyEventSink(int clientId, [MarshalAs(UnmanagedType.Interface)] object obj, [MarshalAs(UnmanagedType.Bool)] bool fForeground);
            void UnadviseKeyEventSink(int clientId);
            void GetForeground(out Guid clsid);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void TestKeyDown(int wParam, int lParam, [MarshalAs(UnmanagedType.Bool)] out bool eaten);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void TestKeyUp(int wParam, int lParam, [MarshalAs(UnmanagedType.Bool)] out bool eaten);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void KeyDown(int wParam, int lParam, [MarshalAs(UnmanagedType.Bool)] out bool eaten);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void KeyUp(int wParam, int lParam, [MarshalAs(UnmanagedType.Bool)] out bool eaten);
            void GetPreservedKey(MS.Win32.UnsafeNativeMethods.ITfContext context, ref MS.Win32.UnsafeNativeMethods.TF_PRESERVEDKEY key, out Guid guid);
            void IsPreservedKey(ref Guid guid, ref MS.Win32.UnsafeNativeMethods.TF_PRESERVEDKEY key, [MarshalAs(UnmanagedType.Bool)] out bool registered);
            void PreserveKey(int clientId, ref Guid guid, ref MS.Win32.UnsafeNativeMethods.TF_PRESERVEDKEY key, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] char[] desc, int descCount);
            void UnpreserveKey(ref Guid guid, ref MS.Win32.UnsafeNativeMethods.TF_PRESERVEDKEY key);
            void SetPreservedKeyDescription(ref Guid guid, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] char[] desc, int descCount);
            void GetPreservedKeyDescription(ref Guid guid, [MarshalAs(UnmanagedType.BStr)] out string desc);
            void SimulatePreservedKey(MS.Win32.UnsafeNativeMethods.ITfContext context, ref Guid guid, [MarshalAs(UnmanagedType.Bool)] out bool eaten);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("43c9fe15-f494-4c17-9de2-b8a4ac350aa8")]
        public interface ITfLanguageProfileNotifySink
        {
            void OnLanguageChange(short langid, [MarshalAs(UnmanagedType.Bool)] out bool bAccept);
            void OnLanguageChanged();
        }

        [ComImport, SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("8f1b8ad8-0b6b-4874-90c5-bd76011e8f7c")]
        internal interface ITfMessagePump
        {
            [SecurityCritical]
            void PeekMessageA(ref MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, int removeMsg, out int result);
            [SecurityCritical]
            void GetMessageA(ref MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, out int result);
            [SecurityCritical]
            void PeekMessageW(ref MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, int removeMsg, out int result);
            [SecurityCritical]
            void GetMessageW(ref MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, out bool result);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a1adaaa2-3a24-449d-ac96-5183e7f5c217")]
        public interface ITfMouseSink
        {
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int OnMouseEvent(int edge, int quadrant, int btnStatus, [MarshalAs(UnmanagedType.Bool)] out bool eaten);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("3bdd78e2-c16e-47fd-b883-ce6facc1a208")]
        public interface ITfMouseTrackerACP
        {
            [PreserveSig]
            int AdviceMouseSink(MS.Win32.UnsafeNativeMethods.ITfRangeACP range, MS.Win32.UnsafeNativeMethods.ITfMouseSink sink, out int dwCookie);
            [PreserveSig]
            int UnadviceMouseSink(int dwCookie);
        }

        [ComImport, SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e2449660-9542-11d2-bf46-00105a2799b5"), SecurityCritical]
        public interface ITfProperty
        {
            void GetType(out Guid type);
            [PreserveSig]
            int EnumRanges(int editcookie, out MS.Win32.UnsafeNativeMethods.IEnumTfRanges ranges, MS.Win32.UnsafeNativeMethods.ITfRange targetRange);
            void GetValue(int editCookie, MS.Win32.UnsafeNativeMethods.ITfRange range, out object value);
            void GetContext(out MS.Win32.UnsafeNativeMethods.ITfContext context);
            void FindRange(int editCookie, MS.Win32.UnsafeNativeMethods.ITfRange inRange, out MS.Win32.UnsafeNativeMethods.ITfRange outRange, MS.Win32.UnsafeNativeMethods.TfAnchor position);
            void stub_SetValueStore();
            void SetValue(int editCookie, MS.Win32.UnsafeNativeMethods.ITfRange range, object value);
            void Clear(int editCookie, MS.Win32.UnsafeNativeMethods.ITfRange range);
        }

        [ComImport, SuppressUnmanagedCodeSecurity, SecurityCritical, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("aa80e7ff-2021-11d2-93e0-0060b067b86e")]
        public interface ITfRange
        {
            [SecurityCritical]
            void GetText(int ec, int flags, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] char[] text, int countMax, out int count);
            [SecurityCritical]
            void SetText(int ec, int flags, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] char[] text, int count);
            [SecurityCritical]
            void GetFormattedText(int ec, [MarshalAs(UnmanagedType.Interface)] out object data);
            [SecurityCritical]
            void GetEmbedded(int ec, ref Guid guidService, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out object obj);
            [SecurityCritical]
            void InsertEmbedded(int ec, int flags, [MarshalAs(UnmanagedType.Interface)] object data);
            [SecurityCritical]
            void ShiftStart(int ec, int count, out int result, int ZeroForNow);
            [SecurityCritical]
            void ShiftEnd(int ec, int count, out int result, int ZeroForNow);
            [SecurityCritical]
            void ShiftStartToRange(int ec, MS.Win32.UnsafeNativeMethods.ITfRange range, MS.Win32.UnsafeNativeMethods.TfAnchor position);
            [SecurityCritical]
            void ShiftEndToRange(int ec, MS.Win32.UnsafeNativeMethods.ITfRange range, MS.Win32.UnsafeNativeMethods.TfAnchor position);
            [SecurityCritical]
            void ShiftStartRegion(int ec, MS.Win32.UnsafeNativeMethods.TfShiftDir dir, [MarshalAs(UnmanagedType.Bool)] out bool noRegion);
            [SecurityCritical]
            void ShiftEndRegion(int ec, MS.Win32.UnsafeNativeMethods.TfShiftDir dir, [MarshalAs(UnmanagedType.Bool)] out bool noRegion);
            [SecurityCritical]
            void IsEmpty(int ec, [MarshalAs(UnmanagedType.Bool)] out bool empty);
            [SecurityCritical]
            void Collapse(int ec, MS.Win32.UnsafeNativeMethods.TfAnchor position);
            [SecurityCritical]
            void IsEqualStart(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, [MarshalAs(UnmanagedType.Bool)] out bool equal);
            [SecurityCritical]
            void IsEqualEnd(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, [MarshalAs(UnmanagedType.Bool)] out bool equal);
            [SecurityCritical]
            void CompareStart(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, out int result);
            [SecurityCritical]
            void CompareEnd(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, out int result);
            [SecurityCritical]
            void AdjustForInsert(int ec, int count, [MarshalAs(UnmanagedType.Bool)] out bool insertOk);
            [SecurityCritical]
            void GetGravity(out MS.Win32.UnsafeNativeMethods.TfGravity start, out MS.Win32.UnsafeNativeMethods.TfGravity end);
            [SecurityCritical]
            void SetGravity(int ec, MS.Win32.UnsafeNativeMethods.TfGravity start, MS.Win32.UnsafeNativeMethods.TfGravity end);
            [SecurityCritical]
            void Clone(out MS.Win32.UnsafeNativeMethods.ITfRange clone);
            [SecurityCritical]
            void GetContext(out MS.Win32.UnsafeNativeMethods.ITfContext context);
        }

        [ComImport, SecurityCritical, Guid("057a6296-029b-4154-b79a-0d461d4ea94c"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity]
        public interface ITfRangeACP
        {
            [SecurityCritical]
            void GetText(int ec, int flags, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] char[] text, int countMax, out int count);
            [SecurityCritical]
            void SetText(int ec, int flags, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=3)] char[] text, int count);
            [SecurityCritical]
            void GetFormattedText(int ec, [MarshalAs(UnmanagedType.Interface)] out object data);
            [SecurityCritical]
            void GetEmbedded(int ec, ref Guid guidService, ref Guid iid, [MarshalAs(UnmanagedType.Interface)] out object obj);
            [SecurityCritical]
            void InsertEmbedded(int ec, int flags, [MarshalAs(UnmanagedType.Interface)] object data);
            [SecurityCritical]
            void ShiftStart(int ec, int count, out int result, int ZeroForNow);
            [SecurityCritical]
            void ShiftEnd(int ec, int count, out int result, int ZeroForNow);
            [SecurityCritical]
            void ShiftStartToRange(int ec, MS.Win32.UnsafeNativeMethods.ITfRange range, MS.Win32.UnsafeNativeMethods.TfAnchor position);
            [SecurityCritical]
            void ShiftEndToRange(int ec, MS.Win32.UnsafeNativeMethods.ITfRange range, MS.Win32.UnsafeNativeMethods.TfAnchor position);
            [SecurityCritical]
            void ShiftStartRegion(int ec, MS.Win32.UnsafeNativeMethods.TfShiftDir dir, [MarshalAs(UnmanagedType.Bool)] out bool noRegion);
            [SecurityCritical]
            void ShiftEndRegion(int ec, MS.Win32.UnsafeNativeMethods.TfShiftDir dir, [MarshalAs(UnmanagedType.Bool)] out bool noRegion);
            [SecurityCritical]
            void IsEmpty(int ec, [MarshalAs(UnmanagedType.Bool)] out bool empty);
            [SecurityCritical]
            void Collapse(int ec, MS.Win32.UnsafeNativeMethods.TfAnchor position);
            [SecurityCritical]
            void IsEqualStart(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, [MarshalAs(UnmanagedType.Bool)] out bool equal);
            [SecurityCritical]
            void IsEqualEnd(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, [MarshalAs(UnmanagedType.Bool)] out bool equal);
            [SecurityCritical]
            void CompareStart(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, out int result);
            [SecurityCritical]
            void CompareEnd(int ec, MS.Win32.UnsafeNativeMethods.ITfRange with, MS.Win32.UnsafeNativeMethods.TfAnchor position, out int result);
            [SecurityCritical]
            void AdjustForInsert(int ec, int count, [MarshalAs(UnmanagedType.Bool)] out bool insertOk);
            [SecurityCritical]
            void GetGravity(out MS.Win32.UnsafeNativeMethods.TfGravity start, out MS.Win32.UnsafeNativeMethods.TfGravity end);
            [SecurityCritical]
            void SetGravity(int ec, MS.Win32.UnsafeNativeMethods.TfGravity start, MS.Win32.UnsafeNativeMethods.TfGravity end);
            [SecurityCritical]
            void Clone(out MS.Win32.UnsafeNativeMethods.ITfRange clone);
            [SecurityCritical]
            void GetContext(out MS.Win32.UnsafeNativeMethods.ITfContext context);
            [SecurityCritical]
            void GetExtent(out int start, out int count);
            [SecurityCritical]
            void SetExtent(int start, int count);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("4ea48a35-60ae-446f-8fd6-e6a8d82459f7")]
        public interface ITfSource
        {
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void AdviseSink(ref Guid riid, [MarshalAs(UnmanagedType.Interface)] object obj, out int cookie);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void UnadviseSink(int cookie);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("8127d409-ccd3-4683-967a-b43d5b482bf7")]
        public interface ITfTextEditSink
        {
            void OnEndEdit(MS.Win32.UnsafeNativeMethods.ITfContext context, int ecReadOnly, MS.Win32.UnsafeNativeMethods.ITfEditRecord editRecord);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("c0f1db0c-3a20-405c-a303-96b6010a885f")]
        public interface ITfThreadFocusSink
        {
            void OnSetThreadFocus();
            void OnKillThreadFocus();
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("aa80e801-2021-11d2-93e0-0060b067b86e")]
        internal interface ITfThreadMgr
        {
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void Activate(out int clientId);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void Deactivate();
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void CreateDocumentMgr(out MS.Win32.UnsafeNativeMethods.ITfDocumentMgr docMgr);
            void EnumDocumentMgrs(out MS.Win32.UnsafeNativeMethods.IEnumTfDocumentMgrs enumDocMgrs);
            void GetFocus(out MS.Win32.UnsafeNativeMethods.ITfDocumentMgr docMgr);
            [SecurityCritical, SuppressUnmanagedCodeSecurity]
            void SetFocus(MS.Win32.UnsafeNativeMethods.ITfDocumentMgr docMgr);
            void AssociateFocus(IntPtr hwnd, MS.Win32.UnsafeNativeMethods.ITfDocumentMgr newDocMgr, out MS.Win32.UnsafeNativeMethods.ITfDocumentMgr prevDocMgr);
            void IsThreadFocus([MarshalAs(UnmanagedType.Bool)] out bool isFocus);
            [PreserveSig, SuppressUnmanagedCodeSecurity, SecurityCritical]
            int GetFunctionProvider(ref Guid classId, out MS.Win32.UnsafeNativeMethods.ITfFunctionProvider funcProvider);
            void EnumFunctionProviders(out MS.Win32.UnsafeNativeMethods.IEnumTfFunctionProviders enumProviders);
            [SuppressUnmanagedCodeSecurity, SecurityCritical]
            void GetGlobalCompartment(out MS.Win32.UnsafeNativeMethods.ITfCompartmentMgr compartmentMgr);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("a615096f-1c57-4813-8a15-55ee6e5a839c")]
        public interface ITfTransitoryExtensionSink
        {
            void OnTransitoryExtensionUpdated(MS.Win32.UnsafeNativeMethods.ITfContext context, int ecReadOnly, MS.Win32.UnsafeNativeMethods.ITfRange rangeResult, MS.Win32.UnsafeNativeMethods.ITfRange rangeComposition, [MarshalAs(UnmanagedType.Bool)] out bool fDeleteResultRange);
        }

        [ComImport, Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E"), TypeLibType(TypeLibTypeFlags.FOleAutomation | TypeLibTypeFlags.FDual | TypeLibTypeFlags.FHidden)]
        public interface IWebBrowser2
        {
            [SecurityCritical, DispId(100), SuppressUnmanagedCodeSecurity]
            void GoBack();
            [SuppressUnmanagedCodeSecurity, DispId(0x65), SecurityCritical]
            void GoForward();
            [DispId(0x66)]
            void GoHome();
            [DispId(0x67)]
            void GoSearch();
            [DispId(0x68)]
            void Navigate([In] string Url, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);
            [DispId(-550), SuppressUnmanagedCodeSecurity, SecurityCritical]
            void Refresh();
            [SecurityCritical, SuppressUnmanagedCodeSecurity, DispId(0x69)]
            void Refresh2([In] ref object level);
            [DispId(0x6a)]
            void Stop();
            [DispId(200)]
            object Application { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
            [DispId(0xc9)]
            object Parent { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
            [DispId(0xca)]
            object Container { [return: MarshalAs(UnmanagedType.IDispatch)] get; }
            [DispId(0xcb)]
            object Document { [return: MarshalAs(UnmanagedType.IDispatch)] [SuppressUnmanagedCodeSecurity, SecurityCritical] get; }
            [DispId(0xcc)]
            bool TopLevelContainer { get; }
            [DispId(0xcd)]
            string Type { get; }
            [DispId(0xce)]
            int Left { get; set; }
            [DispId(0xcf)]
            int Top { get; set; }
            [DispId(0xd0)]
            int Width { get; set; }
            [DispId(0xd1)]
            int Height { get; set; }
            [DispId(210)]
            string LocationName { get; }
            [DispId(0xd3)]
            string LocationURL { [SuppressUnmanagedCodeSecurity, SecurityCritical] get; }
            [DispId(0xd4)]
            bool Busy { get; }
            [DispId(300)]
            void Quit();
            [DispId(0x12d)]
            void ClientToWindow(out int pcx, out int pcy);
            [DispId(0x12e)]
            void PutProperty([In] string property, [In] object vtValue);
            [DispId(0x12f)]
            object GetProperty([In] string property);
            [DispId(0)]
            string Name { get; }
            [DispId(-515)]
            int HWND { get; }
            [DispId(400)]
            string FullName { get; }
            [DispId(0x191)]
            string Path { get; }
            [DispId(0x192)]
            bool Visible { get; set; }
            [DispId(0x193)]
            bool StatusBar { get; set; }
            [DispId(0x194)]
            string StatusText { get; set; }
            [DispId(0x195)]
            int ToolBar { get; set; }
            [DispId(0x196)]
            bool MenuBar { get; set; }
            [DispId(0x197)]
            bool FullScreen { get; set; }
            [SecurityCritical, SuppressUnmanagedCodeSecurity, DispId(500)]
            void Navigate2([In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers);
            [DispId(0x1f5)]
            MS.Win32.UnsafeNativeMethods.OLECMDF QueryStatusWB([In] MS.Win32.UnsafeNativeMethods.OLECMDID cmdID);
            [DispId(0x1f6)]
            void ExecWB([In] MS.Win32.UnsafeNativeMethods.OLECMDID cmdID, [In] MS.Win32.UnsafeNativeMethods.OLECMDEXECOPT cmdexecopt, ref object pvaIn, IntPtr pvaOut);
            [DispId(0x1f7)]
            void ShowBrowserBar([In] ref object pvaClsid, [In] ref object pvarShow, [In] ref object pvarSize);
            [DispId(-525)]
            NativeMethods.WebBrowserReadyState ReadyState { get; }
            [DispId(550)]
            bool Offline { get; set; }
            [DispId(0x227)]
            bool Silent { get; set; }
            [DispId(0x228)]
            bool RegisterAsBrowser { get; set; }
            [DispId(0x229)]
            bool RegisterAsDropTarget { get; set; }
            [DispId(0x22a)]
            bool TheaterMode { get; set; }
            [DispId(0x22b)]
            bool AddressBar { get; set; }
            [DispId(0x22c)]
            bool Resizable { get; set; }
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct LARGE_INTEGER
        {
            [FieldOffset(4)]
            internal int HighPart;
            [FieldOffset(0)]
            internal int LowPart;
            [FieldOffset(0)]
            internal long QuadPart;
        }

        [Flags]
        public enum LockFlags
        {
            TS_LF_READ = 2,
            TS_LF_READWRITE = 6,
            TS_LF_SYNC = 1,
            TS_LF_WRITE = 4
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        internal struct MOUSEQUERY
        {
            internal uint uMsg;
            internal IntPtr wParam;
            internal IntPtr lParam;
            internal int ptX;
            internal int ptY;
            internal IntPtr hwnd;
        }

        public enum OLECMDEXECOPT
        {
            OLECMDEXECOPT_DODEFAULT,
            OLECMDEXECOPT_PROMPTUSER,
            OLECMDEXECOPT_DONTPROMPTUSER,
            OLECMDEXECOPT_SHOWHELP
        }

        public enum OLECMDF
        {
            OLECMDF_DEFHIDEONCTXTMENU = 0x20,
            OLECMDF_ENABLED = 2,
            OLECMDF_INVISIBLE = 0x10,
            OLECMDF_LATCHED = 4,
            OLECMDF_NINCHED = 8,
            OLECMDF_SUPPORTED = 1
        }

        public enum OLECMDID
        {
            OLECMDID_COPY = 12,
            OLECMDID_CUT = 11,
            OLECMDID_PAGESETUP = 8,
            OLECMDID_PASTE = 13,
            OLECMDID_PRINT = 6,
            OLECMDID_PRINTPREVIEW = 7,
            OLECMDID_PROPERTIES = 10,
            OLECMDID_REFRESH = 0x16,
            OLECMDID_SAVE = 3,
            OLECMDID_SAVEAS = 4,
            OLECMDID_SELECTALL = 0x11,
            OLECMDID_STOP = 0x17
        }

        [Flags]
        public enum OnTextChangeFlags
        {
            TS_TC_CORRECTION = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTSTRUCT
        {
            public int x;
            public int y;
            public POINTSTRUCT(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [Flags]
        public enum PopFlags
        {
            TF_POPF_ALL = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        internal sealed class SafeFileMappingHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [SecurityTreatAsSafe, SecurityCritical]
            internal SafeFileMappingHandle() : base(true)
            {
            }

            [SecurityCritical, SecurityTreatAsSafe]
            protected override bool ReleaseHandle()
            {
                bool flag;
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
                try
                {
                    flag = MS.Win32.UnsafeNativeMethods.CloseHandleNoThrow(new HandleRef(null, base.handle));
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                return flag;
            }

            public override bool IsInvalid =>
                (base.handle == IntPtr.Zero);
        }

        internal sealed class SafeViewOfFileHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            [SecurityCritical, SecurityTreatAsSafe]
            internal SafeViewOfFileHandle() : base(true)
            {
            }

            [SecurityCritical, SecurityTreatAsSafe]
            protected override bool ReleaseHandle()
            {
                bool flag;
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
                try
                {
                    flag = MS.Win32.UnsafeNativeMethods.UnmapViewOfFileNoThrow(new HandleRef(null, base.handle));
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                return flag;
            }

            internal void* Memory =>
                ((void*) base.handle);
        }

        [Flags]
        public enum SentenceModeFlags
        {
            TF_SENTENCEMODE_AUTOMATIC = 4,
            TF_SENTENCEMODE_CONVERSATION = 0x10,
            TF_SENTENCEMODE_NONE = 0,
            TF_SENTENCEMODE_PHRASEPREDICT = 8,
            TF_SENTENCEMODE_PLAURALCLAUSE = 1,
            TF_SENTENCEMODE_SINGLECONVERT = 2
        }

        [Flags]
        public enum SetTextFlags
        {
            TS_ST_CORRECTION = 1
        }

        [Flags]
        internal enum ShellExecuteFlags
        {
            SEE_MASK_ASYNCOK = 0x100000,
            SEE_MASK_CLASSKEY = 3,
            SEE_MASK_CLASSNAME = 1,
            SEE_MASK_DOENVSUBST = 0x200,
            SEE_MASK_FLAG_DDEWAIT = 0x100,
            SEE_MASK_FLAG_NO_UI = 0x400,
            SEE_MASK_HMONITOR = 0x200000,
            SEE_MASK_NO_CONSOLE = 0x8000,
            SEE_MASK_NOCLOSEPROCESS = 0x40,
            SEE_MASK_NOQUERYCLASSSTORE = 0x1000000,
            SEE_MASK_NOZONECHECKS = 0x800000,
            SEE_MASK_UNICODE = 0x4000,
            SEE_MASK_WAITFORINPUTIDLE = 0x2000000
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        internal class ShellExecuteInfo
        {
            public int cbSize;
            public MS.Win32.UnsafeNativeMethods.ShellExecuteFlags fMask;
            public IntPtr hwnd;
            public string lpVerb;
            public string lpFile;
            public string lpParameters;
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            public string lpClass;
            public IntPtr hkeyClass;
            public int dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        [Flags]
        public enum StaticStatusFlags
        {
            TS_SS_DISJOINTSEL = 1,
            TS_SS_NOHIDDENTEXT = 8,
            TS_SS_REGIONS = 2,
            TS_SS_TRANSITORY = 4
        }

        public enum TF_DA_ATTR_INFO
        {
            TF_ATTR_CONVERTED = 2,
            TF_ATTR_FIXEDCONVERTED = 5,
            TF_ATTR_INPUT = 0,
            TF_ATTR_INPUT_ERROR = 4,
            TF_ATTR_OTHER = -1,
            TF_ATTR_TARGET_CONVERTED = 1,
            TF_ATTR_TARGET_NOTCONVERTED = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TF_DA_COLOR
        {
            public MS.Win32.UnsafeNativeMethods.TF_DA_COLORTYPE type;
            public int indexOrColorRef;
        }

        public enum TF_DA_COLORTYPE
        {
            TF_CT_NONE,
            TF_CT_SYSCOLOR,
            TF_CT_COLORREF
        }

        public enum TF_DA_LINESTYLE
        {
            TF_LS_NONE,
            TF_LS_SOLID,
            TF_LS_DOT,
            TF_LS_DASH,
            TF_LS_SQUIGGLE
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TF_DISPLAYATTRIBUTE
        {
            public MS.Win32.UnsafeNativeMethods.TF_DA_COLOR crText;
            public MS.Win32.UnsafeNativeMethods.TF_DA_COLOR crBk;
            public MS.Win32.UnsafeNativeMethods.TF_DA_LINESTYLE lsStyle;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fBoldLine;
            public MS.Win32.UnsafeNativeMethods.TF_DA_COLOR crLine;
            public MS.Win32.UnsafeNativeMethods.TF_DA_ATTR_INFO bAttr;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TF_LANGUAGEPROFILE
        {
            internal Guid clsid;
            internal short langid;
            internal Guid catid;
            [MarshalAs(UnmanagedType.Bool)]
            internal bool fActive;
            internal Guid guidProfile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TF_PRESERVEDKEY
        {
            public int vKey;
            public int modifiers;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TF_RENDERINGMARKUP
        {
            public MS.Win32.UnsafeNativeMethods.ITfRange range;
            public MS.Win32.UnsafeNativeMethods.TF_DISPLAYATTRIBUTE tfDisplayAttr;
        }

        public enum TfAnchor
        {
            TF_ANCHOR_START,
            TF_ANCHOR_END
        }

        public enum TfCandidateResult
        {
            CAND_FINALIZED,
            CAND_SELECTED,
            CAND_CANCELED
        }

        public enum TfGravity
        {
            TF_GR_BACKWARD,
            TF_GR_FORWARD
        }

        public enum TfShiftDir
        {
            TF_SD_BACKWARD,
            TF_SD_FORWARD
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TS_ATTRVAL
        {
            public Guid attributeId;
            public int overlappedId;
            public int reserved;
            [MarshalAs(UnmanagedType.Struct)]
            public NativeMethods.VARIANT val;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TS_RUNINFO
        {
            public int count;
            public MS.Win32.UnsafeNativeMethods.TsRunType type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TS_SELECTION_ACP
        {
            public int start;
            public int end;
            public MS.Win32.UnsafeNativeMethods.TS_SELECTIONSTYLE style;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TS_SELECTIONSTYLE
        {
            public MS.Win32.UnsafeNativeMethods.TsActiveSelEnd ase;
            [MarshalAs(UnmanagedType.Bool)]
            public bool interimChar;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TS_STATUS
        {
            public MS.Win32.UnsafeNativeMethods.DynamicStatusFlags dynamicFlags;
            public MS.Win32.UnsafeNativeMethods.StaticStatusFlags staticFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TS_TEXTCHANGE
        {
            public int start;
            public int oldEnd;
            public int newEnd;
        }

        public enum TsActiveSelEnd
        {
            TS_AE_NONE,
            TS_AE_START,
            TS_AE_END
        }

        public enum TsGravity
        {
            TS_GR_BACKWARD,
            TS_GR_FORWARD
        }

        public enum TsLayoutCode
        {
            TS_LC_CREATE,
            TS_LC_CHANGE,
            TS_LC_DESTROY
        }

        public enum TsRunType
        {
            TS_RT_PLAIN,
            TS_RT_HIDDEN,
            TS_RT_OPAQUE
        }

        public enum TsShiftDir
        {
            TS_SD_BACKWARD,
            TS_SD_FORWARD
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct ULARGE_INTEGER
        {
            [FieldOffset(4)]
            internal uint HighPart;
            [FieldOffset(0)]
            internal uint LowPart;
            [FieldOffset(0)]
            internal ulong QuadPart;
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical(SecurityCriticalScope.Everything)]
        internal class WIC
        {
            internal static readonly Guid WICPixelFormat32bppPBGRA = new Guid(0x6fddc324, 0x4e03, 0x4bfe, 0xb1, 0x85, 0x3d, 0x77, 0x76, 0x8d, 0xc9, 0x10);
            internal const int WINCODEC_SDK_VERSION = 0x236;

            [DllImport("WindowsCodecs.dll", EntryPoint="IWICBitmapSource_CopyPixels_Proxy")]
            internal static extern int CopyPixels(IntPtr THIS_PTR, ref Int32Rect prc, int cbStride, int cbBufferSize, IntPtr pvPixels);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICImagingFactory_CreateBitmapFlipRotator_Proxy")]
            internal static extern int CreateBitmapFlipRotator(IntPtr pICodecFactory, out IntPtr ppBitmapFlipRotator);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICImagingFactory_CreateDecoderFromStream_Proxy")]
            internal static extern int CreateDecoderFromStream(IntPtr pICodecFactory, IntPtr pIStream, ref Guid guidVendor, uint metadataFlags, out IntPtr ppIDecode);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICImagingFactory_CreateFormatConverter_Proxy")]
            internal static extern int CreateFormatConverter(IntPtr pICodecFactory, out IntPtr ppFormatConverter);
            [DllImport("WindowsCodecs.dll", EntryPoint="WICCreateImagingFactory_Proxy")]
            internal static extern int CreateImagingFactory(uint SDKVersion, out IntPtr ppICodecFactory);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICImagingFactory_CreateStream_Proxy")]
            internal static extern int CreateStream(IntPtr pICodecFactory, out IntPtr ppIStream);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICBitmapSource_GetSize_Proxy")]
            internal static extern int GetBitmapSize(IntPtr THIS_PTR, out int puiWidth, out int puiHeight);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICBitmapDecoder_GetFrame_Proxy")]
            internal static extern int GetFrame(IntPtr THIS_PTR, uint index, out IntPtr ppIFrameDecode);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICBitmapFlipRotator_Initialize_Proxy")]
            internal static extern int InitializeBitmapFlipRotator(IntPtr THIS_PTR, IntPtr source, WICBitmapTransformOptions options);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICFormatConverter_Initialize_Proxy")]
            internal static extern int InitializeFormatConverter(IntPtr THIS_PTR, IntPtr source, ref Guid dstFormat, int dither, IntPtr bitmapPalette, double alphaThreshold, WICPaletteType paletteTranslate);
            [DllImport("WindowsCodecs.dll", EntryPoint="IWICStream_InitializeFromMemory_Proxy")]
            internal static extern int InitializeStreamFromMemory(IntPtr pIWICStream, IntPtr pbBuffer, uint cbSize);

            internal enum WICBitmapTransformOptions
            {
                WICBitmapTransformFlipHorizontal = 8,
                WICBitmapTransformFlipVertical = 0x10,
                WICBitmapTransformRotate0 = 0,
                WICBitmapTransformRotate180 = 2,
                WICBitmapTransformRotate270 = 3,
                WICBitmapTransformRotate90 = 1
            }

            internal enum WICPaletteType
            {
                WICPaletteTypeCustom = 0,
                WICPaletteTypeFixedBW = 2,
                WICPaletteTypeFixedGray16 = 11,
                WICPaletteTypeFixedGray256 = 12,
                WICPaletteTypeFixedGray4 = 10,
                WICPaletteTypeFixedHalftone125 = 6,
                WICPaletteTypeFixedHalftone216 = 7,
                WICPaletteTypeFixedHalftone252 = 8,
                WICPaletteTypeFixedHalftone256 = 9,
                WICPaletteTypeFixedHalftone27 = 4,
                WICPaletteTypeFixedHalftone64 = 5,
                WICPaletteTypeFixedHalftone8 = 3,
                WICPaletteTypeFixedWebPalette = 7,
                WICPaletteTypeOptimal = 1
            }
        }
    }
}

