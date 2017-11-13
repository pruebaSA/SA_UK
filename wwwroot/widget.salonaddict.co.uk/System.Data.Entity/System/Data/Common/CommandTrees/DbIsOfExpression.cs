namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbIsOfExpression : DbUnaryExpression
    {
        private TypeUsage _ofType;

        internal DbIsOfExpression(DbCommandTree cmdTree, DbExpressionKind isOfKind, TypeUsage type, DbExpression arg) : base(cmdTree, isOfKind)
        {
            cmdTree.TypeHelper.CheckPolymorphicType(type);
            if (!TypeSemantics.IsValidPolymorphicCast(arg.ResultType, type))
            {
                throw EntityUtil.Argument(Strings.Cqt_General_PolymorphicArgRequired(base.GetType().Name));
            }
            this._ofType = type;
            base.ArgumentLink.SetExpectedType(arg.ResultType);
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

        public TypeUsage OfType =>
            this._ofType;
    }
}

