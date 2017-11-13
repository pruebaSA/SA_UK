﻿namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web;
    using System.Web.UI;
    using System.Xml;

    internal sealed class XmlHierarchyData : IHierarchyData, ICustomTypeDescriptor
    {
        private XmlNode _item;
        private XmlHierarchicalEnumerable _parent;
        private string _path;

        internal XmlHierarchyData(XmlHierarchicalEnumerable parent, XmlNode item)
        {
            this._parent = parent;
            this._item = item;
        }

        private string CreateRecursivePath(XmlNode node)
        {
            if (node.ParentNode == null)
            {
                return string.Empty;
            }
            return (this.CreateRecursivePath(node.ParentNode) + this.FindNodePosition(node));
        }

        private string FindNodePosition(XmlNode node)
        {
            XmlNodeList childNodes = node.ParentNode.ChildNodes;
            int num = 0;
            for (int i = 0; i < childNodes.Count; i++)
            {
                if (childNodes[i].NodeType == XmlNodeType.Element)
                {
                    num++;
                }
                if (childNodes[i] == node)
                {
                    return ("/*[position()=" + Convert.ToString(num, CultureInfo.InvariantCulture) + "]");
                }
            }
            throw new ArgumentException(System.Web.SR.GetString("XmlHierarchyData_CouldNotFindNode"));
        }

        System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() => 
            System.ComponentModel.AttributeCollection.Empty;

        string ICustomTypeDescriptor.GetClassName() => 
            base.GetType().Name;

        string ICustomTypeDescriptor.GetComponentName() => 
            null;

        TypeConverter ICustomTypeDescriptor.GetConverter() => 
            null;

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() => 
            null;

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() => 
            new XmlHierarchyDataPropertyDescriptor("#Name");

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
            List<PropertyDescriptor> list = new List<PropertyDescriptor> {
                new XmlHierarchyDataPropertyDescriptor("#Name"),
                new XmlHierarchyDataPropertyDescriptor("#Value"),
                new XmlHierarchyDataPropertyDescriptor("#InnerText")
            };
            XmlAttributeCollection attributes = this._item.Attributes;
            if (attributes != null)
            {
                for (int i = 0; i < attributes.Count; i++)
                {
                    list.Add(new XmlHierarchyDataPropertyDescriptor(attributes[i].Name));
                }
            }
            return new PropertyDescriptorCollection(list.ToArray());
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            if (pd is XmlHierarchyDataPropertyDescriptor)
            {
                return this;
            }
            return null;
        }

        IHierarchicalEnumerable IHierarchyData.GetChildren() => 
            new XmlHierarchicalEnumerable(this._item.ChildNodes);

        IHierarchyData IHierarchyData.GetParent()
        {
            XmlNode parentNode = this._item.ParentNode;
            if (parentNode == null)
            {
                return null;
            }
            return new XmlHierarchyData(null, parentNode);
        }

        public override string ToString() => 
            this._item.Name;

        bool IHierarchyData.HasChildren =>
            this._item.HasChildNodes;

        object IHierarchyData.Item =>
            this._item;

        string IHierarchyData.Path
        {
            get
            {
                if (this._path == null)
                {
                    if (this._parent != null)
                    {
                        if (this._parent.Path == null)
                        {
                            this._parent.Path = this.CreateRecursivePath(this._item.ParentNode);
                        }
                        this._path = this._parent.Path + this.FindNodePosition(this._item);
                    }
                    else
                    {
                        this._path = this.CreateRecursivePath(this._item);
                    }
                }
                return this._path;
            }
        }

        string IHierarchyData.Type =>
            this._item.Name;

        private class XmlHierarchyDataPropertyDescriptor : PropertyDescriptor
        {
            private string _name;

            public XmlHierarchyDataPropertyDescriptor(string name) : base(name, null)
            {
                this._name = name;
            }

            public override bool CanResetValue(object o) => 
                false;

            public override object GetValue(object o)
            {
                XmlHierarchyData data = o as XmlHierarchyData;
                if (data != null)
                {
                    switch (this._name)
                    {
                        case "#Name":
                            return data._item.Name;

                        case "#Value":
                            return data._item.Value;

                        case "#InnerText":
                            return data._item.InnerText;
                    }
                    XmlAttributeCollection attributes = data._item.Attributes;
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
                typeof(XmlHierarchyData);

            public override bool IsReadOnly =>
                true;

            public override Type PropertyType =>
                typeof(string);
        }
    }
}

