namespace System.Data.Metadata.Edm
{
    using System;
    using System.Text;

    public sealed class RefType : EdmType
    {
        private readonly EntityTypeBase _elementType;

        internal RefType(EntityType entityType) : base(GetIdentity(EntityUtil.GenericCheckArgumentNull<EntityType>(entityType, "entityType")), "Transient", entityType.DataSpace)
        {
            this._elementType = entityType;
            this.SetReadOnly();
        }

        private static string GetIdentity(EntityTypeBase entityTypeBase)
        {
            StringBuilder builder = new StringBuilder(50);
            builder.Append("reference[");
            entityTypeBase.BuildIdentity(builder);
            builder.Append("]");
            return builder.ToString();
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.RefType;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EntityTypeBase, false)]
        public EntityTypeBase ElementType =>
            this._elementType;
    }
}

