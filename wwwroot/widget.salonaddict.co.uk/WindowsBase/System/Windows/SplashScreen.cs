namespace System.Windows
{
    using MS.Internal;
    using MS.Utility;
    using MS.Win32;
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Resources;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Threading;

    [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
    public class SplashScreen
    {
        private NativeMethods.BLENDFUNCTION _blendFunc;
        private static NativeMethods.WndProc _defWndProc;
        private DispatcherTimer _dt;
        private TimeSpan _fadeoutDuration;
        private DateTime _fadeoutEnd;
        private NativeMethods.BitmapHandle _hBitmap;
        private IntPtr _hInstance;
        private IntPtr _hwnd;
        private ResourceManager _resourceManager;
        private string _resourceName;
        private ushort _wndClass;
        private const string CLASSNAME = "SplashScreen";

        public SplashScreen(string resourceName) : this(Assembly.GetEntryAssembly(), resourceName)
        {
        }

        [SecurityCritical]
        public SplashScreen(Assembly resourceAssembly, string resourceName)
        {
            this._hwnd = IntPtr.Zero;
            this._resourceName = resourceName.ToLowerInvariant();
            this._hInstance = Marshal.GetHINSTANCE(resourceAssembly.ManifestModule);
            AssemblyName name = new AssemblyName(resourceAssembly.FullName);
            this._resourceManager = new ResourceManager(name.Name + ".g", resourceAssembly);
        }

        [SecurityCritical]
        public void Close(TimeSpan fadeoutDuration)
        {
            if (fadeoutDuration <= TimeSpan.Zero)
            {
                this.DestroyResources();
            }
            else if (this._dt == null)
            {
                MS.Win32.UnsafeNativeMethods.SetActiveWindow(new HandleRef(null, this._hwnd));
                this._dt = new DispatcherTimer();
                this._dt.Interval = TimeSpan.FromMilliseconds(30.0);
                this._fadeoutDuration = fadeoutDuration;
                this._fadeoutEnd = DateTime.UtcNow + this._fadeoutDuration;
                this._dt.Tick += new EventHandler(this.Fadeout_Tick);
                this._dt.Start();
            }
        }

        [SecurityCritical]
        private bool CreateLayeredWindowFromImgBuffer(IntPtr pImgBuffer, long cImgBufferLen)
        {
            bool flag = false;
            IntPtr zero = IntPtr.Zero;
            IntPtr ppIDecode = IntPtr.Zero;
            IntPtr ppIStream = IntPtr.Zero;
            IntPtr ppIFrameDecode = IntPtr.Zero;
            IntPtr ppFormatConverter = IntPtr.Zero;
            IntPtr ppBitmapFlipRotator = IntPtr.Zero;
            try
            {
                int num;
                int num2;
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.CreateImagingFactory(0x236, out zero));
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.CreateStream(zero, out ppIStream));
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.InitializeStreamFromMemory(ppIStream, pImgBuffer, (uint) cImgBufferLen));
                Guid empty = Guid.Empty;
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.CreateDecoderFromStream(zero, ppIStream, ref empty, 0, out ppIDecode));
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.GetFrame(ppIDecode, 0, out ppIFrameDecode));
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.CreateFormatConverter(zero, out ppFormatConverter));
                Guid dstFormat = MS.Win32.UnsafeNativeMethods.WIC.WICPixelFormat32bppPBGRA;
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.InitializeFormatConverter(ppFormatConverter, ppIFrameDecode, ref dstFormat, 0, IntPtr.Zero, 0.0, MS.Win32.UnsafeNativeMethods.WIC.WICPaletteType.WICPaletteTypeCustom));
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.CreateBitmapFlipRotator(zero, out ppBitmapFlipRotator));
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.InitializeBitmapFlipRotator(ppBitmapFlipRotator, ppFormatConverter, MS.Win32.UnsafeNativeMethods.WIC.WICBitmapTransformOptions.WICBitmapTransformFlipVertical));
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.GetBitmapSize(ppBitmapFlipRotator, out num, out num2));
                int cbStride = num * 4;
                NativeMethods.BITMAPINFO bitmapInfo = new NativeMethods.BITMAPINFO(num, num2, 0x20) {
                    bmiHeader_biCompression = 0,
                    bmiHeader_biSizeImage = cbStride * num2
                };
                IntPtr ppvBits = IntPtr.Zero;
                this._hBitmap = MS.Win32.UnsafeNativeMethods.CreateDIBSection(new HandleRef(), ref bitmapInfo, 0, ref ppvBits, IntPtr.Zero, 0);
                Int32Rect prc = new Int32Rect(0, 0, num, num2);
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(MS.Win32.UnsafeNativeMethods.WIC.CopyPixels(ppBitmapFlipRotator, ref prc, cbStride, cbStride * num2, ppvBits));
                EventTrace.NormalTraceEvent(EventTraceGuidId.SPLASHSCREENGUID, 11);
                this._hwnd = this.CreateWindow(this._hBitmap, num, num2);
                flag = true;
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.Release(zero);
                }
                if (ppIDecode != IntPtr.Zero)
                {
                    Marshal.Release(ppIDecode);
                }
                if (ppIStream != IntPtr.Zero)
                {
                    Marshal.Release(ppIStream);
                }
                if (ppIFrameDecode != IntPtr.Zero)
                {
                    Marshal.Release(ppIFrameDecode);
                }
                if (ppFormatConverter != IntPtr.Zero)
                {
                    Marshal.Release(ppFormatConverter);
                }
                if (ppBitmapFlipRotator != IntPtr.Zero)
                {
                    Marshal.Release(ppBitmapFlipRotator);
                }
                if (!flag)
                {
                    this.DestroyResources();
                }
            }
            return flag;
        }

        [SecurityCritical]
        private IntPtr CreateWindow(NativeMethods.BitmapHandle hBitmap, int width, int height)
        {
            if (_defWndProc == null)
            {
                _defWndProc = new NativeMethods.WndProc(MS.Win32.UnsafeNativeMethods.DefWindowProc);
            }
            NativeMethods.WNDCLASSEX_D wndclassex_d = new NativeMethods.WNDCLASSEX_D {
                cbSize = Marshal.SizeOf(typeof(NativeMethods.WNDCLASSEX_D)),
                style = 3,
                lpfnWndProc = null,
                hInstance = this._hInstance,
                hCursor = IntPtr.Zero,
                lpszClassName = "SplashScreen",
                lpszMenuName = string.Empty,
                lpfnWndProc = _defWndProc
            };
            this._wndClass = MS.Win32.UnsafeNativeMethods.IntRegisterClassEx(wndclassex_d);
            if ((this._wndClass == 0) && (Marshal.GetLastWin32Error() != 0x582))
            {
                throw new Win32Exception();
            }
            int systemMetrics = MS.Win32.UnsafeNativeMethods.GetSystemMetrics(0);
            int num2 = MS.Win32.UnsafeNativeMethods.GetSystemMetrics(1);
            int x = (systemMetrics - width) / 2;
            int y = (num2 - height) / 2;
            HandleRef hWndParent = new HandleRef(null, IntPtr.Zero);
            IntPtr hwnd = MS.Win32.UnsafeNativeMethods.CreateWindowEx(0x80180, "SplashScreen", System.Windows.SR.Get("SplashScreenIsLoading"), -1879048192, x, y, width, height, hWndParent, hWndParent, new HandleRef(null, this._hInstance), IntPtr.Zero);
            EventTrace.NormalTraceEvent(EventTraceGuidId.SPLASHSCREENGUID, 12);
            IntPtr dC = MS.Win32.UnsafeNativeMethods.GetDC(new HandleRef());
            IntPtr handle = MS.Win32.UnsafeNativeMethods.CreateCompatibleDC(new HandleRef(null, dC));
            MS.Win32.UnsafeNativeMethods.SelectObject(new HandleRef(null, handle), hBitmap.MakeHandleRef(null).Handle);
            NativeMethods.POINT pSizeDst = new NativeMethods.POINT(width, height);
            NativeMethods.POINT pptDst = new NativeMethods.POINT(x, y);
            NativeMethods.POINT pptSrc = new NativeMethods.POINT(0, 0);
            this._blendFunc = new NativeMethods.BLENDFUNCTION();
            this._blendFunc.BlendOp = 0;
            this._blendFunc.BlendFlags = 0;
            this._blendFunc.SourceConstantAlpha = 0xff;
            this._blendFunc.AlphaFormat = 1;
            bool flag = MS.Win32.UnsafeNativeMethods.UpdateLayeredWindow(hwnd, dC, pptDst, pSizeDst, handle, pptSrc, 0, ref this._blendFunc, 2);
            MS.Win32.UnsafeNativeMethods.ReleaseDC(new HandleRef(), new HandleRef(null, handle));
            MS.Win32.UnsafeNativeMethods.ReleaseDC(new HandleRef(), new HandleRef(null, dC));
            if (!flag)
            {
                MS.Win32.UnsafeNativeMethods.HRESULT.Check(Marshal.GetHRForLastWin32Error());
            }
            return hwnd;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void DestroyResources()
        {
            EventTrace.NormalTraceEvent(EventTraceGuidId.SPLASHSCREENGUID, 13);
            if (this._dt != null)
            {
                this._dt.Stop();
                this._dt = null;
            }
            if (this._hwnd != IntPtr.Zero)
            {
                HandleRef hWnd = new HandleRef(null, this._hwnd);
                if (MS.Win32.UnsafeNativeMethods.IsWindow(hWnd))
                {
                    MS.Win32.UnsafeNativeMethods.DestroyWindow(hWnd);
                }
                this._hwnd = IntPtr.Zero;
            }
            if (!this._hBitmap.IsClosed)
            {
                this._hBitmap.Close();
            }
            if (this._wndClass != 0)
            {
                if (MS.Win32.UnsafeNativeMethods.IntUnregisterClass(new IntPtr(this._wndClass), this._hInstance) != 0)
                {
                    _defWndProc = null;
                }
                this._wndClass = 0;
            }
            if (this._resourceManager != null)
            {
                this._resourceManager.ReleaseAllResources();
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void Fadeout_Tick(object unused, EventArgs args)
        {
            DateTime utcNow = DateTime.UtcNow;
            if (utcNow >= this._fadeoutEnd)
            {
                this.DestroyResources();
            }
            else
            {
                TimeSpan span = (TimeSpan) (this._fadeoutEnd - utcNow);
                double num = span.TotalMilliseconds / this._fadeoutDuration.TotalMilliseconds;
                this._blendFunc.SourceConstantAlpha = (byte) (255.0 * num);
                MS.Win32.UnsafeNativeMethods.UpdateLayeredWindow(this._hwnd, IntPtr.Zero, null, null, IntPtr.Zero, null, 0, ref this._blendFunc, 2);
            }
        }

        private UnmanagedMemoryStream GetResourceStream()
        {
            UnmanagedMemoryStream stream = this._resourceManager.GetStream(this._resourceName, CultureInfo.CurrentUICulture);
            if (stream != null)
            {
                return stream;
            }
            string resourceIDFromRelativePath = ResourceIDHelper.GetResourceIDFromRelativePath(this._resourceName);
            return this._resourceManager.GetStream(resourceIDFromRelativePath, CultureInfo.CurrentUICulture);
        }

        [SecurityCritical]
        public unsafe void Show(bool autoClose)
        {
            EventTrace.NormalTraceEvent(EventTraceGuidId.SPLASHSCREENGUID, 10);
            if (this._hwnd == IntPtr.Zero)
            {
                using (UnmanagedMemoryStream stream = this.GetResourceStream())
                {
                    if (stream == null)
                    {
                        throw new IOException(System.Windows.SR.Get("UnableToLocateResource", new object[] { this._resourceName }));
                    }
                    stream.Seek(0L, SeekOrigin.Begin);
                    IntPtr pImgBuffer = new IntPtr((void*) stream.PositionPointer);
                    if (this.CreateLayeredWindowFromImgBuffer(pImgBuffer, stream.Length) && autoClose)
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, delegate (object splashObj) {
                            ((SplashScreen) splashObj).Close(TimeSpan.FromSeconds(0.3));
                            return null;
                        }, this);
                    }
                }
            }
        }
    }
}

