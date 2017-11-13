namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;
    using System.Collections;

    internal class StorageInfoCore
    {
        internal Hashtable elementInfoCores;
        internal IStorage safeIStorage;
        internal string storageName;
        internal Hashtable validEnumerators;

        internal StorageInfoCore(string nameStorage) : this(nameStorage, null)
        {
        }

        internal StorageInfoCore(string nameStorage, IStorage storage)
        {
            this.storageName = nameStorage;
            this.safeIStorage = storage;
            this.validEnumerators = new Hashtable();
            this.elementInfoCores = new Hashtable(ContainerUtilities.StringCaseInsensitiveComparer);
        }
    }
}

