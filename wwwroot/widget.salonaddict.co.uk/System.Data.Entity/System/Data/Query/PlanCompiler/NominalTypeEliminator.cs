namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class NominalTypeEliminator : BasicOpVisitorOfNode
    {
        private System.Data.Query.PlanCompiler.PlanCompiler m_compilerState;
        private Dictionary<Node, PropertyRefList> m_nodePropertyMap;
        private StructuredTypeInfo m_typeInfo;
        private Dictionary<TypeUsage, TypeUsage> m_typeToNewTypeMap;
        private VarInfoMap m_varInfoMap;
        private Dictionary<Var, PropertyRefList> m_varPropertyMap;
        private const string PrefixMatchCharacter = "%";

        private NominalTypeEliminator(System.Data.Query.PlanCompiler.PlanCompiler compilerState, StructuredTypeInfo typeInfo, Dictionary<Var, PropertyRefList> varPropertyMap, Dictionary<Node, PropertyRefList> nodePropertyMap)
        {
            this.m_compilerState = compilerState;
            this.m_typeInfo = typeInfo;
            this.m_varPropertyMap = varPropertyMap;
            this.m_nodePropertyMap = nodePropertyMap;
            this.m_varInfoMap = new VarInfoMap();
            this.m_typeToNewTypeMap = new Dictionary<TypeUsage, TypeUsage>(TypeUsageEqualityComparer.Instance);
        }

        private Node BuildAccessor(Node input, EdmProperty property)
        {
            Op op = input.Op;
            NewRecordOp op2 = op as NewRecordOp;
            if (op2 != null)
            {
                int num;
                if (op2.GetFieldPosition(property, out num))
                {
                    return this.Copy(input.Children[num]);
                }
                return null;
            }
            if (op.OpType == OpType.Null)
            {
                return null;
            }
            PropertyOp op3 = this.m_command.CreatePropertyOp(property);
            return this.m_command.CreateNode(op3, this.Copy(input));
        }

        private Node BuildAccessorWithNulls(Node input, EdmProperty property)
        {
            Node node = this.BuildAccessor(input, property);
            if (node == null)
            {
                node = this.CreateNullConstantNode(Helper.GetModelTypeUsage(property));
            }
            return node;
        }

        private Node BuildSoftCast(Node node, TypeUsage targetType)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.Op.IsScalarOp, "Attempting SoftCast around non-ScalarOp?");
            if (!Command.EqualTypes(node.Op.Type, targetType))
            {
                while (node.Op.OpType == OpType.SoftCast)
                {
                    node = node.Child0;
                }
                return this.m_command.CreateNode(this.m_command.CreateSoftCastOp(targetType), node);
            }
            return node;
        }

        private Node BuildTypeIdAccessor(Node input, TypeInfo typeInfo)
        {
            if (typeInfo.HasTypeIdProperty)
            {
                return this.BuildAccessorWithNulls(input, typeInfo.TypeIdProperty);
            }
            return this.CreateTypeIdConstant(typeInfo);
        }

        private Node Copy(Node n) => 
            OpCopier.Copy(this.m_command, n);

        private Node CreateDisjunctiveTypeComparisonOp(TypeInfo typeInfo, Node typeIdProperty)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(typeInfo.RootType.DiscriminatorMap != null, "should be used only for DiscriminatorMap type checks");
            IEnumerable<TypeInfo> enumerable = from t in typeInfo.GetTypeHierarchy()
                where !t.Type.EdmType.Abstract
                select t;
            Node node = null;
            foreach (TypeInfo info in enumerable)
            {
                Node node2 = this.CreateTypeEqualsOp(info, typeIdProperty);
                if (node == null)
                {
                    node = node2;
                }
                else
                {
                    node = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.Or), node, node2);
                }
            }
            if (node == null)
            {
                node = this.m_command.CreateNode(this.m_command.CreateFalseOp());
            }
            return node;
        }

        private Node CreateNullConstantNode(TypeUsage type) => 
            this.m_command.CreateNode(this.m_command.CreateNullOp(type));

        private Node CreateNullSentinelConstant()
        {
            InternalConstantOp op = this.m_command.CreateInternalConstantOp(this.m_command.IntegerType, 1);
            return this.m_command.CreateNode(op);
        }

        private Node CreateTypeComparisonOp(Node input, TypeInfo typeInfo, bool isExact)
        {
            Node typeIdProperty = this.BuildTypeIdAccessor(input, typeInfo);
            if (isExact)
            {
                return this.CreateTypeEqualsOp(typeInfo, typeIdProperty);
            }
            if (typeInfo.RootType.DiscriminatorMap != null)
            {
                return this.CreateDisjunctiveTypeComparisonOp(typeInfo, typeIdProperty);
            }
            Node node3 = this.CreateTypeIdConstantForPrefixMatch(typeInfo);
            LikeOp op = this.m_command.CreateLikeOp();
            return this.m_command.CreateNode(op, typeIdProperty, node3, this.CreateNullConstantNode(this.DefaultTypeIdType));
        }

        private Node CreateTypeEqualsOp(TypeInfo typeInfo, Node typeIdProperty)
        {
            Node node = this.CreateTypeIdConstant(typeInfo);
            ComparisonOp op = this.m_command.CreateComparisonOp(OpType.EQ);
            return this.m_command.CreateNode(op, typeIdProperty, node);
        }

        private Node CreateTypeIdConstant(TypeInfo typeInfo)
        {
            TypeUsage modelTypeUsage;
            object typeId = typeInfo.TypeId;
            if (typeInfo.RootType.DiscriminatorMap != null)
            {
                modelTypeUsage = Helper.GetModelTypeUsage(typeInfo.RootType.DiscriminatorMap.DiscriminatorProperty);
            }
            else
            {
                modelTypeUsage = this.DefaultTypeIdType;
            }
            InternalConstantOp op = this.m_command.CreateInternalConstantOp(modelTypeUsage, typeId);
            return this.m_command.CreateNode(op);
        }

        private Node CreateTypeIdConstantForPrefixMatch(TypeInfo typeInfo)
        {
            string str = typeInfo.TypeId + "%";
            InternalConstantOp op = this.m_command.CreateInternalConstantOp(this.DefaultTypeIdType, str);
            return this.m_command.CreateNode(op);
        }

        private Node EliminateInternalConstantAndNullGroupByKeys(Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n != null, "Null input");
            GroupByOp op = n.Op as GroupByOp;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op != null, "The op type of the input node to EliminateInternalConstantAndNullGroupByKeys is not a GroupByOp");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Child1 != null, "A GroupBy node with no Child1");
            if (n.Child1.Children.Count == 0)
            {
                return n;
            }
            List<Node> args = new List<Node>();
            foreach (Node node in n.Child1.Children)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(node.Op.OpType == OpType.VarDef, "A child of a VarDefList is not a VarDef");
                Node node2 = node.Child0;
                if ((node2.Op.OpType == OpType.InternalConstant) || (node2.Op.OpType == OpType.Null))
                {
                    args.Add(node);
                }
            }
            if (args.Count == 0)
            {
                return n;
            }
            Node node3 = this.m_command.CreateNode(this.m_command.CreateProjectOp(op.Outputs.Clone()), n, this.m_command.CreateNode(this.m_command.CreateVarDefListOp(), args));
            foreach (Node node4 in args)
            {
                n.Child1.Children.Remove(node4);
                VarDefOp op2 = node4.Op as VarDefOp;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op2 != null, "A child of a VarDefList is not a VarDef");
                op.Keys.Clear(op2.Var);
                op.Outputs.Clear(op2.Var);
            }
            return node3;
        }

        private SimpleCollectionColumnMap ExpandColumnMap(SimpleCollectionColumnMap columnMap)
        {
            System.Data.Query.PlanCompiler.VarInfo info;
            VarRefColumnMap element = columnMap.Element as VarRefColumnMap;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(element != null, "Encountered a SimpleCollectionColumnMap element that is not VarRefColumnMap when expanding a column map in NominalTypeEliminator.");
            if (!this.m_varInfoMap.TryGetVarInfo(element.Var, out info))
            {
                return columnMap;
            }
            if (TypeUtils.IsStructuredType(element.Var.Type))
            {
                TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(element.Var.Type);
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(typeInfo.RootType.FlattenedType.Properties.Count == info.NewVars.Count, string.Concat(new object[] { "Var count mismatch; Expected ", typeInfo.RootType.FlattenedType.Properties.Count, "; got ", info.NewVars.Count, " instead." }));
            }
            ColumnMap elementMap = new ColumnMapProcessor(element, info, this.m_typeInfo).ExpandColumnMap();
            return new SimpleCollectionColumnMap(TypeUtils.CreateCollectionType(elementMap.Type), elementMap.Name, elementMap, columnMap.Keys, columnMap.ForeignKeys, columnMap.SortKeys);
        }

        private Node FixupSetOpChild(Node setOpChild, VarMap varMap, List<ComputedVar> newComputedVars)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(null != setOpChild, "null setOpChild?");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(null != varMap, "null varMap?");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(null != newComputedVars, "null newComputedVars?");
            VarVec vars = this.m_command.CreateVarVec();
            foreach (KeyValuePair<Var, Var> pair in varMap)
            {
                vars.Set(pair.Value);
            }
            List<Node> args = new List<Node>();
            foreach (Var var in newComputedVars)
            {
                VarDefOp op = this.m_command.CreateVarDefOp(var);
                Node item = this.m_command.CreateNode(op, this.CreateNullConstantNode(var.Type));
                args.Add(item);
            }
            Node node2 = this.m_command.CreateNode(this.m_command.CreateVarDefListOp(), args);
            ProjectOp op2 = this.m_command.CreateProjectOp(vars);
            return this.m_command.CreateNode(op2, setOpChild, node2);
        }

        private Node FlattenCaseOp(CaseOp op, Node n, TypeInfo typeInfo, PropertyRefList desiredProperties)
        {
            List<EdmProperty> fields = new List<EdmProperty>();
            List<Node> args = new List<Node>();
            foreach (PropertyRef ref2 in typeInfo.PropertyRefList)
            {
                if (desiredProperties.Contains(ref2))
                {
                    EdmProperty newProperty = typeInfo.GetNewProperty(ref2);
                    List<Node> list3 = new List<Node>();
                    for (int i = 0; i < (n.Children.Count - 1); i++)
                    {
                        Node node = this.Copy(n.Children[i]);
                        list3.Add(node);
                        i++;
                        Node node2 = this.BuildAccessorWithNulls(n.Children[i], newProperty);
                        list3.Add(node2);
                    }
                    Node item = this.BuildAccessorWithNulls(n.Children[n.Children.Count - 1], newProperty);
                    list3.Add(item);
                    Node node4 = this.m_command.CreateNode(this.m_command.CreateCaseOp(Helper.GetModelTypeUsage(newProperty)), list3);
                    fields.Add(newProperty);
                    args.Add(node4);
                }
            }
            NewRecordOp op2 = this.m_command.CreateNewRecordOp(typeInfo.FlattenedTypeUsage, fields);
            return this.m_command.CreateNode(op2, args);
        }

        private void FlattenComputedVar(ComputedVar v, Node node, out List<Node> newNodes, out TypeUsage newType)
        {
            newNodes = new List<Node>();
            Node definingExpr = node.Child0;
            newType = null;
            if (TypeUtils.IsCollectionType(v.Type))
            {
                Var var;
                newType = this.GetNewType(v.Type);
                Node item = this.m_command.CreateVarDefNode(definingExpr, out var);
                newNodes.Add(item);
                this.m_varInfoMap.CreateCollectionVarInfo(v, var);
            }
            else
            {
                TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(v.Type);
                PropertyRefList list = this.m_varPropertyMap[v];
                List<Var> newVars = new List<Var>();
                List<EdmProperty> newProperties = new List<EdmProperty>();
                newNodes = new List<Node>();
                bool flag = false;
                foreach (PropertyRef ref2 in typeInfo.PropertyRefList)
                {
                    if (list.Contains(ref2))
                    {
                        EdmProperty newProperty = typeInfo.GetNewProperty(ref2);
                        Node node4 = null;
                        if (list.AllProperties)
                        {
                            node4 = this.BuildAccessorWithNulls(definingExpr, newProperty);
                        }
                        else
                        {
                            node4 = this.BuildAccessor(definingExpr, newProperty);
                            if (node4 == null)
                            {
                                continue;
                            }
                        }
                        newProperties.Add(newProperty);
                        if (!flag && (node4.Op.OpType == OpType.VarRef))
                        {
                            newVars.Add(((VarRefOp) node4.Op).Var);
                        }
                        else
                        {
                            Var var2;
                            Node node5 = this.m_command.CreateVarDefNode(node4, out var2);
                            newNodes.Add(node5);
                            newVars.Add(var2);
                        }
                        if (TypeUtils.IsCollectionType(node4.Op.Type))
                        {
                            flag = true;
                        }
                    }
                }
                this.m_varInfoMap.CreateStructuredVarInfo(v, typeInfo.FlattenedType, newVars, newProperties);
            }
        }

        private Node FlattenConstructor(ScalarOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((((op.OpType == OpType.NewInstance) || (op.OpType == OpType.NewRecord)) || (op.OpType == OpType.DiscriminatedNewEntity)) || (op.OpType == OpType.NewEntity), "unexpected op: " + op.OpType + "?");
            this.VisitChildren(n);
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.Type);
            RowType flattenedType = typeInfo.FlattenedType;
            NewEntityBaseOp op2 = op as NewEntityBaseOp;
            IEnumerable properties = null;
            DiscriminatedNewEntityOp op3 = null;
            if (op.OpType == OpType.NewRecord)
            {
                properties = ((NewRecordOp) op).Properties;
            }
            else if (op.OpType == OpType.DiscriminatedNewEntity)
            {
                op3 = (DiscriminatedNewEntityOp) op;
                properties = op3.DiscriminatorMap.Properties;
            }
            else
            {
                properties = TypeHelpers.GetAllStructuralMembers(op.Type);
            }
            List<EdmProperty> fields = new List<EdmProperty>();
            List<Node> args = new List<Node>();
            if (typeInfo.HasTypeIdProperty)
            {
                fields.Add(typeInfo.TypeIdProperty);
                if (op3 == null)
                {
                    args.Add(this.CreateTypeIdConstant(typeInfo));
                }
                else
                {
                    Node discriminator = n.Children[0];
                    if (typeInfo.RootType.DiscriminatorMap == null)
                    {
                        discriminator = this.NormalizeTypeDiscriminatorValues(op3, discriminator);
                    }
                    args.Add(discriminator);
                }
            }
            if (typeInfo.HasEntitySetIdProperty)
            {
                fields.Add(typeInfo.EntitySetIdProperty);
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op2 != null, "unexpected optype:" + op.OpType);
                Node entitySetIdExpr = this.GetEntitySetIdExpr(typeInfo.EntitySetIdProperty, op2);
                args.Add(entitySetIdExpr);
            }
            if (typeInfo.HasNullSentinelProperty)
            {
                fields.Add(typeInfo.NullSentinelProperty);
                args.Add(this.CreateNullSentinelConstant());
            }
            int num = (op3 == null) ? 0 : 1;
            foreach (EdmMember member in properties)
            {
                Node input = n.Children[num];
                if (TypeUtils.IsStructuredType(Helper.GetModelTypeUsage(member)))
                {
                    RowType type2 = this.m_typeInfo.GetTypeInfo(Helper.GetModelTypeUsage(member)).FlattenedType;
                    int nestedStructureOffset = typeInfo.RootType.GetNestedStructureOffset(new SimplePropertyRef(member));
                    foreach (EdmProperty property in type2.Properties)
                    {
                        Node item = this.BuildAccessor(input, property);
                        if (item != null)
                        {
                            fields.Add(flattenedType.Properties[nestedStructureOffset]);
                            args.Add(item);
                        }
                        nestedStructureOffset++;
                    }
                }
                else
                {
                    PropertyRef propertyRef = new SimplePropertyRef(member);
                    EdmProperty newProperty = typeInfo.GetNewProperty(propertyRef);
                    fields.Add(newProperty);
                    args.Add(input);
                }
                num++;
            }
            if (op2 != null)
            {
                foreach (RelProperty property3 in op2.RelationshipProperties)
                {
                    Node node5 = n.Children[num];
                    RowType type3 = this.m_typeInfo.GetTypeInfo(property3.ToEnd.TypeUsage).FlattenedType;
                    int num3 = typeInfo.RootType.GetNestedStructureOffset(new RelPropertyRef(property3));
                    foreach (EdmProperty property4 in type3.Properties)
                    {
                        Node node6 = this.BuildAccessor(node5, property4);
                        if (node6 != null)
                        {
                            fields.Add(flattenedType.Properties[num3]);
                            args.Add(node6);
                        }
                        num3++;
                    }
                    num++;
                }
            }
            NewRecordOp op4 = this.m_command.CreateNewRecordOp(typeInfo.FlattenedTypeUsage, fields);
            return this.m_command.CreateNode(op4, args);
        }

        private Node FlattenGetKeyOp(ScalarOp op, Node n)
        {
            List<EdmProperty> list;
            List<Node> list2;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((op.OpType == OpType.GetEntityRef) || (op.OpType == OpType.GetRefKey), "Expecting GetEntityRef or GetRefKey ops");
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(((ScalarOp) n.Child0.Op).Type);
            TypeInfo info2 = this.m_typeInfo.GetTypeInfo(op.Type);
            this.VisitChildren(n);
            if (op.OpType == OpType.GetRefKey)
            {
                this.GetPropertyValues(typeInfo, OperationKind.GetKeys, n.Child0, false, out list, out list2);
            }
            else
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.OpType == OpType.GetEntityRef, "Expected OpType.GetEntityRef: Found " + op.OpType);
                this.GetPropertyValues(typeInfo, OperationKind.GetIdentity, n.Child0, false, out list, out list2);
            }
            if (info2.HasNullSentinelProperty && !typeInfo.HasNullSentinelProperty)
            {
                list2.Insert(0, this.CreateNullSentinelConstant());
            }
            List<EdmProperty> fields = new List<EdmProperty>(info2.FlattenedType.Properties);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(list2.Count == fields.Count, "fieldTypes.Count mismatch?");
            NewRecordOp op2 = this.m_command.CreateNewRecordOp(info2.FlattenedTypeUsage, fields);
            return this.m_command.CreateNode(op2, list2);
        }

        private System.Data.Query.PlanCompiler.VarInfo FlattenSetOpVar(SetOpVar v)
        {
            if (TypeUtils.IsCollectionType(v.Type))
            {
                TypeUsage newType = this.GetNewType(v.Type);
                Var newVar = this.m_command.CreateSetOpVar(newType);
                return this.m_varInfoMap.CreateCollectionVarInfo(v, newVar);
            }
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(v.Type);
            PropertyRefList list = this.m_varPropertyMap[v];
            List<Var> newVars = new List<Var>();
            List<EdmProperty> newProperties = new List<EdmProperty>();
            foreach (PropertyRef ref2 in typeInfo.PropertyRefList)
            {
                if (list.Contains(ref2))
                {
                    EdmProperty newProperty = typeInfo.GetNewProperty(ref2);
                    newProperties.Add(newProperty);
                    SetOpVar item = this.m_command.CreateSetOpVar(Helper.GetModelTypeUsage(newProperty));
                    newVars.Add(item);
                }
            }
            return this.m_varInfoMap.CreateStructuredVarInfo(v, typeInfo.FlattenedType, newVars, newProperties);
        }

        private VarList FlattenVarList(VarList varList) => 
            Command.CreateVarList(this.FlattenVars(varList));

        private VarMap FlattenVarMap(VarMap varMap, out List<ComputedVar> newComputedVars)
        {
            newComputedVars = null;
            VarMap map = new VarMap();
            foreach (KeyValuePair<Var, Var> pair in varMap)
            {
                System.Data.Query.PlanCompiler.VarInfo info;
                if (!this.m_varInfoMap.TryGetVarInfo(pair.Value, out info))
                {
                    map.Add(pair.Key, pair.Value);
                }
                else
                {
                    System.Data.Query.PlanCompiler.VarInfo info2;
                    if (!this.m_varInfoMap.TryGetVarInfo(pair.Key, out info2))
                    {
                        info2 = this.FlattenSetOpVar((SetOpVar) pair.Key);
                    }
                    if (info2.IsCollectionType)
                    {
                        map.Add(((CollectionVarInfo) info2).NewVar, ((CollectionVarInfo) info).NewVar);
                    }
                    else
                    {
                        StructuredVarInfo info3 = (StructuredVarInfo) info2;
                        StructuredVarInfo info4 = (StructuredVarInfo) info;
                        foreach (EdmProperty property in info3.Fields)
                        {
                            Var var;
                            Var var2;
                            System.Data.Query.PlanCompiler.PlanCompiler.Assert(info3.TryGetVar(property, out var), "Could not find VarInfo for prop " + property.Name);
                            if (!info4.TryGetVar(property, out var2))
                            {
                                var2 = this.m_command.CreateComputedVar(var.Type);
                                if (newComputedVars == null)
                                {
                                    newComputedVars = new List<ComputedVar>();
                                }
                                newComputedVars.Add((ComputedVar) var2);
                            }
                            map.Add(var, var2);
                        }
                    }
                }
            }
            return map;
        }

        private IEnumerable<Var> FlattenVars(IEnumerable<Var> vars)
        {
            foreach (Var iteratorVariable0 in vars)
            {
                System.Data.Query.PlanCompiler.VarInfo iteratorVariable1;
                if (!this.m_varInfoMap.TryGetVarInfo(iteratorVariable0, out iteratorVariable1))
                {
                    yield return iteratorVariable0;
                }
                else
                {
                    foreach (Var iteratorVariable2 in iteratorVariable1.NewVars)
                    {
                        yield return iteratorVariable2;
                    }
                }
            }
        }

        private VarVec FlattenVarSet(VarVec varSet) => 
            this.m_command.CreateVarVec(this.FlattenVars(varSet));

        private Node GetEntitySetIdExpr(EdmProperty entitySetIdProperty, NewEntityBaseOp op)
        {
            EntitySet entitySet = op.EntitySet as EntitySet;
            if (entitySet != null)
            {
                int entitySetId = this.m_typeInfo.GetEntitySetId(entitySet);
                InternalConstantOp op2 = this.m_command.CreateInternalConstantOp(Helper.GetModelTypeUsage(entitySetIdProperty), entitySetId);
                return this.m_command.CreateNode(op2);
            }
            return this.CreateNullConstantNode(Helper.GetModelTypeUsage(entitySetIdProperty));
        }

        private TypeUsage GetNewType(TypeUsage type)
        {
            TypeUsage flattenedTypeUsage;
            if (!this.m_typeToNewTypeMap.TryGetValue(type, out flattenedTypeUsage))
            {
                CollectionType type2;
                if (TypeHelpers.TryGetEdmType<CollectionType>(type, out type2))
                {
                    flattenedTypeUsage = TypeUtils.CreateCollectionType(this.GetNewType(type2.TypeUsage));
                }
                else if (TypeUtils.IsStructuredType(type))
                {
                    flattenedTypeUsage = this.m_typeInfo.GetTypeInfo(type).FlattenedTypeUsage;
                }
                else
                {
                    flattenedTypeUsage = type;
                }
                this.m_typeToNewTypeMap[type] = flattenedTypeUsage;
            }
            return flattenedTypeUsage;
        }

        private IEnumerable<EdmProperty> GetProperties(TypeInfo typeInfo, OperationKind opKind)
        {
            if (opKind == OperationKind.All)
            {
                foreach (EdmProperty iteratorVariable0 in typeInfo.GetAllProperties())
                {
                    yield return iteratorVariable0;
                }
            }
            else
            {
                IEnumerator<PropertyRef> enumerator = this.GetPropertyRefs(typeInfo, opKind).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    PropertyRef current = enumerator.Current;
                    yield return typeInfo.GetNewProperty(current);
                }
            }
        }

        private IEnumerable<PropertyRef> GetPropertyRefs(TypeInfo typeInfo, OperationKind opKind)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(opKind != OperationKind.All, "unexpected attempt to GetPropertyRefs(...,OperationKind.All)");
            if (opKind == OperationKind.GetKeys)
            {
                return typeInfo.GetKeyPropertyRefs();
            }
            if (opKind == OperationKind.GetIdentity)
            {
                return typeInfo.GetIdentityPropertyRefs();
            }
            return this.GetPropertyRefsForComparisonAndIsNull(typeInfo, opKind);
        }

        private IEnumerable<PropertyRef> GetPropertyRefsForComparisonAndIsNull(TypeInfo typeInfo, OperationKind opKind)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((opKind == OperationKind.IsNull) || (opKind == OperationKind.Equality), "Unexpected opKind: " + opKind + "; Can only handle IsNull and Equality");
            TypeUsage type = typeInfo.Type;
            RowType iteratorVariable1 = null;
            if (TypeHelpers.TryGetEdmType<RowType>(type, out iteratorVariable1))
            {
                if ((opKind == OperationKind.IsNull) && typeInfo.HasNullSentinelProperty)
                {
                    yield return NullSentinelPropertyRef.Instance;
                }
                else
                {
                    foreach (EdmProperty iteratorVariable2 in iteratorVariable1.Properties)
                    {
                        if (!TypeUtils.IsStructuredType(Helper.GetModelTypeUsage(iteratorVariable2)))
                        {
                            yield return new SimplePropertyRef(iteratorVariable2);
                        }
                        else
                        {
                            TypeInfo iteratorVariable3 = this.m_typeInfo.GetTypeInfo(Helper.GetModelTypeUsage(iteratorVariable2));
                            foreach (PropertyRef iteratorVariable4 in this.GetPropertyRefs(iteratorVariable3, opKind))
                            {
                                PropertyRef iteratorVariable5 = iteratorVariable4.CreateNestedPropertyRef(iteratorVariable2);
                                yield return iteratorVariable5;
                            }
                        }
                    }
                }
            }
            else
            {
                EntityType iteratorVariable6 = null;
                if (TypeHelpers.TryGetEdmType<EntityType>(type, out iteratorVariable6))
                {
                    if ((opKind != OperationKind.Equality) && ((opKind != OperationKind.IsNull) || typeInfo.HasTypeIdProperty))
                    {
                        yield return TypeIdPropertyRef.Instance;
                    }
                    else
                    {
                        foreach (PropertyRef iteratorVariable7 in typeInfo.GetIdentityPropertyRefs())
                        {
                            yield return iteratorVariable7;
                        }
                    }
                }
                else
                {
                    ComplexType iteratorVariable8 = null;
                    if (TypeHelpers.TryGetEdmType<ComplexType>(type, out iteratorVariable8))
                    {
                        System.Data.Query.PlanCompiler.PlanCompiler.Assert(opKind == OperationKind.IsNull, "complex types not equality-comparable");
                        System.Data.Query.PlanCompiler.PlanCompiler.Assert(typeInfo.HasTypeIdProperty, "complex type with no typeid property: can't handle isNull");
                        yield return TypeIdPropertyRef.Instance;
                    }
                    else
                    {
                        RefType iteratorVariable9 = null;
                        if (TypeHelpers.TryGetEdmType<RefType>(type, out iteratorVariable9))
                        {
                            foreach (PropertyRef iteratorVariable10 in typeInfo.GetAllPropertyRefs())
                            {
                                yield return iteratorVariable10;
                            }
                        }
                        else
                        {
                            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Unknown type");
                        }
                    }
                }
            }
        }

        private KeyValuePair<EdmProperty, Node> GetPropertyValue(Node input, EdmProperty property, bool ignoreMissingProperties)
        {
            Node node = null;
            if (!ignoreMissingProperties)
            {
                node = this.BuildAccessorWithNulls(input, property);
            }
            else
            {
                node = this.BuildAccessor(input, property);
            }
            return new KeyValuePair<EdmProperty, Node>(property, node);
        }

        private void GetPropertyValues(TypeInfo typeInfo, OperationKind opKind, Node input, bool ignoreMissingProperties, out List<EdmProperty> properties, out List<Node> values)
        {
            values = new List<Node>();
            properties = new List<EdmProperty>();
            foreach (EdmProperty property in this.GetProperties(typeInfo, opKind))
            {
                KeyValuePair<EdmProperty, Node> pair = this.GetPropertyValue(input, property, ignoreMissingProperties);
                if (pair.Value != null)
                {
                    properties.Add(pair.Key);
                    values.Add(pair.Value);
                }
            }
        }

        internal static Var GetSingletonVar(Node n)
        {
            switch (n.Op.OpType)
            {
                case OpType.ScanTable:
                {
                    ScanTableOp op2 = (ScanTableOp) n.Op;
                    if (op2.Table.Columns.Count == 1)
                    {
                        return op2.Table.Columns[0];
                    }
                    return null;
                }
                case OpType.Filter:
                case OpType.Sort:
                case OpType.ConstrainedSort:
                case OpType.SingleRow:
                    return GetSingletonVar(n.Child0);

                case OpType.Project:
                {
                    ProjectOp op = (ProjectOp) n.Op;
                    if (op.Outputs.Count == 1)
                    {
                        return op.Outputs.First;
                    }
                    return null;
                }
                case OpType.Unnest:
                {
                    UnnestOp op4 = (UnnestOp) n.Op;
                    if (op4.Table.Columns.Count == 1)
                    {
                        return op4.Table.Columns[0];
                    }
                    return null;
                }
                case OpType.UnionAll:
                case OpType.Intersect:
                case OpType.Except:
                {
                    SetOp op3 = (SetOp) n.Op;
                    if (op3.Outputs.Count == 1)
                    {
                        return op3.Outputs.First;
                    }
                    return null;
                }
                case OpType.Distinct:
                {
                    DistinctOp op5 = (DistinctOp) n.Op;
                    if (op5.Keys.Count == 1)
                    {
                        return op5.Keys.First;
                    }
                    return null;
                }
            }
            return null;
        }

        private List<SortKey> HandleSortKeys(List<SortKey> keys)
        {
            List<SortKey> list = new List<SortKey>();
            bool flag = false;
            foreach (SortKey key in keys)
            {
                System.Data.Query.PlanCompiler.VarInfo info;
                if (!this.m_varInfoMap.TryGetVarInfo(key.Var, out info))
                {
                    list.Add(key);
                }
                else
                {
                    foreach (Var var in info.NewVars)
                    {
                        SortKey item = Command.CreateSortKey(var, key.AscendingSort, key.Collation);
                        list.Add(item);
                    }
                    flag = true;
                }
            }
            return (flag ? list : keys);
        }

        private Node NormalizeTypeDiscriminatorValues(DiscriminatedNewEntityOp op, Node discriminator)
        {
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.Type);
            CaseOp op2 = this.m_command.CreateCaseOp(typeInfo.RootType.TypeIdProperty.TypeUsage);
            List<Node> args = new List<Node>((op.DiscriminatorMap.TypeMap.Count * 2) - 1);
            for (int i = 0; i < op.DiscriminatorMap.TypeMap.Count; i++)
            {
                KeyValuePair<object, EntityType> pair = op.DiscriminatorMap.TypeMap[i];
                object key = pair.Key;
                KeyValuePair<object, EntityType> pair2 = op.DiscriminatorMap.TypeMap[i];
                EntityType edmType = pair2.Value;
                TypeInfo info2 = this.m_typeInfo.GetTypeInfo(TypeUsage.Create(edmType));
                Node item = this.CreateTypeIdConstant(info2);
                if (i == (op.DiscriminatorMap.TypeMap.Count - 1))
                {
                    args.Add(item);
                }
                else
                {
                    ConstantBaseOp op3 = this.m_command.CreateConstantOp(Helper.GetModelTypeUsage(op.DiscriminatorMap.DiscriminatorProperty.TypeUsage), key);
                    Node node2 = this.m_command.CreateNode(op3);
                    ComparisonOp op4 = this.m_command.CreateComparisonOp(OpType.EQ);
                    Node node3 = this.m_command.CreateNode(op4, discriminator, node2);
                    args.Add(node3);
                    args.Add(item);
                }
            }
            discriminator = this.m_command.CreateNode(op2, args);
            return discriminator;
        }

        private void Process()
        {
            Node root = this.m_command.Root;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(root.Op.OpType == OpType.PhysicalProject, "root node is not PhysicalProjectOp?");
            root.Op.Accept<Node>(this, root);
        }

        internal static void Process(System.Data.Query.PlanCompiler.PlanCompiler compilerState, StructuredTypeInfo structuredTypeInfo)
        {
            Dictionary<Var, PropertyRefList> dictionary;
            Dictionary<Node, PropertyRefList> dictionary2;
            PropertyPushdownHelper.Process(compilerState.Command, structuredTypeInfo, out dictionary, out dictionary2);
            new NominalTypeEliminator(compilerState, structuredTypeInfo, dictionary, dictionary2).Process();
        }

        private bool TryRewriteCaseOp(Node n, bool thenClauseIsNull, out Node rewrittenNode)
        {
            rewrittenNode = n;
            if (!this.m_typeInfo.GetTypeInfo(n.Op.Type).HasNullSentinelProperty)
            {
                return false;
            }
            Node node = thenClauseIsNull ? n.Child2 : n.Child1;
            if (node.Op.OpType != OpType.NewRecord)
            {
                return false;
            }
            Node node2 = node.Child0;
            TypeUsage integerType = this.m_command.IntegerType;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node2.Op.Type.EdmEquals(integerType), "Column that is expected to be a null sentinel is not of Integer type.");
            CaseOp op = this.m_command.CreateCaseOp(integerType);
            List<Node> args = new List<Node>(3) {
                n.Child0
            };
            Node node3 = this.m_command.CreateNode(this.m_command.CreateNullOp(integerType));
            Node item = thenClauseIsNull ? node3 : node2;
            Node node5 = thenClauseIsNull ? node2 : node3;
            args.Add(item);
            args.Add(node5);
            node.Child0 = this.m_command.CreateNode(op, args);
            rewrittenNode = node;
            return true;
        }

        public override Node Visit(CaseOp op, Node n)
        {
            bool flag;
            Node node;
            bool flag2 = PlanCompilerUtil.IsRowTypeCaseOpWithNullability(op, n, out flag);
            this.VisitChildren(n);
            if (flag2 && this.TryRewriteCaseOp(n, flag, out node))
            {
                return node;
            }
            if (TypeUtils.IsCollectionType(op.Type))
            {
                TypeUsage newType = this.GetNewType(op.Type);
                n.Op = this.m_command.CreateCaseOp(newType);
                return n;
            }
            if (TypeUtils.IsStructuredType(op.Type))
            {
                PropertyRefList desiredProperties = this.m_nodePropertyMap[n];
                return this.FlattenCaseOp(op, n, this.m_typeInfo.GetTypeInfo(op.Type), desiredProperties);
            }
            return n;
        }

        public override Node Visit(CollectOp op, Node n)
        {
            this.VisitChildren(n);
            n.Op = this.m_command.CreateCollectOp(this.GetNewType(op.Type));
            return n;
        }

        public override Node Visit(ComparisonOp op, Node n)
        {
            List<EdmProperty> list;
            List<EdmProperty> list2;
            List<Node> list3;
            List<Node> list4;
            TypeUsage type = ((ScalarOp) n.Child0.Op).Type;
            TypeUsage usage2 = ((ScalarOp) n.Child1.Op).Type;
            if (!TypeUtils.IsStructuredType(type))
            {
                return this.VisitScalarOpDefault(op, n);
            }
            this.VisitChildren(n);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!TypeSemantics.IsComplexType(type) && !TypeSemantics.IsComplexType(usage2), "complex type?");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((op.OpType == OpType.EQ) || (op.OpType == OpType.NE), "non-equality comparison of structured types?");
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(type);
            TypeInfo info2 = this.m_typeInfo.GetTypeInfo(usage2);
            this.GetPropertyValues(typeInfo, OperationKind.Equality, n.Child0, false, out list, out list3);
            this.GetPropertyValues(info2, OperationKind.Equality, n.Child1, false, out list2, out list4);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((list.Count == list2.Count) && (list3.Count == list4.Count), "different shaped structured types?");
            Node node = null;
            for (int i = 0; i < list3.Count; i++)
            {
                ComparisonOp op2 = this.m_command.CreateComparisonOp(op.OpType);
                Node node2 = this.m_command.CreateNode(op2, list3[i], list4[i]);
                if (node == null)
                {
                    node = node2;
                }
                else
                {
                    node = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.And), node, node2);
                }
            }
            return node;
        }

        public override Node Visit(ConditionalOp op, Node n)
        {
            if (op.OpType != OpType.IsNull)
            {
                return this.VisitScalarOpDefault(op, n);
            }
            TypeUsage type = ((ScalarOp) n.Child0.Op).Type;
            if (!TypeUtils.IsStructuredType(type))
            {
                return this.VisitScalarOpDefault(op, n);
            }
            this.VisitChildren(n);
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(type);
            List<EdmProperty> properties = null;
            List<Node> values = null;
            this.GetPropertyValues(typeInfo, OperationKind.IsNull, n.Child0, false, out properties, out values);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((properties.Count == values.Count) && (properties.Count > 0), "No properties returned from GetPropertyValues(IsNull)?");
            Node node = null;
            foreach (Node node2 in values)
            {
                Node node3 = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.IsNull), node2);
                if (node == null)
                {
                    node = node3;
                }
                else
                {
                    node = this.m_command.CreateNode(this.m_command.CreateConditionalOp(OpType.And), node, node3);
                }
            }
            return node;
        }

        public override Node Visit(ConstrainedSortOp op, Node n)
        {
            this.VisitChildren(n);
            List<SortKey> sortKeys = this.HandleSortKeys(op.Keys);
            if (sortKeys != op.Keys)
            {
                n.Op = this.m_command.CreateConstrainedSortOp(sortKeys, op.WithTies);
            }
            return n;
        }

        public override Node Visit(DiscriminatedNewEntityOp op, Node n) => 
            this.FlattenConstructor(op, n);

        public override Node Visit(DistinctOp op, Node n)
        {
            this.VisitChildren(n);
            VarVec keyVars = this.FlattenVarSet(op.Keys);
            n.Op = this.m_command.CreateDistinctOp(keyVars);
            return n;
        }

        public override Node Visit(GetEntityRefOp op, Node n) => 
            this.FlattenGetKeyOp(op, n);

        public override Node Visit(GetRefKeyOp op, Node n) => 
            this.FlattenGetKeyOp(op, n);

        public override Node Visit(GroupByOp op, Node n)
        {
            this.VisitChildren(n);
            VarVec gbyKeys = this.FlattenVarSet(op.Keys);
            VarVec outputs = this.FlattenVarSet(op.Outputs);
            if ((gbyKeys != op.Keys) || (outputs != op.Outputs))
            {
                n.Op = this.m_command.CreateGroupByOp(gbyKeys, outputs);
            }
            return this.EliminateInternalConstantAndNullGroupByKeys(n);
        }

        public override Node Visit(IsOfOp op, Node n)
        {
            this.VisitChildren(n);
            if (!TypeUtils.IsStructuredType(op.IsOfType))
            {
                return n;
            }
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.IsOfType);
            return this.CreateTypeComparisonOp(n.Child0, typeInfo, op.IsOfOnly);
        }

        public override Node Visit(NewEntityOp op, Node n) => 
            this.FlattenConstructor(op, n);

        public override Node Visit(NewInstanceOp op, Node n) => 
            this.FlattenConstructor(op, n);

        public override Node Visit(NewMultisetOp op, Node n)
        {
            this.VisitChildren(n);
            n.Op = this.m_command.CreateNewMultisetOp(this.GetNewType(op.Type));
            return n;
        }

        public override Node Visit(NewRecordOp op, Node n) => 
            this.FlattenConstructor(op, n);

        public override Node Visit(NullOp op, Node n)
        {
            if (!TypeUtils.IsStructuredType(op.Type))
            {
                return n;
            }
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.Type);
            List<EdmProperty> fields = new List<EdmProperty>();
            List<Node> args = new List<Node>();
            if (typeInfo.HasTypeIdProperty)
            {
                fields.Add(typeInfo.TypeIdProperty);
                TypeUsage modelTypeUsage = Helper.GetModelTypeUsage(typeInfo.TypeIdProperty);
                args.Add(this.CreateNullConstantNode(modelTypeUsage));
            }
            NewRecordOp op2 = new NewRecordOp(typeInfo.FlattenedTypeUsage, fields);
            return this.m_command.CreateNode(op2, args);
        }

        public override Node Visit(PhysicalProjectOp op, Node n)
        {
            this.VisitChildren(n);
            VarList outputVars = this.FlattenVarList(op.Outputs);
            SimpleCollectionColumnMap columnMap = this.ExpandColumnMap(op.ColumnMap);
            PhysicalProjectOp op2 = this.m_command.CreatePhysicalProjectOp(outputVars, columnMap);
            n.Op = op2;
            return n;
        }

        public override Node Visit(ProjectOp op, Node n)
        {
            this.VisitChildren(n);
            VarVec vars = this.FlattenVarSet(op.Outputs);
            if (op.Outputs != vars)
            {
                if (vars.IsEmpty)
                {
                    return n.Child0;
                }
                n.Op = this.m_command.CreateProjectOp(vars);
            }
            return n;
        }

        public override Node Visit(PropertyOp op, Node n) => 
            this.VisitPropertyOp(op, n, new SimplePropertyRef(op.PropertyInfo));

        public override Node Visit(RefOp op, Node n)
        {
            List<EdmProperty> list;
            List<Node> list2;
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(((ScalarOp) n.Child0.Op).Type);
            TypeInfo info2 = this.m_typeInfo.GetTypeInfo(op.Type);
            this.VisitChildren(n);
            this.GetPropertyValues(typeInfo, OperationKind.All, n.Child0, false, out list, out list2);
            List<EdmProperty> fields = new List<EdmProperty>(info2.FlattenedType.Properties);
            if (typeInfo.HasNullSentinelProperty && !info2.HasNullSentinelProperty)
            {
                while (list.Count > fields.Count)
                {
                    list.RemoveAt(0);
                    list2.RemoveAt(0);
                }
            }
            if (info2.HasEntitySetIdProperty)
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(fields.Count == (list.Count + 1), string.Concat(new object[] { "Mismatched field count: Expected ", list.Count + 1, "; Got ", fields.Count }));
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(fields[0] == info2.EntitySetIdProperty, "OutputField0 must be the entitySetId property");
                int entitySetId = this.m_typeInfo.GetEntitySetId(op.EntitySet);
                list2.Insert(0, this.m_command.CreateNode(this.m_command.CreateInternalConstantOp(Helper.GetModelTypeUsage(info2.EntitySetIdProperty), entitySetId)));
            }
            else
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(fields.Count == list.Count, string.Concat(new object[] { "Mismatched field count: Expected ", list.Count, "; Got ", fields.Count }));
            }
            NewRecordOp op2 = this.m_command.CreateNewRecordOp(info2.FlattenedTypeUsage, fields);
            return this.m_command.CreateNode(op2, list2);
        }

        public override Node Visit(RelPropertyOp op, Node n) => 
            this.VisitPropertyOp(op, n, new RelPropertyRef(op.PropertyInfo));

        public override Node Visit(ScanTableOp op, Node n)
        {
            Var v = op.Table.Columns[0];
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(v.Type);
            RowType flattenedType = typeInfo.FlattenedType;
            List<EdmProperty> properties = new List<EdmProperty>();
            List<EdmMember> keyMembers = new List<EdmMember>();
            HashSet<string> set = new HashSet<string>();
            foreach (EdmProperty property in TypeHelpers.GetAllStructuralMembers(v.Type.EdmType))
            {
                set.Add(property.Name);
            }
            foreach (EdmProperty property2 in flattenedType.Properties)
            {
                if (set.Contains(property2.Name))
                {
                    properties.Add(property2);
                }
            }
            foreach (PropertyRef ref2 in typeInfo.GetKeyPropertyRefs())
            {
                EdmProperty newProperty = typeInfo.GetNewProperty(ref2);
                keyMembers.Add(newProperty);
            }
            TableMD tableMetadata = this.m_command.CreateFlatTableDefinition(properties, keyMembers, op.Table.TableMetadata.Extent);
            Table table = this.m_command.CreateTableInstance(tableMetadata);
            this.m_varInfoMap.CreateStructuredVarInfo(v, flattenedType, table.Columns, properties);
            n.Op = this.m_command.CreateScanTableOp(table);
            return n;
        }

        public override Node Visit(ScanViewOp op, Node n)
        {
            System.Data.Query.PlanCompiler.VarInfo info;
            Var singletonVar = GetSingletonVar(n.Child0);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(singletonVar != null, "cannot identify Var for the input node to the ScanViewOp");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.Table.Columns.Count == 1, "table for scanViewOp has more than on column?");
            Var v = op.Table.Columns[0];
            Node node = base.VisitNode(n.Child0);
            if (!this.m_varInfoMap.TryGetVarInfo(singletonVar, out info))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "didn't find inputVar for scanViewOp?");
            }
            StructuredVarInfo info2 = (StructuredVarInfo) info;
            this.m_typeInfo.GetTypeInfo(v.Type);
            this.m_varInfoMap.CreateStructuredVarInfo(v, info2.NewType, info2.NewVars, info2.Fields);
            return node;
        }

        public override Node Visit(SoftCastOp op, Node n)
        {
            TypeUsage type = n.Child0.Op.Type;
            TypeUsage usage2 = op.Type;
            this.VisitChildren(n);
            TypeUsage newType = this.GetNewType(usage2);
            if (TypeSemantics.IsRowType(usage2))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Child0.Op.OpType == OpType.NewRecord, "Expected a record constructor here. Found " + n.Child0.Op.OpType + " instead");
                TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(type);
                TypeInfo info2 = this.m_typeInfo.GetTypeInfo(op.Type);
                NewRecordOp op2 = this.m_command.CreateNewRecordOp(newType);
                List<Node> args = new List<Node>();
                IEnumerator<EdmProperty> enumerator = op2.Properties.GetEnumerator();
                int count = op2.Properties.Count;
                enumerator.MoveNext();
                IEnumerator<Node> enumerator2 = n.Child0.Children.GetEnumerator();
                int num2 = n.Child0.Children.Count;
                enumerator2.MoveNext();
                while (num2 < count)
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(info2.HasNullSentinelProperty && !typeInfo.HasNullSentinelProperty, "NullSentinelProperty mismatch on input?");
                    args.Add(this.CreateNullSentinelConstant());
                    enumerator.MoveNext();
                    count--;
                }
                while (num2 > count)
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(!info2.HasNullSentinelProperty && typeInfo.HasNullSentinelProperty, "NullSentinelProperty mismatch on output?");
                    enumerator2.MoveNext();
                    num2--;
                }
                do
                {
                    EdmProperty current = enumerator.Current;
                    Node item = this.BuildSoftCast(enumerator2.Current, Helper.GetModelTypeUsage(current));
                    args.Add(item);
                    enumerator.MoveNext();
                }
                while (enumerator2.MoveNext());
                return this.m_command.CreateNode(op2, args);
            }
            if (TypeSemantics.IsCollectionType(usage2))
            {
                return this.BuildSoftCast(n.Child0, newType);
            }
            if (TypeSemantics.IsPrimitiveType(usage2))
            {
                return n;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsNominalType(usage2) || TypeSemantics.IsReferenceType(usage2), "Gasp! Not a nominal type or even a reference type");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(Command.EqualTypes(newType, n.Child0.Op.Type), "Types are not equal");
            return n.Child0;
        }

        public override Node Visit(SortOp op, Node n)
        {
            this.VisitChildren(n);
            List<SortKey> sortKeys = this.HandleSortKeys(op.Keys);
            if (sortKeys != op.Keys)
            {
                n.Op = this.m_command.CreateSortOp(sortKeys);
            }
            return n;
        }

        public override Node Visit(TreatOp op, Node n)
        {
            this.VisitChildren(n);
            ScalarOp op2 = (ScalarOp) n.Child0.Op;
            if ((op.IsFakeTreat || TypeSemantics.IsEquivalent(op2.Type, op.Type)) || TypeSemantics.IsSubTypeOf(op2.Type, op.Type))
            {
                return n.Child0;
            }
            if (!TypeUtils.IsStructuredType(op.Type))
            {
                return n;
            }
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(op.Type);
            Node node = this.CreateTypeComparisonOp(n.Child0, typeInfo, false);
            CaseOp op3 = this.m_command.CreateCaseOp(typeInfo.FlattenedTypeUsage);
            Node node2 = this.m_command.CreateNode(op3, node, n.Child0, this.CreateNullConstantNode(op3.Type));
            PropertyRefList desiredProperties = this.m_nodePropertyMap[n];
            return this.FlattenCaseOp(op3, node2, typeInfo, desiredProperties);
        }

        public override Node Visit(UnnestOp op, Node n)
        {
            System.Data.Query.PlanCompiler.VarInfo info;
            this.VisitChildren(n);
            if (n.HasChild0)
            {
                Node node = n.Child0;
                VarDefOp op2 = node.Op as VarDefOp;
                if ((op2 != null) && (TypeUtils.IsStructuredType(op2.Var.Type) || TypeUtils.IsCollectionType(op2.Var.Type)))
                {
                    TypeUsage usage;
                    List<Node> newNodes = new List<Node>();
                    this.FlattenComputedVar((ComputedVar) op2.Var, node, out newNodes, out usage);
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(newNodes.Count == 1, "Flattening unnest var produced more than one Var");
                    n.Child0 = newNodes[0];
                }
            }
            if (!this.m_varInfoMap.TryGetVarInfo(op.Var, out info))
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.WrongVarType);
            }
            if (!info.IsCollectionType)
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.WrongVarType);
            }
            Var newVar = ((CollectionVarInfo) info).NewVar;
            Table t = op.Table;
            Var v = op.Table.Columns[0];
            if (TypeUtils.IsStructuredType(v.Type))
            {
                RowType flattenedType = this.m_typeInfo.GetTypeInfo(v.Type).FlattenedType;
                TableMD tableMetadata = this.m_command.CreateFlatTableDefinition(flattenedType);
                t = this.m_command.CreateTableInstance(tableMetadata);
                this.m_varInfoMap.CreateStructuredVarInfo(v, flattenedType, t.Columns, flattenedType.Properties.ToList<EdmProperty>());
            }
            n.Op = this.m_command.CreateUnnestOp(newVar, t);
            return n;
        }

        public override Node Visit(VarDefListOp op, Node n)
        {
            this.VisitChildren(n);
            List<Node> args = new List<Node>();
            foreach (Node node in n.Children)
            {
                VarDefOp op2 = node.Op as VarDefOp;
                if (TypeUtils.IsStructuredType(op2.Var.Type) || TypeUtils.IsCollectionType(op2.Var.Type))
                {
                    List<Node> list2;
                    TypeUsage usage;
                    this.FlattenComputedVar((ComputedVar) op2.Var, node, out list2, out usage);
                    foreach (Node node2 in list2)
                    {
                        args.Add(node2);
                    }
                }
                else
                {
                    args.Add(node);
                }
            }
            return this.m_command.CreateNode(n.Op, args);
        }

        public override Node Visit(VarRefOp op, Node n)
        {
            System.Data.Query.PlanCompiler.VarInfo info;
            if (!this.m_varInfoMap.TryGetVarInfo(op.Var, out info))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(!TypeUtils.IsStructuredType(op.Type), string.Concat(new object[] { "No varInfo for a structured type var: Id = ", op.Var.Id, " Type = ", op.Type }));
                return n;
            }
            if (info.IsCollectionType)
            {
                n.Op = this.m_command.CreateVarRefOp(((CollectionVarInfo) info).NewVar);
                return n;
            }
            StructuredVarInfo info2 = (StructuredVarInfo) info;
            NewRecordOp op2 = this.m_command.CreateNewRecordOp(info2.NewTypeUsage, info2.Fields);
            List<Node> args = new List<Node>();
            foreach (Var var in info.NewVars)
            {
                VarRefOp op3 = this.m_command.CreateVarRefOp(var);
                args.Add(this.m_command.CreateNode(op3));
            }
            return this.m_command.CreateNode(op2, args);
        }

        private Node VisitPropertyOp(Op op, Node n, PropertyRef propertyRef)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((op.OpType == OpType.Property) || (op.OpType == OpType.RelProperty), "Unexpected optype: " + op.OpType);
            TypeUsage type = n.Child0.Op.Type;
            TypeUsage usage2 = op.Type;
            this.VisitChildren(n);
            if (TypeUtils.IsUdt(type))
            {
                return n;
            }
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(type);
            if (TypeUtils.IsStructuredType(usage2))
            {
                TypeInfo info2 = this.m_typeInfo.GetTypeInfo(usage2);
                List<EdmProperty> fields = new List<EdmProperty>();
                List<Node> args = new List<Node>();
                PropertyRefList list3 = this.m_nodePropertyMap[n];
                foreach (PropertyRef ref2 in info2.PropertyRefList)
                {
                    if (list3.Contains(ref2))
                    {
                        PropertyRef ref3 = ref2.CreateNestedPropertyRef(propertyRef);
                        EdmProperty item = info2.GetNewProperty(ref2);
                        EdmProperty property = typeInfo.GetNewProperty(ref3);
                        Node node2 = this.BuildAccessor(n.Child0, property);
                        if (node2 != null)
                        {
                            fields.Add(item);
                            args.Add(node2);
                        }
                    }
                }
                Op op2 = this.m_command.CreateNewRecordOp(info2.FlattenedTypeUsage, fields);
                return this.m_command.CreateNode(op2, args);
            }
            EdmProperty newProperty = typeInfo.GetNewProperty(propertyRef);
            return this.BuildAccessorWithNulls(n.Child0, newProperty);
        }

        protected override Node VisitSetOp(SetOp op, Node n)
        {
            this.VisitChildren(n);
            for (int i = 0; i < op.VarMap.Length; i++)
            {
                List<ComputedVar> list;
                op.VarMap[i] = this.FlattenVarMap(op.VarMap[i], out list);
                if (list != null)
                {
                    n.Children[i] = this.FixupSetOpChild(n.Children[i], op.VarMap[i], list);
                }
            }
            op.Outputs.Clear();
            foreach (Var var in op.VarMap[0].Keys)
            {
                op.Outputs.Set(var);
            }
            return n;
        }

        private TypeUsage DefaultTypeIdType =>
            this.m_command.StringType;

        private Command m_command =>
            this.m_compilerState.Command;




        internal enum OperationKind
        {
            Equality,
            IsNull,
            GetIdentity,
            GetKeys,
            All
        }
    }
}

