namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Linq;

    internal abstract class StorageSetMapping
    {
        private StorageEntityContainerMapping m_entityContainerMapping;
        private EntitySetBase m_extent;
        private string m_queryView;
        private int m_startLineNumber;
        private int m_startLinePosition;
        private List<StorageTypeMapping> m_typeMappings;
        private Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, string> m_typeSpecificQueryViews = new Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, string>(Pair<EntitySetBase, Pair<EntityTypeBase, bool>>.PairComparer.Instance);

        internal StorageSetMapping(EntitySetBase extent, StorageEntityContainerMapping entityContainerMapping)
        {
            this.m_entityContainerMapping = entityContainerMapping;
            this.m_extent = extent;
            this.m_typeMappings = new List<StorageTypeMapping>();
        }

        internal void AddTypeMapping(StorageTypeMapping typeMapping)
        {
            this.m_typeMappings.Add(typeMapping);
        }

        internal void AddTypeSpecificQueryView(Pair<EntitySetBase, Pair<EntityTypeBase, bool>> key, string viewString)
        {
            this.m_typeSpecificQueryViews.Add(key, viewString);
        }

        internal bool ContainsTypeSpecificQueryView(Pair<EntitySetBase, Pair<EntityTypeBase, bool>> key) => 
            this.m_typeSpecificQueryViews.ContainsKey(key);

        internal string GetTypeSpecificQueryView(Pair<EntitySetBase, Pair<EntityTypeBase, bool>> key) => 
            this.m_typeSpecificQueryViews[key];

        internal ReadOnlyCollection<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>> GetTypeSpecificQVKeys() => 
            new ReadOnlyCollection<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>>(this.m_typeSpecificQueryViews.Keys.ToList<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>>());

        internal abstract void Print(int index);

        internal StorageEntityContainerMapping EntityContainerMapping =>
            this.m_entityContainerMapping;

        internal virtual bool HasNoContent
        {
            get
            {
                if (this.QueryView != null)
                {
                    return false;
                }
                foreach (StorageTypeMapping mapping in this.TypeMappings)
                {
                    foreach (StorageMappingFragment fragment in mapping.MappingFragments)
                    {
                        using (IEnumerator<StoragePropertyMapping> enumerator3 = fragment.AllProperties.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                StoragePropertyMapping current = enumerator3.Current;
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
        }

        internal string QueryView
        {
            get => 
                this.m_queryView;
            set
            {
                this.m_queryView = value;
            }
        }

        internal EntitySetBase Set =>
            this.m_extent;

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

        internal ReadOnlyCollection<StorageTypeMapping> TypeMappings =>
            this.m_typeMappings.AsReadOnly();
    }
}

