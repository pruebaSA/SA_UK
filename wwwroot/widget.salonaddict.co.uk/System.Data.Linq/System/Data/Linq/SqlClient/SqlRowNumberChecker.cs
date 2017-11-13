namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Runtime.CompilerServices;

    internal class SqlRowNumberChecker
    {
        private Visitor rowNumberVisitor = new Visitor();

        internal SqlRowNumberChecker()
        {
        }

        internal bool HasRowNumber(SqlNode node)
        {
            this.rowNumberVisitor.Visit(node);
            return this.rowNumberVisitor.HasRowNumber;
        }

        internal bool HasRowNumber(SqlRow row)
        {
            foreach (SqlColumn column in row.Columns)
            {
                if (this.HasRowNumber(column))
                {
                    return true;
                }
            }
            return false;
        }

        internal SqlColumn RowNumberColumn
        {
            get
            {
                if (!this.rowNumberVisitor.HasRowNumber)
                {
                    return null;
                }
                return this.rowNumberVisitor.CurrentColumn;
            }
        }

        private class Visitor : SqlVisitor
        {
            private bool hasRowNumber;

            internal override SqlRow VisitRow(SqlRow row)
            {
                int num = 0;
                int count = row.Columns.Count;
                while (num < count)
                {
                    row.Columns[num].Expression = this.VisitExpression(row.Columns[num].Expression);
                    if (this.hasRowNumber)
                    {
                        this.CurrentColumn = row.Columns[num];
                        return row;
                    }
                    num++;
                }
                return row;
            }

            internal override SqlRowNumber VisitRowNumber(SqlRowNumber rowNumber)
            {
                this.hasRowNumber = true;
                return rowNumber;
            }

            internal override SqlExpression VisitScalarSubSelect(SqlSubSelect ss) => 
                ss;

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                this.Visit(select.Row);
                this.Visit(select.Where);
                return select;
            }

            internal override SqlExpression VisitSubSelect(SqlSubSelect ss) => 
                ss;

            public SqlColumn CurrentColumn { get; private set; }

            public bool HasRowNumber =>
                this.hasRowNumber;
        }
    }
}

