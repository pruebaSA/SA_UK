namespace System.Data.Query.PlanCompiler
{
    using System;

    internal class NestedPropertyRef : PropertyRef
    {
        private readonly PropertyRef m_inner;
        private readonly PropertyRef m_outer;

        internal NestedPropertyRef(PropertyRef innerProperty, PropertyRef outerProperty)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!(innerProperty is NestedPropertyRef), "innerProperty cannot be a NestedPropertyRef");
            this.m_inner = innerProperty;
            this.m_outer = outerProperty;
        }

        public override bool Equals(object obj)
        {
            NestedPropertyRef ref2 = obj as NestedPropertyRef;
            return (((ref2 != null) && this.m_inner.Equals(ref2.m_inner)) && this.m_outer.Equals(ref2.m_outer));
        }

        public override int GetHashCode() => 
            (this.m_inner.GetHashCode() ^ this.m_outer.GetHashCode());

        public override string ToString() => 
            (this.m_inner + "." + this.m_outer);

        internal PropertyRef InnerProperty =>
            this.m_inner;

        internal PropertyRef OuterProperty =>
            this.m_outer;
    }
}

