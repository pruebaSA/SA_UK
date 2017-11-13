namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;

    [Designer("System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultEvent("HelpRequest"), System.Windows.Forms.SRDescription("DescriptionFolderBrowserDialog"), DefaultProperty("SelectedPath")]
    public sealed class FolderBrowserDialog : CommonDialog
    {
        private System.Windows.Forms.UnsafeNativeMethods.BrowseCallbackProc callback;
        private string descriptionText;
        private Environment.SpecialFolder rootFolder;
        private string selectedPath;
        private bool selectedPathNeedsCheck;
        private bool showNewFolderButton;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler HelpRequest
        {
            add
            {
                base.HelpRequest += value;
            }
            remove
            {
                base.HelpRequest -= value;
            }
        }

        public FolderBrowserDialog()
        {
            this.Reset();
        }

        private int FolderBrowserDialog_BrowseCallbackProc(IntPtr hwnd, int msg, IntPtr lParam, IntPtr lpData)
        {
            switch (msg)
            {
                case 1:
                    if (this.selectedPath.Length != 0)
                    {
                        System.Windows.Forms.UnsafeNativeMethods.SendMessage(new HandleRef(null, hwnd), NativeMethods.BFFM_SETSELECTION, 1, this.selectedPath);
                    }
                    break;

                case 2:
                {
                    IntPtr pidl = lParam;
                    if (pidl != IntPtr.Zero)
                    {
                        IntPtr pszPath = Marshal.AllocHGlobal((int) (260 * Marshal.SystemDefaultCharSize));
                        bool flag = System.Windows.Forms.UnsafeNativeMethods.Shell32.SHGetPathFromIDList(pidl, pszPath);
                        Marshal.FreeHGlobal(pszPath);
                        System.Windows.Forms.UnsafeNativeMethods.SendMessage(new HandleRef(null, hwnd), 0x465, 0, flag ? 1 : 0);
                    }
                    break;
                }
            }
            return 0;
        }

        private static System.Windows.Forms.UnsafeNativeMethods.IMalloc GetSHMalloc()
        {
            System.Windows.Forms.UnsafeNativeMethods.IMalloc[] ppMalloc = new System.Windows.Forms.UnsafeNativeMethods.IMalloc[1];
            System.Windows.Forms.UnsafeNativeMethods.Shell32.SHGetMalloc(ppMalloc);
            return ppMalloc[0];
        }

        public override void Reset()
        {
            this.rootFolder = Environment.SpecialFolder.Desktop;
            this.descriptionText = string.Empty;
            this.selectedPath = string.Empty;
            this.selectedPathNeedsCheck = false;
            this.showNewFolderButton = true;
        }

        protected override bool RunDialog(IntPtr hWndOwner)
        {
            IntPtr zero = IntPtr.Zero;
            bool flag = false;
            System.Windows.Forms.UnsafeNativeMethods.Shell32.SHGetSpecialFolderLocation(hWndOwner, (int) this.rootFolder, ref zero);
            if (zero == IntPtr.Zero)
            {
                System.Windows.Forms.UnsafeNativeMethods.Shell32.SHGetSpecialFolderLocation(hWndOwner, 0, ref zero);
                if (zero == IntPtr.Zero)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("FolderBrowserDialogNoRootFolder"));
                }
            }
            int num = 0x40;
            if (!this.showNewFolderButton)
            {
                num += 0x200;
            }
            if (Control.CheckForIllegalCrossThreadCalls && (Application.OleRequired() != ApartmentState.STA))
            {
                throw new ThreadStateException(System.Windows.Forms.SR.GetString("DebuggingExceptionOnly", new object[] { System.Windows.Forms.SR.GetString("ThreadMustBeSTA") }));
            }
            IntPtr pidl = IntPtr.Zero;
            IntPtr hglobal = IntPtr.Zero;
            IntPtr pszPath = IntPtr.Zero;
            try
            {
                System.Windows.Forms.UnsafeNativeMethods.BROWSEINFO lpbi = new System.Windows.Forms.UnsafeNativeMethods.BROWSEINFO();
                hglobal = Marshal.AllocHGlobal((int) (260 * Marshal.SystemDefaultCharSize));
                pszPath = Marshal.AllocHGlobal((int) (260 * Marshal.SystemDefaultCharSize));
                this.callback = new System.Windows.Forms.UnsafeNativeMethods.BrowseCallbackProc(this.FolderBrowserDialog_BrowseCallbackProc);
                lpbi.pidlRoot = zero;
                lpbi.hwndOwner = hWndOwner;
                lpbi.pszDisplayName = hglobal;
                lpbi.lpszTitle = this.descriptionText;
                lpbi.ulFlags = num;
                lpbi.lpfn = this.callback;
                lpbi.lParam = IntPtr.Zero;
                lpbi.iImage = 0;
                pidl = System.Windows.Forms.UnsafeNativeMethods.Shell32.SHBrowseForFolder(lpbi);
                if (pidl != IntPtr.Zero)
                {
                    System.Windows.Forms.UnsafeNativeMethods.Shell32.SHGetPathFromIDList(pidl, pszPath);
                    this.selectedPathNeedsCheck = true;
                    this.selectedPath = Marshal.PtrToStringAuto(pszPath);
                    flag = true;
                }
            }
            finally
            {
                System.Windows.Forms.UnsafeNativeMethods.IMalloc sHMalloc = GetSHMalloc();
                sHMalloc.Free(zero);
                if (pidl != IntPtr.Zero)
                {
                    sHMalloc.Free(pidl);
                }
                if (pszPath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pszPath);
                }
                if (hglobal != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(hglobal);
                }
                this.callback = null;
            }
            return flag;
        }

        [System.Windows.Forms.SRDescription("FolderBrowserDialogDescription"), DefaultValue(""), System.Windows.Forms.SRCategory("CatFolderBrowsing"), Browsable(true), Localizable(true)]
        public string Description
        {
            get => 
                this.descriptionText;
            set
            {
                this.descriptionText = (value == null) ? string.Empty : value;
            }
        }

        [Browsable(true), System.Windows.Forms.SRCategory("CatFolderBrowsing"), System.Windows.Forms.SRDescription("FolderBrowserDialogRootFolder"), TypeConverter(typeof(SpecialFolderEnumConverter)), DefaultValue(0), Localizable(false)]
        public Environment.SpecialFolder RootFolder
        {
            get => 
                this.rootFolder;
            set
            {
                if (!Enum.IsDefined(typeof(Environment.SpecialFolder), value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(Environment.SpecialFolder));
                }
                this.rootFolder = value;
            }
        }

        [Editor("System.Windows.Forms.Design.SelectedPathEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), System.Windows.Forms.SRDescription("FolderBrowserDialogSelectedPath"), DefaultValue(""), System.Windows.Forms.SRCategory("CatFolderBrowsing"), Browsable(true), Localizable(true)]
        public string SelectedPath
        {
            get
            {
                if (((this.selectedPath != null) && (this.selectedPath.Length != 0)) && this.selectedPathNeedsCheck)
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, this.selectedPath).Demand();
                }
                return this.selectedPath;
            }
            set
            {
                this.selectedPath = (value == null) ? string.Empty : value;
                this.selectedPathNeedsCheck = false;
            }
        }

        [DefaultValue(true), Localizable(false), Browsable(true), System.Windows.Forms.SRCategory("CatFolderBrowsing"), System.Windows.Forms.SRDescription("FolderBrowserDialogShowNewFolderButton")]
        public bool ShowNewFolderButton
        {
            get => 
                this.showNewFolderButton;
            set
            {
                this.showNewFolderButton = value;
            }
        }
    }
}

