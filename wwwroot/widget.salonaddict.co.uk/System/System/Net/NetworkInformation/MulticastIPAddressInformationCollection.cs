namespace System.Net.NetworkInformation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    public class MulticastIPAddressInformationCollection : ICollection<MulticastIPAddressInformation>, IEnumerable<MulticastIPAddressInformation>, IEnumerable
    {
        private Collection<MulticastIPAddressInformation> addresses = new Collection<MulticastIPAddressInformation>();

        protected internal MulticastIPAddressInformationCollection()
        {
        }

        public virtual void Add(MulticastIPAddressInformation address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual void Clear()
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual bool Contains(MulticastIPAddressInformation address) => 
            this.addresses.Contains(address);

        public virtual void CopyTo(MulticastIPAddressInformation[] array, int offset)
        {
            this.addresses.CopyTo(array, offset);
        }

        public virtual IEnumerator<MulticastIPAddressInformation> GetEnumerator() => 
            this.addresses.GetEnumerator();

        internal void InternalAdd(MulticastIPAddressInformation address)
        {
            this.addresses.Add(address);
        }

        public virtual bool Remove(MulticastIPAddressInformation address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            null;

        public virtual int Count =>
            this.addresses.Count;

        public virtual bool IsReadOnly =>
            true;

        public virtual MulticastIPAddressInformation this[int index] =>
            this.addresses[index];
    }
}

