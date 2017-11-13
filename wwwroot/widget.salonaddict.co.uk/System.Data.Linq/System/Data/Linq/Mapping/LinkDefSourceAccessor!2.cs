namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;

    internal class LinkDefSourceAccessor<T, V> : MetaAccessor<T, IEnumerable<V>>
    {
        private MetaAccessor<T, Link<V>> acc;

        internal LinkDefSourceAccessor(MetaAccessor<T, Link<V>> acc)
        {
            this.acc = acc;
        }

        public override IEnumerable<V> GetValue(T instance) => 
            this.acc.GetValue(instance).Source;

        public override void SetValue(ref T instance, IEnumerable<V> value)
        {
            Link<V> link = this.acc.GetValue(instance);
            if (link.HasAssignedValue || link.HasLoadedValue)
            {
                throw System.Data.Linq.Mapping.Error.LinkAlreadyLoaded();
            }
            this.acc.SetValue(ref instance, new Link<V>(value));
        }
    }
}

