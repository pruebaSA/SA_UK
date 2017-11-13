namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Linq.Expressions;

    internal class SqlJoin : SqlSource
    {
        private SqlExpression condition;
        private SqlJoinType joinType;
        private SqlSource left;
        private SqlSource right;

        internal SqlJoin(SqlJoinType type, SqlSource left, SqlSource right, SqlExpression cond, Expression sourceExpression) : base(SqlNodeType.Join, sourceExpression)
        {
            this.JoinType = type;
            this.Left = left;
            this.Right = right;
            this.Condition = cond;
        }

        internal SqlExpression Condition
        {
            get => 
                this.condition;
            set
            {
                this.condition = value;
            }
        }

        internal SqlJoinType JoinType
        {
            get => 
                this.joinType;
            set
            {
                this.joinType = value;
            }
        }

        internal SqlSource Left
        {
            get => 
                this.left;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.left = value;
            }
        }

        internal SqlSource Right
        {
            get => 
                this.right;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.right = value;
            }
        }
    }
}

