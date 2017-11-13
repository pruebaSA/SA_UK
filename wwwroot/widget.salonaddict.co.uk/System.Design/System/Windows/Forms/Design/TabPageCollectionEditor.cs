namespace System.Windows.Forms.Design
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows.Forms;

    internal class TabPageCollectionEditor : CollectionEditor
    {
        public TabPageCollectionEditor() : base(typeof(TabControl.TabPageCollection))
        {
        }

        protected override object SetItems(object editValue, object[] value)
        {
            TabControl instance = base.Context.Instance as TabControl;
            if (instance != null)
            {
                instance.SuspendLayout();
            }
            foreach (object obj2 in value)
            {
                TabPage component = obj2 as TabPage;
                if (component != null)
                {
                    PropertyDescriptor descriptor = TypeDescriptor.GetProperties(component)["UseVisualStyleBackColor"];
                    if (((descriptor != null) && (descriptor.PropertyType == typeof(bool))) && (!descriptor.IsReadOnly && descriptor.IsBrowsable))
                    {
                        descriptor.SetValue(component, true);
                    }
                }
            }
            object obj3 = base.SetItems(editValue, value);
            if (instance != null)
            {
                instance.ResumeLayout();
            }
            return obj3;
        }
    }
}

