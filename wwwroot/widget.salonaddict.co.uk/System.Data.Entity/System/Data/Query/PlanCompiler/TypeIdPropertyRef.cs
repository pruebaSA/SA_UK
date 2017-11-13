namespace System.Data.Query.PlanCompiler
{
    using System;

    internal class TypeIdPropertyRef : PropertyRef
    {
        internal static TypeIdPropertyRef Instance = new TypeIdPropertyRef();

        private TypeIdPropertyRef()
        {
        }

        public override string ToString() => 
            "TYPEID";
    }
}

