namespace MS.Internal.ComponentModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    internal class PropertyDescriptorComparer : IEqualityComparer<PropertyDescriptor>
    {
        public bool Equals(PropertyDescriptor p1, PropertyDescriptor p2) => 
            object.ReferenceEquals(p1, p2);

        public int GetHashCode(PropertyDescriptor p) => 
            p.GetHashCode();
    }
}

