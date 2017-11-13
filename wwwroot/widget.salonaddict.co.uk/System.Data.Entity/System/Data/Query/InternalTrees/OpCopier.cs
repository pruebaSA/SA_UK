namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class OpCopier : BasicOpVisitorOfNode
    {
        protected Command m_destCmd;
        private Command m_srcCmd;
        protected VarMap m_varMap;

        protected OpCopier(Command cmd) : this(cmd, cmd)
        {
        }

        private OpCopier(Command destCommand, Command sourceCommand)
        {
            this.m_srcCmd = sourceCommand;
            this.m_destCmd = destCommand;
            this.m_varMap = new VarMap();
        }

        private List<SortKey> Copy(List<SortKey> sortKeys)
        {
            List<SortKey> list = new List<SortKey>();
            foreach (SortKey key in sortKeys)
            {
                list.Add(this.Copy(key));
            }
            return list;
        }

        private ColumnMap Copy(ColumnMap columnMap) => 
            ColumnMapCopier.Copy(columnMap, this.m_varMap);

        private SortKey Copy(SortKey sortKey) => 
            Command.CreateSortKey(this.GetMappedVar(sortKey.Var), sortKey.AscendingSort, sortKey.Collation);

        private VarList Copy(VarList varList) => 
            Command.CreateVarList(this.MapVars(varList));

        private VarVec Copy(VarVec vars) => 
            this.m_destCmd.CreateVarVec(this.MapVars(vars));

        internal static List<SortKey> Copy(Command cmd, List<SortKey> sortKeys)
        {
            OpCopier copier = new OpCopier(cmd);
            return copier.Copy(sortKeys);
        }

        internal static Node Copy(Command cmd, Node n)
        {
            VarMap map;
            return Copy(cmd, n, out map);
        }

        internal static Node Copy(Command cmd, Node n, out VarMap varMap)
        {
            OpCopier copier = new OpCopier(cmd);
            Node node = copier.CopyNode(n);
            varMap = copier.m_varMap;
            return node;
        }

        internal static Node Copy(Command cmd, Node node, VarList varList, out VarList newVarList)
        {
            VarMap map;
            Node node2 = Copy(cmd, node, out map);
            newVarList = Command.CreateVarList();
            foreach (Var var in varList)
            {
                Var item = map[var];
                newVarList.Add(item);
            }
            return node2;
        }

        private Node CopyDefault(Op op, Node original) => 
            this.m_destCmd.CreateNode(op, this.ProcessChildren(original));

        protected Node CopyNode(Node n) => 
            n.Op.Accept<Node>(this, n);

        private Node CopySetOp(SetOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            VarMap leftMap = new VarMap();
            VarMap rightMap = new VarMap();
            foreach (KeyValuePair<Var, Var> pair in op.VarMap[0])
            {
                Var mappedVar = this.m_destCmd.CreateSetOpVar(pair.Key.Type);
                this.SetMappedVar(pair.Key, mappedVar);
                leftMap.Add(mappedVar, this.GetMappedVar(pair.Value));
                rightMap.Add(mappedVar, this.GetMappedVar(op.VarMap[1][pair.Key]));
            }
            SetOp op2 = null;
            switch (op.OpType)
            {
                case OpType.UnionAll:
                {
                    Var branchDiscriminator = ((UnionAllOp) op).BranchDiscriminator;
                    if (branchDiscriminator != null)
                    {
                        branchDiscriminator = this.GetMappedVar(branchDiscriminator);
                    }
                    op2 = this.m_destCmd.CreateUnionAllOp(leftMap, rightMap, branchDiscriminator);
                    break;
                }
                case OpType.Intersect:
                    op2 = this.m_destCmd.CreateIntersectOp(leftMap, rightMap);
                    break;

                case OpType.Except:
                    op2 = this.m_destCmd.CreateExceptOp(leftMap, rightMap);
                    break;
            }
            return this.m_destCmd.CreateNode(op2, args);
        }

        private Var GetMappedVar(Var v)
        {
            Var var;
            if (this.m_varMap.TryGetValue(v, out var))
            {
                return var;
            }
            if (this.m_destCmd != this.m_srcCmd)
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.UnknownVar, 6);
            }
            return v;
        }

        private void MapTable(Table newTable, Table oldTable)
        {
            for (int i = 0; i < oldTable.Columns.Count; i++)
            {
                this.SetMappedVar(oldTable.Columns[i], newTable.Columns[i]);
            }
        }

        private IEnumerable<Var> MapVars(IEnumerable<Var> vars)
        {
            foreach (Var iteratorVariable0 in vars)
            {
                Var mappedVar = this.GetMappedVar(iteratorVariable0);
                yield return mappedVar;
            }
        }

        private List<Node> ProcessChildren(Node n)
        {
            List<Node> list = new List<Node>();
            foreach (Node node in n.Children)
            {
                list.Add(this.CopyNode(node));
            }
            return list;
        }

        private void SetMappedVar(Var v, Var mappedVar)
        {
            this.m_varMap.Add(v, mappedVar);
        }

        public override Node Visit(AggregateOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateAggregateOp(op.AggFunc, op.IsDistinctAggregate), n);

        public override Node Visit(ArithmeticOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateArithmeticOp(op.OpType, op.Type), n);

        public override Node Visit(CaseOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateCaseOp(op.Type), n);

        public override Node Visit(CastOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateCastOp(op.Type), n);

        public override Node Visit(CollectOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateCollectOp(op.Type), n);

        public override Node Visit(ComparisonOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateComparisonOp(op.OpType), n);

        public override Node Visit(ConditionalOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateConditionalOp(op.OpType), n);

        public override Node Visit(ConstantOp op, Node n)
        {
            ConstantBaseOp op2 = this.m_destCmd.CreateConstantOp(op.Type, op.Value);
            return this.m_destCmd.CreateNode(op2);
        }

        public override Node Visit(ConstantPredicateOp op, Node n) => 
            this.m_destCmd.CreateNode(this.m_destCmd.CreateConstantPredicateOp(op.Value));

        public override Node Visit(ConstrainedSortOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            List<SortKey> sortKeys = this.Copy(op.Keys);
            ConstrainedSortOp op2 = this.m_destCmd.CreateConstrainedSortOp(sortKeys, op.WithTies);
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(CrossApplyOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateCrossApplyOp(), n);

        public override Node Visit(CrossJoinOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateCrossJoinOp(), n);

        public override Node Visit(DerefOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateDerefOp(op.Type), n);

        public override Node Visit(DiscriminatedNewEntityOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateDiscriminatedNewEntityOp(op.Type, op.DiscriminatorMap, op.EntitySet, op.RelationshipProperties), n);

        public override Node Visit(DistinctOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            VarVec keyVars = this.Copy(op.Keys);
            DistinctOp op2 = this.m_destCmd.CreateDistinctOp(keyVars);
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(ElementOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateElementOp(op.Type), n);

        public override Node Visit(ExceptOp op, Node n) => 
            this.CopySetOp(op, n);

        public override Node Visit(ExistsOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateExistsOp(), n);

        public override Node Visit(FilterOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateFilterOp(), n);

        public override Node Visit(FullOuterJoinOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateFullOuterJoinOp(), n);

        public override Node Visit(FunctionOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateFunctionOp(op.Function), n);

        public override Node Visit(GetEntityRefOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateGetEntityRefOp(op.Type), n);

        public override Node Visit(GetRefKeyOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateGetRefKeyOp(op.Type), n);

        public override Node Visit(GroupByOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            GroupByOp op2 = this.m_destCmd.CreateGroupByOp(this.Copy(op.Keys), this.Copy(op.Outputs));
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(InnerJoinOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateInnerJoinOp(), n);

        public override Node Visit(InternalConstantOp op, Node n)
        {
            InternalConstantOp op2 = this.m_destCmd.CreateInternalConstantOp(op.Type, op.Value);
            return this.m_destCmd.CreateNode(op2);
        }

        public override Node Visit(IntersectOp op, Node n) => 
            this.CopySetOp(op, n);

        public override Node Visit(IsOfOp op, Node n)
        {
            if (op.IsOfOnly)
            {
                return this.CopyDefault(this.m_destCmd.CreateIsOfOnlyOp(op.IsOfType), n);
            }
            return this.CopyDefault(this.m_destCmd.CreateIsOfOp(op.IsOfType), n);
        }

        public override Node Visit(LeftOuterJoinOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateLeftOuterJoinOp(), n);

        public override Node Visit(LikeOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateLikeOp(), n);

        public override Node Visit(MultiStreamNestOp op, Node n) => 
            this.VisitNestOp(n);

        public override Node Visit(NavigateOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateNavigateOp(op.Type, op.RelProperty), n);

        public override Node Visit(NewEntityOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateNewEntityOp(op.Type, op.RelationshipProperties, op.EntitySet), n);

        public override Node Visit(NewInstanceOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateNewInstanceOp(op.Type), n);

        public override Node Visit(NewMultisetOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateNewMultisetOp(op.Type), n);

        public override Node Visit(NewRecordOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateNewRecordOp(op.Type), n);

        public override Node Visit(NullOp op, Node n) => 
            this.m_destCmd.CreateNode(this.m_destCmd.CreateNullOp(op.Type));

        public override Node Visit(Op op, Node n)
        {
            throw new NotSupportedException(Strings.Iqt_General_UnsupportedOp(op.GetType().FullName));
        }

        public override Node Visit(OuterApplyOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateOuterApplyOp(), n);

        public override Node Visit(PhysicalProjectOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            VarList outputVars = this.Copy(op.Outputs);
            SimpleCollectionColumnMap columnMap = this.Copy(op.ColumnMap) as SimpleCollectionColumnMap;
            PhysicalProjectOp op2 = this.m_destCmd.CreatePhysicalProjectOp(outputVars, columnMap);
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(ProjectOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            VarVec vars = this.Copy(op.Outputs);
            ProjectOp op2 = this.m_destCmd.CreateProjectOp(vars);
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(PropertyOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreatePropertyOp(op.PropertyInfo), n);

        public override Node Visit(RefOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateRefOp(op.EntitySet, op.Type), n);

        public override Node Visit(RelPropertyOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateRelPropertyOp(op.PropertyInfo), n);

        public override Node Visit(ScanTableOp op, Node n)
        {
            ScanTableOp op2 = this.m_destCmd.CreateScanTableOp(op.Table.TableMetadata);
            this.MapTable(op2.Table, op.Table);
            return this.m_destCmd.CreateNode(op2);
        }

        public override Node Visit(ScanViewOp op, Node n)
        {
            ScanViewOp op2 = this.m_destCmd.CreateScanViewOp(op.Table.TableMetadata);
            this.MapTable(op2.Table, op.Table);
            List<Node> args = this.ProcessChildren(n);
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(SingleRowOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateSingleRowOp(), n);

        public override Node Visit(SingleRowTableOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateSingleRowTableOp(), n);

        public override Node Visit(SingleStreamNestOp op, Node n) => 
            this.VisitNestOp(n);

        public override Node Visit(SoftCastOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateSoftCastOp(op.Type), n);

        public override Node Visit(SortOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            List<SortKey> sortKeys = this.Copy(op.Keys);
            SortOp op2 = this.m_destCmd.CreateSortOp(sortKeys);
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(TreatOp op, Node n)
        {
            TreatOp op2 = op.IsFakeTreat ? this.m_destCmd.CreateFakeTreatOp(op.Type) : this.m_destCmd.CreateTreatOp(op.Type);
            return this.CopyDefault(op2, n);
        }

        public override Node Visit(UnionAllOp op, Node n) => 
            this.CopySetOp(op, n);

        public override Node Visit(UnnestOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            Var mappedVar = this.GetMappedVar(op.Var);
            Table t = this.m_destCmd.CreateTableInstance(op.Table.TableMetadata);
            UnnestOp op2 = this.m_destCmd.CreateUnnestOp(mappedVar, t);
            this.MapTable(op2.Table, op.Table);
            return this.m_destCmd.CreateNode(op2, args);
        }

        public override Node Visit(VarDefListOp op, Node n) => 
            this.CopyDefault(this.m_destCmd.CreateVarDefListOp(), n);

        public override Node Visit(VarDefOp op, Node n)
        {
            List<Node> args = this.ProcessChildren(n);
            Var mappedVar = this.m_destCmd.CreateComputedVar(op.Var.Type);
            this.SetMappedVar(op.Var, mappedVar);
            return this.m_destCmd.CreateNode(this.m_destCmd.CreateVarDefOp(mappedVar), args);
        }

        public override Node Visit(VarRefOp op, Node n)
        {
            Var var;
            if (!this.m_varMap.TryGetValue(op.Var, out var))
            {
                var = op.Var;
            }
            return this.m_destCmd.CreateNode(this.m_destCmd.CreateVarRefOp(var));
        }

        private Node VisitNestOp(Node n)
        {
            NestBaseOp op = n.Op as NestBaseOp;
            SingleStreamNestOp op2 = op as SingleStreamNestOp;
            List<Node> args = this.ProcessChildren(n);
            Var discriminatorVar = null;
            if (op2 != null)
            {
                discriminatorVar = this.GetMappedVar(op2.Discriminator);
            }
            List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
            foreach (CollectionInfo info in op.CollectionInfo)
            {
                ColumnMap columnMap = this.Copy(info.ColumnMap);
                Var mappedVar = this.m_destCmd.CreateComputedVar(info.CollectionVar.Type);
                this.SetMappedVar(info.CollectionVar, mappedVar);
                VarList flattenedElementVars = this.Copy(info.FlattenedElementVars);
                VarVec keys = this.Copy(info.Keys);
                List<SortKey> sortKeys = this.Copy(info.SortKeys);
                CollectionInfo item = Command.CreateCollectionInfo(mappedVar, columnMap, flattenedElementVars, keys, sortKeys, info.DiscriminatorValue);
                collectionInfoList.Add(item);
            }
            VarVec outputVars = this.Copy(op.Outputs);
            NestBaseOp op3 = null;
            List<SortKey> prefixSortKeys = this.Copy(op.PrefixSortKeys);
            if (op2 != null)
            {
                VarVec vec3 = this.Copy(op2.Keys);
                List<SortKey> postfixSortKeys = this.Copy(op2.PostfixSortKeys);
                op3 = this.m_destCmd.CreateSingleStreamNestOp(vec3, prefixSortKeys, postfixSortKeys, outputVars, collectionInfoList, discriminatorVar);
            }
            else
            {
                op3 = this.m_destCmd.CreateMultiStreamNestOp(prefixSortKeys, outputVars, collectionInfoList);
            }
            return this.m_destCmd.CreateNode(op3, args);
        }

    }
}

