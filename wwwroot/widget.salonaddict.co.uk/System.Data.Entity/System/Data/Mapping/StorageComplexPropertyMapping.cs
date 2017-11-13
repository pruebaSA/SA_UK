namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class StorageComplexPropertyMapping : StoragePropertyMapping
    {
        private List<StorageComplexTypeMapping> m_typeMappings;

        internal StorageComplexPropertyMapping(EdmProperty cdmMember) : base(cdmMember)
        {
            this.m_typeMappings = new List<StorageComplexTypeMapping>();
        }

        internal void AddTypeMapping(StorageComplexTypeMapping typeMapping)
        {
            this.m_typeMappings.Add(typeMapping);
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("ComplexPropertyMapping");
            builder.Append("   ");
            if (this.EdmProperty != null)
            {
                builder.Append("Name:");
                builder.Append(this.EdmProperty.Name);
                builder.Append("   ");
            }
            Console.WriteLine(builder.ToString());
            foreach (StorageComplexTypeMapping mapping in this.TypeMappings)
            {
                mapping.Print(index + 5);
            }
        }

        internal ReadOnlyCollection<StorageComplexTypeMapping> TypeMappings =>
            this.m_typeMappings.AsReadOnly();
    }
}

