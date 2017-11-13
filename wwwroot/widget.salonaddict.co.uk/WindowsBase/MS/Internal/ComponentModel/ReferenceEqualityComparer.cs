namespace MS.Internal.ComponentModel
{
    using System;
    using System.Collections.Generic;

    internal class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public int GetHashCode(object o) => 
            o.GetHashCode();

        bool IEqualityComparer<object>.Equals(object o1, object o2) => 
            object.ReferenceEquals(o1, o2);
    }
}

