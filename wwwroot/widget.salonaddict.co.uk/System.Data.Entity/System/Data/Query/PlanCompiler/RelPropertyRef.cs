namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Query.InternalTrees;

    internal class RelPropertyRef : PropertyRef
    {
        private RelProperty m_property;

        internal RelPropertyRef(RelProperty property)
        {
            this.m_property = property;
        }

        public override bool Equals(object obj)
        {
            RelPropertyRef ref2 = obj as RelPropertyRef;
            return ((ref2 != null) && this.m_property.Equals(ref2.m_property));
        }

        public override int GetHashCode() => 
            this.m_property.GetHashCode();

        public override string ToString() => 
            this.m_property.ToString();

        internal RelProperty Property =>
            this.m_property;
    }
}

