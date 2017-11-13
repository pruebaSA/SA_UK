namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;

    internal class EntitySetDefValueAccessor<T, V> : MetaAccessor<T, IEnumerable<V>> where V: class
    {
        private MetaAccessor<T, EntitySet<V>> acc;

        internal EntitySetDefValueAccessor(MetaAccessor<T, EntitySet<V>> acc)
        {
            this.acc = acc;
        }

        public override IEnumerable<V> GetValue(T instance) => 
            this.acc.GetValue(instance).GetUnderlyingValues();

        public override void SetValue(ref T instance, IEnumerable<V> value)
        {
            EntitySet<V> set = this.acc.GetValue(instance);
            if (set == null)
            {
                set = new EntitySet<V>();
                this.acc.SetValue(ref instance, set);
            }
            set.Assign(value);
        }
    }
}

