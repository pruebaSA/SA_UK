namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbJoinExpression : DbExpression
    {
        private ExpressionLink _cond;
        private DbExpressionBinding _left;
        private DbExpressionBinding _right;

        internal DbJoinExpression(DbCommandTree cmdTree, DbExpressionKind joinKind, DbExpressionBinding left, DbExpressionBinding right, DbExpression joinCond) : base(cmdTree, joinKind)
        {
            DbExpressionBinding.Check("Left", left, cmdTree);
            DbExpressionBinding.Check("Right", right, cmdTree);
            if (left.VariableName.Equals(right.VariableName, StringComparison.Ordinal))
            {
                throw EntityUtil.Argument(Strings.Cqt_Join_DuplicateVariableNames);
            }
            this._left = left;
            this._right = right;
            this._cond = new ExpressionLink("JoinCondition", cmdTree, PrimitiveTypeKind.Boolean, joinCond);
            List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>(2) {
                new KeyValuePair<string, TypeUsage>(left.VariableName, left.VariableType),
                new KeyValuePair<string, TypeUsage>(right.VariableName, right.VariableType)
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

        public DbExpression JoinCondition
        {
            get => 
                this._cond.Expression;
            internal set
            {
                this._cond.Expression = value;
            }
        }

        public DbExpressionBinding Left =>
            this._left;

        public DbExpressionBinding Right =>
            this._right;
    }
}

