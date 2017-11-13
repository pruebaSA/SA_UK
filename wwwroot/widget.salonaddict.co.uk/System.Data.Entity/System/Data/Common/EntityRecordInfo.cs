namespace System.Data.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public class EntityRecordInfo : DataRecordInfo
    {
        private readonly System.Data.EntityKey _entityKey;
        private readonly EntitySet _entitySet;

        internal EntityRecordInfo(DataRecordInfo info, System.Data.EntityKey entityKey, EntitySet entitySet) : base(info)
        {
            this._entityKey = entityKey;
            this._entitySet = entitySet;
        }

        internal EntityRecordInfo(EntityType metadata, System.Data.EntityKey entityKey, EntitySet entitySet) : base(TypeUsage.Create(metadata))
        {
            EntityUtil.CheckArgumentNull<System.Data.EntityKey>(entityKey, "entityKey");
            this._entityKey = entityKey;
            this._entitySet = entitySet;
        }

        public EntityRecordInfo(EntityType metadata, IEnumerable<EdmMember> memberInfo, System.Data.EntityKey entityKey, EntitySet entitySet) : base(TypeUsage.Create(metadata), memberInfo)
        {
            EntityUtil.CheckArgumentNull<System.Data.EntityKey>(entityKey, "entityKey");
            EntityUtil.CheckArgumentNull<EntitySet>(entitySet, "entitySet");
            this._entityKey = entityKey;
            this._entitySet = entitySet;
            this.ValidateEntityType(entitySet);
        }

        private void ValidateEntityType(EntitySetBase entitySet)
        {
            if (((!object.ReferenceEquals(base.RecordType.EdmType, null) && !object.ReferenceEquals(this._entityKey, System.Data.EntityKey.EntityNotValidKey)) && (!object.ReferenceEquals(this._entityKey, System.Data.EntityKey.NoEntitySetKey) && !object.ReferenceEquals(base.RecordType.EdmType, entitySet.ElementType))) && !entitySet.ElementType.IsBaseTypeOf(base.RecordType.EdmType))
            {
                throw EntityUtil.Argument(Strings.EntityTypesDoNotAgree);
            }
        }

        public System.Data.EntityKey EntityKey =>
            this._entityKey;
    }
}

