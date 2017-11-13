namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Text;

    internal class StorageEndPropertyMapping : StoragePropertyMapping
    {
        private RelationshipEndMember m_endMember;
        private Dictionary<string, StoragePropertyMapping> m_properties;

        internal StorageEndPropertyMapping(EdmProperty member) : base(member)
        {
            this.m_properties = new Dictionary<string, StoragePropertyMapping>(StringComparer.Ordinal);
        }

        internal void AddProperty(StoragePropertyMapping prop)
        {
            this.m_properties.Add(prop.EdmProperty.Name, prop);
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("EndPropertyMapping");
            builder.Append("   ");
            if (this.EndMember != null)
            {
                builder.Append("Name:");
                builder.Append(this.EndMember.Name);
                builder.Append("   ");
                builder.Append("TypeName:");
                builder.Append(this.EndMember.TypeUsage.EdmType.FullName);
            }
            builder.Append("   ");
            Console.WriteLine(builder.ToString());
            foreach (StoragePropertyMapping mapping in this.Properties)
            {
                mapping.Print(index + 5);
            }
        }

        internal RelationshipEndMember EndMember
        {
            get => 
                this.m_endMember;
            set
            {
                this.m_endMember = value;
            }
        }

        internal ReadOnlyCollection<StoragePropertyMapping> Properties =>
            new List<StoragePropertyMapping>(this.m_properties.Values).AsReadOnly();

        internal IEnumerable<EdmMember> StoreProperties =>
            (from propertyMap in this.m_properties.Values.OfType<StorageScalarPropertyMapping>() select propertyMap.ColumnProperty).Cast<EdmMember>();
    }
}

