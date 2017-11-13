namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Query.InternalTrees;

    internal class ColumnMapTranslator : ColumnMapVisitorWithResults<ColumnMap, ColumnMapTranslatorTranslationDelegate>
    {
        private static ColumnMapTranslator Instance = new ColumnMapTranslator();

        private ColumnMapTranslator()
        {
        }

        private static Var GetReplacementVar(Var originalVar, Dictionary<Var, Var> replacementVarMap)
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

        internal static ColumnMap Translate(ColumnMap columnMapToTranslate, Dictionary<Var, KeyValuePair<int, int>> varToCommandColumnMap) => 
            Translate(columnMapToTranslate, delegate (ColumnMap columnMap) {
                VarRefColumnMap map = columnMap as VarRefColumnMap;
                if (map != null)
                {
                    KeyValuePair<int, int> pair;
                    if (!varToCommandColumnMap.TryGetValue(map.Var, out pair))
                    {
                        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UnknownVar, 1, map.Var.Id);
                    }
                    columnMap = new ScalarColumnMap(map.Type, map.Name, pair.Key, pair.Value);
                }
                if (!columnMap.IsNamed)
                {
                    columnMap.Name = "Value";
                }
                return columnMap;
            });

        internal static ColumnMap Translate(ColumnMap columnMapToTranslate, Dictionary<Var, ColumnMap> varToColumnMap) => 
            Translate(columnMapToTranslate, delegate (ColumnMap columnMap) {
                VarRefColumnMap map = columnMap as VarRefColumnMap;
                if (map != null)
                {
                    if (varToColumnMap.TryGetValue(map.Var, out columnMap))
                    {
                        if (!columnMap.IsNamed && map.IsNamed)
                        {
                            columnMap.Name = map.Name;
                        }
                        return columnMap;
                    }
                    columnMap = map;
                }
                return columnMap;
            });

        internal static ColumnMap Translate(ColumnMap columnMapToTranslate, Dictionary<Var, Var> varToVarMap) => 
            Translate(columnMapToTranslate, delegate (ColumnMap columnMap) {
                VarRefColumnMap map = columnMap as VarRefColumnMap;
                if (map != null)
                {
                    Var v = GetReplacementVar(map.Var, varToVarMap);
                    if (map.Var != v)
                    {
                        columnMap = new VarRefColumnMap(map.Type, map.Name, v);
                    }
                }
                return columnMap;
            });

        internal static ColumnMap Translate(ColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate) => 
            columnMap.Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(Instance, translationDelegate);

        internal override ColumnMap Visit(ComplexTypeColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            SimpleColumnMap nullSentinel = columnMap.NullSentinel;
            if (nullSentinel != null)
            {
                nullSentinel = (SimpleColumnMap) translationDelegate(nullSentinel);
            }
            this.VisitList<ColumnMap>(columnMap.Properties, translationDelegate);
            if (columnMap.NullSentinel != nullSentinel)
            {
                columnMap = new ComplexTypeColumnMap(columnMap.Type, columnMap.Name, columnMap.Properties, nullSentinel);
            }
            return translationDelegate(columnMap);
        }

        internal override ColumnMap Visit(DiscriminatedCollectionColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            ColumnMap map = columnMap.Discriminator.Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(this, translationDelegate);
            this.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, translationDelegate);
            this.VisitList<SimpleColumnMap>(columnMap.Keys, translationDelegate);
            ColumnMap elementMap = columnMap.Element.Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(this, translationDelegate);
            if ((map != columnMap.Discriminator) || (elementMap != columnMap.Element))
            {
                columnMap = new DiscriminatedCollectionColumnMap(columnMap.Type, columnMap.Name, elementMap, columnMap.Keys, columnMap.ForeignKeys, columnMap.SortKeys, (SimpleColumnMap) map, columnMap.DiscriminatorValue);
            }
            return translationDelegate(columnMap);
        }

        internal override ColumnMap Visit(EntityColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            EntityIdentity entityIdentity = base.VisitEntityIdentity(columnMap.EntityIdentity, translationDelegate);
            this.VisitList<ColumnMap>(columnMap.Properties, translationDelegate);
            if (entityIdentity != columnMap.EntityIdentity)
            {
                columnMap = new EntityColumnMap(columnMap.Type, columnMap.Name, columnMap.Properties, entityIdentity);
            }
            return translationDelegate(columnMap);
        }

        internal override ColumnMap Visit(MultipleDiscriminatorPolymorphicColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "unexpected MultipleDiscriminatorPolymorphicColumnMap in ColumnMapTranslator");
            return null;
        }

        internal override ColumnMap Visit(RecordColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            SimpleColumnMap nullSentinel = columnMap.NullSentinel;
            if (nullSentinel != null)
            {
                nullSentinel = (SimpleColumnMap) translationDelegate(nullSentinel);
            }
            this.VisitList<ColumnMap>(columnMap.Properties, translationDelegate);
            if (columnMap.NullSentinel != nullSentinel)
            {
                columnMap = new RecordColumnMap(columnMap.Type, columnMap.Name, columnMap.Properties, nullSentinel);
            }
            return translationDelegate(columnMap);
        }

        internal override ColumnMap Visit(RefColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            EntityIdentity entityIdentity = base.VisitEntityIdentity(columnMap.EntityIdentity, translationDelegate);
            if (entityIdentity != columnMap.EntityIdentity)
            {
                columnMap = new RefColumnMap(columnMap.Type, columnMap.Name, entityIdentity);
            }
            return translationDelegate(columnMap);
        }

        internal override ColumnMap Visit(ScalarColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate) => 
            translationDelegate(columnMap);

        internal override ColumnMap Visit(SimpleCollectionColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            this.VisitList<SimpleColumnMap>(columnMap.ForeignKeys, translationDelegate);
            this.VisitList<SimpleColumnMap>(columnMap.Keys, translationDelegate);
            ColumnMap elementMap = columnMap.Element.Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(this, translationDelegate);
            if (elementMap != columnMap.Element)
            {
                columnMap = new SimpleCollectionColumnMap(columnMap.Type, columnMap.Name, elementMap, columnMap.Keys, columnMap.ForeignKeys, columnMap.SortKeys);
            }
            return translationDelegate(columnMap);
        }

        internal override ColumnMap Visit(SimplePolymorphicColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            ColumnMap map = columnMap.TypeDiscriminator.Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(this, translationDelegate);
            Dictionary<object, TypedColumnMap> typeChoices = columnMap.TypeChoices;
            foreach (KeyValuePair<object, TypedColumnMap> pair in columnMap.TypeChoices)
            {
                TypedColumnMap map2 = (TypedColumnMap) pair.Value.Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(this, translationDelegate);
                if (map2 != pair.Value)
                {
                    if (typeChoices == columnMap.TypeChoices)
                    {
                        typeChoices = new Dictionary<object, TypedColumnMap>(columnMap.TypeChoices);
                    }
                    typeChoices[pair.Key] = map2;
                }
            }
            this.VisitList<ColumnMap>(columnMap.Properties, translationDelegate);
            if ((map != columnMap.TypeDiscriminator) || (typeChoices != columnMap.TypeChoices))
            {
                columnMap = new SimplePolymorphicColumnMap(columnMap.Type, columnMap.Name, columnMap.Properties, (SimpleColumnMap) map, typeChoices);
            }
            return translationDelegate(columnMap);
        }

        internal override ColumnMap Visit(VarRefColumnMap columnMap, ColumnMapTranslatorTranslationDelegate translationDelegate) => 
            translationDelegate(columnMap);

        protected override EntityIdentity VisitEntityIdentity(DiscriminatedEntityIdentity entityIdentity, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            ColumnMap map = entityIdentity.EntitySetColumnMap.Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(this, translationDelegate);
            this.VisitList<SimpleColumnMap>(entityIdentity.Keys, translationDelegate);
            if (map != entityIdentity.EntitySetColumnMap)
            {
                entityIdentity = new DiscriminatedEntityIdentity((SimpleColumnMap) map, entityIdentity.EntitySetMap, entityIdentity.Keys);
            }
            return entityIdentity;
        }

        protected override EntityIdentity VisitEntityIdentity(SimpleEntityIdentity entityIdentity, ColumnMapTranslatorTranslationDelegate translationDelegate)
        {
            this.VisitList<SimpleColumnMap>(entityIdentity.Keys, translationDelegate);
            return entityIdentity;
        }

        private void VisitList<TResultType>(TResultType[] tList, ColumnMapTranslatorTranslationDelegate translationDelegate) where TResultType: ColumnMap
        {
            for (int i = 0; i < tList.Length; i++)
            {
                tList[i] = tList[i].Accept<ColumnMap, ColumnMapTranslatorTranslationDelegate>(this, translationDelegate);
            }
        }
    }
}

