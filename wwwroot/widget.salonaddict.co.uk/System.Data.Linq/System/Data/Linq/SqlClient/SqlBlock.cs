﻿namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class SqlBlock : SqlStatement
    {
        private List<SqlStatement> statements;

        internal SqlBlock(Expression sourceExpression) : base(SqlNodeType.Block, sourceExpression)
        {
            this.statements = new List<SqlStatement>();
        }

        internal List<SqlStatement> Statements =>
            this.statements;
    }
}

