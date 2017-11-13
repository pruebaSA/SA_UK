namespace System.Data.Linq.SqlClient
{
    using System;

    internal class HierarchyChecker
    {
        internal static bool HasHierarchy(SqlExpression expr)
        {
            Visitor visitor = new Visitor();
            visitor.Visit(expr);
            return visitor.foundHierarchy;
        }

        private class Visitor : SqlVisitor
        {
            internal bool foundHierarchy;

            internal override SqlExpression VisitClientQuery(SqlClientQuery cq)
            {
                this.foundHierarchy = true;
                return cq;
            }

            internal override SqlExpression VisitElement(SqlSubSelect elem)
            {
                this.foundHierarchy = true;
                return elem;
            }

            internal override SqlExpression VisitExists(SqlSubSelect ss) => 
                ss;

            internal override SqlExpression VisitMultiset(SqlSubSelect sms)
            {
                this.foundHierarchy = true;
                return sms;
            }

            internal override SqlExpression VisitScalarSubSelect(SqlSubSelect ss) => 
                ss;
        }
    }
}

