namespace System.Data.Linq.SqlClient
{
    using System;

    internal class MultisetChecker
    {
        internal static bool HasMultiset(SqlExpression expr)
        {
            Visitor visitor = new Visitor();
            visitor.Visit(expr);
            return visitor.foundMultiset;
        }

        private class Visitor : SqlVisitor
        {
            internal bool foundMultiset;

            internal override SqlExpression VisitClientQuery(SqlClientQuery cq) => 
                cq;

            internal override SqlExpression VisitElement(SqlSubSelect elem) => 
                elem;

            internal override SqlExpression VisitExists(SqlSubSelect ss) => 
                ss;

            internal override SqlExpression VisitMultiset(SqlSubSelect sms)
            {
                this.foundMultiset = true;
                return sms;
            }

            internal override SqlExpression VisitScalarSubSelect(SqlSubSelect ss) => 
                ss;
        }
    }
}

