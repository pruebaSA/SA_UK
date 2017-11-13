namespace MigraDoc.DocumentObjectModel.Internals
{
    using MigraDoc.DocumentObjectModel;
    using System;
    using System.Reflection;

    public abstract class ValueDescriptor
    {
        private VDFlags flags;
        protected MemberInfo memberInfo;
        public Type MemberType;
        public string ValueName;
        public Type ValueType;

        internal ValueDescriptor(string valueName, Type valueType, Type memberType, MemberInfo memberInfo, VDFlags flags)
        {
            this.ValueName = valueName;
            this.ValueType = valueType;
            this.MemberType = memberType;
            this.memberInfo = memberInfo;
            this.flags = flags;
        }

        public object CreateValue() => 
            this.ValueType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null).Invoke(null);

        internal static ValueDescriptor CreateValueDescriptor(MemberInfo memberInfo, DVAttribute attr)
        {
            Type fieldType;
            VDFlags none = VDFlags.None;
            if (attr.RefOnly)
            {
                none |= VDFlags.RefOnly;
            }
            string name = memberInfo.Name;
            if (memberInfo is System.Reflection.FieldInfo)
            {
                fieldType = ((System.Reflection.FieldInfo) memberInfo).FieldType;
            }
            else
            {
                fieldType = ((System.Reflection.PropertyInfo) memberInfo).PropertyType;
            }
            if (fieldType == typeof(NBool))
            {
                return new NullableDescriptor(name, typeof(bool), fieldType, memberInfo, none);
            }
            if (fieldType == typeof(NInt))
            {
                return new NullableDescriptor(name, typeof(int), fieldType, memberInfo, none);
            }
            if (fieldType == typeof(NDouble))
            {
                return new NullableDescriptor(name, typeof(double), fieldType, memberInfo, none);
            }
            if (fieldType == typeof(NString))
            {
                return new NullableDescriptor(name, typeof(string), fieldType, memberInfo, none);
            }
            if (fieldType == typeof(string))
            {
                return new ValueTypeDescriptor(name, typeof(string), fieldType, memberInfo, none);
            }
            if (fieldType == typeof(NEnum))
            {
                return new NullableDescriptor(name, attr.Type, fieldType, memberInfo, none);
            }
            if (fieldType.IsSubclassOf(typeof(System.ValueType)))
            {
                return new ValueTypeDescriptor(name, fieldType, fieldType, memberInfo, none);
            }
            if (typeof(DocumentObjectCollection).IsAssignableFrom(fieldType))
            {
                return new DocumentObjectCollectionDescriptor(name, fieldType, fieldType, memberInfo, none);
            }
            if (typeof(DocumentObject).IsAssignableFrom(fieldType))
            {
                return new DocumentObjectDescriptor(name, fieldType, fieldType, memberInfo, none);
            }
            return null;
        }

        public abstract object GetValue(DocumentObject dom, GV flags);
        public abstract bool IsNull(DocumentObject dom);
        public abstract void SetNull(DocumentObject dom);
        public abstract void SetValue(DocumentObject dom, object val);

        public System.Reflection.FieldInfo FieldInfo =>
            (this.memberInfo as System.Reflection.FieldInfo);

        public bool IsRefOnly =>
            ((this.flags & VDFlags.RefOnly) == VDFlags.RefOnly);

        public System.Reflection.PropertyInfo PropertyInfo =>
            (this.memberInfo as System.Reflection.PropertyInfo);
    }
}

