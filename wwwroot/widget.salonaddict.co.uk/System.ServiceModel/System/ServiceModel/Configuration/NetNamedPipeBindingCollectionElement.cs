namespace System.ServiceModel.Configuration
{
    public class NetNamedPipeBindingCollectionElement : StandardBindingCollectionElement<NetNamedPipeBinding, NetNamedPipeBindingElement>
    {
        internal static NetNamedPipeBindingCollectionElement GetBindingCollectionElement() => 
            ((NetNamedPipeBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("netNamedPipeBinding"));
    }
}

