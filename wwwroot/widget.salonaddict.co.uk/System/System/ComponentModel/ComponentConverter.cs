namespace System.ComponentModel
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class ComponentConverter : ReferenceConverter
    {
        public ComponentConverter(Type type) : base(type)
        {
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) => 
            TypeDescriptor.GetProperties(value, attributes);

        public override bool GetPropertiesSupported(ITypeDescriptorContext context) => 
            true;
    }
}

