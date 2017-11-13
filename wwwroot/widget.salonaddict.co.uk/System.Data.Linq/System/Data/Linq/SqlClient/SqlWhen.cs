namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlWhen
    {
        private SqlExpression matchExpression;
        private SqlExpression valueExpression;

        internal SqlWhen(SqlExpression match, SqlExpression value)
        {
            if (value == null)
            {
                throw Error.ArgumentNull("value");
            }
            this.Match = match;
            this.Value = value;
        }

        internal SqlExpression Match
        {
            get => 
                this.matchExpression;
            set
            {
                if (((this.matchExpression != null) && (value != null)) && (this.matchExpression.ClrType != value.ClrType))
                {
                    throw Error.ArgumentWrongType("value", this.matchExpression.ClrType, value.ClrType);
                }
                this.matchExpression = value;
            }
        }

        internal SqlExpression Value
        {
            get => 
                this.valueExpression;
            set
            {
                if (value == null)
                {
                    throw Error.ArgumentNull("value");
                }
                if ((this.valueExpression != null) && !this.valueExpression.ClrType.IsAssignableFrom(value.ClrType))
                {
                    throw Error.ArgumentWrongType("value", this.valueExpression.ClrType, value.ClrType);
                }
                this.valueExpression = value;
            }
        }
    }
}

