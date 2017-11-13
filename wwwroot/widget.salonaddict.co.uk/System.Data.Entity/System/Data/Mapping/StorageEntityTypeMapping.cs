namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class StorageEntityTypeMapping : StorageTypeMapping
    {
        private Dictionary<string, EdmType> m_entityTypes;
        private Dictionary<string, EdmType> m_isOfEntityTypes;

        internal StorageEntityTypeMapping(StorageSetMapping setMapping) : base(setMapping)
        {
            this.m_entityTypes = new Dictionary<string, EdmType>(StringComparer.Ordinal);
            this.m_isOfEntityTypes = new Dictionary<string, EdmType>(StringComparer.Ordinal);
        }

        internal void AddIsOfType(EdmType type)
        {
            this.m_isOfEntityTypes.Add(type.FullName, type);
        }

        internal void AddType(EdmType type)
        {
            this.m_entityTypes.Add(type.FullName, type);
        }

        internal EntityType GetContainerType(string memberName)
        {
            foreach (EntityType type in this.m_entityTypes.Values)
            {
                if (type.Properties.Contains(memberName))
                {
                    return type;
                }
            }
            foreach (EntityType type2 in this.m_isOfEntityTypes.Values)
            {
                if (type2.Properties.Contains(memberName))
                {
                    return type2;
                }
            }
            return null;
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("EntityTypeMapping");
            builder.Append("   ");
            foreach (EdmType type in this.m_entityTypes.Values)
            {
                builder.Append("Types:");
                builder.Append(type.FullName);
                builder.Append("   ");
            }
            foreach (EdmType type2 in this.m_isOfEntityTypes.Values)
            {
                builder.Append("Is-Of Types:");
                builder.Append(type2.FullName);
                builder.Append("   ");
            }
            Console.WriteLine(builder.ToString());
            foreach (StorageMappingFragment fragment in base.MappingFragments)
            {
                fragment.Print(index + 5);
            }
        }

        internal override ReadOnlyCollection<EdmType> IsOfTypes =>
            new List<EdmType>(this.m_isOfEntityTypes.Values).AsReadOnly();

        internal override ReadOnlyCollection<EdmType> Types =>
            new List<EdmType>(this.m_entityTypes.Values).AsReadOnly();
    }
}

