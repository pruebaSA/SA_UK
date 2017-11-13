namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Common;
    using System.Runtime.CompilerServices;

    internal delegate DbProviderManifest ProviderManifestNeeded(Action<string, ErrorCode, EdmSchemaErrorSeverity> addError);
}

