namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct EntitySetQualifiedType : IEqualityComparer<EntitySetQualifiedType>
    {
        internal static readonly IEqualityComparer<EntitySetQualifiedType> EqualityComparer;
        internal readonly Type ClrType;
        internal readonly System.Data.Metadata.Edm.EntitySet EntitySet;
        internal EntitySetQualifiedType(Type type, System.Data.Metadata.Edm.EntitySet set)
        {
            this.ClrType = type;
            this.EntitySet = set;
        }

        public bool Equals(EntitySetQualifiedType x, EntitySetQualifiedType y) => 
            (object.ReferenceEquals(x.ClrType, y.ClrType) && object.ReferenceEquals(x.EntitySet, y.EntitySet));

        public int GetHashCode(EntitySetQualifiedType obj) => 
            ((obj.ClrType.GetHashCode() + obj.EntitySet.Name.GetHashCode()) + obj.EntitySet.EntityContainer.Name.GetHashCode());

        static EntitySetQualifiedType()
        {
            EqualityComparer = new EntitySetQualifiedType();
        }
    }
}

