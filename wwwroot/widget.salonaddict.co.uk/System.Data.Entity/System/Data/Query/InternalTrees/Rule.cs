namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal abstract class Rule
    {
        private ProcessNodeDelegate m_nodeDelegate;
        private OpType m_opType;

        protected Rule(OpType opType, ProcessNodeDelegate nodeProcessDelegate)
        {
            this.m_opType = opType;
            this.m_nodeDelegate = nodeProcessDelegate;
        }

        internal bool Apply(RuleProcessingContext ruleProcessingContext, Node node, out Node newNode) => 
            this.m_nodeDelegate(ruleProcessingContext, node, out newNode);

        internal abstract bool Match(Node node);

        internal OpType RuleOpType =>
            this.m_opType;

        internal delegate bool ProcessNodeDelegate(RuleProcessingContext context, Node subTree, out Node newSubTree);
    }
}

