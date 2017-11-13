namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Query.InternalTrees;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class VarRemapper : BasicOpVisitor
    {
        protected Command m_command;
        private HashSet<Var> m_hiddenMappingKeys;
        protected Dictionary<Var, Var> m_varMap;

        internal VarRemapper(Command command) : this(command, new Dictionary<Var, Var>())
        {
        }

        internal VarRemapper(Command command, Dictionary<Var, Var> varMap)
        {
            this.m_command = command;
            this.m_varMap = varMap;
        }

        internal void AddMapping(Var oldVar, Var newVar)
        {
            this.m_varMap[oldVar] = newVar;
        }

        protected void HideMappingKey(Var var)
        {
            if (this.m_hiddenMappingKeys == null)
            {
                this.m_hiddenMappingKeys = new HashSet<Var>();
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_hiddenMappingKeys.Add(var), "The VarRemapper is in an inconsistent state. HideMappingKey called over a map that has already been hidden.");
        }

        private bool IsHiddenMappingKey(Var v) => 
            ((this.m_hiddenMappingKeys != null) && this.m_hiddenMappingKeys.Contains(v));

        private void Map(List<SortKey> sortKeys)
        {
            VarVec vec = this.m_command.CreateVarVec();
            bool flag = false;
            foreach (SortKey key in sortKeys)
            {
                key.Var = this.Map(key.Var);
                if (vec.IsSet(key.Var))
                {
                    flag = true;
                }
                vec.Set(key.Var);
            }
            if (flag)
            {
                List<SortKey> list = new List<SortKey>(sortKeys);
                sortKeys.Clear();
                vec.Clear();
                foreach (SortKey key2 in list)
                {
                    if (!vec.IsSet(key2.Var))
                    {
                        sortKeys.Add(key2);
                    }
                    vec.Set(key2.Var);
                }
            }
        }

        private Var Map(Var v)
        {
            Var var;
            while (!this.IsHiddenMappingKey(v) && this.m_varMap.TryGetValue(v, out var))
            {
                v = var;
            }
            return v;
        }

        private void Map(VarList varList)
        {
            VarList collection = Command.CreateVarList(this.MapVars(varList));
            varList.Clear();
            varList.AddRange(collection);
        }

        private void Map(VarMap varMap)
        {
            VarMap map = new VarMap();
            foreach (KeyValuePair<Var, Var> pair in varMap)
            {
                Var var = this.Map(pair.Value);
                map.Add(pair.Key, var);
            }
            varMap.Clear();
            foreach (KeyValuePair<Var, Var> pair2 in map)
            {
                varMap.Add(pair2.Key, pair2.Value);
            }
        }

        private void Map(VarVec vec)
        {
            VarVec other = this.m_command.CreateVarVec(this.MapVars(vec));
            vec.InitFrom(other);
        }

        private IEnumerable<Var> MapVars(IEnumerable<Var> vars)
        {
            foreach (Var iteratorVariable0 in vars)
            {
                yield return this.Map(iteratorVariable0);
            }
        }

        internal virtual void RemapNode(Node node)
        {
            if (this.m_varMap.Count != 0)
            {
                this.VisitNode(node);
            }
        }

        internal virtual void RemapSubtree(Node subTree)
        {
            this.RemapSubtree(subTree, new VisitNodeDelegate(this.RemapNode));
        }

        protected void RemapSubtree(Node subTree, VisitNodeDelegate remapNodeDelegate)
        {
            if (this.m_varMap.Count != 0)
            {
                foreach (Node node in subTree.Children)
                {
                    this.RemapSubtree(node);
                }
                remapNodeDelegate(subTree);
                this.m_command.RecomputeNodeInfo(subTree);
            }
        }

        internal VarList RemapVarList(VarList varList) => 
            Command.CreateVarList(this.MapVars(varList));

        internal static VarList RemapVarList(Command command, Dictionary<Var, Var> varMap, VarList varList)
        {
            VarRemapper remapper = new VarRemapper(command, varMap);
            return remapper.RemapVarList(varList);
        }

        protected void UnhideMappingKey(Var var)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_hiddenMappingKeys.Remove(var), "The VarRemapper is in an inconsistent state. UnhideMappingKey called over a map that is not hidden.");
        }

        public override void Visit(DistinctOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
            this.Map(op.Keys);
        }

        public override void Visit(GroupByOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
            this.Map(op.Outputs);
            this.Map(op.Keys);
        }

        public override void Visit(PhysicalProjectOp op, Node n)
        {
            this.VisitPhysicalOpDefault(op, n);
            this.Map(op.Outputs);
            SimpleCollectionColumnMap columnMap = (SimpleCollectionColumnMap) ColumnMapTranslator.Translate(op.ColumnMap, this.m_varMap);
            n.Op = this.m_command.CreatePhysicalProjectOp(op.Outputs, columnMap);
        }

        public override void Visit(ProjectOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
            this.Map(op.Outputs);
        }

        public override void Visit(UnnestOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
            Var v = this.Map(op.Var);
            if (v != op.Var)
            {
                n.Op = this.m_command.CreateUnnestOp(v, op.Table);
            }
        }

        public override void Visit(VarRefOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
            Var v = this.Map(op.Var);
            if (v != op.Var)
            {
                n.Op = this.m_command.CreateVarRefOp(v);
            }
        }

        protected override void VisitDefault(Node n)
        {
        }

        protected override void VisitNestOp(NestBaseOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        protected override void VisitSetOp(SetOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
            this.Map(op.VarMap[0]);
            this.Map(op.VarMap[1]);
        }

        protected override void VisitSortOp(SortBaseOp op, Node n)
        {
            this.VisitRelOpDefault(op, n);
            this.Map(op.Keys);
        }


        protected delegate void VisitNodeDelegate(Node n);
    }
}

