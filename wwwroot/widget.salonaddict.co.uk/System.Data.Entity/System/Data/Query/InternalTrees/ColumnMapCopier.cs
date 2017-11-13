namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.PlanCompiler;

    internal class ColumnMapCopier : ColumnMapVisitorWithResults<ColumnMap, VarMap>
    {
        private static ColumnMapCopier Instance = new ColumnMapCopier();

        private ColumnMapCopier()
        {
        }

        internal static ColumnMap Copy(ColumnMap columnMap, VarMap replacementVarMap) => 
            columnMap.Accept<ColumnMap, VarMap>(Instance, replacementVarMap);

        private static Var GetReplacementVar(Var originalVar, VarMap replacementVarMap)
        {
            Var key = originalVar;
            while (replacementVarMap.TryGetValue(key, out originalVar))
            {
                if (originalVar == key)
                {
                    return key;
                }
                key = originalVar;
            }
            return key;
        }

        internal override ColumnMap Visit(ComplexTypeColumnMap columnMap, VarMap replacementVarMap)
        {
            SimpleColumnMap nullSentinel = columnMap.NullSentinel;
            if (nullSentinel != null)
            {
                nullSentinel = (SimpleColumnMap) nullSentinel.Accept<ColumnMap, VarMap>(this, replacementVarMap);
            }
            return new ComplexTypeColumnMap(columnMap.Type, columnMap.Name, this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap), nullSentinel);
        }

        internal override ColumnMap Visit(DiscriminatedCollectionColumnMap columnMap, VarMap replacementVarMap)
        {
            ColumnMap elementMap = columnMap.Element.Accept<ColumnMap, VarMap>(this, replacementVarMap);
            SimpleColumnMap discriminator = (SimpleColumnMap) columnMap.Discriminator.Accept<ColumnMap, VarMap>(this, replacementVarMap);
            SimpleColumnMap[] keys = this.VisitList<SimpleColumnMap>(columnMap.Keys, replacementVarMap);
            SimpleColumnMap[] foreignKeys = this.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, replacementVarMap);
            return new DiscriminatedCollectionColumnMap(columnMap.Type, columnMap.Name, elementMap, keys, foreignKeys, this.VisitSortKey(columnMap.SortKeys, replacementVarMap), discriminator, columnMap.DiscriminatorValue);
        }

        internal override ColumnMap Visit(EntityColumnMap columnMap, VarMap replacementVarMap)
        {
            EntityIdentity entityIdentity = base.VisitEntityIdentity(columnMap.EntityIdentity, replacementVarMap);
            return new EntityColumnMap(columnMap.Type, columnMap.Name, this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap), entityIdentity);
        }

        internal override ColumnMap Visit(MultipleDiscriminatorPolymorphicColumnMap columnMap, VarMap replacementVarMap)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "unexpected MultipleDiscriminatorPolymorphicColumnMap in ColumnMapCopier");
            return null;
        }

        internal override ColumnMap Visit(RecordColumnMap columnMap, VarMap replacementVarMap)
        {
            SimpleColumnMap nullSentinel = columnMap.NullSentinel;
            if (nullSentinel != null)
            {
                nullSentinel = (SimpleColumnMap) nullSentinel.Accept<ColumnMap, VarMap>(this, replacementVarMap);
            }
            return new RecordColumnMap(columnMap.Type, columnMap.Name, this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap), nullSentinel);
        }

        internal override ColumnMap Visit(RefColumnMap columnMap, VarMap replacementVarMap) => 
            new RefColumnMap(columnMap.Type, columnMap.Name, base.VisitEntityIdentity(columnMap.EntityIdentity, replacementVarMap));

        internal override ColumnMap Visit(ScalarColumnMap columnMap, VarMap replacementVarMap) => 
            new ScalarColumnMap(columnMap.Type, columnMap.Name, columnMap.CommandId, columnMap.ColumnPos);

        internal override ColumnMap Visit(SimpleCollectionColumnMap columnMap, VarMap replacementVarMap)
        {
            ColumnMap elementMap = columnMap.Element.Accept<ColumnMap, VarMap>(this, replacementVarMap);
            SimpleColumnMap[] keys = this.VisitList<SimpleColumnMap>(columnMap.Keys, replacementVarMap);
            SimpleColumnMap[] foreignKeys = this.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, replacementVarMap);
            return new SimpleCollectionColumnMap(columnMap.Type, columnMap.Name, elementMap, keys, foreignKeys, this.VisitSortKey(columnMap.SortKeys, replacementVarMap));
        }

        internal override ColumnMap Visit(SimplePolymorphicColumnMap columnMap, VarMap replacementVarMap)
        {
            SimpleColumnMap typeDiscriminator = (SimpleColumnMap) columnMap.TypeDiscriminator.Accept<ColumnMap, VarMap>(this, replacementVarMap);
            Dictionary<object, TypedColumnMap> typeChoices = new Dictionary<object, TypedColumnMap>(columnMap.TypeChoices.Comparer);
            foreach (KeyValuePair<object, TypedColumnMap> pair in columnMap.TypeChoices)
            {
                TypedColumnMap map2 = (TypedColumnMap) pair.Value.Accept<ColumnMap, VarMap>(this, replacementVarMap);
                typeChoices[pair.Key] = map2;
            }
            return new SimplePolymorphicColumnMap(columnMap.Type, columnMap.Name, this.VisitList<ColumnMap>(columnMap.Properties, replacementVarMap), typeDiscriminator, typeChoices);
        }

        internal override ColumnMap Visit(VarRefColumnMap columnMap, VarMap replacementVarMap) => 
            new VarRefColumnMap(columnMap.Type, columnMap.Name, GetReplacementVar(columnMap.Var, replacementVarMap));

        protected override EntityIdentity VisitEntityIdentity(DiscriminatedEntityIdentity entityIdentity, VarMap replacementVarMap)
        {
            SimpleColumnMap entitySetColumn = (SimpleColumnMap) entityIdentity.EntitySetColumnMap.Accept<ColumnMap, VarMap>(this, replacementVarMap);
            return new DiscriminatedEntityIdentity(entitySetColumn, entityIdentity.EntitySetMap, this.VisitList<SimpleColumnMap>(entityIdentity.Keys, replacementVarMap));
        }

        protected override EntityIdentity VisitEntityIdentity(SimpleEntityIdentity entityIdentity, VarMap replacementVarMap) => 
            new SimpleEntityIdentity(entityIdentity.EntitySet, this.VisitList<SimpleColumnMap>(entityIdentity.Keys, replacementVarMap));

        internal TListType[] VisitList<TListType>(TListType[] tList, VarMap replacementVarMap) where TListType: ColumnMap
        {
            TListType[] localArray = new TListType[tList.Length];
            for (int i = 0; i < tList.Length; i++)
            {
                localArray[i] = tList[i].Accept<ColumnMap, VarMap>(this, replacementVarMap);
            }
            return localArray;
        }

        private SortKeyInfo VisitSortKey(SortKeyInfo sortKeyInfo, VarMap replacementVarMap) => 
            new SortKeyInfo((SimpleColumnMap) sortKeyInfo.SortKeyColumn.Accept<ColumnMap, VarMap>(this, replacementVarMap), sortKeyInfo.AscendingSort, sortKeyInfo.Collation);

        private SortKeyInfo[] VisitSortKey(SortKeyInfo[] sortKeyInfoList, VarMap replacementVarMap)
        {
            SortKeyInfo[] infoArray = new SortKeyInfo[sortKeyInfoList.Length];
            for (int i = 0; i < sortKeyInfoList.Length; i++)
            {
                infoArray[i] = this.VisitSortKey(sortKeyInfoList[i], replacementVarMap);
            }
            return infoArray;
        }
    }
}

