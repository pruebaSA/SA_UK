namespace System.ServiceModel.MsmqIntegration
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Channels;

    internal sealed class MsmqIntegrationReceiveParameters : MsmqReceiveParameters
    {
        private MsmqMessageSerializationFormat serializationFormat;
        private Type[] targetSerializationTypes;

        internal MsmqIntegrationReceiveParameters(MsmqIntegrationBindingElement bindingElement) : base(bindingElement)
        {
            this.serializationFormat = bindingElement.SerializationFormat;
            List<Type> list = new List<Type>();
            if (bindingElement.TargetSerializationTypes != null)
            {
                foreach (Type type in bindingElement.TargetSerializationTypes)
                {
                    if (!list.Contains(type))
                    {
                        list.Add(type);
                    }
                }
            }
            this.targetSerializationTypes = list.ToArray();
        }

        internal MsmqMessageSerializationFormat SerializationFormat =>
            this.serializationFormat;

        internal Type[] TargetSerializationTypes =>
            this.targetSerializationTypes;
    }
}

