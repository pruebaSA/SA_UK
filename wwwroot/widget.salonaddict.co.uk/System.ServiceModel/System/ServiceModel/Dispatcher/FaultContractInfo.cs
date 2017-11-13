﻿namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public class FaultContractInfo
    {
        private string action;
        private Type detail;
        private string elementName;
        private IList<Type> knownTypes;
        private string ns;
        private DataContractSerializer serializer;

        public FaultContractInfo(string action, Type detail) : this(action, detail, null, null, null)
        {
        }

        internal FaultContractInfo(string action, Type detail, XmlName elementName, string ns, IList<Type> knownTypes)
        {
            if (action == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("action");
            }
            if (detail == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("detail");
            }
            this.action = action;
            this.detail = detail;
            if (elementName != null)
            {
                this.elementName = elementName.EncodedName;
            }
            this.ns = ns;
            this.knownTypes = knownTypes;
        }

        public string Action =>
            this.action;

        public Type Detail =>
            this.detail;

        internal string ElementName =>
            this.elementName;

        internal string ElementNamespace =>
            this.ns;

        internal IList<Type> KnownTypes =>
            this.knownTypes;

        internal DataContractSerializer Serializer
        {
            get
            {
                if (this.serializer == null)
                {
                    if (this.elementName == null)
                    {
                        this.serializer = DataContractSerializerDefaults.CreateSerializer(this.detail, this.knownTypes, 0x7fffffff);
                    }
                    else
                    {
                        this.serializer = DataContractSerializerDefaults.CreateSerializer(this.detail, this.knownTypes, this.elementName, (this.ns == null) ? string.Empty : this.ns, 0x7fffffff);
                    }
                }
                return this.serializer;
            }
        }
    }
}

