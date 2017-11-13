﻿namespace System.Drawing.Printing
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;

    [DefaultEvent("PrintPage"), ToolboxItemFilter("System.Drawing.Printing"), DefaultProperty("DocumentName"), System.Drawing.SRDescription("PrintDocumentDesc")]
    public class PrintDocument : Component
    {
        private PageSettings defaultPageSettings;
        private string documentName = "document";
        private bool originAtMargins;
        private System.Drawing.Printing.PrintController printController;
        private System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
        private bool userSetPageSettings;

        [System.Drawing.SRDescription("PDOCbeginPrintDescr")]
        public event PrintEventHandler BeginPrint;

        [System.Drawing.SRDescription("PDOCendPrintDescr")]
        public event PrintEventHandler EndPrint;

        [System.Drawing.SRDescription("PDOCprintPageDescr")]
        public event PrintPageEventHandler PrintPage;

        [System.Drawing.SRDescription("PDOCqueryPageSettingsDescr")]
        public event QueryPageSettingsEventHandler QueryPageSettings;

        public PrintDocument()
        {
            this.defaultPageSettings = new PageSettings(this.printerSettings);
        }

        internal void _OnBeginPrint(PrintEventArgs e)
        {
            this.OnBeginPrint(e);
        }

        internal void _OnEndPrint(PrintEventArgs e)
        {
            this.OnEndPrint(e);
        }

        internal void _OnPrintPage(PrintPageEventArgs e)
        {
            this.OnPrintPage(e);
        }

        internal void _OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            this.OnQueryPageSettings(e);
        }

        protected virtual void OnBeginPrint(PrintEventArgs e)
        {
            if (this.beginPrintHandler != null)
            {
                this.beginPrintHandler(this, e);
            }
        }

        protected virtual void OnEndPrint(PrintEventArgs e)
        {
            if (this.endPrintHandler != null)
            {
                this.endPrintHandler(this, e);
            }
        }

        protected virtual void OnPrintPage(PrintPageEventArgs e)
        {
            if (this.printPageHandler != null)
            {
                this.printPageHandler(this, e);
            }
        }

        protected virtual void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            if (this.queryHandler != null)
            {
                this.queryHandler(this, e);
            }
        }

        public void Print()
        {
            if (!this.PrinterSettings.IsDefaultPrinter && !this.PrinterSettings.PrintDialogDisplayed)
            {
                System.Drawing.IntSecurity.AllPrinting.Demand();
            }
            this.PrintController.Print(this);
        }

        public override string ToString() => 
            ("[PrintDocument " + this.DocumentName + "]");

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Drawing.SRDescription("PDOCdocumentPageSettingsDescr")]
        public PageSettings DefaultPageSettings
        {
            get => 
                this.defaultPageSettings;
            set
            {
                if (value == null)
                {
                    value = new PageSettings();
                }
                this.defaultPageSettings = value;
                this.userSetPageSettings = true;
            }
        }

        [System.Drawing.SRDescription("PDOCdocumentNameDescr"), DefaultValue("document")]
        public string DocumentName
        {
            get => 
                this.documentName;
            set
            {
                if (value == null)
                {
                    value = "";
                }
                this.documentName = value;
            }
        }

        [System.Drawing.SRDescription("PDOCoriginAtMarginsDescr"), DefaultValue(false)]
        public bool OriginAtMargins
        {
            get => 
                this.originAtMargins;
            set
            {
                this.originAtMargins = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Drawing.SRDescription("PDOCprintControllerDescr"), Browsable(false)]
        public System.Drawing.Printing.PrintController PrintController
        {
            get
            {
                System.Drawing.IntSecurity.SafePrinting.Demand();
                if (this.printController == null)
                {
                    this.printController = new StandardPrintController();
                    new ReflectionPermission(PermissionState.Unrestricted).Assert();
                    try
                    {
                        Type type = Type.GetType("System.Windows.Forms.PrintControllerWithStatusDialog, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                        this.printController = (System.Drawing.Printing.PrintController) Activator.CreateInstance(type, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { this.printController }, null);
                    }
                    catch (TypeLoadException)
                    {
                    }
                    catch (TargetInvocationException)
                    {
                    }
                    catch (MissingMethodException)
                    {
                    }
                    catch (MethodAccessException)
                    {
                    }
                    catch (MemberAccessException)
                    {
                    }
                    catch (FileNotFoundException)
                    {
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }
                }
                return this.printController;
            }
            set
            {
                System.Drawing.IntSecurity.SafePrinting.Demand();
                this.printController = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Drawing.SRDescription("PDOCprinterSettingsDescr"), Browsable(false)]
        public System.Drawing.Printing.PrinterSettings PrinterSettings
        {
            get => 
                this.printerSettings;
            set
            {
                if (value == null)
                {
                    value = new System.Drawing.Printing.PrinterSettings();
                }
                this.printerSettings = value;
                if (!this.userSetPageSettings)
                {
                    this.defaultPageSettings = this.printerSettings.DefaultPageSettings;
                }
            }
        }
    }
}

