namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class EntityType : EntityTypeBase
    {
        private RowType _keyRow;
        private Dictionary<EdmMember, string> _memberSql;
        private object _memberSqlLock;
        private ReadOnlyMetadataCollection<EdmProperty> _properties;
        private RefType _referenceType;

        internal EntityType(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
            this._memberSqlLock = new object();
        }

        internal EntityType(string name, string namespaceName, DataSpace dataSpace, IEnumerable<string> keyMemberNames, IEnumerable<EdmMember> members) : base(name, namespaceName, dataSpace)
        {
            this._memberSqlLock = new object();
            if (members != null)
            {
                EntityTypeBase.CheckAndAddMembers(members, this);
            }
            if (keyMemberNames != null)
            {
                base.CheckAndAddKeyMembers(keyMemberNames);
            }
        }

        internal RowType GetKeyRowType(MetadataWorkspace metadataWorkspace)
        {
            if (this._keyRow == null)
            {
                List<EdmProperty> properties = new List<EdmProperty>(base.KeyMembers.Count);
                foreach (EdmMember member in base.KeyMembers)
                {
                    properties.Add(new EdmProperty(member.Name, Helper.GetModelTypeUsage(member)));
                }
                Interlocked.CompareExchange<RowType>(ref this._keyRow, new RowType(properties), null);
            }
            return this._keyRow;
        }

        public RefType GetReferenceType()
        {
            if (this._referenceType == null)
            {
                Interlocked.CompareExchange<RefType>(ref this._referenceType, new RefType(this), null);
            }
            return this._referenceType;
        }

        internal void SetMemberSql(EdmMember member, string sql)
        {
            lock (this._memberSqlLock)
            {
                if (this._memberSql == null)
                {
                    this._memberSql = new Dictionary<EdmMember, string>();
                }
                this._memberSql[member] = sql;
            }
        }

        internal bool TryGetMemberSql(EdmMember member, out string sql)
        {
            sql = null;
            return ((this._memberSql != null) && this._memberSql.TryGetValue(member, out sql));
        }

        internal override void ValidateMemberForAdd(EdmMember member)
        {
            if (!Helper.IsEdmProperty(member) && !Helper.IsNavigationProperty(member))
            {
                throw EntityUtil.EntityTypeInvalidMembers();
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EntityType;

        public ReadOnlyMetadataCollection<NavigationProperty> NavigationProperties =>
            new FilteredReadOnlyMetadataCollection<NavigationProperty, EdmMember>(base.Members, new Predicate<EdmMember>(Helper.IsNavigationProperty));

        public ReadOnlyMetadataCollection<EdmProperty> Properties
        {
            get
            {
                if (this._properties == null)
                {
                    Interlocked.CompareExchange<ReadOnlyMetadataCollection<EdmProperty>>(ref this._properties, new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(base.Members, new Predicate<EdmMember>(Helper.IsEdmProperty)), null);
                }
                return this._properties;
            }
        }
    }
}

