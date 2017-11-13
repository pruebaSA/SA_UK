namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct QueryBranchResult
    {
        internal QueryBranch branch;
        private int valIndex;
        internal QueryBranchResult(QueryBranch branch, int valIndex)
        {
            this.branch = branch;
            this.valIndex = valIndex;
        }

        internal QueryBranch Branch =>
            this.branch;
        internal int ValIndex =>
            this.valIndex;
    }
}

