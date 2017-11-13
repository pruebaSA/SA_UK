namespace System.Data.Common.CommandTrees.Internal
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.CompilerServices;

    internal class ExpressionLink
    {
        private TypeUsage _expectedType;
        private DbExpression _expression;
        private string _name;
        private DbCommandTree _tree;

        internal event ExpressionLinkConstraint ValueChanging;

        internal ExpressionLink(string name, DbCommandTree commandTree)
        {
            this._name = name;
            this._tree = EntityUtil.CheckArgumentNull<DbCommandTree>(commandTree, "commandTree");
        }

        internal ExpressionLink(string strName, DbCommandTree commandTree, DbExpression initialValue) : this(strName, commandTree)
        {
            this.InitializeValue(initialValue);
            this.SetExpectedType(initialValue.ResultType);
        }

        internal ExpressionLink(string name, DbCommandTree commandTree, TypeUsage expectedType) : this(name, commandTree)
        {
            this.SetExpectedType(expectedType);
        }

        internal ExpressionLink(string name, DbCommandTree commandTree, PrimitiveTypeKind expectedPrimitiveType, DbExpression initialValue) : this(name, commandTree)
        {
            PrimitiveTypeKind kind;
            EntityUtil.CheckArgumentNull<DbExpression>(initialValue, this.Name);
            bool flag = TypeHelpers.TryGetPrimitiveTypeKind(initialValue.ResultType, out kind);
            if (!flag || (kind != expectedPrimitiveType))
            {
                throw EntityUtil.Argument(Strings.Cqt_ExpressionLink_TypeMismatch(Enum.GetName(typeof(PrimitiveTypeKind), expectedPrimitiveType), flag ? Enum.GetName(typeof(PrimitiveTypeKind), kind) : TypeHelpers.GetFullName(initialValue.ResultType)), this.Name);
            }
            this.SetExpectedType(initialValue.ResultType);
            this.InitializeValue(initialValue);
        }

        internal ExpressionLink(string name, DbCommandTree commandTree, TypeUsage expectedType, DbExpression initialValue) : this(name, commandTree)
        {
            this.SetExpectedType(expectedType);
            this.InitializeValue(initialValue);
        }

        private bool CheckValue(DbExpression newValue)
        {
            EntityUtil.CheckArgumentNull<DbExpression>(newValue, this.Name);
            if (object.ReferenceEquals(newValue, this._expression))
            {
                return false;
            }
            if (!object.ReferenceEquals(this._tree, newValue.CommandTree))
            {
                throw EntityUtil.Argument(Strings.Cqt_General_TreeMismatch, this.Name);
            }
            if ((this._expectedType != null) && !TypeSemantics.IsEquivalentOrPromotableTo(newValue.ResultType, this._expectedType))
            {
                throw EntityUtil.Argument(Strings.Cqt_ExpressionLink_TypeMismatch(TypeHelpers.GetFullName(this._expectedType), TypeHelpers.GetFullName(newValue.ResultType)), this.Name);
            }
            if (TypeSemantics.IsNullType(newValue.ResultType))
            {
                throw EntityUtil.Argument(Strings.Cqt_ExpressionLink_NullTypeInvalid(this.Name), this.Name);
            }
            if (this.ValueChanging != null)
            {
                this.ValueChanging(this, newValue);
            }
            return true;
        }

        internal void InitializeValue(DbExpression initialValue)
        {
            this.CheckValue(initialValue);
            this.UpdateValue(initialValue);
        }

        internal void SetExpectedType(TypeUsage expectedType)
        {
            this._expectedType = expectedType;
        }

        private void UpdateValue(DbExpression newValue)
        {
            this._expression = newValue;
        }

        public DbExpression Expression
        {
            get => 
                this._expression;
            set
            {
                if (this.CheckValue(value))
                {
                    this.UpdateValue(value);
                    this._tree.SetModified();
                }
            }
        }

        internal string Name =>
            this._name;
    }
}

