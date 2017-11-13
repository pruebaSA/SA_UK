namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Web.Configuration;
    using System.Web.Hosting;

    public class WebHttpBindingCollectionElement : StandardBindingCollectionElement<WebHttpBinding, WebHttpBindingElement>
    {
        internal static WebHttpBindingCollectionElement GetBindingCollectionElement()
        {
            BindingCollectionElement element = null;
            BindingsSection sectionFromWebConfiguration = null;
            string sectionPath = "system.serviceModel/bindings";
            if (ServiceHostingEnvironment.IsHosted)
            {
                sectionFromWebConfiguration = GetSectionFromWebConfiguration(sectionPath);
            }
            else
            {
                sectionFromWebConfiguration = (BindingsSection) ConfigurationManager.GetSection(sectionPath);
            }
            element = sectionFromWebConfiguration["webHttpBinding"];
            return (WebHttpBindingCollectionElement) element;
        }

        protected internal override Binding GetDefault() => 
            new WebHttpBinding();

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static BindingsSection GetSectionFromWebConfiguration(string sectionPath)
        {
            if (HostingEnvironment.ApplicationVirtualPath != null)
            {
                return (BindingsSection) WebConfigurationManager.GetSection(sectionPath, HostingEnvironment.ApplicationVirtualPath);
            }
            return (BindingsSection) WebConfigurationManager.GetSection(sectionPath);
        }
    }
}

