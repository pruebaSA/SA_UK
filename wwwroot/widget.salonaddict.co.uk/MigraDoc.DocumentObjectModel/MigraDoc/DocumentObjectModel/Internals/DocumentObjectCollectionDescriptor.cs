namespace MigraDoc.DocumentObjectModel.Internals
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.ComponentModel;
    using System.Reflection;

    internal class DocumentObjectCollectionDescriptor : ValueDescriptor
    {
        internal DocumentObjectCollectionDescriptor(string valueName, Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags) : base(valueName, valueType, fieldType, memberInfo, flags)
        {
        }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            if (!Enum.IsDefined(typeof(GV), flags))
            {
                throw new InvalidEnumArgumentException("flags", (int) flags, typeof(GV));
            }
            DocumentObjectCollection objects = base.FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if ((objects == null) && (flags == GV.ReadWrite))
            {
                objects = base.CreateValue() as DocumentObjectCollection;
                objects.parent = dom;
                base.FieldInfo.SetValue(dom, objects);
                return objects;
            }
            if (((objects != null) && objects.IsNull()) && (flags == GV.GetNull))
            {
                return null;
            }
            return objects;
        }

        public override bool IsNull(DocumentObject dom)
        {
            DocumentObjectCollection objects = base.FieldInfo.GetValue(dom) as DocumentObjectCollection;
            return ((objects == null) || objects.IsNull());
        }

        public override void SetNull(DocumentObject dom)
        {
            DocumentObjectCollection objects = base.FieldInfo.GetValue(dom) as DocumentObjectCollection;
            if (objects != null)
            {
                objects.SetNull();
            }
        }

        public override void SetValue(DocumentObject dom, object val)
        {
            base.FieldInfo.SetValue(dom, val);
        }
    }
}

