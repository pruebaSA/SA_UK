namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal sealed class MappedDataMember : MetaDataMember
    {
        private MetaAccessor accDefSource;
        private MetaAccessor accDefValue;
        private MetaAccessor accPrivate;
        private MetaAccessor accPublic;
        private MappedAssociation assoc;
        private System.Data.Linq.Mapping.AutoSync autoSync = System.Data.Linq.Mapping.AutoSync.Never;
        private bool canBeNull = true;
        private string dbType;
        private MetaType declaringType;
        private string expression;
        private bool hasAccessors;
        private bool hasLoadMethod;
        private bool isDBGenerated;
        private bool isDeferred;
        private bool isDiscriminator;
        private bool isNullableType;
        private bool isPrimaryKey;
        private bool isVersion;
        private MethodInfo loadMethod;
        private object locktarget = new object();
        private string mappedName;
        private MemberInfo member;
        private MemberMapping memberMap;
        private int ordinal;
        private MemberInfo storageMember;
        private System.Type type;
        private System.Data.Linq.Mapping.UpdateCheck updateCheck = System.Data.Linq.Mapping.UpdateCheck.Never;

        internal MappedDataMember(MetaType declaringType, MemberInfo mi, MemberMapping map, int ordinal)
        {
            this.declaringType = declaringType;
            this.member = mi;
            this.ordinal = ordinal;
            this.type = TypeSystem.GetMemberType(mi);
            this.isNullableType = TypeSystem.IsNullableType(this.type);
            this.memberMap = map;
            if ((this.memberMap != null) && (this.memberMap.StorageMemberName != null))
            {
                MemberInfo[] member = mi.DeclaringType.GetMember(this.memberMap.StorageMemberName, BindingFlags.NonPublic | BindingFlags.Instance);
                if ((member == null) || (member.Length != 1))
                {
                    throw System.Data.Linq.Mapping.Error.BadStorageProperty(this.memberMap.StorageMemberName, mi.DeclaringType, mi.Name);
                }
                this.storageMember = member[0];
            }
            System.Type clrType = (this.storageMember != null) ? TypeSystem.GetMemberType(this.storageMember) : this.type;
            this.isDeferred = this.IsDeferredType(clrType);
            ColumnMapping mapping = map as ColumnMapping;
            if ((((mapping != null) && mapping.IsDbGenerated) && (mapping.IsPrimaryKey && (mapping.AutoSync != System.Data.Linq.Mapping.AutoSync.Default))) && (mapping.AutoSync != System.Data.Linq.Mapping.AutoSync.OnInsert))
            {
                throw System.Data.Linq.Mapping.Error.IncorrectAutoSyncSpecification(mi.Name);
            }
            if (mapping != null)
            {
                this.isPrimaryKey = mapping.IsPrimaryKey;
                this.isVersion = mapping.IsVersion;
                this.isDBGenerated = (mapping.IsDbGenerated || !string.IsNullOrEmpty(mapping.Expression)) || this.isVersion;
                this.isDiscriminator = mapping.IsDiscriminator;
                this.canBeNull = !mapping.CanBeNull.HasValue ? (this.isNullableType || !this.type.IsValueType) : mapping.CanBeNull.Value;
                this.dbType = mapping.DbType;
                this.expression = mapping.Expression;
                this.updateCheck = mapping.UpdateCheck;
                if (this.IsDbGenerated && this.IsPrimaryKey)
                {
                    this.autoSync = System.Data.Linq.Mapping.AutoSync.OnInsert;
                }
                else if (mapping.AutoSync != System.Data.Linq.Mapping.AutoSync.Default)
                {
                    this.autoSync = mapping.AutoSync;
                }
                else if (this.IsDbGenerated)
                {
                    this.autoSync = System.Data.Linq.Mapping.AutoSync.Always;
                }
            }
            this.mappedName = (this.memberMap.DbName != null) ? this.memberMap.DbName : this.member.Name;
        }

        private static MetaAccessor CreateAccessor(System.Type accessorType, params object[] args) => 
            ((MetaAccessor) Activator.CreateInstance(accessorType, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, args, null));

        private void InitAccessors()
        {
            if (!this.hasAccessors)
            {
                lock (this.locktarget)
                {
                    if (!this.hasAccessors)
                    {
                        if (this.storageMember != null)
                        {
                            this.accPrivate = MakeMemberAccessor(this.member.ReflectedType, this.storageMember, null);
                            if (this.isDeferred)
                            {
                                MakeDeferredAccessors(this.member.ReflectedType, this.accPrivate, out this.accPrivate, out this.accDefValue, out this.accDefSource);
                            }
                            this.accPublic = MakeMemberAccessor(this.member.ReflectedType, this.member, this.accPrivate);
                        }
                        else
                        {
                            this.accPublic = this.accPrivate = MakeMemberAccessor(this.member.ReflectedType, this.member, null);
                            if (this.isDeferred)
                            {
                                MakeDeferredAccessors(this.member.ReflectedType, this.accPrivate, out this.accPrivate, out this.accDefValue, out this.accDefSource);
                            }
                        }
                        this.hasAccessors = true;
                    }
                }
            }
        }

        public override bool IsDeclaredBy(MetaType metaType) => 
            (metaType?.Type == this.member.DeclaringType);

        private bool IsDeferredType(System.Type clrType)
        {
            if ((clrType == null) || (clrType == typeof(object)))
            {
                return false;
            }
            if (!clrType.IsGenericType)
            {
                return false;
            }
            System.Type genericTypeDefinition = clrType.GetGenericTypeDefinition();
            if (((genericTypeDefinition != typeof(Link<>)) && !typeof(EntitySet<>).IsAssignableFrom(genericTypeDefinition)) && !typeof(EntityRef<>).IsAssignableFrom(genericTypeDefinition))
            {
                return this.IsDeferredType(clrType.BaseType);
            }
            return true;
        }

        private static void MakeDeferredAccessors(System.Type declaringType, MetaAccessor accessor, out MetaAccessor accessorValue, out MetaAccessor accessorDeferredValue, out MetaAccessor accessorDeferredSource)
        {
            if (accessor.Type.IsGenericType)
            {
                System.Type genericTypeDefinition = accessor.Type.GetGenericTypeDefinition();
                System.Type type2 = accessor.Type.GetGenericArguments()[0];
                if (genericTypeDefinition == typeof(Link<>))
                {
                    accessorValue = CreateAccessor(typeof(LinkValueAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    accessorDeferredValue = CreateAccessor(typeof(LinkDefValueAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    accessorDeferredSource = CreateAccessor(typeof(LinkDefSourceAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    return;
                }
                if (typeof(EntityRef<>).IsAssignableFrom(genericTypeDefinition))
                {
                    accessorValue = CreateAccessor(typeof(EntityRefValueAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    accessorDeferredValue = CreateAccessor(typeof(EntityRefDefValueAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    accessorDeferredSource = CreateAccessor(typeof(EntityRefDefSourceAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    return;
                }
                if (typeof(EntitySet<>).IsAssignableFrom(genericTypeDefinition))
                {
                    accessorValue = CreateAccessor(typeof(EntitySetValueAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    accessorDeferredValue = CreateAccessor(typeof(EntitySetDefValueAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    accessorDeferredSource = CreateAccessor(typeof(EntitySetDefSourceAccessor<,>).MakeGenericType(new System.Type[] { declaringType, type2 }), new object[] { accessor });
                    return;
                }
            }
            throw System.Data.Linq.Mapping.Error.UnhandledDeferredStorageType(accessor.Type);
        }

        private static MetaAccessor MakeMemberAccessor(System.Type accessorType, MemberInfo mi, MetaAccessor storage)
        {
            FieldInfo fi = mi as FieldInfo;
            if (fi != null)
            {
                return FieldAccessor.Create(accessorType, fi);
            }
            PropertyInfo pi = (PropertyInfo) mi;
            return PropertyAccessor.Create(accessorType, pi, storage);
        }

        public override MetaAssociation Association
        {
            get
            {
                if (this.IsAssociation && (this.assoc == null))
                {
                    lock (this.locktarget)
                    {
                        if (this.assoc == null)
                        {
                            this.assoc = new MappedAssociation(this, (AssociationMapping) this.memberMap);
                        }
                    }
                }
                return this.assoc;
            }
        }

        public override System.Data.Linq.Mapping.AutoSync AutoSync =>
            this.autoSync;

        public override bool CanBeNull =>
            this.canBeNull;

        public override string DbType =>
            this.dbType;

        public override MetaType DeclaringType =>
            this.declaringType;

        public override MetaAccessor DeferredSourceAccessor
        {
            get
            {
                this.InitAccessors();
                return this.accDefSource;
            }
        }

        public override MetaAccessor DeferredValueAccessor
        {
            get
            {
                this.InitAccessors();
                return this.accDefValue;
            }
        }

        public override string Expression =>
            this.expression;

        public override bool IsAssociation =>
            (this.memberMap is AssociationMapping);

        public override bool IsDbGenerated =>
            this.isDBGenerated;

        public override bool IsDeferred =>
            this.isDeferred;

        public override bool IsDiscriminator =>
            this.isDiscriminator;

        public override bool IsPersistent =>
            (this.memberMap != null);

        public override bool IsPrimaryKey =>
            this.isPrimaryKey;

        public override bool IsVersion =>
            this.isVersion;

        public override MethodInfo LoadMethod
        {
            get
            {
                if (!this.hasLoadMethod && this.IsDeferred)
                {
                    this.loadMethod = MethodFinder.FindMethod(((MappedMetaModel) this.declaringType.Model).ContextType, "Load" + this.member.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, new System.Type[] { this.DeclaringType.Type });
                    this.hasLoadMethod = true;
                }
                return this.loadMethod;
            }
        }

        public override string MappedName =>
            this.mappedName;

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
                return this.accPrivate;
            }
        }

        public override MemberInfo StorageMember =>
            this.storageMember;

        public override System.Type Type =>
            this.type;

        public override System.Data.Linq.Mapping.UpdateCheck UpdateCheck =>
            this.updateCheck;
    }
}

