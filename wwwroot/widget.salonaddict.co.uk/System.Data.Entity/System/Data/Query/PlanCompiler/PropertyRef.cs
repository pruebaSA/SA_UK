namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;

    internal abstract class PropertyRef
    {
        internal PropertyRef()
        {
        }

        internal PropertyRef CreateNestedPropertyRef(EdmMember p) => 
            this.CreateNestedPropertyRef(new SimplePropertyRef(p));

        internal PropertyRef CreateNestedPropertyRef(RelProperty p) => 
            this.CreateNestedPropertyRef(new RelPropertyRef(p));

        internal virtual PropertyRef CreateNestedPropertyRef(PropertyRef p) => 
            new NestedPropertyRef(p, this);

        public override string ToString() => 
            "";
    }
}

