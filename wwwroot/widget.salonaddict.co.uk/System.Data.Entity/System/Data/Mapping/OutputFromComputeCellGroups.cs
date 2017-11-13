namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct OutputFromComputeCellGroups
    {
        internal List<Cell> Cells;
        internal CqlIdentifiers Identifiers;
        internal List<Set<Cell>> CellGroups;
        internal List<ForeignConstraint> ForeignKeyConstraints;
        internal bool Success;
    }
}

