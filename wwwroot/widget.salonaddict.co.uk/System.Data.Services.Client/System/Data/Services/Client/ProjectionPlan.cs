namespace System.Data.Services.Client
{
    using System;
    using System.Runtime.CompilerServices;

    internal class ProjectionPlan
    {
        internal object Run(AtomMaterializer materializer, AtomEntry entry, Type expectedType) => 
            this.Plan(materializer, entry, expectedType);

        internal Type LastSegmentType { get; set; }

        internal Func<object, object, Type, object> Plan { get; set; }

        internal Type ProjectedType { get; set; }
    }
}

