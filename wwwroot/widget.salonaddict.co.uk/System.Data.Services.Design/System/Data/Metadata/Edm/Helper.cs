namespace System.Data.Metadata.Edm
{
    using System;

    internal static class Helper
    {
        internal static bool IsCollectionType(GlobalItem item) => 
            (BuiltInTypeKind.CollectionType == item.BuiltInTypeKind);

        internal static bool IsComplexType(EdmType type) => 
            (BuiltInTypeKind.ComplexType == type.BuiltInTypeKind);

        internal static bool IsEntitySet(EntitySetBase entitySetBase) => 
            (BuiltInTypeKind.EntitySet == entitySetBase.BuiltInTypeKind);

        internal static bool IsPrimitiveType(EdmType type) => 
            (BuiltInTypeKind.PrimitiveType == type.BuiltInTypeKind);
    }
}

