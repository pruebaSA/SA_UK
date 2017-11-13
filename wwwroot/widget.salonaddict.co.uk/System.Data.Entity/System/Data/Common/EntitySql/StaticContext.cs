namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;

    internal sealed class StaticContext : ScopeManager
    {
        internal StaticContext(IEqualityComparer<string> keyComparer) : base(keyComparer)
        {
        }

        internal void AddAggregateToScope(string aggregateName, DbVariableReferenceExpression sourceVar)
        {
            base.Add(aggregateName, new SourceScopeEntry(ScopeEntryKind.SourceVar, aggregateName, sourceVar, SourceVarKind.GroupAggregate));
        }

        internal void AddGroupDummyVar(string groupKey, DbExpression varBasedExpression, DbExpression groupVarBasedExpression)
        {
            base.Add(groupKey, new DummyGroupVarScopeEntry(varBasedExpression, groupVarBasedExpression));
        }

        internal void AddSourceBinding(DbExpressionBinding sourceBinding, ScopeEntryKind scopeEntryKind, string varTag)
        {
            base.Add(sourceBinding.VariableName, new SourceScopeEntry(scopeEntryKind, sourceBinding, varTag));
        }

        internal void ReplaceGroupVarInScope(string groupVarName, DbVariableReferenceExpression groupSourceBinding)
        {
            if (!base.IsInCurrentScope(groupVarName))
            {
                throw EntityUtil.EntitySqlError(Strings.CouldNotFindAggregateKey);
            }
            base.RemoveFromScope(groupVarName);
            base.Add(groupVarName, new SourceScopeEntry(ScopeEntryKind.SourceVar, groupVarName, groupSourceBinding, SourceVarKind.GroupKey));
        }
    }
}

