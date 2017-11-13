namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;

    internal abstract class VarInfo
    {
        protected VarInfo()
        {
        }

        internal virtual bool IsCollectionType =>
            false;

        internal virtual bool IsStructuredType =>
            false;

        internal virtual List<Var> NewVars =>
            null;
    }
}

