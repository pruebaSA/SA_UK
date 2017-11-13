namespace System.Data.Common
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    public abstract class DbProviderManifest
    {
        internal const string CollationFacetName = "Collation";
        public static readonly string ConceptualSchemaDefinition = "ConceptualSchemaDefinition";
        internal const string DefaultValueFacetName = "DefaultValue";
        internal const string FixedLengthFacetName = "FixedLength";
        internal const string MaxLengthFacetName = "MaxLength";
        internal const string NullableFacetName = "Nullable";
        internal const string PrecisionFacetName = "Precision";
        internal const string ScaleFacetName = "Scale";
        public static readonly string StoreSchemaDefinition = "StoreSchemaDefinition";
        public static readonly string StoreSchemaMapping = "StoreSchemaMapping";
        internal const string UnicodeFacetName = "Unicode";

        protected DbProviderManifest()
        {
        }

        protected abstract XmlReader GetDbInformation(string informationType);
        public abstract TypeUsage GetEdmType(TypeUsage storeType);
        public abstract System.Collections.ObjectModel.ReadOnlyCollection<FacetDescription> GetFacetDescriptions(EdmType edmType);
        public XmlReader GetInformation(string informationType)
        {
            XmlReader dbInformation = null;
            try
            {
                dbInformation = this.GetDbInformation(informationType);
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.ProviderIncompatible(Strings.EntityClient_FailedToGetInformation(informationType), exception);
                }
                throw;
            }
            if (dbInformation != null)
            {
                return dbInformation;
            }
            if (informationType != ConceptualSchemaDefinition)
            {
                throw EntityUtil.ProviderIncompatible(Strings.ProviderReturnedNullForGetDbInformation(informationType));
            }
            return DbProviderServices.GetConceptualSchemaDescription();
        }

        public abstract System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> GetStoreFunctions();
        public abstract TypeUsage GetStoreType(TypeUsage edmType);
        public abstract System.Collections.ObjectModel.ReadOnlyCollection<PrimitiveType> GetStoreTypes();

        public abstract string NamespaceName { get; }
    }
}

