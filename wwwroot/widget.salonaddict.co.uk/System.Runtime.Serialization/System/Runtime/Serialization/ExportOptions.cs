namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.ObjectModel;

    public class ExportOptions
    {
        private IDataContractSurrogate dataContractSurrogate;
        private Collection<Type> knownTypes;

        internal IDataContractSurrogate GetSurrogate() => 
            this.dataContractSurrogate;

        public IDataContractSurrogate DataContractSurrogate
        {
            get => 
                this.dataContractSurrogate;
            set
            {
                this.dataContractSurrogate = value;
            }
        }

        public Collection<Type> KnownTypes
        {
            get
            {
                if (this.knownTypes == null)
                {
                    this.knownTypes = new Collection<Type>();
                }
                return this.knownTypes;
            }
        }
    }
}

