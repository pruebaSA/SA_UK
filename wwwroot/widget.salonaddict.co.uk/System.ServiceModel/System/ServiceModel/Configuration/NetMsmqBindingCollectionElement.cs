namespace System.ServiceModel.Configuration
{
    public class NetMsmqBindingCollectionElement : StandardBindingCollectionElement<NetMsmqBinding, NetMsmqBindingElement>
    {
        internal static NetMsmqBindingCollectionElement GetBindingCollectionElement() => 
            ((NetMsmqBindingCollectionElement) ConfigurationHelpers.GetBindingCollectionElement("netMsmqBinding"));
    }
}

