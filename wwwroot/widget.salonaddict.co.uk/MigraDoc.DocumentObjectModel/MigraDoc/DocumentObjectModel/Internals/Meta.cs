namespace MigraDoc.DocumentObjectModel.Internals
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.Reflection;

    public class Meta
    {
        private ValueDescriptorCollection vds = new ValueDescriptorCollection();

        public Meta(Type documentObjectType)
        {
            AddValueDescriptors(this, documentObjectType);
        }

        private static void AddValueDescriptors(Meta meta, Type type)
        {
            foreach (FieldInfo info in type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                DVAttribute[] customAttributes = (DVAttribute[]) info.GetCustomAttributes(typeof(DVAttribute), false);
                if (customAttributes.Length == 1)
                {
                    ValueDescriptor vd = ValueDescriptor.CreateValueDescriptor(info, customAttributes[0]);
                    meta.ValueDescriptors.Add(vd);
                }
            }
            foreach (PropertyInfo info2 in type.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                DVAttribute[] attributeArray2 = (DVAttribute[]) info2.GetCustomAttributes(typeof(DVAttribute), false);
                if (attributeArray2.Length == 1)
                {
                    ValueDescriptor descriptor2 = ValueDescriptor.CreateValueDescriptor(info2, attributeArray2[0]);
                    meta.ValueDescriptors.Add(descriptor2);
                }
            }
        }

        public static Meta GetMeta(DocumentObject documentObject) => 
            documentObject.Meta;

        public object GetValue(DocumentObject dom, string name, GV flags)
        {
            int index = name.IndexOf('.');
            if (index == 0)
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            string str = null;
            if (index > 0)
            {
                str = name.Substring(index + 1);
                name = name.Substring(0, index);
            }
            ValueDescriptor descriptor = this.vds[name];
            if (descriptor == null)
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            object obj2 = descriptor.GetValue(dom, flags);
            if ((obj2 == null) && (flags == GV.GetNull))
            {
                return null;
            }
            if (str == null)
            {
                return obj2;
            }
            if ((obj2 == null) || (str == ""))
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            DocumentObject obj3 = obj2 as DocumentObject;
            return obj3?.GetValue(str, flags);
        }

        public bool HasValue(string name)
        {
            ValueDescriptor descriptor = this.vds[name];
            return (descriptor != null);
        }

        public bool IsNull(DocumentObject dom)
        {
            int count = this.vds.Count;
            for (int i = 0; i < count; i++)
            {
                ValueDescriptor descriptor = this.vds[i];
                if (!descriptor.IsRefOnly && !descriptor.IsNull(dom))
                {
                    return false;
                }
            }
            return true;
        }

        public virtual bool IsNull(DocumentObject dom, string name)
        {
            int index = name.IndexOf('.');
            if (index == 0)
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            string str = null;
            if (index > 0)
            {
                str = name.Substring(index + 1);
                name = name.Substring(0, index);
            }
            ValueDescriptor descriptor = this.vds[name];
            if (descriptor == null)
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            if ((descriptor is NullableDescriptor) || (descriptor is ValueTypeDescriptor))
            {
                if (str != null)
                {
                    throw new ArgumentException(DomSR.InvalidValueName(name));
                }
                return descriptor.IsNull(dom);
            }
            DocumentObject obj2 = (DocumentObject) descriptor.GetValue(dom, GV.ReadOnly);
            if (obj2 == null)
            {
                return true;
            }
            if (str != null)
            {
                return obj2.IsNull(str);
            }
            return obj2.IsNull();
        }

        public virtual void SetNull(DocumentObject dom)
        {
            int count = this.vds.Count;
            for (int i = 0; i < count; i++)
            {
                if (!this.vds[i].IsRefOnly)
                {
                    this.vds[i].SetNull(dom);
                }
            }
        }

        public void SetNull(DocumentObject dom, string name)
        {
            ValueDescriptor descriptor = this.vds[name];
            if (descriptor == null)
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            descriptor.SetNull(dom);
        }

        public void SetValue(DocumentObject dom, string name, object val)
        {
            int index = name.IndexOf('.');
            if (index == 0)
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            string str = null;
            if (index > 0)
            {
                str = name.Substring(index + 1);
                name = name.Substring(0, index);
            }
            ValueDescriptor descriptor = this.vds[name];
            if (descriptor == null)
            {
                throw new ArgumentException(DomSR.InvalidValueName(name));
            }
            if (str != null)
            {
                (dom.GetValue(name) as DocumentObject).SetValue(str, val);
            }
            else
            {
                descriptor.SetValue(dom, val);
            }
        }

        public ValueDescriptor this[string name] =>
            this.vds[name];

        public ValueDescriptorCollection ValueDescriptors =>
            this.vds;
    }
}

