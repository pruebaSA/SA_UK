namespace System.ServiceModel.Configuration
{
    public class MsmqIntegrationBindingCollectionElement : StandardBindingCollectionElement<MsmqIntegrationBinding, MsmqIntegrationBindingElement>
    {
        internal static MsmqIntegrationBindingCollectionElement GetBindingCollectionElement() => 
            ((MsmqIntegrationBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("msmqIntegrationBinding"));
    }
}

