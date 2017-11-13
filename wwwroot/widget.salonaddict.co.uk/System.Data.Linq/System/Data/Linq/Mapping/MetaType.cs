namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.ObjectModel;
    using System.Reflection;

    public abstract class MetaType
    {
        protected MetaType()
        {
        }

        public abstract MetaDataMember GetDataMember(MemberInfo member);
        public abstract MetaType GetInheritanceType(System.Type type);
        public abstract MetaType GetTypeForInheritanceCode(object code);

        public abstract ReadOnlyCollection<MetaAssociation> Associations { get; }

        public abstract bool CanInstantiate { get; }

        public abstract ReadOnlyCollection<MetaDataMember> DataMembers { get; }

        public abstract MetaDataMember DBGeneratedIdentityMember { get; }

        public abstract ReadOnlyCollection<MetaType> DerivedTypes { get; }

        public abstract MetaDataMember Discriminator { get; }

        public abstract bool HasAnyLoadMethod { get; }

        public abstract bool HasAnyValidateMethod { get; }

        public abstract bool HasInheritance { get; }

        public abstract bool HasInheritanceCode { get; }

        public abstract bool HasUpdateCheck { get; }

        public abstract ReadOnlyCollection<MetaDataMember> IdentityMembers { get; }

        public abstract MetaType InheritanceBase { get; }

        public abstract object InheritanceCode { get; }

        public abstract MetaType InheritanceDefault { get; }

        public abstract MetaType InheritanceRoot { get; }

        public abstract ReadOnlyCollection<MetaType> InheritanceTypes { get; }

        public abstract bool IsEntity { get; }

        public abstract bool IsInheritanceDefault { get; }

        public abstract MetaModel Model { get; }

        public abstract string Name { get; }

        public abstract MethodInfo OnLoadedMethod { get; }

        public abstract MethodInfo OnValidateMethod { get; }

        public abstract ReadOnlyCollection<MetaDataMember> PersistentDataMembers { get; }

        public abstract MetaTable Table { get; }

        public abstract System.Type Type { get; }

        public abstract MetaDataMember VersionMember { get; }
    }
}

