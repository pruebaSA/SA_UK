namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    internal sealed class EntitySetBaseCollection : MetadataCollection<EntitySetBase>
    {
        private readonly EntityContainer _entityContainer;

        internal EntitySetBaseCollection(EntityContainer entityContainer) : this(entityContainer, null)
        {
        }

        internal EntitySetBaseCollection(EntityContainer entityContainer, IEnumerable<EntitySetBase> items) : base(items)
        {
            EntityUtil.GenericCheckArgumentNull<EntityContainer>(entityContainer, "entityContainer");
            this._entityContainer = entityContainer;
        }

        public override void Add(EntitySetBase item)
        {
            EntityUtil.GenericCheckArgumentNull<EntitySetBase>(item, "item");
            ThrowIfItHasEntityContainer(item, "item");
            base.Add(item);
            item.ChangeEntityContainerWithoutCollectionFixup(this._entityContainer);
        }

        private static void ThrowIfItHasEntityContainer(EntitySetBase entitySet, string argumentName)
        {
            EntityUtil.GenericCheckArgumentNull<EntitySetBase>(entitySet, argumentName);
            if (entitySet.EntityContainer != null)
            {
                throw EntityUtil.EntitySetInAnotherContainer(argumentName);
            }
        }

        public override EntitySetBase this[int index]
        {
            get => 
                base[index];
            set
            {
                throw EntityUtil.OperationOnReadOnlyCollection();
            }
        }

        public override EntitySetBase this[string identity]
        {
            get => 
                base[identity];
            set
            {
                throw EntityUtil.OperationOnReadOnlyCollection();
            }
        }
    }
}

