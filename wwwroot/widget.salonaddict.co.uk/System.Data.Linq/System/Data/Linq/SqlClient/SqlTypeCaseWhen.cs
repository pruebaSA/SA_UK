namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlTypeCaseWhen
    {
        private SqlExpression match;
        private SqlExpression @new;

        internal SqlTypeCaseWhen(SqlExpression match, SqlExpression typeBinding)
        {
            this.Match = match;
            this.TypeBinding = typeBinding;
        }

        internal SqlExpression Match
        {
            get => 
                this.match;
            set
            {
                if (((this.match != null) && (value != null)) && (this.match.ClrType != value.ClrType))
                {
                    throw Error.ArgumentWrongType("value", this.match.ClrType, value.ClrType);
                }
                this.match = value;
            }
        }

        internal SqlExpression TypeBinding
        {
            get => 
                this.@new;
            set
            {
                this.@new = value;
            }
        }
    }
}

