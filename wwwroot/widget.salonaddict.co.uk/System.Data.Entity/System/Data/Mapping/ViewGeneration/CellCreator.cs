namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class CellCreator : InternalBase
    {
        private StorageEntityContainerMapping m_containerMapping;
        private int m_currentCellNumber;
        private CqlIdentifiers m_identifiers;
        private MetadataWorkspace m_workspace;

        internal CellCreator(StorageEntityContainerMapping containerMapping, MetadataWorkspace workspace)
        {
            this.m_containerMapping = containerMapping;
            this.m_identifiers = new CqlIdentifiers();
            this.m_workspace = workspace;
        }

        private void ExtractCellsForContainer(StorageEntityContainerMapping containerMap, List<Cell> cells, MetadataWorkspace workspace)
        {
            foreach (StorageSetMapping mapping in containerMap.AllSetMaps)
            {
                foreach (StorageTypeMapping mapping2 in mapping.TypeMappings)
                {
                    StorageEntityTypeMapping mapping3 = mapping2 as StorageEntityTypeMapping;
                    Set<EdmType> allTypes = new Set<EdmType>();
                    if (mapping3 != null)
                    {
                        allTypes.AddRange(mapping3.Types);
                        foreach (EdmType type in mapping3.IsOfTypes)
                        {
                            IEnumerable<EdmType> elements = MetadataHelper.GetTypeAndSubtypesOf(type, this.m_workspace, false);
                            allTypes.AddRange(elements);
                        }
                    }
                    EntitySetBase set = mapping.Set;
                    foreach (StorageMappingFragment fragment in mapping2.MappingFragments)
                    {
                        this.ExtractCellsFromTableFragment(set, fragment, allTypes, cells);
                    }
                }
            }
        }

        private void ExtractCellsFromTableFragment(EntitySetBase extent, StorageMappingFragment fragmentMap, Set<EdmType> allTypes, List<Cell> cells)
        {
            ExtentJoinTreeNode node = null;
            node = new ExtentJoinTreeNode(extent, new MemberJoinTreeNode[0], this.m_workspace);
            BoolExpression @true = BoolExpression.True;
            List<ProjectedSlot> cSlots = new List<ProjectedSlot>();
            if (allTypes.Count > 0)
            {
                @true = BoolExpression.CreateLiteral(new OneOfTypeConst(node, allTypes), null);
            }
            ExtentJoinTreeNode sRootExtentNode = new ExtentJoinTreeNode(fragmentMap.TableSet, new MemberJoinTreeNode[0], this.m_workspace);
            BoolExpression sQueryWhereClause = BoolExpression.True;
            List<ProjectedSlot> sSlots = new List<ProjectedSlot>();
            this.ExtractProperties(fragmentMap.AllProperties, node, cSlots, ref @true, sRootExtentNode, sSlots, ref sQueryWhereClause);
            CellQuery cQuery = new CellQuery(cSlots, @true, node);
            CellQuery sQuery = new CellQuery(sSlots, sQueryWhereClause, sRootExtentNode);
            StorageMappingFragment fragmentInfo = fragmentMap;
            CellLabel label = new CellLabel(fragmentInfo);
            Cell item = Cell.CreateCS(cQuery, sQuery, label, this.m_currentCellNumber);
            this.m_currentCellNumber++;
            cells.Add(item);
        }

        private void ExtractProperties(IEnumerable<StoragePropertyMapping> properties, JoinTreeNode cNode, List<ProjectedSlot> cSlots, ref BoolExpression cQueryWhereClause, JoinTreeNode sRootExtentNode, List<ProjectedSlot> sSlots, ref BoolExpression sQueryWhereClause)
        {
            foreach (StoragePropertyMapping mapping in properties)
            {
                StorageScalarPropertyMapping mapping2 = mapping as StorageScalarPropertyMapping;
                StorageComplexPropertyMapping mapping3 = mapping as StorageComplexPropertyMapping;
                StorageEndPropertyMapping mapping4 = mapping as StorageEndPropertyMapping;
                StorageConditionPropertyMapping conditionMap = mapping as StorageConditionPropertyMapping;
                if (mapping2 != null)
                {
                    JoinTreeNode node = cNode.CreateAttributeNode(mapping2.EdmProperty);
                    JoinTreeNode node2 = sRootExtentNode.CreateAttributeNode(mapping2.ColumnProperty);
                    cSlots.Add(new JoinTreeSlot(node));
                    sSlots.Add(new JoinTreeSlot(node2));
                }
                if (mapping3 != null)
                {
                    foreach (StorageComplexTypeMapping mapping6 in mapping3.TypeMappings)
                    {
                        JoinTreeNode node3 = cNode.CreateAttributeNode(mapping3.EdmProperty);
                        Set<EdmType> values = new Set<EdmType>();
                        IEnumerable<EdmType> elements = Helpers.AsSuperTypeList<ComplexType, EdmType>(mapping6.Types);
                        values.AddRange(elements);
                        foreach (EdmType type in mapping6.IsOfTypes)
                        {
                            values.AddRange(MetadataHelper.GetTypeAndSubtypesOf(type, this.m_workspace, false));
                        }
                        BoolExpression expression = BoolExpression.CreateLiteral(new OneOfTypeConst(node3, values), null);
                        cQueryWhereClause = BoolExpression.CreateAnd(new BoolExpression[] { cQueryWhereClause, expression });
                        this.ExtractProperties(mapping6.AllProperties, node3, cSlots, ref cQueryWhereClause, sRootExtentNode, sSlots, ref sQueryWhereClause);
                    }
                }
                if (mapping4 != null)
                {
                    JoinTreeNode node4 = cNode.CreateAttributeNode(mapping4.EndMember);
                    this.ExtractProperties(mapping4.Properties, node4, cSlots, ref cQueryWhereClause, sRootExtentNode, sSlots, ref sQueryWhereClause);
                }
                if (conditionMap != null)
                {
                    if (conditionMap.ColumnProperty != null)
                    {
                        BoolExpression conditionExpression = GetConditionExpression(sRootExtentNode, conditionMap);
                        sQueryWhereClause = BoolExpression.CreateAnd(new BoolExpression[] { sQueryWhereClause, conditionExpression });
                    }
                    else
                    {
                        BoolExpression expression3 = GetConditionExpression(cNode, conditionMap);
                        cQueryWhereClause = BoolExpression.CreateAnd(new BoolExpression[] { cQueryWhereClause, expression3 });
                    }
                }
            }
        }

        internal List<Cell> GenerateCells(ConfigViewGenerator config)
        {
            List<Cell> cells = new List<Cell>();
            if (config.IsNormalTracing)
            {
                this.m_containerMapping.Print(0);
            }
            this.ExtractCellsForContainer(this.m_containerMapping, cells, this.m_workspace);
            this.m_identifiers.AddIdentifier(this.m_containerMapping.EdmEntityContainer.Name);
            this.m_identifiers.AddIdentifier(this.m_containerMapping.StorageEntityContainer.Name);
            foreach (Cell cell in cells)
            {
                cell.GetIdentifiers(this.m_identifiers);
            }
            return cells;
        }

        private static BoolExpression GetConditionExpression(JoinTreeNode joinTreeNode, StorageConditionPropertyMapping conditionMap)
        {
            EdmMember member = (conditionMap.ColumnProperty != null) ? conditionMap.ColumnProperty : conditionMap.EdmProperty;
            JoinTreeNode node = joinTreeNode.CreateAttributeNode(member);
            OneOfConst literal = null;
            if (conditionMap.IsNull.HasValue)
            {
                CellConstant constant = conditionMap.IsNull.Value ? CellConstant.Null : CellConstant.NotNull;
                if (MetadataHelper.IsNonRefSimpleMember(member))
                {
                    literal = new OneOfScalarConst(node, constant);
                }
                else
                {
                    literal = new OneOfTypeConst(node, constant);
                }
            }
            else
            {
                TypeUsage memberType = (conditionMap.ColumnProperty != null) ? conditionMap.ColumnProperty.TypeUsage : conditionMap.EdmProperty.TypeUsage;
                literal = new OneOfScalarConst(node, conditionMap.Value, memberType);
            }
            return BoolExpression.CreateLiteral(literal, null);
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("CellCreator");
        }

        internal CqlIdentifiers Identifiers =>
            this.m_identifiers;
    }
}

