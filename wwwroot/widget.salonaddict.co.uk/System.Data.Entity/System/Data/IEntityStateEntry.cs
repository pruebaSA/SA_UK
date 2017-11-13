namespace System.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;

    internal interface IEntityStateEntry
    {
        void AcceptChanges();
        void Delete();
        IEnumerable<string> GetModifiedProperties();
        void SetModified();
        void SetModifiedProperty(string propertyName);

        CurrentValueRecord CurrentValues { get; }

        System.Data.EntityKey EntityKey { get; }

        EntitySetBase EntitySet { get; }

        bool IsKeyEntry { get; }

        bool IsRelationship { get; }

        DbDataRecord OriginalValues { get; }

        EntityState State { get; }

        IEntityStateManager StateManager { get; }
    }
}

