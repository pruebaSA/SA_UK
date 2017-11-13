﻿namespace System.Net.NetworkInformation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Reflection;

    public class IPAddressCollection : ICollection<IPAddress>, IEnumerable<IPAddress>, IEnumerable
    {
        private Collection<IPAddress> addresses = new Collection<IPAddress>();

        protected internal IPAddressCollection()
        {
        }

        public virtual void Add(IPAddress address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual void Clear()
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        public virtual bool Contains(IPAddress address) => 
            this.addresses.Contains(address);

        public virtual void CopyTo(IPAddress[] array, int offset)
        {
            this.addresses.CopyTo(array, offset);
        }

        public virtual IEnumerator<IPAddress> GetEnumerator() => 
            this.addresses.GetEnumerator();

        internal void InternalAdd(IPAddress address)
        {
            this.addresses.Add(address);
        }

        public virtual bool Remove(IPAddress address)
        {
            throw new NotSupportedException(SR.GetString("net_collection_readonly"));
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            null;

        public virtual int Count =>
            this.addresses.Count;

        public virtual bool IsReadOnly =>
            true;

        public virtual IPAddress this[int index] =>
            this.addresses[index];
    }
}

