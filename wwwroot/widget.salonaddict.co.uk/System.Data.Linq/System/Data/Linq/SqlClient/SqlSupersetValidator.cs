namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal class SqlSupersetValidator
    {
        private List<SqlVisitor> validators = new List<SqlVisitor>();

        internal void AddValidator(SqlVisitor validator)
        {
            this.validators.Add(validator);
        }

        internal void Validate(SqlNode node)
        {
            foreach (SqlVisitor visitor in this.validators)
            {
                visitor.Visit(node);
            }
        }
    }
}

