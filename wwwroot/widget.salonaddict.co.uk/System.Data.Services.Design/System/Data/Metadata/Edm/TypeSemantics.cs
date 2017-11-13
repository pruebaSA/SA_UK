namespace System.Data.Metadata.Edm
{
    using System;

    internal static class TypeSemantics
    {
        internal static bool IsComplexType(TypeUsage type) => 
            System.Data.Metadata.Edm.Helper.IsComplexType(type.EdmType);
    }
}

