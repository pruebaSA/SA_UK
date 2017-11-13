namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlLift : SqlExpression
    {
        internal SqlExpression liftedExpression;

        internal SqlLift(Type type, SqlExpression liftedExpression, System.Linq.Expressions.Expression sourceExpression) : base(SqlNodeType.Lift, type, sourceExpression)
        {
            if (liftedExpression == null)
            {
                throw System.Data.Linq.SqlClient.Error.ArgumentNull("liftedExpression");
            }
            this.liftedExpression = liftedExpression;
        }

        internal SqlExpression Expression
        {
            get => 
                this.liftedExpression;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.liftedExpression = value;
            }
        }

        internal override ProviderType SqlType =>
            this.liftedExpression.SqlType;
    }
}

