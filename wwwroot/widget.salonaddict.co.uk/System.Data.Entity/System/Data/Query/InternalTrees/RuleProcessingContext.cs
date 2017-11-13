namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class RuleProcessingContext
    {
        private System.Data.Query.InternalTrees.Command m_command;

        internal RuleProcessingContext(System.Data.Query.InternalTrees.Command command)
        {
            this.m_command = command;
        }

        internal virtual int GetHashCode(Node node) => 
            node.GetHashCode();

        internal virtual void PostProcess(Node node, Rule rule)
        {
        }

        internal virtual void PreProcess(Node node)
        {
        }

        internal virtual void PreProcessSubTree(Node node)
        {
        }

        internal System.Data.Query.InternalTrees.Command Command =>
            this.m_command;
    }
}

