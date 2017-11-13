namespace System.Data.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    internal sealed class ObjectReferenceEqualityComparer : IEqualityComparer<object>
    {
        bool IEqualityComparer<object>.Equals(object x, object y) => 
            object.ReferenceEquals(x, y);

        int IEqualityComparer<object>.GetHashCode(object obj) => 
            RuntimeHelpers.GetHashCode(obj);
    }
}

