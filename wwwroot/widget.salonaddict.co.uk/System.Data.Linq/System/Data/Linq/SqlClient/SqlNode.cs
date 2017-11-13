namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Diagnostics;
    using System.Linq.Expressions;

    [DebuggerDisplay("text = {Text}, \r\nsource = {SourceExpression}")]
    internal abstract class SqlNode
    {
        private SqlNodeType nodeType;
        private Expression sourceExpression;

        internal SqlNode(SqlNodeType nodeType, Expression sourceExpression)
        {
            this.nodeType = nodeType;
            this.sourceExpression = sourceExpression;
        }

        internal void ClearSourceExpression()
        {
            this.sourceExpression = null;
        }

        internal SqlNodeType NodeType =>
            this.nodeType;

        internal Expression SourceExpression =>
            this.sourceExpression;
    }
}

