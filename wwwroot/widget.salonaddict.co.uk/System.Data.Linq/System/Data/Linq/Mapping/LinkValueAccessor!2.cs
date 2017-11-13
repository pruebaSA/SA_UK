namespace System.Data.Linq.Mapping
{
    using System;
    using System.Data.Linq;

    internal class LinkValueAccessor<T, V> : MetaAccessor<T, V>
    {
        private MetaAccessor<T, Link<V>> acc;

        internal LinkValueAccessor(MetaAccessor<T, Link<V>> acc)
        {
            this.acc = acc;
        }

        public override V GetValue(T instance) => 
            this.acc.GetValue(instance).Value;

        public override bool HasAssignedValue(object instance) => 
            this.acc.GetValue((T) instance).HasAssignedValue;

        public override bool HasLoadedValue(object instance) => 
            this.acc.GetValue((T) instance).HasLoadedValue;

        public override bool HasValue(object instance) => 
            this.acc.GetValue((T) instance).HasValue;

        public override void SetValue(ref T instance, V value)
        {
            this.acc.SetValue(ref instance, new Link<V>(value));
        }
    }
}

