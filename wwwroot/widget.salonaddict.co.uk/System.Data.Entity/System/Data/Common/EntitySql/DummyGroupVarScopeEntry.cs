namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data.Common.CommandTrees;

    internal class DummyGroupVarScopeEntry : ScopeEntry
    {
        private DbExpression _aggreateExpression;

        internal DummyGroupVarScopeEntry(DbExpression varBasedExpression, DbExpression groupVarBasedExpression) : base(ScopeEntryKind.DummyGroupKey, varBasedExpression)
        {
            this._aggreateExpression = groupVarBasedExpression;
            base.VarKind = SourceVarKind.GroupKey;
        }

        internal DbExpression AggregateExpression =>
            this._aggreateExpression;
    }
}

