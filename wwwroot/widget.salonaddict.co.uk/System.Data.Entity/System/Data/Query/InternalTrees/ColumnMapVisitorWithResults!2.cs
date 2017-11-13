namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class ColumnMapVisitorWithResults<TResultType, TArgType>
    {
        protected ColumnMapVisitorWithResults()
        {
        }

        internal abstract TResultType Visit(ComplexTypeColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(DiscriminatedCollectionColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(EntityColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(MultipleDiscriminatorPolymorphicColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(RecordColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(RefColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(ScalarColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(SimpleCollectionColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(SimplePolymorphicColumnMap columnMap, TArgType arg);
        internal abstract TResultType Visit(VarRefColumnMap columnMap, TArgType arg);
        protected virtual EntityIdentity VisitEntityIdentity(DiscriminatedEntityIdentity entityIdentity, TArgType arg) => 
            entityIdentity;

        protected EntityIdentity VisitEntityIdentity(EntityIdentity entityIdentity, TArgType arg)
        {
            DiscriminatedEntityIdentity identity = entityIdentity as DiscriminatedEntityIdentity;
            if (identity != null)
            {
                return this.VisitEntityIdentity(identity, arg);
            }
            return this.VisitEntityIdentity((SimpleEntityIdentity) entityIdentity, arg);
        }

        protected virtual EntityIdentity VisitEntityIdentity(SimpleEntityIdentity entityIdentity, TArgType arg) => 
            entityIdentity;
    }
}

