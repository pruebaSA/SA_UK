namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Internal.Materialization;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal abstract class InitializerMetadata : IEquatable<InitializerMetadata>
    {
        internal readonly Type ClrType;
        internal readonly string Identity;
        private static long s_identifier;
        private static readonly string s_identifierPrefix = typeof(InitializerMetadata).Name;

        private InitializerMetadata(Type clrType)
        {
            this.ClrType = clrType;
            this.Identity = s_identifierPrefix + Interlocked.Increment(ref s_identifier).ToString(CultureInfo.InvariantCulture);
        }

        internal virtual void AppendColumnMapKey(ColumnMapKeyBuilder builder)
        {
            builder.Append("CLR-", this.ClrType);
        }

        internal static InitializerMetadata CreateEmptyProjectionInitializer(EdmItemCollection itemCollection, NewExpression newExpression) => 
            itemCollection.GetCanonicalInitializerMetadata(new EmptyProjectionNewMetadata(newExpression));

        internal static InitializerMetadata CreateEntityCollectionInitializer(EdmItemCollection itemCollection, Type type, NavigationProperty navigationProperty) => 
            itemCollection.GetCanonicalInitializerMetadata(new EntityCollectionInitializerMetadata(type, navigationProperty));

        internal static InitializerMetadata CreateGroupingInitializer(EdmItemCollection itemCollection, Type resultType) => 
            itemCollection.GetCanonicalInitializerMetadata(new GroupingInitializerMetadata(resultType));

        internal static InitializerMetadata CreateProjectionInitializer(EdmItemCollection itemCollection, NewExpression newExpression) => 
            itemCollection.GetCanonicalInitializerMetadata(new ProjectionNewMetadata(newExpression));

        internal static InitializerMetadata CreateProjectionInitializer(EdmItemCollection itemCollection, MemberInitExpression initExpression, MemberInfo[] members) => 
            itemCollection.GetCanonicalInitializerMetadata(new ProjectionInitializerMetadata(initExpression, members));

        internal abstract Expression Emit(System.Data.Common.Internal.Materialization.Translator translator, List<TranslatorResult> propertyTranslatorResults);
        public bool Equals(InitializerMetadata other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }
            if (this.Kind != other.Kind)
            {
                return false;
            }
            if (!this.ClrType.Equals(other.ClrType))
            {
                return false;
            }
            return this.IsStructurallyEquivalent(other);
        }

        public override bool Equals(object obj) => 
            this.Equals(obj as InitializerMetadata);

        internal abstract IEnumerable<Type> GetChildTypes();
        public override int GetHashCode() => 
            this.ClrType.GetHashCode();

        protected static List<Expression> GetPropertyReaders(List<TranslatorResult> propertyTranslatorResults) => 
            (from s in propertyTranslatorResults select s.Expression).ToList<Expression>();

        protected virtual bool IsStructurallyEquivalent(InitializerMetadata other) => 
            true;

        internal static bool TryGetInitializerMetadata(TypeUsage typeUsage, out InitializerMetadata initializerMetadata)
        {
            initializerMetadata = null;
            if (BuiltInTypeKind.RowType == typeUsage.EdmType.BuiltInTypeKind)
            {
                initializerMetadata = ((RowType) typeUsage.EdmType).InitializerMetadata;
            }
            return (null != initializerMetadata);
        }

        internal abstract InitializerMetadataKind Kind { get; }

        private class EmptyProjectionNewMetadata : InitializerMetadata.ProjectionNewMetadata
        {
            internal EmptyProjectionNewMetadata(NewExpression newExpression) : base(newExpression)
            {
            }

            internal override Expression Emit(System.Data.Common.Internal.Materialization.Translator translator, List<TranslatorResult> propertyReaders) => 
                base.Emit(translator, new List<TranslatorResult>());

            internal override IEnumerable<Type> GetChildTypes()
            {
                yield return null;
            }

        }

        private class EntityCollectionInitializerMetadata : InitializerMetadata
        {
            private readonly NavigationProperty _navigationProperty;
            private static readonly MethodInfo s_createEntityCollectionMethod = typeof(InitializerMetadata.EntityCollectionInitializerMetadata).GetMethod("CreateEntityCollection", BindingFlags.Public | BindingFlags.Static);

            internal EntityCollectionInitializerMetadata(Type type, NavigationProperty navigationProperty) : base(type)
            {
                this._navigationProperty = navigationProperty;
            }

            internal override void AppendColumnMapKey(ColumnMapKeyBuilder builder)
            {
                base.AppendColumnMapKey(builder);
                builder.Append(",NP" + this._navigationProperty.Name);
                builder.Append(",AT", this._navigationProperty.DeclaringType);
            }

            public static EntityCollection<T> CreateEntityCollection<T>(Shaper state, IEntityWithRelationships owner, Coordinator<T> coordinator, string relationshipName, string targetRoleName) where T: class, IEntityWithRelationships
            {
                if (owner == null)
                {
                    return null;
                }
                EntityCollection<T> result = owner.RelationshipManager.GetRelatedCollection<T>(relationshipName, targetRoleName);
                coordinator.RegisterCloseHandler(delegate (Shaper readerState, List<T> elements) {
                    result.Load(elements, readerState.MergeOption);
                });
                return result;
            }

            internal override Expression Emit(System.Data.Common.Internal.Materialization.Translator translator, List<TranslatorResult> propertyTranslatorResults)
            {
                Type elementType = this.GetElementType();
                MethodInfo method = s_createEntityCollectionMethod.MakeGenericMethod(new Type[] { elementType });
                Expression expression = System.Data.Common.Internal.Materialization.Translator.Shaper_Parameter;
                Expression expression2 = propertyTranslatorResults[0].Expression;
                CollectionTranslatorResult result = propertyTranslatorResults[1] as CollectionTranslatorResult;
                Expression expressionToGetCoordinator = result.ExpressionToGetCoordinator;
                return Expression.Call(method, new Expression[] { expression, expression2, expressionToGetCoordinator, Expression.Constant(this._navigationProperty.RelationshipType.FullName), Expression.Constant(this._navigationProperty.ToEndMember.Name) });
            }

            internal override IEnumerable<Type> GetChildTypes()
            {
                Type elementType = this.GetElementType();
                yield return null;
                yield return typeof(IEnumerable<>).MakeGenericType(new Type[] { elementType });
            }

            private Type GetElementType()
            {
                if (!base.ClrType.IsGenericType || !typeof(EntityCollection<>).Equals(base.ClrType.GetGenericTypeDefinition()))
                {
                    throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.ELinq_UnexpectedTypeForNavigationProperty(this._navigationProperty, typeof(EntityCollection<>), base.ClrType));
                }
                return base.ClrType.GetGenericArguments()[0];
            }

            protected override bool IsStructurallyEquivalent(InitializerMetadata other)
            {
                InitializerMetadata.EntityCollectionInitializerMetadata metadata = (InitializerMetadata.EntityCollectionInitializerMetadata) other;
                return this._navigationProperty.Equals(metadata._navigationProperty);
            }

            internal override InitializerMetadataKind Kind =>
                InitializerMetadataKind.EntityCollection;

        }

        private class Grouping<K, T> : IGrouping<K, T>, IEnumerable<T>, IEnumerable
        {
            private readonly IEnumerable<T> _group;
            private readonly K _key;

            public Grouping(K key, IEnumerable<T> group)
            {
                this._key = key;
                this._group = group;
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                if (this._group != null)
                {
                    foreach (T iteratorVariable0 in this._group)
                    {
                        yield return iteratorVariable0;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => 
                ((IEnumerable<T>) this).GetEnumerator();

            public IEnumerable<T> Group =>
                this._group;

            public K Key =>
                this._key;

            [CompilerGenerated]
            private sealed class GetEnumerator>d__2 : IEnumerator<T>, IEnumerator, IDisposable
            {
                private int <>1__state;
                private T <>2__current;
                public InitializerMetadata.Grouping<K, T> <>4__this;
                public IEnumerator<T> <>7__wrap4;
                public T <member>5__3;

                [DebuggerHidden]
                public GetEnumerator>d__2(int <>1__state)
                {
                    this.<>1__state = <>1__state;
                }

                private void <>m__Finally5()
                {
                    this.<>1__state = -1;
                    if (this.<>7__wrap4 != null)
                    {
                        this.<>7__wrap4.Dispose();
                    }
                }

                private bool MoveNext()
                {
                    bool flag;
                    try
                    {
                        switch (this.<>1__state)
                        {
                            case 0:
                                this.<>1__state = -1;
                                if (this.<>4__this._group == null)
                                {
                                    goto Label_0090;
                                }
                                this.<>7__wrap4 = this.<>4__this._group.GetEnumerator();
                                this.<>1__state = 1;
                                goto Label_007D;

                            case 2:
                                this.<>1__state = 1;
                                goto Label_007D;

                            default:
                                goto Label_0090;
                        }
                    Label_004E:
                        this.<member>5__3 = this.<>7__wrap4.Current;
                        this.<>2__current = this.<member>5__3;
                        this.<>1__state = 2;
                        return true;
                    Label_007D:
                        if (this.<>7__wrap4.MoveNext())
                        {
                            goto Label_004E;
                        }
                        this.<>m__Finally5();
                    Label_0090:
                        flag = false;
                    }
                    fault
                    {
                        this.System.IDisposable.Dispose();
                    }
                    return flag;
                }

                [DebuggerHidden]
                void IEnumerator.Reset()
                {
                    throw new NotSupportedException();
                }

                void IDisposable.Dispose()
                {
                    switch (this.<>1__state)
                    {
                        case 1:
                        case 2:
                            try
                            {
                            }
                            finally
                            {
                                this.<>m__Finally5();
                            }
                            return;
                    }
                }

                T IEnumerator<T>.Current =>
                    this.<>2__current;

                object IEnumerator.Current =>
                    this.<>2__current;
            }
        }

        private class GroupingInitializerMetadata : InitializerMetadata
        {
            internal GroupingInitializerMetadata(Type type) : base(type)
            {
            }

            internal override Expression Emit(System.Data.Common.Internal.Materialization.Translator translator, List<TranslatorResult> propertyTranslatorResults)
            {
                Type type = base.ClrType.GetGenericArguments()[0];
                Type type2 = base.ClrType.GetGenericArguments()[1];
                return Expression.Convert(Expression.New(typeof(InitializerMetadata.Grouping<, >).MakeGenericType(new Type[] { type, type2 }).GetConstructors().Single<ConstructorInfo>(), InitializerMetadata.GetPropertyReaders(propertyTranslatorResults)), base.ClrType);
            }

            internal override IEnumerable<Type> GetChildTypes()
            {
                Type iteratorVariable0 = this.ClrType.GetGenericArguments()[0];
                Type iteratorVariable1 = this.ClrType.GetGenericArguments()[1];
                yield return iteratorVariable0;
                yield return typeof(IEnumerable<>).MakeGenericType(new Type[] { iteratorVariable1 });
            }

            internal override InitializerMetadataKind Kind =>
                InitializerMetadataKind.Grouping;

        }

        private class ProjectionInitializerMetadata : InitializerMetadata
        {
            private readonly MemberInitExpression _initExpression;
            private readonly MemberInfo[] _members;

            internal ProjectionInitializerMetadata(MemberInitExpression initExpression, MemberInfo[] members) : base(initExpression.Type)
            {
                this._initExpression = initExpression;
                this._members = members;
            }

            internal override void AppendColumnMapKey(ColumnMapKeyBuilder builder)
            {
                base.AppendColumnMapKey(builder);
                foreach (MemberBinding binding in this._initExpression.Bindings)
                {
                    builder.Append(",", binding.Member.DeclaringType);
                    builder.Append("." + binding.Member.Name);
                }
            }

            internal override Expression Emit(System.Data.Common.Internal.Materialization.Translator translator, List<TranslatorResult> propertyReaders)
            {
                MemberBinding[] bindings = new MemberBinding[this._initExpression.Bindings.Count];
                MemberBinding[] bindingArray2 = new MemberBinding[bindings.Length];
                for (int i = 0; i < bindings.Length; i++)
                {
                    MemberBinding binding = this._initExpression.Bindings[i];
                    Expression expression = propertyReaders[i].Expression;
                    MemberBinding binding2 = Expression.Bind(binding.Member, expression);
                    MemberBinding binding3 = Expression.Bind(binding.Member, Expression.Constant(TypeSystem.GetDefaultValue(expression.Type), expression.Type));
                    bindings[i] = binding2;
                    bindingArray2[i] = binding3;
                }
                Expression expression2 = Expression.MemberInit(this._initExpression.NewExpression, bindings);
                Expression expression3 = Expression.MemberInit(this._initExpression.NewExpression, bindingArray2);
                translator.RegisterUserExpression(expression3);
                return expression2;
            }

            internal override IEnumerable<Type> GetChildTypes()
            {
                foreach (MemberBinding iteratorVariable0 in this._initExpression.Bindings)
                {
                    string iteratorVariable2;
                    Type iteratorVariable1;
                    TypeSystem.PropertyOrField(iteratorVariable0.Member, out iteratorVariable2, out iteratorVariable1);
                    yield return iteratorVariable1;
                }
            }

            protected override bool IsStructurallyEquivalent(InitializerMetadata other)
            {
                InitializerMetadata.ProjectionInitializerMetadata metadata = (InitializerMetadata.ProjectionInitializerMetadata) other;
                if (this._initExpression.Bindings.Count != metadata._initExpression.Bindings.Count)
                {
                    return false;
                }
                for (int i = 0; i < this._initExpression.Bindings.Count; i++)
                {
                    MemberBinding binding = this._initExpression.Bindings[i];
                    MemberBinding binding2 = metadata._initExpression.Bindings[i];
                    if (!binding.Member.Equals(binding2.Member))
                    {
                        return false;
                    }
                }
                return true;
            }

            internal override InitializerMetadataKind Kind =>
                InitializerMetadataKind.ProjectionInitializer;

        }

        private class ProjectionNewMetadata : InitializerMetadata
        {
            private readonly NewExpression _newExpression;

            internal ProjectionNewMetadata(NewExpression newExpression) : base(newExpression.Type)
            {
                this._newExpression = newExpression;
            }

            internal override void AppendColumnMapKey(ColumnMapKeyBuilder builder)
            {
                base.AppendColumnMapKey(builder);
                builder.Append(this._newExpression.Constructor.ToString());
                foreach (MemberInfo info in this._newExpression.Members ?? Enumerable.Empty<MemberInfo>())
                {
                    builder.Append("DT", info.DeclaringType);
                    builder.Append("." + info.Name);
                }
            }

            internal override Expression Emit(System.Data.Common.Internal.Materialization.Translator translator, List<TranslatorResult> propertyTranslatorResults)
            {
                Expression.Constant(null, base.ClrType);
                Expression expression = Expression.New(this._newExpression.Constructor, InitializerMetadata.GetPropertyReaders(propertyTranslatorResults));
                Expression expression2 = Expression.New(this._newExpression.Constructor, (IEnumerable<Expression>) (from child in propertyTranslatorResults select Expression.Constant(TypeSystem.GetDefaultValue(child.Expression.Type), child.Expression.Type)));
                translator.RegisterUserExpression(expression2);
                return expression;
            }

            internal override IEnumerable<Type> GetChildTypes() => 
                (from arg in this._newExpression.Arguments select arg.Type);

            protected override bool IsStructurallyEquivalent(InitializerMetadata other)
            {
                InitializerMetadata.ProjectionNewMetadata metadata = (InitializerMetadata.ProjectionNewMetadata) other;
                if (this._newExpression.Members.Count != metadata._newExpression.Members.Count)
                {
                    return false;
                }
                for (int i = 0; i < this._newExpression.Members.Count; i++)
                {
                    MemberInfo info = this._newExpression.Members[i];
                    MemberInfo info2 = metadata._newExpression.Members[i];
                    if (!info.Equals(info2))
                    {
                        return false;
                    }
                }
                return true;
            }

            internal override InitializerMetadataKind Kind =>
                InitializerMetadataKind.ProjectionNew;
        }
    }
}

