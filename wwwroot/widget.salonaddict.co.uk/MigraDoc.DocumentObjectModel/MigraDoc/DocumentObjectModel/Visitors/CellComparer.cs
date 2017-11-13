namespace MigraDoc.DocumentObjectModel.Visitors
{
    using MigraDoc.DocumentObjectModel;
    using MigraDoc.DocumentObjectModel.Tables;
    using System;
    using System.Collections;

    public class CellComparer : IComparer
    {
        public int Compare(object lhs, object rhs)
        {
            if (!(lhs is Cell))
            {
                throw new ArgumentException(DomSR.CompareJustCells, "lhs");
            }
            if (!(rhs is Cell))
            {
                throw new ArgumentException(DomSR.CompareJustCells, "rhs");
            }
            Cell cell = lhs as Cell;
            Cell cell2 = rhs as Cell;
            int num = cell.Row.Index - cell2.Row.Index;
            if (num != 0)
            {
                return num;
            }
            return (cell.Column.Index - cell2.Column.Index);
        }
    }
}

