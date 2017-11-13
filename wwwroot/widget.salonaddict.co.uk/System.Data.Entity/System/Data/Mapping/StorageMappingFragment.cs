namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class StorageMappingFragment
    {
        private Dictionary<EdmProperty, StoragePropertyMapping> m_conditionProperties = new Dictionary<EdmProperty, StoragePropertyMapping>(EqualityComparer<EdmProperty>.Default);
        private int m_endLineNumber;
        private int m_endLinePosition;
        private List<StoragePropertyMapping> m_properties = new List<StoragePropertyMapping>();
        private int m_startLineNumber;
        private int m_startLinePosition;
        private EntitySet m_tableExtent;
        private StorageTypeMapping m_typeMapping;

        internal StorageMappingFragment(EntitySet tableExtent, StorageTypeMapping typeMapping)
        {
            this.m_tableExtent = tableExtent;
            this.m_typeMapping = typeMapping;
        }

        internal bool AddConditionProperty(StorageConditionPropertyMapping conditionPropertyMap)
        {
            EdmProperty key = (conditionPropertyMap.EdmProperty != null) ? conditionPropertyMap.EdmProperty : conditionPropertyMap.ColumnProperty;
            if (this.m_conditionProperties.ContainsKey(key))
            {
                return false;
            }
            this.m_conditionProperties.Add(key, conditionPropertyMap);
            return true;
        }

        internal void AddProperty(StoragePropertyMapping prop)
        {
            this.m_properties.Add(prop);
        }

        internal virtual void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("MappingFragment");
            builder.Append("   ");
            builder.Append("Table Name:");
            builder.Append(this.m_tableExtent.Name);
            Console.WriteLine(builder.ToString());
            foreach (StorageConditionPropertyMapping mapping in this.m_conditionProperties.Values)
            {
                mapping.Print(index + 5);
            }
            foreach (StoragePropertyMapping mapping2 in this.m_properties)
            {
                mapping2.Print(index + 5);
            }
        }

        internal ReadOnlyCollection<StoragePropertyMapping> AllProperties
        {
            get
            {
                List<StoragePropertyMapping> list = new List<StoragePropertyMapping>();
                list.AddRange(this.m_properties);
                list.AddRange(this.m_conditionProperties.Values);
                return list.AsReadOnly();
            }
        }

        internal int EndLineNumber
        {
            set
            {
                this.m_endLineNumber = value;
            }
        }

        internal int EndLinePosition
        {
            set
            {
                this.m_endLinePosition = value;
            }
        }

        internal ReadOnlyCollection<StoragePropertyMapping> Properties =>
            this.m_properties.AsReadOnly();

        internal string SourceLocation =>
            this.m_typeMapping.SetMapping.EntityContainerMapping.SourceLocation;

        internal int StartLineNumber
        {
            get => 
                this.m_startLineNumber;
            set
            {
                this.m_startLineNumber = value;
            }
        }

        internal int StartLinePosition
        {
            get => 
                this.m_startLinePosition;
            set
            {
                this.m_startLinePosition = value;
            }
        }

        internal EntitySet TableSet =>
            this.m_tableExtent;
    }
}

