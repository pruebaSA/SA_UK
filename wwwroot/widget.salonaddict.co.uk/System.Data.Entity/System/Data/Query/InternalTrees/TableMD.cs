namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Query.PlanCompiler;

    internal class TableMD
    {
        private List<ColumnMD> m_columns;
        private EntitySetBase m_extent;
        private bool m_flattened;
        private List<ColumnMD> m_keys;

        private TableMD(EntitySetBase extent)
        {
            this.m_columns = new List<ColumnMD>();
            this.m_keys = new List<ColumnMD>();
            this.m_extent = extent;
        }

        internal TableMD(TypeUsage type, EntitySetBase extent) : this(extent)
        {
            this.m_columns.Add(new ColumnMD(this, "element", type));
            this.m_flattened = !TypeUtils.IsStructuredType(type);
        }

        internal TableMD(IEnumerable<EdmProperty> properties, IEnumerable<EdmMember> keyProperties, EntitySetBase extent) : this(extent)
        {
            Dictionary<string, ColumnMD> dictionary = new Dictionary<string, ColumnMD>();
            this.m_flattened = true;
            foreach (EdmProperty property in properties)
            {
                ColumnMD item = new ColumnMD(this, property);
                this.m_columns.Add(item);
                dictionary[property.Name] = item;
            }
            foreach (EdmMember member in keyProperties)
            {
                ColumnMD nmd2;
                if (dictionary.TryGetValue(member.Name, out nmd2))
                {
                    this.m_keys.Add(nmd2);
                }
            }
        }

        public override string ToString() => 
            this.m_extent?.Name;

        internal List<ColumnMD> Columns =>
            this.m_columns;

        internal EntitySetBase Extent =>
            this.m_extent;

        internal bool Flattened =>
            this.m_flattened;

        internal List<ColumnMD> Keys =>
            this.m_keys;
    }
}

