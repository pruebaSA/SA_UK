namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlUpdate : SqlStatement
    {
        private List<SqlAssign> assignments;
        private SqlSelect select;

        internal SqlUpdate(SqlSelect select, IEnumerable<SqlAssign> assignments, Expression sourceExpression) : base(SqlNodeType.Update, sourceExpression)
        {
            this.Select = select;
            this.assignments = new List<SqlAssign>(assignments);
        }

        internal List<SqlAssign> Assignments =>
            this.assignments;

        internal SqlSelect Select
        {
            get => 
                this.select;
            set
            {
                if (value == null)
                {
                    throw System.Data.Linq.SqlClient.Error.ArgumentNull("value");
                }
                this.select = value;
            }
        }
    }
}

