namespace System.Windows.Forms.PropertyGridInternal
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.ButtonInternal;
    using System.Windows.Forms.VisualStyles;

    internal sealed class DropDownButton : System.Windows.Forms.Button
    {
        private bool ignoreMouse;
        private bool useComboBoxTheme;

        public DropDownButton()
        {
            base.SetStyle(ControlStyles.Selectable, true);
            base.AccessibleName = System.Windows.Forms.SR.GetString("PropertyGridDropDownButtonAccessibleName");
        }

        internal override ButtonBaseAdapter CreateStandardAdapter() => 
            new DropDownButtonAdapter(this);

        protected override void OnClick(EventArgs e)
        {
            if (!this.IgnoreMouse)
            {
                base.OnClick(e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!this.IgnoreMouse)
            {
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!this.IgnoreMouse)
            {
                base.OnMouseUp(e);
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);
            if (Application.RenderWithVisualStyles & this.useComboBoxTheme)
            {
                ComboBoxState normal = ComboBoxState.Normal;
                if (base.MouseIsDown)
                {
                    normal = ComboBoxState.Pressed;
                }
                else if (base.MouseIsOver)
                {
                    normal = ComboBoxState.Hot;
                }
                ComboBoxRenderer.DrawDropDownButton(pevent.Graphics, new Rectangle(0, 0, base.Width, base.Height), normal);
            }
        }

        public bool IgnoreMouse
        {
            get => 
                this.ignoreMouse;
            set
            {
                this.ignoreMouse = value;
            }
        }

        public bool UseComboBoxTheme
        {
            set
            {
                if (this.useComboBoxTheme != value)
                {
                    this.useComboBoxTheme = value;
                    base.Invalidate();
                }
            }
        }
    }
}

