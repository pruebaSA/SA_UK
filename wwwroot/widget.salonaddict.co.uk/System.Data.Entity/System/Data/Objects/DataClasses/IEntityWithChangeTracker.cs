namespace System.Data.Objects.DataClasses
{
    using System;

    public interface IEntityWithChangeTracker
    {
        void SetChangeTracker(IEntityChangeTracker changeTracker);
    }
}

