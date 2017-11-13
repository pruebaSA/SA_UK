namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Text;

    internal class ForeignConstraint : InternalBase
    {
        private List<MemberPath> m_childColumns;
        private EntitySet m_childTable;
        private AssociationSet m_fKeySet;
        private List<MemberPath> m_parentColumns;
        private EntitySet m_parentTable;

        internal ForeignConstraint(AssociationSet i_fkeySet, EntitySet i_parentTable, EntitySet i_childTable, ReadOnlyMetadataCollection<EdmProperty> i_parentColumns, ReadOnlyMetadataCollection<EdmProperty> i_childColumns, MetadataWorkspace workspace)
        {
            this.m_fKeySet = i_fkeySet;
            this.m_parentTable = i_parentTable;
            this.m_childTable = i_childTable;
            this.m_childColumns = new List<MemberPath>();
            foreach (EdmProperty property in i_childColumns)
            {
                MemberPath item = new MemberPath(this.m_childTable, property, workspace);
                this.m_childColumns.Add(item);
            }
            this.m_parentColumns = new List<MemberPath>();
            foreach (EdmProperty property2 in i_parentColumns)
            {
                MemberPath path2 = new MemberPath(this.m_parentTable, property2, workspace);
                this.m_parentColumns.Add(path2);
            }
        }

        internal void CheckConstraint(Set<Cell> cells, QueryRewriter childRewriter, QueryRewriter parentRewriter, ErrorLog errorLog, ConfigViewGenerator config, MetadataWorkspace workspace)
        {
            if (this.IsConstraintRelevantForCells(cells))
            {
                if (config.IsNormalTracing)
                {
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine(string.Empty);
                    Trace.Write("Checking: ");
                    Trace.WriteLine(this);
                }
                if ((childRewriter != null) || (parentRewriter != null))
                {
                    if (childRewriter == null)
                    {
                        string message = Strings.ViewGen_Foreign_Key_Missing_Table_Mapping_1(this.ToUserString(), this.ChildTable.Name);
                        ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyMissingTableMapping, message, parentRewriter.UsedCells, string.Empty);
                        errorLog.AddEntry(record);
                    }
                    else if (parentRewriter == null)
                    {
                        string str2 = Strings.ViewGen_Foreign_Key_Missing_Table_Mapping_1(this.ToUserString(), this.ParentTable.Name);
                        ErrorLog.Record record2 = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyMissingTableMapping, str2, childRewriter.UsedCells, string.Empty);
                        errorLog.AddEntry(record2);
                    }
                    else
                    {
                        int count = errorLog.Count;
                        if (this.IsForeignKeySuperSetOfPrimaryKeyInChildTable())
                        {
                            this.GuaranteeForeignKeyConstraintInCSpace(childRewriter, parentRewriter, errorLog, config);
                        }
                        else
                        {
                            this.GuaranteeMappedRelationshipForForeignKey(childRewriter, parentRewriter, cells, errorLog, config, workspace);
                        }
                        if (count == errorLog.Count)
                        {
                            this.CheckForeignKeyColumnOrder(cells, errorLog);
                        }
                    }
                }
            }
        }

        private bool CheckConstraintWhenOnlyParentMapped(Cell cell, EntitySet parentSet, AssociationSet assocSet, AssociationEndMember endMember, QueryRewriter childRewriter, QueryRewriter parentRewriter, ConfigViewGenerator config, MetadataWorkspace workspace)
        {
            CellNormalizer normalizer = childRewriter.Normalizer;
            CellNormalizer normalizer2 = parentRewriter.Normalizer;
            CellTreeNode basicView = parentRewriter.BasicView;
            RoleBoolean literal = new RoleBoolean(assocSet.AssociationSetEnds[endMember.Name]);
            BoolExpression whereClause = basicView.RightFragmentQuery.Condition.Create(literal);
            FragmentQuery query = FragmentQuery.Create(basicView.RightFragmentQuery.Attributes, whereClause);
            return FragmentQueryProcessor.Merge(normalizer.RightFragmentQP, normalizer2.RightFragmentQP).IsContainedIn(query, basicView.RightFragmentQuery);
        }

        private bool CheckConstraintWhenParentChildMapped(Cell cell, ErrorLog errorLog, AssociationEndMember parentEnd, ConfigViewGenerator config)
        {
            bool flag = true;
            if (parentEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many)
            {
                string message = Strings.ViewGen_Foreign_Key_UpperBound_MustBeOne_2(this.ToUserString(), cell.CQuery.Extent.Name, parentEnd.Name);
                ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyUpperBoundMustBeOne, message, cell, string.Empty);
                errorLog.AddEntry(record);
                flag = false;
            }
            if (!MemberPath.AreAllMembersNullable(this.ChildColumns) && (parentEnd.RelationshipMultiplicity != RelationshipMultiplicity.One))
            {
                string str2 = Strings.ViewGen_Foreign_Key_LowerBound_MustBeOne_2(this.ToUserString(), cell.CQuery.Extent.Name, parentEnd.Name);
                ErrorLog.Record record2 = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyLowerBoundMustBeOne, str2, cell, string.Empty);
                errorLog.AddEntry(record2);
                flag = false;
            }
            if (config.IsNormalTracing && flag)
            {
                Trace.WriteLine("Foreign key mapped to relationship " + cell.CQuery.Extent.Name);
            }
            return flag;
        }

        private bool CheckForeignKeyColumnOrder(Set<Cell> cells, ErrorLog errorLog)
        {
            List<Cell> list = new List<Cell>();
            List<Cell> list2 = new List<Cell>();
            foreach (Cell cell in cells)
            {
                if (cell.SQuery.Extent.Equals(this.ChildTable))
                {
                    list2.Add(cell);
                }
                if (cell.SQuery.Extent.Equals(this.ParentTable))
                {
                    list.Add(cell);
                }
            }
            foreach (Cell cell2 in list2)
            {
                List<List<int>> slotNumsForColumns = GetSlotNumsForColumns(cell2, this.ChildColumns);
                if (slotNumsForColumns.Count != 0)
                {
                    bool flag = false;
                    List<MemberPath> members = null;
                    List<MemberPath> list5 = null;
                    Cell cell3 = null;
                    foreach (List<int> list6 in slotNumsForColumns)
                    {
                        members = new List<MemberPath>(list6.Count);
                        foreach (int num in list6)
                        {
                            JoinTreeSlot slot = (JoinTreeSlot) cell2.CQuery.ProjectedSlotAt(num);
                            members.Add(slot.MemberPath);
                        }
                        foreach (Cell cell4 in list)
                        {
                            List<List<int>> list7 = GetSlotNumsForColumns(cell4, this.ParentColumns);
                            if (!cell4.Equals(cell2) && (list7.Count != 0))
                            {
                                foreach (List<int> list8 in list7)
                                {
                                    list5 = new List<MemberPath>(list8.Count);
                                    foreach (int num2 in list8)
                                    {
                                        JoinTreeSlot slot2 = (JoinTreeSlot) cell4.CQuery.ProjectedSlotAt(num2);
                                        list5.Add(slot2.MemberPath);
                                    }
                                    flag = members.Count != list5.Count;
                                    if (!flag)
                                    {
                                        for (int i = 0; i < members.Count; i++)
                                        {
                                            MemberPath path = list5[i];
                                            MemberPath path2 = members[i];
                                            if (!path.LastMember.Equals(path2.LastMember))
                                            {
                                                if (path.IsEquivalentViaRefConstraint(path2))
                                                {
                                                    return true;
                                                }
                                                flag = true;
                                                cell3 = cell4;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (flag)
                    {
                        string message = EntityRes.GetString("ViewGen_Foreign_Key_ColumnOrder_Incorrect_8", new object[] { this.ToUserString(), MemberPath.PropertiesToUserString(this.ChildColumns, false), this.ChildTable.Name, MemberPath.PropertiesToUserString(members, false), cell2.CQuery.Extent.Name, MemberPath.PropertiesToUserString(this.ParentColumns, false), this.ParentTable.Name, MemberPath.PropertiesToUserString(list5, false), cell3.CQuery.Extent.Name });
                        ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyColumnOrderIncorrect, message, new Cell[] { cell3, cell2 }, string.Empty);
                        errorLog.AddEntry(record);
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckParentColumnsForForeignKey(Cell cell, IEnumerable<Cell> cells, AssociationEndMember parentEnd, ref List<ErrorLog.Record> errorList)
        {
            AssociationSet extent = (AssociationSet) cell.CQuery.Extent;
            EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd(extent, parentEnd);
            EntitySet set3 = FindEntitySetForColumnsMappedToEntityKeys(cells, this.ParentColumns);
            if ((set3 != null) && entitySetAtEnd.Equals(set3))
            {
                return true;
            }
            if (errorList == null)
            {
                errorList = new List<ErrorLog.Record>();
            }
            string message = Strings.ViewGen_Foreign_Key_ParentTable_NotMappedToEnd_5(this.ToUserString(), this.ChildTable.Name, cell.CQuery.Extent.Name, parentEnd.Name, this.ParentTable.Name, entitySetAtEnd.Name);
            ErrorLog.Record item = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyParentTableNotMappedToEnd, message, cell, string.Empty);
            errorList.Add(item);
            return false;
        }

        private static EntitySet FindEntitySetForColumnsMappedToEntityKeys(IEnumerable<Cell> cells, IEnumerable<MemberPath> tableColumns)
        {
            foreach (Cell cell in cells)
            {
                CellQuery cQuery = cell.CQuery;
                if (!(cQuery.Extent is AssociationSet))
                {
                    Set<EdmProperty> cSlotsForTableColumns = cell.GetCSlotsForTableColumns(tableColumns);
                    if (cSlotsForTableColumns != null)
                    {
                        EntitySet extent = cQuery.Extent as EntitySet;
                        List<EdmProperty> elements = new List<EdmProperty>();
                        foreach (EdmProperty property in extent.ElementType.KeyMembers)
                        {
                            elements.Add(property);
                        }
                        if (new Set<EdmProperty>(elements).MakeReadOnly().SetEquals(cSlotsForTableColumns))
                        {
                            return extent;
                        }
                    }
                }
            }
            return null;
        }

        internal static List<ForeignConstraint> GetForeignConstraints(EntityContainer container, MetadataWorkspace workspace)
        {
            List<ForeignConstraint> list = new List<ForeignConstraint>();
            foreach (EntitySetBase base2 in container.BaseEntitySets)
            {
                AssociationSet set = base2 as AssociationSet;
                if (set != null)
                {
                    Dictionary<string, EntitySet> dictionary = new Dictionary<string, EntitySet>();
                    foreach (AssociationSetEnd end in set.AssociationSetEnds)
                    {
                        dictionary.Add(end.Name, end.EntitySet);
                    }
                    foreach (ReferentialConstraint constraint in set.ElementType.ReferentialConstraints)
                    {
                        EntitySet set2 = dictionary[constraint.FromRole.Name];
                        EntitySet set3 = dictionary[constraint.ToRole.Name];
                        ForeignConstraint item = new ForeignConstraint(set, set2, set3, constraint.FromProperties, constraint.ToProperties, workspace);
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private static AssociationEndMember GetRelationEndForColumns(Cell cell, IEnumerable<MemberPath> columns, MetadataWorkspace workspace)
        {
            if (!(cell.CQuery.Extent is EntitySet))
            {
                AssociationSet extent = (AssociationSet) cell.CQuery.Extent;
                foreach (AssociationSetEnd end in extent.AssociationSetEnds)
                {
                    AssociationEndMember correspondingAssociationEndMember = end.CorrespondingAssociationEndMember;
                    MemberPath prefix = new MemberPath(extent, correspondingAssociationEndMember, workspace);
                    ExtentKey primaryKeyForEntityType = ExtentKey.GetPrimaryKeyForEntityType(prefix, end.EntitySet.ElementType);
                    List<int> projectedPositions = cell.CQuery.GetProjectedPositions(primaryKeyForEntityType.KeyFields);
                    if (projectedPositions != null)
                    {
                        List<int> list2 = cell.SQuery.GetProjectedPositions(columns, projectedPositions);
                        if ((list2 != null) && Helpers.IsSetEqual<int>(list2, projectedPositions, EqualityComparer<int>.Default))
                        {
                            return correspondingAssociationEndMember;
                        }
                    }
                }
            }
            return null;
        }

        private static List<List<int>> GetSlotNumsForColumns(Cell cell, IEnumerable<MemberPath> columns)
        {
            List<List<int>> list = new List<List<int>>();
            AssociationSet extent = cell.CQuery.Extent as AssociationSet;
            if (extent != null)
            {
                foreach (AssociationSetEnd end in extent.AssociationSetEnds)
                {
                    List<int> associationEndSlots = cell.CQuery.GetAssociationEndSlots(end.CorrespondingAssociationEndMember);
                    List<int> item = cell.SQuery.GetProjectedPositions(columns, associationEndSlots);
                    if (item != null)
                    {
                        list.Add(item);
                    }
                }
                return list;
            }
            List<int> projectedPositions = cell.SQuery.GetProjectedPositions(columns);
            if (projectedPositions != null)
            {
                list.Add(projectedPositions);
            }
            return list;
        }

        private static List<LeftCellWrapper> GetWrappersFromNormalizer(CellNormalizer normalizer, EntitySetBase extent) => 
            normalizer?.AllWrappersForExtent;

        private void GuaranteeForeignKeyConstraintInCSpace(QueryRewriter childRewriter, QueryRewriter parentRewriter, ErrorLog errorLog, ConfigViewGenerator config)
        {
            CellNormalizer normalizer = childRewriter.Normalizer;
            CellNormalizer normalizer2 = parentRewriter.Normalizer;
            CellTreeNode basicView = childRewriter.BasicView;
            CellTreeNode node2 = parentRewriter.BasicView;
            if (!FragmentQueryProcessor.Merge(normalizer.RightFragmentQP, normalizer2.RightFragmentQP).IsContainedIn(basicView.RightFragmentQuery, node2.RightFragmentQuery))
            {
                LeftCellWrapper.GetExtentListAsUserString(basicView.GetLeaves());
                LeftCellWrapper.GetExtentListAsUserString(node2.GetLeaves());
                string message = Strings.ViewGen_Foreign_Key_Not_Guaranteed_InCSpace_0(this.ToUserString());
                Set<LeftCellWrapper> wrappers = new Set<LeftCellWrapper>(node2.GetLeaves());
                wrappers.AddRange(basicView.GetLeaves());
                ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyNotGuaranteedInCSpace, message, wrappers, string.Empty);
                errorLog.AddEntry(record);
            }
        }

        private void GuaranteeMappedRelationshipForForeignKey(QueryRewriter childRewriter, QueryRewriter parentRewriter, IEnumerable<Cell> cells, ErrorLog errorLog, ConfigViewGenerator config, MetadataWorkspace workspace)
        {
            CellNormalizer normalizer = childRewriter.Normalizer;
            CellNormalizer normalizer2 = parentRewriter.Normalizer;
            MemberPath prefix = new MemberPath(this.ChildTable, workspace);
            IEnumerable<MemberPath> keyFields = ExtentKey.GetPrimaryKeyForEntityType(prefix, this.ChildTable.ElementType).KeyFields;
            bool flag = false;
            bool flag2 = false;
            List<ErrorLog.Record> errorList = null;
            foreach (Cell cell in cells)
            {
                if (cell.SQuery.Extent.Equals(this.ChildTable))
                {
                    AssociationEndMember parentEnd = GetRelationEndForColumns(cell, this.ChildColumns, workspace);
                    if ((parentEnd == null) || this.CheckParentColumnsForForeignKey(cell, cells, parentEnd, ref errorList))
                    {
                        flag2 = true;
                        if (((GetRelationEndForColumns(cell, keyFields, workspace) != null) && (parentEnd != null)) && (FindEntitySetForColumnsMappedToEntityKeys(cells, keyFields) != null))
                        {
                            flag = true;
                            this.CheckConstraintWhenParentChildMapped(cell, errorLog, parentEnd, config);
                            break;
                        }
                        if (parentEnd != null)
                        {
                            AssociationSet extent = (AssociationSet) cell.CQuery.Extent;
                            EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd(extent, parentEnd);
                            flag = this.CheckConstraintWhenOnlyParentMapped(cell, entitySetAtEnd, extent, parentEnd, childRewriter, parentRewriter, config, workspace);
                            if (flag)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag2)
            {
                foreach (ErrorLog.Record record in errorList)
                {
                    errorLog.AddEntry(record);
                }
            }
            else if (!flag)
            {
                string message = Strings.ViewGen_Foreign_Key_Missing_Relationship_Mapping_0(this.ToUserString());
                IEnumerable<LeftCellWrapper> wrappersFromNormalizer = GetWrappersFromNormalizer(normalizer2, this.ParentTable);
                IEnumerable<LeftCellWrapper> elements = GetWrappersFromNormalizer(normalizer, this.ChildTable);
                Set<LeftCellWrapper> wrappers = new Set<LeftCellWrapper>(wrappersFromNormalizer);
                wrappers.AddRange(elements);
                ErrorLog.Record record2 = new ErrorLog.Record(true, ViewGenErrorCode.ForeignKeyMissingRelationshipMapping, message, wrappers, string.Empty);
                errorLog.AddEntry(record2);
            }
        }

        private bool IsConstraintRelevantForCells(IEnumerable<Cell> cells)
        {
            foreach (Cell cell in cells)
            {
                EntitySetBase extent = cell.SQuery.Extent;
                if (extent.Equals(this.m_parentTable) || extent.Equals(this.m_childTable))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsForeignKeySuperSetOfPrimaryKeyInChildTable()
        {
            foreach (EdmProperty property in this.m_childTable.ElementType.KeyMembers)
            {
                bool flag2 = false;
                foreach (MemberPath path in this.m_childColumns)
                {
                    if (path.LastMember.Equals(property))
                    {
                        flag2 = true;
                        break;
                    }
                }
                if (!flag2)
                {
                    return false;
                }
            }
            return true;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append(this.m_fKeySet.Name + ": ");
            builder.Append(this.ToUserString());
        }

        internal string ToUserString()
        {
            string str = MemberPath.PropertiesToUserString(this.m_childColumns, false);
            string str2 = MemberPath.PropertiesToUserString(this.m_parentColumns, false);
            return Strings.ViewGen_Foreign_Key_4(this.m_fKeySet.Name, this.m_childTable.Name, str, this.m_parentTable.Name, str2);
        }

        internal IEnumerable<MemberPath> ChildColumns =>
            this.m_childColumns;

        internal EntitySet ChildTable =>
            this.m_childTable;

        internal IEnumerable<MemberPath> ParentColumns =>
            this.m_parentColumns;

        internal EntitySet ParentTable =>
            this.m_parentTable;
    }
}

