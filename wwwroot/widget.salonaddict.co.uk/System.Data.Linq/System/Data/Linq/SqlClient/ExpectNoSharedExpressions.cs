namespace System.Data.Linq.SqlClient
{
    internal class ExpectNoSharedExpressions : SqlVisitor
    {
        internal override SqlExpression VisitSharedExpression(SqlSharedExpression shared)
        {
            throw Error.UnexpectedSharedExpression();
        }

        internal override SqlExpression VisitSharedExpressionRef(SqlSharedExpressionRef sref)
        {
            throw Error.UnexpectedSharedExpressionReference();
        }
    }
}

