﻿namespace System.Windows.Forms.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms;

    internal class DataGridTableStyleMappingNameEditor : UITypeEditor
    {
        private DesignBindingPicker designBindingPicker;

        private DataGridTableStyleMappingNameEditor()
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((provider != null) && (context.Instance != null))
            {
                object instance = context.Instance;
                DataGridTableStyle style = (DataGridTableStyle) context.Instance;
                if (style.DataGrid == null)
                {
                    return value;
                }
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(style.DataGrid)["DataSource"];
                if (descriptor == null)
                {
                    return value;
                }
                object dataSource = descriptor.GetValue(style.DataGrid);
                if (this.designBindingPicker == null)
                {
                    this.designBindingPicker = new DesignBindingPicker();
                }
                DesignBinding initialSelectedItem = new DesignBinding(dataSource, (string) value);
                DesignBinding binding2 = this.designBindingPicker.Pick(context, provider, false, true, true, dataSource, string.Empty, initialSelectedItem);
                if ((dataSource == null) || (binding2 == null))
                {
                    return value;
                }
                if (string.IsNullOrEmpty(binding2.DataMember) || (binding2.DataMember == null))
                {
                    value = "";
                    return value;
                }
                value = binding2.DataField;
            }
            return value;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => 
            UITypeEditorEditStyle.DropDown;

        public override bool IsDropDownResizable =>
            true;
    }
}

