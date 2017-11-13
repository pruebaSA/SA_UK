namespace System.Data.Query.PlanCompiler
{
    using System;

    internal class AllPropertyRef : PropertyRef
    {
        internal static AllPropertyRef Instance = new AllPropertyRef();

        private AllPropertyRef()
        {
        }

        internal override PropertyRef CreateNestedPropertyRef(PropertyRef p) => 
            p;

        public override string ToString() => 
            "ALL";
    }
}

