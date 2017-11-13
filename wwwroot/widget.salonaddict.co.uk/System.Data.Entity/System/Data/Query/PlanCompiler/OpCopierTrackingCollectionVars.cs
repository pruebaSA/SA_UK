namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class OpCopierTrackingCollectionVars : OpCopier
    {
        private Dictionary<Var, Node> m_newCollectionVarDefinitions;

        private OpCopierTrackingCollectionVars(Command cmd) : base(cmd)
        {
            this.m_newCollectionVarDefinitions = new Dictionary<Var, Node>();
        }

        internal static Node Copy(Command cmd, Node n, out VarMap varMap, out Dictionary<Var, Node> newCollectionVarDefinitions)
        {
            OpCopierTrackingCollectionVars vars = new OpCopierTrackingCollectionVars(cmd);
            Node node = vars.CopyNode(n);
            varMap = vars.m_varMap;
            newCollectionVarDefinitions = vars.m_newCollectionVarDefinitions;
            return node;
        }

        public override Node Visit(MultiStreamNestOp op, Node n)
        {
            Node node = base.Visit(op, n);
            MultiStreamNestOp op2 = (MultiStreamNestOp) node.Op;
            for (int i = 0; i < op2.CollectionInfo.Count; i++)
            {
                this.m_newCollectionVarDefinitions.Add(op2.CollectionInfo[i].CollectionVar, node.Children[i + 1]);
            }
            return node;
        }
    }
}

