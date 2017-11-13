namespace System.Data.Linq.Mapping
{
    using System;
    using System.Data.Linq.SqlClient;
    using System.Reflection;

    internal sealed class UnmappedDataMember : MetaDataMember
    {
        private MetaAccessor accPublic;
        private MetaType declaringType;
        private object lockTarget = new object();
        private MemberInfo member;
        private int ordinal;
        private System.Type type;

        internal UnmappedDataMember(MetaType declaringType, MemberInfo mi, int ordinal)
        {
            this.declaringType = declaringType;
            this.member = mi;
            this.ordinal = ordinal;
            this.type = TypeSystem.GetMemberType(mi);
        }

        private void InitAccessors()
        {
            if (this.accPublic == null)
            {
                lock (this.lockTarget)
                {
                    if (this.accPublic == null)
                    {
                        this.accPublic = MakeMemberAccessor(this.member.ReflectedType, this.member);
                    }
                }
            }
        }

        public override bool IsDeclaredBy(MetaType metaType) => 
            (metaType?.Type == this.member.DeclaringType);

        private static MetaAccessor MakeMemberAccessor(System.Type accessorType, MemberInfo mi)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null)
            {
                return FieldAccessor.Create(accessorType, fi);
            }
            PropertyInfo pi = (PropertyInfo) mi;
            return PropertyAccessor.Create(accessorType, pi, null);
        }

        public override MetaAssociation Association =>
            null;

        public override System.Data.Linq.Mapping.AutoSync AutoSync =>
            System.Data.Linq.Mapping.AutoSync.Never;

        public override bool CanBeNull
        {
            get
            {
                if (this.type.IsValueType)
                {
                    return TypeSystem.IsNullableType(this.type);
                }
                return true;
            }
        }

        public override string DbType =>
            null;

        public override MetaType DeclaringType =>
            this.declaringType;

        public override MetaAccessor DeferredSourceAccessor =>
            null;

        public override MetaAccessor DeferredValueAccessor =>
            null;

        public override string Expression =>
            null;

        public override bool IsAssociation =>
            false;

        public override bool IsDbGenerated =>
            false;

        public override bool IsDeferred =>
            false;

        public override bool IsDiscriminator =>
            false;

        public override bool IsPersistent =>
            false;

        public override bool IsPrimaryKey =>
            false;

        public override bool IsVersion =>
            false;

        public override MethodInfo LoadMethod =>
            null;

        public override string MappedName =>
            this.member.Name;

        public override MemberInfo Member =>
            this.member;

        public override MetaAccessor MemberAccessor
        {
            get
            {
                this.InitAccessors();
                return this.accPublic;
            }
        }

        public override string Name =>
            this.member.Name;

        public override int Ordinal =>
            this.ordinal;

        public override MetaAccessor StorageAccessor
        {
            get
            {
                this.InitAccessors();
                return this.accPublic;
            }
        }

        public override MemberInfo StorageMember =>
            this.member;

        public override System.Type Type =>
            this.type;

        public override System.Data.Linq.Mapping.UpdateCheck UpdateCheck =>
            System.Data.Linq.Mapping.UpdateCheck.Never;
    }
}

