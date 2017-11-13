namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;

    internal sealed class TypeUsageEqualityComparer : IEqualityComparer<TypeUsage>
    {
        internal static readonly TypeUsageEqualityComparer Instance = new TypeUsageEqualityComparer();

        private TypeUsageEqualityComparer()
        {
        }

        internal static bool Equals(EdmType x, EdmType y) => 
            x.Identity.Equals(y.Identity);

        public bool Equals(TypeUsage x, TypeUsage y) => 
            (((x != null) && (y != null)) && Equals(x.EdmType, y.EdmType));

        public int GetHashCode(TypeUsage obj) => 
            obj.EdmType.Identity.GetHashCode();
    }
}

