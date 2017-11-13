namespace System.ServiceModel.Configuration
{
    public class NetTcpBindingCollectionElement : StandardBindingCollectionElement<NetTcpBinding, NetTcpBindingElement>
    {
        internal static NetTcpBindingCollectionElement GetBindingCollectionElement() => 
            ((NetTcpBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("netTcpBinding"));
    }
}

