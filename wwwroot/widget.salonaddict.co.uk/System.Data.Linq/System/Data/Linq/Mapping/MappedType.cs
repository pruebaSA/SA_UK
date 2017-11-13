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

    internal class MappedType : MetaType
    {
        private ReadOnlyCollection<MetaAssociation> associations;
        private Dictionary<object, MetaDataMember> dataMemberMap;
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
        private ReadOnlyCollection<MetaDataMember> persistentDataMembers;
        private MetaTable table;
        private System.Type type;
        private TypeMapping typeMapping;
        private MetaDataMember version;

        internal MappedType(MetaModel model, MetaTable table, TypeMapping typeMapping, System.Type type, MetaType inheritanceRoot)
        {
            this.model = model;
            this.table = table;
            this.typeMapping = typeMapping;
            this.type = type;
            this.inheritanceRoot = (inheritanceRoot != null) ? inheritanceRoot : this;
            this.InitDataMembers();
            this.identities = (from m in this.dataMembers
                where m.IsPrimaryKey
                select m).ToList<MetaDataMember>().AsReadOnly();
            this.persistentDataMembers = (from m in this.dataMembers
                where m.IsPersistent
                select m).ToList<MetaDataMember>().AsReadOnly();
        }

        public override MetaDataMember GetDataMember(MemberInfo mi)
        {
            MetaDataMember member;
            if (mi == null)
            {
                throw System.Data.Linq.Mapping.Error.ArgumentNull("mi");
            }
            if (this.dataMemberMap.TryGetValue(InheritanceRules.DistinguishedMemberName(mi), out member))
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
            foreach (MetaType type in this.InheritanceTypes)
            {
                if (type.Type == inheritanceType)
                {
                    return type;
                }
            }
            return null;
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
                Dictionary<object, MetaDataMember> dictionary = new Dictionary<object, MetaDataMember>();
                List<MetaDataMember> list = new List<MetaDataMember>();
                int ordinal = 0;
                BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                Dictionary<string, MemberMapping> dictionary2 = new Dictionary<string, MemberMapping>();
                System.Type baseType = this.type;
                for (TypeMapping mapping = this.typeMapping; mapping != null; mapping = mapping.BaseType)
                {
                    foreach (MemberMapping mapping2 in mapping.Members)
                    {
                        dictionary2[mapping2.MemberName + ":" + baseType.Name] = mapping2;
                    }
                    baseType = baseType.BaseType;
                }
                HashSet<string> set = new HashSet<string>();
                FieldInfo[] infoArray = TypeSystem.GetAllFields(this.type, flags).ToArray<FieldInfo>();
                if (infoArray != null)
                {
                    foreach (FieldInfo info in infoArray)
                    {
                        MemberMapping mapping3;
                        string key = info.Name + ":" + info.DeclaringType.Name;
                        if (dictionary2.TryGetValue(key, out mapping3))
                        {
                            MetaDataMember member;
                            set.Add(key);
                            object obj2 = InheritanceRules.DistinguishedMemberName(info);
                            if (!dictionary.TryGetValue(obj2, out member))
                            {
                                member = new MappedDataMember(this, info, mapping3, ordinal);
                                dictionary.Add(InheritanceRules.DistinguishedMemberName(member.Member), member);
                                list.Add(member);
                                this.InitSpecialMember(member);
                            }
                            this.ValidatePrimaryKeyMember(member);
                            ordinal++;
                        }
                    }
                }
                PropertyInfo[] infoArray2 = TypeSystem.GetAllProperties(this.type, flags).ToArray<PropertyInfo>();
                if (infoArray2 != null)
                {
                    foreach (PropertyInfo info2 in infoArray2)
                    {
                        MemberMapping mapping4;
                        string str2 = info2.Name + ":" + info2.DeclaringType.Name;
                        if (dictionary2.TryGetValue(str2, out mapping4))
                        {
                            MetaDataMember member2;
                            set.Add(str2);
                            object obj3 = InheritanceRules.DistinguishedMemberName(info2);
                            if (!dictionary.TryGetValue(obj3, out member2))
                            {
                                member2 = new MappedDataMember(this, info2, mapping4, ordinal);
                                dictionary.Add(InheritanceRules.DistinguishedMemberName(member2.Member), member2);
                                list.Add(member2);
                                this.InitSpecialMember(member2);
                            }
                            this.ValidatePrimaryKeyMember(member2);
                            ordinal++;
                        }
                    }
                }
                this.dataMembers = list.AsReadOnly();
                this.dataMemberMap = dictionary;
                foreach (string str3 in set)
                {
                    dictionary2.Remove(str3);
                }
                foreach (KeyValuePair<string, MemberMapping> pair in dictionary2)
                {
                    for (System.Type type2 = this.inheritanceRoot.Type.BaseType; type2 != null; type2 = type2.BaseType)
                    {
                        foreach (MemberInfo info3 in type2.GetMembers(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                        {
                            if (string.Compare(info3.Name, pair.Value.MemberName, StringComparison.Ordinal) == 0)
                            {
                                throw System.Data.Linq.Mapping.Error.MappedMemberHadNoCorrespondingMemberInType(pair.Value.MemberName, this.type.Name);
                            }
                        }
                    }
                }
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
                    if (!InheritanceRules.AreSameMember(this.discriminator.Member, mm.Member))
                    {
                        throw System.Data.Linq.Mapping.Error.TwoMembersMarkedAsInheritanceDiscriminator(mm.Member, this.discriminator.Member);
                    }
                }
                else
                {
                    this.discriminator = mm;
                }
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
            (this.InheritanceCode != null);

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

        public override MetaType InheritanceDefault
        {
            get
            {
                if (this.inheritanceRoot == this)
                {
                    throw System.Data.Linq.Mapping.Error.CannotGetInheritanceDefaultFromNonInheritanceClass();
                }
                return this.InheritanceRoot.InheritanceDefault;
            }
        }

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
            this.persistentDataMembers;

        public override MetaTable Table =>
            this.table;

        public override System.Type Type =>
            this.type;

        public override MetaDataMember VersionMember =>
            this.version;
    }
}

