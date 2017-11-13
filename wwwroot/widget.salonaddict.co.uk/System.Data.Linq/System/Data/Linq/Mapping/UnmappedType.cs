namespace System.Data.Linq.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Linq.SqlClient;
    using System.Linq;
    using System.Reflection;

    internal sealed class UnmappedType : MetaType
    {
        private static ReadOnlyCollection<MetaAssociation> _emptyAssociations = new List<MetaAssociation>().AsReadOnly();
        private static ReadOnlyCollection<MetaDataMember> _emptyDataMembers = new List<MetaDataMember>().AsReadOnly();
        private static ReadOnlyCollection<MetaType> _emptyTypes = new List<MetaType>().AsReadOnly();
        private Dictionary<object, MetaDataMember> dataMemberMap;
        private ReadOnlyCollection<MetaDataMember> dataMembers;
        private ReadOnlyCollection<MetaType> inheritanceTypes;
        private object locktarget = new object();
        private MetaModel model;
        private System.Type type;

        internal UnmappedType(MetaModel model, System.Type type)
        {
            this.model = model;
            this.type = type;
        }

        public override MetaDataMember GetDataMember(MemberInfo mi)
        {
            MetaDataMember member2;
            if (mi == null)
            {
                throw System.Data.Linq.Mapping.Error.ArgumentNull("mi");
            }
            this.InitDataMembers();
            if (this.dataMemberMap == null)
            {
                lock (this.locktarget)
                {
                    if (this.dataMemberMap == null)
                    {
                        Dictionary<object, MetaDataMember> dictionary = new Dictionary<object, MetaDataMember>();
                        foreach (MetaDataMember member in this.dataMembers)
                        {
                            dictionary.Add(InheritanceRules.DistinguishedMemberName(member.Member), member);
                        }
                        this.dataMemberMap = dictionary;
                    }
                }
            }
            object key = InheritanceRules.DistinguishedMemberName(mi);
            this.dataMemberMap.TryGetValue(key, out member2);
            return member2;
        }

        public override MetaType GetInheritanceType(System.Type inheritanceType)
        {
            if (inheritanceType == this.type)
            {
                return this;
            }
            return null;
        }

        public override MetaType GetTypeForInheritanceCode(object key) => 
            null;

        private void InitDataMembers()
        {
            if (this.dataMembers == null)
            {
                lock (this.locktarget)
                {
                    if (this.dataMembers == null)
                    {
                        List<MetaDataMember> list = new List<MetaDataMember>();
                        int ordinal = 0;
                        BindingFlags bindingAttr = BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                        foreach (FieldInfo info in this.type.GetFields(bindingAttr))
                        {
                            MetaDataMember item = new UnmappedDataMember(this, info, ordinal);
                            list.Add(item);
                            ordinal++;
                        }
                        foreach (PropertyInfo info2 in this.type.GetProperties(bindingAttr))
                        {
                            MetaDataMember member2 = new UnmappedDataMember(this, info2, ordinal);
                            list.Add(member2);
                            ordinal++;
                        }
                        this.dataMembers = list.AsReadOnly();
                    }
                }
            }
        }

        public override string ToString() => 
            this.Name;

        public override ReadOnlyCollection<MetaAssociation> Associations =>
            _emptyAssociations;

        public override bool CanInstantiate =>
            !this.type.IsAbstract;

        public override ReadOnlyCollection<MetaDataMember> DataMembers
        {
            get
            {
                this.InitDataMembers();
                return this.dataMembers;
            }
        }

        public override MetaDataMember DBGeneratedIdentityMember =>
            null;

        public override ReadOnlyCollection<MetaType> DerivedTypes =>
            _emptyTypes;

        public override MetaDataMember Discriminator =>
            null;

        public override bool HasAnyLoadMethod =>
            false;

        public override bool HasAnyValidateMethod =>
            false;

        public override bool HasInheritance =>
            false;

        public override bool HasInheritanceCode =>
            false;

        public override bool HasUpdateCheck =>
            false;

        public override ReadOnlyCollection<MetaDataMember> IdentityMembers
        {
            get
            {
                this.InitDataMembers();
                return this.dataMembers;
            }
        }

        public override MetaType InheritanceBase =>
            null;

        public override object InheritanceCode =>
            null;

        public override MetaType InheritanceDefault =>
            null;

        public override MetaType InheritanceRoot =>
            this;

        public override ReadOnlyCollection<MetaType> InheritanceTypes
        {
            get
            {
                if (this.inheritanceTypes == null)
                {
                    lock (this.locktarget)
                    {
                        if (this.inheritanceTypes == null)
                        {
                            this.inheritanceTypes = new MetaType[] { this }.ToList<MetaType>().AsReadOnly();
                        }
                    }
                }
                return this.inheritanceTypes;
            }
        }

        public override bool IsEntity =>
            false;

        public override bool IsInheritanceDefault =>
            false;

        public override MetaModel Model =>
            this.model;

        public override string Name =>
            this.type.Name;

        public override MethodInfo OnLoadedMethod =>
            null;

        public override MethodInfo OnValidateMethod =>
            null;

        public override ReadOnlyCollection<MetaDataMember> PersistentDataMembers =>
            _emptyDataMembers;

        public override MetaTable Table =>
            null;

        public override System.Type Type =>
            this.type;

        public override MetaDataMember VersionMember =>
            null;
    }
}

