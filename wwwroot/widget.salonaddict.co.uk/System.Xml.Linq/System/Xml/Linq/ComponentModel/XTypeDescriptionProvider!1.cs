namespace System.Xml.Linq.ComponentModel
{
    using System;
    using System.ComponentModel;

    internal class XTypeDescriptionProvider<T> : TypeDescriptionProvider
    {
        public XTypeDescriptionProvider() : base(TypeDescriptor.GetProvider(typeof(T)))
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type type, object instance) => 
            new XTypeDescriptor<T>(base.GetTypeDescriptor(type, instance));
    }
}

