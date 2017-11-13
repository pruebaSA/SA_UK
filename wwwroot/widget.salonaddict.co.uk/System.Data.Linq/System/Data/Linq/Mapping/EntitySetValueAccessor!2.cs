namespace System.Data.Linq.Mapping
{
    using System;
    using System.Data.Linq;

    internal class EntitySetValueAccessor<T, V> : MetaAccessor<T, EntitySet<V>> where V: class
    {
        private MetaAccessor<T, EntitySet<V>> acc;

        internal EntitySetValueAccessor(MetaAccessor<T, EntitySet<V>> acc)
        {
            this.acc = acc;
        }

        public override EntitySet<V> GetValue(T instance) => 
            this.acc.GetValue(instance);

        public override bool HasAssignedValue(object instance)
        {
            EntitySet<V> set = this.acc.GetValue((T) instance);
            return ((set != null) && set.HasAssignedValues);
        }

        public override bool HasLoadedValue(object instance)
        {
            EntitySet<V> set = this.acc.GetValue((T) instance);
            return ((set != null) && set.HasLoadedValues);
        }

        public override bool HasValue(object instance)
        {
            EntitySet<V> set = this.acc.GetValue((T) instance);
            return ((set != null) && set.HasValues);
        }

        public override void SetValue(ref T instance, EntitySet<V> value)
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

