namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    internal class ComboBoxDesignerActionList : DesignerActionList
    {
        private ComboBox _comboBox;

        public ComboBoxDesignerActionList(IComponent component) : base(component)
        {
            this._comboBox = (ComboBox) component;
        }

        protected virtual DesignerActionPropertyItem GetPropertyItem(string propertyName, string displayName)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this._comboBox)[propertyName];
            if ((descriptor != null) && descriptor.IsBrowsable)
            {
                return new DesignerActionPropertyItem(propertyName, displayName, descriptor.Category, descriptor.Description);
            }
            return null;
        }

        public override DesignerActionItemCollection GetSortedActionItems()
        {
            DesignerActionItemCollection items = new DesignerActionItemCollection();
            DesignerActionPropertyItem propertyItem = this.GetPropertyItem("AppendDataBoundItems", "Append DataBound Items");
            if (propertyItem != null)
            {
                items.Add(propertyItem);
            }
            propertyItem = this.GetPropertyItem("CaseSensitive", "Case Sensitive");
            if (propertyItem != null)
            {
                items.Add(propertyItem);
            }
            propertyItem = this.GetPropertyItem("DropDownStyle", "DropDown Style");
            if (propertyItem != null)
            {
                items.Add(propertyItem);
            }
            propertyItem = this.GetPropertyItem("AutoCompleteMode", "AutoComplete Mode");
            if (propertyItem != null)
            {
                items.Add(propertyItem);
            }
            return items;
        }

        protected virtual void SetComponentProperty(string propertyName, object value)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(this._comboBox)[propertyName];
            if (descriptor == null)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
            descriptor.SetValue(this._comboBox, value);
        }

        public bool AppendDataBoundItems
        {
            get => 
                this._comboBox.AppendDataBoundItems;
            set
            {
                this.SetComponentProperty("AppendDataBoundItems", value);
            }
        }

        public ComboBoxAutoCompleteMode AutoCompleteMode
        {
            get => 
                this._comboBox.AutoCompleteMode;
            set
            {
                this.SetComponentProperty("AutoCompleteMode", value);
            }
        }

        public bool CaseSensitive
        {
            get => 
                this._comboBox.CaseSensitive;
            set
            {
                this.SetComponentProperty("CaseSensitive", value);
            }
        }

        public ComboBoxStyle DropDownStyle
        {
            get => 
                this._comboBox.DropDownStyle;
            set
            {
                this.SetComponentProperty("DropDownStyle", value);
            }
        }
    }
}

