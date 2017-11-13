namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal static class BasicHttpBindingDefaults
    {
        internal const WSMessageEncoding MessageEncoding = WSMessageEncoding.Text;
        internal const System.ServiceModel.TransferMode TransferMode = System.ServiceModel.TransferMode.Buffered;
    }
}

