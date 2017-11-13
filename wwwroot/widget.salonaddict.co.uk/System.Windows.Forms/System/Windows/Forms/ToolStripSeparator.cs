namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Design;

    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.ToolStrip)]
    public class ToolStripSeparator : ToolStripItem
    {
        private const int WINBAR_SEPARATORHEIGHT = 0x17;
        private const int WINBAR_SEPARATORTHICKNESS = 6;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler DisplayStyleChanged
        {
            add
            {
                base.DisplayStyleChanged += value;
            }
            remove
            {
                base.DisplayStyleChanged -= value;
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

        public ToolStripSeparator()
        {
            this.ForeColor = SystemColors.ControlDark;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override AccessibleObject CreateAccessibilityInstance() => 
            new ToolStripSeparatorAccessibleObject(this);

        public override Size GetPreferredSize(Size constrainingSize)
        {
            ToolStrip parentInternal = base.ParentInternal;
            if (parentInternal == null)
            {
                parentInternal = base.Owner;
            }
            if (parentInternal == null)
            {
                return new Size(6, 6);
            }
            ToolStripDropDownMenu menu = parentInternal as ToolStripDropDownMenu;
            if (menu != null)
            {
                return new Size(parentInternal.Width - (parentInternal.Padding.Horizontal - menu.ImageMargin.Width), 6);
            }
            if ((parentInternal.LayoutStyle != ToolStripLayoutStyle.HorizontalStackWithOverflow) || (parentInternal.LayoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow))
            {
                constrainingSize.Width = 0x17;
                constrainingSize.Height = 0x17;
            }
            if (this.IsVertical)
            {
                return new Size(6, constrainingSize.Height);
            }
            return new Size(constrainingSize.Width, 6);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void OnFontChanged(EventArgs e)
        {
            base.RaiseEvent(ToolStripItem.EventFontChanged, e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if ((base.Owner != null) && (base.ParentInternal != null))
            {
                base.Renderer.DrawSeparator(new ToolStripSeparatorRenderEventArgs(e.Graphics, this, this.IsVertical));
            }
        }

        protected internal override void SetBounds(Rectangle rect)
        {
            ToolStripDropDownMenu owner = base.Owner as ToolStripDropDownMenu;
            if ((owner != null) && (owner != null))
            {
                rect.X = 2;
                rect.Width = owner.Width - 4;
            }
            base.SetBounds(rect);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override bool ShouldSerializeForeColor() => 
            (this.ForeColor != SystemColors.ControlDark);

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public bool AutoToolTip
        {
            get => 
                base.AutoToolTip;
            set
            {
                base.AutoToolTip = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Image BackgroundImage
        {
            get => 
                base.BackgroundImage;
            set
            {
                base.BackgroundImage = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get => 
                base.BackgroundImageLayout;
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        public override bool CanSelect =>
            base.DesignMode;

        protected internal override Padding DefaultMargin =>
            Padding.Empty;

        protected override Size DefaultSize =>
            new Size(6, 6);

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ToolStripItemDisplayStyle DisplayStyle
        {
            get => 
                base.DisplayStyle;
            set
            {
                base.DisplayStyle = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public bool DoubleClickEnabled
        {
            get => 
                base.DoubleClickEnabled;
            set
            {
                base.DoubleClickEnabled = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Font Font
        {
            get => 
                base.Font;
            set
            {
                base.Font = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Image Image
        {
            get => 
                base.Image;
            set
            {
                base.Image = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public ContentAlignment ImageAlign
        {
            get => 
                base.ImageAlign;
            set
            {
                base.ImageAlign = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), RefreshProperties(RefreshProperties.Repaint)]
        public int ImageIndex
        {
            get => 
                base.ImageIndex;
            set
            {
                base.ImageIndex = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ImageKey
        {
            get => 
                base.ImageKey;
            set
            {
                base.ImageKey = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public ToolStripItemImageScaling ImageScaling
        {
            get => 
                base.ImageScaling;
            set
            {
                base.ImageScaling = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color ImageTransparentColor
        {
            get => 
                base.ImageTransparentColor;
            set
            {
                base.ImageTransparentColor = value;
            }
        }

        private bool IsVertical
        {
            get
            {
                ToolStrip parentInternal = base.ParentInternal;
                if (parentInternal == null)
                {
                    parentInternal = base.Owner;
                }
                if (parentInternal is ToolStripDropDownMenu)
                {
                    return false;
                }
                switch (parentInternal.LayoutStyle)
                {
                    case ToolStripLayoutStyle.VerticalStackWithOverflow:
                        return false;
                }
                return true;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool RightToLeftAutoMirrorImage
        {
            get => 
                base.RightToLeftAutoMirrorImage;
            set
            {
                base.RightToLeftAutoMirrorImage = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get => 
                base.Text;
            set
            {
                base.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public ContentAlignment TextAlign
        {
            get => 
                base.TextAlign;
            set
            {
                base.TextAlign = value;
            }
        }

        [DefaultValue(1), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ToolStripTextDirection TextDirection
        {
            get => 
                base.TextDirection;
            set
            {
                base.TextDirection = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Windows.Forms.TextImageRelation TextImageRelation
        {
            get => 
                base.TextImageRelation;
            set
            {
                base.TextImageRelation = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ToolTipText
        {
            get => 
                base.ToolTipText;
            set
            {
                base.ToolTipText = value;
            }
        }

        [ComVisible(true)]
        internal class ToolStripSeparatorAccessibleObject : ToolStripItem.ToolStripItemAccessibleObject
        {
            private ToolStripSeparator ownerItem;

            public ToolStripSeparatorAccessibleObject(ToolStripSeparator ownerItem) : base(ownerItem)
            {
                this.ownerItem = ownerItem;
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole accessibleRole = this.ownerItem.AccessibleRole;
                    if (accessibleRole != AccessibleRole.Default)
                    {
                        return accessibleRole;
                    }
                    return AccessibleRole.Separator;
                }
            }
        }
    }
}

