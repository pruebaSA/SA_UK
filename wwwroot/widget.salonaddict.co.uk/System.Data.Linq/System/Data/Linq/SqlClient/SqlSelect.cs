namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlSelect : SqlStatement
    {
        private SqlSource from;
        private List<SqlExpression> groupBy;
        private SqlExpression having;
        private bool isDistinct;
        private bool isPercent;
        private List<SqlOrderExpression> orderBy;
        private SqlOrderingType orderingType;
        private SqlRow row;
        private SqlExpression selection;
        private bool squelch;
        private SqlExpression top;
        private SqlExpression where;

        internal SqlSelect(SqlExpression selection, SqlSource from, Expression sourceExpression) : base(SqlNodeType.Select, sourceExpression)
        {
            this.Row = new SqlRow(sourceExpression);
            this.Selection = selection;
            this.From = from;
            this.groupBy = new List<SqlExpression>();
            this.orderBy = new List<SqlOrderExpression>();
            this.orderingType = SqlOrderingType.Default;
        }

        internal bool DoNotOutput
        {
            get => 
                this.squelch;
            set
            {
                this.squelch = value;
            }
        }

        internal SqlSource From
        {
            get => 
                this.from;
            set
            {
                this.from = value;
            }
        }

        internal List<SqlExpression> GroupBy =>
            this.groupBy;

        internal SqlExpression Having
        {
            get => 
                this.having;
            set
            {
                if ((value != null) && (TypeSystem.GetNonNullableType(value.ClrType) != typeof(bool)))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", "bool", value.ClrType);
                }
                this.having = value;
            }
        }

        internal bool IsDistinct
        {
            get => 
                this.isDistinct;
            set
            {
                this.isDistinct = value;
            }
        }

        internal bool IsPercent
        {
            get => 
                this.isPercent;
            set
            {
                this.isPercent = value;
            }
        }

        internal List<SqlOrderExpression> OrderBy =>
            this.orderBy;

        internal SqlOrderingType OrderingType
        {
            get => 
                this.orderingType;
            set
            {
                this.orderingType = value;
            }
        }

        internal SqlRow Row
        {
            get => 
                this.row;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.row = value;
            }
        }

        internal SqlExpression Selection
        {
            get => 
                this.selection;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.selection = value;
            }
        }

        internal SqlExpression Top
        {
            get => 
                this.top;
            set
            {
                this.top = value;
            }
        }

        internal SqlExpression Where
        {
            get => 
                this.where;
            set
            {
                if ((value != null) && (TypeSystem.GetNonNullableType(value.ClrType) != typeof(bool)))
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentWrongType("value", "bool", value.ClrType);
                }
                this.where = value;
            }
        }
    }
}

