namespace System.ComponentModel
{
    using System;
    using System.Collections;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    internal sealed class DelegatingTypeDescriptionProvider : TypeDescriptionProvider
    {
        private Type _type;

        internal DelegatingTypeDescriptionProvider(Type type)
        {
            this._type = type;
        }

        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args) => 
            this.Provider.CreateInstance(provider, objectType, argTypes, args);

        public override IDictionary GetCache(object instance) => 
            this.Provider.GetCache(instance);

        public override ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance) => 
            this.Provider.GetExtendedTypeDescriptor(instance);

        public override string GetFullComponentName(object component) => 
            this.Provider.GetFullComponentName(component);

        public override Type GetReflectionType(Type objectType, object instance) => 
            this.Provider.GetReflectionType(objectType, instance);

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance) => 
            this.Provider.GetTypeDescriptor(objectType, instance);

        private TypeDescriptionProvider Provider =>
            TypeDescriptor.GetProviderRecursive(this._type);
    }
}

