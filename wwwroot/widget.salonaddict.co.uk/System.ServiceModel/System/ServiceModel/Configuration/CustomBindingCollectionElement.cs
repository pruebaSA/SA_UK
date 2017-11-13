namespace System.ServiceModel.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class CustomBindingCollectionElement : BindingCollectionElement
    {
        private ConfigurationPropertyCollection properties;

        public override bool ContainsKey(string name) => 
            this.Bindings.ContainsKey(name);

        internal static CustomBindingCollectionElement GetBindingCollectionElement() => 
            ((CustomBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("customBinding"));

        protected internal override Binding GetDefault() => 
            Activator.CreateInstance<CustomBinding>();

        protected internal override bool TryAdd(string name, Binding binding, System.Configuration.Configuration config)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
            }
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
            }
            if (config == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("config");
            }
            ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(config);
            CustomBindingElementCollection bindings = sectionGroup.Bindings.CustomBinding.Bindings;
            CustomBindingElement element = new CustomBindingElement(name);
            bindings.Add(element);
            ExtensionElementCollection bindingElementExtensions = sectionGroup.Extensions.BindingElementExtensions;
            CustomBinding binding2 = (CustomBinding) binding;
            foreach (BindingElement element2 in binding2.Elements)
            {
                BindingElementExtensionElement element3;
                bool flag = this.TryCreateMatchingExtension(element2, bindingElementExtensions, false, element.CollectionElementBaseType.AssemblyQualifiedName, out element3);
                if (!flag)
                {
                    flag = this.TryCreateMatchingExtension(element2, bindingElementExtensions, true, element.CollectionElementBaseType.AssemblyQualifiedName, out element3);
                }
                if (!flag)
                {
                    break;
                }
                element3.InitializeFrom(element2);
                element.Add(element3);
            }
            bool flag2 = element.Count == binding2.Elements.Count;
            if (!flag2)
            {
                bindings.Remove(element);
            }
            return flag2;
        }

        private bool TryCreateMatchingExtension(BindingElement bindingElement, ExtensionElementCollection collection, bool allowDerivedTypes, string assemblyName, out BindingElementExtensionElement result)
        {
            result = null;
            foreach (ExtensionElement element in collection)
            {
                bool flag;
                BindingElementExtensionElement element2 = Activator.CreateInstance(Type.GetType(element.Type, true)) as BindingElementExtensionElement;
                if (element2 == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidExtensionType", new object[] { element.Type, assemblyName, "bindingElementExtensions" })));
                }
                if (allowDerivedTypes)
                {
                    flag = element2.BindingElementType.IsAssignableFrom(bindingElement.GetType());
                }
                else
                {
                    flag = element2.BindingElementType.Equals(bindingElement.GetType());
                }
                if (flag)
                {
                    result = element2;
                    return true;
                }
            }
            return false;
        }

        [ConfigurationProperty("", Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public CustomBindingElementCollection Bindings =>
            ((CustomBindingElementCollection) base[""]);

        public override Type BindingType =>
            typeof(CustomBinding);

        public override ReadOnlyCollection<IBindingConfigurationElement> ConfiguredBindings
        {
            get
            {
                List<IBindingConfigurationElement> list = new List<IBindingConfigurationElement>();
                foreach (IBindingConfigurationElement element in this.Bindings)
                {
                    list.Add(element);
                }
                return new ReadOnlyCollection<IBindingConfigurationElement>(list);
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("", typeof(CustomBindingElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

