namespace System.ServiceModel.Configuration
{
    public class WSHttpBindingCollectionElement : StandardBindingCollectionElement<WSHttpBinding, WSHttpBindingElement>
    {
        internal static WSHttpBindingCollectionElement GetBindingCollectionElement() => 
            ((WSHttpBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("wsHttpBinding"));
    }
}

