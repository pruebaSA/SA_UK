namespace System.ServiceModel.Configuration
{
    public class BasicHttpBindingCollectionElement : StandardBindingCollectionElement<BasicHttpBinding, BasicHttpBindingElement>
    {
        internal static BasicHttpBindingCollectionElement GetBindingCollectionElement() => 
            ((BasicHttpBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("basicHttpBinding"));
    }
}

