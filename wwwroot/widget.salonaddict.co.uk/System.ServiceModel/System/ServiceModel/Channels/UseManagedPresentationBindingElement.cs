namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.Xml;

    public sealed class UseManagedPresentationBindingElement : BindingElement, IPolicyExportExtension
    {
        public override BindingElement Clone() => 
            new UseManagedPresentationBindingElement();

        public override T GetProperty<T>(BindingContext context) where T: class => 
            context?.GetInnerProperty<T>();

        void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if ((context.BindingElements != null) && (context.BindingElements.Find<UseManagedPresentationBindingElement>() != null))
            {
                XmlElement item = new XmlDocument().CreateElement("ic", "RequireFederatedIdentityProvisioning", "http://schemas.xmlsoap.org/ws/2005/05/identity");
                context.GetBindingAssertions().Add(item);
            }
        }
    }
}

