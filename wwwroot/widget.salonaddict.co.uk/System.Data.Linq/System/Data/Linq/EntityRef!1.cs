namespace System.Data.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct EntityRef<TEntity> where TEntity: class
    {
        private IEnumerable<TEntity> source;
        private TEntity entity;
        public EntityRef(TEntity entity)
        {
            this.entity = entity;
            this.source = SourceState<TEntity>.Assigned;
        }

        public EntityRef(IEnumerable<TEntity> source)
        {
            this.source = source;
            this.entity = default(TEntity);
        }

        public EntityRef(EntityRef<TEntity> entityRef)
        {
            this.source = entityRef.source;
            this.entity = entityRef.entity;
        }

        public TEntity Entity
        {
            get
            {
                if (this.HasSource)
                {
                    this.entity = this.source.SingleOrDefault<TEntity>();
                    this.source = SourceState<TEntity>.Loaded;
                }
                return this.entity;
            }
            set
            {
                this.entity = value;
                this.source = SourceState<TEntity>.Assigned;
            }
        }
        public bool HasLoadedOrAssignedValue
        {
            get
            {
                if (!this.HasLoadedValue)
                {
                    return this.HasAssignedValue;
                }
                return true;
            }
        }
        internal bool HasValue
        {
            get
            {
                if ((this.source != null) && !this.HasLoadedValue)
                {
                    return this.HasAssignedValue;
                }
                return true;
            }
        }
        internal bool HasLoadedValue =>
            (this.source == SourceState<TEntity>.Loaded);
        internal bool HasAssignedValue =>
            (this.source == SourceState<TEntity>.Assigned);
        internal bool HasSource =>
            (((this.source != null) && !this.HasLoadedValue) && !this.HasAssignedValue);
        internal IEnumerable<TEntity> Source =>
            this.source;
        internal TEntity UnderlyingValue =>
            this.entity;
    }
}

