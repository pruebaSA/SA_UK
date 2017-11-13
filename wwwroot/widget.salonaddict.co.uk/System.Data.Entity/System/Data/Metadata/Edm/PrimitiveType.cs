namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Linq;

    public sealed class PrimitiveType : System.Data.Metadata.Edm.SimpleType
    {
        private System.Data.Metadata.Edm.PrimitiveTypeKind _primitiveTypeKind;
        private DbProviderManifest _providerManifest;

        internal PrimitiveType()
        {
        }

        internal PrimitiveType(Type clrType, PrimitiveType baseType, DbProviderManifest providerManifest) : this(EntityUtil.GenericCheckArgumentNull<Type>(clrType, "clrType").Name, clrType.Namespace, DataSpace.OSpace, baseType, providerManifest)
        {
        }

        internal PrimitiveType(string name, string namespaceName, DataSpace dataSpace, PrimitiveType baseType, DbProviderManifest providerManifest) : base(name, namespaceName, dataSpace)
        {
            EntityUtil.GenericCheckArgumentNull<PrimitiveType>(baseType, "baseType");
            EntityUtil.GenericCheckArgumentNull<DbProviderManifest>(providerManifest, "providerManifest");
            base.BaseType = baseType;
            Initialize(this, baseType.PrimitiveTypeKind, false, providerManifest);
        }

        internal override IEnumerable<FacetDescription> GetAssociatedFacetDescriptions() => 
            base.GetAssociatedFacetDescriptions().Concat<FacetDescription>(this.FacetDescriptions);

        public EdmType GetEdmPrimitiveType() => 
            MetadataItem.EdmProviderManifest.GetPrimitiveType(this.PrimitiveTypeKind);

        public static PrimitiveType GetEdmPrimitiveType(System.Data.Metadata.Edm.PrimitiveTypeKind primitiveTypeKind) => 
            MetadataItem.EdmProviderManifest.GetPrimitiveType(primitiveTypeKind);

        public static System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetEdmPrimitiveTypes() => 
            MetadataItem.EdmProviderManifest.GetStoreTypes();

        internal static void Initialize(PrimitiveType primitiveType, System.Data.Metadata.Edm.PrimitiveTypeKind primitiveTypeKind, bool isDefaultType, DbProviderManifest providerManifest)
        {
            primitiveType._primitiveTypeKind = primitiveTypeKind;
            primitiveType._providerManifest = providerManifest;
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.PrimitiveType;

        public Type ClrEquivalentType
        {
            get
            {
                switch (this.PrimitiveTypeKind)
                {
                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Binary:
                        return typeof(byte[]);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Boolean:
                        return typeof(bool);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Byte:
                        return typeof(byte);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.DateTime:
                        return typeof(DateTime);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Decimal:
                        return typeof(decimal);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Double:
                        return typeof(double);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Guid:
                        return typeof(Guid);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Single:
                        return typeof(float);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.SByte:
                        return typeof(sbyte);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Int16:
                        return typeof(short);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Int32:
                        return typeof(int);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Int64:
                        return typeof(long);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.String:
                        return typeof(string);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.Time:
                        return typeof(TimeSpan);

                    case System.Data.Metadata.Edm.PrimitiveTypeKind.DateTimeOffset:
                        return typeof(DateTimeOffset);
                }
                return null;
            }
        }

        internal override Type ClrType =>
            this.ClrEquivalentType;

        public System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription> FacetDescriptions =>
            this.ProviderManifest.GetFacetDescriptions(this);

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.PrimitiveTypeKind, false)]
        public System.Data.Metadata.Edm.PrimitiveTypeKind PrimitiveTypeKind
        {
            get => 
                this._primitiveTypeKind;
            internal set
            {
                this._primitiveTypeKind = value;
            }
        }

        internal DbProviderManifest ProviderManifest
        {
            get => 
                this._providerManifest;
            set
            {
                this._providerManifest = value;
            }
        }
    }
}

