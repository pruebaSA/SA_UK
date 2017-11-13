namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;

    public sealed class DbSortExpression : DbExpression
    {
        private DbExpressionBinding _input;
        private IList<DbSortClause> _keys;

        internal DbSortExpression(DbCommandTree cmdTree, DbExpressionBinding input, IList<DbSortClause> sortOrder) : base(cmdTree, DbExpressionKind.Sort)
        {
            CheckSortArguments(cmdTree, input, sortOrder);
            this._input = input;
            this._keys = CommandTreeUtils.NewReadOnlyList<DbSortClause>(sortOrder);
            base.ResultType = input.Expression.ResultType;
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

        internal static void CheckSortArguments(DbCommandTree cmdTree, DbExpressionBinding input, IList<DbSortClause> keys)
        {
            DbExpressionBinding.Check("Input", input, cmdTree);
            EntityUtil.CheckArgumentNull<IList<DbSortClause>>(keys, "SortOrder");
            if (keys.Count < 1)
            {
                throw EntityUtil.Argument(Strings.Cqt_SkipSort_AtLeastOneKey, "SortOrder");
            }
            for (int i = 0; i < keys.Count; i++)
            {
                DbSortClause clause = keys[i];
                if (clause == null)
                {
                    throw EntityUtil.ArgumentNull(CommandTreeUtils.FormatIndex("SortOrder", i));
                }
                if (!object.ReferenceEquals(clause.Expression.CommandTree, cmdTree))
                {
                    throw EntityUtil.Argument(Strings.Cqt_General_TreeMismatch, CommandTreeUtils.FormatIndex("SortOrder", i));
                }
            }
        }

        public DbExpressionBinding Input =>
            this._input;

        public IList<DbSortClause> SortOrder =>
            this._keys;
    }
}

