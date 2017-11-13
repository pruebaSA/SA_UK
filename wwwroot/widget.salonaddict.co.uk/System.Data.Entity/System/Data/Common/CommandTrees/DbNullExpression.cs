namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;

    public sealed class DbNullExpression : DbExpression
    {
        internal DbNullExpression(DbCommandTree commandTree, TypeUsage type) : base(commandTree, DbExpressionKind.Null)
        {
            commandTree.TypeHelper.CheckType(type);
            base.ResultType = type;
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
    }
}

