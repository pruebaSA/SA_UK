namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class CodeGen
    {
        private System.Data.Query.PlanCompiler.PlanCompiler m_compilerState;
        private List<Node> m_subCommands;

        private CodeGen(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            this.m_compilerState = compilerState;
        }

        private ColumnMap BuildResultColumnMap(PhysicalProjectOp projectOp)
        {
            Dictionary<Var, KeyValuePair<int, int>> varToCommandColumnMap = this.BuildVarMap();
            return ColumnMapTranslator.Translate(projectOp.ColumnMap, varToCommandColumnMap);
        }

        private Dictionary<Var, KeyValuePair<int, int>> BuildVarMap()
        {
            Dictionary<Var, KeyValuePair<int, int>> dictionary = new Dictionary<Var, KeyValuePair<int, int>>();
            int key = 0;
            foreach (Node node in this.m_subCommands)
            {
                PhysicalProjectOp op = (PhysicalProjectOp) node.Op;
                int num2 = 0;
                foreach (Var var in op.Outputs)
                {
                    KeyValuePair<int, int> pair = new KeyValuePair<int, int>(key, num2);
                    dictionary[var] = pair;
                    num2++;
                }
                key++;
            }
            return dictionary;
        }

        private void Process(out List<ProviderCommandInfo> childCommands, out ColumnMap resultColumnMap, out int columnCount)
        {
            PhysicalProjectOp projectOp = (PhysicalProjectOp) this.Command.Root.Op;
            this.m_subCommands = new List<Node>(new Node[] { this.Command.Root });
            childCommands = new List<ProviderCommandInfo>(new ProviderCommandInfo[] { ProviderCommandInfoUtils.Create(this.Command, this.Command.Root) });
            resultColumnMap = this.BuildResultColumnMap(projectOp);
            columnCount = projectOp.Outputs.Count;
        }

        internal static void Process(System.Data.Query.PlanCompiler.PlanCompiler compilerState, out List<ProviderCommandInfo> childCommands, out ColumnMap resultColumnMap, out int columnCount)
        {
            new CodeGen(compilerState).Process(out childCommands, out resultColumnMap, out columnCount);
        }

        private System.Data.Query.InternalTrees.Command Command =>
            this.m_compilerState.Command;
    }
}

