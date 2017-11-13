namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbSkipExpression : DbExpression
    {
        private ExpressionLink _count;
        private DbExpressionBinding _input;
        private IList<DbSortClause> _keys;

        internal DbSkipExpression(DbCommandTree commandTree, DbExpressionBinding input, IList<DbSortClause> sortOrder, DbExpression count) : base(commandTree, DbExpressionKind.Skip)
        {
            DbSortExpression.CheckSortArguments(commandTree, input, sortOrder);
            this._input = input;
            this._keys = CommandTreeUtils.NewReadOnlyList<DbSortClause>(sortOrder);
            this._count = new ExpressionLink("Count", commandTree, count);
            if (!TypeSemantics.IsIntegerNumericType(this._count.Expression.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Skip_IntegerRequired, "Count");
            }
            if ((count.ExpressionKind != DbExpressionKind.Constant) && (count.ExpressionKind != DbExpressionKind.ParameterReference))
            {
                throw EntityUtil.Argument(Strings.Cqt_Skip_ConstantOrParameterRefRequired, "Count");
            }
            if (CommandTreeTypeHelper.IsConstantNegativeInteger(count))
            {
                throw EntityUtil.Argument(Strings.Cqt_Skip_NonNegativeCountRequired, "Count");
            }
            base.ResultType = this._input.Expression.ResultType;
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

        public DbExpression Count
        {
            get => 
                this._count.Expression;
            internal set
            {
                this._count.Expression = value;
            }
        }

        public DbExpressionBinding Input =>
            this._input;

        public IList<DbSortClause> SortOrder =>
            this._keys;
    }
}

