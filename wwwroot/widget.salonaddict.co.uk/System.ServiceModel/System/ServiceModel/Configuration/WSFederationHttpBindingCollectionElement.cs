namespace System.ServiceModel.Configuration
{
    public class WSFederationHttpBindingCollectionElement : StandardBindingCollectionElement<WSFederationHttpBinding, WSFederationHttpBindingElement>
    {
        internal static WSFederationHttpBindingCollectionElement GetBindingCollectionElement() => 
            ((WSFederationHttpBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("wsFederationHttpBinding"));
    }
}

