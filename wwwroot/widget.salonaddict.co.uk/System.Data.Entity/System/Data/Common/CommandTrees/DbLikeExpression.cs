namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Metadata.Edm;

    public sealed class DbLikeExpression : DbExpression
    {
        private ExpressionLink _argument;
        private ExpressionLink _escape;
        private ExpressionLink _pattern;

        internal DbLikeExpression(DbCommandTree cmdTree, DbExpression input, DbExpression pattern, DbExpression escape) : base(cmdTree, DbExpressionKind.Like)
        {
            this._argument = new ExpressionLink("Argument", cmdTree, PrimitiveTypeKind.String, input);
            this._pattern = new ExpressionLink("Pattern", cmdTree, PrimitiveTypeKind.String, pattern);
            if (escape == null)
            {
                escape = cmdTree.CreateNullExpression(pattern.ResultType);
            }
            this._escape = new ExpressionLink("Escape", cmdTree, PrimitiveTypeKind.String, escape);
            base.ResultType = cmdTree.TypeHelper.CreateBooleanResultType();
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

        public DbExpression Escape
        {
            get => 
                this._escape.Expression;
            internal set
            {
                this._escape.Expression = value;
            }
        }

        public DbExpression Pattern
        {
            get => 
                this._pattern.Expression;
            internal set
            {
                this._pattern.Expression = value;
            }
        }
    }
}

