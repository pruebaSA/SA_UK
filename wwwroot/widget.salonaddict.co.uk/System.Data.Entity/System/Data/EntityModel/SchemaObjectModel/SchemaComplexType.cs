namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    internal sealed class SchemaComplexType : StructuredType
    {
        internal SchemaComplexType(Schema parentElement) : base(parentElement)
        {
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if ((base.BaseType != null) && !(base.BaseType is SchemaComplexType))
            {
                base.AddError(ErrorCode.InvalidBaseType, EdmSchemaErrorSeverity.Error, Strings.InvalidBaseTypeForNestedType(base.BaseType.FQName, this.FQName));
            }
        }
    }
}

