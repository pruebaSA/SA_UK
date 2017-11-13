namespace System.Data.Linq.Mapping
{
    using System;

    public abstract class MetaAccessor<TEntity, TMember> : MetaAccessor
    {
        protected MetaAccessor()
        {
        }

        public override object GetBoxedValue(object instance) => 
            this.GetValue((TEntity) instance);

        public abstract TMember GetValue(TEntity instance);
        public override void SetBoxedValue(ref object instance, object value)
        {
            TEntity local = (TEntity) instance;
            this.SetValue(ref local, (TMember) value);
            instance = local;
        }

        public abstract void SetValue(ref TEntity instance, TMember value);

        public override System.Type Type =>
            typeof(TMember);
    }
}

