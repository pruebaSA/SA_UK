namespace System.Net.NetworkInformation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;

    public class IPAddressInformationCollection : ICollection<IPAddressInformation>, IEnumerable<IPAddressInformation>, IEnumerable
    {
        private Collection<IPAddressInformation> addresses = new Collection<IPAddressInformation>();

        internal IPAddressInformationCollection()
        {
        }

        public virtual void Add(IPAddressInformation address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual void Clear()
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual bool Contains(IPAddressInformation address) => 
            this.addresses.Contains(address);

        public virtual void CopyTo(IPAddressInformation[] array, int offset)
        {
            this.addresses.CopyTo(array, offset);
        }

        public virtual IEnumerator<IPAddressInformation> GetEnumerator() => 
            this.addresses.GetEnumerator();

        internal void InternalAdd(IPAddressInformation address)
        {
            this.addresses.Add(address);
        }

        public virtual bool Remove(IPAddressInformation address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            null;

        public virtual int Count =>
            this.addresses.Count;

        public virtual bool IsReadOnly =>
            true;

        public virtual IPAddressInformation this[int index] =>
            this.addresses[index];
    }
}

