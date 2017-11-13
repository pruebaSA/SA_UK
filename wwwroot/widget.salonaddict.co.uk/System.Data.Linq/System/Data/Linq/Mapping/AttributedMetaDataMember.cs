namespace System.Data.Linq.Mapping
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal sealed class AttributedMetaDataMember : MetaDataMember
    {
        private MetaAccessor accDefSource;
        private MetaAccessor accDefValue;
        private MetaAccessor accPrivate;
        private MetaAccessor accPublic;
        private AttributedMetaAssociation assoc;
        private DataAttribute attr;
        private AssociationAttribute attrAssoc;
        private ColumnAttribute attrColumn;
        private System.Type declaringType;
        private bool hasAccessors;
        private bool hasLoadMethod;
        private bool isDeferred;
        private bool isNullableType;
        private MethodInfo loadMethod;
        private object locktarget = new object();
        private MemberInfo member;
        private AttributedMetaType metaType;
        private int ordinal;
        private MemberInfo storageMember;
        private System.Type type;

        internal AttributedMetaDataMember(AttributedMetaType metaType, MemberInfo mi, int ordinal)
        {
            this.declaringType = mi.DeclaringType;
            this.metaType = metaType;
            this.member = mi;
            this.ordinal = ordinal;
            this.type = TypeSystem.GetMemberType(mi);
            this.isNullableType = TypeSystem.IsNullableType(this.type);
            this.attrColumn = (ColumnAttribute) Attribute.GetCustomAttribute(mi, typeof(ColumnAttribute));
            this.attrAssoc = (AssociationAttribute) Attribute.GetCustomAttribute(mi, typeof(AssociationAttribute));
            this.attr = (this.attrColumn != null) ? ((DataAttribute) this.attrColumn) : ((DataAttribute) this.attrAssoc);
            if ((this.attr != null) && (this.attr.Storage != null))
            {
                MemberInfo[] member = mi.DeclaringType.GetMember(this.attr.Storage, BindingFlags.NonPublic | BindingFlags.Instance);
                if ((member == null) || (member.Length != 1))
                {
                    throw System.Data.Linq.Mapping.Error.BadStorageProperty(this.attr.Storage, mi.DeclaringType, mi.Name);
                }
                this.storageMember = member[0];
            }
            System.Type entityType = (this.storageMember != null) ? TypeSystem.GetMemberType(this.storageMember) : this.type;
            this.isDeferred = this.IsDeferredType(entityType);
            if ((((this.attrColumn != null) && this.attrColumn.IsDbGenerated) && (this.attrColumn.IsPrimaryKey && (this.attrColumn.AutoSync != System.Data.Linq.Mapping.AutoSync.Default))) && (this.attrColumn.AutoSync != System.Data.Linq.Mapping.AutoSync.OnInsert))
            {
                throw System.Data.Linq.Mapping.Error.IncorrectAutoSyncSpecification(mi.Name);
            }
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

        public override bool IsDeclaredBy(MetaType declaringMetaType) => 
            (declaringMetaType?.Type == this.declaringType);

        private bool IsDeferredType(System.Type entityType)
        {
            if ((entityType == null) || (entityType == typeof(object)))
            {
                return false;
            }
            if (!entityType.IsGenericType)
            {
                return false;
            }
            System.Type genericTypeDefinition = entityType.GetGenericTypeDefinition();
            if (((genericTypeDefinition != typeof(Link<>)) && !typeof(EntitySet<>).IsAssignableFrom(genericTypeDefinition)) && !typeof(EntityRef<>).IsAssignableFrom(genericTypeDefinition))
            {
                return this.IsDeferredType(entityType.BaseType);
            }
            return true;
        }

        private static void MakeDeferredAccessors(System.Type objectDeclaringType, MetaAccessor accessor, out MetaAccessor accessorValue, out MetaAccessor accessorDeferredValue, out MetaAccessor accessorDeferredSource)
        {
            if (accessor.Type.IsGenericType)
            {
                System.Type genericTypeDefinition = accessor.Type.GetGenericTypeDefinition();
                System.Type type2 = accessor.Type.GetGenericArguments()[0];
                if (genericTypeDefinition == typeof(Link<>))
                {
                    accessorValue = CreateAccessor(typeof(LinkValueAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    accessorDeferredValue = CreateAccessor(typeof(LinkDefValueAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    accessorDeferredSource = CreateAccessor(typeof(LinkDefSourceAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    return;
                }
                if (typeof(EntityRef<>).IsAssignableFrom(genericTypeDefinition))
                {
                    accessorValue = CreateAccessor(typeof(EntityRefValueAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    accessorDeferredValue = CreateAccessor(typeof(EntityRefDefValueAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    accessorDeferredSource = CreateAccessor(typeof(EntityRefDefSourceAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    return;
                }
                if (typeof(EntitySet<>).IsAssignableFrom(genericTypeDefinition))
                {
                    accessorValue = CreateAccessor(typeof(EntitySetValueAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    accessorDeferredValue = CreateAccessor(typeof(EntitySetDefValueAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
                    accessorDeferredSource = CreateAccessor(typeof(EntitySetDefSourceAccessor<,>).MakeGenericType(new System.Type[] { objectDeclaringType, type2 }), new object[] { accessor });
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

        public override string ToString() => 
            (this.DeclaringType.ToString() + ":" + this.Member.ToString());

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
                            this.assoc = new AttributedMetaAssociation(this, this.attrAssoc);
                        }
                    }
                }
                return this.assoc;
            }
        }

        public override System.Data.Linq.Mapping.AutoSync AutoSync
        {
            get
            {
                if (this.attrColumn != null)
                {
                    if (this.IsDbGenerated && this.IsPrimaryKey)
                    {
                        return System.Data.Linq.Mapping.AutoSync.OnInsert;
                    }
                    if (this.attrColumn.AutoSync != System.Data.Linq.Mapping.AutoSync.Default)
                    {
                        return this.attrColumn.AutoSync;
                    }
                    if (this.IsDbGenerated)
                    {
                        return System.Data.Linq.Mapping.AutoSync.Always;
                    }
                }
                return System.Data.Linq.Mapping.AutoSync.Never;
            }
        }

        public override bool CanBeNull
        {
            get
            {
                if (this.attrColumn != null)
                {
                    if (this.attrColumn.CanBeNullSet)
                    {
                        return this.attrColumn.CanBeNull;
                    }
                    if (!this.isNullableType)
                    {
                        return !this.type.IsValueType;
                    }
                }
                return true;
            }
        }

        public override string DbType
        {
            get
            {
                if (this.attrColumn != null)
                {
                    return this.attrColumn.DbType;
                }
                return null;
            }
        }

        public override MetaType DeclaringType =>
            this.metaType;

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

        public override string Expression
        {
            get
            {
                if (this.attrColumn != null)
                {
                    return this.attrColumn.Expression;
                }
                return null;
            }
        }

        public override bool IsAssociation =>
            (this.attrAssoc != null);

        public override bool IsDbGenerated =>
            (((this.attrColumn != null) && (this.attrColumn.IsDbGenerated || !string.IsNullOrEmpty(this.attrColumn.Expression))) || this.IsVersion);

        public override bool IsDeferred =>
            this.isDeferred;

        public override bool IsDiscriminator =>
            ((this.attrColumn != null) && this.attrColumn.IsDiscriminator);

        public override bool IsPersistent
        {
            get
            {
                if (this.attrColumn == null)
                {
                    return (this.attrAssoc != null);
                }
                return true;
            }
        }

        public override bool IsPrimaryKey =>
            ((this.attrColumn != null) && this.attrColumn.IsPrimaryKey);

        public override bool IsVersion =>
            ((this.attrColumn != null) && this.attrColumn.IsVersion);

        public override MethodInfo LoadMethod
        {
            get
            {
                if (!this.hasLoadMethod && (this.IsDeferred || this.IsAssociation))
                {
                    this.loadMethod = MethodFinder.FindMethod(((AttributedMetaModel) this.metaType.Model).ContextType, "Load" + this.member.Name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, new System.Type[] { this.DeclaringType.Type });
                    this.hasLoadMethod = true;
                }
                return this.loadMethod;
            }
        }

        public override string MappedName
        {
            get
            {
                if ((this.attrColumn != null) && (this.attrColumn.Name != null))
                {
                    return this.attrColumn.Name;
                }
                if ((this.attrAssoc != null) && (this.attrAssoc.Name != null))
                {
                    return this.attrAssoc.Name;
                }
                return this.member.Name;
            }
        }

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

        public override System.Data.Linq.Mapping.UpdateCheck UpdateCheck
        {
            get
            {
                if (this.attrColumn != null)
                {
                    return this.attrColumn.UpdateCheck;
                }
                return System.Data.Linq.Mapping.UpdateCheck.Never;
            }
        }
    }
}

