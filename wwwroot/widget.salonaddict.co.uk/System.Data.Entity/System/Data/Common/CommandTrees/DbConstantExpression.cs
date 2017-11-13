namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbConstantExpression : DbExpression
    {
        private object _value;

        internal DbConstantExpression(DbCommandTree commandTree, object value) : base(commandTree, DbExpressionKind.Constant)
        {
            PrimitiveTypeKind kind;
            if (value == null)
            {
                throw EntityUtil.ArgumentNull("value");
            }
            if (!CommandTreeUtils.TryGetPrimtiveTypeKind(value.GetType(), out kind))
            {
                throw EntityUtil.Argument(Strings.Cqt_Constant_InvalidType, "value");
            }
            TypeUsage literalTypeUsage = TypeHelpers.GetLiteralTypeUsage(kind);
            this._value = value;
            base.ResultType = literalTypeUsage;
        }

        internal DbConstantExpression(DbCommandTree commandTree, object value, TypeUsage constantType) : base(commandTree, DbExpressionKind.Constant)
        {
            PrimitiveType type;
            PrimitiveTypeKind kind;
            EntityUtil.CheckArgumentNull<object>(value, "value");
            commandTree.TypeHelper.CheckType(constantType, "constantType");
            if (!TypeHelpers.TryGetEdmType<PrimitiveType>(constantType, out type))
            {
                throw EntityUtil.Argument(Strings.Cqt_Constant_InvalidConstantType(constantType.ToString()), "constantType");
            }
            if (!CommandTreeUtils.TryGetPrimtiveTypeKind(value.GetType(), out kind) || (type.PrimitiveTypeKind != kind))
            {
                throw EntityUtil.Argument(Strings.Cqt_Constant_InvalidValueForType(constantType.ToString()), "value");
            }
            this._value = value;
            base.ResultType = constantType;
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

        public object Value =>
            this._value;
    }
}

