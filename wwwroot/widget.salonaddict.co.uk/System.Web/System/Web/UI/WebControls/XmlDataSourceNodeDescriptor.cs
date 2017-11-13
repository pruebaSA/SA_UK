﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.XPath;

    internal sealed class XmlDataSourceNodeDescriptor : ICustomTypeDescriptor, IXPathNavigable
    {
        private XmlNode _node;

        public XmlDataSourceNodeDescriptor(XmlNode node)
        {
            this._node = node;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            AttributeCollection.Empty;

        string ICustomTypeDescriptor.GetClassName() => 
            base.GetType().Name;

        string ICustomTypeDescriptor.GetComponentName() => 
            null;

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            null;

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            null;

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            null;

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType) => 
            null;

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => 
            null;

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attrs) => 
            null;

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => 
            ((ICustomTypeDescriptor) this).GetProperties(null);

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attrFilter)
        {
            List<PropertyDescriptor> list = new List<PropertyDescriptor>();
            XmlAttributeCollection attributes = this._node.Attributes;
            if (attributes != null)
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    list.Add(new XmlDataSourcePropertyDescriptor(attributes[i].Name));
                }
            }
            return new PropertyDescriptorCollection(list.ToArray());
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            if (pd is XmlDataSourcePropertyDescriptor)
            {
                return this;
            }
            return null;
        }

        XPathNavigator IXPathNavigable.CreateNavigator() => 
            this._node.CreateNavigator();

        private class XmlDataSourcePropertyDescriptor : PropertyDescriptor
        {
            private string _name;

            public XmlDataSourcePropertyDescriptor(string name) : base(name, null)
            {
                this._name = name;
            }

            public override bool CanResetValue(object o) => 
                false;

            public override object GetValue(object o)
            {
                XmlDataSourceNodeDescriptor descriptor = o as XmlDataSourceNodeDescriptor;
                if (descriptor != null)
                {
                    XmlAttributeCollection attributes = descriptor._node.Attributes;
                    if (attributes != null)
                    {
                        XmlAttribute attribute = attributes[this._name];
                        if (attribute != null)
                        {
                            return attribute.Value;
                        }
                    }
                }
                return string.Empty;
            }

            public override void ResetValue(object o)
            {
            }

            public override void SetValue(object o, object value)
            {
            }

            public override bool ShouldSerializeValue(object o) => 
                true;

            public override Type ComponentType =>
                typeof(XmlDataSourceNodeDescriptor);

            public override bool IsReadOnly =>
                true;

            public override Type PropertyType =>
                typeof(string);
        }
    }
}

