namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Query.PlanCompiler;
    using System.Runtime.InteropServices;

    internal class Command
    {
        private TypeUsage m_boolType;
        private System.Data.Metadata.Edm.DataSpace m_dataSpace;
        private bool m_disableVarVecEnumCaching;
        private ConstantPredicateOp m_falseOp;
        private Stack<VarVec.VarVecEnumerator> m_freeVarVecEnumerators;
        private Stack<VarVec> m_freeVarVecs;
        private TypeUsage m_intType;
        private KeyPullup m_keyPullupVisitor;
        private System.Data.Metadata.Edm.MetadataWorkspace m_metadataWorkspace;
        private int m_nextBranchDiscriminatorValue = 0x3e8;
        private int m_nextNodeId;
        private NodeInfoVisitor m_nodeInfoVisitor;
        private Dictionary<string, ParameterVar> m_parameterMap = new Dictionary<string, ParameterVar>();
        private HashSet<RelProperty> m_referencedRelProperties;
        private Node m_root;
        private TypeUsage m_stringType;
        private List<Table> m_tables = new List<Table>();
        private ConstantPredicateOp m_trueOp;
        private List<Var> m_vars = new List<Var>();

        internal Command(System.Data.Metadata.Edm.MetadataWorkspace metadataWorkspace, System.Data.Metadata.Edm.DataSpace dataSpace)
        {
            this.m_metadataWorkspace = metadataWorkspace;
            this.m_dataSpace = dataSpace;
            if (!this.TryGetPrimitiveType(PrimitiveTypeKind.Boolean, out this.m_boolType))
            {
                throw EntityUtil.ProviderIncompatible(Strings.Cqt_General_NoProviderBooleanType);
            }
            if (!this.TryGetPrimitiveType(PrimitiveTypeKind.Int32, out this.m_intType))
            {
                throw EntityUtil.ProviderIncompatible(Strings.Cqt_General_NoProviderIntegerType);
            }
            if (!this.TryGetPrimitiveType(PrimitiveTypeKind.String, out this.m_stringType))
            {
                throw EntityUtil.ProviderIncompatible(Strings.Cqt_General_NoProviderStringType);
            }
            this.m_trueOp = new ConstantPredicateOp(this.m_boolType, true);
            this.m_falseOp = new ConstantPredicateOp(this.m_boolType, false);
            this.m_nodeInfoVisitor = new NodeInfoVisitor(this);
            this.m_keyPullupVisitor = new KeyPullup(this);
            this.m_freeVarVecEnumerators = new Stack<VarVec.VarVecEnumerator>();
            this.m_freeVarVecs = new Stack<VarVec>();
            this.m_referencedRelProperties = new HashSet<RelProperty>();
        }

        private void AddRelPropertyReference(RelProperty relProperty)
        {
            if ((relProperty.ToEnd.RelationshipMultiplicity != RelationshipMultiplicity.Many) && !this.m_referencedRelProperties.Contains(relProperty))
            {
                this.m_referencedRelProperties.Add(relProperty);
            }
        }

        internal Node BuildCollect(Node relOpNode, Var relOpVar)
        {
            Node node = this.CreateNode(this.CreatePhysicalProjectOp(relOpVar), relOpNode);
            TypeUsage type = TypeHelpers.CreateCollectionTypeUsage(relOpVar.Type);
            return this.CreateNode(this.CreateCollectOp(type), node);
        }

        internal Node BuildComparison(OpType opType, Node arg0, Node arg1)
        {
            if (!EqualTypes(arg0.Op.Type, arg1.Op.Type))
            {
                TypeUsage commonTypeUsage = TypeHelpers.GetCommonTypeUsage(arg0.Op.Type, arg1.Op.Type);
                if (!EqualTypes(commonTypeUsage, arg0.Op.Type))
                {
                    arg0 = this.CreateNode(this.CreateSoftCastOp(commonTypeUsage), arg0);
                }
                if (!EqualTypes(commonTypeUsage, arg1.Op.Type))
                {
                    arg1 = this.CreateNode(this.CreateSoftCastOp(commonTypeUsage), arg1);
                }
            }
            return this.CreateNode(this.CreateComparisonOp(opType), arg0, arg1);
        }

        internal void BuildOfTypeTree(Node inputNode, Var inputVar, TypeUsage desiredType, bool includeSubtypes, out Node resultNode, out Var resultVar)
        {
            Op op = includeSubtypes ? this.CreateIsOfOp(desiredType) : this.CreateIsOfOnlyOp(desiredType);
            Node node = this.CreateNode(op, this.CreateNode(this.CreateVarRefOp(inputVar)));
            Node input = this.CreateNode(this.CreateFilterOp(), inputNode, node);
            Node computedExpression = this.CreateNode(this.CreateFakeTreatOp(desiredType), this.CreateNode(this.CreateVarRefOp(inputVar)));
            resultNode = this.BuildProject(input, computedExpression, out resultVar);
        }

        internal Node BuildProject(Node inputNode, IEnumerable<Var> inputVars, IEnumerable<Node> computedExpressions)
        {
            VarDefListOp op = this.CreateVarDefListOp();
            Node node = this.CreateNode(op);
            VarVec vars = this.CreateVarVec(inputVars);
            foreach (Node node2 in computedExpressions)
            {
                Var v = this.CreateComputedVar(node2.Op.Type);
                vars.Set(v);
                VarDefOp op2 = this.CreateVarDefOp(v);
                Node item = this.CreateNode(op2, node2);
                node.Children.Add(item);
            }
            return this.CreateNode(this.CreateProjectOp(vars), inputNode, node);
        }

        internal Node BuildProject(Node input, Node computedExpression, out Var projectVar)
        {
            Node node = this.BuildProject(input, new Var[0], new Node[] { computedExpression });
            projectVar = ((ProjectOp) node.Op).Outputs.First;
            return node;
        }

        internal void BuildUnionAllLadder(IList<Node> inputNodes, IList<Var> inputVars, out Node resultNode, out IList<Var> resultVars)
        {
            if (inputNodes.Count == 0)
            {
                resultNode = null;
                resultVars = null;
            }
            else
            {
                int num = inputVars.Count / inputNodes.Count;
                if (inputNodes.Count == 1)
                {
                    resultNode = inputNodes[0];
                    resultVars = inputVars;
                }
                else
                {
                    List<Var> list = new List<Var>();
                    Node node = inputNodes[0];
                    for (int i = 0; i < num; i++)
                    {
                        list.Add(inputVars[i]);
                    }
                    for (int j = 1; j < inputNodes.Count; j++)
                    {
                        VarMap leftMap = this.CreateVarMap();
                        VarMap rightMap = this.CreateVarMap();
                        List<Var> list2 = new List<Var>();
                        for (int k = 0; k < num; k++)
                        {
                            SetOpVar item = this.CreateSetOpVar(list[k].Type);
                            list2.Add(item);
                            leftMap.Add(item, list[k]);
                            rightMap.Add(item, inputVars[(j * num) + k]);
                        }
                        Op op = this.CreateUnionAllOp(leftMap, rightMap);
                        node = this.CreateNode(op, node, inputNodes[j]);
                        list = list2;
                    }
                    resultNode = node;
                    resultVars = list;
                }
            }
        }

        internal void BuildUnionAllLadder(IList<Node> inputNodes, IList<Var> inputVars, out Node resultNode, out Var resultVar)
        {
            IList<Var> list;
            this.BuildUnionAllLadder(inputNodes, inputVars, out resultNode, out list);
            if ((list != null) && (list.Count > 0))
            {
                resultVar = list[0];
            }
            else
            {
                resultVar = null;
            }
        }

        internal AggregateOp CreateAggregateOp(EdmFunction aggFunc, bool distinctAgg) => 
            new AggregateOp(aggFunc, distinctAgg);

        internal ArithmeticOp CreateArithmeticOp(OpType opType, TypeUsage type) => 
            new ArithmeticOp(opType, type);

        internal CaseOp CreateCaseOp(TypeUsage type) => 
            new CaseOp(type);

        internal CastOp CreateCastOp(TypeUsage type) => 
            new CastOp(type);

        internal static CollectionInfo CreateCollectionInfo(Var collectionVar, ColumnMap columnMap, VarList flattenedElementVars, VarVec keys, List<SortKey> sortKeys, object discriminatorValue) => 
            new CollectionInfo(collectionVar, columnMap, flattenedElementVars, keys, sortKeys, discriminatorValue);

        internal CollectOp CreateCollectOp(TypeUsage type) => 
            new CollectOp(type);

        internal ColumnVar CreateColumnVar(Table table, ColumnMD columnMD)
        {
            ColumnVar item = new ColumnVar(this.NewVarId(), table, columnMD);
            table.Columns.Add(item);
            this.m_vars.Add(item);
            return item;
        }

        internal ComparisonOp CreateComparisonOp(OpType opType) => 
            new ComparisonOp(opType, this.BooleanType);

        internal ComputedVar CreateComputedVar(TypeUsage type)
        {
            ComputedVar item = new ComputedVar(this.NewVarId(), type);
            this.m_vars.Add(item);
            return item;
        }

        internal ConditionalOp CreateConditionalOp(OpType opType) => 
            new ConditionalOp(opType, this.BooleanType);

        internal ConstantBaseOp CreateConstantOp(TypeUsage type, object value)
        {
            if (value == null)
            {
                return new NullOp(type);
            }
            if (TypeSemantics.IsBooleanType(type))
            {
                return new InternalConstantOp(type, value);
            }
            return new ConstantOp(type, value);
        }

        internal ConstantPredicateOp CreateConstantPredicateOp(bool value)
        {
            if (!value)
            {
                return this.m_falseOp;
            }
            return this.m_trueOp;
        }

        internal ConstrainedSortOp CreateConstrainedSortOp(List<SortKey> sortKeys) => 
            new ConstrainedSortOp(sortKeys, false);

        internal ConstrainedSortOp CreateConstrainedSortOp(List<SortKey> sortKeys, bool withTies) => 
            new ConstrainedSortOp(sortKeys, withTies);

        internal CrossApplyOp CreateCrossApplyOp() => 
            CrossApplyOp.Instance;

        internal CrossJoinOp CreateCrossJoinOp() => 
            CrossJoinOp.Instance;

        internal DerefOp CreateDerefOp(TypeUsage type) => 
            new DerefOp(type);

        internal DiscriminatedNewEntityOp CreateDiscriminatedNewEntityOp(TypeUsage type, ExplicitDiscriminatorMap discriminatorMap, EntitySetBase entitySet, List<RelProperty> relProperties) => 
            new DiscriminatedNewEntityOp(type, discriminatorMap, entitySet, relProperties);

        internal DistinctOp CreateDistinctOp(Var keyVar) => 
            new DistinctOp(this.CreateVarVec(keyVar));

        internal DistinctOp CreateDistinctOp(VarVec keyVars) => 
            new DistinctOp(keyVars);

        internal ElementOp CreateElementOp(TypeUsage type) => 
            new ElementOp(type);

        internal ExceptOp CreateExceptOp(VarMap leftMap, VarMap rightMap)
        {
            VarVec outputs = this.CreateVarVec();
            foreach (Var var in leftMap.Keys)
            {
                outputs.Set(var);
            }
            return new ExceptOp(outputs, leftMap, rightMap);
        }

        internal ExistsOp CreateExistsOp() => 
            new ExistsOp(this.BooleanType);

        internal TreatOp CreateFakeTreatOp(TypeUsage type) => 
            new TreatOp(type, true);

        internal ConstantPredicateOp CreateFalseOp() => 
            this.m_falseOp;

        internal FilterOp CreateFilterOp() => 
            FilterOp.Instance;

        internal TableMD CreateFlatTableDefinition(RowType type) => 
            this.CreateFlatTableDefinition(type.Properties, new List<EdmMember>(), null);

        internal TableMD CreateFlatTableDefinition(IEnumerable<EdmProperty> properties, IEnumerable<EdmMember> keyMembers, EntitySetBase entitySet) => 
            new TableMD(properties, keyMembers, entitySet);

        internal FullOuterJoinOp CreateFullOuterJoinOp() => 
            FullOuterJoinOp.Instance;

        internal FunctionOp CreateFunctionOp(EdmFunction function) => 
            new FunctionOp(function);

        internal GetEntityRefOp CreateGetEntityRefOp(TypeUsage type) => 
            new GetEntityRefOp(type);

        internal GetRefKeyOp CreateGetRefKeyOp(TypeUsage type) => 
            new GetRefKeyOp(type);

        internal GroupByOp CreateGroupByOp(VarVec gbyKeys, VarVec outputs) => 
            new GroupByOp(gbyKeys, outputs);

        internal InnerJoinOp CreateInnerJoinOp() => 
            InnerJoinOp.Instance;

        internal InternalConstantOp CreateInternalConstantOp(TypeUsage type, object value) => 
            new InternalConstantOp(type, value);

        internal IntersectOp CreateIntersectOp(VarMap leftMap, VarMap rightMap)
        {
            VarVec outputs = this.CreateVarVec();
            foreach (Var var in leftMap.Keys)
            {
                outputs.Set(var);
            }
            return new IntersectOp(outputs, leftMap, rightMap);
        }

        internal IsOfOp CreateIsOfOnlyOp(TypeUsage isOfType) => 
            new IsOfOp(isOfType, true, this.m_boolType);

        internal IsOfOp CreateIsOfOp(TypeUsage isOfType) => 
            new IsOfOp(isOfType, false, this.m_boolType);

        internal LeftOuterJoinOp CreateLeftOuterJoinOp() => 
            LeftOuterJoinOp.Instance;

        internal LikeOp CreateLikeOp() => 
            new LikeOp(this.BooleanType);

        internal MultiStreamNestOp CreateMultiStreamNestOp(List<SortKey> prefixSortKeys, VarVec outputVars, List<CollectionInfo> collectionInfoList) => 
            new MultiStreamNestOp(prefixSortKeys, outputVars, collectionInfoList);

        internal NavigateOp CreateNavigateOp(TypeUsage type, RelProperty relProperty)
        {
            this.AddRelPropertyReference(relProperty);
            return new NavigateOp(type, relProperty);
        }

        internal NewEntityOp CreateNewEntityOp(TypeUsage type, List<RelProperty> relProperties) => 
            this.CreateNewEntityOp(type, relProperties, null);

        internal NewEntityOp CreateNewEntityOp(TypeUsage type, List<RelProperty> relProperties, EntitySetBase entitySet) => 
            new NewEntityOp(type, relProperties, entitySet);

        internal NewInstanceOp CreateNewInstanceOp(TypeUsage type) => 
            new NewInstanceOp(type);

        internal NewMultisetOp CreateNewMultisetOp(TypeUsage type) => 
            new NewMultisetOp(type);

        internal NewRecordOp CreateNewRecordOp(RowType type) => 
            new NewRecordOp(TypeUsage.Create(type));

        internal NewRecordOp CreateNewRecordOp(TypeUsage type) => 
            new NewRecordOp(type);

        internal NewRecordOp CreateNewRecordOp(TypeUsage type, List<EdmProperty> fields) => 
            new NewRecordOp(type, fields);

        internal Node CreateNode(Op op) => 
            this.CreateNode(op, new List<Node>());

        internal Node CreateNode(Op op, IList<Node> args) => 
            new Node(this.m_nextNodeId++, op, new List<Node>(args));

        internal Node CreateNode(Op op, List<Node> args) => 
            new Node(this.m_nextNodeId++, op, args);

        internal Node CreateNode(Op op, Node arg1)
        {
            List<Node> args = new List<Node> {
                arg1
            };
            return this.CreateNode(op, args);
        }

        internal Node CreateNode(Op op, Node arg1, Node arg2)
        {
            List<Node> args = new List<Node> {
                arg1,
                arg2
            };
            return this.CreateNode(op, args);
        }

        internal Node CreateNode(Op op, Node arg1, Node arg2, Node arg3)
        {
            List<Node> args = new List<Node> {
                arg1,
                arg2,
                arg3
            };
            return this.CreateNode(op, args);
        }

        internal NullOp CreateNullOp(TypeUsage type) => 
            new NullOp(type);

        internal OuterApplyOp CreateOuterApplyOp() => 
            OuterApplyOp.Instance;

        internal ParameterVar CreateParameterVar(string parameterName, TypeUsage parameterType)
        {
            if (this.m_parameterMap.ContainsKey(parameterName))
            {
                throw new Exception("duplicate parameter name: " + parameterName);
            }
            ParameterVar item = new ParameterVar(this.NewVarId(), parameterType, parameterName);
            this.m_vars.Add(item);
            this.m_parameterMap[parameterName] = item;
            return item;
        }

        internal PhysicalProjectOp CreatePhysicalProjectOp(Var outputVar)
        {
            VarList outputVars = CreateVarList();
            outputVars.Add(outputVar);
            VarRefColumnMap elementMap = new VarRefColumnMap(outputVar);
            SimpleCollectionColumnMap columnMap = new SimpleCollectionColumnMap(TypeUtils.CreateCollectionType(elementMap.Type), null, elementMap, new SimpleColumnMap[0], new SimpleColumnMap[0], new SortKeyInfo[0]);
            return this.CreatePhysicalProjectOp(outputVars, columnMap);
        }

        internal PhysicalProjectOp CreatePhysicalProjectOp(VarList outputVars, SimpleCollectionColumnMap columnMap) => 
            new PhysicalProjectOp(outputVars, columnMap);

        internal ProjectOp CreateProjectOp(Var v)
        {
            VarVec vars = this.CreateVarVec();
            vars.Set(v);
            return new ProjectOp(vars);
        }

        internal ProjectOp CreateProjectOp(VarVec vars) => 
            new ProjectOp(vars);

        internal PropertyOp CreatePropertyOp(EdmMember prop)
        {
            NavigationProperty property = prop as NavigationProperty;
            if (property != null)
            {
                RelProperty relProperty = new RelProperty(property.RelationshipType, property.FromEndMember, property.ToEndMember);
                this.AddRelPropertyReference(relProperty);
                RelProperty property3 = new RelProperty(property.RelationshipType, property.ToEndMember, property.FromEndMember);
                this.AddRelPropertyReference(property3);
            }
            return new PropertyOp(Helper.GetModelTypeUsage(prop), prop);
        }

        internal RefOp CreateRefOp(EntitySet entitySet, TypeUsage type) => 
            new RefOp(entitySet, type);

        internal RelPropertyOp CreateRelPropertyOp(RelProperty prop)
        {
            this.AddRelPropertyReference(prop);
            return new RelPropertyOp(prop.ToEnd.TypeUsage, prop);
        }

        internal ScanTableOp CreateScanTableOp(Table table) => 
            new ScanTableOp(table);

        internal ScanTableOp CreateScanTableOp(TableMD tableMetadata)
        {
            Table table = this.CreateTableInstance(tableMetadata);
            return this.CreateScanTableOp(table);
        }

        internal ScanViewOp CreateScanViewOp(Table table) => 
            new ScanViewOp(table);

        internal ScanViewOp CreateScanViewOp(TableMD tableMetadata)
        {
            Table table = this.CreateTableInstance(tableMetadata);
            return this.CreateScanViewOp(table);
        }

        internal SetOpVar CreateSetOpVar(TypeUsage type)
        {
            SetOpVar item = new SetOpVar(this.NewVarId(), type);
            this.m_vars.Add(item);
            return item;
        }

        internal SingleRowOp CreateSingleRowOp() => 
            SingleRowOp.Instance;

        internal SingleRowTableOp CreateSingleRowTableOp() => 
            SingleRowTableOp.Instance;

        internal SingleStreamNestOp CreateSingleStreamNestOp(VarVec keys, List<SortKey> prefixSortKeys, List<SortKey> postfixSortKeys, VarVec outputVars, List<CollectionInfo> collectionInfoList, Var discriminatorVar) => 
            new SingleStreamNestOp(keys, prefixSortKeys, postfixSortKeys, outputVars, collectionInfoList, discriminatorVar);

        internal SoftCastOp CreateSoftCastOp(TypeUsage type) => 
            new SoftCastOp(type);

        internal static SortKey CreateSortKey(Var v) => 
            new SortKey(v, true, "");

        internal static SortKey CreateSortKey(Var v, bool asc) => 
            new SortKey(v, asc, "");

        internal static SortKey CreateSortKey(Var v, bool asc, string collation) => 
            new SortKey(v, asc, collation);

        internal SortOp CreateSortOp(List<SortKey> sortKeys) => 
            new SortOp(sortKeys);

        internal static TableMD CreateTableDefinition(EntitySetBase extent) => 
            new TableMD(TypeUsage.Create(extent.ElementType), extent);

        internal static TableMD CreateTableDefinition(TypeUsage elementType) => 
            new TableMD(elementType, null);

        internal Table CreateTableInstance(TableMD tableMetadata)
        {
            Table item = new Table(this, tableMetadata, this.NewTableId());
            this.m_tables.Add(item);
            return item;
        }

        internal TreatOp CreateTreatOp(TypeUsage type) => 
            new TreatOp(type, false);

        internal ConstantPredicateOp CreateTrueOp() => 
            this.m_trueOp;

        internal UnionAllOp CreateUnionAllOp(VarMap leftMap, VarMap rightMap) => 
            this.CreateUnionAllOp(leftMap, rightMap, null);

        internal UnionAllOp CreateUnionAllOp(VarMap leftMap, VarMap rightMap, Var branchDiscriminator)
        {
            VarVec outputs = this.CreateVarVec();
            foreach (Var var in leftMap.Keys)
            {
                outputs.Set(var);
            }
            return new UnionAllOp(outputs, leftMap, rightMap, branchDiscriminator);
        }

        internal UnnestOp CreateUnnestOp(Var v)
        {
            Table t = this.CreateTableInstance(CreateTableDefinition(TypeHelpers.GetEdmType<CollectionType>(v.Type).TypeUsage));
            return this.CreateUnnestOp(v, t);
        }

        internal UnnestOp CreateUnnestOp(Var v, Table t) => 
            new UnnestOp(v, t);

        internal Node CreateVarDefListNode(Node definingExpr, out Var computedVar)
        {
            Node node = this.CreateVarDefNode(definingExpr, out computedVar);
            VarDefListOp op = this.CreateVarDefListOp();
            return this.CreateNode(op, node);
        }

        internal VarDefListOp CreateVarDefListOp() => 
            VarDefListOp.Instance;

        internal Node CreateVarDefNode(Node definingExpr, out Var computedVar)
        {
            ScalarOp op = definingExpr.Op as ScalarOp;
            computedVar = this.CreateComputedVar(op.Type);
            VarDefOp op2 = this.CreateVarDefOp(computedVar);
            return this.CreateNode(op2, definingExpr);
        }

        internal VarDefOp CreateVarDefOp(Var v) => 
            new VarDefOp(v);

        internal static VarList CreateVarList() => 
            new VarList();

        internal static VarList CreateVarList(IEnumerable<Var> vars) => 
            new VarList(vars);

        internal VarMap CreateVarMap() => 
            new VarMap();

        internal VarRefOp CreateVarRefOp(Var v) => 
            new VarRefOp(v);

        internal VarVec CreateVarVec()
        {
            if (this.m_freeVarVecs.Count == 0)
            {
                return new VarVec(this);
            }
            VarVec vec = this.m_freeVarVecs.Pop();
            vec.Clear();
            return vec;
        }

        internal VarVec CreateVarVec(IEnumerable<Var> v)
        {
            VarVec vec = this.CreateVarVec();
            vec.InitFrom(v);
            return vec;
        }

        internal VarVec CreateVarVec(Var v)
        {
            VarVec vec = this.CreateVarVec();
            vec.Set(v);
            return vec;
        }

        internal VarVec CreateVarVec(VarVec v)
        {
            VarVec vec = this.CreateVarVec();
            vec.InitFrom(v);
            return vec;
        }

        internal void DisableVarVecEnumCaching()
        {
            this.m_disableVarVecEnumCaching = true;
        }

        internal static bool EqualTypes(EdmType x, EdmType y) => 
            TypeUsageEqualityComparer.Equals(x, y);

        internal static bool EqualTypes(TypeUsage x, TypeUsage y) => 
            TypeUsageEqualityComparer.Instance.Equals(x, y);

        internal ExtendedNodeInfo GetExtendedNodeInfo(Node n) => 
            n.GetExtendedNodeInfo(this);

        internal NodeInfo GetNodeInfo(Node n) => 
            n.GetNodeInfo(this);

        internal ParameterVar GetParameter(string paramName) => 
            this.m_parameterMap[paramName];

        internal void GetParameters(List<ParameterVar> destList)
        {
            foreach (ParameterVar var in this.m_parameterMap.Values)
            {
                destList.Add(var);
            }
        }

        internal Var GetVar(int id) => 
            this.m_vars[id];

        internal VarVec.VarVecEnumerator GetVarVecEnumerator(VarVec vec)
        {
            if (this.m_disableVarVecEnumCaching || (this.m_freeVarVecEnumerators.Count == 0))
            {
                return new VarVec.VarVecEnumerator(vec);
            }
            VarVec.VarVecEnumerator enumerator = this.m_freeVarVecEnumerators.Pop();
            enumerator.Init(vec);
            return enumerator;
        }

        internal bool IsRelPropertyReferenced(RelProperty relProperty) => 
            this.m_referencedRelProperties.Contains(relProperty);

        private int NewTableId() => 
            this.m_tables.Count;

        private int NewVarId() => 
            this.m_vars.Count;

        internal KeyVec PullupKeys(Node n) => 
            this.m_keyPullupVisitor.GetKeys(n);

        internal NodeInfo RecomputeNodeInfo(Node n) => 
            this.m_nodeInfoVisitor.ComputeNodeInfo(n);

        internal void ReleaseVarVec(VarVec vec)
        {
            this.m_freeVarVecs.Push(vec);
        }

        internal void ReleaseVarVecEnumerator(VarVec.VarVecEnumerator enumerator)
        {
            if (!this.m_disableVarVecEnumCaching)
            {
                this.m_freeVarVecEnumerators.Push(enumerator);
            }
        }

        private bool TryGetPrimitiveType(PrimitiveTypeKind modelType, out TypeUsage type)
        {
            type = null;
            if (modelType == PrimitiveTypeKind.String)
            {
                type = TypeUsage.CreateStringTypeUsage(this.m_metadataWorkspace.GetModelPrimitiveType(modelType), false, false);
            }
            else
            {
                type = this.m_metadataWorkspace.GetCanonicalModelTypeUsage(modelType);
            }
            return (null != type);
        }

        internal TypeUsage BooleanType =>
            this.m_boolType;

        internal System.Data.Metadata.Edm.DataSpace DataSpace =>
            this.m_dataSpace;

        internal TypeUsage IntegerType =>
            this.m_intType;

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this.m_metadataWorkspace;

        internal int NextBranchDiscriminatorValue =>
            this.m_nextBranchDiscriminatorValue++;

        internal int NextNodeId =>
            this.m_nextNodeId;

        internal HashSet<RelProperty> ReferencedRelProperties =>
            this.m_referencedRelProperties;

        internal Node Root
        {
            get => 
                this.m_root;
            set
            {
                this.m_root = value;
            }
        }

        internal TypeUsage StringType =>
            this.m_stringType;

        internal IEnumerable<Var> Vars =>
            this.m_vars;
    }
}

