namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.Internal;
    using System.Linq;
    using System.Reflection;

    public class ObjectQuery<T> : ObjectQuery, IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable, IListSource
    {
        private string _name;
        private const string DefaultName = "it";

        internal ObjectQuery(ObjectQueryState queryState) : base(queryState)
        {
            this._name = "it";
        }

        public ObjectQuery(string commandText, ObjectContext context) : this(new EntitySqlQueryState(typeof(T), commandText, false, context, null, null))
        {
            context.MetadataWorkspace.LoadAssemblyForType(typeof(T), Assembly.GetCallingAssembly());
        }

        public ObjectQuery(string commandText, ObjectContext context, MergeOption mergeOption) : this(new EntitySqlQueryState(typeof(T), commandText, false, context, null, null))
        {
            EntityUtil.CheckArgumentMergeOption(mergeOption);
            base.QueryState.UserSpecifiedMergeOption = new MergeOption?(mergeOption);
            context.MetadataWorkspace.LoadAssemblyForType(typeof(T), Assembly.GetCallingAssembly());
        }

        public ObjectQuery<T> Distinct() => 
            new ObjectQuery<T>(EntitySqlQueryBuilder.Distinct(base.QueryState));

        public ObjectQuery<T> Except(ObjectQuery<T> query)
        {
            EntityUtil.CheckArgumentNull<ObjectQuery<T>>(query, "query");
            return new ObjectQuery<T>(EntitySqlQueryBuilder.Except(base.QueryState, query.QueryState));
        }

        public ObjectResult<T> Execute(MergeOption mergeOption)
        {
            EntityUtil.CheckArgumentMergeOption(mergeOption);
            return this.GetResults(new MergeOption?(mergeOption));
        }

        internal override ObjectResult ExecuteInternal(MergeOption mergeOption) => 
            this.GetResults(new MergeOption?(mergeOption));

        internal override IEnumerator GetEnumeratorInternal() => 
            ((IEnumerable<T>) this).GetEnumerator();

        internal override IList GetIListSourceListInternal() => 
            ((IListSource) this.GetResults(null)).GetList();

        private ObjectResult<T> GetResults(MergeOption? forMergeOption)
        {
            ObjectResult<T> result;
            base.QueryState.ObjectContext.EnsureConnection();
            try
            {
                result = base.QueryState.GetExecutionPlan(forMergeOption).Execute<T>(base.QueryState.ObjectContext, base.QueryState.Parameters);
            }
            catch
            {
                base.QueryState.ObjectContext.ReleaseConnection();
                throw;
            }
            return result;
        }

        public ObjectQuery<DbDataRecord> GroupBy(string keys, string projection, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(keys, "keys");
            EntityUtil.CheckArgumentNull<string>(projection, "projection");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(keys))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidGroupKeyList, "keys");
            }
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(projection))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidProjectionList, "projection");
            }
            return new ObjectQuery<DbDataRecord>(EntitySqlQueryBuilder.GroupBy(base.QueryState, this.Name, keys, projection, parameters));
        }

        public ObjectQuery<T> Include(string path)
        {
            EntityUtil.CheckStringArgument(path, "path");
            return new ObjectQuery<T>(base.QueryState.Include<T>((ObjectQuery<T>) this, path));
        }

        public ObjectQuery<T> Intersect(ObjectQuery<T> query)
        {
            EntityUtil.CheckArgumentNull<ObjectQuery<T>>(query, "query");
            return new ObjectQuery<T>(EntitySqlQueryBuilder.Intersect(base.QueryState, query.QueryState));
        }

        public ObjectQuery<TResultType> OfType<TResultType>()
        {
            base.QueryState.ObjectContext.MetadataWorkspace.LoadAssemblyForType(typeof(TResultType), Assembly.GetCallingAssembly());
            Type clrOfType = typeof(TResultType);
            EdmType type = null;
            if (!base.QueryState.ObjectContext.MetadataWorkspace.GetItemCollection(DataSpace.OSpace).TryGetType(clrOfType.Name, clrOfType.Namespace ?? string.Empty, out type) || (!Helper.IsEntityType(type) && !Helper.IsComplexType(type)))
            {
                throw EntityUtil.EntitySqlError(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidResultType(typeof(TResultType).FullName));
            }
            return new ObjectQuery<TResultType>(EntitySqlQueryBuilder.OfType(base.QueryState, type, clrOfType));
        }

        public ObjectQuery<T> OrderBy(string keys, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(keys, "keys");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(keys))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidSortKeyList, "keys");
            }
            return new ObjectQuery<T>(EntitySqlQueryBuilder.OrderBy(base.QueryState, this.Name, keys, parameters));
        }

        public ObjectQuery<DbDataRecord> Select(string projection, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(projection, "projection");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(projection))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidProjectionList, "projection");
            }
            return new ObjectQuery<DbDataRecord>(EntitySqlQueryBuilder.Select(base.QueryState, this.Name, projection, parameters));
        }

        public ObjectQuery<TResultType> SelectValue<TResultType>(string projection, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(projection, "projection");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(projection))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidProjectionList, "projection");
            }
            base.QueryState.ObjectContext.MetadataWorkspace.LoadAssemblyForType(typeof(TResultType), Assembly.GetCallingAssembly());
            return new ObjectQuery<TResultType>(EntitySqlQueryBuilder.SelectValue(base.QueryState, this.Name, projection, parameters, typeof(TResultType)));
        }

        public ObjectQuery<T> Skip(string keys, string count, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(keys, "keys");
            EntityUtil.CheckArgumentNull<string>(count, "count");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(keys))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidSortKeyList, "keys");
            }
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(count))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidSkipCount, "count");
            }
            return new ObjectQuery<T>(EntitySqlQueryBuilder.Skip(base.QueryState, this.Name, keys, count, parameters));
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            IEnumerator<T> enumerator;
            ObjectResult<T> results = this.GetResults(null);
            try
            {
                enumerator = results.GetEnumerator();
            }
            catch
            {
                results.Dispose();
                throw;
            }
            return enumerator;
        }

        public ObjectQuery<T> Top(string count, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(count, "count");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(count))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidTopCount, "count");
            }
            return new ObjectQuery<T>(EntitySqlQueryBuilder.Top(base.QueryState, this.Name, count, parameters));
        }

        public ObjectQuery<T> Union(ObjectQuery<T> query)
        {
            EntityUtil.CheckArgumentNull<ObjectQuery<T>>(query, "query");
            return new ObjectQuery<T>(EntitySqlQueryBuilder.Union(base.QueryState, query.QueryState));
        }

        public ObjectQuery<T> UnionAll(ObjectQuery<T> query)
        {
            EntityUtil.CheckArgumentNull<ObjectQuery<T>>(query, "query");
            return new ObjectQuery<T>(EntitySqlQueryBuilder.UnionAll(base.QueryState, query.QueryState));
        }

        public ObjectQuery<T> Where(string predicate, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(predicate, "predicate");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            if (StringUtil.IsNullOrEmptyOrWhiteSpace(predicate))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_QueryBuilder_InvalidFilterPredicate, "predicate");
            }
            return new ObjectQuery<T>(EntitySqlQueryBuilder.Where(base.QueryState, this.Name, predicate, parameters));
        }

        public string Name
        {
            get => 
                this._name;
            set
            {
                EntityUtil.CheckArgumentNull<string>(value, "value");
                if (!ObjectParameter.ValidateParameterName(value))
                {
                    throw EntityUtil.Argument(System.Data.Entity.Strings.ObjectQuery_InvalidQueryName(value), "value");
                }
                this._name = value;
            }
        }
    }
}

