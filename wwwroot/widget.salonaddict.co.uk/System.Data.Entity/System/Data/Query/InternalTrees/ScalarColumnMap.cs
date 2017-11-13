namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;

    internal class ScalarColumnMap : SimpleColumnMap
    {
        private int m_columnPos;
        private int m_commandId;

        internal ScalarColumnMap(TypeUsage type, string name, int commandId, int columnPos) : base(type, name)
        {
            this.m_commandId = commandId;
            this.m_columnPos = columnPos;
        }

        [DebuggerNonUserCode]
        internal override void Accept<TArgType>(ColumnMapVisitor<TArgType> visitor, TArgType arg)
        {
            visitor.Visit(this, arg);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType, TArgType>(ColumnMapVisitorWithResults<TResultType, TArgType> visitor, TArgType arg) => 
            visitor.Visit(this, arg);

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "S({0},{1})", new object[] { this.CommandId, this.ColumnPos });

        internal int ColumnPos =>
            this.m_columnPos;

        internal int CommandId =>
            this.m_commandId;
    }
}

