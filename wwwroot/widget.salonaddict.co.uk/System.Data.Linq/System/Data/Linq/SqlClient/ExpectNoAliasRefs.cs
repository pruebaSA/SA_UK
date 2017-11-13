namespace System.Data.Linq.SqlClient
{
    internal class ExpectNoAliasRefs : SqlVisitor
    {
        internal override SqlExpression VisitAliasRef(SqlAliasRef aref)
        {
            throw Error.UnexpectedNode(aref.NodeType);
        }
    }
}

