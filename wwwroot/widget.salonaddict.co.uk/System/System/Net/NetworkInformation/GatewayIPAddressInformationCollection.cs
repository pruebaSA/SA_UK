namespace System.Net.NetworkInformation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    public class GatewayIPAddressInformationCollection : ICollection<GatewayIPAddressInformation>, IEnumerable<GatewayIPAddressInformation>, IEnumerable
    {
        private Collection<GatewayIPAddressInformation> addresses = new Collection<GatewayIPAddressInformation>();

        protected internal GatewayIPAddressInformationCollection()
        {
        }

        public virtual void Add(GatewayIPAddressInformation address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual void Clear()
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual bool Contains(GatewayIPAddressInformation address) => 
            this.addresses.Contains(address);

        public virtual void CopyTo(GatewayIPAddressInformation[] array, int offset)
        {
            this.addresses.CopyTo(array, offset);
        }

        public virtual IEnumerator<GatewayIPAddressInformation> GetEnumerator() => 
            this.addresses.GetEnumerator();

        internal void InternalAdd(GatewayIPAddressInformation address)
        {
            this.addresses.Add(address);
        }

        public virtual bool Remove(GatewayIPAddressInformation address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            null;

        public virtual int Count =>
            this.addresses.Count;

        public virtual bool IsReadOnly =>
            true;

        public virtual GatewayIPAddressInformation this[int index] =>
            this.addresses[index];
    }
}

