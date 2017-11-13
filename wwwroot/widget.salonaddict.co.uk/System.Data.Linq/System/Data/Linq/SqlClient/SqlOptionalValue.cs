namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlOptionalValue : SqlSimpleTypeExpression
    {
        private SqlExpression expressionValue;
        private SqlExpression hasValue;

        internal SqlOptionalValue(SqlExpression hasValue, SqlExpression value) : base(SqlNodeType.OptionalValue, value.ClrType, value.SqlType, value.SourceExpression)
        {
            this.HasValue = hasValue;
            this.Value = value;
        }

        internal SqlExpression HasValue
        {
            get => 
                this.hasValue;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                this.hasValue = value;
            }
        }

        internal SqlExpression Value
        {
            get => 
                this.expressionValue;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                if (value.ClrType != base.ClrType)
                {
                    throw Error.ArgumentWrongType("value", base.ClrType, value.ClrType);
                }
                this.expressionValue = value;
            }
        }
    }
}

