namespace System.Data.Mapping
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Runtime.InteropServices;

    internal sealed class FunctionImportMapping
    {
        internal readonly System.Collections.ObjectModel.ReadOnlyCollection<string> DiscriminatorColumns;
        internal readonly KeyToListMap<EntityType, IXmlLineInfo> EntityTypeLineInfos;
        internal readonly EdmFunction FunctionImport;
        internal readonly KeyToListMap<EntityType, IXmlLineInfo> IsTypeOfLineInfos;
        internal readonly System.Collections.ObjectModel.ReadOnlyCollection<EntityType> MappedEntityTypes;
        internal readonly System.Collections.ObjectModel.ReadOnlyCollection<FunctionImportNormalizedEntityTypeMapping> NormalizedEntityTypeMappings;
        internal readonly EdmFunction TargetFunction;

        internal FunctionImportMapping(EdmFunction targetFunction, EdmFunction functionImport, IEnumerable<FunctionImportEntityTypeMapping> entityTypeMappings, ItemCollection itemCollection)
        {
            Func<FunctionImportEntityTypeMapping, IEnumerable<EntityType>> selector = null;
            this.TargetFunction = EntityUtil.CheckArgumentNull<EdmFunction>(targetFunction, "targetFunction");
            this.FunctionImport = EntityUtil.CheckArgumentNull<EdmFunction>(functionImport, "functionImport");
            EntityUtil.CheckArgumentNull<ItemCollection>(itemCollection, "itemCollection");
            if (selector == null)
            {
                selector = mapping => mapping.GetMappedEntityTypes(itemCollection);
            }
            this.MappedEntityTypes = entityTypeMappings.SelectMany<FunctionImportEntityTypeMapping, EntityType>(selector).Distinct<EntityType>().ToList<EntityType>().AsReadOnly();
            this.DiscriminatorColumns = (from mapping in entityTypeMappings select mapping.GetDiscriminatorColumns()).Distinct<string>().ToList<string>().AsReadOnly();
            this.EntityTypeLineInfos = new KeyToListMap<EntityType, IXmlLineInfo>(EqualityComparer<EntityType>.Default);
            this.IsTypeOfLineInfos = new KeyToListMap<EntityType, IXmlLineInfo>(EqualityComparer<EntityType>.Default);
            List<FunctionImportNormalizedEntityTypeMapping> list = new List<FunctionImportNormalizedEntityTypeMapping>();
            foreach (FunctionImportEntityTypeMapping mapping in entityTypeMappings)
            {
                foreach (EntityType type in mapping.EntityTypes)
                {
                    this.EntityTypeLineInfos.Add(type, mapping.LineInfo);
                }
                foreach (EntityType type2 in mapping.IsOfTypeEntityTypes)
                {
                    this.IsTypeOfLineInfos.Add(type2, mapping.LineInfo);
                }
                Dictionary<string, FunctionImportEntityTypeMappingCondition> dictionary = mapping.Conditions.ToDictionary<FunctionImportEntityTypeMappingCondition, string, FunctionImportEntityTypeMappingCondition>(condition => condition.ColumnName, condition => condition);
                List<FunctionImportEntityTypeMappingCondition> columnConditions = new List<FunctionImportEntityTypeMappingCondition>(this.DiscriminatorColumns.Count);
                for (int i = 0; i < this.DiscriminatorColumns.Count; i++)
                {
                    FunctionImportEntityTypeMappingCondition condition;
                    string key = this.DiscriminatorColumns[i];
                    if (dictionary.TryGetValue(key, out condition))
                    {
                        columnConditions.Add(condition);
                    }
                    else
                    {
                        columnConditions.Add(null);
                    }
                }
                bool[] values = new bool[this.MappedEntityTypes.Count];
                System.Data.Common.Utils.Set<EntityType> set = new System.Data.Common.Utils.Set<EntityType>(mapping.GetMappedEntityTypes(itemCollection));
                for (int j = 0; j < this.MappedEntityTypes.Count; j++)
                {
                    values[j] = set.Contains(this.MappedEntityTypes[j]);
                }
                list.Add(new FunctionImportNormalizedEntityTypeMapping(this, columnConditions, new BitArray(values)));
            }
            this.NormalizedEntityTypeMappings = new System.Collections.ObjectModel.ReadOnlyCollection<FunctionImportNormalizedEntityTypeMapping>(list);
        }

        private void CollectUnreachableTypes(EdmItemCollection itemCollection, System.Data.Common.Utils.Set<EntityType> reachableTypes, out KeyToListMap<EntityType, IXmlLineInfo> entityTypes, out KeyToListMap<EntityType, IXmlLineInfo> isTypeOfEntityTypes)
        {
            entityTypes = new KeyToListMap<EntityType, IXmlLineInfo>(EqualityComparer<EntityType>.Default);
            isTypeOfEntityTypes = new KeyToListMap<EntityType, IXmlLineInfo>(EqualityComparer<EntityType>.Default);
            if (reachableTypes.Count != this.MappedEntityTypes.Count)
            {
                foreach (EntityType type in this.IsTypeOfLineInfos.Keys)
                {
                    if (!MetadataHelper.GetTypeAndSubtypesOf(type, itemCollection, false).Cast<EntityType>().Intersect<EntityType>(reachableTypes).Any<EntityType>())
                    {
                        isTypeOfEntityTypes.AddRange(type, this.IsTypeOfLineInfos.EnumerateValues(type));
                    }
                }
                foreach (EntityType type2 in this.EntityTypeLineInfos.Keys)
                {
                    if (!reachableTypes.Contains(type2))
                    {
                        entityTypes.AddRange(type2, this.EntityTypeLineInfos.EnumerateValues(type2));
                    }
                }
            }
        }

        private DomainVariable<string, ValueCondition>[] ConstructDomainVariables()
        {
            System.Data.Common.Utils.Set<ValueCondition>[] setArray = new System.Data.Common.Utils.Set<ValueCondition>[this.DiscriminatorColumns.Count];
            for (int i = 0; i < setArray.Length; i++)
            {
                setArray[i] = new System.Data.Common.Utils.Set<ValueCondition>();
                setArray[i].Add(ValueCondition.IsOther);
                setArray[i].Add(ValueCondition.IsNull);
            }
            foreach (FunctionImportNormalizedEntityTypeMapping mapping in this.NormalizedEntityTypeMappings)
            {
                for (int k = 0; k < this.DiscriminatorColumns.Count; k++)
                {
                    FunctionImportEntityTypeMappingCondition condition = mapping.ColumnConditions[k];
                    if ((condition != null) && !condition.ConditionValue.IsNotNullCondition)
                    {
                        setArray[k].Add(condition.ConditionValue);
                    }
                }
            }
            DomainVariable<string, ValueCondition>[] variableArray = new DomainVariable<string, ValueCondition>[setArray.Length];
            for (int j = 0; j < variableArray.Length; j++)
            {
                variableArray[j] = new DomainVariable<string, ValueCondition>(this.DiscriminatorColumns[j], setArray[j].MakeReadOnly());
            }
            return variableArray;
        }

        private Vertex[] ConvertMappingConditionsToVertices(ConversionContext<DomainConstraint<string, ValueCondition>> converter, DomainVariable<string, ValueCondition>[] variables)
        {
            Vertex[] vertexArray = new Vertex[this.NormalizedEntityTypeMappings.Count];
            for (int i = 0; i < vertexArray.Length; i++)
            {
                FunctionImportNormalizedEntityTypeMapping mapping = this.NormalizedEntityTypeMappings[i];
                Vertex one = Vertex.One;
                for (int j = 0; j < this.DiscriminatorColumns.Count; j++)
                {
                    FunctionImportEntityTypeMappingCondition condition = mapping.ColumnConditions[j];
                    if (condition != null)
                    {
                        ValueCondition conditionValue = condition.ConditionValue;
                        if (conditionValue.IsNotNullCondition)
                        {
                            TermExpr<DomainConstraint<string, ValueCondition>> term = new TermExpr<DomainConstraint<string, ValueCondition>>(new DomainConstraint<string, ValueCondition>(variables[j], ValueCondition.IsNull));
                            Vertex vertex = converter.TranslateTermToVertex(term);
                            one = converter.Solver.And(one, converter.Solver.Not(vertex));
                        }
                        else
                        {
                            TermExpr<DomainConstraint<string, ValueCondition>> expr2 = new TermExpr<DomainConstraint<string, ValueCondition>>(new DomainConstraint<string, ValueCondition>(variables[j], conditionValue));
                            one = converter.Solver.And(one, converter.TranslateTermToVertex(expr2));
                        }
                    }
                }
                vertexArray[i] = one;
            }
            return vertexArray;
        }

        internal EntityType Discriminate(object[] discriminatorValues)
        {
            BitArray array = new BitArray(this.MappedEntityTypes.Count, true);
            foreach (FunctionImportNormalizedEntityTypeMapping mapping in this.NormalizedEntityTypeMappings)
            {
                bool flag = true;
                System.Collections.ObjectModel.ReadOnlyCollection<FunctionImportEntityTypeMappingCondition> columnConditions = mapping.ColumnConditions;
                for (int j = 0; j < columnConditions.Count; j++)
                {
                    if ((columnConditions[j] != null) && !columnConditions[j].ColumnValueMatchesCondition(discriminatorValues[j]))
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    array = array.And(mapping.ImpliedEntityTypes);
                }
                else
                {
                    array = array.And(mapping.ComplementImpliedEntityTypes);
                }
            }
            EntityType type = null;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i])
                {
                    if (type != null)
                    {
                        throw EntityUtil.CommandExecution(System.Data.Entity.Strings.ADP_InvalidDataReaderUnableToDetermineType);
                    }
                    type = this.MappedEntityTypes[i];
                }
            }
            if (type == null)
            {
                throw EntityUtil.CommandExecution(System.Data.Entity.Strings.ADP_InvalidDataReaderUnableToDetermineType);
            }
            return type;
        }

        private System.Data.Common.Utils.Set<EntityType> FindReachableTypes(DomainConstraintConversionContext<string, ValueCondition> converter, Vertex[] mappingConditions)
        {
            System.Data.Common.Utils.Set<EntityType> set = new System.Data.Common.Utils.Set<EntityType>();
            Vertex[] source = new Vertex[this.MappedEntityTypes.Count];
            for (int j = 0; j < source.Length; j++)
            {
                Vertex one = Vertex.One;
                for (int k = 0; k < this.NormalizedEntityTypeMappings.Count; k++)
                {
                    FunctionImportNormalizedEntityTypeMapping mapping = this.NormalizedEntityTypeMappings[k];
                    if (mapping.ImpliedEntityTypes[j])
                    {
                        one = converter.Solver.And(one, mappingConditions[k]);
                    }
                    else
                    {
                        one = converter.Solver.And(one, converter.Solver.Not(mappingConditions[k]));
                    }
                }
                source[j] = one;
            }
            System.Func<Vertex, int, Vertex> selector = null;
            for (int i = 0; i < source.Length; i++)
            {
                if (selector == null)
                {
                    selector = delegate (Vertex typeCondition, int ordinal) {
                        if (ordinal != i)
                        {
                            return converter.Solver.Not(typeCondition);
                        }
                        return typeCondition;
                    };
                }
                if (!converter.Solver.And(source.Select<Vertex, Vertex>(selector)).IsZero())
                {
                    set.Add(this.MappedEntityTypes[i]);
                }
            }
            return set;
        }

        internal TypeUsage GetExpectedTargetResultType(MetadataWorkspace workspace)
        {
            IEnumerable<EntityType> mappedEntityTypes;
            Dictionary<string, TypeUsage> dictionary = new Dictionary<string, TypeUsage>();
            if (this.NormalizedEntityTypeMappings.Count == 0)
            {
                EntityType type;
                MetadataHelper.TryGetFunctionImportReturnEntityType(this.FunctionImport, out type);
                mappedEntityTypes = new EntityType[] { type };
            }
            else
            {
                mappedEntityTypes = this.MappedEntityTypes;
            }
            foreach (EntityType type2 in mappedEntityTypes)
            {
                foreach (EdmProperty property in TypeHelpers.GetAllStructuralMembers(type2))
                {
                    dictionary[property.Name] = property.TypeUsage;
                }
            }
            foreach (string str in this.DiscriminatorColumns)
            {
                if (!dictionary.ContainsKey(str))
                {
                    TypeUsage usage = TypeUsage.CreateStringTypeUsage(workspace.GetModelPrimitiveType(PrimitiveTypeKind.String), true, false);
                    dictionary.Add(str, usage);
                }
            }
            RowType edmType = new RowType(from c in dictionary select new EdmProperty(c.Key, c.Value));
            return TypeUsage.Create(new CollectionType(TypeUsage.Create(edmType)));
        }

        internal void GetUnreachableTypes(EdmItemCollection itemCollection, out KeyToListMap<EntityType, IXmlLineInfo> unreachableEntityTypes, out KeyToListMap<EntityType, IXmlLineInfo> unreachableIsTypeOfs)
        {
            DomainVariable<string, ValueCondition>[] variables = this.ConstructDomainVariables();
            DomainConstraintConversionContext<string, ValueCondition> converter = new DomainConstraintConversionContext<string, ValueCondition>();
            Vertex[] mappingConditions = this.ConvertMappingConditionsToVertices(converter, variables);
            System.Data.Common.Utils.Set<EntityType> reachableTypes = this.FindReachableTypes(converter, mappingConditions);
            this.CollectUnreachableTypes(itemCollection, reachableTypes, out unreachableEntityTypes, out unreachableIsTypeOfs);
        }
    }
}

