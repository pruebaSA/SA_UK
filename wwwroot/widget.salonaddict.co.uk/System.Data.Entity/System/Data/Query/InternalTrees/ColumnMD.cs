namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;

    internal class ColumnMD
    {
        private string m_name;
        private EdmMember m_property;
        private TypeUsage m_type;

        internal ColumnMD(TableMD table, EdmMember property) : this(table, property.Name, property.TypeUsage)
        {
            this.m_property = property;
        }

        internal ColumnMD(TableMD table, string name, TypeUsage type)
        {
            this.m_name = name;
            this.m_type = type;
        }

        public override string ToString() => 
            this.m_name;

        internal bool IsNullable
        {
            get
            {
                if (this.m_property != null)
                {
                    return TypeSemantics.IsNullable(this.m_property);
                }
                return true;
            }
        }

        internal string Name =>
            this.m_name;

        internal TypeUsage Type =>
            this.m_type;
    }
}

