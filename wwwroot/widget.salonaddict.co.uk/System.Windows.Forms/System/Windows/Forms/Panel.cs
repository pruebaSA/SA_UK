namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms.Layout;

    [Docking(DockingBehavior.Ask), ComVisible(true), Designer("System.Windows.Forms.Design.PanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), System.Windows.Forms.SRDescription("DescriptionPanel"), ClassInterface(ClassInterfaceType.AutoDispatch), DefaultProperty("BorderStyle"), DefaultEvent("Paint")]
    public class Panel : ScrollableControl
    {
        private System.Windows.Forms.BorderStyle borderStyle;

        [EditorBrowsable(EditorBrowsableState.Always), System.Windows.Forms.SRCategory("CatPropertyChanged"), System.Windows.Forms.SRDescription("ControlOnAutoSizeChangedDescr"), Browsable(true)]
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
        public event KeyEventHandler KeyDown
        {
            add
            {
                base.KeyDown += value;
            }
            remove
            {
                base.KeyDown -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event KeyPressEventHandler KeyPress
        {
            add
            {
                base.KeyPress += value;
            }
            remove
            {
                base.KeyPress -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event KeyEventHandler KeyUp
        {
            add
            {
                base.KeyUp += value;
            }
            remove
            {
                base.KeyUp -= value;
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

        public Panel()
        {
            base.SetState2(0x800, true);
            this.TabStop = false;
            base.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Selectable, false);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        internal override Size GetPreferredSizeCore(Size proposedSize)
        {
            Size size2 = this.SizeFromClientSize(Size.Empty) + base.Padding.Size;
            return (this.LayoutEngine.GetPreferredSize(this, proposedSize - size2) + size2);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            if (base.DesignMode && (this.borderStyle == System.Windows.Forms.BorderStyle.None))
            {
                base.Invalidate();
            }
            base.OnResize(eventargs);
        }

        internal override void PrintToMetaFileRecursive(HandleRef hDC, IntPtr lParam, Rectangle bounds)
        {
            base.PrintToMetaFileRecursive(hDC, lParam, bounds);
            using (new WindowsFormsUtils.DCMapping(hDC, bounds))
            {
                using (Graphics graphics = Graphics.FromHdcInternal(hDC.Handle))
                {
                    ControlPaint.PrintBorder(graphics, new Rectangle(Point.Empty, base.Size), this.BorderStyle, Border3DStyle.Sunken);
                }
            }
        }

        private static string StringFromBorderStyle(System.Windows.Forms.BorderStyle value)
        {
            System.Type type = typeof(System.Windows.Forms.BorderStyle);
            if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 0, 2))
            {
                return "[Invalid BorderStyle]";
            }
            return (type.ToString() + "." + value.ToString());
        }

        public override string ToString() => 
            (base.ToString() + ", BorderStyle: " + StringFromBorderStyle(this.borderStyle));

        [EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), Browsable(true)]
        public override bool AutoSize
        {
            get => 
                base.AutoSize;
            set
            {
                base.AutoSize = value;
            }
        }

        [Localizable(true), System.Windows.Forms.SRCategory("CatLayout"), DefaultValue(1), System.Windows.Forms.SRDescription("ControlAutoSizeModeDescr"), Browsable(true)]
        public virtual System.Windows.Forms.AutoSizeMode AutoSizeMode
        {
            get => 
                base.GetAutoSizeMode();
            set
            {
                if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 0, 1))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(System.Windows.Forms.AutoSizeMode));
                }
                if (base.GetAutoSizeMode() != value)
                {
                    base.SetAutoSizeMode(value);
                    if (this.ParentInternal != null)
                    {
                        if (this.ParentInternal.LayoutEngine == DefaultLayout.Instance)
                        {
                            this.ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(this.ParentInternal, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        [DefaultValue(0), System.Windows.Forms.SRDescription("PanelBorderStyleDescr"), System.Windows.Forms.SRCategory("CatAppearance"), DispId(-504)]
        public System.Windows.Forms.BorderStyle BorderStyle
        {
            get => 
                this.borderStyle;
            set
            {
                if (this.borderStyle != value)
                {
                    if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 0, 2))
                    {
                        throw new InvalidEnumArgumentException("value", (int) value, typeof(System.Windows.Forms.BorderStyle));
                    }
                    this.borderStyle = value;
                    base.UpdateStyles();
                }
            }
        }

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                System.Windows.Forms.CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x10000;
                createParams.ExStyle &= -513;
                createParams.Style &= -8388609;
                switch (this.borderStyle)
                {
                    case System.Windows.Forms.BorderStyle.FixedSingle:
                        createParams.Style |= 0x800000;
                        return createParams;

                    case System.Windows.Forms.BorderStyle.Fixed3D:
                        createParams.ExStyle |= 0x200;
                        return createParams;
                }
                return createParams;
            }
        }

        protected override Size DefaultSize =>
            new Size(200, 100);

        [DefaultValue(false)]
        public bool TabStop
        {
            get => 
                base.TabStop;
            set
            {
                base.TabStop = value;
            }
        }

        [Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get => 
                base.Text;
            set
            {
                base.Text = value;
            }
        }
    }
}

