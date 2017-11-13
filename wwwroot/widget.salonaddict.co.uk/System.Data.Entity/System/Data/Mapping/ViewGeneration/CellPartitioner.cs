namespace System.Data.Mapping.ViewGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Mapping.Update.Internal;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Validation;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class CellPartitioner : InternalBase
    {
        private IEnumerable<Cell> m_cells;
        private IEnumerable<ForeignConstraint> m_foreignKeyConstraints;

        internal CellPartitioner(IEnumerable<Cell> cells, IEnumerable<ForeignConstraint> foreignKeyConstraints)
        {
            this.m_foreignKeyConstraints = foreignKeyConstraints;
            this.m_cells = cells;
        }

        private static bool AreCellsConnectedViaRelationship(Cell cell1, Cell cell2)
        {
            AssociationSet extent = cell1.CQuery.Extent as AssociationSet;
            AssociationSet relationshipSet = cell2.CQuery.Extent as AssociationSet;
            return (((extent != null) && MetadataHelper.IsExtentAtSomeRelationshipEnd(extent, cell2.CQuery.Extent)) || ((relationshipSet != null) && MetadataHelper.IsExtentAtSomeRelationshipEnd(relationshipSet, cell1.CQuery.Extent)));
        }

        private static List<Set<Cell>> GenerateConnectedComponents(UndirectedGraph<Cell> graph)
        {
            KeyToListMap<int, Cell> map = graph.GenerateConnectedComponents();
            List<Set<Cell>> list = new List<Set<Cell>>();
            foreach (int num in map.Keys)
            {
                Set<Cell> item = new Set<Cell>(map.ListForKey(num));
                list.Add(item);
            }
            return list;
        }

        internal List<Set<Cell>> GroupRelatedCells()
        {
            UndirectedGraph<Cell> graph = new UndirectedGraph<Cell>(EqualityComparer<Cell>.Default);
            List<Cell> list = new List<Cell>();
            foreach (Cell cell in this.m_cells)
            {
                graph.AddVertex(cell);
                EntitySetBase extent = cell.CQuery.Extent;
                EntitySetBase base3 = cell.SQuery.Extent;
                foreach (Cell cell2 in list)
                {
                    EntitySetBase base4 = cell2.CQuery.Extent;
                    EntitySetBase base5 = cell2.SQuery.Extent;
                    bool flag = base4.Equals(extent) || base5.Equals(base3);
                    bool flag2 = this.OverlapViaForeignKeys(cell, cell2);
                    bool flag3 = AreCellsConnectedViaRelationship(cell, cell2);
                    if ((flag || flag2) || flag3)
                    {
                        graph.AddEdge(cell2, cell);
                    }
                }
                list.Add(cell);
            }
            return GenerateConnectedComponents(graph);
        }

        private bool OverlapViaForeignKeys(Cell cell1, Cell cell2)
        {
            EntitySetBase extent = cell1.SQuery.Extent;
            EntitySetBase base3 = cell2.SQuery.Extent;
            foreach (ForeignConstraint constraint in this.m_foreignKeyConstraints)
            {
                if ((extent.Equals(constraint.ParentTable) && base3.Equals(constraint.ChildTable)) || (base3.Equals(constraint.ParentTable) && extent.Equals(constraint.ChildTable)))
                {
                    return true;
                }
            }
            return false;
        }

        internal override void ToCompactString(StringBuilder builder)
        {
            Cell.CellsToBuilder(builder, this.m_cells);
        }
    }
}

