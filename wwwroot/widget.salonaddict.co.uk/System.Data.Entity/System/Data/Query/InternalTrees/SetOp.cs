namespace System.Data.Query.InternalTrees
{
    using System;

    internal abstract class SetOp : RelOp
    {
        private VarVec m_outputVars;
        private System.Data.Query.InternalTrees.VarMap[] m_varMap;

        protected SetOp(OpType opType) : base(opType)
        {
        }

        internal SetOp(OpType opType, VarVec outputs, System.Data.Query.InternalTrees.VarMap left, System.Data.Query.InternalTrees.VarMap right) : this(opType)
        {
            this.m_varMap = new System.Data.Query.InternalTrees.VarMap[] { left, right };
            this.m_outputVars = outputs;
        }

        internal override int Arity =>
            2;

        internal VarVec Outputs =>
            this.m_outputVars;

        internal System.Data.Query.InternalTrees.VarMap[] VarMap =>
            this.m_varMap;
    }
}

