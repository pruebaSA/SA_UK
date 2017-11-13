namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlDiscriminatorOf : SqlSimpleTypeExpression
    {
        private SqlExpression obj;

        internal SqlDiscriminatorOf(SqlExpression obj, Type clrType, ProviderType sqlType, Expression sourceExpression) : base(SqlNodeType.DiscriminatorOf, clrType, sqlType, sourceExpression)
        {
            this.obj = obj;
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
    }
}

