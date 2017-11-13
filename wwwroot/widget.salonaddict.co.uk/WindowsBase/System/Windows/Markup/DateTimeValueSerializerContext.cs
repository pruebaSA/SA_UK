namespace System.Windows.Markup
{
    using System;
    using System.ComponentModel;

    internal class DateTimeValueSerializerContext : IValueSerializerContext, ITypeDescriptorContext, IServiceProvider
    {
        public object GetService(Type serviceType) => 
            null;

        public ValueSerializer GetValueSerializerFor(System.ComponentModel.PropertyDescriptor descriptor) => 
            null;

        public ValueSerializer GetValueSerializerFor(Type type) => 
            null;

        public void OnComponentChanged()
        {
        }

        public bool OnComponentChanging() => 
            false;

        public IContainer Container =>
            null;

        public object Instance =>
            null;

        public System.ComponentModel.PropertyDescriptor PropertyDescriptor =>
            null;
    }
}

