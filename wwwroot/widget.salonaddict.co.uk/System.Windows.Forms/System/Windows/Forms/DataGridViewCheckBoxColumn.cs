﻿namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Text;

    [ToolboxBitmap(typeof(DataGridViewCheckBoxColumn), "DataGridViewCheckBoxColumn.bmp")]
    public class DataGridViewCheckBoxColumn : DataGridViewColumn
    {
        public DataGridViewCheckBoxColumn() : this(false)
        {
        }

        public DataGridViewCheckBoxColumn(bool threeState) : base(new DataGridViewCheckBoxCell(threeState))
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle {
                AlignmentInternal = DataGridViewContentAlignment.MiddleCenter
            };
            if (threeState)
            {
                style.NullValue = CheckState.Indeterminate;
            }
            else
            {
                style.NullValue = false;
            }
            this.DefaultCellStyle = style;
        }

        private bool ShouldSerializeDefaultCellStyle()
        {
            DataGridViewCheckBoxCell cellTemplate = this.CellTemplate as DataGridViewCheckBoxCell;
            if (cellTemplate != null)
            {
                object indeterminate;
                if (cellTemplate.ThreeState)
                {
                    indeterminate = CheckState.Indeterminate;
                }
                else
                {
                    indeterminate = false;
                }
                if (!base.HasDefaultCellStyle)
                {
                    return false;
                }
                DataGridViewCellStyle defaultCellStyle = this.DefaultCellStyle;
                if ((((defaultCellStyle.BackColor.IsEmpty && defaultCellStyle.ForeColor.IsEmpty) && (defaultCellStyle.SelectionBackColor.IsEmpty && defaultCellStyle.SelectionForeColor.IsEmpty)) && (((defaultCellStyle.Font == null) && defaultCellStyle.NullValue.Equals(indeterminate)) && (defaultCellStyle.IsDataSourceNullValueDefault && string.IsNullOrEmpty(defaultCellStyle.Format)))) && ((defaultCellStyle.FormatProvider.Equals(CultureInfo.CurrentCulture) && (defaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter)) && ((defaultCellStyle.WrapMode == DataGridViewTriState.NotSet) && (defaultCellStyle.Tag == null))))
                {
                    return !defaultCellStyle.Padding.Equals(Padding.Empty);
                }
            }
            return true;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(0x40);
            builder.Append("DataGridViewCheckBoxColumn { Name=");
            builder.Append(base.Name);
            builder.Append(", Index=");
            builder.Append(base.Index.ToString(CultureInfo.CurrentCulture));
            builder.Append(" }");
            return builder.ToString();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override DataGridViewCell CellTemplate
        {
            get => 
                base.CellTemplate;
            set
            {
                if ((value != null) && !(value is DataGridViewCheckBoxCell))
                {
                    throw new InvalidCastException(System.Windows.Forms.SR.GetString("DataGridViewTypeColumn_WrongCellTemplateType", new object[] { "System.Windows.Forms.DataGridViewCheckBoxCell" }));
                }
                base.CellTemplate = value;
            }
        }

        private DataGridViewCheckBoxCell CheckBoxCellTemplate =>
            ((DataGridViewCheckBoxCell) this.CellTemplate);

        [System.Windows.Forms.SRDescription("DataGridView_ColumnDefaultCellStyleDescr"), Browsable(true), System.Windows.Forms.SRCategory("CatAppearance")]
        public override DataGridViewCellStyle DefaultCellStyle
        {
            get => 
                base.DefaultCellStyle;
            set
            {
                base.DefaultCellStyle = value;
            }
        }

        [System.Windows.Forms.SRDescription("DataGridView_CheckBoxColumnFalseValueDescr"), System.Windows.Forms.SRCategory("CatData"), TypeConverter(typeof(StringConverter)), DefaultValue((string) null)]
        public object FalseValue
        {
            get => 
                this.CheckBoxCellTemplate?.FalseValue;
            set
            {
                if (this.FalseValue != value)
                {
                    this.CheckBoxCellTemplate.FalseValueInternal = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewCheckBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewCheckBoxCell;
                            if (cell != null)
                            {
                                cell.FalseValueInternal = value;
                            }
                        }
                        base.DataGridView.InvalidateColumn(base.Index);
                    }
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatAppearance"), DefaultValue(2), System.Windows.Forms.SRDescription("DataGridView_CheckBoxColumnFlatStyleDescr")]
        public System.Windows.Forms.FlatStyle FlatStyle
        {
            get => 
                this.CheckBoxCellTemplate?.FlatStyle;
            set
            {
                if (this.FlatStyle != value)
                {
                    this.CheckBoxCellTemplate.FlatStyle = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewCheckBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewCheckBoxCell;
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

        [System.Windows.Forms.SRDescription("DataGridView_CheckBoxColumnIndeterminateValueDescr"), System.Windows.Forms.SRCategory("CatData"), TypeConverter(typeof(StringConverter)), DefaultValue((string) null)]
        public object IndeterminateValue
        {
            get => 
                this.CheckBoxCellTemplate?.IndeterminateValue;
            set
            {
                if (this.IndeterminateValue != value)
                {
                    this.CheckBoxCellTemplate.IndeterminateValueInternal = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewCheckBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewCheckBoxCell;
                            if (cell != null)
                            {
                                cell.IndeterminateValueInternal = value;
                            }
                        }
                        base.DataGridView.InvalidateColumn(base.Index);
                    }
                }
            }
        }

        [System.Windows.Forms.SRCategory("CatBehavior"), DefaultValue(false), System.Windows.Forms.SRDescription("DataGridView_CheckBoxColumnThreeStateDescr")]
        public bool ThreeState
        {
            get => 
                this.CheckBoxCellTemplate?.ThreeState;
            set
            {
                if (this.ThreeState != value)
                {
                    this.CheckBoxCellTemplate.ThreeStateInternal = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewCheckBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewCheckBoxCell;
                            if (cell != null)
                            {
                                cell.ThreeStateInternal = value;
                            }
                        }
                        base.DataGridView.InvalidateColumn(base.Index);
                    }
                    if ((value && (this.DefaultCellStyle.NullValue is bool)) && !((bool) this.DefaultCellStyle.NullValue))
                    {
                        this.DefaultCellStyle.NullValue = CheckState.Indeterminate;
                    }
                    else if ((!value && (this.DefaultCellStyle.NullValue is CheckState)) && (((CheckState) this.DefaultCellStyle.NullValue) == CheckState.Indeterminate))
                    {
                        this.DefaultCellStyle.NullValue = false;
                    }
                }
            }
        }

        [TypeConverter(typeof(StringConverter)), System.Windows.Forms.SRCategory("CatData"), System.Windows.Forms.SRDescription("DataGridView_CheckBoxColumnTrueValueDescr"), DefaultValue((string) null)]
        public object TrueValue
        {
            get => 
                this.CheckBoxCellTemplate?.TrueValue;
            set
            {
                if (this.TrueValue != value)
                {
                    this.CheckBoxCellTemplate.TrueValueInternal = value;
                    if (base.DataGridView != null)
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for (int i = 0; i < count; i++)
                        {
                            DataGridViewCheckBoxCell cell = rows.SharedRow(i).Cells[base.Index] as DataGridViewCheckBoxCell;
                            if (cell != null)
                            {
                                cell.TrueValueInternal = value;
                            }
                        }
                        base.DataGridView.InvalidateColumn(base.Index);
                    }
                }
            }
        }
    }
}

