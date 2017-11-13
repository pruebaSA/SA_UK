namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public sealed class EntitySet<TEntity> : IList, ICollection, IList<TEntity>, ICollection<TEntity>, IEnumerable<TEntity>, IEnumerable, IListSource where TEntity: class
    {
        private IBindingList cachedList;
        private ItemList<TEntity> entities;
        private bool isLoaded;
        private bool isModified;
        private bool listChanged;
        private Action<TEntity> onAdd;
        private TEntity onAddEntity;
        private Action<TEntity> onRemove;
        private TEntity onRemoveEntity;
        private ItemList<TEntity> removedEntities;
        private IEnumerable<TEntity> source;
        private int version;

        public event ListChangedEventHandler ListChanged;

        public EntitySet()
        {
        }

        public EntitySet(Action<TEntity> onAdd, Action<TEntity> onRemove)
        {
            this.onAdd = onAdd;
            this.onRemove = onRemove;
        }

        internal EntitySet(EntitySet<TEntity> es, bool copyNotifications)
        {
            this.source = es.source;
            ItemList<TEntity>.Enumerator enumerator = es.entities.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TEntity current = enumerator.Current;
                this.entities.Add(current);
            }
            ItemList<TEntity>.Enumerator enumerator2 = es.removedEntities.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                TEntity item = enumerator2.Current;
                this.removedEntities.Add(item);
            }
            this.version = es.version;
            if (copyNotifications)
            {
                this.onAdd = es.onAdd;
                this.onRemove = es.onRemove;
            }
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            if (entity != this.onAddEntity)
            {
                this.CheckModify();
                if (!this.entities.Contains(entity))
                {
                    this.OnAdd(entity);
                    if (this.HasSource)
                    {
                        this.removedEntities.Remove(entity);
                    }
                    this.entities.Add(entity);
                    if (this.IsLoaded)
                    {
                        this.OnListChanged(ListChangedType.ItemAdded, this.IndexOf(entity));
                    }
                }
                this.OnModified();
            }
        }

        public void AddRange(IEnumerable<TEntity> collection)
        {
            if (collection == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("collection");
            }
            this.CheckModify();
            collection = collection.ToList<TEntity>();
            foreach (TEntity local in collection)
            {
                if (!this.entities.Contains(local))
                {
                    this.OnAdd(local);
                    if (this.HasSource)
                    {
                        this.removedEntities.Remove(local);
                    }
                    this.entities.Add(local);
                    if (this.IsLoaded)
                    {
                        this.OnListChanged(ListChangedType.ItemAdded, this.IndexOf(local));
                    }
                }
            }
            this.OnModified();
        }

        public void Assign(IEnumerable<TEntity> entitySource)
        {
            this.Clear();
            if (entitySource != null)
            {
                this.AddRange(entitySource);
            }
            this.isLoaded = true;
        }

        private void CheckModify()
        {
            if ((this.onAddEntity != null) || (this.onRemoveEntity != null))
            {
                throw System.Data.Linq.Error.ModifyDuringAddOrRemove();
            }
            this.version++;
        }

        public void Clear()
        {
            this.Load();
            this.CheckModify();
            if (this.entities.Items != null)
            {
                List<TEntity> list = new List<TEntity>(this.entities.Items);
                foreach (TEntity local in list)
                {
                    this.Remove(local);
                }
            }
            this.entities = new ItemList<TEntity>();
            this.OnModified();
            this.OnListChanged(ListChangedType.Reset, 0);
        }

        public bool Contains(TEntity entity) => 
            (this.IndexOf(entity) >= 0);

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            this.Load();
            if (this.entities.Count > 0)
            {
                Array.Copy(this.entities.Items, 0, array, arrayIndex, this.entities.Count);
            }
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            this.Load();
            return new Enumerator<TEntity>((EntitySet<TEntity>) this);
        }

        public IBindingList GetNewBindingList() => 
            new EntitySetBindingList<TEntity>(this.ToList<TEntity>(), (EntitySet<TEntity>) this);

        internal IEnumerable<TEntity> GetUnderlyingValues() => 
            new UnderlyingValues<TEntity>((EntitySet<TEntity>) this);

        public int IndexOf(TEntity entity)
        {
            this.Load();
            return this.entities.IndexOf(entity);
        }

        public void Insert(int index, TEntity entity)
        {
            this.Load();
            if ((index < 0) || (index > this.Count))
            {
                throw System.Data.Linq.Error.ArgumentOutOfRange("index");
            }
            if ((entity == null) || (this.IndexOf(entity) >= 0))
            {
                throw System.Data.Linq.Error.ArgumentOutOfRange("entity");
            }
            this.CheckModify();
            this.entities.Insert(index, entity);
            this.OnListChanged(ListChangedType.ItemAdded, index);
            this.OnAdd(entity);
        }

        public void Load()
        {
            if (this.HasSource)
            {
                ItemList<TEntity> entities = this.entities;
                this.entities = new ItemList<TEntity>();
                foreach (TEntity local in this.source)
                {
                    this.entities.Add(local);
                }
                ItemList<TEntity>.Enumerator enumerator = entities.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TEntity current = enumerator.Current;
                    this.entities.Include(current);
                }
                ItemList<TEntity>.Enumerator enumerator3 = this.removedEntities.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    TEntity item = enumerator3.Current;
                    this.entities.Remove(item);
                }
                this.source = SourceState<TEntity>.Loaded;
                this.isLoaded = true;
                this.removedEntities = new ItemList<TEntity>();
            }
        }

        private void OnAdd(TEntity entity)
        {
            if (this.onAdd != null)
            {
                TEntity onAddEntity = this.onAddEntity;
                this.onAddEntity = entity;
                try
                {
                    this.onAdd(entity);
                }
                finally
                {
                    this.onAddEntity = onAddEntity;
                }
            }
        }

        private void OnListChanged(ListChangedType type, int index)
        {
            this.listChanged = true;
            if (this.onListChanged != null)
            {
                this.onListChanged(this, new ListChangedEventArgs(type, index));
            }
        }

        private void OnModified()
        {
            this.isModified = true;
        }

        private void OnRemove(TEntity entity)
        {
            if (this.onRemove != null)
            {
                TEntity onRemoveEntity = this.onRemoveEntity;
                this.onRemoveEntity = entity;
                try
                {
                    this.onRemove(entity);
                }
                finally
                {
                    this.onRemoveEntity = onRemoveEntity;
                }
            }
        }

        public bool Remove(TEntity entity)
        {
            if ((entity == null) || (entity == this.onRemoveEntity))
            {
                return false;
            }
            this.CheckModify();
            int index = -1;
            bool flag = false;
            if (this.HasSource)
            {
                if (!this.removedEntities.Contains(entity))
                {
                    this.OnRemove(entity);
                    index = this.entities.IndexOf(entity);
                    if (index != -1)
                    {
                        this.entities.RemoveAt(index);
                    }
                    else
                    {
                        this.removedEntities.Add(entity);
                    }
                    flag = true;
                }
            }
            else
            {
                index = this.entities.IndexOf(entity);
                if (index != -1)
                {
                    this.OnRemove(entity);
                    this.entities.RemoveAt(index);
                    flag = true;
                }
            }
            if (flag)
            {
                this.OnModified();
                if (this.IsLoaded)
                {
                    this.OnListChanged(ListChangedType.ItemDeleted, index);
                }
            }
            return flag;
        }

        public void RemoveAt(int index)
        {
            this.Load();
            if ((index < 0) || (index >= this.Count))
            {
                throw System.Data.Linq.Error.ArgumentOutOfRange("index");
            }
            this.CheckModify();
            TEntity entity = this.entities[index];
            this.OnRemove(entity);
            this.entities.RemoveAt(index);
            this.OnModified();
            this.OnListChanged(ListChangedType.ItemDeleted, index);
        }

        public void SetSource(IEnumerable<TEntity> entitySource)
        {
            if (this.HasAssignedValues || this.HasLoadedValues)
            {
                throw System.Data.Linq.Error.EntitySetAlreadyLoaded();
            }
            this.source = entitySource;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.Load();
            if (this.entities.Count > 0)
            {
                Array.Copy(this.entities.Items, 0, array, index, this.entities.Count);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        int IList.Add(object value)
        {
            TEntity entity = value as TEntity;
            if ((entity == null) || (this.IndexOf(entity) >= 0))
            {
                throw System.Data.Linq.Error.ArgumentOutOfRange("value");
            }
            this.CheckModify();
            int count = this.entities.Count;
            this.entities.Add(entity);
            this.OnAdd(entity);
            return count;
        }

        bool IList.Contains(object value) => 
            this.Contains(value as TEntity);

        int IList.IndexOf(object value) => 
            this.IndexOf(value as TEntity);

        void IList.Insert(int index, object value)
        {
            TEntity entity = value as TEntity;
            if (value == null)
            {
                throw System.Data.Linq.Error.ArgumentOutOfRange("value");
            }
            this.Insert(index, entity);
        }

        void IList.Remove(object value)
        {
            this.Remove(value as TEntity);
        }

        IList IListSource.GetList()
        {
            if ((this.cachedList == null) || this.listChanged)
            {
                this.cachedList = this.GetNewBindingList();
                this.listChanged = false;
            }
            return this.cachedList;
        }

        public int Count
        {
            get
            {
                this.Load();
                return this.entities.Count;
            }
        }

        internal bool HasAssignedValues =>
            this.isModified;

        public bool HasLoadedOrAssignedValues
        {
            get
            {
                if (!this.HasAssignedValues)
                {
                    return this.HasLoadedValues;
                }
                return true;
            }
        }

        internal bool HasLoadedValues =>
            this.isLoaded;

        internal bool HasSource =>
            ((this.source != null) && !this.HasLoadedValues);

        internal bool HasValues
        {
            get
            {
                if ((this.source != null) && !this.HasAssignedValues)
                {
                    return this.HasLoadedValues;
                }
                return true;
            }
        }

        public bool IsDeferred =>
            this.HasSource;

        internal bool IsLoaded =>
            this.isLoaded;

        public TEntity this[int index]
        {
            get
            {
                this.Load();
                if ((index < 0) || (index >= this.entities.Count))
                {
                    throw System.Data.Linq.Error.ArgumentOutOfRange("index");
                }
                return this.entities[index];
            }
            set
            {
                this.Load();
                if ((index < 0) || (index >= this.entities.Count))
                {
                    throw System.Data.Linq.Error.ArgumentOutOfRange("index");
                }
                if ((value == null) || (this.IndexOf(value) >= 0))
                {
                    throw System.Data.Linq.Error.ArgumentOutOfRange("value");
                }
                this.CheckModify();
                TEntity entity = this.entities[index];
                this.OnRemove(entity);
                this.OnListChanged(ListChangedType.ItemDeleted, index);
                this.OnAdd(value);
                this.entities[index] = value;
                this.OnModified();
                this.OnListChanged(ListChangedType.ItemAdded, index);
            }
        }

        internal IEnumerable<TEntity> Source =>
            this.source;

        bool ICollection<TEntity>.IsReadOnly =>
            false;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            false;

        bool IList.IsReadOnly =>
            false;

        object IList.this[int index]
        {
            get => 
                this[index];
            set
            {
                TEntity local = value as TEntity;
                if (value == null)
                {
                    throw System.Data.Linq.Error.ArgumentOutOfRange("value");
                }
                this[index] = local;
            }
        }

        bool IListSource.ContainsListCollection =>
            true;

        private class Enumerable : IEnumerable<TEntity>, IEnumerable
        {
            private EntitySet<TEntity> entitySet;

            public Enumerable(EntitySet<TEntity> entitySet)
            {
                this.entitySet = entitySet;
            }

            public IEnumerator<TEntity> GetEnumerator() => 
                new EntitySet<TEntity>.Enumerator(this.entitySet);

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();
        }

        private class Enumerator : IEnumerator<TEntity>, IDisposable, IEnumerator
        {
            private int endIndex;
            private EntitySet<TEntity> entitySet;
            private int index;
            private TEntity[] items;
            private int version;

            public Enumerator(EntitySet<TEntity> entitySet)
            {
                this.entitySet = entitySet;
                this.items = entitySet.entities.Items;
                this.index = -1;
                this.endIndex = entitySet.entities.Count - 1;
                this.version = entitySet.version;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this.version != this.entitySet.version)
                {
                    throw System.Data.Linq.Error.EntitySetModifiedDuringEnumeration();
                }
                if (this.index == this.endIndex)
                {
                    return false;
                }
                this.index++;
                return true;
            }

            void IEnumerator.Reset()
            {
                if (this.version != this.entitySet.version)
                {
                    throw System.Data.Linq.Error.EntitySetModifiedDuringEnumeration();
                }
                this.index = -1;
            }

            public TEntity Current =>
                this.items[this.index];

            object IEnumerator.Current =>
                this.items[this.index];
        }

        private class UnderlyingValues : IEnumerable<TEntity>, IEnumerable
        {
            private EntitySet<TEntity> entitySet;

            internal UnderlyingValues(EntitySet<TEntity> entitySet)
            {
                this.entitySet = entitySet;
            }

            public IEnumerator<TEntity> GetEnumerator() => 
                new EntitySet<TEntity>.Enumerator(this.entitySet);

            IEnumerator IEnumerable.GetEnumerator() => 
                this.GetEnumerator();
        }
    }
}

