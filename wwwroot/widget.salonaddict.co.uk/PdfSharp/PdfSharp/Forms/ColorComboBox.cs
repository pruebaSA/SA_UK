namespace PdfSharp.Forms
{
    using PdfSharp.Drawing;
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class ColorComboBox : ComboBox
    {
        private XColor color = XColor.Empty;
        private XColorResourceManager crm = new XColorResourceManager();

        public ColorComboBox()
        {
            base.DropDownStyle = ComboBoxStyle.DropDownList;
            base.DrawMode = DrawMode.OwnerDrawFixed;
            this.Fill();
        }

        private void Fill()
        {
            base.Items.Add(new ColorItem(XColor.Empty, "custom"));
            foreach (XKnownColor color in XColorResourceManager.GetKnownColors(false))
            {
                base.Items.Add(new ColorItem(XColor.FromKnownColor(color), this.crm.ToColorName(color)));
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            int index = e.Index;
            if (index >= 0)
            {
                object obj2 = base.Items[index];
                if (obj2 is ColorItem)
                {
                    ColorItem item = (ColorItem) obj2;
                    if (index == 0)
                    {
                        string str;
                        if (this.color.IsEmpty)
                        {
                            str = "custom";
                        }
                        else
                        {
                            str = this.crm.ToColorName(this.color);
                        }
                        item = new ColorItem(this.color, str);
                    }
                    XColor color = item.Color;
                    Graphics graphics = e.Graphics;
                    Rectangle bounds = e.Bounds;
                    Brush controlText = SystemBrushes.ControlText;
                    if ((e.State & DrawItemState.Selected) == DrawItemState.None)
                    {
                        graphics.FillRectangle(SystemBrushes.Window, bounds);
                        controlText = SystemBrushes.ControlText;
                    }
                    else
                    {
                        graphics.FillRectangle(SystemBrushes.Highlight, bounds);
                        controlText = SystemBrushes.HighlightText;
                    }
                    if (!color.IsEmpty)
                    {
                        Rectangle rect = new Rectangle(bounds.X + 3, bounds.Y + 1, bounds.Height * 2, bounds.Height - 3);
                        graphics.FillRectangle(new SolidBrush(color.ToGdiColor()), rect);
                        graphics.DrawRectangle(Pens.Black, rect);
                    }
                    StringFormat format = new StringFormat(StringFormat.GenericDefault) {
                        Alignment = StringAlignment.Near,
                        LineAlignment = StringAlignment.Center
                    };
                    bounds.X += ((bounds.Height * 2) + 3) + 3;
                    graphics.DrawString(item.Name, this.Font, controlText, bounds, format);
                }
            }
        }

        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            base.DropDownStyle = ComboBoxStyle.DropDownList;
            base.OnDropDownStyleChanged(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            int selectedIndex = this.SelectedIndex;
            if (selectedIndex > 0)
            {
                ColorItem item = (ColorItem) base.Items[selectedIndex];
                this.color = item.Color;
            }
            base.OnSelectedIndexChanged(e);
        }

        public XColor Color
        {
            get => 
                this.color;
            set
            {
                this.color = value;
                if (value.IsKnownColor)
                {
                    XColorResourceManager.GetKnownColor(value.Argb);
                    for (int i = 1; i < base.Items.Count; i++)
                    {
                        if (((ColorItem) base.Items[i]).Color.Argb == value.Argb)
                        {
                            this.SelectedIndex = i;
                            break;
                        }
                    }
                }
                else
                {
                    this.SelectedIndex = 0;
                }
                base.Invalidate();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ColorItem
        {
            public XColor Color;
            public string Name;
            public ColorItem(XColor color, string name)
            {
                this.Color = color;
                this.Name = name;
            }

            public override string ToString() => 
                this.Name;
        }
    }
}

