﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Printing;
    using System.Runtime.InteropServices;
    using System.Security;

    [System.Windows.Forms.SRDescription("DescriptionPrintDialog"), DefaultProperty("Document"), Designer("System.Windows.Forms.Design.PrintDialogDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public sealed class PrintDialog : CommonDialog
    {
        private bool allowCurrentPage;
        private bool allowPages;
        private bool allowPrintToFile;
        private bool allowSelection;
        private PrintDocument printDocument;
        private const int printRangeMask = 0x400003;
        private bool printToFile;
        private System.Drawing.Printing.PrinterSettings settings;
        private bool showHelp;
        private bool showNetwork;
        private bool useEXDialog;

        public PrintDialog()
        {
            this.Reset();
        }

        internal static NativeMethods.PRINTDLG CreatePRINTDLG()
        {
            NativeMethods.PRINTDLG structure = new NativeMethods.PRINTDLG();
            structure.lStructSize = Marshal.SizeOf(structure);
            structure.hwndOwner = IntPtr.Zero;
            structure.hDevMode = IntPtr.Zero;
            structure.hDevNames = IntPtr.Zero;
            structure.Flags = 0;
            structure.hDC = IntPtr.Zero;
            structure.nFromPage = 1;
            structure.nToPage = 1;
            structure.nMinPage = 0;
            structure.nMaxPage = 0x270f;
            structure.nCopies = 1;
            structure.hInstance = IntPtr.Zero;
            structure.lCustData = IntPtr.Zero;
            structure.lpfnPrintHook = null;
            structure.lpfnSetupHook = null;
            structure.lpPrintTemplateName = null;
            structure.lpSetupTemplateName = null;
            structure.hPrintTemplate = IntPtr.Zero;
            structure.hSetupTemplate = IntPtr.Zero;
            return structure;
        }

        internal static NativeMethods.PRINTDLGEX CreatePRINTDLGEX()
        {
            NativeMethods.PRINTDLGEX structure = new NativeMethods.PRINTDLGEX();
            structure.lStructSize = Marshal.SizeOf(structure);
            structure.hwndOwner = IntPtr.Zero;
            structure.hDevMode = IntPtr.Zero;
            structure.hDevNames = IntPtr.Zero;
            structure.hDC = IntPtr.Zero;
            structure.Flags = 0;
            structure.Flags2 = 0;
            structure.ExclusionFlags = 0;
            structure.nPageRanges = 0;
            structure.nMaxPageRanges = 1;
            structure.pageRanges = UnsafeNativeMethods.GlobalAlloc(0x40, structure.nMaxPageRanges * Marshal.SizeOf(typeof(NativeMethods.PRINTPAGERANGE)));
            structure.nMinPage = 0;
            structure.nMaxPage = 0x270f;
            structure.nCopies = 1;
            structure.hInstance = IntPtr.Zero;
            structure.lpPrintTemplateName = null;
            structure.nPropertyPages = 0;
            structure.lphPropertyPages = IntPtr.Zero;
            structure.nStartPage = NativeMethods.START_PAGE_GENERAL;
            structure.dwResultAction = 0;
            return structure;
        }

        private int GetFlags()
        {
            int num = 0;
            if ((!this.UseEXDialog || (Environment.OSVersion.Platform != PlatformID.Win32NT)) || (Environment.OSVersion.Version.Major < 5))
            {
                num |= 0x1000;
            }
            if (!this.allowCurrentPage)
            {
                num |= 0x800000;
            }
            if (!this.allowPages)
            {
                num |= 8;
            }
            if (!this.allowPrintToFile)
            {
                num |= 0x80000;
            }
            if (!this.allowSelection)
            {
                num |= 4;
            }
            num |= this.PrinterSettings.PrintRange;
            if (this.printToFile)
            {
                num |= 0x20;
            }
            if (this.showHelp)
            {
                num |= 0x800;
            }
            if (!this.showNetwork)
            {
                num |= 0x200000;
            }
            if (this.PrinterSettings.Collate)
            {
                num |= 0x10;
            }
            return num;
        }

        public override void Reset()
        {
            this.allowCurrentPage = false;
            this.allowPages = false;
            this.allowPrintToFile = true;
            this.allowSelection = false;
            this.printDocument = null;
            this.printToFile = false;
            this.settings = null;
            this.showHelp = false;
            this.showNetwork = true;
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            System.Windows.Forms.IntSecurity.SafePrinting.Demand();
            NativeMethods.WndProc hookProcPtr = new NativeMethods.WndProc(this.HookProc);
            if ((!this.UseEXDialog || (Environment.OSVersion.Platform != PlatformID.Win32NT)) || (Environment.OSVersion.Version.Major < 5))
            {
                NativeMethods.PRINTDLG printdlg = CreatePRINTDLG();
                return this.ShowPrintDialog(hwndOwner, hookProcPtr, printdlg);
            }
            NativeMethods.PRINTDLGEX data = CreatePRINTDLGEX();
            return this.ShowPrintDialog(hwndOwner, data);
        }

        private unsafe bool ShowPrintDialog(IntPtr hwndOwner, NativeMethods.PRINTDLGEX data)
        {
            bool flag;
            data.Flags = this.GetFlags();
            data.nCopies = this.PrinterSettings.Copies;
            data.hwndOwner = hwndOwner;
            System.Windows.Forms.IntSecurity.AllPrintingAndUnmanagedCode.Assert();
            try
            {
                if (this.PageSettings == null)
                {
                    data.hDevMode = this.PrinterSettings.GetHdevmode();
                }
                else
                {
                    data.hDevMode = this.PrinterSettings.GetHdevmode(this.PageSettings);
                }
                data.hDevNames = this.PrinterSettings.GetHdevnames();
            }
            catch (InvalidPrinterException)
            {
                data.hDevMode = IntPtr.Zero;
                data.hDevNames = IntPtr.Zero;
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            try
            {
                if (this.AllowSomePages)
                {
                    if ((this.PrinterSettings.FromPage < this.PrinterSettings.MinimumPage) || (this.PrinterSettings.FromPage > this.PrinterSettings.MaximumPage))
                    {
                        throw new ArgumentException(System.Windows.Forms.SR.GetString("PDpageOutOfRange", new object[] { "FromPage" }));
                    }
                    if ((this.PrinterSettings.ToPage < this.PrinterSettings.MinimumPage) || (this.PrinterSettings.ToPage > this.PrinterSettings.MaximumPage))
                    {
                        throw new ArgumentException(System.Windows.Forms.SR.GetString("PDpageOutOfRange", new object[] { "ToPage" }));
                    }
                    if (this.PrinterSettings.ToPage < this.PrinterSettings.FromPage)
                    {
                        throw new ArgumentException(System.Windows.Forms.SR.GetString("PDpageOutOfRange", new object[] { "FromPage" }));
                    }
                    int* pageRanges = (int*) data.pageRanges;
                    pageRanges[0] = this.PrinterSettings.FromPage;
                    pageRanges++;
                    pageRanges[0] = this.PrinterSettings.ToPage;
                    data.nPageRanges = 1;
                    data.nMinPage = this.PrinterSettings.MinimumPage;
                    data.nMaxPage = this.PrinterSettings.MaximumPage;
                }
                data.Flags &= -2099201;
                if (NativeMethods.Failed(UnsafeNativeMethods.PrintDlgEx(data)) || (data.dwResultAction == 0))
                {
                    return false;
                }
                System.Windows.Forms.IntSecurity.AllPrintingAndUnmanagedCode.Assert();
                try
                {
                    UpdatePrinterSettings(data.hDevMode, data.hDevNames, (short) data.nCopies, data.Flags, this.PrinterSettings, this.PageSettings);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                this.PrintToFile = (data.Flags & 0x20) != 0;
                this.PrinterSettings.PrintToFile = this.PrintToFile;
                if (this.AllowSomePages)
                {
                    int* numPtr2 = (int*) data.pageRanges;
                    this.PrinterSettings.FromPage = numPtr2[0];
                    numPtr2++;
                    this.PrinterSettings.ToPage = numPtr2[0];
                }
                flag = data.dwResultAction == 1;
            }
            finally
            {
                if (data.hDevMode != IntPtr.Zero)
                {
                    UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevMode));
                }
                if (data.hDevNames != IntPtr.Zero)
                {
                    UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevNames));
                }
                if (data.pageRanges != IntPtr.Zero)
                {
                    UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.pageRanges));
                }
            }
            return flag;
        }

        private bool ShowPrintDialog(IntPtr hwndOwner, NativeMethods.WndProc hookProcPtr, NativeMethods.PRINTDLG data)
        {
            bool flag;
            data.Flags = this.GetFlags();
            data.nCopies = this.PrinterSettings.Copies;
            data.hwndOwner = hwndOwner;
            data.lpfnPrintHook = hookProcPtr;
            System.Windows.Forms.IntSecurity.AllPrintingAndUnmanagedCode.Assert();
            try
            {
                if (this.PageSettings == null)
                {
                    data.hDevMode = this.PrinterSettings.GetHdevmode();
                }
                else
                {
                    data.hDevMode = this.PrinterSettings.GetHdevmode(this.PageSettings);
                }
                data.hDevNames = this.PrinterSettings.GetHdevnames();
            }
            catch (InvalidPrinterException)
            {
                data.hDevMode = IntPtr.Zero;
                data.hDevNames = IntPtr.Zero;
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            try
            {
                if (this.AllowSomePages)
                {
                    if ((this.PrinterSettings.FromPage < this.PrinterSettings.MinimumPage) || (this.PrinterSettings.FromPage > this.PrinterSettings.MaximumPage))
                    {
                        throw new ArgumentException(System.Windows.Forms.SR.GetString("PDpageOutOfRange", new object[] { "FromPage" }));
                    }
                    if ((this.PrinterSettings.ToPage < this.PrinterSettings.MinimumPage) || (this.PrinterSettings.ToPage > this.PrinterSettings.MaximumPage))
                    {
                        throw new ArgumentException(System.Windows.Forms.SR.GetString("PDpageOutOfRange", new object[] { "ToPage" }));
                    }
                    if (this.PrinterSettings.ToPage < this.PrinterSettings.FromPage)
                    {
                        throw new ArgumentException(System.Windows.Forms.SR.GetString("PDpageOutOfRange", new object[] { "FromPage" }));
                    }
                    data.nFromPage = (short) this.PrinterSettings.FromPage;
                    data.nToPage = (short) this.PrinterSettings.ToPage;
                    data.nMinPage = (short) this.PrinterSettings.MinimumPage;
                    data.nMaxPage = (short) this.PrinterSettings.MaximumPage;
                }
                if (!UnsafeNativeMethods.PrintDlg(data))
                {
                    return false;
                }
                System.Windows.Forms.IntSecurity.AllPrintingAndUnmanagedCode.Assert();
                try
                {
                    UpdatePrinterSettings(data.hDevMode, data.hDevNames, data.nCopies, data.Flags, this.settings, this.PageSettings);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                this.PrintToFile = (data.Flags & 0x20) != 0;
                this.PrinterSettings.PrintToFile = this.PrintToFile;
                if (this.AllowSomePages)
                {
                    this.PrinterSettings.FromPage = data.nFromPage;
                    this.PrinterSettings.ToPage = data.nToPage;
                }
                flag = true;
            }
            finally
            {
                UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevMode));
                UnsafeNativeMethods.GlobalFree(new HandleRef(data, data.hDevNames));
            }
            return flag;
        }

        private static void UpdatePrinterSettings(IntPtr hDevMode, IntPtr hDevNames, short copies, int flags, System.Drawing.Printing.PrinterSettings settings, System.Drawing.Printing.PageSettings pageSettings)
        {
            settings.SetHdevmode(hDevMode);
            settings.SetHdevnames(hDevNames);
            if (pageSettings != null)
            {
                pageSettings.SetHdevmode(hDevMode);
            }
            if (settings.Copies == 1)
            {
                settings.Copies = copies;
            }
            settings.PrintRange = ((PrintRange) flags) & (PrintRange.CurrentPage | PrintRange.SomePages | PrintRange.Selection);
        }

        [DefaultValue(false), System.Windows.Forms.SRDescription("PDallowCurrentPageDescr")]
        public bool AllowCurrentPage
        {
            get => 
                this.allowCurrentPage;
            set
            {
                this.allowCurrentPage = value;
            }
        }

        [System.Windows.Forms.SRDescription("PDallowPrintToFileDescr"), DefaultValue(true), System.Windows.Forms.SRCategory("CatBehavior")]
        public bool AllowPrintToFile
        {
            get => 
                this.allowPrintToFile;
            set
            {
                this.allowPrintToFile = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("PDallowSelectionDescr"), DefaultValue(false)]
        public bool AllowSelection
        {
            get => 
                this.allowSelection;
            set
            {
                this.allowSelection = value;
            }
        }

        [System.Windows.Forms.SRDescription("PDallowPagesDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false)]
        public bool AllowSomePages
        {
            get => 
                this.allowPages;
            set
            {
                this.allowPages = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), DefaultValue((string) null), System.Windows.Forms.SRDescription("PDdocumentDescr")]
        public PrintDocument Document
        {
            get => 
                this.printDocument;
            set
            {
                this.printDocument = value;
                if (this.printDocument == null)
                {
                    this.settings = new System.Drawing.Printing.PrinterSettings();
                }
                else
                {
                    this.settings = this.printDocument.PrinterSettings;
                }
            }
        }

        private System.Drawing.Printing.PageSettings PageSettings =>
            this.Document?.DefaultPageSettings;

        [Browsable(false), System.Windows.Forms.SRDescription("PDprinterSettingsDescr"), DefaultValue((string) null), System.Windows.Forms.SRCategory("CatData"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Printing.PrinterSettings PrinterSettings
        {
            get
            {
                if (this.settings == null)
                {
                    this.settings = new System.Drawing.Printing.PrinterSettings();
                }
                return this.settings;
            }
            set
            {
                if (value != this.PrinterSettings)
                {
                    this.settings = value;
                    this.printDocument = null;
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false), System.Windows.Forms.SRDescription("PDprintToFileDescr")]
        public bool PrintToFile
        {
            get => 
                this.printToFile;
            set
            {
                this.printToFile = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false), System.Windows.Forms.SRDescription("PDshowHelpDescr")]
        public bool ShowHelp
        {
            get => 
                this.showHelp;
            set
            {
                this.showHelp = value;
            }
        }

        [System.Windows.Forms.SRDescription("PDshowNetworkDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(true)]
        public bool ShowNetwork
        {
            get => 
                this.showNetwork;
            set
            {
                this.showNetwork = value;
            }
        }

        [DefaultValue(false), System.Windows.Forms.SRDescription("PDuseEXDialog")]
        public bool UseEXDialog
        {
            get => 
                this.useEXDialog;
            set
            {
                this.useEXDialog = value;
            }
        }
    }
}

