namespace System.Data.Query.PlanCompiler
{
    using System;

    internal class EntitySetIdPropertyRef : PropertyRef
    {
        internal static EntitySetIdPropertyRef Instance = new EntitySetIdPropertyRef();

        private EntitySetIdPropertyRef()
        {
        }

        public override string ToString() => 
            "ENTITYSETID";
    }
}

