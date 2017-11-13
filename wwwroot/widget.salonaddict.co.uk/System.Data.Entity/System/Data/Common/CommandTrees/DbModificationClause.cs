namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Common.Utils;

    public abstract class DbModificationClause
    {
        private DbModificationCommandTree _commandTree;

        internal DbModificationClause(DbModificationCommandTree commandTree)
        {
            this._commandTree = commandTree;
        }

        internal abstract DbModificationClause Copy(DbModificationCommandTree newTree);
        internal abstract void DumpStructure(ExpressionDumper dumper);
        internal abstract TreeNode Print(DbExpressionVisitor<TreeNode> visitor);
    }
}

