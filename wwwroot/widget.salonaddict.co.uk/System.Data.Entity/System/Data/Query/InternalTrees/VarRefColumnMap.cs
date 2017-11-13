namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;

    internal class VarRefColumnMap : SimpleColumnMap
    {
        private System.Data.Query.InternalTrees.Var m_var;

        internal VarRefColumnMap(System.Data.Query.InternalTrees.Var v) : this(v.Type, null, v)
        {
        }

        internal VarRefColumnMap(TypeUsage type, string name, System.Data.Query.InternalTrees.Var v) : base(type, name)
        {
            this.m_var = v;
        }

        [DebuggerNonUserCode]
        internal override void Accept<TArgType>(ColumnMapVisitor<TArgType> visitor, TArgType arg)
        {
            visitor.Visit(this, arg);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType, TArgType>(ColumnMapVisitorWithResults<TResultType, TArgType> visitor, TArgType arg) => 
            visitor.Visit(this, arg);

        public override string ToString()
        {
            if (!base.IsNamed)
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { this.m_var.Id });
            }
            return base.Name;
        }

        internal System.Data.Query.InternalTrees.Var Var =>
            this.m_var;
    }
}

