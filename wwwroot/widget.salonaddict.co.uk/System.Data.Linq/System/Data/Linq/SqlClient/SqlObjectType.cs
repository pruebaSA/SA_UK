namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlObjectType : SqlExpression
    {
        private SqlExpression obj;
        private ProviderType sqlType;

        internal SqlObjectType(SqlExpression obj, ProviderType sqlType, Expression sourceExpression) : base(SqlNodeType.ObjectType, typeof(Type), sourceExpression)
        {
            this.obj = obj;
            this.sqlType = sqlType;
        }

        internal void SetSqlType(ProviderType type)
        {
            this.sqlType = type;
        }

        internal SqlExpression Object
        {
            get => 
                this.obj;
            set
            {
                this.obj = value;
            }
        }

        internal override ProviderType SqlType =>
            this.sqlType;
    }
}

