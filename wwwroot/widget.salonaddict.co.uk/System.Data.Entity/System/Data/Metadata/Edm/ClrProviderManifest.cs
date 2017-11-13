namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Xml;

    internal class ClrProviderManifest : DbProviderManifest
    {
        private static ClrProviderManifest _instance = new ClrProviderManifest();
        private System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> _primitiveTypes;
        private const int s_PrimitiveTypeCount = 15;

        private ClrProviderManifest()
        {
        }

        private PrimitiveType CreatePrimitiveType(Type clrType, PrimitiveTypeKind primitiveTypeKind) => 
            new PrimitiveType(clrType, MetadataItem.EdmProviderManifest.GetPrimitiveType(primitiveTypeKind), this);

        protected override XmlReader GetDbInformation(string informationType)
        {
            throw new NotImplementedException();
        }

        public override TypeUsage GetEdmType(TypeUsage storeType)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription> GetFacetDescriptions(EdmType type)
        {
            if (Helper.IsPrimitiveType(type) && (((PrimitiveType) type).DataSpace == DataSpace.OSpace))
            {
                PrimitiveType baseType = (PrimitiveType) type.BaseType;
                return baseType.ProviderManifest.GetFacetDescriptions(baseType);
            }
            return Helper.EmptyFacetDescriptionEnumerable;
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> GetStoreFunctions() => 
            Helper.EmptyEdmFunctionReadOnlyCollection;

        public override TypeUsage GetStoreType(TypeUsage edmType)
        {
            throw new NotImplementedException();
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetStoreTypes()
        {
            this.InitializePrimitiveTypes();
            return this._primitiveTypes;
        }

        private void InitializePrimitiveTypes()
        {
            if (this._primitiveTypes == null)
            {
                PrimitiveType[] list = new PrimitiveType[15];
                list[0] = this.CreatePrimitiveType(typeof(byte[]), PrimitiveTypeKind.Binary);
                list[1] = this.CreatePrimitiveType(typeof(bool), PrimitiveTypeKind.Boolean);
                list[2] = this.CreatePrimitiveType(typeof(byte), PrimitiveTypeKind.Byte);
                list[3] = this.CreatePrimitiveType(typeof(DateTime), PrimitiveTypeKind.DateTime);
                list[13] = this.CreatePrimitiveType(typeof(TimeSpan), PrimitiveTypeKind.Time);
                list[14] = this.CreatePrimitiveType(typeof(DateTimeOffset), PrimitiveTypeKind.DateTimeOffset);
                list[4] = this.CreatePrimitiveType(typeof(decimal), PrimitiveTypeKind.Decimal);
                list[5] = this.CreatePrimitiveType(typeof(double), PrimitiveTypeKind.Double);
                list[6] = this.CreatePrimitiveType(typeof(Guid), PrimitiveTypeKind.Guid);
                list[9] = this.CreatePrimitiveType(typeof(short), PrimitiveTypeKind.Int16);
                list[10] = this.CreatePrimitiveType(typeof(int), PrimitiveTypeKind.Int32);
                list[11] = this.CreatePrimitiveType(typeof(long), PrimitiveTypeKind.Int64);
                list[8] = this.CreatePrimitiveType(typeof(sbyte), PrimitiveTypeKind.SByte);
                list[7] = this.CreatePrimitiveType(typeof(float), PrimitiveTypeKind.Single);
                list[12] = this.CreatePrimitiveType(typeof(string), PrimitiveTypeKind.String);
                for (int i = 0; i < list.Length; i++)
                {
                    list[i].SetReadOnly();
                }
                System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> onlys = new System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>(list);
                Interlocked.CompareExchange<System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType>>(ref this._primitiveTypes, onlys, null);
            }
        }

        internal bool TryGetPrimitiveType(Type clrType, out PrimitiveType primitiveType)
        {
            primitiveType = null;
            this.InitializePrimitiveTypes();
            for (int i = 0; i < this._primitiveTypes.Count; i++)
            {
                if (this._primitiveTypes[i].ClrEquivalentType.Equals(clrType))
                {
                    primitiveType = this._primitiveTypes[i];
                    return true;
                }
            }
            return false;
        }

        internal static ClrProviderManifest Instance =>
            _instance;

        public override string NamespaceName =>
            "System";
    }
}

