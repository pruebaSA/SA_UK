namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlAssign : SqlStatement
    {
        private SqlExpression leftValue;
        private SqlExpression rightValue;

        internal SqlAssign(SqlExpression lValue, SqlExpression rValue, Expression sourceExpression) : base(SqlNodeType.Assign, sourceExpression)
        {
            this.LValue = lValue;
            this.RValue = rValue;
        }

        internal SqlExpression LValue
        {
            get => 
                this.leftValue;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if ((this.rightValue != null) && !value.ClrType.IsAssignableFrom(this.rightValue.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.rightValue.ClrType, value.ClrType);
                }
                this.leftValue = value;
            }
        }

        internal SqlExpression RValue
        {
            get => 
                this.rightValue;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                if ((this.leftValue != null) && !this.leftValue.ClrType.IsAssignableFrom(value.ClrType))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", this.leftValue.ClrType, value.ClrType);
                }
                this.rightValue = value;
            }
        }
    }
}

