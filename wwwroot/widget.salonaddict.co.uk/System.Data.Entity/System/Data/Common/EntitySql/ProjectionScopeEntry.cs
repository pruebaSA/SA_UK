namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data.Common.CommandTrees;

    internal class ProjectionScopeEntry : ScopeEntry
    {
        private string _varName;

        internal ProjectionScopeEntry(string varName, DbExpression expression) : base(ScopeEntryKind.ProjectList, expression)
        {
            this._varName = varName;
        }

        internal override DbExpression Expression =>
            base._expression;
    }
}

