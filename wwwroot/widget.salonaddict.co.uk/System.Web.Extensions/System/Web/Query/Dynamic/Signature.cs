namespace System.Web.Query.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Signature : IEquatable<System.Web.Query.Dynamic.Signature>
    {
        public int hashCode;
        public DynamicProperty[] properties;

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            this.properties = properties.ToArray<DynamicProperty>();
            this.hashCode = 0;
            foreach (DynamicProperty property in properties)
            {
                this.hashCode ^= property.Name.GetHashCode() ^ property.Type.GetHashCode();
            }
        }

        public override bool Equals(object obj) => 
            ((obj is System.Web.Query.Dynamic.Signature) && this.Equals((System.Web.Query.Dynamic.Signature) obj));

        public bool Equals(System.Web.Query.Dynamic.Signature other)
        {
            if (this.properties.Length != other.properties.Length)
            {
                return false;
            }
            for (int i = 0; i < this.properties.Length; i++)
            {
                if ((this.properties[i].Name != other.properties[i].Name) || (this.properties[i].Type != other.properties[i].Type))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode() => 
            this.hashCode;
    }
}

