namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public abstract class DbUnaryExpression : DbExpression
    {
        private ExpressionLink _argLink;

        internal DbUnaryExpression(DbCommandTree commandTree, DbExpressionKind kind) : base(commandTree, kind)
        {
            this._argLink = new ExpressionLink("Argument", commandTree);
        }

        internal DbUnaryExpression(DbCommandTree commandTree, DbExpressionKind kind, DbExpression arg) : base(commandTree, kind)
        {
            this._argLink = new ExpressionLink("Argument", commandTree, arg);
        }

        internal void CheckCollectionArgument()
        {
            if (!TypeSemantics.IsCollectionType(this.Argument.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Unary_CollectionRequired(base.GetType().Name));
            }
        }

        public DbExpression Argument
        {
            get => 
                this._argLink.Expression;
            internal set
            {
                this._argLink.Expression = value;
            }
        }

        internal ExpressionLink ArgumentLink =>
            this._argLink;
    }
}

