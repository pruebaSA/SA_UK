﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Text;

    [Designer("System.Windows.Forms.Design.DataGridViewComboBoxColumnDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxBitmap(typeof(DataGridViewComboBoxColumn), "DataGridViewComboBoxColumn.bmp")]
    public class DataGridViewComboBoxColumn : DataGridViewColumn
    {
        private static System.Type columnType = typeof(DataGridViewComboBoxColumn);

        public DataGridViewComboBoxColumn() : base(new DataGridViewComboBoxCell())
        {
            ((DataGridViewComboBoxCell) base.CellTemplate).TemplateComboBoxColumn = this;
        }

        public override object Clone()
        {
            DataGridViewComboBoxColumn column;
            System.Type type = base.GetType();
            if (type == columnType)
            {
                column = new DataGridViewComboBoxColumn();
            }
            else
            {
                column = (DataGridViewComboBoxColumn) Activator.CreateInstance(type);
            }
            if (column != null)
            {
                base.CloneInternal(column);
                ((DataGridViewComboBoxCell) column.CellTemplate).TemplateComboBoxColumn = column;
            }
            return column;
        }

        internal void OnItemsCollectionChanged()
        {
            if (base.DataGridView != null)
            {
                DataGridViewRowCollection rows = base.DataGridView.Rows;
                int count = rows.Count;
                object[] items = ((DataGridViewComboBoxCell) this.CellTemplate).Items.InnerArray.ToArray();
                for (int i = 0; i < count; i++)
                {
                    DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                    if (cell != null)
                    {
                        cell.Items.ClearInternal();
                        cell.Items.AddRangeInternal(items);
                    }
                }
                base.DataGridView.OnColumnCommonChange(base.Index);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append("DataGridViewComboBoxColumn { Name=");
            builder.Append(base.Name);
            builder.Append(", Index=");
            builder.Append(base.Index.ToString(CultureInfo.CurrentCulture));
            builder.Append(" }");
            return builder.ToString();
        }

        [DefaultValue(true), Browsable(true), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnAutoCompleteDescr"), System.Windows.Forms.SRCategory("CatBehavior")]
        public bool AutoComplete
        {
            get => 
                this.ComboBoxCellTemplate?.AutoComplete;
            set
            {
                if (this.AutoComplete != value)
                {
                    this.ComboBoxCellTemplate.AutoComplete = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                            if (cell != null)
                            {
                                cell.AutoComplete = value;
                            }
                        }
                    }
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DataGridViewCell CellTemplate
        {
            get => 
                base.CellTemplate;
            set
            {
                DataGridViewComboBoxCell cell = value as DataGridViewComboBoxCell;
                if ((value != null) && (cell == null))
                {
                    throw new InvalidCastException(System.Windows.Forms.SR.GetString("DataGridViewTypeColumn_WrongCellTemplateType", new object[] { "System.Windows.Forms.DataGridViewComboBoxCell" }));
                }
                base.CellTemplate = value;
                if (value != null)
                {
                    cell.TemplateComboBoxColumn = this;
                }
            }
        }

        private DataGridViewComboBoxCell ComboBoxCellTemplate =>
            ((DataGridViewComboBoxCell) this.CellTemplate);

        [AttributeProvider(typeof(IListSource)), DefaultValue((string) null), System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnDataSourceDescr"), RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get => 
                this.ComboBoxCellTemplate?.DataSource;
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewColumn_CellTemplateRequired"));
                }
                this.ComboBoxCellTemplate.DataSource = value;
                if (base.DataGridView != null)
                {
                    DataGridViewRowCollection rows = base.DataGridView.Rows;
                    int count = rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            cell.DataSource = value;
                        }
                    }
                    base.DataGridView.OnColumnCommonChange(base.Index);
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatData"), Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnDisplayMemberDescr"), TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string DisplayMember
        {
            get => 
                this.ComboBoxCellTemplate?.DisplayMember;
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewColumn_CellTemplateRequired"));
                }
                this.ComboBoxCellTemplate.DisplayMember = value;
                if (base.DataGridView != null)
                {
                    DataGridViewRowCollection rows = base.DataGridView.Rows;
                    int count = rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            cell.DisplayMember = value;
                        }
                    }
                    base.DataGridView.OnColumnCommonChange(base.Index);
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(1), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnDisplayStyleDescr")]
        public DataGridViewComboBoxDisplayStyle DisplayStyle
        {
            get => 
                this.ComboBoxCellTemplate?.DisplayStyle;
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewColumn_CellTemplateRequired"));
                }
                this.ComboBoxCellTemplate.DisplayStyle = value;
                if (base.DataGridView != null)
                {
                    DataGridViewRowCollection rows = base.DataGridView.Rows;
                    int count = rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            cell.DisplayStyleInternal = value;
                        }
                    }
                    base.DataGridView.InvalidateColumn(base.Index);
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(false), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnDisplayStyleForCurrentCellOnlyDescr")]
        public bool DisplayStyleForCurrentCellOnly
        {
            get => 
                this.ComboBoxCellTemplate?.DisplayStyleForCurrentCellOnly;
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewColumn_CellTemplateRequired"));
                }
                this.ComboBoxCellTemplate.DisplayStyleForCurrentCellOnly = value;
                if (base.DataGridView != null)
                {
                    DataGridViewRowCollection rows = base.DataGridView.Rows;
                    int count = rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            cell.DisplayStyleForCurrentCellOnlyInternal = value;
                        }
                    }
                    base.DataGridView.InvalidateColumn(base.Index);
                }
            }
        }

        [DefaultValue(1), System.Windows.Forms.SRCategory("CatBehavior"), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnDropDownWidthDescr")]
        public int DropDownWidth
        {
            get => 
                this.ComboBoxCellTemplate?.DropDownWidth;
            set
            {
                if (this.DropDownWidth != value)
                {
                    this.ComboBoxCellTemplate.DropDownWidth = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                            if (cell != null)
                            {
                                cell.DropDownWidth = value;
                            }
                        }
                    }
                }
            }
        }

        [System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnFlatStyleDescr"), System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(2)]
        public System.Windows.Forms.FlatStyle FlatStyle
        {
            get
            {
                if (this.CellTemplate == null)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewColumn_CellTemplateRequired"));
                }
                return ((DataGridViewComboBoxCell) this.CellTemplate).FlatStyle;
            }
            set
            {
                if (this.FlatStyle != value)
                {
                    ((DataGridViewComboBoxCell) this.CellTemplate).FlatStyle = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                            if (cell != null)
                            {
                                cell.FlatStyleInternal = value;
                            }
                        }
                        base.DataGridView.OnColumnCommonChange(base.Index);
                    }
                }
            }
        }

        [System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnItemsDescr"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), System.Windows.Forms.SRCategory("CatData"), Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public DataGridViewComboBoxCell.ObjectCollection Items =>
            this.ComboBoxCellTemplate?.GetItems(base.DataGridView);

        [System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnMaxDropDownItemsDescr"), System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(8)]
        public int MaxDropDownItems
        {
            get => 
                this.ComboBoxCellTemplate?.MaxDropDownItems;
            set
            {
                if (this.MaxDropDownItems != value)
                {
                    this.ComboBoxCellTemplate.MaxDropDownItems = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                            if (cell != null)
                            {
                                cell.MaxDropDownItems = value;
                            }
                        }
                    }
                }
            }
        }

        [DefaultValue(false), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnSortedDescr"), System.Windows.Forms.SRCategory("CatBehavior")]
        public bool Sorted
        {
            get => 
                this.ComboBoxCellTemplate?.Sorted;
            set
            {
                if (this.Sorted != value)
                {
                    this.ComboBoxCellTemplate.Sorted = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                            if (cell != null)
                            {
                                cell.Sorted = value;
                            }
                        }
                    }
                }
            }
        }

        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), System.Windows.Forms.SRDescription("DataGridView_ComboBoxColumnValueMemberDescr"), TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultValue(""), System.Windows.Forms.SRCategory("CatData")]
        public string ValueMember
        {
            get => 
                this.ComboBoxCellTemplate?.ValueMember;
            set
            {
                if (this.ComboBoxCellTemplate == null)
                {
                    throw new InvalidOperationException(System.Windows.Forms.SR.GetString("DataGridViewColumn_CellTemplateRequired"));
                }
                this.ComboBoxCellTemplate.ValueMember = value;
                if (base.DataGridView != null)
                {
                    DataGridViewRowCollection rows = base.DataGridView.Rows;
                    int count = rows.Count;
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewComboBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewComboBoxCell;
                        if (cell != null)
                        {
                            cell.ValueMember = value;
                        }
                    }
                    base.DataGridView.OnColumnCommonChange(base.Index);
                }
            }
        }
    }
}

