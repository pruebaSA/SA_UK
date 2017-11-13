namespace System.Data.Mapping
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    public abstract class MappingItemCollection : ItemCollection
    {
        internal MappingItemCollection(DataSpace dataSpace) : base(dataSpace)
        {
        }

        internal virtual Map GetMap(GlobalItem item)
        {
            throw Error.NotSupported();
        }

        internal virtual Map GetMap(string identity, DataSpace typeSpace)
        {
            throw Error.NotSupported();
        }

        internal virtual Map GetMap(string identity, DataSpace typeSpace, bool ignoreCase)
        {
            throw Error.NotSupported();
        }

        internal virtual bool TryGetMap(GlobalItem item, out Map map)
        {
            throw Error.NotSupported();
        }

        internal virtual bool TryGetMap(string identity, DataSpace typeSpace, out Map map)
        {
            throw Error.NotSupported();
        }

        internal virtual bool TryGetMap(string identity, DataSpace typeSpace, bool ignoreCase, out Map map)
        {
            throw Error.NotSupported();
        }
    }
}

