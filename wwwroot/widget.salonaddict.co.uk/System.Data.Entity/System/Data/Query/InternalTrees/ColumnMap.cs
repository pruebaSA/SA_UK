namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;

    internal abstract class ColumnMap
    {
        internal const string DefaultColumnName = "Value";
        private string m_name;
        private TypeUsage m_type;

        internal ColumnMap(TypeUsage type, string name)
        {
            this.m_type = type;
            this.m_name = name;
        }

        [DebuggerNonUserCode]
        internal abstract void Accept<TArgType>(ColumnMapVisitor<TArgType> visitor, TArgType arg);
        [DebuggerNonUserCode]
        internal abstract TResultType Accept<TResultType, TArgType>(ColumnMapVisitorWithResults<TResultType, TArgType> visitor, TArgType arg);

        internal bool IsNamed =>
            (this.m_name != null);

        internal string Name
        {
            get => 
                this.m_name;
            set
            {
                this.m_name = value;
            }
        }

        internal TypeUsage Type =>
            this.m_type;
    }
}

