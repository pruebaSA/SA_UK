namespace MigraDoc.DocumentObjectModel.Internals
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.ComponentModel;
    using System.Reflection;

    internal class DocumentObjectDescriptor : ValueDescriptor
    {
        internal DocumentObjectDescriptor(string valueName, Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags) : base(valueName, valueType, fieldType, memberInfo, flags)
        {
        }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            DocumentObject obj2;
            if (!Enum.IsDefined(typeof(GV), flags))
            {
                throw new InvalidEnumArgumentException("flags", (int) flags, typeof(GV));
            }
            if (base.FieldInfo != null)
            {
                obj2 = base.FieldInfo.GetValue(dom) as DocumentObject;
                if ((obj2 == null) && (flags == GV.ReadWrite))
                {
                    obj2 = base.CreateValue() as DocumentObject;
                    obj2.parent = dom;
                    base.FieldInfo.SetValue(dom, obj2);
                    return obj2;
                }
            }
            else
            {
                obj2 = base.PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes) as DocumentObject;
            }
            if (((obj2 != null) && obj2.IsNull()) && (flags == GV.GetNull))
            {
                return null;
            }
            return obj2;
        }

        public override bool IsNull(DocumentObject dom)
        {
            DocumentObject obj2;
            if (base.FieldInfo != null)
            {
                obj2 = base.FieldInfo.GetValue(dom) as DocumentObject;
                return obj2?.IsNull();
            }
            obj2 = base.PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes) as DocumentObject;
            if (obj2 != null)
            {
                obj2.IsNull();
            }
            return true;
        }

        public override void SetNull(DocumentObject dom)
        {
            DocumentObject obj2;
            if (base.FieldInfo != null)
            {
                obj2 = base.FieldInfo.GetValue(dom) as DocumentObject;
                if (obj2 != null)
                {
                    obj2.SetNull();
                }
            }
            if (base.PropertyInfo != null)
            {
                obj2 = base.PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes) as DocumentObject;
                if (obj2 != null)
                {
                    obj2.SetNull();
                }
            }
        }

        public override void SetValue(DocumentObject dom, object val)
        {
            FieldInfo fieldInfo = base.FieldInfo;
            if (fieldInfo == null)
            {
                throw new InvalidOperationException("This value cannot be set.");
            }
            fieldInfo.SetValue(dom, val);
        }
    }
}

