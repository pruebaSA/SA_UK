namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;

    public sealed class DbApplyExpression : DbExpression
    {
        private DbExpressionBinding m_apply;
        private DbExpressionBinding m_input;

        internal DbApplyExpression(DbCommandTree cmdTree, DbExpressionBinding input, DbExpressionBinding apply, DbExpressionKind applyKind) : base(cmdTree, applyKind)
        {
            DbExpressionBinding.Check("Input", input, cmdTree);
            this.m_input = input;
            DbExpressionBinding.Check("Apply", apply, cmdTree);
            this.m_apply = apply;
            if (input.VariableName.Equals(apply.VariableName, StringComparison.Ordinal))
            {
                throw EntityUtil.Argument(Strings.Cqt_Apply_DuplicateVariableNames);
            }
            List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>> {
                new KeyValuePair<string, TypeUsage>(this.m_input.VariableName, this.m_input.VariableType),
                new KeyValuePair<string, TypeUsage>(this.m_apply.VariableName, this.m_apply.VariableType)
            };
            base.ResultType = CommandTreeTypeHelper.CreateCollectionOfRowResultType(columns);
        }

        public override void Accept(DbExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw EntityUtil.ArgumentNull("visitor");
            }
            visitor.Visit(this);
        }

        public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor) => 
            visitor?.Visit(this);

        public DbExpressionBinding Apply =>
            this.m_apply;

        public DbExpressionBinding Input =>
            this.m_input;
    }
}

