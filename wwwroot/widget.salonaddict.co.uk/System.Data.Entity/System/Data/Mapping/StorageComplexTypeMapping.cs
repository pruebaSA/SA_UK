namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class StorageComplexTypeMapping
    {
        private Dictionary<EdmProperty, StoragePropertyMapping> m_conditionProperties = new Dictionary<EdmProperty, StoragePropertyMapping>(EqualityComparer<EdmProperty>.Default);
        private Dictionary<string, ComplexType> m_isOfTypes = new Dictionary<string, ComplexType>(StringComparer.Ordinal);
        private bool m_isPartial;
        private Dictionary<string, StoragePropertyMapping> m_properties = new Dictionary<string, StoragePropertyMapping>(StringComparer.Ordinal);
        private Dictionary<string, ComplexType> m_types = new Dictionary<string, ComplexType>(StringComparer.Ordinal);

        internal StorageComplexTypeMapping(bool isPartial)
        {
            this.m_isPartial = isPartial;
        }

        internal void AddConditionProperty(StorageConditionPropertyMapping conditionPropertyMap)
        {
            EdmProperty key = (conditionPropertyMap.EdmProperty != null) ? conditionPropertyMap.EdmProperty : conditionPropertyMap.ColumnProperty;
            if (this.m_conditionProperties.ContainsKey(key))
            {
                throw new MappingException(Strings.Mapping_InvalidContent_Duplicate_Condition_Member_1(key.Name));
            }
            this.m_conditionProperties.Add(key, conditionPropertyMap);
        }

        internal void AddIsOfType(ComplexType type)
        {
            this.m_isOfTypes.Add(type.FullName, type);
        }

        internal void AddProperty(StoragePropertyMapping prop)
        {
            this.m_properties.Add(prop.EdmProperty.Name, prop);
        }

        internal void AddType(ComplexType type)
        {
            this.m_types.Add(type.FullName, type);
        }

        internal ComplexType GetOwnerType(string memberName)
        {
            foreach (ComplexType type in this.m_types.Values)
            {
                EdmMember member;
                if (type.Members.TryGetValue(memberName, false, out member) && (member is EdmProperty))
                {
                    return type;
                }
            }
            foreach (ComplexType type2 in this.m_isOfTypes.Values)
            {
                EdmMember member2;
                if (type2.Members.TryGetValue(memberName, false, out member2) && (member2 is EdmProperty))
                {
                    return type2;
                }
            }
            return null;
        }

        internal void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("ComplexTypeMapping");
            builder.Append("   ");
            if (this.m_isPartial)
            {
                builder.Append("IsPartial:True");
            }
            builder.Append("   ");
            foreach (ComplexType type in this.m_types.Values)
            {
                builder.Append("Types:");
                builder.Append(type.FullName);
                builder.Append("   ");
            }
            foreach (ComplexType type2 in this.m_isOfTypes.Values)
            {
                builder.Append("Is-Of Types:");
                builder.Append(type2.FullName);
                builder.Append("   ");
            }
            Console.WriteLine(builder.ToString());
            foreach (StorageConditionPropertyMapping mapping in this.m_conditionProperties.Values)
            {
                mapping.Print(index + 5);
            }
            foreach (StoragePropertyMapping mapping2 in this.Properties)
            {
                mapping2.Print(index + 5);
            }
        }

        internal ReadOnlyCollection<StoragePropertyMapping> AllProperties
        {
            get
            {
                List<StoragePropertyMapping> list = new List<StoragePropertyMapping>();
                list.AddRange(this.m_properties.Values);
                list.AddRange(this.m_conditionProperties.Values);
                return list.AsReadOnly();
            }
        }

        internal ReadOnlyCollection<ComplexType> IsOfTypes =>
            new List<ComplexType>(this.m_isOfTypes.Values).AsReadOnly();

        internal ReadOnlyCollection<StoragePropertyMapping> Properties =>
            new List<StoragePropertyMapping>(this.m_properties.Values).AsReadOnly();

        internal ReadOnlyCollection<ComplexType> Types =>
            new List<ComplexType>(this.m_types.Values).AsReadOnly();
    }
}

