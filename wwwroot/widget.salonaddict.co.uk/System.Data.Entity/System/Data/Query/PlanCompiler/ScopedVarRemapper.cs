namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class ScopedVarRemapper : VarRemapper
    {
        private Dictionary<Node, Var> m_hidingScopeNodesMap;

        internal ScopedVarRemapper(Command command) : base(command)
        {
        }

        internal void AddMapping(Var oldVar, Var newVar, Node hidingScopeNode)
        {
            if (this.m_hidingScopeNodesMap == null)
            {
                this.m_hidingScopeNodesMap = new Dictionary<Node, Var>();
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!this.m_hidingScopeNodesMap.ContainsKey(hidingScopeNode), "The ScopedVarRemapper is in an inconsistent state. The given node has already been used as a hiding scope.");
            this.m_hidingScopeNodesMap[hidingScopeNode] = oldVar;
            base.AddMapping(oldVar, newVar);
        }

        internal override void RemapNode(Node n)
        {
            this.VisitHonoringHiding(n, new VarRemapper.VisitNodeDelegate(this.RemapNode));
        }

        internal override void RemapSubtree(Node subTree)
        {
            this.VisitHonoringHiding(subTree, new VarRemapper.VisitNodeDelegate(this.RemapSubtreeUsingBaseRemapNode));
        }

        private void RemapSubtreeUsingBaseRemapNode(Node subTree)
        {
            base.RemapSubtree(subTree, new VarRemapper.VisitNodeDelegate(this.RemapNode));
        }

        private bool TryGetHiddenMappingKey(Node node, out Var var) => 
            this.m_hidingScopeNodesMap?.TryGetValue(node, out var);

        private void VisitHonoringHiding(Node n, VarRemapper.VisitNodeDelegate visitMethod)
        {
            Var var;
            if (this.TryGetHiddenMappingKey(n, out var))
            {
                base.HideMappingKey(var);
            }
            visitMethod(n);
            if (var != null)
            {
                base.UnhideMappingKey(var);
            }
        }
    }
}

