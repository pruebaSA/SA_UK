namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.ObjectModel;

    public abstract class MetaAssociation
    {
        protected MetaAssociation()
        {
        }

        public abstract bool DeleteOnNull { get; }

        public abstract string DeleteRule { get; }

        public abstract bool IsForeignKey { get; }

        public abstract bool IsMany { get; }

        public abstract bool IsNullable { get; }

        public abstract bool IsUnique { get; }

        public abstract ReadOnlyCollection<MetaDataMember> OtherKey { get; }

        public abstract bool OtherKeyIsPrimaryKey { get; }

        public abstract MetaDataMember OtherMember { get; }

        public abstract MetaType OtherType { get; }

        public abstract ReadOnlyCollection<MetaDataMember> ThisKey { get; }

        public abstract bool ThisKeyIsPrimaryKey { get; }

        public abstract MetaDataMember ThisMember { get; }
    }
}

