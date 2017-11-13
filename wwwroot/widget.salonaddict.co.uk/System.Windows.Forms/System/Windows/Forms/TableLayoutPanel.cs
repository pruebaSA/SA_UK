namespace System.Windows.Forms
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms.Layout;

    [ProvideProperty("ColumnSpan", typeof(Control)), Docking(DockingBehavior.Never), System.Windows.Forms.SRDescription("DescriptionTableLayoutPanel"), ProvideProperty("Row", typeof(Control)), ProvideProperty("Column", typeof(Control)), ProvideProperty("CellPosition", typeof(Control)), DefaultProperty("ColumnCount"), DesignerSerializer("System.Windows.Forms.Design.TableLayoutPanelCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ProvideProperty("RowSpan", typeof(Control)), Designer("System.Windows.Forms.Design.TableLayoutPanelDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class TableLayoutPanel : Panel, IExtenderProvider
    {
        private TableLayoutSettings _tableLayoutSettings;
        private static readonly object EventCellPaint = new object();

        [System.Windows.Forms.SRDescription("TableLayoutPanelOnPaintCellDescr"), System.Windows.Forms.SRCategory("CatAppearance")]
        public event TableLayoutCellPaintEventHandler CellPaint
        {
            add
            {
                base.Events.AddHandler(EventCellPaint, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventCellPaint, value);
            }
        }

        public TableLayoutPanel()
        {
            this._tableLayoutSettings = TableLayout.CreateSettings(this);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance() => 
            new TableLayoutControlCollection(this);

        [System.Windows.Forms.SRDescription("GridPanelCellPositionDescr"), System.Windows.Forms.SRCategory("CatLayout"), DisplayName("Cell"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(-1)]
        public TableLayoutPanelCellPosition GetCellPosition(Control control) => 
            this._tableLayoutSettings.GetCellPosition(control);

        [System.Windows.Forms.SRCategory("CatLayout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DisplayName("Column"), DefaultValue(-1), System.Windows.Forms.SRDescription("GridPanelColumnDescr")]
        public int GetColumn(Control control) => 
            this._tableLayoutSettings.GetColumn(control);

        [DefaultValue(1), System.Windows.Forms.SRDescription("GridPanelGetColumnSpanDescr"), System.Windows.Forms.SRCategory("CatLayout"), DisplayName("ColumnSpan")]
        public int GetColumnSpan(Control control) => 
            this._tableLayoutSettings.GetColumnSpan(control);

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int[] GetColumnWidths()
        {
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
            if (containerInfo.Columns == null)
            {
                return new int[0];
            }
            int[] numArray = new int[containerInfo.Columns.Length];
            for (int i = 0; i < containerInfo.Columns.Length; i++)
            {
                numArray[i] = containerInfo.Columns[i].MinSize;
            }
            return numArray;
        }

        public Control GetControlFromPosition(int column, int row) => 
            ((Control) this._tableLayoutSettings.GetControlFromPosition(column, row));

        public TableLayoutPanelCellPosition GetPositionFromControl(Control control) => 
            this._tableLayoutSettings.GetPositionFromControl(control);

        [System.Windows.Forms.SRDescription("GridPanelRowDescr"), DefaultValue(-1), DisplayName("Row"), System.Windows.Forms.SRCategory("CatLayout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int GetRow(Control control) => 
            this._tableLayoutSettings.GetRow(control);

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public int[] GetRowHeights()
        {
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
            if (containerInfo.Rows == null)
            {
                return new int[0];
            }
            int[] numArray = new int[containerInfo.Rows.Length];
            for (int i = 0; i < containerInfo.Rows.Length; i++)
            {
                numArray[i] = containerInfo.Rows[i].MinSize;
            }
            return numArray;
        }

        [System.Windows.Forms.SRDescription("GridPanelGetRowSpanDescr"), System.Windows.Forms.SRCategory("CatLayout"), DisplayName("RowSpan"), DefaultValue(1)]
        public int GetRowSpan(Control control) => 
            this._tableLayoutSettings.GetRowSpan(control);

        protected virtual void OnCellPaint(TableLayoutCellPaintEventArgs e)
        {
            TableLayoutCellPaintEventHandler handler = (TableLayoutCellPaintEventHandler) base.Events[EventCellPaint];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            base.Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            int cellBorderWidth = this.CellBorderWidth;
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
            TableLayout.Strip[] columns = containerInfo.Columns;
            TableLayout.Strip[] rows = containerInfo.Rows;
            TableLayoutPanelCellBorderStyle cellBorderStyle = this.CellBorderStyle;
            if ((columns != null) && (rows != null))
            {
                int num6;
                int length = columns.Length;
                int num3 = rows.Length;
                int num4 = 0;
                int num5 = 0;
                Graphics g = e.Graphics;
                Rectangle displayRectangle = this.DisplayRectangle;
                Rectangle clipRectangle = e.ClipRectangle;
                bool flag = this.RightToLeft == RightToLeft.Yes;
                if (flag)
                {
                    num6 = displayRectangle.Right - (cellBorderWidth / 2);
                }
                else
                {
                    num6 = displayRectangle.X + (cellBorderWidth / 2);
                }
                for (int i = 0; i < length; i++)
                {
                    int y = displayRectangle.Y + (cellBorderWidth / 2);
                    if (flag)
                    {
                        num6 -= columns[i].MinSize;
                    }
                    for (int j = 0; j < num3; j++)
                    {
                        Rectangle bound = new Rectangle(num6, y, columns[i].MinSize, rows[j].MinSize);
                        Rectangle rect = new Rectangle(bound.X + ((cellBorderWidth + 1) / 2), bound.Y + ((cellBorderWidth + 1) / 2), bound.Width - ((cellBorderWidth + 1) / 2), bound.Height - ((cellBorderWidth + 1) / 2));
                        if (clipRectangle.IntersectsWith(rect))
                        {
                            TableLayoutCellPaintEventArgs args = new TableLayoutCellPaintEventArgs(g, clipRectangle, rect, i, j);
                            this.OnCellPaint(args);
                            ControlPaint.PaintTableCellBorder(cellBorderStyle, g, bound);
                        }
                        y += rows[j].MinSize;
                        if (i == 0)
                        {
                            num5 += rows[j].MinSize;
                        }
                    }
                    if (!flag)
                    {
                        num6 += columns[i].MinSize;
                    }
                    num4 += columns[i].MinSize;
                }
                if ((base.HScroll || base.VScroll) || (cellBorderStyle == TableLayoutPanelCellBorderStyle.None))
                {
                    ControlPaint.PaintTableControlBorder(cellBorderStyle, g, displayRectangle);
                }
                else
                {
                    Rectangle rectangle5 = new Rectangle((cellBorderWidth / 2) + displayRectangle.X, (cellBorderWidth / 2) + displayRectangle.Y, displayRectangle.Width - cellBorderWidth, displayRectangle.Height - cellBorderWidth);
                    switch (cellBorderStyle)
                    {
                        case TableLayoutPanelCellBorderStyle.Inset:
                            g.DrawLine(SystemPens.ControlDark, rectangle5.Right, rectangle5.Y, rectangle5.Right, rectangle5.Bottom);
                            g.DrawLine(SystemPens.ControlDark, rectangle5.X, (rectangle5.Y + rectangle5.Height) - 1, (rectangle5.X + rectangle5.Width) - 1, (rectangle5.Y + rectangle5.Height) - 1);
                            break;

                        case TableLayoutPanelCellBorderStyle.Outset:
                        {
                            using (Pen pen = new Pen(SystemColors.Window))
                            {
                                g.DrawLine(pen, (rectangle5.X + rectangle5.Width) - 1, rectangle5.Y, (rectangle5.X + rectangle5.Width) - 1, (rectangle5.Y + rectangle5.Height) - 1);
                                g.DrawLine(pen, rectangle5.X, (rectangle5.Y + rectangle5.Height) - 1, (rectangle5.X + rectangle5.Width) - 1, (rectangle5.Y + rectangle5.Height) - 1);
                                break;
                            }
                        }
                        default:
                            ControlPaint.PaintTableCellBorder(cellBorderStyle, g, rectangle5);
                            break;
                    }
                    ControlPaint.PaintTableControlBorder(cellBorderStyle, g, displayRectangle);
                }
            }
        }

        private void ScaleAbsoluteStyles(SizeF factor)
        {
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(this);
            int num = 0;
            int minSize = -1;
            int index = containerInfo.Rows.Length - 1;
            if (containerInfo.Rows.Length > 0)
            {
                minSize = containerInfo.Rows[index].MinSize;
            }
            int num4 = -1;
            int num5 = containerInfo.Columns.Length - 1;
            if (containerInfo.Columns.Length > 0)
            {
                num4 = containerInfo.Columns[containerInfo.Columns.Length - 1].MinSize;
            }
            foreach (ColumnStyle style in (IEnumerable) this.ColumnStyles)
            {
                if (style.SizeType == SizeType.Absolute)
                {
                    if ((num == num5) && (num4 > 0))
                    {
                        style.Width = (float) Math.Round((double) (num4 * factor.Width));
                    }
                    else
                    {
                        style.Width = (float) Math.Round((double) (style.Width * factor.Width));
                    }
                }
                num++;
            }
            num = 0;
            foreach (RowStyle style2 in (IEnumerable) this.RowStyles)
            {
                if (style2.SizeType == SizeType.Absolute)
                {
                    if ((num == index) && (minSize > 0))
                    {
                        style2.Height = (float) Math.Round((double) (minSize * factor.Height));
                    }
                    else
                    {
                        style2.Height = (float) Math.Round((double) (style2.Height * factor.Height));
                    }
                }
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            base.ScaleControl(factor, specified);
            this.ScaleAbsoluteStyles(factor);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float dx, float dy)
        {
            base.ScaleCore(dx, dy);
            this.ScaleAbsoluteStyles(new SizeF(dx, dy));
        }

        public void SetCellPosition(Control control, TableLayoutPanelCellPosition position)
        {
            this._tableLayoutSettings.SetCellPosition(control, position);
        }

        public void SetColumn(Control control, int column)
        {
            this._tableLayoutSettings.SetColumn(control, column);
        }

        public void SetColumnSpan(Control control, int value)
        {
            this._tableLayoutSettings.SetColumnSpan(control, value);
        }

        public void SetRow(Control control, int row)
        {
            this._tableLayoutSettings.SetRow(control, row);
        }

        public void SetRowSpan(Control control, int value)
        {
            this._tableLayoutSettings.SetRowSpan(control, value);
        }

        private bool ShouldSerializeControls()
        {
            TableLayoutControlCollection controls = this.Controls;
            return ((controls != null) && (controls.Count > 0));
        }

        bool IExtenderProvider.CanExtend(object obj)
        {
            Control control = obj as Control;
            return ((control != null) && (control.Parent == this));
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Localizable(true)]
        public System.Windows.Forms.BorderStyle BorderStyle
        {
            get => 
                base.BorderStyle;
            set
            {
                base.BorderStyle = value;
            }
        }

        [Localizable(true), System.Windows.Forms.SRDescription("TableLayoutPanelCellBorderStyleDescr"), DefaultValue(0), System.Windows.Forms.SRCategory("CatAppearance")]
        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get => 
                this._tableLayoutSettings.CellBorderStyle;
            set
            {
                this._tableLayoutSettings.CellBorderStyle = value;
                if (value != TableLayoutPanelCellBorderStyle.None)
                {
                    base.SetStyle(ControlStyles.ResizeRedraw, true);
                }
                base.Invalidate();
            }
        }

        private int CellBorderWidth =>
            this._tableLayoutSettings.CellBorderWidth;

        [Localizable(true), System.Windows.Forms.SRDescription("GridPanelColumnsDescr"), System.Windows.Forms.SRCategory("CatLayout"), DefaultValue(0)]
        public int ColumnCount
        {
            get => 
                this._tableLayoutSettings.ColumnCount;
            set
            {
                this._tableLayoutSettings.ColumnCount = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), System.Windows.Forms.SRDescription("GridPanelColumnStylesDescr"), Browsable(false), System.Windows.Forms.SRCategory("CatLayout"), DisplayName("Columns"), MergableProperty(false)]
        public TableLayoutColumnStyleCollection ColumnStyles =>
            this._tableLayoutSettings.ColumnStyles;

        [System.Windows.Forms.SRDescription("ControlControlsDescr"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TableLayoutControlCollection Controls =>
            ((TableLayoutControlCollection) base.Controls);

        [System.Windows.Forms.SRDescription("TableLayoutPanelGrowStyleDescr"), DefaultValue(1), System.Windows.Forms.SRCategory("CatLayout")]
        public TableLayoutPanelGrowStyle GrowStyle
        {
            get => 
                this._tableLayoutSettings.GrowStyle;
            set
            {
                this._tableLayoutSettings.GrowStyle = value;
            }
        }

        public override System.Windows.Forms.Layout.LayoutEngine LayoutEngine =>
            TableLayout.Instance;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public TableLayoutSettings LayoutSettings
        {
            get => 
                this._tableLayoutSettings;
            set
            {
                if ((value != null) && value.IsStub)
                {
                    using (new LayoutTransaction(this, this, PropertyNames.LayoutSettings))
                    {
                        this._tableLayoutSettings.ApplySettings(value);
                        return;
                    }
                }
                throw new NotSupportedException(System.Windows.Forms.SR.GetString("TableLayoutSettingSettingsIsNotSupported"));
            }
        }

        [System.Windows.Forms.SRDescription("GridPanelRowsDescr"), Localizable(true), System.Windows.Forms.SRCategory("CatLayout"), DefaultValue(0)]
        public int RowCount
        {
            get => 
                this._tableLayoutSettings.RowCount;
            set
            {
                this._tableLayoutSettings.RowCount = value;
            }
        }

        [System.Windows.Forms.SRDescription("GridPanelRowStylesDescr"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), System.Windows.Forms.SRCategory("CatLayout"), DisplayName("Rows"), MergableProperty(false)]
        public TableLayoutRowStyleCollection RowStyles =>
            this._tableLayoutSettings.RowStyles;
    }
}

