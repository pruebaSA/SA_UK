namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Runtime.CompilerServices;

    internal delegate void AttributeValueNotification(string token, Action<string, ErrorCode, EdmSchemaErrorSeverity> addError);
}

