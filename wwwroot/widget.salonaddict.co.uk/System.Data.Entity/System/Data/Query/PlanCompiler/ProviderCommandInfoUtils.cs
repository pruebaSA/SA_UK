namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;

    internal static class ProviderCommandInfoUtils
    {
        private static Dictionary<Var, EdmProperty> BuildOutputVarMap(PhysicalProjectOp projectOp, TypeUsage outputType)
        {
            bool flag;
            Dictionary<Var, EdmProperty> dictionary = new Dictionary<Var, EdmProperty>();
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsRowType(outputType), "PhysicalProjectOp result type is not a RowType?");
            IEnumerator<EdmProperty> enumerator = TypeHelpers.GetEdmType<RowType>(outputType).Properties.GetEnumerator();
            IEnumerator<Var> enumerator2 = projectOp.Outputs.GetEnumerator();
        Label_003D:
            flag = enumerator.MoveNext();
            bool flag2 = enumerator2.MoveNext();
            if (flag != flag2)
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 1);
            }
            if (flag)
            {
                dictionary[enumerator2.Current] = enumerator.Current;
                goto Label_003D;
            }
            return dictionary;
        }

        internal static ProviderCommandInfo Create(Command command, Node node) => 
            Create(command, node, new List<ProviderCommandInfo>());

        internal static ProviderCommandInfo Create(Command command, Node node, List<ProviderCommandInfo> children)
        {
            PhysicalProjectOp projectOp = node.Op as PhysicalProjectOp;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(projectOp != null, "Expected root Op to be a physical Project");
            DbCommandTree commandTree = CTreeGenerator.Generate(command, node);
            DbQueryCommandTree tree2 = commandTree as DbQueryCommandTree;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(tree2 != null, "null query command tree");
            CollectionType edmType = TypeHelpers.GetEdmType<CollectionType>(tree2.Query.ResultType);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsRowType(edmType.TypeUsage), "command rowtype is not a record");
            BuildOutputVarMap(projectOp, edmType.TypeUsage);
            return new ProviderCommandInfo(commandTree, children);
        }
    }
}

