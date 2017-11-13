namespace System.Data.Metadata.Edm
{
    using System;
    using System.Text;

    public sealed class CollectionType : EdmType
    {
        private readonly System.Data.Metadata.Edm.TypeUsage _typeUsage;

        internal CollectionType(EdmType elementType) : this(System.Data.Metadata.Edm.TypeUsage.Create(elementType))
        {
            base.DataSpace = elementType.DataSpace;
        }

        internal CollectionType(System.Data.Metadata.Edm.TypeUsage elementType) : base(GetIdentity(EntityUtil.GenericCheckArgumentNull<System.Data.Metadata.Edm.TypeUsage>(elementType, "elementType")), "Transient", elementType.EdmType.DataSpace)
        {
            this._typeUsage = elementType;
            this.SetReadOnly();
        }

        internal override bool EdmEquals(MetadataItem item)
        {
            if (object.ReferenceEquals(this, item))
            {
                return true;
            }
            if ((item == null) || (System.Data.Metadata.Edm.BuiltInTypeKind.CollectionType != item.BuiltInTypeKind))
            {
                return false;
            }
            CollectionType type = (CollectionType) item;
            return this.TypeUsage.EdmEquals(type.TypeUsage);
        }

        private static string GetIdentity(System.Data.Metadata.Edm.TypeUsage typeUsage)
        {
            StringBuilder builder = new StringBuilder(50);
            builder.Append("collection[");
            typeUsage.BuildIdentity(builder);
            builder.Append("]");
            return builder.ToString();
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.CollectionType;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage, false)]
        public System.Data.Metadata.Edm.TypeUsage TypeUsage =>
            this._typeUsage;
    }
}

