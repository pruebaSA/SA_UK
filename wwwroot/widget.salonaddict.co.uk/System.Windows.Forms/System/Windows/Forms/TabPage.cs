namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms.Layout;

    [Designer("System.Windows.Forms.Design.TabPageDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultProperty("Text"), ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch), ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("Click")]
    public class TabPage : Panel
    {
        private bool enterFired;
        private ImageList.Indexer imageIndexer;
        private bool leaveFired;
        private string toolTipText;
        private bool useVisualStyleBackColor;

        [System.Windows.Forms.SRCategory("CatPropertyChanged"), Browsable(false), System.Windows.Forms.SRDescription("ControlOnAutoSizeChangedDescr"), EditorBrowsable(EditorBrowsableState.Never)]
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

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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
        public event EventHandler TabIndexChanged
        {
            add
            {
                base.TabIndexChanged += value;
            }
            remove
            {
                base.TabIndexChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
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

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
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

        public TabPage()
        {
            this.toolTipText = "";
            base.SetStyle(ControlStyles.CacheText, true);
            this.Text = null;
        }

        public TabPage(string text) : this()
        {
            this.Text = text;
        }

        internal override void AssignParent(Control value)
        {
            if ((value != null) && !(value is TabControl))
            {
                throw new ArgumentException(System.Windows.Forms.SR.GetString("TABCONTROLTabPageNotOnTabControl", new object[] { value.GetType().FullName }));
            }
            base.AssignParent(value);
        }

        protected override Control.ControlCollection CreateControlsInstance() => 
            new TabPageControlCollection(this);

        internal void FireEnter(EventArgs e)
        {
            this.enterFired = true;
            this.OnEnter(e);
        }

        internal void FireLeave(EventArgs e)
        {
            this.leaveFired = true;
            this.OnLeave(e);
        }

        public static TabPage GetTabPageOfComponent(object comp)
        {
            if (!(comp is Control))
            {
                return null;
            }
            Control parentInternal = (Control) comp;
            while ((parentInternal != null) && !(parentInternal is TabPage))
            {
                parentInternal = parentInternal.ParentInternal;
            }
            return (TabPage) parentInternal;
        }

        internal System.Windows.Forms.NativeMethods.TCITEM_T GetTCITEM()
        {
            System.Windows.Forms.NativeMethods.TCITEM_T tcitem_t = new System.Windows.Forms.NativeMethods.TCITEM_T {
                mask = 0,
                pszText = null,
                cchTextMax = 0,
                lParam = IntPtr.Zero
            };
            string text = this.Text;
            this.PrefixAmpersands(ref text);
            if (text != null)
            {
                tcitem_t.mask |= 1;
                tcitem_t.pszText = text;
                tcitem_t.cchTextMax = text.Length;
            }
            int imageIndex = this.ImageIndex;
            tcitem_t.mask |= 2;
            tcitem_t.iImage = this.ImageIndexer.ActualIndex;
            return tcitem_t;
        }

        protected override void OnEnter(EventArgs e)
        {
            if (this.ParentInternal is TabControl)
            {
                if (this.enterFired)
                {
                    base.OnEnter(e);
                }
                this.enterFired = false;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            if (this.ParentInternal is TabControl)
            {
                if (this.leaveFired)
                {
                    base.OnLeave(e);
                }
                this.leaveFired = false;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            TabControl parentInternal = this.ParentInternal as TabControl;
            if ((Application.RenderWithVisualStyles && this.UseVisualStyleBackColor) && ((parentInternal != null) && (parentInternal.Appearance == TabAppearance.Normal)))
            {
                Color backColor = this.UseVisualStyleBackColor ? Color.Transparent : this.BackColor;
                Rectangle bounds = LayoutUtils.InflateRect(this.DisplayRectangle, base.Padding);
                Rectangle rectangle2 = new Rectangle(bounds.X - 4, bounds.Y - 2, bounds.Width + 8, bounds.Height + 6);
                TabRenderer.DrawTabPage(e.Graphics, rectangle2);
                if (this.BackgroundImage != null)
                {
                    ControlPaint.DrawBackgroundImage(e.Graphics, this.BackgroundImage, backColor, this.BackgroundImageLayout, bounds, bounds, this.DisplayRectangle.Location);
                }
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        private void PrefixAmpersands(ref string value)
        {
            if (((value != null) && (value.Length != 0)) && (value.IndexOf('&') >= 0))
            {
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < value.Length; i++)
                {
                    if (value[i] == '&')
                    {
                        if ((i < (value.Length - 1)) && (value[i + 1] == '&'))
                        {
                            i++;
                        }
                        builder.Append("&&");
                    }
                    else
                    {
                        builder.Append(value[i]);
                    }
                }
                value = builder.ToString();
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Control parentInternal = this.ParentInternal;
            if ((parentInternal is TabControl) && parentInternal.IsHandleCreated)
            {
                Rectangle displayRectangle = parentInternal.DisplayRectangle;
                base.SetBoundsCore(displayRectangle.X, displayRectangle.Y, displayRectangle.Width, displayRectangle.Height, (specified == BoundsSpecified.None) ? BoundsSpecified.None : BoundsSpecified.All);
            }
            else
            {
                base.SetBoundsCore(x, y, width, height, specified);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeLocation()
        {
            if (base.Left == 0)
            {
                return (base.Top != 0);
            }
            return true;
        }

        public override string ToString() => 
            ("TabPage: {" + this.Text + "}");

        internal void UpdateParent()
        {
            TabControl parentInternal = this.ParentInternal as TabControl;
            if (parentInternal != null)
            {
                parentInternal.UpdateTab(this);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override AnchorStyles Anchor
        {
            get => 
                base.Anchor;
            set
            {
                base.Anchor = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override bool AutoSize
        {
            get => 
                base.AutoSize;
            set
            {
                base.AutoSize = value;
            }
        }

        [Localizable(false), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Windows.Forms.AutoSizeMode AutoSizeMode
        {
            get => 
                System.Windows.Forms.AutoSizeMode.GrowOnly;
            set
            {
            }
        }

        [System.Windows.Forms.SRDescription("ControlBackColorDescr"), System.Windows.Forms.SRCategory("CatAppearance")]
        public override Color BackColor
        {
            get
            {
                Color backColor = base.BackColor;
                if (backColor == Control.DefaultBackColor)
                {
                    TabControl parentInternal = this.ParentInternal as TabControl;
                    if ((Application.RenderWithVisualStyles && this.UseVisualStyleBackColor) && ((parentInternal != null) && (parentInternal.Appearance == TabAppearance.Normal)))
                    {
                        return Color.Transparent;
                    }
                }
                return backColor;
            }
            set
            {
                if (base.DesignMode)
                {
                    if (value != Color.Empty)
                    {
                        PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this)["UseVisualStyleBackColor"];
                        if (descriptor != null)
                        {
                            descriptor.SetValue(this, false);
                        }
                    }
                }
                else
                {
                    this.UseVisualStyleBackColor = false;
                }
                base.BackColor = value;
            }
        }

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

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
            }
        }

        [DefaultValue(-1), Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), RefreshProperties(RefreshProperties.Repaint), TypeConverter(typeof(ImageIndexConverter)), System.Windows.Forms.SRDescription("TabItemImageIndexDescr"), Localizable(true)]
        public int ImageIndex
        {
            get => 
                this.ImageIndexer.Index;
            set
            {
                if (value < -1)
                {
                    object[] args = new object[] { "imageIndex", value.ToString(CultureInfo.CurrentCulture), -1.ToString(CultureInfo.CurrentCulture) };
                    throw new ArgumentOutOfRangeException("ImageIndex", System.Windows.Forms.SR.GetString("InvalidLowBoundArgumentEx", args));
                }
                TabControl parentInternal = this.ParentInternal as TabControl;
                if (parentInternal != null)
                {
                    this.ImageIndexer.ImageList = parentInternal.ImageList;
                }
                this.ImageIndexer.Index = value;
                this.UpdateParent();
            }
        }

        internal ImageList.Indexer ImageIndexer
        {
            get
            {
                if (this.imageIndexer == null)
                {
                    this.imageIndexer = new ImageList.Indexer();
                }
                return this.imageIndexer;
            }
        }

        [RefreshProperties(RefreshProperties.Repaint), DefaultValue(""), Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), TypeConverter(typeof(ImageKeyConverter)), Localizable(true), System.Windows.Forms.SRDescription("TabItemImageIndexDescr")]
        public string ImageKey
        {
            get => 
                this.ImageIndexer.Key;
            set
            {
                this.ImageIndexer.Key = value;
                TabControl parentInternal = this.ParentInternal as TabControl;
                if (parentInternal != null)
                {
                    this.ImageIndexer.ImageList = parentInternal.ImageList;
                }
                this.UpdateParent();
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public Point Location
        {
            get => 
                base.Location;
            set
            {
                base.Location = value;
            }
        }

        [DefaultValue(typeof(Size), "0, 0"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Size MaximumSize
        {
            get => 
                base.MaximumSize;
            set
            {
                base.MaximumSize = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Size MinimumSize
        {
            get => 
                base.MinimumSize;
            set
            {
                base.MinimumSize = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public Size PreferredSize =>
            base.PreferredSize;

        internal override bool RenderTransparencyWithVisualStyles =>
            true;

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public int TabIndex
        {
            get => 
                base.TabIndex;
            set
            {
                base.TabIndex = value;
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

        [Localizable(true), EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
        public override string Text
        {
            get => 
                base.Text;
            set
            {
                base.Text = value;
                this.UpdateParent();
            }
        }

        [DefaultValue(""), Localizable(true), System.Windows.Forms.SRDescription("TabItemToolTipTextDescr")]
        public string ToolTipText
        {
            get => 
                this.toolTipText;
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if (value != this.toolTipText)
                {
                    this.toolTipText = value;
                    this.UpdateParent();
                }
            }
        }

        [DefaultValue(false), System.Windows.Forms.SRDescription("TabItemUseVisualStyleBackColorDescr"), System.Windows.Forms.SRCategory("CatAppearance")]
        public bool UseVisualStyleBackColor
        {
            get => 
                this.useVisualStyleBackColor;
            set
            {
                this.useVisualStyleBackColor = value;
                base.Invalidate(true);
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

        [ComVisible(false)]
        public class TabPageControlCollection : Control.ControlCollection
        {
            public TabPageControlCollection(TabPage owner) : base(owner)
            {
            }

            public override void Add(Control value)
            {
                if (value is TabPage)
                {
                    throw new ArgumentException(System.Windows.Forms.SR.GetString("TABCONTROLTabPageOnTabPage"));
                }
                base.Add(value);
            }
        }
    }
}

