namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class NestPullup : BasicOpVisitorOfNode
    {
        private System.Data.Query.PlanCompiler.PlanCompiler m_compilerState;
        private Dictionary<Var, Node> m_definingNodeMap = new Dictionary<Var, Node>();
        private Dictionary<Var, Var> m_varRefMap = new Dictionary<Var, Var>();
        private VarRemapper m_varRemapper;

        private NestPullup(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            this.m_compilerState = compilerState;
            this.m_varRemapper = new VarRemapper(compilerState.Command);
        }

        private Node ApplyOpJoinOp(Op op, Node n)
        {
            this.VisitChildren(n);
            int num = 0;
            foreach (Node node in n.Children)
            {
                if (node.Op is NestBaseOp)
                {
                    num++;
                    if (OpType.SingleStreamNest == node.Op.OpType)
                    {
                        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.JoinOverSingleStreamNest);
                    }
                }
            }
            if (num == 0)
            {
                return n;
            }
            foreach (Node node2 in n.Children)
            {
                if ((op.OpType != OpType.MultiStreamNest) && node2.Op.IsRelOp)
                {
                    KeyVec vec = this.Command.PullupKeys(node2);
                    if ((vec == null) || vec.NoKeys)
                    {
                        throw EntityUtil.KeysRequiredForJoinOverNest(op);
                    }
                }
            }
            List<Node> args = new List<Node>();
            List<Node> list2 = new List<Node>();
            List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
            foreach (Node node3 in n.Children)
            {
                if (node3.Op.OpType == OpType.MultiStreamNest)
                {
                    collectionInfoList.AddRange(((MultiStreamNestOp) node3.Op).CollectionInfo);
                    if ((op.OpType == OpType.FullOuterJoin) || (((op.OpType == OpType.LeftOuterJoin) || (op.OpType == OpType.OuterApply)) && (n.Child1.Op.OpType == OpType.MultiStreamNest)))
                    {
                        Var internalConstantVar = null;
                        list2.Add(this.AugmentNodeWithInternalConstant(node3.Child0, 1, out internalConstantVar));
                        for (int i = 1; i < node3.Children.Count; i++)
                        {
                            Node node7;
                            Node input = node3.Children[i];
                            Node node5 = null;
                            while (input.Op.OpType == OpType.MultiStreamNest)
                            {
                                node5 = input;
                                input = input.Child0;
                            }
                            Node node6 = this.CapWithIsNotNullFilter(input, internalConstantVar);
                            if (node5 != null)
                            {
                                node5.Child0 = node6;
                                node7 = node3.Children[i];
                            }
                            else
                            {
                                node7 = node6;
                            }
                            args.Add(node7);
                        }
                    }
                    else
                    {
                        list2.Add(node3.Child0);
                        for (int j = 1; j < node3.Children.Count; j++)
                        {
                            args.Add(node3.Children[j]);
                        }
                    }
                }
                else
                {
                    list2.Add(node3);
                }
            }
            Node item = this.Command.CreateNode(op, list2);
            args.Insert(0, item);
            ExtendedNodeInfo extendedNodeInfo = item.GetExtendedNodeInfo(this.Command);
            VarVec outputVars = this.Command.CreateVarVec(extendedNodeInfo.Definitions);
            foreach (CollectionInfo info2 in collectionInfoList)
            {
                outputVars.Set(info2.CollectionVar);
            }
            NestBaseOp op3 = this.Command.CreateMultiStreamNestOp(new List<SortKey>(), outputVars, collectionInfoList);
            return this.Command.CreateNode(op3, args);
        }

        private Node AugmentNodeWithInternalConstant(Node input, int internalConstantValue, out Var internalConstantVar)
        {
            InternalConstantOp op = this.Command.CreateInternalConstantOp(this.Command.IntegerType, internalConstantValue);
            Node definingExpr = this.Command.CreateNode(op);
            Node node2 = this.Command.CreateVarDefListNode(definingExpr, out internalConstantVar);
            ExtendedNodeInfo extendedNodeInfo = this.Command.GetExtendedNodeInfo(input);
            VarVec vars = this.Command.CreateVarVec(extendedNodeInfo.Definitions);
            vars.Set(internalConstantVar);
            ProjectOp op2 = this.Command.CreateProjectOp(vars);
            return this.Command.CreateNode(op2, input, node2);
        }

        private Node BuildSortForNestElimination(SingleStreamNestOp ssnOp, Node nestNode)
        {
            List<SortKey> sortKeys = this.BuildSortKeyList(ssnOp);
            if (sortKeys.Count > 0)
            {
                SortOp op = this.Command.CreateSortOp(sortKeys);
                return this.Command.CreateNode(op, nestNode.Child0);
            }
            return nestNode.Child0;
        }

        private List<SortKey> BuildSortKeyList(SingleStreamNestOp ssnOp)
        {
            VarVec vec = this.Command.CreateVarVec();
            List<SortKey> list = new List<SortKey>();
            foreach (SortKey key in ssnOp.PrefixSortKeys)
            {
                if (!vec.IsSet(key.Var))
                {
                    vec.Set(key.Var);
                    list.Add(key);
                }
            }
            foreach (Var var in ssnOp.Keys)
            {
                if (!vec.IsSet(var))
                {
                    vec.Set(var);
                    SortKey item = System.Data.Query.InternalTrees.Command.CreateSortKey(var);
                    list.Add(item);
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!vec.IsSet(ssnOp.Discriminator), "prefix sort on discriminator?");
            list.Add(System.Data.Query.InternalTrees.Command.CreateSortKey(ssnOp.Discriminator));
            foreach (SortKey key3 in ssnOp.PostfixSortKeys)
            {
                if (!vec.IsSet(key3.Var))
                {
                    vec.Set(key3.Var);
                    list.Add(key3);
                }
            }
            return list;
        }

        private Node BuildUnionAllSubqueryForNestOp(NestBaseOp nestOp, Node nestNode, VarList drivingNodeVars, VarList discriminatorVarList, out Var discriminatorVar, out List<Dictionary<Var, Var>> varMapList)
        {
            Node node = nestNode.Child0;
            Node node2 = null;
            VarList leftVars = null;
            for (int i = 1; i < nestNode.Children.Count; i++)
            {
                VarList list2;
                Node node3;
                VarList flattenedElementVars;
                Op op;
                if (i > 1)
                {
                    node3 = OpCopier.Copy(this.Command, node, drivingNodeVars, out list2);
                    VarRemapper remapper = new VarRemapper(this.Command);
                    for (int m = 0; m < drivingNodeVars.Count; m++)
                    {
                        remapper.AddMapping(drivingNodeVars[m], list2[m]);
                    }
                    remapper.RemapSubtree(nestNode.Children[i]);
                    flattenedElementVars = remapper.RemapVarList(nestOp.CollectionInfo[i - 1].FlattenedElementVars);
                    op = this.Command.CreateCrossApplyOp();
                }
                else
                {
                    node3 = node;
                    list2 = drivingNodeVars;
                    flattenedElementVars = nestOp.CollectionInfo[i - 1].FlattenedElementVars;
                    op = this.Command.CreateOuterApplyOp();
                }
                Node node4 = this.Command.CreateNode(op, node3, nestNode.Children[i]);
                List<Node> args = new List<Node>();
                VarList v = System.Data.Query.InternalTrees.Command.CreateVarList();
                v.Add(discriminatorVarList[i]);
                v.AddRange(list2);
                for (int k = 1; k < nestNode.Children.Count; k++)
                {
                    CollectionInfo info = nestOp.CollectionInfo[k - 1];
                    if (i == k)
                    {
                        v.AddRange(flattenedElementVars);
                    }
                    else
                    {
                        foreach (Var var in info.FlattenedElementVars)
                        {
                            Var var2;
                            NullOp op2 = this.Command.CreateNullOp(var.Type);
                            Node definingExpr = this.Command.CreateNode(op2);
                            Node item = this.Command.CreateVarDefNode(definingExpr, out var2);
                            args.Add(item);
                            v.Add(var2);
                        }
                    }
                }
                Node node7 = this.Command.CreateNode(this.Command.CreateVarDefListOp(), args);
                VarVec vars = this.Command.CreateVarVec(v);
                ProjectOp op3 = this.Command.CreateProjectOp(vars);
                Node node8 = this.Command.CreateNode(op3, node4, node7);
                if (node2 == null)
                {
                    node2 = node8;
                    leftVars = v;
                }
                else
                {
                    VarMap leftMap = new VarMap();
                    VarMap rightMap = new VarMap();
                    for (int n = 0; n < leftVars.Count; n++)
                    {
                        Var key = this.Command.CreateSetOpVar(leftVars[n].Type);
                        leftMap.Add(key, leftVars[n]);
                        rightMap.Add(key, v[n]);
                    }
                    UnionAllOp op4 = this.Command.CreateUnionAllOp(leftMap, rightMap);
                    node2 = this.Command.CreateNode(op4, node2, node8);
                    leftVars = GetUnionOutputs(op4, leftVars);
                }
            }
            varMapList = new List<Dictionary<Var, Var>>();
            IEnumerator<Var> enumerator = leftVars.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 4);
            }
            discriminatorVar = enumerator.Current;
            for (int j = 0; j < nestNode.Children.Count; j++)
            {
                Dictionary<Var, Var> dictionary = new Dictionary<Var, Var>();
                VarList list6 = (j == 0) ? drivingNodeVars : nestOp.CollectionInfo[j - 1].FlattenedElementVars;
                foreach (Var var4 in list6)
                {
                    if (!enumerator.MoveNext())
                    {
                        throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 5);
                    }
                    dictionary[var4] = enumerator.Current;
                }
                varMapList.Add(dictionary);
            }
            if (enumerator.MoveNext())
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 6);
            }
            return node2;
        }

        private Node CapWithIsNotNullFilter(Node input, Var var)
        {
            Node node = this.Command.CreateNode(this.Command.CreateVarRefOp(var));
            Node node2 = this.Command.CreateNode(this.Command.CreateConditionalOp(OpType.Not), this.Command.CreateNode(this.Command.CreateConditionalOp(OpType.IsNull), node));
            return this.Command.CreateNode(this.Command.CreateFilterOp(), input, node2);
        }

        private List<SortKey> ConsolidateSortKeys(List<SortKey> sortKeyList1, List<SortKey> sortKeyList2)
        {
            VarVec vec = this.Command.CreateVarVec();
            List<SortKey> list = new List<SortKey>();
            foreach (SortKey key in sortKeyList1)
            {
                if (!vec.IsSet(key.Var))
                {
                    vec.Set(key.Var);
                    list.Add(System.Data.Query.InternalTrees.Command.CreateSortKey(key.Var, key.AscendingSort, key.Collation));
                }
            }
            foreach (SortKey key2 in sortKeyList2)
            {
                if (!vec.IsSet(key2.Var))
                {
                    vec.Set(key2.Var);
                    list.Add(System.Data.Query.InternalTrees.Command.CreateSortKey(key2.Var, key2.AscendingSort, key2.Collation));
                }
            }
            return list;
        }

        private void ConvertToNestOpInput(Node physicalProjectNode, Var collectionVar, List<CollectionInfo> collectionInfoList, List<Node> collectionNodes, VarVec externalReferences, VarVec collectionReferences)
        {
            externalReferences.Or(this.Command.GetNodeInfo(physicalProjectNode).ExternalReferences);
            Node n = physicalProjectNode.Child0;
            PhysicalProjectOp op = (PhysicalProjectOp) physicalProjectNode.Op;
            VarList v = System.Data.Query.InternalTrees.Command.CreateVarList(op.Outputs);
            VarVec vec = this.Command.CreateVarVec(v);
            List<SortKey> sortKeys = null;
            if (OpType.Sort == n.Op.OpType)
            {
                SortOp op2 = (SortOp) n.Op;
                sortKeys = OpCopier.Copy(this.Command, op2.Keys);
                foreach (SortKey key in sortKeys)
                {
                    if (!vec.IsSet(key.Var))
                    {
                        v.Add(key.Var);
                        vec.Set(key.Var);
                    }
                }
            }
            else
            {
                sortKeys = new List<SortKey>();
            }
            VarVec keys = this.Command.CreateVarVec();
            foreach (Var var in this.Command.GetExtendedNodeInfo(n).Keys.KeyVars)
            {
                if (vec.IsSet(var))
                {
                    keys.Set(var);
                }
            }
            CollectionInfo item = System.Data.Query.InternalTrees.Command.CreateCollectionInfo(collectionVar, op.ColumnMap.Element, v, keys, sortKeys, null);
            collectionInfoList.Add(item);
            collectionNodes.Add(n);
            collectionReferences.Set(collectionVar);
        }

        private Node ConvertToSingleStreamNest(Node nestNode, Dictionary<Var, ColumnMap> varRefReplacementMap, out VarList flattenedOutputVarList, out SimpleColumnMap[] parentKeyColumnMaps)
        {
            VarList list3;
            List<List<SortKey>> list4;
            List<Dictionary<Var, Var>> list5;
            Var var2;
            MultiStreamNestOp nestOp = (MultiStreamNestOp) nestNode.Op;
            for (int i = 1; i < nestNode.Children.Count; i++)
            {
                Node node = nestNode.Children[i];
                if (node.Op.OpType == OpType.MultiStreamNest)
                {
                    VarList list;
                    SimpleColumnMap[] mapArray;
                    CollectionInfo info = nestOp.CollectionInfo[i - 1];
                    nestNode.Children[i] = this.ConvertToSingleStreamNest(node, varRefReplacementMap, out list, out mapArray);
                    ColumnMap columnMap = ColumnMapTranslator.Translate(info.ColumnMap, varRefReplacementMap);
                    VarVec vec = this.Command.CreateVarVec(((SingleStreamNestOp) nestNode.Children[i].Op).Keys);
                    nestOp.CollectionInfo[i - 1] = System.Data.Query.InternalTrees.Command.CreateCollectionInfo(info.CollectionVar, columnMap, list, vec, info.SortKeys, null);
                }
            }
            Node n = nestNode.Child0;
            KeyVec vec2 = this.Command.PullupKeys(n);
            VarVec definitions = this.Command.GetExtendedNodeInfo(n).Definitions;
            VarList drivingNodeVars = System.Data.Query.InternalTrees.Command.CreateVarList(definitions);
            if (vec2.NoKeys)
            {
                EdmFunction function;
                Var var;
                if (!this.TryGetNewGuidFunction(out function))
                {
                    throw EntityUtil.KeysRequiredForNesting();
                }
                Node node3 = this.Command.CreateVarDefListNode(this.Command.CreateNode(this.Command.CreateFunctionOp(function)), out var);
                definitions = this.Command.CreateVarVec(definitions);
                definitions.Set(var);
                drivingNodeVars.Add(var);
                vec2 = new KeyVec(this.Command);
                vec2.KeyVars.Set(var);
                n = this.Command.CreateNode(this.Command.CreateProjectOp(definitions), n, node3);
                nestNode.Child0 = n;
                VarVec vec4 = this.Command.CreateVarVec(nestOp.Outputs);
                vec4.Set(var);
                nestOp = this.Command.CreateMultiStreamNestOp(nestOp.PrefixSortKeys, vec4, nestOp.CollectionInfo);
                nestNode.Op = nestOp;
                this.Command.RecomputeNodeInfo(nestNode);
            }
            this.NormalizeNestOpInputs(nestOp, nestNode, out list3, out list4);
            Node node4 = this.BuildUnionAllSubqueryForNestOp(nestOp, nestNode, drivingNodeVars, list3, out var2, out list5);
            Dictionary<Var, Var> varMap = list5[0];
            flattenedOutputVarList = System.Data.Query.InternalTrees.Command.CreateVarList(this.RemapVars(drivingNodeVars, varMap));
            VarVec v = this.Command.CreateVarVec(flattenedOutputVarList);
            VarVec outputVars = this.Command.CreateVarVec(v);
            foreach (KeyValuePair<Var, Var> pair in varMap)
            {
                if (pair.Key != pair.Value)
                {
                    varRefReplacementMap[pair.Key] = new VarRefColumnMap(pair.Value);
                }
            }
            RemapSortKeys(nestOp.PrefixSortKeys, varMap);
            List<SortKey> postfixSortKeys = new List<SortKey>();
            List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
            VarRefColumnMap discriminator = new VarRefColumnMap(var2);
            outputVars.Set(var2);
            if (!v.IsSet(var2))
            {
                flattenedOutputVarList.Add(var2);
                v.Set(var2);
            }
            VarVec keys = this.RemapVarVec(vec2.KeyVars, varMap);
            parentKeyColumnMaps = new SimpleColumnMap[keys.Count];
            int index = 0;
            foreach (Var var3 in keys)
            {
                parentKeyColumnMaps[index] = new VarRefColumnMap(var3);
                index++;
                if (!v.IsSet(var3))
                {
                    flattenedOutputVarList.Add(var3);
                    v.Set(var3);
                }
            }
            for (int j = 1; j < nestNode.Children.Count; j++)
            {
                CollectionInfo info3 = nestOp.CollectionInfo[j - 1];
                List<SortKey> sortKeys = list4[j];
                RemapSortKeys(sortKeys, list5[j]);
                postfixSortKeys.AddRange(sortKeys);
                ColumnMap map3 = ColumnMapTranslator.Translate(info3.ColumnMap, list5[j]);
                VarList flattenedElementVars = this.RemapVarList(info3.FlattenedElementVars, list5[j]);
                VarVec vec8 = this.RemapVarVec(info3.Keys, list5[j]);
                RemapSortKeys(info3.SortKeys, list5[j]);
                CollectionInfo item = System.Data.Query.InternalTrees.Command.CreateCollectionInfo(info3.CollectionVar, map3, flattenedElementVars, vec8, info3.SortKeys, j);
                collectionInfoList.Add(item);
                foreach (Var var4 in flattenedElementVars)
                {
                    if (!v.IsSet(var4))
                    {
                        flattenedOutputVarList.Add(var4);
                        v.Set(var4);
                    }
                }
                outputVars.Set(info3.CollectionVar);
                int num4 = 0;
                SimpleColumnMap[] mapArray2 = new SimpleColumnMap[item.Keys.Count];
                foreach (Var var5 in item.Keys)
                {
                    mapArray2[num4] = new VarRefColumnMap(var5);
                    num4++;
                }
                SortKeyInfo[] infoArray = new SortKeyInfo[item.SortKeys.Count];
                for (int k = 0; k < infoArray.Length; k++)
                {
                    SortKey key = item.SortKeys[k];
                    VarRefColumnMap sortKeyColumn = new VarRefColumnMap(key.Var);
                    infoArray[k] = new SortKeyInfo(sortKeyColumn, key.AscendingSort, key.Collation);
                }
                DiscriminatedCollectionColumnMap map5 = new DiscriminatedCollectionColumnMap(TypeUtils.CreateCollectionType(item.ColumnMap.Type), item.ColumnMap.Name, item.ColumnMap, mapArray2, parentKeyColumnMaps, infoArray, discriminator, item.DiscriminatorValue);
                varRefReplacementMap[info3.CollectionVar] = map5;
            }
            SingleStreamNestOp op = this.Command.CreateSingleStreamNestOp(keys, nestOp.PrefixSortKeys, postfixSortKeys, outputVars, collectionInfoList, var2);
            return this.Command.CreateNode(op, node4);
        }

        private Node CopyCollectionVarDefinition(Node refVarDefiningNode)
        {
            VarMap map;
            Dictionary<Var, Node> dictionary;
            Node node = OpCopierTrackingCollectionVars.Copy(this.Command, refVarDefiningNode, out map, out dictionary);
            if (dictionary.Count != 0)
            {
                VarMap reverseMap = map.GetReverseMap();
                foreach (KeyValuePair<Var, Node> pair in dictionary)
                {
                    Node node2;
                    Var key = reverseMap[pair.Key];
                    if (this.m_definingNodeMap.TryGetValue(key, out node2))
                    {
                        PhysicalProjectOp op = (PhysicalProjectOp) node2.Op;
                        VarList outputVars = VarRemapper.RemapVarList(this.Command, map, op.Outputs);
                        SimpleCollectionColumnMap columnMap = (SimpleCollectionColumnMap) ColumnMapCopier.Copy(op.ColumnMap, map);
                        PhysicalProjectOp op2 = this.Command.CreatePhysicalProjectOp(outputVars, columnMap);
                        Node node3 = this.Command.CreateNode(op2, pair.Value);
                        this.m_definingNodeMap.Add(pair.Key, node3);
                    }
                }
            }
            return node;
        }

        private void EnsureReferencedVarsAreRemapped(List<Node> referencedVars)
        {
            foreach (Node node in referencedVars)
            {
                VarDefOp op = (VarDefOp) node.Op;
                Var refVar = op.Var;
                Var oldVar = this.ResolveVarReference(refVar);
                this.m_varRemapper.AddMapping(oldVar, refVar);
            }
        }

        private void EnsureReferencedVarsAreRemoved(List<Node> referencedVars, VarVec outputVars)
        {
            foreach (Node node in referencedVars)
            {
                VarDefOp op = (VarDefOp) node.Op;
                Var refVar = op.Var;
                Var newVar = this.ResolveVarReference(refVar);
                this.m_varRemapper.AddMapping(refVar, newVar);
                outputVars.Clear(refVar);
                outputVars.Set(newVar);
            }
        }

        private NestBaseOp GetNestOpWithConsolidatedSortKeys(NestBaseOp inputNestOp, List<SortKey> sortKeys)
        {
            if (inputNestOp.PrefixSortKeys.Count == 0)
            {
                foreach (SortKey key in sortKeys)
                {
                    inputNestOp.PrefixSortKeys.Add(System.Data.Query.InternalTrees.Command.CreateSortKey(key.Var, key.AscendingSort, key.Collation));
                }
                return inputNestOp;
            }
            this.Command.CreateVarVec();
            List<SortKey> prefixSortKeys = this.ConsolidateSortKeys(sortKeys, inputNestOp.PrefixSortKeys);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(inputNestOp is MultiStreamNestOp, "Unexpected SingleStreamNestOp?");
            return this.Command.CreateMultiStreamNestOp(prefixSortKeys, inputNestOp.Outputs, inputNestOp.CollectionInfo);
        }

        private static VarList GetUnionOutputs(UnionAllOp unionOp, VarList leftVars)
        {
            Dictionary<Var, Var> reverseMap = unionOp.VarMap[0].GetReverseMap();
            VarList list = System.Data.Query.InternalTrees.Command.CreateVarList();
            foreach (Var var in leftVars)
            {
                Var item = reverseMap[var];
                list.Add(item);
            }
            return list;
        }

        private static bool IsNestOpNode(Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Op.OpType != OpType.SingleStreamNest, "illegal singleStreamNest?");
            if (n.Op.OpType != OpType.SingleStreamNest)
            {
                return (n.Op.OpType == OpType.MultiStreamNest);
            }
            return true;
        }

        private Node MergeNestedNestOps(Node nestNode)
        {
            if (!IsNestOpNode(nestNode) || !IsNestOpNode(nestNode.Child0))
            {
                return nestNode;
            }
            NestBaseOp op = (NestBaseOp) nestNode.Op;
            Node node = nestNode.Child0;
            NestBaseOp op2 = (NestBaseOp) node.Op;
            VarVec vec = this.Command.CreateVarVec();
            foreach (CollectionInfo info in op.CollectionInfo)
            {
                vec.Set(info.CollectionVar);
            }
            List<Node> args = new List<Node>();
            List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
            VarVec outputVars = this.Command.CreateVarVec(op.Outputs);
            args.Add(node.Child0);
            for (int i = 1; i < node.Children.Count; i++)
            {
                CollectionInfo item = op2.CollectionInfo[i - 1];
                if (vec.IsSet(item.CollectionVar) || outputVars.IsSet(item.CollectionVar))
                {
                    collectionInfoList.Add(item);
                    args.Add(node.Children[i]);
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(outputVars.IsSet(item.CollectionVar), "collectionVar not in output Vars?");
                }
            }
            for (int j = 1; j < nestNode.Children.Count; j++)
            {
                CollectionInfo info3 = op.CollectionInfo[j - 1];
                collectionInfoList.Add(info3);
                args.Add(nestNode.Children[j]);
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(outputVars.IsSet(info3.CollectionVar), "collectionVar not in output Vars?");
            }
            List<SortKey> prefixSortKeys = this.ConsolidateSortKeys(op.PrefixSortKeys, op2.PrefixSortKeys);
            foreach (SortKey key in prefixSortKeys)
            {
                outputVars.Set(key.Var);
            }
            MultiStreamNestOp op3 = this.Command.CreateMultiStreamNestOp(prefixSortKeys, outputVars, collectionInfoList);
            Node n = this.Command.CreateNode(op3, args);
            this.Command.RecomputeNodeInfo(n);
            return n;
        }

        private Node NestingNotSupported(Op op, Node n)
        {
            this.VisitChildren(n);
            this.m_varRemapper.RemapNode(n);
            foreach (Node node in n.Children)
            {
                if (IsNestOpNode(node))
                {
                    throw EntityUtil.NestingNotSupported(op, node.Op);
                }
            }
            return n;
        }

        private void NormalizeNestOpInputs(NestBaseOp nestOp, Node nestNode, out VarList discriminatorVarList, out List<List<SortKey>> sortKeys)
        {
            discriminatorVarList = System.Data.Query.InternalTrees.Command.CreateVarList();
            discriminatorVarList.Add(null);
            sortKeys = new List<List<SortKey>>();
            sortKeys.Add(nestOp.PrefixSortKeys);
            for (int i = 1; i < nestNode.Children.Count; i++)
            {
                Var var;
                Node input = nestNode.Children[i];
                SingleStreamNestOp ssnOp = input.Op as SingleStreamNestOp;
                if (ssnOp != null)
                {
                    List<SortKey> item = this.BuildSortKeyList(ssnOp);
                    sortKeys.Add(item);
                    input = input.Child0;
                }
                else
                {
                    SortOp op = input.Op as SortOp;
                    if (op != null)
                    {
                        input = input.Child0;
                        sortKeys.Add(op.Keys);
                    }
                    else
                    {
                        sortKeys.Add(new List<SortKey>());
                    }
                }
                VarList flattenedElementVars = nestOp.CollectionInfo[i - 1].FlattenedElementVars;
                foreach (SortKey key in sortKeys[i])
                {
                    if (!flattenedElementVars.Contains(key.Var))
                    {
                        flattenedElementVars.Add(key.Var);
                    }
                }
                Node node2 = this.AugmentNodeWithInternalConstant(input, i, out var);
                nestNode.Children[i] = node2;
                discriminatorVarList.Add(var);
            }
        }

        private void Process()
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.Command.Root.Op.OpType == OpType.PhysicalProject, "root node is not physicalProject?");
            this.Command.Root = base.VisitNode(this.Command.Root);
        }

        internal static void Process(System.Data.Query.PlanCompiler.PlanCompiler compilerState)
        {
            new NestPullup(compilerState).Process();
        }

        private Node ProjectOpCase1(Node projectNode)
        {
            ProjectOp op = (ProjectOp) projectNode.Op;
            List<CollectionInfo> collectionInfoList = new List<CollectionInfo>();
            List<Node> args = new List<Node>();
            List<Node> collectionNodes = new List<Node>();
            VarVec externalReferences = this.Command.CreateVarVec();
            VarVec collectionReferences = this.Command.CreateVarVec();
            List<Node> collection = new List<Node>();
            List<Node> referencedVars = new List<Node>();
            foreach (Node node in projectNode.Child1.Children)
            {
                VarDefOp op2 = (VarDefOp) node.Op;
                Node node2 = node.Child0;
                if (OpType.Collect == node2.Op.OpType)
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(node2.HasChild0, "collect without input?");
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(OpType.PhysicalProject == node2.Child0.Op.OpType, "collect without physicalProject?");
                    Node node3 = node2.Child0;
                    this.m_definingNodeMap.Add(op2.Var, node3);
                    this.ConvertToNestOpInput(node3, op2.Var, collectionInfoList, collectionNodes, externalReferences, collectionReferences);
                }
                else if (OpType.VarRef == node2.Op.OpType)
                {
                    Node node4;
                    Var key = this.ResolveVarReference(op2.Var);
                    if (this.m_definingNodeMap.TryGetValue(key, out node4))
                    {
                        node4 = this.CopyCollectionVarDefinition(node4);
                        this.m_definingNodeMap.Add(op2.Var, node4);
                        this.ConvertToNestOpInput(node4, op2.Var, collectionInfoList, collectionNodes, externalReferences, collectionReferences);
                    }
                    else
                    {
                        referencedVars.Add(node);
                        args.Add(node);
                    }
                }
                else
                {
                    collection.Add(node);
                    args.Add(node);
                }
            }
            if (collectionNodes.Count == 0)
            {
                return projectNode;
            }
            VarVec outputVars = this.Command.CreateVarVec(op.Outputs);
            VarVec vars = this.Command.CreateVarVec(op.Outputs);
            vars.Minus(collectionReferences);
            vars.Or(externalReferences);
            if (!vars.IsEmpty)
            {
                if (IsNestOpNode(projectNode.Child0))
                {
                    if ((collection.Count == 0) && (referencedVars.Count == 0))
                    {
                        projectNode = projectNode.Child0;
                        this.EnsureReferencedVarsAreRemoved(referencedVars, outputVars);
                    }
                    else
                    {
                        NestBaseOp op3 = (NestBaseOp) projectNode.Child0.Op;
                        List<Node> list6 = new List<Node> {
                            projectNode.Child0.Child0
                        };
                        referencedVars.AddRange(collection);
                        list6.Add(this.Command.CreateNode(this.Command.CreateVarDefListOp(), referencedVars));
                        VarVec vec5 = this.Command.CreateVarVec(op3.Outputs);
                        foreach (CollectionInfo info in op3.CollectionInfo)
                        {
                            vec5.Clear(info.CollectionVar);
                        }
                        foreach (Node node5 in referencedVars)
                        {
                            vec5.Set(((VarDefOp) node5.Op).Var);
                        }
                        Node node6 = this.Command.CreateNode(this.Command.CreateProjectOp(vec5), list6);
                        VarVec vec6 = this.Command.CreateVarVec(vec5);
                        vec6.Or(op3.Outputs);
                        MultiStreamNestOp op4 = this.Command.CreateMultiStreamNestOp(op3.PrefixSortKeys, vec6, op3.CollectionInfo);
                        List<Node> list7 = new List<Node> {
                            node6
                        };
                        for (int i = 1; i < projectNode.Child0.Children.Count; i++)
                        {
                            list7.Add(projectNode.Child0.Children[i]);
                        }
                        projectNode = this.Command.CreateNode(op4, list7);
                    }
                }
                else
                {
                    ProjectOp op5 = this.Command.CreateProjectOp(vars);
                    projectNode.Child1 = this.Command.CreateNode(projectNode.Child1.Op, args);
                    projectNode.Op = op5;
                    this.EnsureReferencedVarsAreRemapped(referencedVars);
                }
            }
            else
            {
                projectNode = projectNode.Child0;
                this.EnsureReferencedVarsAreRemoved(referencedVars, outputVars);
            }
            externalReferences.And(projectNode.GetExtendedNodeInfo(this.Command).Definitions);
            outputVars.Or(externalReferences);
            MultiStreamNestOp op6 = this.Command.CreateMultiStreamNestOp(new List<SortKey>(), outputVars, collectionInfoList);
            collectionNodes.Insert(0, projectNode);
            Node n = this.Command.CreateNode(op6, collectionNodes);
            this.Command.RecomputeNodeInfo(projectNode);
            this.Command.RecomputeNodeInfo(n);
            return n;
        }

        private Node ProjectOpCase2(Node projectNode)
        {
            List<CollectionInfo> collectionInfo;
            List<Node> list2;
            ProjectOp op = (ProjectOp) projectNode.Op;
            Node n = projectNode.Child0;
            NestBaseOp op2 = n.Op as NestBaseOp;
            VarVec other = this.Command.CreateVarVec();
            foreach (CollectionInfo info in op2.CollectionInfo)
            {
                other.Set(info.CollectionVar);
            }
            VarVec vec2 = this.Command.CreateVarVec(op2.Outputs);
            vec2.Minus(other);
            VarVec vec3 = this.Command.CreateVarVec(op.Outputs);
            vec3.Minus(other);
            VarVec vec4 = this.Command.CreateVarVec(op.Outputs);
            vec4.Minus(vec3);
            VarVec vec5 = this.Command.CreateVarVec(other);
            vec5.Minus(vec4);
            if (vec5.IsEmpty)
            {
                collectionInfo = op2.CollectionInfo;
                list2 = new List<Node>(n.Children);
            }
            else
            {
                collectionInfo = new List<CollectionInfo>();
                list2 = new List<Node> {
                    n.Child0
                };
                int num = 1;
                foreach (CollectionInfo info2 in op2.CollectionInfo)
                {
                    if (!vec5.IsSet(info2.CollectionVar))
                    {
                        collectionInfo.Add(info2);
                        list2.Add(n.Children[num]);
                    }
                    num++;
                }
            }
            VarVec vec6 = this.Command.CreateVarVec();
            for (int i = 1; i < n.Children.Count; i++)
            {
                vec6.Or(n.Children[i].GetExtendedNodeInfo(this.Command).ExternalReferences);
            }
            vec6.And(n.Child0.GetExtendedNodeInfo(this.Command).Definitions);
            VarVec v = this.Command.CreateVarVec(vec3);
            v.Or(vec2);
            v.Or(vec6);
            List<Node> args = new List<Node>(projectNode.Child1.Children.Count);
            foreach (Node node2 in projectNode.Child1.Children)
            {
                VarDefOp op3 = (VarDefOp) node2.Op;
                if (v.IsSet(op3.Var))
                {
                    args.Add(node2);
                }
            }
            if ((collectionInfo.Count != 0) && v.IsEmpty)
            {
                Var var;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(args.Count == 0, "outputs is empty with non-zero count of children?");
                NullOp op4 = this.Command.CreateNullOp(this.Command.StringType);
                Node definingExpr = this.Command.CreateNode(op4);
                Node item = this.Command.CreateVarDefNode(definingExpr, out var);
                args.Add(item);
                v.Set(var);
            }
            projectNode.Op = this.Command.CreateProjectOp(this.Command.CreateVarVec(v));
            projectNode.Child1 = this.Command.CreateNode(projectNode.Child1.Op, args);
            if (collectionInfo.Count == 0)
            {
                projectNode.Child0 = n.Child0;
                n = projectNode;
            }
            else
            {
                VarVec outputVars = this.Command.CreateVarVec(op.Outputs);
                for (int j = 1; j < list2.Count; j++)
                {
                    outputVars.Or(list2[j].GetNodeInfo(this.Command).ExternalReferences);
                }
                foreach (SortKey key in op2.PrefixSortKeys)
                {
                    outputVars.Set(key.Var);
                }
                n.Op = this.Command.CreateMultiStreamNestOp(op2.PrefixSortKeys, outputVars, collectionInfo);
                n = this.Command.CreateNode(n.Op, list2);
                projectNode.Child0 = n.Child0;
                n.Child0 = projectNode;
                this.Command.RecomputeNodeInfo(projectNode);
            }
            this.Command.RecomputeNodeInfo(n);
            return n;
        }

        private static void RemapSortKeys(List<SortKey> sortKeys, Dictionary<Var, Var> varMap)
        {
            if (sortKeys != null)
            {
                foreach (SortKey key in sortKeys)
                {
                    Var var;
                    if (varMap.TryGetValue(key.Var, out var))
                    {
                        key.Var = var;
                    }
                }
            }
        }

        private VarList RemapVarList(VarList varList, Dictionary<Var, Var> varMap) => 
            System.Data.Query.InternalTrees.Command.CreateVarList(this.RemapVars(varList, varMap));

        private IEnumerable<Var> RemapVars(IEnumerable<Var> vars, Dictionary<Var, Var> varMap)
        {
            foreach (Var iteratorVariable0 in vars)
            {
                Var iteratorVariable1;
                if (varMap.TryGetValue(iteratorVariable0, out iteratorVariable1))
                {
                    yield return iteratorVariable1;
                }
                else
                {
                    yield return iteratorVariable0;
                }
            }
        }

        private VarVec RemapVarVec(VarVec varVec, Dictionary<Var, Var> varMap) => 
            this.Command.CreateVarVec(this.RemapVars(varVec, varMap));

        private Var ResolveVarReference(Var refVar)
        {
            Var key = refVar;
            while (this.m_varRefMap.TryGetValue(key, out key))
            {
                refVar = key;
            }
            return refVar;
        }

        private bool TryGetNewGuidFunction(out EdmFunction newGuidFunction)
        {
            newGuidFunction = null;
            System.Collections.ObjectModel.ReadOnlyCollection<EdmFunction> onlys = this.m_compilerState.MetadataWorkspace.GetFunctions("NewGuid", "Edm", DataSpace.CSpace);
            if ((onlys != null) && (1 == onlys.Count))
            {
                newGuidFunction = onlys[0];
            }
            return (null != newGuidFunction);
        }

        private void UpdateReplacementVarMap(IEnumerable<Var> fromVars, IEnumerable<Var> toVars)
        {
            IEnumerator<Var> enumerator = toVars.GetEnumerator();
            foreach (Var var in fromVars)
            {
                if (!enumerator.MoveNext())
                {
                    throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 2);
                }
                this.m_varRemapper.AddMapping(var, enumerator.Current);
            }
            if (enumerator.MoveNext())
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.ColumnCountMismatch, 3);
            }
        }

        public override Node Visit(CaseOp op, Node n)
        {
            foreach (Node node in n.Children)
            {
                if (node.Op.OpType == OpType.Collect)
                {
                    throw EntityUtil.NestingNotSupported(op, node.Op);
                }
                if (node.Op.OpType == OpType.VarRef)
                {
                    Var key = this.ResolveVarReference(((VarRefOp) node.Op).Var);
                    if (this.m_definingNodeMap.ContainsKey(key))
                    {
                        throw EntityUtil.NestingNotSupported(op, node.Op);
                    }
                }
            }
            return this.VisitDefault(n);
        }

        public override Node Visit(ConstrainedSortOp op, Node n)
        {
            this.VisitChildren(n);
            NestBaseOp inputNestOp = n.Child0.Op as NestBaseOp;
            if (inputNestOp != null)
            {
                Node node = n.Child0;
                n.Child0 = node.Child0;
                node.Child0 = n;
                node.Op = this.GetNestOpWithConsolidatedSortKeys(inputNestOp, op.Keys);
                n = node;
            }
            return n;
        }

        public override Node Visit(DistinctOp op, Node n) => 
            this.NestingNotSupported(op, n);

        public override Node Visit(FilterOp op, Node n)
        {
            this.VisitChildren(n);
            if (n.Child0.Op is NestBaseOp)
            {
                Node node = n.Child0;
                Node node2 = node.Child0;
                n.Child0 = node2;
                node.Child0 = n;
                this.Command.RecomputeNodeInfo(n);
                this.Command.RecomputeNodeInfo(node);
                return node;
            }
            return n;
        }

        public override Node Visit(GroupByOp op, Node n) => 
            this.NestingNotSupported(op, n);

        public override Node Visit(PhysicalProjectOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count == 1, "multiple inputs to physicalProject?");
            this.VisitChildren(n);
            this.m_varRemapper.RemapNode(n);
            if ((n == this.Command.Root) && IsNestOpNode(n.Child0))
            {
                VarList list;
                SimpleColumnMap[] mapArray;
                Node nestNode = n.Child0;
                Dictionary<Var, ColumnMap> varRefReplacementMap = new Dictionary<Var, ColumnMap>();
                nestNode = this.ConvertToSingleStreamNest(nestNode, varRefReplacementMap, out list, out mapArray);
                SingleStreamNestOp ssnOp = (SingleStreamNestOp) nestNode.Op;
                Node node2 = this.BuildSortForNestElimination(ssnOp, nestNode);
                SimpleCollectionColumnMap columnMap = (SimpleCollectionColumnMap) ColumnMapTranslator.Translate(((PhysicalProjectOp) n.Op).ColumnMap, varRefReplacementMap);
                columnMap = new SimpleCollectionColumnMap(columnMap.Type, columnMap.Name, columnMap.Element, mapArray, null, columnMap.SortKeys);
                n.Op = this.Command.CreatePhysicalProjectOp(list, columnMap);
                n.Child0 = node2;
            }
            return n;
        }

        public override Node Visit(ProjectOp op, Node n)
        {
            this.VisitChildren(n);
            this.m_varRemapper.RemapNode(n);
            Node projectNode = this.ProjectOpCase1(n);
            if ((projectNode.Op.OpType == OpType.Project) && IsNestOpNode(projectNode.Child0))
            {
                projectNode = this.ProjectOpCase2(projectNode);
            }
            return this.MergeNestedNestOps(projectNode);
        }

        public override Node Visit(SingleRowOp op, Node n)
        {
            this.VisitChildren(n);
            if (IsNestOpNode(n.Child0))
            {
                n = n.Child0;
                Node node = this.Command.CreateNode(op, n.Child0);
                n.Child0 = node;
                this.Command.RecomputeNodeInfo(n);
            }
            return n;
        }

        public override Node Visit(SortOp op, Node n)
        {
            this.VisitChildren(n);
            this.m_varRemapper.RemapNode(n);
            NestBaseOp inputNestOp = n.Child0.Op as NestBaseOp;
            if (inputNestOp != null)
            {
                n.Child0.Op = this.GetNestOpWithConsolidatedSortKeys(inputNestOp, op.Keys);
                return n.Child0;
            }
            return n;
        }

        public override Node Visit(UnnestOp op, Node n)
        {
            this.VisitChildren(n);
            if (TypeUtils.IsUdt(TypeHelpers.GetEdmType<CollectionType>(op.Var.Type).TypeUsage))
            {
                return n;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.Op.OpType == OpType.VarDef, "Unnest without VarDef input?");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(((VarDefOp) n.Child0.Op).Var == op.Var, "Unnest var not found?");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.HasChild0, "VarDef without input?");
            Node node = n.Child0.Child0;
            if (OpType.Function == node.Op.OpType)
            {
                return n;
            }
            if (OpType.Collect == node.Op.OpType)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.HasChild0, "collect without input?");
                node = node.Child0;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.Op.OpType == OpType.PhysicalProject, "collect without physicalProject?");
                this.m_definingNodeMap.Add(op.Var, node);
            }
            else
            {
                Node node2;
                if (OpType.VarRef != node.Op.OpType)
                {
                    throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.InvalidInternalTree, 2, node.Op.OpType);
                }
                Var key = this.ResolveVarReference(((VarRefOp) node.Op).Var);
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_definingNodeMap.TryGetValue(key, out node2), "Could not find a definition for a referenced collection var");
                node = this.CopyCollectionVarDefinition(node2);
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.Op.OpType == OpType.PhysicalProject, "driving node is not physicalProject?");
            }
            IEnumerable<Var> outputs = ((PhysicalProjectOp) node.Op).Outputs;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.HasChild0, "physicalProject without input?");
            node = node.Child0;
            if (node.Op.OpType == OpType.Sort)
            {
                node = node.Child0;
            }
            this.UpdateReplacementVarMap(op.Table.Columns, outputs);
            return node;
        }

        public override Node Visit(VarDefOp op, Node n)
        {
            this.VisitChildren(n);
            this.m_varRemapper.RemapNode(n);
            if (n.Child0.Op.OpType == OpType.VarRef)
            {
                this.m_varRefMap.Add(op.Var, ((VarRefOp) n.Child0.Op).Var);
            }
            return n;
        }

        public override Node Visit(VarRefOp op, Node n)
        {
            this.VisitChildren(n);
            this.m_varRemapper.RemapNode(n);
            return n;
        }

        protected override Node VisitApplyOp(ApplyBaseOp op, Node n) => 
            this.ApplyOpJoinOp(op, n);

        protected override Node VisitJoinOp(JoinBaseOp op, Node n) => 
            this.ApplyOpJoinOp(op, n);

        protected override Node VisitNestOp(NestBaseOp op, Node n)
        {
            this.VisitChildren(n);
            foreach (Node node in n.Children)
            {
                if (IsNestOpNode(node))
                {
                    throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.NestOverNest);
                }
            }
            return n;
        }

        protected override Node VisitRelOpDefault(RelOp op, Node n) => 
            this.NestingNotSupported(op, n);

        protected override Node VisitSetOp(SetOp op, Node n) => 
            this.NestingNotSupported(op, n);

        private System.Data.Query.InternalTrees.Command Command =>
            this.m_compilerState.Command;

    }
}

