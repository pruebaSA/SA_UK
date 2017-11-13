namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbIsNullExpression : DbUnaryExpression
    {
        internal DbIsNullExpression(DbCommandTree cmdTree, DbExpression arg, bool isRowTypeArgumentAllowed) : base(cmdTree, DbExpressionKind.IsNull)
        {
            if (TypeSemantics.IsCollectionType(arg.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_IsNull_CollectionNotAllowed);
            }
            if (!TypeHelpers.IsValidIsNullOpType(arg.ResultType) && (!isRowTypeArgumentAllowed || !TypeSemantics.IsRowType(arg.ResultType)))
            {
                throw EntityUtil.Argument(Strings.Cqt_IsNull_InvalidType);
            }
            base.ArgumentLink.InitializeValue(arg);
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
    }
}

