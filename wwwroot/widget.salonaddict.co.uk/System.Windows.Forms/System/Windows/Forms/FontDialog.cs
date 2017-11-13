namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [System.Windows.Forms.SRDescription("DescriptionFontDialog"), DefaultEvent("Apply"), DefaultProperty("Font")]
    public class FontDialog : CommonDialog
    {
        private System.Drawing.Color color;
        private const int defaultMaxSize = 0;
        private const int defaultMinSize = 0;
        protected static readonly object EventApply = new object();
        private System.Drawing.Font font;
        private int maxSize;
        private int minSize;
        private int options;
        private bool showColor;

        [System.Windows.Forms.SRDescription("FnDapplyDescr")]
        public event EventHandler Apply
        {
            add
            {
                base.Events.AddHandler(EventApply, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventApply, value);
            }
        }

        public FontDialog()
        {
            this.Reset();
        }

        internal bool GetOption(int option) => 
            ((this.options & option) != 0);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            switch (msg)
            {
                case 0x110:
                    if (!this.showColor)
                    {
                        IntPtr dlgItem = System.Windows.Forms.UnsafeNativeMethods.GetDlgItem(new HandleRef(null, hWnd), 0x473);
                        System.Windows.Forms.SafeNativeMethods.ShowWindow(new HandleRef(null, dlgItem), 0);
                        dlgItem = System.Windows.Forms.UnsafeNativeMethods.GetDlgItem(new HandleRef(null, hWnd), 0x443);
                        System.Windows.Forms.SafeNativeMethods.ShowWindow(new HandleRef(null, dlgItem), 0);
                    }
                    break;

                case 0x111:
                    if (((int) wparam) == 0x402)
                    {
                        System.Windows.Forms.NativeMethods.LOGFONT lParam = new System.Windows.Forms.NativeMethods.LOGFONT();
                        System.Windows.Forms.UnsafeNativeMethods.SendMessage(new HandleRef(null, hWnd), 0x401, 0, lParam);
                        this.UpdateFont(lParam);
                        int num = (int) System.Windows.Forms.UnsafeNativeMethods.SendDlgItemMessage(new HandleRef(null, hWnd), 0x473, 0x147, IntPtr.Zero, IntPtr.Zero);
                        if (num != -1)
                        {
                            this.UpdateColor((int) System.Windows.Forms.UnsafeNativeMethods.SendDlgItemMessage(new HandleRef(null, hWnd), 0x473, 0x150, (IntPtr) num, IntPtr.Zero));
                        }
                        if (NativeWindow.WndProcShouldBeDebuggable)
                        {
                            this.OnApply(EventArgs.Empty);
                        }
                        else
                        {
                            try
                            {
                                this.OnApply(EventArgs.Empty);
                            }
                            catch (Exception exception)
                            {
                                Application.OnThreadException(exception);
                            }
                        }
                    }
                    break;
            }
            return base.HookProc(hWnd, msg, wparam, lparam);
        }

        protected virtual void OnApply(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventApply];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public override void Reset()
        {
            this.options = 0x101;
            this.font = null;
            this.color = System.Drawing.Color.Black;
            this.showColor = false;
            this.minSize = 0;
            this.maxSize = 0;
            this.SetOption(0x40000, true);
        }

        private void ResetFont()
        {
            this.font = null;
        }

        protected override bool RunDialog(IntPtr hWndOwner)
        {
            bool flag;
            System.Windows.Forms.NativeMethods.WndProc proc = new System.Windows.Forms.NativeMethods.WndProc(this.HookProc);
            System.Windows.Forms.NativeMethods.CHOOSEFONT cf = new System.Windows.Forms.NativeMethods.CHOOSEFONT();
            IntPtr dC = System.Windows.Forms.UnsafeNativeMethods.GetDC(System.Windows.Forms.NativeMethods.NullHandleRef);
            System.Windows.Forms.NativeMethods.LOGFONT logFont = new System.Windows.Forms.NativeMethods.LOGFONT();
            Graphics graphics = Graphics.FromHdcInternal(dC);
            System.Windows.Forms.IntSecurity.ObjectFromWin32Handle.Assert();
            try
            {
                this.Font.ToLogFont(logFont, graphics);
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
                graphics.Dispose();
            }
            System.Windows.Forms.UnsafeNativeMethods.ReleaseDC(System.Windows.Forms.NativeMethods.NullHandleRef, new HandleRef(null, dC));
            IntPtr zero = IntPtr.Zero;
            try
            {
                zero = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(System.Windows.Forms.NativeMethods.LOGFONT)));
                Marshal.StructureToPtr(logFont, zero, false);
                cf.lStructSize = Marshal.SizeOf(typeof(System.Windows.Forms.NativeMethods.CHOOSEFONT));
                cf.hwndOwner = hWndOwner;
                cf.hDC = IntPtr.Zero;
                cf.lpLogFont = zero;
                cf.Flags = (this.Options | 0x40) | 8;
                if ((this.minSize > 0) || (this.maxSize > 0))
                {
                    cf.Flags |= 0x2000;
                }
                if (this.ShowColor || this.ShowEffects)
                {
                    cf.rgbColors = ColorTranslator.ToWin32(this.color);
                }
                else
                {
                    cf.rgbColors = ColorTranslator.ToWin32(System.Drawing.Color.Black);
                }
                cf.lpfnHook = proc;
                cf.hInstance = System.Windows.Forms.UnsafeNativeMethods.GetModuleHandle(null);
                cf.nSizeMin = this.minSize;
                if (this.maxSize == 0)
                {
                    cf.nSizeMax = 0x7fffffff;
                }
                else
                {
                    cf.nSizeMax = this.maxSize;
                }
                if (!System.Windows.Forms.SafeNativeMethods.ChooseFont(cf))
                {
                    return false;
                }
                System.Windows.Forms.NativeMethods.LOGFONT logfont2 = null;
                logfont2 = (System.Windows.Forms.NativeMethods.LOGFONT) System.Windows.Forms.UnsafeNativeMethods.PtrToStructure(zero, typeof(System.Windows.Forms.NativeMethods.LOGFONT));
                if ((logfont2.lfFaceName != null) && (logfont2.lfFaceName.Length > 0))
                {
                    logFont = logfont2;
                    this.UpdateFont(logFont);
                    this.UpdateColor(cf.rgbColors);
                }
                flag = true;
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(zero);
                }
            }
            return flag;
        }

        internal void SetOption(int option, bool value)
        {
            if (value)
            {
                this.options |= option;
            }
            else
            {
                this.options &= ~option;
            }
        }

        private bool ShouldSerializeFont() => 
            !this.Font.Equals(Control.DefaultFont);

        public override string ToString() => 
            (base.ToString() + ",  Font: " + this.Font.ToString());

        private void UpdateColor(int rgb)
        {
            if (ColorTranslator.ToWin32(this.color) != rgb)
            {
                this.color = ColorTranslator.FromOle(rgb);
            }
        }

        private void UpdateFont(System.Windows.Forms.NativeMethods.LOGFONT lf)
        {
            IntPtr dC = System.Windows.Forms.UnsafeNativeMethods.GetDC(System.Windows.Forms.NativeMethods.NullHandleRef);
            try
            {
                using (System.Drawing.Font font = null)
                {
                    System.Windows.Forms.IntSecurity.UnmanagedCode.Assert();
                    try
                    {
                        font = System.Drawing.Font.FromLogFont(lf, dC);
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }
                    this.font = ControlPaint.FontInPoints(font);
                }
            }
            finally
            {
                System.Windows.Forms.UnsafeNativeMethods.ReleaseDC(System.Windows.Forms.NativeMethods.NullHandleRef, new HandleRef(null, dC));
            }
        }

        [DefaultValue(true), System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("FnDallowScriptChangeDescr")]
        public bool AllowScriptChange
        {
            get => 
                !this.GetOption(0x400000);
            set
            {
                this.SetOption(0x400000, !value);
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("FnDallowSimulationsDescr"), DefaultValue(true)]
        public bool AllowSimulations
        {
            get => 
                !this.GetOption(0x1000);
            set
            {
                this.SetOption(0x1000, !value);
            }
        }

        [DefaultValue(true), System.Windows.Forms.SRDescription("FnDallowVectorFontsDescr"), System.Windows.Forms.SRCategory("CatBehavior")]
        public bool AllowVectorFonts
        {
            get => 
                !this.GetOption(0x800);
            set
            {
                this.SetOption(0x800, !value);
            }
        }

        [System.Windows.Forms.SRDescription("FnDallowVerticalFontsDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(true)]
        public bool AllowVerticalFonts
        {
            get => 
                !this.GetOption(0x1000000);
            set
            {
                this.SetOption(0x1000000, !value);
            }
        }

        [DefaultValue(typeof(System.Drawing.Color), "Black"), System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("FnDcolorDescr")]
        public System.Drawing.Color Color
        {
            get => 
                this.color;
            set
            {
                if (!value.IsEmpty)
                {
                    this.color = value;
                }
                else
                {
                    this.color = System.Drawing.Color.Black;
                }
            }
        }

        [DefaultValue(false), System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("FnDfixedPitchOnlyDescr")]
        public bool FixedPitchOnly
        {
            get => 
                this.GetOption(0x4000);
            set
            {
                this.SetOption(0x4000, value);
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("FnDfontDescr")]
        public System.Drawing.Font Font
        {
            get
            {
                System.Drawing.Font defaultFont = this.font;
                if (defaultFont == null)
                {
                    defaultFont = Control.DefaultFont;
                }
                float sizeInPoints = defaultFont.SizeInPoints;
                if ((this.minSize != 0) && (sizeInPoints < this.MinSize))
                {
                    defaultFont = new System.Drawing.Font(defaultFont.FontFamily, (float) this.MinSize, defaultFont.Style, GraphicsUnit.Point);
                }
                if ((this.maxSize != 0) && (sizeInPoints > this.MaxSize))
                {
                    defaultFont = new System.Drawing.Font(defaultFont.FontFamily, (float) this.MaxSize, defaultFont.Style, GraphicsUnit.Point);
                }
                return defaultFont;
            }
            set
            {
                this.font = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false), System.Windows.Forms.SRDescription("FnDfontMustExistDescr")]
        public bool FontMustExist
        {
            get => 
                this.GetOption(0x10000);
            set
            {
                this.SetOption(0x10000, value);
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("FnDmaxSizeDescr"), DefaultValue(0)]
        public int MaxSize
        {
            get => 
                this.maxSize;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.maxSize = value;
                if ((this.maxSize > 0) && (this.maxSize < this.minSize))
                {
                    this.minSize = this.maxSize;
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), DefaultValue(0), System.Windows.Forms.SRDescription("FnDminSizeDescr")]
        public int MinSize
        {
            get => 
                this.minSize;
            set
            {
                if (value < 0)
                {
                    value = 0;
                }
                this.minSize = value;
                if ((this.maxSize > 0) && (this.maxSize < this.minSize))
                {
                    this.maxSize = this.minSize;
                }
            }
        }

        protected int Options =>
            this.options;

        [System.Windows.Forms.SRDescription("FnDscriptsOnlyDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false)]
        public bool ScriptsOnly
        {
            get => 
                this.GetOption(0x400);
            set
            {
                this.SetOption(0x400, value);
            }
        }

        [System.Windows.Forms.SRDescription("FnDshowApplyDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false)]
        public bool ShowApply
        {
            get => 
                this.GetOption(0x200);
            set
            {
                this.SetOption(0x200, value);
            }
        }

        [System.Windows.Forms.SRDescription("FnDshowColorDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false)]
        public bool ShowColor
        {
            get => 
                this.showColor;
            set
            {
                this.showColor = value;
            }
        }

        [System.Windows.Forms.SRDescription("FnDshowEffectsDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(true)]
        public bool ShowEffects
        {
            get => 
                this.GetOption(0x100);
            set
            {
                this.SetOption(0x100, value);
            }
        }

        [System.Windows.Forms.SRDescription("FnDshowHelpDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false)]
        public bool ShowHelp
        {
            get => 
                this.GetOption(4);
            set
            {
                this.SetOption(4, value);
            }
        }
    }
}

