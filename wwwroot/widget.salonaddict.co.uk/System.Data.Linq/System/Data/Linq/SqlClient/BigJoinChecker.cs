namespace System.Data.Linq.SqlClient
{
    using System;

    internal class BigJoinChecker
    {
        internal static bool CanBigJoin(SqlSelect select)
        {
            Visitor visitor = new Visitor();
            visitor.Visit(select);
            return visitor.canBigJoin;
        }

        private class Visitor : SqlVisitor
        {
            internal bool canBigJoin = true;

            internal override SqlExpression VisitClientQuery(SqlClientQuery cq) => 
                cq;

            internal override SqlExpression VisitElement(SqlSubSelect elem) => 
                elem;

            internal override SqlExpression VisitExists(SqlSubSelect ss) => 
                ss;

            internal override SqlExpression VisitMultiset(SqlSubSelect sms) => 
                sms;

            internal override SqlExpression VisitScalarSubSelect(SqlSubSelect ss) => 
                ss;

            internal override SqlSelect VisitSelect(SqlSelect select)
            {
                this.canBigJoin &= ((select.GroupBy.Count == 0) && (select.Top == null)) && !select.IsDistinct;
                if (!this.canBigJoin)
                {
                    return select;
                }
                return base.VisitSelect(select);
            }
        }
    }
}

