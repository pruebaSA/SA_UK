namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Printing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    [DefaultProperty("Document"), System.Windows.Forms.SRDescription("DescriptionPageSetupDialog")]
    public sealed class PageSetupDialog : CommonDialog
    {
        private bool allowMargins;
        private bool allowOrientation;
        private bool allowPaper;
        private bool allowPrinter;
        private bool enableMetric;
        private Margins minMargins;
        private System.Drawing.Printing.PageSettings pageSettings;
        private PrintDocument printDocument;
        private System.Drawing.Printing.PrinterSettings printerSettings;
        private bool showHelp;
        private bool showNetwork;

        public PageSetupDialog()
        {
            this.Reset();
        }

        private int GetFlags()
        {
            int num = 0;
            num |= 0x2000;
            if (!this.allowMargins)
            {
                num |= 0x10;
            }
            if (!this.allowOrientation)
            {
                num |= 0x100;
            }
            if (!this.allowPaper)
            {
                num |= 0x200;
            }
            if (!this.allowPrinter || (this.printerSettings == null))
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
            if (this.minMargins != null)
            {
                num |= 1;
            }
            if (this.pageSettings.Margins != null)
            {
                num |= 2;
            }
            return num;
        }

        public override void Reset()
        {
            this.allowMargins = true;
            this.allowOrientation = true;
            this.allowPaper = true;
            this.allowPrinter = true;
            this.MinMargins = null;
            this.pageSettings = null;
            this.printDocument = null;
            this.printerSettings = null;
            this.showHelp = false;
            this.showNetwork = true;
        }

        private void ResetMinMargins()
        {
            this.MinMargins = null;
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            bool flag2;
            System.Windows.Forms.IntSecurity.SafePrinting.Demand();
            NativeMethods.WndProc proc = new NativeMethods.WndProc(this.HookProc);
            if (this.pageSettings == null)
            {
                throw new ArgumentException(System.Windows.Forms.SR.GetString("PSDcantShowWithoutPage"));
            }
            NativeMethods.PAGESETUPDLG structure = new NativeMethods.PAGESETUPDLG();
            structure.lStructSize = Marshal.SizeOf(structure);
            structure.Flags = this.GetFlags();
            structure.hwndOwner = hwndOwner;
            structure.lpfnPageSetupHook = proc;
            PrinterUnit thousandthsOfAnInch = PrinterUnit.ThousandthsOfAnInch;
            if (this.EnableMetric)
            {
                StringBuilder lpLCData = new StringBuilder(2);
                if ((UnsafeNativeMethods.GetLocaleInfo(NativeMethods.LOCALE_USER_DEFAULT, 13, lpLCData, lpLCData.Capacity) > 0) && (int.Parse(lpLCData.ToString(), CultureInfo.InvariantCulture) == 0))
                {
                    thousandthsOfAnInch = PrinterUnit.HundredthsOfAMillimeter;
                }
            }
            if (this.MinMargins != null)
            {
                Margins margins = PrinterUnitConvert.Convert(this.MinMargins, PrinterUnit.Display, thousandthsOfAnInch);
                structure.minMarginLeft = margins.Left;
                structure.minMarginTop = margins.Top;
                structure.minMarginRight = margins.Right;
                structure.minMarginBottom = margins.Bottom;
            }
            if (this.pageSettings.Margins != null)
            {
                Margins margins2 = PrinterUnitConvert.Convert(this.pageSettings.Margins, PrinterUnit.Display, thousandthsOfAnInch);
                structure.marginLeft = margins2.Left;
                structure.marginTop = margins2.Top;
                structure.marginRight = margins2.Right;
                structure.marginBottom = margins2.Bottom;
            }
            structure.marginLeft = Math.Max(structure.marginLeft, structure.minMarginLeft);
            structure.marginTop = Math.Max(structure.marginTop, structure.minMarginTop);
            structure.marginRight = Math.Max(structure.marginRight, structure.minMarginRight);
            structure.marginBottom = Math.Max(structure.marginBottom, structure.minMarginBottom);
            System.Drawing.Printing.PrinterSettings settings = (this.printerSettings == null) ? this.pageSettings.PrinterSettings : this.printerSettings;
            System.Windows.Forms.IntSecurity.AllPrintingAndUnmanagedCode.Assert();
            try
            {
                structure.hDevMode = settings.GetHdevmode(this.pageSettings);
                structure.hDevNames = settings.GetHdevnames();
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            try
            {
                if (!UnsafeNativeMethods.PageSetupDlg(structure))
                {
                    return false;
                }
                UpdateSettings(structure, this.pageSettings, this.printerSettings);
                flag2 = true;
            }
            finally
            {
                UnsafeNativeMethods.GlobalFree(new HandleRef(structure, structure.hDevMode));
                UnsafeNativeMethods.GlobalFree(new HandleRef(structure, structure.hDevNames));
            }
            return flag2;
        }

        private bool ShouldSerializeMinMargins()
        {
            if (((this.minMargins.Left == 0) && (this.minMargins.Right == 0)) && (this.minMargins.Top == 0))
            {
                return (this.minMargins.Bottom != 0);
            }
            return true;
        }

        private static void UpdateSettings(NativeMethods.PAGESETUPDLG data, System.Drawing.Printing.PageSettings pageSettings, System.Drawing.Printing.PrinterSettings printerSettings)
        {
            System.Windows.Forms.IntSecurity.AllPrintingAndUnmanagedCode.Assert();
            try
            {
                pageSettings.SetHdevmode(data.hDevMode);
                if (printerSettings != null)
                {
                    printerSettings.SetHdevmode(data.hDevMode);
                    printerSettings.SetHdevnames(data.hDevNames);
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            Margins margins = new Margins {
                Left = data.marginLeft,
                Top = data.marginTop,
                Right = data.marginRight,
                Bottom = data.marginBottom
            };
            PrinterUnit fromUnit = ((data.Flags & 8) != 0) ? PrinterUnit.HundredthsOfAMillimeter : PrinterUnit.ThousandthsOfAnInch;
            pageSettings.Margins = PrinterUnitConvert.Convert(margins, fromUnit, PrinterUnit.Display);
        }

        [DefaultValue(true), System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("PSDallowMarginsDescr")]
        public bool AllowMargins
        {
            get => 
                this.allowMargins;
            set
            {
                this.allowMargins = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("PSDallowOrientationDescr"), DefaultValue(true)]
        public bool AllowOrientation
        {
            get => 
                this.allowOrientation;
            set
            {
                this.allowOrientation = value;
            }
        }

        [System.Windows.Forms.SRDescription("PSDallowPaperDescr"), DefaultValue(true), System.Windows.Forms.SRCategory("CatBehavior")]
        public bool AllowPaper
        {
            get => 
                this.allowPaper;
            set
            {
                this.allowPaper = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("PSDallowPrinterDescr"), DefaultValue(true)]
        public bool AllowPrinter
        {
            get => 
                this.allowPrinter;
            set
            {
                this.allowPrinter = value;
            }
        }

        [DefaultValue((string) null), System.Windows.Forms.SRDescription("PDdocumentDescr"), System.Windows.Forms.SRCategory("CatData")]
        public PrintDocument Document
        {
            get => 
                this.printDocument;
            set
            {
                this.printDocument = value;
                if (this.printDocument != null)
                {
                    this.pageSettings = this.printDocument.DefaultPageSettings;
                    this.printerSettings = this.printDocument.PrinterSettings;
                }
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), System.Windows.Forms.SRDescription("PSDenableMetricDescr"), DefaultValue(false)]
        public bool EnableMetric
        {
            get => 
                this.enableMetric;
            set
            {
                this.enableMetric = value;
            }
        }

        [System.Windows.Forms.SRDescription("PSDminMarginsDescr"), System.Windows.Forms.SRCategory("CatData")]
        public Margins MinMargins
        {
            get => 
                this.minMargins;
            set
            {
                if (value == null)
                {
                    value = new Margins(0, 0, 0, 0);
                }
                this.minMargins = value;
            }
        }

        [Browsable(false), System.Windows.Forms.SRDescription("PSDpageSettingsDescr"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Windows.Forms.SRCategory("CatData")]
        public System.Drawing.Printing.PageSettings PageSettings
        {
            get => 
                this.pageSettings;
            set
            {
                this.pageSettings = value;
                this.printDocument = null;
            }
        }

        [Browsable(false), System.Windows.Forms.SRDescription("PSDprinterSettingsDescr"), System.Windows.Forms.SRCategory("CatData"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Printing.PrinterSettings PrinterSettings
        {
            get => 
                this.printerSettings;
            set
            {
                this.printerSettings = value;
                this.printDocument = null;
            }
        }

        [System.Windows.Forms.SRDescription("PSDshowHelpDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false)]
        public bool ShowHelp
        {
            get => 
                this.showHelp;
            set
            {
                this.showHelp = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("PSDshowNetworkDescr"), DefaultValue(true)]
        public bool ShowNetwork
        {
            get => 
                this.showNetwork;
            set
            {
                this.showNetwork = value;
            }
        }
    }
}

