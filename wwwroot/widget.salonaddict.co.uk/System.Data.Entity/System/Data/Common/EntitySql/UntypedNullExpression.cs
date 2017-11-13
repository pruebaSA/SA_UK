namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;

    internal sealed class UntypedNullExpression : DbExpression
    {
        internal UntypedNullExpression(DbCommandTree commandTree) : base(commandTree, DbExpressionKind.Null)
        {
            base.ResultType = MetadataItem.NullType;
        }

        public override void Accept(DbExpressionVisitor visitor)
        {
            throw EntityUtil.NotSupported();
        }

        public override T Accept<T>(DbExpressionVisitor<T> visitor)
        {
            throw EntityUtil.NotSupported();
        }
    }
}

