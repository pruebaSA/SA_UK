namespace System.Data.Objects.DataClasses
{
    using System;
    using System.Collections;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;

    public interface IRelatedEnd
    {
        void Add(IEntityWithRelationships entity);
        void Attach(IEntityWithRelationships entity);
        IEnumerable CreateSourceQuery();
        IEnumerator GetEnumerator();
        void Load();
        void Load(MergeOption mergeOption);
        bool Remove(IEntityWithRelationships entity);

        bool IsLoaded { get; }

        string RelationshipName { get; }

        System.Data.Metadata.Edm.RelationshipSet RelationshipSet { get; }

        string SourceRoleName { get; }

        string TargetRoleName { get; }
    }
}

