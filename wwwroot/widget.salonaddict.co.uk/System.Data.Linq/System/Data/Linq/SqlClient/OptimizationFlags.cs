namespace System.Data.Linq.SqlClient
{
    using System;

    [Flags]
    internal enum OptimizationFlags
    {
        None,
        SimplifyCaseStatements,
        OptimizeLinkExpansions,
        All
    }
}

