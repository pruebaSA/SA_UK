namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;

    internal class SimplePropertyRef : PropertyRef
    {
        private EdmMember m_property;

        internal SimplePropertyRef(EdmMember property)
        {
            this.m_property = property;
        }

        public override bool Equals(object obj)
        {
            SimplePropertyRef ref2 = obj as SimplePropertyRef;
            return (((ref2 != null) && Command.EqualTypes(this.m_property.DeclaringType, ref2.m_property.DeclaringType)) && ref2.m_property.Name.Equals(this.m_property.Name));
        }

        public override int GetHashCode() => 
            this.m_property.Name.GetHashCode();

        public override string ToString() => 
            this.m_property.Name;

        internal EdmMember Property =>
            this.m_property;
    }
}

