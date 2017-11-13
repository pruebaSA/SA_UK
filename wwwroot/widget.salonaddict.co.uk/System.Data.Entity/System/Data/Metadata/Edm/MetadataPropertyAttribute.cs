namespace System.Data.Metadata.Edm
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    internal sealed class MetadataPropertyAttribute : Attribute
    {
        private readonly bool _isCollectionType;
        private readonly EdmType _type;

        internal MetadataPropertyAttribute(BuiltInTypeKind builtInTypeKind, bool isCollectionType) : this(MetadataItem.GetBuiltInType(builtInTypeKind), isCollectionType)
        {
        }

        private MetadataPropertyAttribute(EdmType type, bool isCollectionType)
        {
            this._type = type;
            this._isCollectionType = isCollectionType;
        }

        internal MetadataPropertyAttribute(PrimitiveTypeKind primitiveTypeKind, bool isCollectionType) : this(MetadataItem.EdmProviderManifest.GetPrimitiveType(primitiveTypeKind), isCollectionType)
        {
        }

        internal MetadataPropertyAttribute(System.Type type, bool isCollection) : this(ClrComplexType.CreateReadonlyClrComplexType(type, type.Namespace ?? string.Empty, type.Name), isCollection)
        {
        }

        internal bool IsCollectionType =>
            this._isCollectionType;

        internal EdmType Type =>
            this._type;
    }
}

