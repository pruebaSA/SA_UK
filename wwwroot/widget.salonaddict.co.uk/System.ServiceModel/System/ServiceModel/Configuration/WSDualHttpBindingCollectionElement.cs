namespace System.ServiceModel.Configuration
{
    public class WSDualHttpBindingCollectionElement : StandardBindingCollectionElement<WSDualHttpBinding, WSDualHttpBindingElement>
    {
        internal static WSDualHttpBindingCollectionElement GetBindingCollectionElement() => 
            ((WSDualHttpBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("wsDualHttpBinding"));
    }
}

