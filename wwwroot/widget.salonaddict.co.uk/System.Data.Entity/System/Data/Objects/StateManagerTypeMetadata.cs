namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;

    internal sealed class StateManagerTypeMetadata
    {
        private readonly Dictionary<string, int> _cLayerNameToOrdinal;
        private readonly StateManagerMemberMetadata[] _members;
        private readonly Dictionary<string, int> _objectNameToOrdinal;
        private readonly ObjectTypeMapping _ocObjectMap;
        private readonly System.Data.Common.DataRecordInfo _recordInfo;
        private readonly TypeUsage _typeUsage;

        internal StateManagerTypeMetadata(EdmType edmType, ObjectTypeMapping mapping)
        {
            this._typeUsage = TypeUsage.Create(edmType);
            this._recordInfo = new System.Data.Common.DataRecordInfo(this._typeUsage);
            this._ocObjectMap = mapping;
            ReadOnlyMetadataCollection<EdmProperty> properties = TypeHelpers.GetProperties(edmType);
            this._members = new StateManagerMemberMetadata[properties.Count];
            this._objectNameToOrdinal = new Dictionary<string, int>(properties.Count);
            this._cLayerNameToOrdinal = new Dictionary<string, int>(properties.Count);
            ReadOnlyMetadataCollection<EdmMember> keyMembers = null;
            if (Helper.IsEntityType(edmType))
            {
                keyMembers = ((EntityType) edmType).KeyMembers;
            }
            for (int i = 0; i < this._members.Length; i++)
            {
                EdmProperty memberMetadata = properties[i];
                ObjectPropertyMapping memberMap = null;
                if (mapping != null)
                {
                    memberMap = mapping.GetPropertyMap(memberMetadata.Name);
                    if (memberMap != null)
                    {
                        this._objectNameToOrdinal.Add(memberMap.ClrProperty.Name, i);
                    }
                }
                this._cLayerNameToOrdinal.Add(memberMetadata.Name, i);
                this._members[i] = new StateManagerMemberMetadata(memberMap, memberMetadata, (keyMembers != null) && keyMembers.Contains(memberMetadata));
            }
        }

        internal string CLayerMemberName(int ordinal) => 
            this.Member(ordinal).CLayerName;

        internal Type GetFieldType(int ordinal) => 
            this.Member(ordinal).ClrType;

        internal int GetOrdinalforCLayerMemberName(string name)
        {
            int num;
            if (!string.IsNullOrEmpty(name) && this._cLayerNameToOrdinal.TryGetValue(name, out num))
            {
                return num;
            }
            return -1;
        }

        internal int GetOrdinalforOLayerMemberName(string name)
        {
            int num;
            if (!string.IsNullOrEmpty(name) && this._objectNameToOrdinal.TryGetValue(name, out num))
            {
                return num;
            }
            return -1;
        }

        internal bool IsMemberPartofShadowState(int ordinal) => 
            (null == this.Member(ordinal).ClrMetadata);

        internal StateManagerMemberMetadata Member(int ordinal)
        {
            if (ordinal >= this._members.Length)
            {
                throw EntityUtil.ArgumentOutOfRange("ordinal");
            }
            return this._members[ordinal];
        }

        internal TypeUsage CdmMetadata =>
            this._typeUsage;

        internal System.Data.Common.DataRecordInfo DataRecordInfo =>
            this._recordInfo;

        internal int FieldCount =>
            this._members.Length;
    }
}

