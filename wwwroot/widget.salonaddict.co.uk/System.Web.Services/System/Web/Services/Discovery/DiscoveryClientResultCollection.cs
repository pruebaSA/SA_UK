namespace System.Web.Services.Discovery
{
    using System;
    using System.Collections;
    using System.Reflection;

    public sealed class DiscoveryClientResultCollection : CollectionBase
    {
        public int Add(DiscoveryClientResult value) => 
            base.List.Add(value);

        public bool Contains(DiscoveryClientResult value) => 
            base.List.Contains(value);

        public void Remove(DiscoveryClientResult value)
        {
            base.List.Remove(value);
        }

        public DiscoveryClientResult this[int i]
        {
            get => 
                ((DiscoveryClientResult) base.List[i]);
            set
            {
                base.List[i] = value;
            }
        }
    }
}

