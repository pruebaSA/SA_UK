namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;

    internal sealed class ProviderCommandInfo
    {
        private List<ProviderCommandInfo> _children;
        private DbCommandTree _commandTree;
        private ProviderCommandInfo _parent;

        internal ProviderCommandInfo(DbCommandTree commandTree, List<ProviderCommandInfo> children)
        {
            this._commandTree = commandTree;
            this._children = children;
            if (this._children == null)
            {
                this._children = new List<ProviderCommandInfo>();
            }
            foreach (ProviderCommandInfo info in this._children)
            {
                info._parent = this;
            }
        }

        internal DbCommandTree CommandTree =>
            this._commandTree;
    }
}

