namespace System.Data.Query.PlanCompiler
{
    using System;

    internal enum PlanCompilerPhase
    {
        PreProcessor,
        NTE,
        ProjectionPruning,
        NestPullup,
        Transformations,
        JoinElimination,
        CodeGen,
        PostCodeGen,
        MaxMarker
    }
}

