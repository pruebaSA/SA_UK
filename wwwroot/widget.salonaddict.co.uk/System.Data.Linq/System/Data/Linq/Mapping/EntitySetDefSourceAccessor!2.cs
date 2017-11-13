namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;

    internal class EntitySetDefSourceAccessor<T, V> : MetaAccessor<T, IEnumerable<V>> where V: class
    {
        private MetaAccessor<T, EntitySet<V>> acc;

        internal EntitySetDefSourceAccessor(MetaAccessor<T, EntitySet<V>> acc)
        {
            this.acc = acc;
        }

        public override IEnumerable<V> GetValue(T instance) => 
            this.acc.GetValue(instance).Source;

        public override void SetValue(ref T instance, IEnumerable<V> value)
        {
            EntitySet<V> set = this.acc.GetValue(instance);
            if (set == null)
            {
                set = new EntitySet<V>();
                this.acc.SetValue(ref instance, set);
            }
            set.SetSource(value);
        }
    }
}

