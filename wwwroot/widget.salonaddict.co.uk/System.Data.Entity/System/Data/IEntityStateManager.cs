namespace System.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    internal interface IEntityStateManager
    {
        IEnumerable<IEntityStateEntry> FindRelationshipsByKey(EntityKey key);
        IEnumerable<IEntityStateEntry> GetEntityStateEntries(EntityState state);
        IEntityStateEntry GetEntityStateEntry(EntityKey key);
        bool TryGetEntityStateEntry(EntityKey key, out IEntityStateEntry stateEntry);
    }
}

