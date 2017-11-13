namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbElementExpression : DbUnaryExpression
    {
        private bool _singlePropertyUnwrapped;

        internal DbElementExpression(DbCommandTree cmdTree, DbExpression arg, bool unwrapSingleProperty) : base(cmdTree, DbExpressionKind.Element, arg)
        {
            base.CheckCollectionArgument();
            this._singlePropertyUnwrapped = unwrapSingleProperty;
            TypeUsage typeUsage = TypeHelpers.GetEdmType<CollectionType>(arg.ResultType).TypeUsage;
            if (unwrapSingleProperty)
            {
                IList<EdmProperty> properties = TypeHelpers.GetProperties(typeUsage);
                if ((properties == null) || (properties.Count != 1))
                {
                    throw EntityUtil.Argument(Strings.Cqt_Element_InvalidArgumentForUnwrapSingleProperty, "arg");
                }
                typeUsage = properties[0].TypeUsage;
            }
            base.ResultType = typeUsage;
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

        internal bool IsSinglePropertyUnwrapped =>
            this._singlePropertyUnwrapped;
    }
}

