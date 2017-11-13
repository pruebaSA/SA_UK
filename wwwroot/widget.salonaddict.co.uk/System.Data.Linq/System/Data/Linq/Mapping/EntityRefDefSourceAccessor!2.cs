namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;

    internal class EntityRefDefSourceAccessor<T, V> : MetaAccessor<T, IEnumerable<V>> where V: class
    {
        private MetaAccessor<T, EntityRef<V>> acc;

        internal EntityRefDefSourceAccessor(MetaAccessor<T, EntityRef<V>> acc)
        {
            this.acc = acc;
        }

        public override IEnumerable<V> GetValue(T instance) => 
            this.acc.GetValue(instance).Source;

        public override void SetValue(ref T instance, IEnumerable<V> value)
        {
            EntityRef<V> ref2 = this.acc.GetValue(instance);
            if (ref2.HasAssignedValue || ref2.HasLoadedValue)
            {
                throw System.Data.Linq.Mapping.Error.EntityRefAlreadyLoaded();
            }
            this.acc.SetValue(ref instance, new EntityRef<V>(value));
        }
    }
}

