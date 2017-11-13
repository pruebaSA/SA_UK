﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [DesignTimeVisible(false), ToolboxItem(false), DefaultProperty("Text")]
    public class StatusBarPanel : Component, ISupportInitialize
    {
        private HorizontalAlignment alignment;
        private StatusBarPanelAutoSize autoSize = StatusBarPanelAutoSize.None;
        private StatusBarPanelBorderStyle borderStyle = StatusBarPanelBorderStyle.Sunken;
        private const int DEFAULTMINWIDTH = 10;
        private const int DEFAULTWIDTH = 100;
        private System.Drawing.Icon icon;
        private int index;
        private bool initializing;
        private int minWidth = 10;
        private string name = "";
        private const int PANELGAP = 2;
        private const int PANELTEXTINSET = 3;
        private StatusBar parent;
        private int right;
        private StatusBarPanelStyle style = StatusBarPanelStyle.Text;
        private string text = "";
        private string toolTipText = "";
        private object userData;
        private int width = 100;

        private void ApplyContentSizing()
        {
            if ((this.autoSize == StatusBarPanelAutoSize.Contents) && (this.parent != null))
            {
                int contentsWidth = this.GetContentsWidth(false);
                if (contentsWidth != this.Width)
                {
                    this.Width = contentsWidth;
                    if (this.Created)
                    {
                        this.parent.DirtyLayout();
                        this.parent.PerformLayout();
                    }
                }
            }
        }

        public void BeginInit()
        {
            this.initializing = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.parent != null))
            {
                int index = this.GetIndex();
                if (index != -1)
                {
                    this.parent.Panels.RemoveAt(index);
                }
            }
            base.Dispose(disposing);
        }

        public void EndInit()
        {
            this.initializing = false;
            if (this.Width < this.MinWidth)
            {
                this.Width = this.MinWidth;
            }
        }

        internal int GetContentsWidth(bool newPanel)
        {
            string text;
            if (newPanel)
            {
                if (this.text == null)
                {
                    text = "";
                }
                else
                {
                    text = this.text;
                }
            }
            else
            {
                text = this.Text;
            }
            Graphics graphics = this.parent.CreateGraphicsInternal();
            Size size = Size.Ceiling(graphics.MeasureString(text, this.parent.Font));
            if (this.icon != null)
            {
                size.Width += this.icon.Size.Width + 5;
            }
            graphics.Dispose();
            int num = ((size.Width + (SystemInformation.BorderSize.Width * 2)) + 6) + 2;
            return Math.Max(num, this.minWidth);
        }

        private int GetIndex() => 
            this.index;

        internal void Realize()
        {
            if (this.Created)
            {
                string text;
                string str2;
                int num = 0;
                if (this.text == null)
                {
                    text = "";
                }
                else
                {
                    text = this.text;
                }
                HorizontalAlignment right = this.alignment;
                if (this.parent.RightToLeft == RightToLeft.Yes)
                {
                    switch (right)
                    {
                        case HorizontalAlignment.Left:
                            right = HorizontalAlignment.Right;
                            break;

                        case HorizontalAlignment.Right:
                            right = HorizontalAlignment.Left;
                            break;
                    }
                }
                switch (right)
                {
                    case HorizontalAlignment.Right:
                        str2 = "\t\t" + text;
                        break;

                    case HorizontalAlignment.Center:
                        str2 = "\t" + text;
                        break;

                    default:
                        str2 = text;
                        break;
                }
                switch (this.borderStyle)
                {
                    case StatusBarPanelBorderStyle.None:
                        num |= 0x100;
                        break;

                    case StatusBarPanelBorderStyle.Raised:
                        num |= 0x200;
                        break;
                }
                switch (this.style)
                {
                    case StatusBarPanelStyle.OwnerDraw:
                        num |= 0x1000;
                        break;
                }
                int num2 = this.GetIndex() | num;
                if (this.parent.RightToLeft == RightToLeft.Yes)
                {
                    num2 |= 0x400;
                }
                if (((int) System.Windows.Forms.UnsafeNativeMethods.SendMessage(new HandleRef(this.parent, this.parent.Handle), System.Windows.Forms.NativeMethods.SB_SETTEXT, (IntPtr) num2, str2)) == 0)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("UnableToSetPanelText"));
                }
                if ((this.icon != null) && (this.style != StatusBarPanelStyle.OwnerDraw))
                {
                    this.parent.SendMessage(0x40f, (IntPtr) this.GetIndex(), this.icon.Handle);
                }
                else
                {
                    this.parent.SendMessage(0x40f, (IntPtr) this.GetIndex(), IntPtr.Zero);
                }
                if (this.style == StatusBarPanelStyle.OwnerDraw)
                {
                    System.Windows.Forms.NativeMethods.RECT lParam = new System.Windows.Forms.NativeMethods.RECT();
                    if (((int) System.Windows.Forms.UnsafeNativeMethods.SendMessage(new HandleRef(this.parent, this.parent.Handle), 0x40a, (IntPtr) this.GetIndex(), ref lParam)) != 0)
                    {
                        this.parent.Invalidate(Rectangle.FromLTRB(lParam.left, lParam.top, lParam.right, lParam.bottom));
                    }
                }
            }
        }

        public override string ToString() => 
            ("StatusBarPanel: {" + this.Text + "}");

        private void UpdateSize()
        {
            if (this.autoSize == StatusBarPanelAutoSize.Contents)
            {
                this.ApplyContentSizing();
            }
            else if (this.Created)
            {
                this.parent.DirtyLayout();
                this.parent.PerformLayout();
            }
        }

        [Localizable(true), System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(0), System.Windows.Forms.SRDescription("StatusBarPanelAlignmentDescr")]
        public HorizontalAlignment Alignment
        {
            get => 
                this.alignment;
            set
            {
                if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 0, 2))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(HorizontalAlignment));
                }
                if (this.alignment != value)
                {
                    this.alignment = value;
                    this.Realize();
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), System.Windows.Forms.SRDescription("StatusBarPanelAutoSizeDescr"), DefaultValue(1), RefreshProperties(RefreshProperties.All)]
        public StatusBarPanelAutoSize AutoSize
        {
            get => 
                this.autoSize;
            set
            {
                if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 1, 3))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(StatusBarPanelAutoSize));
                }
                if (this.autoSize != value)
                {
                    this.autoSize = value;
                    this.UpdateSize();
                }
            }
        }

        [System.Windows.Forms.SRDescription("StatusBarPanelBorderStyleDescr"), System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(3), DispId(-504)]
        public StatusBarPanelBorderStyle BorderStyle
        {
            get => 
                this.borderStyle;
            set
            {
                if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 1, 3))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(StatusBarPanelBorderStyle));
                }
                if (this.borderStyle != value)
                {
                    this.borderStyle = value;
                    this.Realize();
                    if (this.Created)
                    {
                        this.parent.Invalidate();
                    }
                }
            }
        }

        internal bool Created =>
            ((this.parent != null) && this.parent.ArePanelsRealized());

        [Localizable(true), System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue((string) null), System.Windows.Forms.SRDescription("StatusBarPanelIconDescr")]
        public System.Drawing.Icon Icon
        {
            get => 
                this.icon;
            set
            {
                if ((value != null) && ((value.Height > SystemInformation.SmallIconSize.Height) || (value.Width > SystemInformation.SmallIconSize.Width)))
                {
                    this.icon = new System.Drawing.Icon(value, SystemInformation.SmallIconSize);
                }
                else
                {
                    this.icon = value;
                }
                if (this.Created)
                {
                    IntPtr lparam = (this.icon == null) ? IntPtr.Zero : this.icon.Handle;
                    this.parent.SendMessage(0x40f, (IntPtr) this.GetIndex(), lparam);
                }
                this.UpdateSize();
                if (this.Created)
                {
                    this.parent.Invalidate();
                }
            }
        }

        internal int Index
        {
            get => 
                this.index;
            set
            {
                this.index = value;
            }
        }

        [System.Windows.Forms.SRDescription("StatusBarPanelMinWidthDescr"), DefaultValue(10), System.Windows.Forms.SRCategory("CatBehavior"), Localizable(true), RefreshProperties(RefreshProperties.All)]
        public int MinWidth
        {
            get => 
                this.minWidth;
            set
            {
                if (value < 0)
                {
                    object[] args = new object[] { "MinWidth", value.ToString(CultureInfo.CurrentCulture), 0.ToString(CultureInfo.CurrentCulture) };
                    throw new ArgumentOutOfRangeException("MinWidth", System.Windows.Forms.SR.GetString("InvalidLowBoundArgumentEx", args));
                }
                if (value != this.minWidth)
                {
                    this.minWidth = value;
                    this.UpdateSize();
                    if (this.minWidth > this.Width)
                    {
                        this.Width = value;
                    }
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), Localizable(true), System.Windows.Forms.SRDescription("StatusBarPanelNameDescr")]
        public string Name
        {
            get => 
                WindowsFormsUtils.GetComponentName(this, this.name);
            set
            {
                this.name = value;
                if (this.Site != null)
                {
                    this.Site.Name = this.name;
                }
            }
        }

        [Browsable(false)]
        public StatusBar Parent =>
            this.parent;

        internal StatusBar ParentInternal
        {
            set
            {
                this.parent = value;
            }
        }

        internal int Right
        {
            get => 
                this.right;
            set
            {
                this.right = value;
            }
        }

        [DefaultValue(1), System.Windows.Forms.SRCategory("CatAppearance"), System.Windows.Forms.SRDescription("StatusBarPanelStyleDescr")]
        public StatusBarPanelStyle Style
        {
            get => 
                this.style;
            set
            {
                if (!System.Windows.Forms.ClientUtils.IsEnumValid(value, (int) value, 1, 2))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(StatusBarPanelStyle));
                }
                if (this.style != value)
                {
                    this.style = value;
                    this.Realize();
                    if (this.Created)
                    {
                        this.parent.Invalidate();
                    }
                }
            }
        }

        [System.Windows.Forms.SRDescription("ControlTagDescr"), Localizable(false), Bindable(true), System.Windows.Forms.SRCategory("CatData"), DefaultValue((string) null), TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get => 
                this.userData;
            set
            {
                this.userData = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), Localizable(true), DefaultValue(""), System.Windows.Forms.SRDescription("StatusBarPanelTextDescr")]
        public string Text
        {
            get
            {
                if (this.text == null)
                {
                    return "";
                }
                return this.text;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if (!this.Text.Equals(value))
                {
                    if (value.Length == 0)
                    {
                        this.text = null;
                    }
                    else
                    {
                        this.text = value;
                    }
                    this.Realize();
                    this.UpdateSize();
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), System.Windows.Forms.SRDescription("StatusBarPanelToolTipTextDescr"), Localizable(true), DefaultValue("")]
        public string ToolTipText
        {
            get
            {
                if (this.toolTipText == null)
                {
                    return "";
                }
                return this.toolTipText;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if (!this.ToolTipText.Equals(value))
                {
                    if (value.Length == 0)
                    {
                        this.toolTipText = null;
                    }
                    else
                    {
                        this.toolTipText = value;
                    }
                    if (this.Created)
                    {
                        this.parent.UpdateTooltip(this);
                    }
                }
            }
        }

        [DefaultValue(100), Localizable(true), System.Windows.Forms.SRCategory("CatAppearance"), System.Windows.Forms.SRDescription("StatusBarPanelWidthDescr")]
        public int Width
        {
            get => 
                this.width;
            set
            {
                if (!this.initializing && (value < this.minWidth))
                {
                    throw new ArgumentOutOfRangeException("Width", System.Windows.Forms.SR.GetString("WidthGreaterThanMinWidth"));
                }
                this.width = value;
                this.UpdateSize();
            }
        }
    }
}

