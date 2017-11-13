namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbLimitExpression : DbExpression
    {
        private ExpressionLink _argument;
        private ExpressionLink _limit;
        private bool _withTies;

        internal DbLimitExpression(DbCommandTree commandTree, DbExpression argument, DbExpression limit, bool withTies) : base(commandTree, DbExpressionKind.Limit)
        {
            this._argument = new ExpressionLink("Argument", commandTree, argument);
            if (!TypeSemantics.IsCollectionType(argument.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Unary_CollectionRequired(base.GetType().Name), "Argument");
            }
            this._limit = new ExpressionLink("Limit", commandTree, limit);
            if (!TypeSemantics.IsIntegerNumericType(this._limit.Expression.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Limit_IntegerRequired, "Limit");
            }
            if ((limit.ExpressionKind != DbExpressionKind.Constant) && (limit.ExpressionKind != DbExpressionKind.ParameterReference))
            {
                throw EntityUtil.Argument(Strings.Cqt_Limit_ConstantOrParameterRefRequired, "Limit");
            }
            if (CommandTreeTypeHelper.IsConstantNegativeInteger(limit))
            {
                throw EntityUtil.Argument(Strings.Cqt_Limit_NonNegativeLimitRequired, "Limit");
            }
            this._withTies = withTies;
            base.ResultType = this._argument.Expression.ResultType;
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

        public DbExpression Argument
        {
            get => 
                this._argument.Expression;
            internal set
            {
                this._argument.Expression = value;
            }
        }

        public DbExpression Limit
        {
            get => 
                this._limit.Expression;
            internal set
            {
                this._limit.Expression = value;
            }
        }

        public bool WithTies =>
            this._withTies;
    }
}

