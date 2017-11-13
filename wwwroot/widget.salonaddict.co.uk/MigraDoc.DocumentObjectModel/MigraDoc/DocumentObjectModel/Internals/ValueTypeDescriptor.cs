namespace MigraDoc.DocumentObjectModel.Internals
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.ComponentModel;
    using System.Reflection;

    internal class ValueTypeDescriptor : ValueDescriptor
    {
        internal ValueTypeDescriptor(string valueName, Type valueType, Type fieldType, MemberInfo memberInfo, VDFlags flags) : base(valueName, valueType, fieldType, memberInfo, flags)
        {
        }

        public override object GetValue(DocumentObject dom, GV flags)
        {
            object obj2;
            if (!Enum.IsDefined(typeof(GV), flags))
            {
                throw new InvalidEnumArgumentException("flags", (int) flags, typeof(GV));
            }
            if (base.FieldInfo != null)
            {
                obj2 = base.FieldInfo.GetValue(dom);
            }
            else
            {
                obj2 = base.PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
            }
            INullableValue value2 = obj2 as INullableValue;
            if (((value2 != null) && value2.IsNull) && (flags == GV.GetNull))
            {
                return null;
            }
            return obj2;
        }

        public override bool IsNull(DocumentObject dom)
        {
            object obj2;
            if (base.FieldInfo != null)
            {
                obj2 = base.FieldInfo.GetValue(dom);
            }
            else
            {
                obj2 = base.PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
            }
            INullableValue value2 = obj2 as INullableValue;
            return ((value2 != null) && value2.IsNull);
        }

        public override void SetNull(DocumentObject dom)
        {
            INullableValue value2;
            if (base.FieldInfo != null)
            {
                value2 = (INullableValue) base.FieldInfo.GetValue(dom);
                value2.SetNull();
                base.FieldInfo.SetValue(dom, value2);
            }
            else
            {
                value2 = (INullableValue) base.PropertyInfo.GetGetMethod(true).Invoke(dom, Type.EmptyTypes);
                value2.SetNull();
                base.PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { value2 });
            }
        }

        public override void SetValue(DocumentObject dom, object value)
        {
            if (base.FieldInfo != null)
            {
                base.FieldInfo.SetValue(dom, value);
            }
            else
            {
                base.PropertyInfo.GetSetMethod(true).Invoke(dom, new object[] { value });
            }
        }
    }
}

