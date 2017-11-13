namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Metadata.Edm;
    using System.Linq;

    internal class ExplicitDiscriminatorMap
    {
        private readonly EdmMember m_discriminatorProperty;
        private readonly ReadOnlyCollection<EdmProperty> m_properties;
        private readonly ReadOnlyCollection<KeyValuePair<object, EntityType>> m_typeMap;

        internal ExplicitDiscriminatorMap(DiscriminatorMap template)
        {
            this.m_typeMap = template.TypeMap;
            this.m_discriminatorProperty = template.Discriminator.Property;
            this.m_properties = (from propertyValuePair in template.PropertyMap select propertyValuePair.Key).ToList<EdmProperty>().AsReadOnly();
        }

        internal object GetTypeId(EntityType entityType)
        {
            foreach (KeyValuePair<object, EntityType> pair in this.TypeMap)
            {
                if (pair.Value.EdmEquals(entityType))
                {
                    return pair.Key;
                }
            }
            return null;
        }

        internal EdmMember DiscriminatorProperty =>
            this.m_discriminatorProperty;

        internal ReadOnlyCollection<EdmProperty> Properties =>
            this.m_properties;

        internal ReadOnlyCollection<KeyValuePair<object, EntityType>> TypeMap =>
            this.m_typeMap;
    }
}

