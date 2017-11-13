namespace System.Windows.Markup
{
    using System;
    using System.ComponentModel;

    public interface IValueSerializerContext : ITypeDescriptorContext, IServiceProvider
    {
        ValueSerializer GetValueSerializerFor(PropertyDescriptor descriptor);
        ValueSerializer GetValueSerializerFor(Type type);
    }
}

