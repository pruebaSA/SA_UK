namespace System.Data.Mapping.ViewGeneration
{
    using System;

    internal enum PerfType
    {
        InitialSetup,
        CellCreation,
        KeyConstraint,
        CellNormalizer,
        UpdateViews,
        DisjointConstraint,
        PartitionConstraint,
        DomainConstraint,
        ForeignConstraint,
        QueryViews,
        BoolResolution,
        Unsatisfiability,
        ViewParsing
    }
}

