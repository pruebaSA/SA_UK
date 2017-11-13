namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlSubSelect : SqlSimpleTypeExpression
    {
        private SqlSelect select;

        internal SqlSubSelect(SqlNodeType nt, Type clrType, ProviderType sqlType, SqlSelect select) : base(nt, clrType, sqlType, select.SourceExpression)
        {
            switch (nt)
            {
                case SqlNodeType.Element:
                case SqlNodeType.Exists:
                case SqlNodeType.Multiset:
                case SqlNodeType.ScalarSubSelect:
                    this.Select = select;
                    return;
            }
            throw Error.UnexpectedNode(nt);
        }

        internal SqlSelect Select
        {
            get => 
                this.select;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                this.select = value;
            }
        }
    }
}

