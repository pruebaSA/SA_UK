namespace System.Data.Linq.Mapping
{
    using LinqToSqlShared.Mapping;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq;
    using System.Data.Linq.SqlClient;
    using System.Linq;
    using System.Reflection;

    internal class AttributedMetaType : MetaType
    {
        private ReadOnlyCollection<MetaAssociation> associations;
        private Dictionary<MetaPosition, MetaDataMember> dataMemberMap;
        private ReadOnlyCollection<MetaDataMember> dataMembers;
        private MetaDataMember dbGeneratedIdentity;
        private ReadOnlyCollection<MetaType> derivedTypes;
        private MetaDataMember discriminator;
        private bool hasAnyLoadMethod;
        private bool hasAnyValidateMethod;
        private bool hasMethods;
        private ReadOnlyCollection<MetaDataMember> identities;
        private MetaType inheritanceBase;
        private bool inheritanceBaseSet;
        internal object inheritanceCode;
        private MetaType inheritanceRoot;
        private object locktarget = new object();
        private MetaModel model;
        private MethodInfo onLoadedMethod;
        private MethodInfo onValidateMethod;
        private ReadOnlyCollection<MetaDataMember> persistentMembers;
        private MetaTable table;
        private System.Type type;
        private MetaDataMember version;

        internal AttributedMetaType(MetaModel model, MetaTable table, System.Type type, MetaType inheritanceRoot)
        {
            this.model = model;
            this.table = table;
            this.type = type;
            this.inheritanceRoot = (inheritanceRoot != null) ? inheritanceRoot : this;
            this.InitDataMembers();
            this.identities = (from m in this.dataMembers
                where m.IsPrimaryKey
                select m).ToList<MetaDataMember>().AsReadOnly();
            this.persistentMembers = (from m in this.dataMembers
                where m.IsPersistent
                select m).ToList<MetaDataMember>().AsReadOnly();
        }

        public override MetaDataMember GetDataMember(MemberInfo mi)
        {
            if (mi == null)
            {
                throw System.Data.Linq.Mapping.Error.ArgumentNull("mi");
            }
            MetaDataMember member = null;
            if (this.dataMemberMap.TryGetValue(new MetaPosition(mi), out member))
            {
                return member;
            }
            if (mi.DeclaringType.IsInterface)
            {
                throw System.Data.Linq.Mapping.Error.MappingOfInterfacesMemberIsNotSupported(mi.DeclaringType.Name, mi.Name);
            }
            throw System.Data.Linq.Mapping.Error.UnmappedClassMember(mi.DeclaringType.Name, mi.Name);
        }

        public override MetaType GetInheritanceType(System.Type inheritanceType)
        {
            if (inheritanceType == this.type)
            {
                return this;
            }
            return this.inheritanceRoot.GetInheritanceType(inheritanceType);
        }

        public override MetaType GetTypeForInheritanceCode(object key)
        {
            if (this.InheritanceRoot.Discriminator.Type == typeof(string))
            {
                string strB = (string) key;
                foreach (MetaType type in this.InheritanceRoot.InheritanceTypes)
                {
                    if (string.Compare((string) type.InheritanceCode, strB, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return type;
                    }
                }
            }
            else
            {
                foreach (MetaType type2 in this.InheritanceRoot.InheritanceTypes)
                {
                    if (object.Equals(type2.InheritanceCode, key))
                    {
                        return type2;
                    }
                }
            }
            return null;
        }

        private void InitDataMembers()
        {
            if (this.dataMembers == null)
            {
                this.dataMemberMap = new Dictionary<MetaPosition, MetaDataMember>();
                int ordinal = 0;
                BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                FieldInfo[] infoArray = TypeSystem.GetAllFields(this.type, flags).ToArray<FieldInfo>();
                if (infoArray != null)
                {
                    int index = 0;
                    int length = infoArray.Length;
                    while (index < length)
                    {
                        FieldInfo mi = infoArray[index];
                        MetaDataMember mm = new AttributedMetaDataMember(this, mi, ordinal);
                        this.ValidatePrimaryKeyMember(mm);
                        if (mm.IsPersistent || mi.IsPublic)
                        {
                            this.dataMemberMap.Add(new MetaPosition(mi), mm);
                            ordinal++;
                            if (mm.IsPersistent)
                            {
                                this.InitSpecialMember(mm);
                            }
                        }
                        index++;
                    }
                }
                PropertyInfo[] infoArray2 = TypeSystem.GetAllProperties(this.type, flags).ToArray<PropertyInfo>();
                if (infoArray2 != null)
                {
                    int num4 = 0;
                    int num5 = infoArray2.Length;
                    while (num4 < num5)
                    {
                        PropertyInfo info2 = infoArray2[num4];
                        MetaDataMember member2 = new AttributedMetaDataMember(this, info2, ordinal);
                        this.ValidatePrimaryKeyMember(member2);
                        bool flag = (info2.CanRead && (info2.GetGetMethod(false) != null)) && (!info2.CanWrite || (info2.GetSetMethod(false) != null));
                        if (member2.IsPersistent || flag)
                        {
                            this.dataMemberMap.Add(new MetaPosition(info2), member2);
                            ordinal++;
                            if (member2.IsPersistent)
                            {
                                this.InitSpecialMember(member2);
                            }
                        }
                        num4++;
                    }
                }
                this.dataMembers = new List<MetaDataMember>(this.dataMemberMap.Values).AsReadOnly();
            }
        }

        private void InitMethods()
        {
            if (!this.hasMethods)
            {
                this.onLoadedMethod = MethodFinder.FindMethod(this.Type, "OnLoaded", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, System.Type.EmptyTypes, false);
                this.onValidateMethod = MethodFinder.FindMethod(this.Type, "OnValidate", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, new System.Type[] { typeof(ChangeAction) }, false);
                this.hasAnyLoadMethod = (this.onLoadedMethod != null) || ((this.InheritanceBase != null) && this.InheritanceBase.HasAnyLoadMethod);
                this.hasAnyValidateMethod = (this.onValidateMethod != null) || ((this.InheritanceBase != null) && this.InheritanceBase.HasAnyValidateMethod);
                this.hasMethods = true;
            }
        }

        private void InitSpecialMember(MetaDataMember mm)
        {
            if ((mm.IsDbGenerated && mm.IsPrimaryKey) && string.IsNullOrEmpty(mm.Expression))
            {
                if (this.dbGeneratedIdentity != null)
                {
                    throw System.Data.Linq.Mapping.Error.TwoMembersMarkedAsPrimaryKeyAndDBGenerated(mm.Member, this.dbGeneratedIdentity.Member);
                }
                this.dbGeneratedIdentity = mm;
            }
            if (mm.IsPrimaryKey && !MappingSystem.IsSupportedIdentityType(mm.Type))
            {
                throw System.Data.Linq.Mapping.Error.IdentityClrTypeNotSupported(mm.DeclaringType, mm.Name, mm.Type);
            }
            if (mm.IsVersion)
            {
                if (this.version != null)
                {
                    throw System.Data.Linq.Mapping.Error.TwoMembersMarkedAsRowVersion(mm.Member, this.version.Member);
                }
                this.version = mm;
            }
            if (mm.IsDiscriminator)
            {
                if (this.discriminator != null)
                {
                    throw System.Data.Linq.Mapping.Error.TwoMembersMarkedAsInheritanceDiscriminator(mm.Member, this.discriminator.Member);
                }
                this.discriminator = mm;
            }
        }

        public override string ToString() => 
            this.Name;

        private void ValidatePrimaryKeyMember(MetaDataMember mm)
        {
            if ((mm.IsPrimaryKey && (this.inheritanceRoot != this)) && (mm.Member.DeclaringType == this.type))
            {
                throw System.Data.Linq.Mapping.Error.PrimaryKeyInSubTypeNotSupported(this.type.Name, mm.Name);
            }
        }

        public override ReadOnlyCollection<MetaAssociation> Associations
        {
            get
            {
                if (this.associations == null)
                {
                    lock (this.locktarget)
                    {
                        if (this.associations == null)
                        {
                            this.associations = (from m in this.dataMembers
                                where m.IsAssociation
                                select m.Association).ToList<MetaAssociation>().AsReadOnly();
                        }
                    }
                }
                return this.associations;
            }
        }

        public override bool CanInstantiate
        {
            get
            {
                if (this.type.IsAbstract)
                {
                    return false;
                }
                if (this != this.InheritanceRoot)
                {
                    return this.HasInheritanceCode;
                }
                return true;
            }
        }

        public override ReadOnlyCollection<MetaDataMember> DataMembers =>
            this.dataMembers;

        public override MetaDataMember DBGeneratedIdentityMember =>
            this.dbGeneratedIdentity;

        public override ReadOnlyCollection<MetaType> DerivedTypes
        {
            get
            {
                if (this.derivedTypes == null)
                {
                    lock (this.locktarget)
                    {
                        if (this.derivedTypes == null)
                        {
                            List<MetaType> list = new List<MetaType>();
                            foreach (MetaType type in this.InheritanceTypes)
                            {
                                if (type.Type.BaseType == this.type)
                                {
                                    list.Add(type);
                                }
                            }
                            this.derivedTypes = list.AsReadOnly();
                        }
                    }
                }
                return this.derivedTypes;
            }
        }

        public override MetaDataMember Discriminator =>
            this.discriminator;

        public override bool HasAnyLoadMethod
        {
            get
            {
                this.InitMethods();
                return this.hasAnyLoadMethod;
            }
        }

        public override bool HasAnyValidateMethod
        {
            get
            {
                this.InitMethods();
                return this.hasAnyValidateMethod;
            }
        }

        public override bool HasInheritance =>
            this.inheritanceRoot.HasInheritance;

        public override bool HasInheritanceCode =>
            (this.inheritanceCode != null);

        public override bool HasUpdateCheck
        {
            get
            {
                foreach (MetaDataMember member in this.PersistentDataMembers)
                {
                    if (member.UpdateCheck != UpdateCheck.Never)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public override ReadOnlyCollection<MetaDataMember> IdentityMembers =>
            this.identities;

        public override MetaType InheritanceBase
        {
            get
            {
                if (!this.inheritanceBaseSet && (this.inheritanceBase == null))
                {
                    lock (this.locktarget)
                    {
                        if (this.inheritanceBase == null)
                        {
                            this.inheritanceBase = InheritanceBaseFinder.FindBase(this);
                            this.inheritanceBaseSet = true;
                        }
                    }
                }
                return this.inheritanceBase;
            }
        }

        public override object InheritanceCode =>
            this.inheritanceCode;

        public override MetaType InheritanceDefault =>
            this.InheritanceRoot.InheritanceDefault;

        public override MetaType InheritanceRoot =>
            this.inheritanceRoot;

        public override ReadOnlyCollection<MetaType> InheritanceTypes =>
            this.inheritanceRoot.InheritanceTypes;

        public override bool IsEntity =>
            ((this.table != null) && (this.table.RowType.IdentityMembers.Count > 0));

        public override bool IsInheritanceDefault =>
            (this.InheritanceDefault == this);

        public override MetaModel Model =>
            this.model;

        public override string Name =>
            this.type.Name;

        public override MethodInfo OnLoadedMethod
        {
            get
            {
                this.InitMethods();
                return this.onLoadedMethod;
            }
        }

        public override MethodInfo OnValidateMethod
        {
            get
            {
                this.InitMethods();
                return this.onValidateMethod;
            }
        }

        public override ReadOnlyCollection<MetaDataMember> PersistentDataMembers =>
            this.persistentMembers;

        public override MetaTable Table =>
            this.table;

        public override System.Type Type =>
            this.type;

        public override MetaDataMember VersionMember =>
            this.version;
    }
}

