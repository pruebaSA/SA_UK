namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Data;

    public interface IEntityChangeTracker
    {
        void EntityComplexMemberChanged(string entityMemberName, object complexObject, string complexObjectMemberName);
        void EntityComplexMemberChanging(string entityMemberName, object complexObject, string complexObjectMemberName);
        void EntityMemberChanged(string entityMemberName);
        void EntityMemberChanging(string entityMemberName);

        System.Data.EntityState EntityState { get; }
    }
}

