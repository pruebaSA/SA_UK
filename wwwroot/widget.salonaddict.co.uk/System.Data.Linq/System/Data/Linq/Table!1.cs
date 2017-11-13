namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Data.Linq.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;

    public sealed class Table<TEntity> : IQueryable<TEntity>, IQueryProvider, IEnumerable<TEntity>, ITable, IQueryable, IEnumerable, IListSource where TEntity: class
    {
        private IBindingList cachedList;
        private DataContext context;
        private MetaTable metaTable;

        internal Table(DataContext context, MetaTable metaTable)
        {
            this.context = context;
            this.metaTable = metaTable;
        }

        public void Attach(TEntity entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            this.Attach(entity, false);
        }

        public void Attach(TEntity entity, bool asModified)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            MetaType inheritanceType = this.metaTable.RowType.GetInheritanceType(entity.GetType());
            if (!Table<TEntity>.IsTrackableType(inheritanceType))
            {
                throw System.Data.Linq.Error.TypeCouldNotBeTracked(inheritanceType.Type);
            }
            if (asModified && ((inheritanceType.VersionMember == null) && inheritanceType.HasUpdateCheck))
            {
                throw System.Data.Linq.Error.CannotAttachAsModifiedWithoutOriginalState();
            }
            TrackedObject trackedObject = this.Context.Services.ChangeTracker.GetTrackedObject(entity);
            if ((trackedObject != null) && !trackedObject.IsWeaklyTracked)
            {
                throw System.Data.Linq.Error.CannotAttachAlreadyExistingEntity();
            }
            if (trackedObject == null)
            {
                trackedObject = this.context.Services.ChangeTracker.Track(entity, true);
            }
            if (asModified)
            {
                trackedObject.ConvertToModified();
            }
            else
            {
                trackedObject.ConvertToUnmodified();
            }
            if (this.Context.Services.InsertLookupCachedObject(inheritanceType, entity) != entity)
            {
                throw new DuplicateKeyException(entity, System.Data.Linq.Strings.CantAddAlreadyExistingKey);
            }
            trackedObject.InitializeDeferredLoaders();
        }

        public void Attach(TEntity entity, TEntity original)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            if (original == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("original");
            }
            if (entity.GetType() != original.GetType())
            {
                throw System.Data.Linq.Error.OriginalEntityIsWrongType();
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            MetaType inheritanceType = this.metaTable.RowType.GetInheritanceType(entity.GetType());
            if (!Table<TEntity>.IsTrackableType(inheritanceType))
            {
                throw System.Data.Linq.Error.TypeCouldNotBeTracked(inheritanceType.Type);
            }
            TrackedObject trackedObject = this.context.Services.ChangeTracker.GetTrackedObject(entity);
            if ((trackedObject != null) && !trackedObject.IsWeaklyTracked)
            {
                throw System.Data.Linq.Error.CannotAttachAlreadyExistingEntity();
            }
            if (trackedObject == null)
            {
                trackedObject = this.context.Services.ChangeTracker.Track(entity, true);
            }
            trackedObject.ConvertToPossiblyModified(original);
            if (this.Context.Services.InsertLookupCachedObject(inheritanceType, entity) != entity)
            {
                throw new DuplicateKeyException(entity, System.Data.Linq.Strings.CantAddAlreadyExistingKey);
            }
            trackedObject.InitializeDeferredLoaders();
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity: TEntity
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.AttachAll<TSubEntity>(entities, false);
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, bool asModified) where TSubEntity: TEntity
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            foreach (TEntity local in entities.ToList<TSubEntity>())
            {
                this.Attach(local, asModified);
            }
        }

        private void CheckReadOnly()
        {
            if (this.IsReadOnly)
            {
                throw System.Data.Linq.Error.CannotPerformCUDOnReadOnlyTable(this.ToString());
            }
        }

        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity: TEntity
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            foreach (TEntity local in entities.ToList<TSubEntity>())
            {
                this.DeleteOnSubmit(local);
            }
        }

        public void DeleteOnSubmit(TEntity entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            TrackedObject trackedObject = this.context.Services.ChangeTracker.GetTrackedObject(entity);
            if (trackedObject == null)
            {
                throw System.Data.Linq.Error.CannotRemoveUnattachedEntity();
            }
            if (trackedObject.IsNew)
            {
                trackedObject.ConvertToRemoved();
            }
            else if (trackedObject.IsPossiblyModified || trackedObject.IsModified)
            {
                trackedObject.ConvertToDeleted();
            }
        }

        public IEnumerator<TEntity> GetEnumerator() => 
            ((IEnumerable<TEntity>) this.context.Provider.Execute(Expression.Constant(this)).ReturnValue).GetEnumerator();

        public ModifiedMemberInfo[] GetModifiedMembers(TEntity entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            MetaType metaType = this.Context.Mapping.GetMetaType(entity.GetType());
            if ((metaType == null) || !metaType.IsEntity)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            TrackedObject trackedObject = this.Context.Services.ChangeTracker.GetTrackedObject(entity);
            if (trackedObject != null)
            {
                return trackedObject.GetModifiedMembers().ToArray<ModifiedMemberInfo>();
            }
            return new ModifiedMemberInfo[0];
        }

        public IBindingList GetNewBindingList() => 
            BindingList.Create<TEntity>(this.context, this);

        public TEntity GetOriginalEntityState(TEntity entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            MetaType metaType = this.Context.Mapping.GetMetaType(entity.GetType());
            if ((metaType == null) || !metaType.IsEntity)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            TrackedObject trackedObject = this.Context.Services.ChangeTracker.GetTrackedObject(entity);
            if (trackedObject == null)
            {
                return default(TEntity);
            }
            if (trackedObject.Original != null)
            {
                return (TEntity) trackedObject.CreateDataCopy(trackedObject.Original);
            }
            return (TEntity) trackedObject.CreateDataCopy(trackedObject.Current);
        }

        public void InsertAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity: TEntity
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            foreach (TEntity local in entities.ToList<TSubEntity>())
            {
                this.InsertOnSubmit(local);
            }
        }

        public void InsertOnSubmit(TEntity entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            MetaType inheritanceType = this.metaTable.RowType.GetInheritanceType(entity.GetType());
            if (!Table<TEntity>.IsTrackableType(inheritanceType))
            {
                throw System.Data.Linq.Error.TypeCouldNotBeAdded(inheritanceType.Type);
            }
            TrackedObject trackedObject = this.context.Services.ChangeTracker.GetTrackedObject(entity);
            if (trackedObject == null)
            {
                this.context.Services.ChangeTracker.Track(entity).ConvertToNew();
            }
            else if (trackedObject.IsWeaklyTracked)
            {
                trackedObject.ConvertToNew();
            }
            else if (trackedObject.IsDeleted)
            {
                trackedObject.ConvertToPossiblyModified();
            }
            else if (trackedObject.IsRemoved)
            {
                trackedObject.ConvertToNew();
            }
            else if (!trackedObject.IsNew)
            {
                throw System.Data.Linq.Error.CantAddAlreadyExistingItem();
            }
        }

        private static bool IsTrackableType(MetaType type)
        {
            if (type == null)
            {
                return false;
            }
            if (!type.CanInstantiate)
            {
                return false;
            }
            if (type.HasInheritance && !type.HasInheritanceCode)
            {
                return false;
            }
            return true;
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator() => 
            this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        IList IListSource.GetList()
        {
            if (this.cachedList == null)
            {
                this.cachedList = this.GetNewBindingList();
            }
            return this.cachedList;
        }

        void ITable.Attach(object entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            TEntity local = entity as TEntity;
            if (local == null)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            this.Attach(local, false);
        }

        void ITable.Attach(object entity, bool asModified)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            TEntity local = entity as TEntity;
            if (local == null)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            this.Attach(local, asModified);
        }

        void ITable.Attach(object entity, object original)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            if (original == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("original");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            TEntity local = entity as TEntity;
            if (local == null)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            if (entity.GetType() != original.GetType())
            {
                throw System.Data.Linq.Error.OriginalEntityIsWrongType();
            }
            this.Attach(local, (TEntity) original);
        }

        void ITable.AttachAll(IEnumerable entities)
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            ((ITable) this).AttachAll(entities, false);
        }

        void ITable.AttachAll(IEnumerable entities, bool asModified)
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            List<object> list = entities.Cast<object>().ToList<object>();
            ITable table = this;
            foreach (object obj2 in list)
            {
                table.Attach(obj2, asModified);
            }
        }

        void ITable.DeleteAllOnSubmit(IEnumerable entities)
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            List<object> list = entities.Cast<object>().ToList<object>();
            ITable table = this;
            foreach (object obj2 in list)
            {
                table.DeleteOnSubmit(obj2);
            }
        }

        void ITable.DeleteOnSubmit(object entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            TEntity local = entity as TEntity;
            if (local == null)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            this.DeleteOnSubmit(local);
        }

        ModifiedMemberInfo[] ITable.GetModifiedMembers(object entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            TEntity local = entity as TEntity;
            if (local == null)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            return this.GetModifiedMembers(local);
        }

        object ITable.GetOriginalEntityState(object entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            TEntity local = entity as TEntity;
            if (local == null)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            return this.GetOriginalEntityState(local);
        }

        void ITable.InsertAllOnSubmit(IEnumerable entities)
        {
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.CheckReadOnly();
            this.context.CheckNotInSubmitChanges();
            this.context.VerifyTrackingEnabled();
            List<object> list = entities.Cast<object>().ToList<object>();
            ITable table = this;
            foreach (object obj2 in list)
            {
                table.InsertOnSubmit(obj2);
            }
        }

        void ITable.InsertOnSubmit(object entity)
        {
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            TEntity local = entity as TEntity;
            if (local == null)
            {
                throw System.Data.Linq.Error.EntityIsTheWrongType();
            }
            this.InsertOnSubmit(local);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            Type elementType = TypeSystem.GetElementType(expression.Type);
            Type type2 = typeof(IQueryable<>).MakeGenericType(new Type[] { elementType });
            if (!type2.IsAssignableFrom(expression.Type))
            {
                throw System.Data.Linq.Error.ExpectedQueryableArgument("expression", type2);
            }
            return (IQueryable) Activator.CreateInstance(typeof(DataQuery<>).MakeGenericType(new Type[] { elementType }), new object[] { this.context, expression });
        }

        IQueryable<TResult> IQueryProvider.CreateQuery<TResult>(Expression expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            if (!typeof(IQueryable<TResult>).IsAssignableFrom(expression.Type))
            {
                throw System.Data.Linq.Error.ExpectedQueryableArgument("expression", typeof(IEnumerable<TResult>));
            }
            return new DataQuery<TResult>(this.context, expression);
        }

        object IQueryProvider.Execute(Expression expression) => 
            this.context.Provider.Execute(expression).ReturnValue;

        TResult IQueryProvider.Execute<TResult>(Expression expression) => 
            ((TResult) this.context.Provider.Execute(expression).ReturnValue);

        public override string ToString() => 
            ("Table(" + typeof(TEntity).Name + ")");

        public DataContext Context =>
            this.context;

        public bool IsReadOnly =>
            !this.metaTable.RowType.IsEntity;

        bool IListSource.ContainsListCollection =>
            false;

        Type IQueryable.ElementType =>
            typeof(TEntity);

        Expression IQueryable.Expression =>
            Expression.Constant(this);

        IQueryProvider IQueryable.Provider =>
            this;
    }
}

