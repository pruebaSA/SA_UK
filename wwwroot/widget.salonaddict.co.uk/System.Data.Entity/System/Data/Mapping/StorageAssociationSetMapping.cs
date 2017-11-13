namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Text;

    internal class StorageAssociationSetMapping : StorageSetMapping
    {
        private StorageAssociationSetFunctionMapping m_functionMapping;

        internal StorageAssociationSetMapping(AssociationSet extent, StorageEntityContainerMapping entityContainerMapping) : base(extent, entityContainerMapping)
        {
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("AssociationSetMapping");
            builder.Append("   ");
            builder.Append("Name:");
            builder.Append(base.Set.Name);
            if (base.QueryView != null)
            {
                builder.Append("   ");
                builder.Append("Query View:");
                builder.Append(base.QueryView);
            }
            Console.WriteLine(builder.ToString());
            foreach (StorageTypeMapping mapping in base.TypeMappings)
            {
                mapping.Print(index + 5);
            }
            if (this.m_functionMapping != null)
            {
                this.m_functionMapping.Print(index + 5);
            }
        }

        internal StorageAssociationSetFunctionMapping FunctionMapping
        {
            get => 
                this.m_functionMapping;
            set
            {
                this.m_functionMapping = value;
            }
        }

        internal EntitySetBase StoreEntitySet
        {
            get
            {
                if ((base.TypeMappings.Count != 0) && (base.TypeMappings.First<StorageTypeMapping>().MappingFragments.Count != 0))
                {
                    return base.TypeMappings.First<StorageTypeMapping>().MappingFragments.First<StorageMappingFragment>().TableSet;
                }
                return null;
            }
        }
    }
}

