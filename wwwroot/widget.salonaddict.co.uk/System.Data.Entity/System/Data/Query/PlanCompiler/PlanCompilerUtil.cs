namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal static class PlanCompilerUtil
    {
        internal static bool IsRowTypeCaseOpWithNullability(CaseOp op, Node n, out bool thenClauseIsNull)
        {
            thenClauseIsNull = false;
            if (!TypeSemantics.IsRowType(op.Type))
            {
                return false;
            }
            if (n.Children.Count != 3)
            {
                return false;
            }
            if (!n.Child1.Op.Type.EdmEquals(op.Type) || !n.Child2.Op.Type.EdmEquals(op.Type))
            {
                return false;
            }
            if (n.Child1.Op.OpType == OpType.Null)
            {
                thenClauseIsNull = true;
                return true;
            }
            return (n.Child2.Op.OpType == OpType.Null);
        }
    }
}

