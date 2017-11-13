namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class StorageAssociationTypeMapping : StorageTypeMapping
    {
        private System.Data.Metadata.Edm.AssociationType m_relation;

        internal StorageAssociationTypeMapping(System.Data.Metadata.Edm.AssociationType relation, StorageSetMapping setMapping) : base(setMapping)
        {
            this.m_relation = relation;
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("AssociationTypeMapping");
            builder.Append("   ");
            builder.Append("Type Name:");
            builder.Append(this.m_relation.Name);
            builder.Append("   ");
            Console.WriteLine(builder.ToString());
            foreach (StorageMappingFragment fragment in base.MappingFragments)
            {
                fragment.Print(index + 5);
            }
        }

        internal System.Data.Metadata.Edm.AssociationType AssociationType =>
            this.m_relation;

        internal override ReadOnlyCollection<EdmType> IsOfTypes =>
            new List<EdmType>().AsReadOnly();

        internal override ReadOnlyCollection<EdmType> Types =>
            new ReadOnlyCollection<EdmType>(new System.Data.Metadata.Edm.AssociationType[] { this.m_relation });
    }
}

