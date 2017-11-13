namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Drawing;

    internal sealed class ErrorStyle : Style, ICustomTypeDescriptor
    {
        public ErrorStyle()
        {
            base.ForeColor = Color.Red;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            TypeDescriptor.GetAttributes(this, true);

        string ICustomTypeDescriptor.GetClassName() => 
            TypeDescriptor.GetClassName(this, true);

        string ICustomTypeDescriptor.GetComponentName() => 
            TypeDescriptor.GetComponentName(this, true);

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            TypeDescriptor.GetConverter(this, true);

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            TypeDescriptor.GetDefaultEvent(this, true);

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            TypeDescriptor.GetDefaultProperty(this, true);

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => 
            TypeDescriptor.GetEditor(this, editorBaseType, true);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => 
            TypeDescriptor.GetEvents(this, true);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => 
            TypeDescriptor.GetEvents(this, attributes, true);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            ((ICustomTypeDescriptor) this).GetProperties(null);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(base.GetType(), attributes);
            PropertyDescriptor[] descriptorArray = new PropertyDescriptor[properties.Count];
            PropertyDescriptor descriptor = properties["ForeColor"];
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor oldPropertyDescriptor = properties[i];
                if (oldPropertyDescriptor == descriptor)
                {
                    descriptorArray[i] = TypeDescriptor.CreateProperty(base.GetType(), oldPropertyDescriptor, new Attribute[] { new DefaultValueAttribute(typeof(Color), "Red") });
                }
                else
                {
                    descriptorArray[i] = oldPropertyDescriptor;
                }
            }
            return new PropertyDescriptorCollection(descriptorArray, true);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            this;
    }
}

