namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;

    [Designer("System.Windows.Forms.Design.SplitterPanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ClassInterface(ClassInterfaceType.AutoDispatch), ComVisible(true), ToolboxItem(false), Docking(DockingBehavior.Never)]
    public sealed class SplitterPanel : Panel
    {
        private bool collapsed;
        private SplitContainer owner;

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

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
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

        public SplitterPanel(SplitContainer owner)
        {
            this.owner = owner;
            base.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnchorStyles Anchor
        {
            get => 
                base.Anchor;
            set
            {
                base.Anchor = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool AutoSize
        {
            get => 
                base.AutoSize;
            set
            {
                base.AutoSize = value;
            }
        }

        [Browsable(false), Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override System.Windows.Forms.AutoSizeMode AutoSizeMode
        {
            get => 
                System.Windows.Forms.AutoSizeMode.GrowOnly;
            set
            {
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public System.Windows.Forms.BorderStyle BorderStyle
        {
            get => 
                base.BorderStyle;
            set
            {
                base.BorderStyle = value;
            }
        }

        internal bool Collapsed
        {
            get => 
                this.collapsed;
            set
            {
                this.collapsed = value;
            }
        }

        protected override Padding DefaultMargin =>
            new Padding(0, 0, 0, 0);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockStyle Dock
        {
            get => 
                base.Dock;
            set
            {
                base.Dock = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScrollableControl.DockPaddingEdges DockPadding =>
            base.DockPadding;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Windows.Forms.SRCategory("CatLayout"), System.Windows.Forms.SRDescription("ControlHeightDescr"), Browsable(false), EditorBrowsable(EditorBrowsableState.Always)]
        public int Height
        {
            get
            {
                if (this.Collapsed)
                {
                    return 0;
                }
                return base.Height;
            }
            set
            {
                throw new NotSupportedException(System.Windows.Forms.SR.GetString("SplitContainerPanelHeight"));
            }
        }

        internal int HeightInternal
        {
            get => 
                base.Height;
            set
            {
                base.Height = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public Point Location
        {
            get => 
                base.Location;
            set
            {
                base.Location = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Size MaximumSize
        {
            get => 
                base.MaximumSize;
            set
            {
                base.MaximumSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Drawing.Size MinimumSize
        {
            get => 
                base.MinimumSize;
            set
            {
                base.MinimumSize = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public string Name
        {
            get => 
                base.Name;
            set
            {
                base.Name = value;
            }
        }

        internal SplitContainer Owner =>
            this.owner;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public Control Parent
        {
            get => 
                base.Parent;
            set
            {
                base.Parent = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public System.Drawing.Size Size
        {
            get
            {
                if (this.Collapsed)
                {
                    return System.Drawing.Size.Empty;
                }
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int TabIndex
        {
            get => 
                base.TabIndex;
            set
            {
                base.TabIndex = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public bool TabStop
        {
            get => 
                base.TabStop;
            set
            {
                base.TabStop = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Visible
        {
            get => 
                base.Visible;
            set
            {
                base.Visible = value;
            }
        }

        [Browsable(false), System.Windows.Forms.SRDescription("ControlWidthDescr"), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Windows.Forms.SRCategory("CatLayout")]
        public int Width
        {
            get
            {
                if (this.Collapsed)
                {
                    return 0;
                }
                return base.Width;
            }
            set
            {
                throw new NotSupportedException(System.Windows.Forms.SR.GetString("SplitContainerPanelWidth"));
            }
        }

        internal int WidthInternal
        {
            get => 
                base.Width;
            set
            {
                base.Width = value;
            }
        }
    }
}

