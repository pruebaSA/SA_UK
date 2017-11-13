namespace System.Data.Linq.Mapping
{
    using System;
    using System.Reflection;

    public abstract class MetaDataMember
    {
        protected MetaDataMember()
        {
        }

        public abstract bool IsDeclaredBy(MetaType type);

        public abstract MetaAssociation Association { get; }

        public abstract System.Data.Linq.Mapping.AutoSync AutoSync { get; }

        public abstract bool CanBeNull { get; }

        public abstract string DbType { get; }

        public abstract MetaType DeclaringType { get; }

        public abstract MetaAccessor DeferredSourceAccessor { get; }

        public abstract MetaAccessor DeferredValueAccessor { get; }

        public abstract string Expression { get; }

        public abstract bool IsAssociation { get; }

        public abstract bool IsDbGenerated { get; }

        public abstract bool IsDeferred { get; }

        public abstract bool IsDiscriminator { get; }

        public abstract bool IsPersistent { get; }

        public abstract bool IsPrimaryKey { get; }

        public abstract bool IsVersion { get; }

        public abstract MethodInfo LoadMethod { get; }

        public abstract string MappedName { get; }

        public abstract MemberInfo Member { get; }

        public abstract MetaAccessor MemberAccessor { get; }

        public abstract string Name { get; }

        public abstract int Ordinal { get; }

        public abstract MetaAccessor StorageAccessor { get; }

        public abstract MemberInfo StorageMember { get; }

        public abstract System.Type Type { get; }

        public abstract System.Data.Linq.Mapping.UpdateCheck UpdateCheck { get; }
    }
}

