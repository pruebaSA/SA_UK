namespace System.Data.Services.Client
{
    using System;
    using System.Collections;

    internal class ReferenceEqualityComparer : IEqualityComparer
    {
        protected ReferenceEqualityComparer()
        {
        }

        bool IEqualityComparer.Equals(object x, object y) => 
            object.ReferenceEquals(x, y);

        int IEqualityComparer.GetHashCode(object obj) => 
            obj?.GetHashCode();
    }
}

