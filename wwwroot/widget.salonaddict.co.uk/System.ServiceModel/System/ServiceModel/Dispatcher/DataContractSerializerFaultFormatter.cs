namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;

    internal class DataContractSerializerFaultFormatter : FaultFormatter
    {
        internal DataContractSerializerFaultFormatter(SynchronizedCollection<FaultContractInfo> faultContractInfoCollection) : base(faultContractInfoCollection)
        {
        }

        internal DataContractSerializerFaultFormatter(Type[] detailTypes) : base(detailTypes)
        {
        }
    }
}

