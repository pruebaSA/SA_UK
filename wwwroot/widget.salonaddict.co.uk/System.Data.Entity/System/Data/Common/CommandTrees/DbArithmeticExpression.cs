namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbArithmeticExpression : DbExpression
    {
        private readonly ExpressionList _args;

        internal DbArithmeticExpression(DbCommandTree commandTree, DbExpressionKind kind, IList<DbExpression> args) : base(commandTree, kind)
        {
            EntityUtil.CheckArgumentNull<IList<DbExpression>>(args, "args");
            ExpressionList list = new ExpressionList("Arguments", commandTree, args.Count);
            list.SetElements(args);
            TypeUsage commonElementType = list.GetCommonElementType();
            if (TypeSemantics.IsNullOrNullType(commonElementType) || !TypeSemantics.IsNumericType(commonElementType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Arithmetic_NumericCommonType);
            }
            list.SetExpectedElementType(commonElementType);
            this._args = list;
            base.ResultType = commonElementType;
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

        public IList<DbExpression> Arguments =>
            this._args;
    }
}

