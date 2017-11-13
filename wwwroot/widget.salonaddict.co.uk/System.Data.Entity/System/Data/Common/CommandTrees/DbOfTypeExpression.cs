namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbOfTypeExpression : DbUnaryExpression
    {
        private TypeUsage _ofType;

        internal DbOfTypeExpression(DbCommandTree cmdTree, DbExpressionKind ofTypeKind, TypeUsage type, DbExpression arg) : base(cmdTree, ofTypeKind, arg)
        {
            cmdTree.TypeHelper.CheckPolymorphicType(type);
            base.CheckCollectionArgument();
            TypeUsage elementType = null;
            if (!TypeHelpers.TryGetCollectionElementType(arg.ResultType, out elementType) || !TypeSemantics.IsValidPolymorphicCast(elementType, type))
            {
                throw EntityUtil.Argument(Strings.Cqt_General_PolymorphicArgRequired(base.GetType().Name));
            }
            base.ResultType = CommandTreeTypeHelper.CreateCollectionResultType(type);
            this._ofType = type;
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

