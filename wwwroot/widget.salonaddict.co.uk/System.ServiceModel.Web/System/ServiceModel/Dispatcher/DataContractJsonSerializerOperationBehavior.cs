namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel.Description;
    using System.Xml;

    internal class DataContractJsonSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        private bool alwaysEmitTypeInformation;

        public DataContractJsonSerializerOperationBehavior(OperationDescription description, int maxItemsInObjectGraph, bool ignoreExtensionDataObject, IDataContractSurrogate dataContractSurrogate, bool alwaysEmitTypeInformation) : base(description)
        {
            base.MaxItemsInObjectGraph = maxItemsInObjectGraph;
            base.IgnoreExtensionDataObject = ignoreExtensionDataObject;
            base.DataContractSurrogate = dataContractSurrogate;
            this.alwaysEmitTypeInformation = alwaysEmitTypeInformation;
        }

        public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes) => 
            new DataContractJsonSerializer(type, name, knownTypes, base.MaxItemsInObjectGraph, base.IgnoreExtensionDataObject, base.DataContractSurrogate, this.alwaysEmitTypeInformation);

        public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes) => 
            new DataContractJsonSerializer(type, name, knownTypes, base.MaxItemsInObjectGraph, base.IgnoreExtensionDataObject, base.DataContractSurrogate, this.alwaysEmitTypeInformation);
    }
}

