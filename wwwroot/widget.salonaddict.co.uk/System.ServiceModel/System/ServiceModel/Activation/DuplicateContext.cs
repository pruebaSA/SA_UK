namespace System.ServiceModel.Activation
{
    using System;
    using System.Runtime.Serialization;

    [KnownType(typeof(TcpDuplicateContext)), DataContract, KnownType(typeof(NamedPipeDuplicateContext))]
    internal class DuplicateContext
    {
        [DataMember]
        private byte[] readData;
        [DataMember]
        private Uri via;

        protected DuplicateContext(Uri via, byte[] readData)
        {
            this.via = via;
            this.readData = readData;
        }

        public byte[] ReadData =>
            this.readData;

        public Uri Via =>
            this.via;
    }
}

