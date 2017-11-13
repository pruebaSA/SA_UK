namespace System.Data.Linq.Mapping
{
    using System;
    using System.Data.Linq;

    internal class LinkDefValueAccessor<T, V> : MetaAccessor<T, V>
    {
        private MetaAccessor<T, Link<V>> acc;

        internal LinkDefValueAccessor(MetaAccessor<T, Link<V>> acc)
        {
            this.acc = acc;
        }

        public override V GetValue(T instance) => 
            this.acc.GetValue(instance).UnderlyingValue;

        public override void SetValue(ref T instance, V value)
        {
            this.acc.SetValue(ref instance, new Link<V>(value));
        }
    }
}

