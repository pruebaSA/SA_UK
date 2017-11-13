namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class ColumnMapVisitor<TArgType>
    {
        protected ColumnMapVisitor()
        {
        }

        internal virtual void Visit(ComplexTypeColumnMap columnMap, TArgType arg)
        {
            ColumnMap nullSentinel = columnMap.NullSentinel;
            if (nullSentinel != null)
            {
                nullSentinel.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            foreach (ColumnMap map2 in columnMap.Properties)
            {
                map2.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }

        internal virtual void Visit(DiscriminatedCollectionColumnMap columnMap, TArgType arg)
        {
            columnMap.Discriminator.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            foreach (SimpleColumnMap map in columnMap.ForeignKeys)
            {
                map.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            foreach (SimpleColumnMap map2 in columnMap.Keys)
            {
                map2.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            columnMap.Element.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
        }

        internal virtual void Visit(EntityColumnMap columnMap, TArgType arg)
        {
            this.VisitEntityIdentity(columnMap.EntityIdentity, arg);
            foreach (ColumnMap map in columnMap.Properties)
            {
                map.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }

        internal virtual void Visit(MultipleDiscriminatorPolymorphicColumnMap columnMap, TArgType arg)
        {
            foreach (SimpleColumnMap map in columnMap.TypeDiscriminators)
            {
                map.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            foreach (TypedColumnMap map2 in columnMap.TypeChoices.Values)
            {
                map2.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            foreach (ColumnMap map3 in columnMap.Properties)
            {
                map3.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }

        internal virtual void Visit(RecordColumnMap columnMap, TArgType arg)
        {
            ColumnMap nullSentinel = columnMap.NullSentinel;
            if (nullSentinel != null)
            {
                nullSentinel.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            foreach (ColumnMap map2 in columnMap.Properties)
            {
                map2.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }

        internal virtual void Visit(RefColumnMap columnMap, TArgType arg)
        {
            this.VisitEntityIdentity(columnMap.EntityIdentity, arg);
        }

        internal virtual void Visit(ScalarColumnMap columnMap, TArgType arg)
        {
        }

        internal virtual void Visit(SimpleCollectionColumnMap columnMap, TArgType arg)
        {
            foreach (SimpleColumnMap map in columnMap.ForeignKeys)
            {
                map.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            foreach (SimpleColumnMap map2 in columnMap.Keys)
            {
                map2.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            columnMap.Element.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
        }

        internal virtual void Visit(SimplePolymorphicColumnMap columnMap, TArgType arg)
        {
            columnMap.TypeDiscriminator.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            foreach (ColumnMap map in columnMap.TypeChoices.Values)
            {
                map.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
            foreach (ColumnMap map2 in columnMap.Properties)
            {
                map2.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }

        internal virtual void Visit(VarRefColumnMap columnMap, TArgType arg)
        {
        }

        protected virtual void VisitEntityIdentity(DiscriminatedEntityIdentity entityIdentity, TArgType arg)
        {
            entityIdentity.EntitySetColumnMap.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            foreach (SimpleColumnMap map in entityIdentity.Keys)
            {
                map.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }

        protected void VisitEntityIdentity(EntityIdentity entityIdentity, TArgType arg)
        {
            DiscriminatedEntityIdentity identity = entityIdentity as DiscriminatedEntityIdentity;
            if (identity != null)
            {
                this.VisitEntityIdentity(identity, arg);
            }
            else
            {
                this.VisitEntityIdentity((SimpleEntityIdentity) entityIdentity, arg);
            }
        }

        protected virtual void VisitEntityIdentity(SimpleEntityIdentity entityIdentity, TArgType arg)
        {
            foreach (SimpleColumnMap map in entityIdentity.Keys)
            {
                map.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }

        protected void VisitList<TListType>(TListType[] columnMaps, TArgType arg) where TListType: ColumnMap
        {
            foreach (TListType local in columnMaps)
            {
                local.Accept<TArgType>((ColumnMapVisitor<TArgType>) this, arg);
            }
        }
    }
}

