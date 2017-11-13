namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlAliasRef : SqlExpression
    {
        private SqlAlias alias;

        internal SqlAliasRef(SqlAlias alias) : base(SqlNodeType.AliasRef, GetClrType(alias.Node), alias.SourceExpression)
        {
            if (alias == null)
            {
                throw Error.ArgumentNull("alias");
            }
            this.alias = alias;
        }

        private static Type GetClrType(SqlNode node)
        {
            SqlTableValuedFunctionCall call = node as SqlTableValuedFunctionCall;
            if (call != null)
            {
                return call.RowType.Type;
            }
            SqlExpression expression = node as SqlExpression;
            if (expression != null)
            {
                if (TypeSystem.IsSequenceType(expression.ClrType))
                {
                    return TypeSystem.GetElementType(expression.ClrType);
                }
                return expression.ClrType;
            }
            SqlSelect select = node as SqlSelect;
            if (select != null)
            {
                return select.Selection.ClrType;
            }
            SqlTable table = node as SqlTable;
            if (table != null)
            {
                return table.RowType.Type;
            }
            SqlUnion union = node as SqlUnion;
            return union?.GetClrType();
        }

        private static ProviderType GetSqlType(SqlNode node)
        {
            SqlExpression expression = node as SqlExpression;
            if (expression != null)
            {
                return expression.SqlType;
            }
            SqlSelect select = node as SqlSelect;
            if (select != null)
            {
                return select.Selection.SqlType;
            }
            SqlTable table = node as SqlTable;
            if (table != null)
            {
                return table.SqlRowType;
            }
            SqlUnion union = node as SqlUnion;
            return union?.GetSqlType();
        }

        internal SqlAlias Alias =>
            this.alias;

        internal override ProviderType SqlType =>
            GetSqlType(this.alias.Node);
    }
}

