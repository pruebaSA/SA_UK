namespace System.Runtime.Serialization
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal sealed class SerializationFieldInfo : System.Reflection.FieldInfo
    {
        internal static readonly char FakeNameSeparatorChar = '+';
        internal static readonly string FakeNameSeparatorString = "+";
        private RuntimeFieldInfo m_field;
        private string m_serializationName;

        internal SerializationFieldInfo(RuntimeFieldInfo field, string namePrefix)
        {
            this.m_field = field;
            this.m_serializationName = namePrefix + FakeNameSeparatorString + this.m_field.Name;
        }

        public override object[] GetCustomAttributes(bool inherit) => 
            this.m_field.GetCustomAttributes(inherit);

        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => 
            this.m_field.GetCustomAttributes(attributeType, inherit);

        public override object GetValue(object obj) => 
            this.m_field.GetValue(obj);

        internal object InternalGetValue(object obj, bool requiresAccessCheck)
        {
            RtFieldInfo field = this.m_field as RtFieldInfo;
            if (field != null)
            {
                return field.InternalGetValue(obj, requiresAccessCheck);
            }
            return this.m_field.GetValue(obj);
        }

        internal void InternalSetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture, bool requiresAccessCheck, bool isBinderDefault)
        {
            RtFieldInfo field = this.m_field as RtFieldInfo;
            if (field != null)
            {
                field.InternalSetValue(obj, value, invokeAttr, binder, culture, false);
            }
            else
            {
                this.m_field.SetValue(obj, value, invokeAttr, binder, culture);
            }
        }

        public override bool IsDefined(Type attributeType, bool inherit) => 
            this.m_field.IsDefined(attributeType, inherit);

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
        {
            this.m_field.SetValue(obj, value, invokeAttr, binder, culture);
        }

        public override FieldAttributes Attributes =>
            this.m_field.Attributes;

        public override Type DeclaringType =>
            this.m_field.DeclaringType;

        public override RuntimeFieldHandle FieldHandle =>
            this.m_field.FieldHandle;

        internal RuntimeFieldInfo FieldInfo =>
            this.m_field;

        public override Type FieldType =>
            this.m_field.FieldType;

        public override int MetadataToken =>
            this.m_field.MetadataToken;

        public override System.Reflection.Module Module =>
            this.m_field.Module;

        public override string Name =>
            this.m_serializationName;

        public override Type ReflectedType =>
            this.m_field.ReflectedType;
    }
}

