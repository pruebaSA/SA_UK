namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Xml;

    [DebuggerDisplay("DataModel={DataModel}")]
    internal class SchemaManager
    {
        private readonly SchemaDataModelOption _dataModel;
        private readonly HashSet<string> _namespaceLookUpTable = new HashSet<string>(StringComparer.Ordinal);
        private System.Data.EntityModel.SchemaObjectModel.PrimitiveSchema _primitiveSchema;
        private DbProviderManifest _providerManifest;
        private readonly ProviderManifestNeeded _providerManifestNeeded;
        private readonly AttributeValueNotification _providerManifestTokenNotification;
        private readonly AttributeValueNotification _providerNotification;
        private readonly SchemaElementLookUpTable<SchemaType> _schemaTypes = new SchemaElementLookUpTable<SchemaType>();
        private const int MaxErrorCount = 100;

        private SchemaManager(SchemaDataModelOption dataModel, AttributeValueNotification providerNotification, AttributeValueNotification providerManifestTokenNotification, ProviderManifestNeeded providerManifestNeeded)
        {
            this._dataModel = dataModel;
            this._providerNotification = providerNotification;
            this._providerManifestTokenNotification = providerManifestTokenNotification;
            this._providerManifestNeeded = providerManifestNeeded;
        }

        public void AddSchema(Schema schema)
        {
            if (((this._namespaceLookUpTable.Count == 0) && (schema.DataModel != SchemaDataModelOption.ProviderManifestModel)) && (this.PrimitiveSchema.Namespace != null))
            {
                this._namespaceLookUpTable.Add(this.PrimitiveSchema.Namespace);
            }
            this._namespaceLookUpTable.Add(schema.Namespace);
        }

        internal void EnsurePrimitiveSchemaIsLoaded()
        {
            if (this._primitiveSchema == null)
            {
                this._primitiveSchema = new System.Data.EntityModel.SchemaObjectModel.PrimitiveSchema(this);
            }
        }

        internal DbProviderManifest GetProviderManifest(Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
        {
            if (this._providerManifest == null)
            {
                this._providerManifest = this._providerManifestNeeded(addError);
            }
            return this._providerManifest;
        }

        public bool IsValidNamespaceName(string namespaceName) => 
            this._namespaceLookUpTable.Contains(namespaceName);

        public static IList<EdmSchemaError> LoadProviderManifest(XmlReader xmlReader, string location, bool checkForSystemNamespace, out Schema schema)
        {
            IList<Schema> schemaCollection = new List<Schema>(1);
            DbProviderManifest providerManifest = checkForSystemNamespace ? EdmProviderManifest.Instance : null;
            IList<EdmSchemaError> list2 = ParseAndValidate(new XmlReader[] { xmlReader }, new string[] { location }, SchemaDataModelOption.ProviderManifestModel, providerManifest, out schemaCollection);
            if (schemaCollection.Count != 0)
            {
                schema = schemaCollection[0];
                return list2;
            }
            schema = null;
            return list2;
        }

        public static void NoOpAttributeValueNotification(string attributeValue, Action<string, ErrorCode, EdmSchemaErrorSeverity> addError)
        {
        }

        public static IList<EdmSchemaError> ParseAndValidate(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> sourceFilePaths, SchemaDataModelOption dataModel, DbProviderManifest providerManifest, out IList<Schema> schemaCollection) => 
            ParseAndValidate(xmlReaders, sourceFilePaths, dataModel, new AttributeValueNotification(SchemaManager.NoOpAttributeValueNotification), new AttributeValueNotification(SchemaManager.NoOpAttributeValueNotification), delegate (Action<string, ErrorCode, EdmSchemaErrorSeverity> addError) {
                if (providerManifest != null)
                {
                    return providerManifest;
                }
                return MetadataItem.EdmProviderManifest;
            }, out schemaCollection);

        public static IList<EdmSchemaError> ParseAndValidate(IEnumerable<XmlReader> xmlReaders, IEnumerable<string> sourceFilePaths, SchemaDataModelOption dataModel, AttributeValueNotification providerNotification, AttributeValueNotification providerManifestTokenNotification, ProviderManifestNeeded providerManifestNeeded, out IList<Schema> schemaCollection)
        {
            List<string> list2;
            SchemaManager schemaManager = new SchemaManager(dataModel, providerNotification, providerManifestTokenNotification, providerManifestNeeded);
            List<EdmSchemaError> errorCollection = new List<EdmSchemaError>();
            schemaCollection = new List<Schema>();
            bool errorEncountered = false;
            if (sourceFilePaths != null)
            {
                list2 = new List<string>(sourceFilePaths);
            }
            else
            {
                list2 = new List<string>();
            }
            int num = 0;
            foreach (XmlReader reader in xmlReaders)
            {
                string location = null;
                if (list2.Count <= num)
                {
                    TryGetBaseUri(reader, out location);
                }
                else
                {
                    location = list2[num];
                }
                Schema item = new Schema(schemaManager);
                if (UpdateErrorCollectionAndCheckForMaxErrors(errorCollection, item.Parse(reader, location), ref errorEncountered))
                {
                    return errorCollection;
                }
                if (!errorEncountered)
                {
                    schemaCollection.Add(item);
                    schemaManager.AddSchema(item);
                }
                num++;
            }
            if (!errorEncountered)
            {
                foreach (Schema schema2 in schemaCollection)
                {
                    if (UpdateErrorCollectionAndCheckForMaxErrors(errorCollection, schema2.Resolve(), ref errorEncountered))
                    {
                        return errorCollection;
                    }
                }
                if (errorEncountered)
                {
                    return errorCollection;
                }
                foreach (Schema schema3 in schemaCollection)
                {
                    if (UpdateErrorCollectionAndCheckForMaxErrors(errorCollection, schema3.ValidateSchema(), ref errorEncountered))
                    {
                        return errorCollection;
                    }
                }
            }
            return errorCollection;
        }

        internal static bool TryGetBaseUri(XmlReader xmlReader, out string location)
        {
            string baseURI = xmlReader.BaseURI;
            Uri result = null;
            if ((!string.IsNullOrEmpty(baseURI) && Uri.TryCreate(baseURI, UriKind.Absolute, out result)) && (result.Scheme == "file"))
            {
                location = Helper.GetFileNameFromUri(result);
                return true;
            }
            location = null;
            return false;
        }

        public bool TryResolveType(string namespaceName, string typeName, out SchemaType schemaType)
        {
            string key = string.IsNullOrEmpty(namespaceName) ? typeName : (namespaceName + "." + typeName);
            schemaType = this.SchemaTypes.LookUpEquivalentKey(key);
            return (schemaType != null);
        }

        private static bool UpdateErrorCollectionAndCheckForMaxErrors(List<EdmSchemaError> errorCollection, IList<EdmSchemaError> newErrors, ref bool errorEncountered)
        {
            if (!errorEncountered && !MetadataHelper.CheckIfAllErrorsAreWarnings(newErrors))
            {
                errorEncountered = true;
            }
            errorCollection.AddRange(newErrors);
            return (errorCollection.Count >= 100);
        }

        internal SchemaDataModelOption DataModel =>
            this._dataModel;

        internal System.Data.EntityModel.SchemaObjectModel.PrimitiveSchema PrimitiveSchema =>
            this._primitiveSchema;

        internal AttributeValueNotification ProviderManifestTokenNotification =>
            this._providerManifestTokenNotification;

        internal AttributeValueNotification ProviderNotification =>
            this._providerNotification;

        internal SchemaElementLookUpTable<SchemaType> SchemaTypes =>
            this._schemaTypes;
    }
}

