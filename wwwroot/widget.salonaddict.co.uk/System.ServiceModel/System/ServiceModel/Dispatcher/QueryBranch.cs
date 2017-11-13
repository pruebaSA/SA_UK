namespace System.ServiceModel.Dispatcher
{
    using System;

    internal class QueryBranch
    {
        internal Opcode branch;
        internal int id;

        internal QueryBranch(Opcode branch, int id)
        {
            this.branch = branch;
            this.id = id;
        }

        internal Opcode Branch =>
            this.branch;

        internal int ID =>
            this.id;
    }
}

