namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal abstract class StorageTypeMapping
    {
        private List<StorageMappingFragment> m_fragments = new List<StorageMappingFragment>();
        private StorageSetMapping m_setMapping;

        internal StorageTypeMapping(StorageSetMapping setMapping)
        {
            this.m_setMapping = setMapping;
        }

        internal void AddFragment(StorageMappingFragment fragment)
        {
            this.m_fragments.Add(fragment);
        }

        internal abstract void Print(int index);

        internal abstract ReadOnlyCollection<EdmType> IsOfTypes { get; }

        internal ReadOnlyCollection<StorageMappingFragment> MappingFragments =>
            this.m_fragments.AsReadOnly();

        internal StorageSetMapping SetMapping =>
            this.m_setMapping;

        internal abstract ReadOnlyCollection<EdmType> Types { get; }
    }
}

