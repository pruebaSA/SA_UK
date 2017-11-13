namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Mapping.ViewGeneration.Structures;

    internal abstract class ConstraintBase : InternalBase
    {
        protected ConstraintBase()
        {
        }

        internal abstract ErrorLog.Record GetErrorRecord();
    }
}

