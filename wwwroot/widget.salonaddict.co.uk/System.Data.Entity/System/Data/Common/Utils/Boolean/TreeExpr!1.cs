namespace System.Data.Common.Utils.Boolean
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;

    internal abstract class TreeExpr<T_Identifier> : BoolExpr<T_Identifier>
    {
        private readonly Set<BoolExpr<T_Identifier>> _children;
        private readonly int _hashCode;

        protected TreeExpr(IEnumerable<BoolExpr<T_Identifier>> children)
        {
            this._children = new Set<BoolExpr<T_Identifier>>(children);
            this._children.MakeReadOnly();
            this._hashCode = this._children.GetElementsHashCode();
        }

        public override bool Equals(object obj) => 
            base.Equals(obj as BoolExpr<T_Identifier>);

        protected override bool EquivalentTypeEquals(BoolExpr<T_Identifier> other) => 
            ((TreeExpr<T_Identifier>) other).Children.SetEquals(this.Children);

        public override int GetHashCode() => 
            this._hashCode;

        public override string ToString() => 
            StringUtil.FormatInvariant("{0}({1})", new object[] { this.ExprType, this._children });

        internal Set<BoolExpr<T_Identifier>> Children =>
            this._children;
    }
}

