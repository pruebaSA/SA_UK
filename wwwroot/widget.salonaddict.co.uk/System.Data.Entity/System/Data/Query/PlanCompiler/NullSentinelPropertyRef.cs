namespace System.Data.Query.PlanCompiler
{
    using System;

    internal class NullSentinelPropertyRef : PropertyRef
    {
        private static NullSentinelPropertyRef s_singleton = new NullSentinelPropertyRef();

        private NullSentinelPropertyRef()
        {
        }

        public override string ToString() => 
            "NULLSENTINEL";

        internal static NullSentinelPropertyRef Instance =>
            s_singleton;
    }
}

