namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbTreatExpression : DbUnaryExpression
    {
        internal DbTreatExpression(DbCommandTree cmdTree, TypeUsage type, DbExpression arg) : base(cmdTree, DbExpressionKind.Treat)
        {
            cmdTree.TypeHelper.CheckPolymorphicType(type);
            if (!TypeSemantics.IsValidPolymorphicCast(arg.ResultType, type))
            {
                throw EntityUtil.Argument(Strings.Cqt_General_PolymorphicArgRequired(base.GetType().Name));
            }
            base.ArgumentLink.InitializeValue(arg);
            base.ArgumentLink.SetExpectedType(arg.ResultType);
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

