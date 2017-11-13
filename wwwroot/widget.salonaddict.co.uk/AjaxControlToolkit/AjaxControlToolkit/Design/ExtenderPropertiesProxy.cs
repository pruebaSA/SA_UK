namespace AjaxControlToolkit.Design
{
    using AjaxControlToolkit;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.UI;

    internal class ExtenderPropertiesProxy : ICustomTypeDescriptor
    {
        private string[] _propsToHide;
        private object _target;

        public ExtenderPropertiesProxy(object target, params string[] propsToHide)
        {
            this._target = target;
            this._propsToHide = propsToHide;
        }

        System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            TypeDescriptor.GetAttributes(this.Target);

        string ICustomTypeDescriptor.GetClassName() => 
            TypeDescriptor.GetClassName(this.Target);

        string ICustomTypeDescriptor.GetComponentName() => 
            TypeDescriptor.GetComponentName(this.Target);

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            TypeDescriptor.GetConverter(this.Target);

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            TypeDescriptor.GetDefaultEvent(this.Target);

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            TypeDescriptor.GetDefaultProperty(this.Target);

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => 
            TypeDescriptor.GetEditor(this.Target, editorBaseType);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => 
            TypeDescriptor.GetEvents(this.Target);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) => 
            TypeDescriptor.GetEvents(this.Target, attributes);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            TypeDescriptor.GetProperties(this.Target);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.Target);
            if ((this._propsToHide == null) || (this._propsToHide.Length <= 0))
            {
                return properties;
            }
            List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            for (int i = 0; i < properties.Count; i++)
            {
                PropertyDescriptor prop = properties[i];
                if (((ExtenderControlPropertyAttribute) prop.Attributes[typeof(ExtenderControlPropertyAttribute)]) != null)
                {
                    ExtenderVisiblePropertyAttribute attribute2 = (ExtenderVisiblePropertyAttribute) prop.Attributes[typeof(ExtenderVisiblePropertyAttribute)];
                    if (((attribute2 != null) && attribute2.Value) && (Array.FindIndex<string>(this._propsToHide, s => s == prop.Name) == -1))
                    {
                        IDReferencePropertyAttribute attribute3 = (IDReferencePropertyAttribute) prop.Attributes[typeof(IDReferencePropertyAttribute)];
                        Attribute attribute4 = prop.Attributes[typeof(TypeConverterAttribute)];
                        if ((attribute3 != null) && !attribute3.IsDefaultAttribute())
                        {
                            attribute4 = new TypeConverterAttribute(typeof(TypedControlIDConverter<Control>).GetGenericTypeDefinition().MakeGenericType(new Type[] { attribute3.ReferencedControlType }));
                        }
                        prop = TypeDescriptor.CreateProperty(prop.ComponentType, prop, new Attribute[] { BrowsableAttribute.Yes, attribute4 });
                        list.Add(prop);
                    }
                }
            }
            return new PropertyDescriptorCollection(list.ToArray());
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) => 
            this.Target;

        private object Target =>
            this._target;
    }
}

