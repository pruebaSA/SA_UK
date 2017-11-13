namespace System.Windows.Forms.Design
{
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Windows.Forms;

    internal class DataGridColumnStyleMappingNameEditor : UITypeEditor
    {
        private DesignBindingPicker designBindingPicker;

        private DataGridColumnStyleMappingNameEditor()
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((provider != null) && (context.Instance != null))
            {
                object instance = context.Instance;
                DataGridColumnStyle style = (DataGridColumnStyle) context.Instance;
                if ((style.DataGridTableStyle == null) || (style.DataGridTableStyle.DataGrid == null))
                {
                    return value;
                }
                PropertyDescriptor descriptor = TypeDescriptor.GetProperties(style.DataGridTableStyle.DataGrid)["DataSource"];
                if (descriptor == null)
                {
                    return value;
                }
                object rootDataSource = descriptor.GetValue(style.DataGridTableStyle.DataGrid);
                if (this.designBindingPicker == null)
                {
                    this.designBindingPicker = new DesignBindingPicker();
                }
                DesignBinding initialSelectedItem = new DesignBinding(null, (string) value);
                DesignBinding binding2 = this.designBindingPicker.Pick(context, provider, false, true, false, rootDataSource, string.Empty, initialSelectedItem);
                if ((rootDataSource == null) || (binding2 == null))
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

