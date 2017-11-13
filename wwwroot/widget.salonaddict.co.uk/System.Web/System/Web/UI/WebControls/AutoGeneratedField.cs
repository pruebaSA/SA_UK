﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [EditorBrowsable(EditorBrowsableState.Never), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class AutoGeneratedField : BoundField
    {
        private bool _suppressPropertyThrows;
        private bool _useCheckBox;
        private bool _useCheckBoxValid;

        public AutoGeneratedField(string dataField)
        {
            this.DataField = dataField;
        }

        protected override void CopyProperties(DataControlField newField)
        {
            ((AutoGeneratedField) newField).DataType = this.DataType;
            this._suppressPropertyThrows = true;
            ((AutoGeneratedField) newField)._suppressPropertyThrows = true;
            base.CopyProperties(newField);
            this._suppressPropertyThrows = false;
            ((AutoGeneratedField) newField)._suppressPropertyThrows = false;
        }

        protected override DataControlField CreateField() => 
            new AutoGeneratedField(this.DataField);

        public override void ExtractValuesFromCell(IOrderedDictionary dictionary, DataControlFieldCell cell, DataControlRowState rowState, bool includeReadOnly)
        {
            if (this.UseCheckBox)
            {
                Control control = null;
                string dataField = this.DataField;
                object obj2 = null;
                if (cell.Controls.Count > 0)
                {
                    control = cell.Controls[0];
                    CheckBox box = control as CheckBox;
                    if ((box != null) && (includeReadOnly || box.Enabled))
                    {
                        obj2 = box.Checked;
                    }
                }
                if (obj2 != null)
                {
                    if (dictionary.Contains(dataField))
                    {
                        dictionary[dataField] = obj2;
                    }
                    else
                    {
                        dictionary.Add(dataField, obj2);
                    }
                }
            }
            else
            {
                base.ExtractValuesFromCell(dictionary, cell, rowState, includeReadOnly);
            }
        }

        protected override object GetDesignTimeValue() => 
            (this.UseCheckBox || base.GetDesignTimeValue());

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            if (this.UseCheckBox)
            {
                CheckBox child = null;
                CheckBox box2 = null;
                if ((((rowState & DataControlRowState.Edit) != DataControlRowState.Normal) && !this.ReadOnly) || ((rowState & DataControlRowState.Insert) != DataControlRowState.Normal))
                {
                    CheckBox box3 = new CheckBox();
                    child = box3;
                    if ((this.DataField.Length != 0) && ((rowState & DataControlRowState.Edit) != DataControlRowState.Normal))
                    {
                        box2 = box3;
                    }
                }
                else if (this.DataField.Length != 0)
                {
                    CheckBox box4 = new CheckBox {
                        Enabled = false
                    };
                    child = box4;
                    box2 = box4;
                }
                if (child != null)
                {
                    child.ToolTip = this.HeaderText;
                    cell.Controls.Add(child);
                }
                if (box2 != null)
                {
                    box2.DataBinding += new EventHandler(this.OnDataBindField);
                }
            }
            else
            {
                base.InitializeDataCell(cell, rowState);
            }
        }

        protected override void OnDataBindField(object sender, EventArgs e)
        {
            if (this.UseCheckBox)
            {
                Control control = (Control) sender;
                Control namingContainer = control.NamingContainer;
                object obj2 = this.GetValue(namingContainer);
                if (!(control is CheckBox))
                {
                    throw new HttpException(System.Web.SR.GetString("CheckBoxField_WrongControlType", new object[] { this.DataField }));
                }
                if (DataBinder.IsNull(obj2))
                {
                    ((CheckBox) control).Checked = false;
                    return;
                }
                if (obj2 is bool)
                {
                    ((CheckBox) control).Checked = (bool) obj2;
                    return;
                }
                try
                {
                    ((CheckBox) control).Checked = bool.Parse(obj2.ToString());
                    return;
                }
                catch (FormatException exception)
                {
                    throw new HttpException(System.Web.SR.GetString("CheckBoxField_CouldntParseAsBoolean", new object[] { this.DataField }), exception);
                }
            }
            base.OnDataBindField(sender, e);
        }

        public override void ValidateSupportsCallback()
        {
        }

        public override bool ConvertEmptyStringToNull
        {
            get => 
                base.ConvertEmptyStringToNull;
            set
            {
                if (!this._suppressPropertyThrows)
                {
                    throw new NotSupportedException();
                }
            }
        }

        public override string DataFormatString
        {
            get => 
                base.DataFormatString;
            set
            {
                if (!this._suppressPropertyThrows)
                {
                    throw new NotSupportedException();
                }
            }
        }

        public Type DataType
        {
            get
            {
                object obj2 = base.ViewState["DataType"];
                if (obj2 != null)
                {
                    return (Type) obj2;
                }
                return typeof(string);
            }
            set
            {
                base.ViewState["DataType"] = value;
            }
        }

        public override bool InsertVisible
        {
            get => 
                base.InsertVisible;
            set
            {
                if (!this._suppressPropertyThrows)
                {
                    throw new NotSupportedException();
                }
            }
        }

        private bool UseCheckBox
        {
            get
            {
                if (!this._useCheckBoxValid)
                {
                    this._useCheckBox = (this.DataType == typeof(bool)) || (this.DataType == typeof(bool?));
                    this._useCheckBoxValid = true;
                }
                return this._useCheckBox;
            }
        }
    }
}
