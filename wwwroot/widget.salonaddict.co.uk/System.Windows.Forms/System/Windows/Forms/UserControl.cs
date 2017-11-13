namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms.Layout;

    [ComVisible(true), Designer("System.Windows.Forms.Design.UserControlDocumentDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(IRootDesigner)), ClassInterface(ClassInterfaceType.AutoDispatch), Designer("System.Windows.Forms.Design.ControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DesignerCategory("UserControl"), DefaultEvent("Load")]
    public class UserControl : ContainerControl
    {
        private System.Windows.Forms.BorderStyle borderStyle;
        private static readonly object EVENT_LOAD = new object();

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
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

        [EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
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

        [System.Windows.Forms.SRDescription("UserControlOnLoadDescr"), System.Windows.Forms.SRCategory("CatBehavior")]
        public event EventHandler Load
        {
            add
            {
                base.Events.AddHandler(EVENT_LOAD, value);
            }
            remove
            {
                base.Events.RemoveHandler(EVENT_LOAD, value);
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

        public UserControl()
        {
            base.SetScrollState(1, false);
            base.SetState(2, true);
            base.SetState(0x80000, false);
            base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private bool FocusInside()
        {
            if (!base.IsHandleCreated)
            {
                return false;
            }
            IntPtr focus = System.Windows.Forms.UnsafeNativeMethods.GetFocus();
            if (focus == IntPtr.Zero)
            {
                return false;
            }
            IntPtr handle = base.Handle;
            if (!(handle == focus) && !System.Windows.Forms.SafeNativeMethods.IsChild(new HandleRef(this, handle), new HandleRef(null, focus)))
            {
                return false;
            }
            return true;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            this.OnLoad(EventArgs.Empty);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLoad(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EVENT_LOAD];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.FocusInside())
            {
                this.FocusInternal();
            }
            base.OnMouseDown(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.BackgroundImage != null)
            {
                base.Invalidate();
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren() => 
            base.ValidateChildren();

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren(ValidationConstraints validationConstraints) => 
            base.ValidateChildren(validationConstraints);

        private void WmSetFocus(ref Message m)
        {
            if (!base.HostedInWin32DialogManager)
            {
                System.Windows.Forms.IntSecurity.ModifyFocus.Assert();
                try
                {
                    if (base.ActiveControl == null)
                    {
                        base.SelectNextControl(null, true, true, true, false);
                    }
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }
            if (!base.ValidationCancelled)
            {
                base.WndProc(ref m);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 7)
            {
                this.WmSetFocus(ref m);
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get => 
                base.AutoSize;
            set
            {
                base.AutoSize = value;
            }
        }

        [Browsable(true), System.Windows.Forms.SRCategory("CatLayout"), System.Windows.Forms.SRDescription("ControlAutoSizeModeDescr"), DefaultValue(1), Localizable(true)]
        public System.Windows.Forms.AutoSizeMode AutoSizeMode
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
                    Control elementToLayout = (base.DesignMode || (this.ParentInternal == null)) ? this : this.ParentInternal;
                    if (elementToLayout != null)
                    {
                        if (elementToLayout.LayoutEngine == DefaultLayout.Instance)
                        {
                            elementToLayout.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(elementToLayout, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override System.Windows.Forms.AutoValidate AutoValidate
        {
            get => 
                base.AutoValidate;
            set
            {
                base.AutoValidate = value;
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(0), System.Windows.Forms.SRDescription("UserControlBorderStyleDescr"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
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
            new Size(150, 150);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

