namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;

    internal class TypeHandleRefEqualityComparer : IEqualityComparer<TypeHandleRef>
    {
        public bool Equals(TypeHandleRef x, TypeHandleRef y) => 
            x.Value.Equals(y.Value);

        public int GetHashCode(TypeHandleRef obj) => 
            obj.Value.GetHashCode();
    }
}

