namespace System.Data.Linq.Mapping
{
    using System;

    public abstract class MetaAccessor
    {
        protected MetaAccessor()
        {
        }

        public abstract object GetBoxedValue(object instance);
        public virtual bool HasAssignedValue(object instance) => 
            true;

        public virtual bool HasLoadedValue(object instance) => 
            false;

        public virtual bool HasValue(object instance) => 
            true;

        public abstract void SetBoxedValue(ref object instance, object value);

        public abstract System.Type Type { get; }
    }
}

