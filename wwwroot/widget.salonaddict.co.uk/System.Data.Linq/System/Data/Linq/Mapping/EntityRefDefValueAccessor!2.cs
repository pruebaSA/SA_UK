namespace System.Data.Linq.Mapping
{
    using System;
    using System.Data.Linq;

    internal class EntityRefDefValueAccessor<T, V> : MetaAccessor<T, V> where V: class
    {
        private MetaAccessor<T, EntityRef<V>> acc;

        internal EntityRefDefValueAccessor(MetaAccessor<T, EntityRef<V>> acc)
        {
            this.acc = acc;
        }

        public override V GetValue(T instance) => 
            this.acc.GetValue(instance).UnderlyingValue;

        public override void SetValue(ref T instance, V value)
        {
            this.acc.SetValue(ref instance, new EntityRef<V>(value));
        }
    }
}

