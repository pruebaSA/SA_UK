namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.Data.Linq.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class CommonDataServices : IDataServices
    {
        private DataContext context;
        private System.Data.Linq.ChangeDirector director;
        private Dictionary<MetaDataMember, IDeferredSourceFactory> factoryMap;
        private bool hasCachedObjects;
        private System.Data.Linq.IdentityManager identifier;
        private MetaModel metaModel;
        private System.Data.Linq.ChangeTracker tracker;

        internal CommonDataServices(DataContext context, MetaModel model)
        {
            this.context = context;
            this.metaModel = model;
            bool asReadOnly = !context.ObjectTrackingEnabled;
            this.identifier = System.Data.Linq.IdentityManager.CreateIdentityManager(asReadOnly);
            this.tracker = System.Data.Linq.ChangeTracker.CreateChangeTracker(this, asReadOnly);
            this.director = System.Data.Linq.ChangeDirector.CreateChangeDirector(context);
            this.factoryMap = new Dictionary<MetaDataMember, IDeferredSourceFactory>();
        }

        private static Expression[] BuildKeyExpressions(object[] keyValues, ReadOnlyCollection<MetaDataMember> keyMembers)
        {
            Expression[] expressionArray = new Expression[keyValues.Length];
            int index = 0;
            int count = keyMembers.Count;
            while (index < count)
            {
                MetaDataMember member = keyMembers[index];
                expressionArray[index] = Expression.Constant(keyValues[index], member.Type);
                index++;
            }
            return expressionArray;
        }

        public object GetCachedObject(Expression query)
        {
            if (query != null)
            {
                string str;
                MethodCallExpression expression = query as MethodCallExpression;
                if ((expression == null) || (expression.Arguments.Count != 2))
                {
                    return null;
                }
                if ((expression.Method.DeclaringType != typeof(Queryable)) && (((str = expression.Method.Name) == null) || ((((str != "Where") && (str != "First")) && ((str != "FirstOrDefault") && (str != "Single"))) && (str != "SingleOrDefault"))))
                {
                    return null;
                }
                UnaryExpression expression2 = expression.Arguments[1] as UnaryExpression;
                if ((expression2 == null) || (expression2.NodeType != ExpressionType.Quote))
                {
                    return null;
                }
                LambdaExpression operand = expression2.Operand as LambdaExpression;
                if (operand != null)
                {
                    ConstantExpression expression4 = expression.Arguments[0] as ConstantExpression;
                    if (expression4 == null)
                    {
                        return null;
                    }
                    ITable table = expression4.Value as ITable;
                    if (table == null)
                    {
                        return null;
                    }
                    if (TypeSystem.GetElementType(query.Type) != table.ElementType)
                    {
                        return null;
                    }
                    MetaTable table2 = this.metaModel.GetTable(table.ElementType);
                    object[] keyValues = this.GetKeyValues(table2.RowType, operand);
                    if (keyValues != null)
                    {
                        return this.GetCachedObject(table2.RowType, keyValues);
                    }
                }
            }
            return null;
        }

        internal object GetCachedObject(MetaType type, object[] keyValues)
        {
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            if (!type.IsEntity)
            {
                return null;
            }
            return this.identifier.Find(type, keyValues);
        }

        internal object GetCachedObjectLike(MetaType type, object instance)
        {
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            if (!type.IsEntity)
            {
                return null;
            }
            return this.identifier.FindLike(type, instance);
        }

        internal IEnumerable<RelatedItem> GetChildren(MetaType type, object item) => 
            this.GetRelations(type, item, false);

        internal Expression GetDataMemberQuery(MetaDataMember member, Expression[] keyValues)
        {
            if (member == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("member");
            }
            if (keyValues == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("keyValues");
            }
            if (member.IsAssociation)
            {
                MetaAssociation association = member.Association;
                Type type = association.ThisMember.DeclaringType.InheritanceRoot.Type;
                Expression source = Expression.Constant(this.context.GetTable(type));
                if (type != association.ThisMember.DeclaringType.Type)
                {
                    source = Expression.Call(typeof(Enumerable), "Cast", new Type[] { association.ThisMember.DeclaringType.Type }, new Expression[] { source });
                }
                Expression thisInstance = Expression.Call(typeof(Enumerable), "FirstOrDefault", new Type[] { association.ThisMember.DeclaringType.Type }, new Expression[] { Translator.WhereClauseFromSourceAndKeys(source, association.ThisKey.ToArray<MetaDataMember>(), keyValues) });
                Expression otherSource = Expression.Constant(this.context.GetTable(association.OtherType.InheritanceRoot.Type));
                if (association.OtherType.Type != association.OtherType.InheritanceRoot.Type)
                {
                    otherSource = Expression.Call(typeof(Enumerable), "Cast", new Type[] { association.OtherType.Type }, new Expression[] { otherSource });
                }
                return Translator.TranslateAssociation(this.context, association, otherSource, keyValues, thisInstance);
            }
            Expression objectQuery = this.GetObjectQuery(member.DeclaringType, keyValues);
            Type elementType = TypeSystem.GetElementType(objectQuery.Type);
            ParameterExpression expression6 = Expression.Parameter(elementType, "p");
            Expression expression7 = expression6;
            if (elementType != member.DeclaringType.Type)
            {
                expression7 = Expression.Convert(expression7, member.DeclaringType.Type);
            }
            Expression body = (member.Member is PropertyInfo) ? Expression.Property(expression7, (PropertyInfo) member.Member) : Expression.Field(expression7, (FieldInfo) member.Member);
            LambdaExpression expression9 = Expression.Lambda(body, new ParameterExpression[] { expression6 });
            return Expression.Call(typeof(Queryable), "Select", new Type[] { elementType, expression9.Body.Type }, new Expression[] { objectQuery, expression9 });
        }

        public IDeferredSourceFactory GetDeferredSourceFactory(MetaDataMember member)
        {
            IDeferredSourceFactory factory;
            if (member == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("member");
            }
            if (!this.factoryMap.TryGetValue(member, out factory))
            {
                Type type = (member.IsAssociation && member.Association.IsMany) ? TypeSystem.GetElementType(member.Type) : member.Type;
                factory = (IDeferredSourceFactory) Activator.CreateInstance(typeof(DeferredSourceFactory).MakeGenericType(new Type[] { type }), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { member, this }, null);
                this.factoryMap.Add(member, factory);
            }
            return factory;
        }

        internal static object[] GetForeignKeyValues(MetaAssociation association, object instance)
        {
            List<object> list = new List<object>();
            foreach (MetaDataMember member in association.ThisKey)
            {
                list.Add(member.MemberAccessor.GetBoxedValue(instance));
            }
            return list.ToArray();
        }

        private static bool GetKeyFromPredicate(MetaType type, Dictionary<MetaDataMember, object> keys, Expression mex, Expression vex)
        {
            MemberExpression expression = mex as MemberExpression;
            if ((((expression != null) && (expression.Expression != null)) && ((expression.Expression.NodeType == ExpressionType.Parameter) && (expression.Expression.Type == type.Type))) && (type.Type.IsAssignableFrom(expression.Member.ReflectedType) || expression.Member.ReflectedType.IsAssignableFrom(type.Type)))
            {
                MetaDataMember dataMember = type.GetDataMember(expression.Member);
                if (!dataMember.IsPrimaryKey)
                {
                    return false;
                }
                if (keys.ContainsKey(dataMember))
                {
                    return false;
                }
                ConstantExpression expression2 = vex as ConstantExpression;
                if (expression2 != null)
                {
                    keys.Add(dataMember, expression2.Value);
                    return true;
                }
                InvocationExpression expression3 = vex as InvocationExpression;
                if (((expression3 != null) && (expression3.Arguments != null)) && (expression3.Arguments.Count == 0))
                {
                    ConstantExpression expression4 = expression3.Expression as ConstantExpression;
                    if (expression4 != null)
                    {
                        keys.Add(dataMember, ((Delegate) expression4.Value).DynamicInvoke(new object[0]));
                        return true;
                    }
                }
            }
            return false;
        }

        private bool GetKeysFromPredicate(MetaType type, Dictionary<MetaDataMember, object> keys, Expression expr)
        {
            BinaryExpression expression = expr as BinaryExpression;
            if (expression == null)
            {
                MethodCallExpression expression2 = expr as MethodCallExpression;
                if (((expression2 == null) || (expression2.Method.Name != "op_Equality")) || (expression2.Arguments.Count != 2))
                {
                    return false;
                }
                expression = Expression.Equal(expression2.Arguments[0], expression2.Arguments[1]);
            }
            ExpressionType nodeType = expression.NodeType;
            if (nodeType != ExpressionType.And)
            {
                if (nodeType != ExpressionType.Equal)
                {
                    return false;
                }
            }
            else
            {
                return (this.GetKeysFromPredicate(type, keys, expression.Left) && this.GetKeysFromPredicate(type, keys, expression.Right));
            }
            if (!GetKeyFromPredicate(type, keys, expression.Left, expression.Right))
            {
                return GetKeyFromPredicate(type, keys, expression.Right, expression.Left);
            }
            return true;
        }

        internal object[] GetKeyValues(MetaType type, LambdaExpression predicate)
        {
            if (predicate == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("predicate");
            }
            if (predicate.Parameters.Count != 1)
            {
                return null;
            }
            Dictionary<MetaDataMember, object> keys = new Dictionary<MetaDataMember, object>();
            if (!this.GetKeysFromPredicate(type, keys, predicate.Body) || (keys.Count != type.IdentityMembers.Count))
            {
                return null;
            }
            return (from kv in keys
                orderby kv.Key.Ordinal
                select kv.Value).ToArray<object>();
        }

        internal static object[] GetKeyValues(MetaType type, object instance)
        {
            List<object> list = new List<object>();
            foreach (MetaDataMember member in type.IdentityMembers)
            {
                list.Add(member.MemberAccessor.GetBoxedValue(instance));
            }
            return list.ToArray();
        }

        internal object GetObjectByKey(MetaType type, object[] keyValues)
        {
            object cachedObject = this.GetCachedObject(type, keyValues);
            if (cachedObject == null)
            {
                cachedObject = ((IEnumerable) this.context.Provider.Execute(this.GetObjectQuery(type, keyValues)).ReturnValue).OfType<object>().SingleOrDefault<object>();
            }
            return cachedObject;
        }

        internal Expression GetObjectQuery(MetaType type, Expression[] keyValues)
        {
            ITable table = this.context.GetTable(type.InheritanceRoot.Type);
            ParameterExpression expression = Expression.Parameter(table.ElementType, "p");
            Expression left = null;
            int index = 0;
            int count = type.IdentityMembers.Count;
            while (index < count)
            {
                MetaDataMember member = type.IdentityMembers[index];
                Expression expression3 = (member.Member is FieldInfo) ? Expression.Field(expression, (FieldInfo) member.Member) : Expression.Property(expression, (PropertyInfo) member.Member);
                Expression right = Expression.Equal(expression3, keyValues[index]);
                left = (left != null) ? Expression.And(left, right) : right;
                index++;
            }
            return Expression.Call(typeof(Queryable), "Where", new Type[] { table.ElementType }, new Expression[] { table.Expression, Expression.Lambda(left, new ParameterExpression[] { expression }) });
        }

        internal Expression GetObjectQuery(MetaType type, object[] keyValues)
        {
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            if (keyValues == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("keyValues");
            }
            return this.GetObjectQuery(type, BuildKeyExpressions(keyValues, type.IdentityMembers));
        }

        internal IEnumerable<RelatedItem> GetParents(MetaType type, object item) => 
            this.GetRelations(type, item, true);

        private IEnumerable<RelatedItem> GetRelations(MetaType type, object item, bool isForeignKey)
        {
            foreach (MetaDataMember iteratorVariable0 in type.PersistentDataMembers)
            {
                if (iteratorVariable0.IsAssociation)
                {
                    MetaType otherType = iteratorVariable0.Association.OtherType;
                    if (iteratorVariable0.Association.IsForeignKey == isForeignKey)
                    {
                        object boxedValue = null;
                        if (iteratorVariable0.IsDeferred)
                        {
                            boxedValue = iteratorVariable0.DeferredValueAccessor.GetBoxedValue(item);
                        }
                        else
                        {
                            boxedValue = iteratorVariable0.StorageAccessor.GetBoxedValue(item);
                        }
                        if (boxedValue != null)
                        {
                            if (iteratorVariable0.Association.IsMany)
                            {
                                IEnumerator enumerator = ((IEnumerable) boxedValue).GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    object current = enumerator.Current;
                                    yield return new RelatedItem(otherType.GetInheritanceType(current.GetType()), current);
                                }
                            }
                            else
                            {
                                yield return new RelatedItem(otherType.GetInheritanceType(boxedValue.GetType()), boxedValue);
                            }
                        }
                    }
                }
            }
        }

        public object InsertLookupCachedObject(MetaType type, object instance)
        {
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            this.hasCachedObjects = true;
            if (!type.IsEntity)
            {
                return instance;
            }
            return this.identifier.InsertLookup(type, instance);
        }

        public bool IsCachedObject(MetaType type, object instance)
        {
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            if (!type.IsEntity)
            {
                return false;
            }
            return (this.identifier.FindLike(type, instance) == instance);
        }

        public void OnEntityMaterialized(MetaType type, object instance)
        {
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            this.tracker.FastTrack(instance);
            if (type.HasAnyLoadMethod)
            {
                SendOnLoaded(type, instance);
            }
        }

        public bool RemoveCachedObjectLike(MetaType type, object instance)
        {
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            if (!type.IsEntity)
            {
                return false;
            }
            return this.identifier.RemoveLike(type, instance);
        }

        internal void ResetServices()
        {
            this.hasCachedObjects = false;
            bool asReadOnly = !this.context.ObjectTrackingEnabled;
            this.identifier = System.Data.Linq.IdentityManager.CreateIdentityManager(asReadOnly);
            this.tracker = System.Data.Linq.ChangeTracker.CreateChangeTracker(this, asReadOnly);
            this.factoryMap = new Dictionary<MetaDataMember, IDeferredSourceFactory>();
        }

        private static void SendOnLoaded(MetaType type, object item)
        {
            if (type != null)
            {
                SendOnLoaded(type.InheritanceBase, item);
                if (type.OnLoadedMethod != null)
                {
                    try
                    {
                        type.OnLoadedMethod.Invoke(item, new object[0]);
                    }
                    catch (TargetInvocationException exception)
                    {
                        if (exception.InnerException != null)
                        {
                            throw exception.InnerException;
                        }
                        throw;
                    }
                }
            }
        }

        internal void SetModel(MetaModel model)
        {
            this.metaModel = model;
        }

        internal System.Data.Linq.ChangeDirector ChangeDirector =>
            this.director;

        internal System.Data.Linq.ChangeTracker ChangeTracker =>
            this.tracker;

        public DataContext Context =>
            this.context;

        internal bool HasCachedObjects =>
            this.hasCachedObjects;

        internal System.Data.Linq.IdentityManager IdentityManager =>
            this.identifier;

        public MetaModel Model =>
            this.metaModel;


        private class DeferredSourceFactory<T> : IDeferredSourceFactory
        {
            private T[] empty;
            private MetaDataMember member;
            private ICompiledQuery query;
            private bool refersToPrimaryKey;
            private CommonDataServices services;

            internal DeferredSourceFactory(MetaDataMember member, CommonDataServices services)
            {
                this.member = member;
                this.services = services;
                this.refersToPrimaryKey = this.member.IsAssociation && this.member.Association.OtherKeyIsPrimaryKey;
                this.empty = new T[0];
            }

            public IEnumerable CreateDeferredSource(object instance)
            {
                if (instance == null)
                {
                    throw System.Data.Linq.Error.ArgumentNull("instance");
                }
                return new DeferredSource<T>((CommonDataServices.DeferredSourceFactory<T>) this, instance);
            }

            public IEnumerable CreateDeferredSource(object[] keyValues)
            {
                if (keyValues == null)
                {
                    throw System.Data.Linq.Error.ArgumentNull("keyValues");
                }
                return new DeferredSource<T>((CommonDataServices.DeferredSourceFactory<T>) this, keyValues);
            }

            private IEnumerator<T> Execute(object instance)
            {
                ReadOnlyCollection<MetaDataMember> thisKey = null;
                T local;
                if (this.member.IsAssociation)
                {
                    thisKey = this.member.Association.ThisKey;
                }
                else
                {
                    thisKey = this.member.DeclaringType.IdentityMembers;
                }
                object[] keyValues = new object[thisKey.Count];
                int index = 0;
                int count = thisKey.Count;
                while (index < count)
                {
                    keyValues[index] = thisKey[index].StorageAccessor.GetBoxedValue(instance);
                    index++;
                }
                if (this.HasNullForeignKey(keyValues))
                {
                    return ((IEnumerable<T>) this.empty).GetEnumerator();
                }
                if (this.TryGetCachedObject(keyValues, out local))
                {
                    return ((IEnumerable<T>) new T[] { local }).GetEnumerator();
                }
                if (this.member.LoadMethod != null)
                {
                    try
                    {
                        object obj3 = this.member.LoadMethod.Invoke(this.services.Context, new object[] { instance });
                        if (typeof(T).IsAssignableFrom(this.member.LoadMethod.ReturnType))
                        {
                            return ((IEnumerable<T>) new T[] { ((T) obj3) }).GetEnumerator();
                        }
                        return ((IEnumerable<T>) obj3).GetEnumerator();
                    }
                    catch (TargetInvocationException exception)
                    {
                        if (exception.InnerException != null)
                        {
                            throw exception.InnerException;
                        }
                        throw;
                    }
                }
                return this.ExecuteKeyQuery(keyValues);
            }

            private IEnumerator<T> ExecuteKeyQuery(object[] keyValues)
            {
                if (this.query == null)
                {
                    ParameterExpression expression;
                    Expression[] expressionArray = new Expression[keyValues.Length];
                    ReadOnlyCollection<MetaDataMember> onlys = this.member.IsAssociation ? this.member.Association.OtherKey : this.member.DeclaringType.IdentityMembers;
                    int index = 0;
                    int length = keyValues.Length;
                    while (index < length)
                    {
                        MetaDataMember member = onlys[index];
                        expressionArray[index] = Expression.Convert(Expression.ArrayIndex(expression = Expression.Parameter(typeof(object[]), "keys"), Expression.Constant(index)), member.Type);
                        index++;
                    }
                    LambdaExpression query = Expression.Lambda(this.services.GetDataMemberQuery(this.member, expressionArray), new ParameterExpression[] { expression });
                    this.query = this.services.Context.Provider.Compile(query);
                }
                return ((IEnumerable<T>) this.query.Execute(this.services.Context.Provider, new object[] { keyValues }).ReturnValue).GetEnumerator();
            }

            private IEnumerator<T> ExecuteKeys(object[] keyValues)
            {
                T local;
                if (this.HasNullForeignKey(keyValues))
                {
                    return ((IEnumerable<T>) this.empty).GetEnumerator();
                }
                if (this.TryGetCachedObject(keyValues, out local))
                {
                    return ((IEnumerable<T>) new T[] { local }).GetEnumerator();
                }
                return this.ExecuteKeyQuery(keyValues);
            }

            private bool HasNullForeignKey(object[] keyValues)
            {
                if (this.refersToPrimaryKey)
                {
                    bool flag = false;
                    int index = 0;
                    int length = keyValues.Length;
                    while (index < length)
                    {
                        flag |= keyValues[index] == null;
                        index++;
                    }
                    if (flag)
                    {
                        return true;
                    }
                }
                return false;
            }

            private bool TryGetCachedObject(object[] keyValues, out T cached)
            {
                cached = default(T);
                if (this.refersToPrimaryKey)
                {
                    MetaType type = this.member.IsAssociation ? this.member.Association.OtherType : this.member.DeclaringType;
                    object cachedObject = this.services.GetCachedObject(type, keyValues);
                    if (cachedObject != null)
                    {
                        cached = (T) cachedObject;
                        return true;
                    }
                }
                return false;
            }

            private class DeferredSource : IEnumerable<T>, IEnumerable
            {
                private CommonDataServices.DeferredSourceFactory<T> factory;
                private object instance;

                internal DeferredSource(CommonDataServices.DeferredSourceFactory<T> factory, object instance)
                {
                    this.factory = factory;
                    this.instance = instance;
                }

                public IEnumerator<T> GetEnumerator()
                {
                    object[] instance = this.instance as object[];
                    if (instance != null)
                    {
                        return this.factory.ExecuteKeys(instance);
                    }
                    return this.factory.Execute(this.instance);
                }

                IEnumerator IEnumerable.GetEnumerator() => 
                    this.GetEnumerator();
            }
        }
    }
}

