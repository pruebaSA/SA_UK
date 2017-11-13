namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Printing;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ToolboxItem(true), ToolboxItemFilter("System.Windows.Forms.Control.TopLevel"), System.Windows.Forms.SRDescription("DescriptionPrintPreviewDialog"), ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch), Designer("System.ComponentModel.Design.ComponentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DesignTimeVisible(true), DefaultProperty("Document")]
    public class PrintPreviewDialog : Form
    {
        private ToolStripMenuItem autoToolStripMenuItem;
        private ToolStripButton closeToolStripButton;
        private ToolStripButton fourpagesToolStripButton;
        private ImageList imageList;
        private ToolStripButton onepageToolStripButton;
        private NumericUpDown pageCounter;
        private ToolStripLabel pageToolStripLabel;
        private System.Windows.Forms.PrintPreviewControl previewControl;
        private ToolStripButton printToolStripButton;
        private ToolStripSeparator separatorToolStripSeparator;
        private ToolStripSeparator separatorToolStripSeparator1;
        private ToolStripButton sixpagesToolStripButton;
        private ToolStripButton threepagesToolStripButton;
        private ToolStrip toolStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripButton twopagesToolStripButton;
        private ToolStripSplitButton zoomToolStripSplitButton;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler AutoSizeChanged
        {
            add
            {
                base.AutoSizeChanged += value;
            }
            remove
            {
                base.AutoSizeChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler AutoValidateChanged
        {
            add
            {
                base.AutoValidateChanged += value;
            }
            remove
            {
                base.AutoValidateChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler BackColorChanged
        {
            add
            {
                base.BackColorChanged += value;
            }
            remove
            {
                base.BackColorChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler BackgroundImageChanged
        {
            add
            {
                base.BackgroundImageChanged += value;
            }
            remove
            {
                base.BackgroundImageChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler BackgroundImageLayoutChanged
        {
            add
            {
                base.BackgroundImageLayoutChanged += value;
            }
            remove
            {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler CausesValidationChanged
        {
            add
            {
                base.CausesValidationChanged += value;
            }
            remove
            {
                base.CausesValidationChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler ContextMenuChanged
        {
            add
            {
                base.ContextMenuChanged += value;
            }
            remove
            {
                base.ContextMenuChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler ContextMenuStripChanged
        {
            add
            {
                base.ContextMenuStripChanged += value;
            }
            remove
            {
                base.ContextMenuStripChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler CursorChanged
        {
            add
            {
                base.CursorChanged += value;
            }
            remove
            {
                base.CursorChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler DockChanged
        {
            add
            {
                base.DockChanged += value;
            }
            remove
            {
                base.DockChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler EnabledChanged
        {
            add
            {
                base.EnabledChanged += value;
            }
            remove
            {
                base.EnabledChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler FontChanged
        {
            add
            {
                base.FontChanged += value;
            }
            remove
            {
                base.FontChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler ForeColorChanged
        {
            add
            {
                base.ForeColorChanged += value;
            }
            remove
            {
                base.ForeColorChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler ImeModeChanged
        {
            add
            {
                base.ImeModeChanged += value;
            }
            remove
            {
                base.ImeModeChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler LocationChanged
        {
            add
            {
                base.LocationChanged += value;
            }
            remove
            {
                base.LocationChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler MarginChanged
        {
            add
            {
                base.MarginChanged += value;
            }
            remove
            {
                base.MarginChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler MaximumSizeChanged
        {
            add
            {
                base.MaximumSizeChanged += value;
            }
            remove
            {
                base.MaximumSizeChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler MinimumSizeChanged
        {
            add
            {
                base.MinimumSizeChanged += value;
            }
            remove
            {
                base.MinimumSizeChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler PaddingChanged
        {
            add
            {
                base.PaddingChanged += value;
            }
            remove
            {
                base.PaddingChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler RightToLeftChanged
        {
            add
            {
                base.RightToLeftChanged += value;
            }
            remove
            {
                base.RightToLeftChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler RightToLeftLayoutChanged
        {
            add
            {
                base.RightToLeftLayoutChanged += value;
            }
            remove
            {
                base.RightToLeftLayoutChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler SizeChanged
        {
            add
            {
                base.SizeChanged += value;
            }
            remove
            {
                base.SizeChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler TabStopChanged
        {
            add
            {
                base.TabStopChanged += value;
            }
            remove
            {
                base.TabStopChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event EventHandler VisibleChanged
        {
            add
            {
                base.VisibleChanged += value;
            }
            remove
            {
                base.VisibleChanged -= value;
            }
        }

        public PrintPreviewDialog()
        {
            base.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.previewControl = new System.Windows.Forms.PrintPreviewControl();
            this.imageList = new ImageList();
            Bitmap bitmap = new Bitmap(typeof(PrintPreviewDialog), "PrintPreviewStrip.bmp");
            bitmap.MakeTransparent();
            this.imageList.Images.AddStrip(bitmap);
            this.InitForm();
        }

        private void CheckZoomMenu(ToolStripMenuItem toChecked)
        {
            foreach (ToolStripMenuItem item in this.zoomToolStripSplitButton.DropDownItems)
            {
                item.Checked = toChecked == item;
            }
        }

        protected override void CreateHandle()
        {
            if ((this.Document != null) && !this.Document.PrinterSettings.IsValid)
            {
                throw new InvalidPrinterException(this.Document.PrinterSettings);
            }
            base.CreateHandle();
        }

        private void InitForm()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(PrintPreviewDialog));
            this.toolStrip1 = new ToolStrip();
            this.printToolStripButton = new ToolStripButton();
            this.zoomToolStripSplitButton = new ToolStripSplitButton();
            this.autoToolStripMenuItem = new ToolStripMenuItem();
            this.toolStripMenuItem1 = new ToolStripMenuItem();
            this.toolStripMenuItem2 = new ToolStripMenuItem();
            this.toolStripMenuItem3 = new ToolStripMenuItem();
            this.toolStripMenuItem4 = new ToolStripMenuItem();
            this.toolStripMenuItem5 = new ToolStripMenuItem();
            this.toolStripMenuItem6 = new ToolStripMenuItem();
            this.toolStripMenuItem7 = new ToolStripMenuItem();
            this.toolStripMenuItem8 = new ToolStripMenuItem();
            this.separatorToolStripSeparator = new ToolStripSeparator();
            this.onepageToolStripButton = new ToolStripButton();
            this.twopagesToolStripButton = new ToolStripButton();
            this.threepagesToolStripButton = new ToolStripButton();
            this.fourpagesToolStripButton = new ToolStripButton();
            this.sixpagesToolStripButton = new ToolStripButton();
            this.separatorToolStripSeparator1 = new ToolStripSeparator();
            this.closeToolStripButton = new ToolStripButton();
            this.pageCounter = new NumericUpDown();
            this.pageToolStripLabel = new ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            this.pageCounter.BeginInit();
            base.SuspendLayout();
            manager.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new ToolStripItem[] { this.printToolStripButton, this.zoomToolStripSplitButton, this.separatorToolStripSeparator, this.onepageToolStripButton, this.twopagesToolStripButton, this.threepagesToolStripButton, this.fourpagesToolStripButton, this.sixpagesToolStripButton, this.separatorToolStripSeparator1, this.closeToolStripButton });
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = ToolStripRenderMode.System;
            this.toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
            this.printToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.printToolStripButton.Name = "printToolStripButton";
            manager.ApplyResources(this.printToolStripButton, "printToolStripButton");
            this.zoomToolStripSplitButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.zoomToolStripSplitButton.DoubleClickEnabled = true;
            this.zoomToolStripSplitButton.DropDownItems.AddRange(new ToolStripItem[] { this.autoToolStripMenuItem, this.toolStripMenuItem1, this.toolStripMenuItem2, this.toolStripMenuItem3, this.toolStripMenuItem4, this.toolStripMenuItem5, this.toolStripMenuItem6, this.toolStripMenuItem7, this.toolStripMenuItem8 });
            this.zoomToolStripSplitButton.Name = "zoomToolStripSplitButton";
            this.zoomToolStripSplitButton.SplitterWidth = 1;
            manager.ApplyResources(this.zoomToolStripSplitButton, "zoomToolStripSplitButton");
            this.autoToolStripMenuItem.CheckOnClick = true;
            this.autoToolStripMenuItem.DoubleClickEnabled = true;
            this.autoToolStripMenuItem.Checked = true;
            this.autoToolStripMenuItem.Name = "autoToolStripMenuItem";
            manager.ApplyResources(this.autoToolStripMenuItem, "autoToolStripMenuItem");
            this.toolStripMenuItem1.CheckOnClick = true;
            this.toolStripMenuItem1.DoubleClickEnabled = true;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            manager.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem2.CheckOnClick = true;
            this.toolStripMenuItem2.DoubleClickEnabled = true;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            manager.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem3.CheckOnClick = true;
            this.toolStripMenuItem3.DoubleClickEnabled = true;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            manager.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem4.CheckOnClick = true;
            this.toolStripMenuItem4.DoubleClickEnabled = true;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            manager.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem5.CheckOnClick = true;
            this.toolStripMenuItem5.DoubleClickEnabled = true;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            manager.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            this.toolStripMenuItem6.CheckOnClick = true;
            this.toolStripMenuItem6.DoubleClickEnabled = true;
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            manager.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            this.toolStripMenuItem7.CheckOnClick = true;
            this.toolStripMenuItem7.DoubleClickEnabled = true;
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            manager.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
            this.toolStripMenuItem8.CheckOnClick = true;
            this.toolStripMenuItem8.DoubleClickEnabled = true;
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            manager.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
            this.separatorToolStripSeparator.Name = "separatorToolStripSeparator";
            this.onepageToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.onepageToolStripButton.Name = "onepageToolStripButton";
            manager.ApplyResources(this.onepageToolStripButton, "onepageToolStripButton");
            this.twopagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.twopagesToolStripButton.Name = "twopagesToolStripButton";
            manager.ApplyResources(this.twopagesToolStripButton, "twopagesToolStripButton");
            this.threepagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.threepagesToolStripButton.Name = "threepagesToolStripButton";
            manager.ApplyResources(this.threepagesToolStripButton, "threepagesToolStripButton");
            this.fourpagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.fourpagesToolStripButton.Name = "fourpagesToolStripButton";
            manager.ApplyResources(this.fourpagesToolStripButton, "fourpagesToolStripButton");
            this.sixpagesToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.sixpagesToolStripButton.Name = "sixpagesToolStripButton";
            manager.ApplyResources(this.sixpagesToolStripButton, "sixpagesToolStripButton");
            this.separatorToolStripSeparator1.Name = "separatorToolStripSeparator1";
            this.closeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.closeToolStripButton.Name = "closeToolStripButton";
            manager.ApplyResources(this.closeToolStripButton, "closeToolStripButton");
            manager.ApplyResources(this.pageCounter, "pageCounter");
            this.pageCounter.Text = "1";
            this.pageCounter.TextAlign = HorizontalAlignment.Right;
            this.pageCounter.DecimalPlaces = 0;
            this.pageCounter.Minimum = 0M;
            this.pageCounter.Maximum = 1000M;
            this.pageCounter.ValueChanged += new EventHandler(this.UpdownMove);
            this.pageCounter.Name = "pageCounter";
            this.pageToolStripLabel.Alignment = ToolStripItemAlignment.Right;
            this.pageToolStripLabel.Name = "pageToolStripLabel";
            manager.ApplyResources(this.pageToolStripLabel, "pageToolStripLabel");
            this.previewControl.Size = new System.Drawing.Size(0x318, 610);
            this.previewControl.Location = new Point(0, 0x2b);
            this.previewControl.Dock = DockStyle.Fill;
            this.previewControl.StartPageChanged += new EventHandler(this.previewControl_StartPageChanged);
            this.printToolStripButton.Click += new EventHandler(this.OnprintToolStripButtonClick);
            this.autoToolStripMenuItem.Click += new EventHandler(this.ZoomAuto);
            this.toolStripMenuItem1.Click += new EventHandler(this.Zoom500);
            this.toolStripMenuItem2.Click += new EventHandler(this.Zoom250);
            this.toolStripMenuItem3.Click += new EventHandler(this.Zoom150);
            this.toolStripMenuItem4.Click += new EventHandler(this.Zoom100);
            this.toolStripMenuItem5.Click += new EventHandler(this.Zoom75);
            this.toolStripMenuItem6.Click += new EventHandler(this.Zoom50);
            this.toolStripMenuItem7.Click += new EventHandler(this.Zoom25);
            this.toolStripMenuItem8.Click += new EventHandler(this.Zoom10);
            this.onepageToolStripButton.Click += new EventHandler(this.OnonepageToolStripButtonClick);
            this.twopagesToolStripButton.Click += new EventHandler(this.OntwopagesToolStripButtonClick);
            this.threepagesToolStripButton.Click += new EventHandler(this.OnthreepagesToolStripButtonClick);
            this.fourpagesToolStripButton.Click += new EventHandler(this.OnfourpagesToolStripButtonClick);
            this.sixpagesToolStripButton.Click += new EventHandler(this.OnsixpagesToolStripButtonClick);
            this.closeToolStripButton.Click += new EventHandler(this.OncloseToolStripButtonClick);
            this.closeToolStripButton.Paint += new PaintEventHandler(this.OncloseToolStripButtonPaint);
            this.toolStrip1.ImageList = this.imageList;
            this.printToolStripButton.ImageIndex = 0;
            this.zoomToolStripSplitButton.ImageIndex = 1;
            this.onepageToolStripButton.ImageIndex = 2;
            this.twopagesToolStripButton.ImageIndex = 3;
            this.threepagesToolStripButton.ImageIndex = 4;
            this.fourpagesToolStripButton.ImageIndex = 5;
            this.sixpagesToolStripButton.ImageIndex = 6;
            this.previewControl.TabIndex = 0;
            this.toolStrip1.TabIndex = 1;
            this.zoomToolStripSplitButton.DefaultItem = this.autoToolStripMenuItem;
            ToolStripDropDownMenu dropDown = this.zoomToolStripSplitButton.DropDown as ToolStripDropDownMenu;
            if (dropDown != null)
            {
                dropDown.ShowCheckMargin = true;
                dropDown.ShowImageMargin = false;
                dropDown.RenderMode = ToolStripRenderMode.System;
            }
            ToolStripControlHost host = new ToolStripControlHost(this.pageCounter) {
                Alignment = ToolStripItemAlignment.Right
            };
            this.toolStrip1.Items.Add(host);
            this.toolStrip1.Items.Add(this.pageToolStripLabel);
            manager.ApplyResources(this, "$this");
            base.Controls.Add(this.previewControl);
            base.Controls.Add(this.toolStrip1);
            base.ClientSize = new System.Drawing.Size(400, 300);
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.toolStrip1.ResumeLayout(false);
            this.pageCounter.EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void OncloseToolStripButtonClick(object sender, EventArgs e)
        {
            base.Close();
        }

        private void OncloseToolStripButtonPaint(object sender, PaintEventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if ((item != null) && !item.Selected)
            {
                Rectangle rect = new Rectangle(0, 0, item.Bounds.Width - 1, item.Bounds.Height - 1);
                e.Graphics.DrawRectangle(new Pen(SystemColors.ControlDark), rect);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.previewControl.InvalidatePreview();
        }

        private void OnfourpagesToolStripButtonClick(object sender, EventArgs e)
        {
            this.previewControl.Rows = 2;
            this.previewControl.Columns = 2;
        }

        private void OnonepageToolStripButtonClick(object sender, EventArgs e)
        {
            this.previewControl.Rows = 1;
            this.previewControl.Columns = 1;
        }

        private void OnprintToolStripButtonClick(object sender, EventArgs e)
        {
            if (this.previewControl.Document != null)
            {
                this.previewControl.Document.Print();
            }
        }

        private void OnsixpagesToolStripButtonClick(object sender, EventArgs e)
        {
            this.previewControl.Rows = 2;
            this.previewControl.Columns = 3;
        }

        private void OnthreepagesToolStripButtonClick(object sender, EventArgs e)
        {
            this.previewControl.Rows = 1;
            this.previewControl.Columns = 3;
        }

        private void OntwopagesToolStripButtonClick(object sender, EventArgs e)
        {
            this.previewControl.Rows = 1;
            this.previewControl.Columns = 2;
        }

        private void OnzoomToolStripSplitButtonClick(object sender, EventArgs e)
        {
            this.ZoomAuto(null, EventArgs.Empty);
        }

        private void previewControl_StartPageChanged(object sender, EventArgs e)
        {
            this.pageCounter.Value = this.previewControl.StartPage + 1;
        }

        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
            {
                switch ((keyData & Keys.KeyCode))
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Right:
                    case Keys.Down:
                        return false;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessTabKey(bool forward)
        {
            if (base.ActiveControl == this.previewControl)
            {
                this.pageCounter.FocusInternal();
                return true;
            }
            return false;
        }

        internal override bool ShouldSerializeAutoScaleBaseSize() => 
            false;

        internal override bool ShouldSerializeText() => 
            !this.Text.Equals(System.Windows.Forms.SR.GetString("PrintPreviewDialog_PrintPreview"));

        private void UpdownMove(object sender, EventArgs eventargs)
        {
            int num = ((int) this.pageCounter.Value) - 1;
            if (num >= 0)
            {
                this.previewControl.StartPage = num;
            }
            else
            {
                this.pageCounter.Value = this.previewControl.StartPage + 1;
            }
        }

        private void Zoom10(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 0.1;
        }

        private void Zoom100(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 1.0;
        }

        private void Zoom150(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 1.5;
        }

        private void Zoom25(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 0.25;
        }

        private void Zoom250(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 2.5;
        }

        private void Zoom50(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 0.5;
        }

        private void Zoom500(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 5.0;
        }

        private void Zoom75(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.Zoom = 0.75;
        }

        private void ZoomAuto(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem toChecked = sender as ToolStripMenuItem;
            this.CheckZoomMenu(toChecked);
            this.previewControl.AutoZoom = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public IButtonControl AcceptButton
        {
            get => 
                base.AcceptButton;
            set
            {
                base.AcceptButton = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string AccessibleDescription
        {
            get => 
                base.AccessibleDescription;
            set
            {
                base.AccessibleDescription = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public string AccessibleName
        {
            get => 
                base.AccessibleName;
            set
            {
                base.AccessibleName = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Forms.AccessibleRole AccessibleRole
        {
            get => 
                base.AccessibleRole;
            set
            {
                base.AccessibleRole = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop
        {
            get => 
                base.AllowDrop;
            set
            {
                base.AllowDrop = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AnchorStyles Anchor
        {
            get => 
                base.Anchor;
            set
            {
                base.Anchor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool AutoScale
        {
            get => 
                base.AutoScale;
            set
            {
                base.AutoScale = value;
            }
        }

        [Browsable(false), Obsolete("This property has been deprecated. Use the AutoScaleDimensions property instead.  http://go.microsoft.com/fwlink/?linkid=14202"), EditorBrowsable(EditorBrowsableState.Never)]
        public override System.Drawing.Size AutoScaleBaseSize
        {
            get => 
                base.AutoScaleBaseSize;
            set
            {
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override bool AutoScroll
        {
            get => 
                base.AutoScroll;
            set
            {
                base.AutoScroll = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public System.Drawing.Size AutoScrollMargin
        {
            get => 
                base.AutoScrollMargin;
            set
            {
                base.AutoScrollMargin = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public System.Drawing.Size AutoScrollMinSize
        {
            get => 
                base.AutoScrollMinSize;
            set
            {
                base.AutoScrollMinSize = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override bool AutoSize
        {
            get => 
                base.AutoSize;
            set
            {
                base.AutoSize = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override System.Windows.Forms.AutoValidate AutoValidate
        {
            get => 
                base.AutoValidate;
            set
            {
                base.AutoValidate = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get => 
                base.BackColor;
            set
            {
                base.BackColor = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Image BackgroundImage
        {
            get => 
                base.BackgroundImage;
            set
            {
                base.BackgroundImage = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => 
                base.BackgroundImageLayout;
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public IButtonControl CancelButton
        {
            get => 
                base.CancelButton;
            set
            {
                base.CancelButton = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool CausesValidation
        {
            get => 
                base.CausesValidation;
            set
            {
                base.CausesValidation = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override System.Windows.Forms.ContextMenu ContextMenu
        {
            get => 
                base.ContextMenu;
            set
            {
                base.ContextMenu = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override System.Windows.Forms.ContextMenuStrip ContextMenuStrip
        {
            get => 
                base.ContextMenuStrip;
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool ControlBox
        {
            get => 
                base.ControlBox;
            set
            {
                base.ControlBox = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override System.Windows.Forms.Cursor Cursor
        {
            get => 
                base.Cursor;
            set
            {
                base.Cursor = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ControlBindingsCollection DataBindings =>
            base.DataBindings;

        protected override System.Drawing.Size DefaultMinimumSize =>
            new System.Drawing.Size(0x177, 250);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override DockStyle Dock
        {
            get => 
                base.Dock;
            set
            {
                base.Dock = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ScrollableControl.DockPaddingEdges DockPadding =>
            base.DockPadding;

        [System.Windows.Forms.SRDescription("PrintPreviewDocumentDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue((string) null)]
        public PrintDocument Document
        {
            get => 
                this.previewControl.Document;
            set
            {
                this.previewControl.Document = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override System.Drawing.Font Font
        {
            get => 
                base.Font;
            set
            {
                base.Font = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Color ForeColor
        {
            get => 
                base.ForeColor;
            set
            {
                base.ForeColor = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public System.Windows.Forms.FormBorderStyle FormBorderStyle
        {
            get => 
                base.FormBorderStyle;
            set
            {
                base.FormBorderStyle = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool HelpButton
        {
            get => 
                base.HelpButton;
            set
            {
                base.HelpButton = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Drawing.Icon Icon
        {
            get => 
                base.Icon;
            set
            {
                base.Icon = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Forms.ImeMode ImeMode
        {
            get => 
                base.ImeMode;
            set
            {
                base.ImeMode = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool IsMdiContainer
        {
            get => 
                base.IsMdiContainer;
            set
            {
                base.IsMdiContainer = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool KeyPreview
        {
            get => 
                base.KeyPreview;
            set
            {
                base.KeyPreview = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point Location
        {
            get => 
                base.Location;
            set
            {
                base.Location = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public System.Windows.Forms.Padding Margin
        {
            get => 
                base.Margin;
            set
            {
                base.Margin = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool MaximizeBox
        {
            get => 
                base.MaximizeBox;
            set
            {
                base.MaximizeBox = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Drawing.Size MaximumSize
        {
            get => 
                base.MaximumSize;
            set
            {
                base.MaximumSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public MainMenu Menu
        {
            get => 
                base.Menu;
            set
            {
                base.Menu = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DefaultValue(false), Browsable(false)]
        public bool MinimizeBox
        {
            get => 
                base.MinimizeBox;
            set
            {
                base.MinimizeBox = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public System.Drawing.Size MinimumSize
        {
            get => 
                base.MinimumSize;
            set
            {
                base.MinimumSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public double Opacity
        {
            get => 
                base.Opacity;
            set
            {
                base.Opacity = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Forms.Padding Padding
        {
            get => 
                base.Padding;
            set
            {
                base.Padding = value;
            }
        }

        [Browsable(false), System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("PrintPreviewPrintPreviewControlDescr")]
        public System.Windows.Forms.PrintPreviewControl PrintPreviewControl =>
            this.previewControl;

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override System.Windows.Forms.RightToLeft RightToLeft
        {
            get => 
                base.RightToLeft;
            set
            {
                base.RightToLeft = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool RightToLeftLayout
        {
            get => 
                base.RightToLeftLayout;
            set
            {
                base.RightToLeftLayout = value;
            }
        }

        [DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool ShowInTaskbar
        {
            get => 
                base.ShowInTaskbar;
            set
            {
                base.ShowInTaskbar = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public System.Drawing.Size Size
        {
            get => 
                base.Size;
            set
            {
                base.Size = value;
            }
        }

        [Browsable(false), DefaultValue(2), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Windows.Forms.SizeGripStyle SizeGripStyle
        {
            get => 
                base.SizeGripStyle;
            set
            {
                base.SizeGripStyle = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public FormStartPosition StartPosition
        {
            get => 
                base.StartPosition;
            set
            {
                base.StartPosition = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool TabStop
        {
            get => 
                base.TabStop;
            set
            {
                base.TabStop = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public object Tag
        {
            get => 
                base.Tag;
            set
            {
                base.Tag = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get => 
                base.Text;
            set
            {
                base.Text = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool TopMost
        {
            get => 
                base.TopMost;
            set
            {
                base.TopMost = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public Color TransparencyKey
        {
            get => 
                base.TransparencyKey;
            set
            {
                base.TransparencyKey = value;
            }
        }

        [System.Windows.Forms.SRDescription("PrintPreviewAntiAliasDescr"), DefaultValue(false), System.Windows.Forms.SRCategory("CatBehavior")]
        public bool UseAntiAlias
        {
            get => 
                this.PrintPreviewControl.UseAntiAlias;
            set
            {
                this.PrintPreviewControl.UseAntiAlias = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool UseWaitCursor
        {
            get => 
                base.UseWaitCursor;
            set
            {
                base.UseWaitCursor = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool Visible
        {
            get => 
                base.Visible;
            set
            {
                base.Visible = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public FormWindowState WindowState
        {
            get => 
                base.WindowState;
            set
            {
                base.WindowState = value;
            }
        }
    }
}

