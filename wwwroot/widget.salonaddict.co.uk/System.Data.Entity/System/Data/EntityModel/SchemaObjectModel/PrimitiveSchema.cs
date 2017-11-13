namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal class PrimitiveSchema : Schema
    {
        public PrimitiveSchema(SchemaManager schemaManager) : base(schemaManager)
        {
            base.Schema = this;
            DbProviderManifest providerManifest = base.ProviderManifest;
            if (providerManifest == null)
            {
                base.AddError(new EdmSchemaError(Strings.FailedToRetrieveProviderManifest, 0xa8, EdmSchemaErrorSeverity.Error));
            }
            else
            {
                foreach (PrimitiveType type in providerManifest.GetStoreTypes())
                {
                    base.TryAddType(new ScalarType(this, type.Name, type), false);
                }
            }
        }

        protected override bool HandleAttribute(XmlReader reader) => 
            false;

        internal override string Alias =>
            base.ProviderManifest.NamespaceName;

        internal override string Namespace
        {
            get
            {
                if (base.ProviderManifest != null)
                {
                    return base.ProviderManifest.NamespaceName;
                }
                return string.Empty;
            }
        }
    }
}

