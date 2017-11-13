namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class PreProcessor : BasicOpVisitorOfNode
    {
        private Stack<Node> m_ancestors;
        private System.Data.Query.PlanCompiler.PlanCompiler m_compilerState;
        private readonly Dictionary<EntitySetBase, DiscriminatorMapInfo> m_discriminatorMaps;
        private List<EntityType> m_freeFloatingEntityConstructorTypes;
        private Dictionary<Node, List<Node>> m_nodeSubqueries;
        private HashSet<EntityContainer> m_referencedEntityContainers;
        private List<EntitySet> m_referencedEntitySets;
        private List<TypeUsage> m_referencedTypes;
        private RelPropertyHelper m_relPropertyHelper;
        private bool m_suppressDiscriminatorMaps;
        private HashSet<string> m_typesNeedingNullSentinel;
        private int m_viewNestingLevel;

        private PreProcessor(System.Data.Query.PlanCompiler.PlanCompiler planCompilerState)
        {
            this.m_compilerState = planCompilerState;
            this.m_ancestors = new Stack<Node>();
            this.m_nodeSubqueries = new Dictionary<Node, List<Node>>();
            this.m_viewNestingLevel = 0;
            this.m_referencedEntitySets = new List<EntitySet>();
            this.m_referencedTypes = new List<TypeUsage>();
            this.m_freeFloatingEntityConstructorTypes = new List<EntityType>();
            this.m_referencedEntityContainers = new HashSet<EntityContainer>();
            this.m_relPropertyHelper = new RelPropertyHelper(this.m_command.MetadataWorkspace, this.m_command.ReferencedRelProperties);
            this.m_discriminatorMaps = new Dictionary<EntitySetBase, DiscriminatorMapInfo>();
            this.m_typesNeedingNullSentinel = new HashSet<string>();
        }

        private void AddEntitySetReference(EntitySet entitySet)
        {
            this.m_referencedEntitySets.Add(entitySet);
            if (!this.m_referencedEntityContainers.Contains(entitySet.EntityContainer))
            {
                this.m_referencedEntityContainers.Add(entitySet.EntityContainer);
            }
        }

        private Node AddSubqueryToParentRelOp(Var outputVar, Node subquery)
        {
            Node relOpNode = this.FindRelOpAncestor();
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(relOpNode != null, "no ancestors found?");
            this.AddSubqueryToRelOpNode(relOpNode, subquery);
            subquery = this.m_command.CreateNode(this.m_command.CreateVarRefOp(outputVar));
            return subquery;
        }

        private void AddSubqueryToRelOpNode(Node relOpNode, Node subquery)
        {
            List<Node> list;
            if (!this.m_nodeSubqueries.TryGetValue(relOpNode, out list))
            {
                list = new List<Node>();
                this.m_nodeSubqueries[relOpNode] = list;
            }
            list.Add(subquery);
        }

        private void AddTypeReference(TypeUsage type)
        {
            if (TypeUtils.IsStructuredType(type) || TypeUtils.IsCollectionType(type))
            {
                this.m_referencedTypes.Add(type);
            }
        }

        private bool AreAllConstantsOrNulls(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if ((node.Op.OpType != OpType.Constant) && (node.Op.OpType != OpType.Null))
                {
                    return false;
                }
            }
            return true;
        }

        private Node AugmentWithSubqueries(Node input, List<Node> subqueries, bool inputFirst)
        {
            Node node;
            int num;
            if (inputFirst)
            {
                node = input;
                num = 0;
            }
            else
            {
                node = subqueries[0];
                num = 1;
            }
            for (int i = num; i < subqueries.Count; i++)
            {
                OuterApplyOp op = this.m_command.CreateOuterApplyOp();
                node = this.m_command.CreateNode(op, node, subqueries[i]);
            }
            if (!inputFirst)
            {
                node = this.m_command.CreateNode(this.m_command.CreateOuterApplyOp(), node, input);
            }
            this.m_compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.JoinElimination);
            return node;
        }

        private IEnumerable<Node> BuildAllRelPropertyExpressions(EntitySetBase entitySet, List<RelProperty> relPropertyList, Dictionary<RelProperty, Node> prebuiltExpressions, Node keyExpr)
        {
            foreach (RelProperty iteratorVariable0 in relPropertyList)
            {
                Node iteratorVariable1;
                if (!prebuiltExpressions.TryGetValue(iteratorVariable0, out iteratorVariable1))
                {
                    iteratorVariable1 = this.BuildRelPropertyExpression(entitySet, iteratorVariable0, keyExpr);
                }
                yield return iteratorVariable1;
            }
        }

        private Node BuildDummyProjectForExists(Node child)
        {
            Var var;
            return this.m_command.BuildProject(child, this.m_command.CreateNode(this.m_command.CreateInternalConstantOp(this.m_command.BooleanType, true)), out var);
        }

        private Node BuildJoinForNavProperty(RelationshipSet relSet, RelationshipEndMember end, out Var rsVar, out Var esVar)
        {
            EntitySetBase entitySet = FindTargetEntitySet(relSet, end);
            Node node = this.BuildOfTypeTable(relSet, null, out rsVar);
            Node node2 = this.BuildOfTypeTable(entitySet, TypeHelpers.GetElementTypeUsage(end.TypeUsage), out esVar);
            Node node3 = this.m_command.BuildComparison(OpType.EQ, this.m_command.CreateNode(this.m_command.CreateGetEntityRefOp(end.TypeUsage), this.m_command.CreateNode(this.m_command.CreateVarRefOp(esVar))), this.m_command.CreateNode(this.m_command.CreatePropertyOp(end), this.m_command.CreateNode(this.m_command.CreateVarRefOp(rsVar))));
            return this.m_command.CreateNode(this.m_command.CreateInnerJoinOp(), node, node2, node3);
        }

        private Node BuildKeyExpressionForNewEntityOp(Op op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((op.OpType == OpType.NewEntity) || (op.OpType == OpType.DiscriminatedNewEntity), "BuildKeyExpression: Unexpected OpType:" + op.OpType);
            int num = (op.OpType == OpType.DiscriminatedNewEntity) ? 1 : 0;
            EntityTypeBase edmType = (EntityTypeBase) op.Type.EdmType;
            List<Node> args = new List<Node>();
            List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>();
            foreach (EdmMember member in edmType.KeyMembers)
            {
                int num2 = this.FindPosition(edmType, member) + num;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.Children.Count > num2, string.Concat(new object[] { "invalid position ", num2, "; total count = ", n.Children.Count }));
                args.Add(n.Children[num2]);
                columns.Add(new KeyValuePair<string, TypeUsage>(member.Name, member.TypeUsage));
            }
            TypeUsage type = TypeHelpers.CreateRowTypeUsage(columns, true);
            NewRecordOp op2 = this.m_command.CreateNewRecordOp(type);
            return this.m_command.CreateNode(op2, args);
        }

        private Node BuildOfTypeTable(EntitySetBase entitySet, TypeUsage ofType, out Var resultVar)
        {
            Node node2;
            TableMD tableMetadata = Command.CreateTableDefinition(entitySet);
            ScanTableOp op = this.m_command.CreateScanTableOp(tableMetadata);
            Node inputNode = this.m_command.CreateNode(op);
            Var inputVar = op.Table.Columns[0];
            if ((ofType != null) && !entitySet.ElementType.EdmEquals(ofType.EdmType))
            {
                this.m_command.BuildOfTypeTree(inputNode, inputVar, ofType, true, out node2, out resultVar);
                return node2;
            }
            node2 = inputNode;
            resultVar = inputVar;
            return node2;
        }

        private Node BuildRelPropertyExpression(EntitySetBase entitySet, RelProperty relProperty, Node keyExpr)
        {
            keyExpr = OpCopier.Copy(this.m_command, keyExpr);
            RelationshipSet extent = this.FindRelationshipSet(entitySet, relProperty);
            if (extent == null)
            {
                return this.m_command.CreateNode(this.m_command.CreateNullOp(relProperty.ToEnd.TypeUsage));
            }
            ScanTableOp op = this.m_command.CreateScanTableOp(Command.CreateTableDefinition(extent));
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.Table.Columns.Count == 1, string.Concat(new object[] { "Unexpected column count for table:", op.Table.TableMetadata.Extent, "=", op.Table.Columns.Count }));
            Var v = op.Table.Columns[0];
            Node node = this.m_command.CreateNode(op);
            Node node2 = this.m_command.CreateNode(this.m_command.CreatePropertyOp(relProperty.FromEnd), this.m_command.CreateNode(this.m_command.CreateVarRefOp(v)));
            Node node3 = this.m_command.BuildComparison(OpType.EQ, keyExpr, this.m_command.CreateNode(this.m_command.CreateGetRefKeyOp(keyExpr.Op.Type), node2));
            Node n = this.m_command.CreateNode(this.m_command.CreateFilterOp(), node, node3);
            Node subquery = base.VisitNode(n);
            subquery = this.AddSubqueryToParentRelOp(v, subquery);
            return this.m_command.CreateNode(this.m_command.CreatePropertyOp(relProperty.ToEnd), subquery);
        }

        private Node BuildUnnest(Node collectionNode)
        {
            Var var;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(collectionNode.Op.IsScalarOp, "non-scalar usage of Unnest?");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsCollectionType(collectionNode.Op.Type), "non-collection usage for Unnest?");
            Node node = this.m_command.CreateVarDefNode(collectionNode, out var);
            UnnestOp op = this.m_command.CreateUnnestOp(var);
            return this.m_command.CreateNode(op, node);
        }

        private bool CanRewriteTypeTest(EdmType testType, EdmType argumentType)
        {
            if (!testType.EdmEquals(argumentType))
            {
                return false;
            }
            if (testType.BaseType != null)
            {
                return false;
            }
            int num = 0;
            using (IEnumerator<EdmType> enumerator = MetadataHelper.GetTypeAndSubtypesOf(testType, this.m_command.MetadataWorkspace, true).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    EdmType current = enumerator.Current;
                    num++;
                    if (2 == num)
                    {
                        goto Label_0054;
                    }
                }
            }
        Label_0054:
            return (1 == num);
        }

        private Node ConvertToInternalTree(GeneratedView generatedView)
        {
            Node internalTree = generatedView.GetInternalTree(this.m_command.MetadataWorkspace);
            return OpCopier.Copy(this.m_command, internalTree);
        }

        private void DetermineDiscriminatorMapUsage(Node viewNode, EntitySetBase entitySet, EntityTypeBase rootEntityType, bool includeSubtypes)
        {
            ExplicitDiscriminatorMap discriminatorMap = null;
            DiscriminatorMapInfo info;
            if (viewNode.Op.OpType == OpType.Project)
            {
                DiscriminatedNewEntityOp op = viewNode.Child1.Child0.Child0.Op as DiscriminatedNewEntityOp;
                if (op != null)
                {
                    discriminatorMap = op.DiscriminatorMap;
                }
            }
            if (!this.m_discriminatorMaps.TryGetValue(entitySet, out info))
            {
                if (rootEntityType == null)
                {
                    rootEntityType = entitySet.ElementType;
                    includeSubtypes = true;
                }
                info = new DiscriminatorMapInfo(rootEntityType, includeSubtypes, discriminatorMap);
                this.m_discriminatorMaps.Add(entitySet, info);
            }
            else
            {
                info.Merge(rootEntityType, includeSubtypes, discriminatorMap);
            }
        }

        private Node ExpandView(Node node, ScanTableOp scanTableOp, ref IsOfOp typeFilter)
        {
            EntitySetBase extent = scanTableOp.Table.TableMetadata.Extent;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(extent != null, "The target of a ScanTableOp must reference an EntitySet to be used with ExpandView");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(extent.EntityContainer.DataSpace == DataSpace.CSpace, "Store entity sets cannot have Query Mapping Views and should not be used with ExpandView");
            if (((typeFilter != null) && !typeFilter.IsOfOnly) && TypeSemantics.IsSubTypeOf(extent.ElementType, typeFilter.IsOfType.EdmType))
            {
                typeFilter = null;
            }
            GeneratedView generatedView = null;
            EntityTypeBase elementType = scanTableOp.Table.TableMetadata.Extent.ElementType;
            bool includeSubtypes = true;
            if (typeFilter != null)
            {
                elementType = (EntityTypeBase) typeFilter.IsOfType.EdmType;
                includeSubtypes = !typeFilter.IsOfOnly;
                if (this.m_command.MetadataWorkspace.TryGetGeneratedViewOfType(extent, elementType, includeSubtypes, out generatedView))
                {
                    typeFilter = null;
                }
            }
            if (generatedView == null)
            {
                generatedView = this.m_command.MetadataWorkspace.GetGeneratedView(extent);
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(generatedView != null, Strings.ADP_NoQueryMappingView(extent.EntityContainer.Name, extent.Name));
            Node viewNode = this.ConvertToInternalTree(generatedView);
            this.DetermineDiscriminatorMapUsage(viewNode, extent, elementType, includeSubtypes);
            ScanViewOp op = this.m_command.CreateScanViewOp(scanTableOp.Table);
            return this.m_command.CreateNode(op, viewNode);
        }

        private EntitySetBase FindEnclosingEntitySetView()
        {
            if (this.m_viewNestingLevel != 0)
            {
                foreach (Node node in this.m_ancestors)
                {
                    if (node.Op.OpType == OpType.ScanView)
                    {
                        ScanViewOp op = (ScanViewOp) node.Op;
                        return op.Table.TableMetadata.Extent;
                    }
                }
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "found no enclosing view - but view-nesting level is greater than zero");
            }
            return null;
        }

        private int FindPosition(EdmType type, EdmMember member)
        {
            int num = 0;
            foreach (EdmMember member2 in TypeHelpers.GetAllStructuralMembers(type))
            {
                if (member2.EdmEquals(member))
                {
                    return num;
                }
                num++;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, string.Concat(new object[] { "Could not find property ", member, " in type ", type.Name }));
            return -1;
        }

        private RelationshipSet FindRelationshipSet(EntitySetBase entitySet, RelProperty relProperty)
        {
            foreach (EntitySetBase base2 in entitySet.EntityContainer.BaseEntitySets)
            {
                AssociationSet set = base2 as AssociationSet;
                if (((set != null) && set.ElementType.EdmEquals(relProperty.Relationship)) && set.AssociationSetEnds[relProperty.FromEnd.Identity].EntitySet.EdmEquals(entitySet))
                {
                    return set;
                }
            }
            return null;
        }

        private Node FindRelOpAncestor()
        {
            foreach (Node node in this.m_ancestors)
            {
                if (node.Op.IsRelOp)
                {
                    return node;
                }
                if (node.Op.IsPhysicalOp)
                {
                    return null;
                }
            }
            return null;
        }

        private static EntitySetBase FindTargetEntitySet(RelationshipSet relationshipSet, RelationshipEndMember targetEnd)
        {
            EntitySetBase entitySet = null;
            AssociationSet set = (AssociationSet) relationshipSet;
            entitySet = null;
            foreach (AssociationSetEnd end in set.AssociationSetEnds)
            {
                if (end.CorrespondingAssociationEndMember.EdmEquals(targetEnd))
                {
                    entitySet = end.EntitySet;
                    break;
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(entitySet != null, string.Concat(new object[] { "Could not find entityset for relationshipset ", relationshipSet, ";association end ", targetEnd }));
            return entitySet;
        }

        private Node GetDefiningQueryForSSpaceSet(Node node, ScanTableOp scanTableOp)
        {
            EntityType elementType = scanTableOp.Table.TableMetadata.Extent.ElementType as EntityType;
            if (elementType != null)
            {
                Var var2;
                Table table = this.m_command.CreateTableInstance(scanTableOp.Table.TableMetadata);
                ScanTableOp op = this.m_command.CreateScanTableOp(table);
                Node input = this.m_command.CreateNode(op);
                Var v = table.Columns[0];
                List<Node> args = new List<Node>();
                foreach (EdmProperty property in elementType.Properties)
                {
                    Node item = this.m_command.CreateNode(this.m_command.CreatePropertyOp(property), this.m_command.CreateNode(this.m_command.CreateVarRefOp(v)));
                    args.Add(item);
                }
                Node computedExpression = this.m_command.CreateNode(this.m_command.CreateNewInstanceOp(TypeUsage.Create(elementType)), args);
                node = this.m_command.BuildProject(input, computedExpression, out var2);
            }
            return node;
        }

        private List<EntitySet> GetEntitySets(TypeUsage entityType)
        {
            List<EntitySet> list = new List<EntitySet>();
            foreach (EntityContainer container in this.m_referencedEntityContainers)
            {
                foreach (EntitySetBase base2 in container.BaseEntitySets)
                {
                    EntitySet item = base2 as EntitySet;
                    if ((item != null) && ((item.ElementType.Equals(entityType.EdmType) || TypeSemantics.IsSubTypeOf(entityType.EdmType, item.ElementType)) || TypeSemantics.IsSubTypeOf(item.ElementType, entityType.EdmType)))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private List<RelationshipSet> GetRelationshipSets(RelationshipType relType)
        {
            List<RelationshipSet> list = new List<RelationshipSet>();
            foreach (EntityContainer container in this.m_referencedEntityContainers)
            {
                foreach (EntitySetBase base2 in container.BaseEntitySets)
                {
                    RelationshipSet item = base2 as RelationshipSet;
                    if ((item != null) && item.ElementType.Equals(relType))
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        private void HandleTableOpMetadata(ScanTableBaseOp op)
        {
            EntitySet extent = op.Table.TableMetadata.Extent as EntitySet;
            if (extent != null)
            {
                this.AddEntitySetReference(extent);
            }
            TypeUsage type = TypeUsage.Create(op.Table.TableMetadata.Extent.ElementType);
            this.AddTypeReference(type);
        }

        private static bool IsCollectionAggregateFunction(FunctionOp op, Node n) => 
            (((n.Children.Count == 1) && TypeSemantics.IsCollectionType(n.Child0.Op.Type)) && TypeSemantics.IsAggregateFunction(op.Function));

        private static bool IsCollectionFunction(FunctionOp op) => 
            TypeSemantics.IsCollectionType(op.Type);

        private bool IsOfTypeOverScanTable(Node n, out IsOfOp typeFilter)
        {
            typeFilter = null;
            IsOfOp op = n.Child1.Op as IsOfOp;
            if (op == null)
            {
                return false;
            }
            ScanTableOp op2 = n.Child0.Op as ScanTableOp;
            if ((op2 == null) || (op2.Table.Columns.Count != 1))
            {
                return false;
            }
            VarRefOp op3 = n.Child1.Child0.Op as VarRefOp;
            if ((op3 == null) || (op3.Var != op2.Table.Columns[0]))
            {
                return false;
            }
            typeFilter = op;
            return true;
        }

        private bool IsSortUnnecessary()
        {
            Node node = this.m_ancestors.Peek();
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node != null, "unexpected SortOp as root node?");
            if (node.Op.OpType == OpType.PhysicalProject)
            {
                return false;
            }
            return true;
        }

        internal void Process()
        {
            this.m_command.Root = base.VisitNode(this.m_command.Root);
            foreach (Var var in this.m_command.Vars)
            {
                this.AddTypeReference(var.Type);
            }
            if (this.m_referencedTypes.Count > 0)
            {
                this.m_compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.NTE);
                PhysicalProjectOp op = (PhysicalProjectOp) this.m_command.Root.Op;
                op.ColumnMap.Accept<HashSet<string>>(StructuredTypeNullabilityAnalyzer.Instance, this.m_typesNeedingNullSentinel);
            }
        }

        internal static void Process(System.Data.Query.PlanCompiler.PlanCompiler planCompilerState, out StructuredTypeInfo typeInfo)
        {
            PreProcessor processor = new PreProcessor(planCompilerState);
            processor.Process();
            StructuredTypeInfo.Process(planCompilerState.Command, processor.m_referencedTypes, processor.m_referencedEntitySets, processor.m_freeFloatingEntityConstructorTypes, processor.m_suppressDiscriminatorMaps ? null : processor.m_discriminatorMaps, processor.m_relPropertyHelper, processor.m_typesNeedingNullSentinel, out typeInfo);
        }

        private Node ProcessScanTable(Node scanTableNode, ScanTableOp scanTableOp, ref IsOfOp typeFilter)
        {
            this.HandleTableOpMetadata(scanTableOp);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(scanTableOp.Table.TableMetadata.Extent != null, "ScanTableOp must reference a table with an extent");
            Node n = null;
            if (scanTableOp.Table.TableMetadata.Extent.EntityContainer.DataSpace == DataSpace.SSpace)
            {
                if (this.m_viewNestingLevel > 0)
                {
                    return scanTableNode;
                }
                Node definingQueryForSSpaceSet = this.GetDefiningQueryForSSpaceSet(scanTableNode, scanTableOp);
                ScanViewOp op = this.m_command.CreateScanViewOp(scanTableOp.Table);
                n = this.m_command.CreateNode(op, definingQueryForSSpaceSet);
            }
            else
            {
                n = this.ExpandView(scanTableNode, scanTableOp, ref typeFilter);
            }
            this.m_viewNestingLevel++;
            n = base.VisitNode(n);
            this.m_viewNestingLevel--;
            return n;
        }

        private Node RewriteDerefOp(Node derefOpNode, DerefOp derefOp, out Var outputVar)
        {
            Node node2;
            Var var2;
            TypeUsage type = derefOp.Type;
            List<EntitySet> entitySets = this.GetEntitySets(type);
            if (entitySets.Count == 0)
            {
                outputVar = null;
                return this.m_command.CreateNode(this.m_command.CreateNullOp(type));
            }
            List<Node> inputNodes = new List<Node>();
            List<Var> inputVars = new List<Var>();
            foreach (EntitySet set in entitySets)
            {
                Var var;
                Node item = this.BuildOfTypeTable(set, type, out var);
                inputNodes.Add(item);
                inputVars.Add(var);
            }
            this.m_command.BuildUnionAllLadder(inputNodes, inputVars, out node2, out var2);
            Node node3 = this.m_command.CreateNode(this.m_command.CreateGetEntityRefOp(derefOpNode.Child0.Op.Type), this.m_command.CreateNode(this.m_command.CreateVarRefOp(var2)));
            Node node4 = this.m_command.BuildComparison(OpType.EQ, derefOpNode.Child0, node3);
            Node node5 = this.m_command.CreateNode(this.m_command.CreateFilterOp(), node2, node4);
            outputVar = var2;
            return node5;
        }

        private Node RewriteIsOfAsIsNull(IsOfOp op, Node n)
        {
            ConditionalOp op2 = this.m_command.CreateConditionalOp(OpType.IsNull);
            Node node = this.m_command.CreateNode(op2, n.Child0);
            ConditionalOp op3 = this.m_command.CreateConditionalOp(OpType.Not);
            Node node2 = this.m_command.CreateNode(op3, node);
            ConstantBaseOp op4 = this.m_command.CreateConstantOp(op.Type, true);
            Node node3 = this.m_command.CreateNode(op4);
            NullOp op5 = this.m_command.CreateNullOp(op.Type);
            Node node4 = this.m_command.CreateNode(op5);
            CaseOp op6 = this.m_command.CreateCaseOp(op.Type);
            Node node5 = this.m_command.CreateNode(op6, node2, node3, node4);
            ComparisonOp op7 = this.m_command.CreateComparisonOp(OpType.EQ);
            return this.m_command.CreateNode(op7, node5, node3);
        }

        private Node RewriteManyToManyNavigationProperty(RelProperty relProperty, List<RelationshipSet> relationshipSets, Node sourceRefNode, TypeUsage resultType)
        {
            Node node3;
            IList<Var> list3;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(relationshipSets.Count > 0, "expected at least one relationshipset here");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((relProperty.ToEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many) && (relProperty.FromEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many), string.Concat(new object[] { "Expected target end multiplicity to be 'many'. Found ", relProperty, "; multiplicity = ", relProperty.ToEnd.RelationshipMultiplicity }));
            List<Node> inputNodes = new List<Node>(relationshipSets.Count);
            List<Var> inputVars = new List<Var>(relationshipSets.Count * 2);
            foreach (RelationshipSet set in relationshipSets)
            {
                Var var;
                Var var2;
                Node item = this.BuildJoinForNavProperty(set, relProperty.ToEnd, out var, out var2);
                inputNodes.Add(item);
                inputVars.Add(var);
                inputVars.Add(var2);
            }
            this.m_command.BuildUnionAllLadder(inputNodes, inputVars, out node3, out list3);
            Node node4 = this.m_command.CreateNode(this.m_command.CreatePropertyOp(relProperty.FromEnd), this.m_command.CreateNode(this.m_command.CreateVarRefOp(list3[0])));
            Node node5 = this.m_command.BuildComparison(OpType.EQ, sourceRefNode, node4);
            Node inputNode = this.m_command.CreateNode(this.m_command.CreateFilterOp(), node3, node5);
            Node relOpNode = this.m_command.BuildProject(inputNode, new Var[] { list3[1] }, new Node[0]);
            return this.m_command.BuildCollect(relOpNode, list3[1]);
        }

        private Node RewriteNavigateOp(Node navigateOpNode, NavigateOp navigateOp, out Var outputVar)
        {
            Var var2;
            Var var3;
            outputVar = null;
            if (!Helper.IsAssociationType(navigateOp.Relationship))
            {
                throw EntityUtil.NotSupported(Strings.Cqt_RelNav_NoCompositions);
            }
            if ((navigateOpNode.Child0.Op.OpType == OpType.GetEntityRef) && ((navigateOp.ToEnd.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne) || (navigateOp.ToEnd.RelationshipMultiplicity == RelationshipMultiplicity.One)))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_command.IsRelPropertyReferenced(navigateOp.RelProperty), "Unreferenced rel property? " + navigateOp.RelProperty);
                Op op = this.m_command.CreateRelPropertyOp(navigateOp.RelProperty);
                return this.m_command.CreateNode(op, navigateOpNode.Child0.Child0);
            }
            List<RelationshipSet> relationshipSets = this.GetRelationshipSets(navigateOp.Relationship);
            if (relationshipSets.Count == 0)
            {
                if (navigateOp.ToEnd.RelationshipMultiplicity != RelationshipMultiplicity.Many)
                {
                    return this.m_command.CreateNode(this.m_command.CreateNullOp(navigateOp.Type));
                }
                return this.m_command.CreateNode(this.m_command.CreateNewMultisetOp(navigateOp.Type));
            }
            List<Node> inputNodes = new List<Node>();
            List<Var> inputVars = new List<Var>();
            foreach (RelationshipSet set in relationshipSets)
            {
                TableMD tableMetadata = Command.CreateTableDefinition(set);
                ScanTableOp op2 = this.m_command.CreateScanTableOp(tableMetadata);
                Node item = this.m_command.CreateNode(op2);
                Var var = op2.Table.Columns[0];
                inputVars.Add(var);
                inputNodes.Add(item);
            }
            Node resultNode = null;
            this.m_command.BuildUnionAllLadder(inputNodes, inputVars, out resultNode, out var2);
            Node computedExpression = this.m_command.CreateNode(this.m_command.CreatePropertyOp(navigateOp.ToEnd), this.m_command.CreateNode(this.m_command.CreateVarRefOp(var2)));
            Node node5 = this.m_command.CreateNode(this.m_command.CreatePropertyOp(navigateOp.FromEnd), this.m_command.CreateNode(this.m_command.CreateVarRefOp(var2)));
            Node node6 = this.m_command.BuildComparison(OpType.EQ, navigateOpNode.Child0, node5);
            Node input = this.m_command.CreateNode(this.m_command.CreateFilterOp(), resultNode, node6);
            Node relOpNode = this.m_command.BuildProject(input, computedExpression, out var3);
            if (navigateOp.ToEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many)
            {
                return this.m_command.BuildCollect(relOpNode, var3);
            }
            Node node9 = relOpNode;
            outputVar = var3;
            return node9;
        }

        private Node RewriteNavigationProperty(NavigationProperty navProperty, Node sourceEntityNode, TypeUsage resultType)
        {
            RelProperty relProperty = new RelProperty(navProperty.RelationshipType, navProperty.FromEndMember, navProperty.ToEndMember);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_command.IsRelPropertyReferenced(relProperty) || (relProperty.ToEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many), "Unreferenced rel property? " + relProperty);
            if ((relProperty.ToEnd.RelationshipMultiplicity == RelationshipMultiplicity.One) || (relProperty.ToEnd.RelationshipMultiplicity == RelationshipMultiplicity.ZeroOrOne))
            {
                return this.RewriteToOneNavigationProperty(relProperty, sourceEntityNode, resultType);
            }
            List<RelationshipSet> relationshipSets = this.GetRelationshipSets(relProperty.Relationship);
            if (relationshipSets.Count == 0)
            {
                return this.m_command.CreateNode(this.m_command.CreateNewMultisetOp(resultType));
            }
            Node sourceRefNode = this.m_command.CreateNode(this.m_command.CreateGetEntityRefOp(relProperty.FromEnd.TypeUsage), sourceEntityNode);
            if (relProperty.FromEnd.RelationshipMultiplicity == RelationshipMultiplicity.Many)
            {
                return this.RewriteManyToManyNavigationProperty(relProperty, relationshipSets, sourceRefNode, resultType);
            }
            return this.RewriteOneToManyNavigationProperty(relProperty, relationshipSets, sourceRefNode, resultType);
        }

        private Node RewriteOneToManyNavigationProperty(RelProperty relProperty, List<RelationshipSet> relationshipSets, Node sourceRefNode, TypeUsage resultType)
        {
            Node node3;
            Var var2;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(relationshipSets.Count > 0, "expected at least one relationshipset here");
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(relProperty.FromEnd.RelationshipMultiplicity != RelationshipMultiplicity.Many, "Expected source end multiplicity to be one. Found 'Many' instead " + relProperty);
            TypeUsage elementTypeUsage = TypeHelpers.GetElementTypeUsage(relProperty.ToEnd.TypeUsage);
            List<Node> inputNodes = new List<Node>(relationshipSets.Count);
            List<Var> inputVars = new List<Var>(relationshipSets.Count);
            foreach (RelationshipSet set in relationshipSets)
            {
                Var var;
                EntitySetBase entitySet = FindTargetEntitySet(set, relProperty.ToEnd);
                Node item = this.BuildOfTypeTable(entitySet, elementTypeUsage, out var);
                inputNodes.Add(item);
                inputVars.Add(var);
            }
            this.m_command.BuildUnionAllLadder(inputNodes, inputVars, out node3, out var2);
            RelProperty property = new RelProperty(relProperty.Relationship, relProperty.ToEnd, relProperty.FromEnd);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(this.m_command.IsRelPropertyReferenced(property), "Unreferenced rel property? " + property);
            Node node4 = this.m_command.CreateNode(this.m_command.CreateRelPropertyOp(property), this.m_command.CreateNode(this.m_command.CreateVarRefOp(var2)));
            Node node5 = this.m_command.BuildComparison(OpType.EQ, sourceRefNode, node4);
            Node relOpNode = this.m_command.CreateNode(this.m_command.CreateFilterOp(), node3, node5);
            return this.m_command.BuildCollect(relOpNode, var2);
        }

        private Node RewriteToOneNavigationProperty(RelProperty relProperty, Node sourceEntityNode, TypeUsage resultType)
        {
            RelPropertyOp op = this.m_command.CreateRelPropertyOp(relProperty);
            Node node = this.m_command.CreateNode(op, sourceEntityNode);
            DerefOp op2 = this.m_command.CreateDerefOp(resultType);
            return this.m_command.CreateNode(op2, node);
        }

        private void ValidateNavPropertyOp(PropertyOp op, Node n)
        {
            NavigationProperty propertyInfo = (NavigationProperty) op.PropertyInfo;
            TypeUsage typeUsage = propertyInfo.ToEndMember.TypeUsage;
            if (TypeSemantics.IsReferenceType(typeUsage))
            {
                typeUsage = TypeHelpers.GetElementTypeUsage(typeUsage);
            }
            if (propertyInfo.ToEndMember.RelationshipMultiplicity == RelationshipMultiplicity.Many)
            {
                typeUsage = TypeUsage.Create(typeUsage.EdmType.GetCollectionType());
            }
            if (!TypeSemantics.IsEquivalentOrPromotableTo(typeUsage, op.Type))
            {
                throw EntityUtil.Metadata(Strings.EntityClient_IncompatibleNavigationPropertyResult(propertyInfo.DeclaringType.FullName, propertyInfo.Name));
            }
        }

        public override Node Visit(CaseOp op, Node n)
        {
            bool flag;
            this.VisitScalarOpDefault(op, n);
            if (PlanCompilerUtil.IsRowTypeCaseOpWithNullability(op, n, out flag))
            {
                this.m_typesNeedingNullSentinel.Add(op.Type.EdmType.Identity);
            }
            return n;
        }

        public override Node Visit(CollectOp op, Node n)
        {
            this.m_compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.NestPullup);
            return this.VisitScalarOpDefault(op, n);
        }

        public override Node Visit(ConditionalOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
            if ((op.OpType == OpType.IsNull) && TypeSemantics.IsRowType(n.Child0.Op.Type))
            {
                StructuredTypeNullabilityAnalyzer.MarkAsNeedingNullSentinel(this.m_typesNeedingNullSentinel, n.Child0.Op.Type);
            }
            return n;
        }

        public override Node Visit(DerefOp op, Node n)
        {
            Var var;
            this.VisitScalarOpDefault(op, n);
            Node node = this.RewriteDerefOp(n, op, out var);
            node = base.VisitNode(node);
            if (var != null)
            {
                node = this.AddSubqueryToParentRelOp(var, node);
            }
            return node;
        }

        public override Node Visit(DiscriminatedNewEntityOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(0 < this.m_viewNestingLevel, "DiscriminatedNewInstanceOp may appear only within view definition");
            HashSet<RelProperty> collection = new HashSet<RelProperty>();
            List<RelProperty> relPropertyList = new List<RelProperty>();
            foreach (KeyValuePair<object, EntityType> pair in op.DiscriminatorMap.TypeMap)
            {
                EntityTypeBase edmType = pair.Value;
                this.AddTypeReference(TypeUsage.Create(edmType));
                foreach (RelProperty property in this.m_relPropertyHelper.GetRelProperties(edmType))
                {
                    collection.Add(property);
                }
            }
            relPropertyList = new List<RelProperty>(collection);
            this.VisitScalarOpDefault(op, n);
            Node keyExpr = this.BuildKeyExpressionForNewEntityOp(op, n);
            List<Node> args = new List<Node>();
            int num = n.Children.Count - op.RelationshipProperties.Count;
            for (int i = 0; i < num; i++)
            {
                args.Add(n.Children[i]);
            }
            Dictionary<RelProperty, Node> prebuiltExpressions = new Dictionary<RelProperty, Node>();
            int num3 = num;
            for (int j = 0; num3 < n.Children.Count; j++)
            {
                prebuiltExpressions[op.RelationshipProperties[j]] = n.Children[num3];
                num3++;
            }
            foreach (Node node2 in this.BuildAllRelPropertyExpressions(op.EntitySet, relPropertyList, prebuiltExpressions, keyExpr))
            {
                args.Add(node2);
            }
            Op op2 = this.m_command.CreateDiscriminatedNewEntityOp(op.Type, op.DiscriminatorMap, op.EntitySet, relPropertyList);
            return this.m_command.CreateNode(op2, args);
        }

        public override Node Visit(ElementOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
            Node subquery = n.Child0;
            ProjectOp op2 = (ProjectOp) subquery.Op;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op2.Outputs.Count == 1, "input to ElementOp has more than one output var?");
            Var first = op2.Outputs.First;
            return this.AddSubqueryToParentRelOp(first, subquery);
        }

        public override Node Visit(ExistsOp op, Node n)
        {
            this.VisitChildren(n);
            n.Child0 = this.BuildDummyProjectForExists(n.Child0);
            return n;
        }

        public override Node Visit(FilterOp op, Node n)
        {
            IsOfOp op2;
            if (!this.IsOfTypeOverScanTable(n, out op2))
            {
                return this.VisitRelOpDefault(op, n);
            }
            Node node = this.ProcessScanTable(n.Child0, (ScanTableOp) n.Child0.Op, ref op2);
            if (op2 != null)
            {
                n.Child1 = base.VisitNode(n.Child1);
                n.Child0 = node;
                node = n;
            }
            return node;
        }

        public override Node Visit(FunctionOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
            Node node = null;
            if (IsCollectionFunction(op))
            {
                node = this.VisitCollectionFunction(op, n);
            }
            else if (IsCollectionAggregateFunction(op, n))
            {
                node = this.VisitCollectionAggregateFunction(op, n);
            }
            else
            {
                node = n;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(node != null, "failure to construct a functionOp?");
            return node;
        }

        public override Node Visit(IsOfOp op, Node n)
        {
            n = this.VisitScalarOpDefault(op, n);
            this.AddTypeReference(op.IsOfType);
            if (this.CanRewriteTypeTest(op.IsOfType.EdmType, n.Child0.Op.Type.EdmType))
            {
                n = this.RewriteIsOfAsIsNull(op, n);
            }
            if (op.IsOfOnly && op.IsOfType.EdmType.Abstract)
            {
                this.m_suppressDiscriminatorMaps = true;
            }
            return n;
        }

        public override Node Visit(NavigateOp op, Node n)
        {
            Var var;
            this.VisitScalarOpDefault(op, n);
            Node node = this.RewriteNavigateOp(n, op, out var);
            node = base.VisitNode(node);
            if (var != null)
            {
                node = this.AddSubqueryToParentRelOp(var, node);
            }
            return node;
        }

        public override Node Visit(NewEntityOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
            EntityType edmType = op.Type.EdmType as EntityType;
            EntitySetBase entitySet = this.FindEnclosingEntitySetView();
            if (entitySet == null)
            {
                if (edmType != null)
                {
                    this.m_freeFloatingEntityConstructorTypes.Add(edmType);
                }
                System.Data.Query.PlanCompiler.PlanCompiler.Assert((op.RelationshipProperties == null) || (op.RelationshipProperties.Count == 0), "Related Entities cannot be specified for Entity constructors that are not part of the Query Mapping View for an Entity Set.");
                return n;
            }
            List<RelProperty> relPropertyList = new List<RelProperty>(this.m_relPropertyHelper.GetRelProperties(edmType));
            Node keyExpr = this.BuildKeyExpressionForNewEntityOp(op, n);
            Dictionary<RelProperty, Node> prebuiltExpressions = new Dictionary<RelProperty, Node>();
            int num = 0;
            int count = edmType.Properties.Count;
            while (count < n.Children.Count)
            {
                prebuiltExpressions[op.RelationshipProperties[num]] = n.Children[count];
                count++;
                num++;
            }
            List<Node> args = new List<Node>();
            for (int i = 0; i < edmType.Properties.Count; i++)
            {
                args.Add(n.Children[i]);
            }
            foreach (Node node2 in this.BuildAllRelPropertyExpressions(entitySet, relPropertyList, prebuiltExpressions, keyExpr))
            {
                args.Add(node2);
            }
            Op op2 = this.m_command.CreateNewEntityOp(op.Type, relPropertyList, entitySet);
            return this.m_command.CreateNode(op2, args);
        }

        public override Node Visit(NewMultisetOp op, Node n)
        {
            Node resultNode = null;
            Var resultVar = null;
            CollectionType edmType = TypeHelpers.GetEdmType<CollectionType>(op.Type);
            if (!n.HasChild0)
            {
                Var var2;
                Node node2 = this.m_command.CreateNode(this.m_command.CreateSingleRowTableOp());
                Node input = this.m_command.CreateNode(this.m_command.CreateFilterOp(), node2, this.m_command.CreateNode(this.m_command.CreateFalseOp()));
                Node computedExpression = this.m_command.CreateNode(this.m_command.CreateNullOp(edmType.TypeUsage));
                resultNode = this.m_command.BuildProject(input, computedExpression, out var2);
                resultVar = var2;
            }
            else if ((n.Children.Count == 1) || this.AreAllConstantsOrNulls(n.Children))
            {
                List<Node> inputNodes = new List<Node>();
                List<Var> inputVars = new List<Var>();
                foreach (Node node6 in n.Children)
                {
                    Var var3;
                    Node node7 = this.m_command.CreateNode(this.m_command.CreateSingleRowTableOp());
                    Node item = this.m_command.BuildProject(node7, node6, out var3);
                    inputNodes.Add(item);
                    inputVars.Add(var3);
                }
                this.m_command.BuildUnionAllLadder(inputNodes, inputVars, out resultNode, out resultVar);
            }
            else
            {
                List<Node> list3 = new List<Node>();
                List<Var> list4 = new List<Var>();
                for (int i = 0; i < n.Children.Count; i++)
                {
                    Var var4;
                    Node node9 = this.m_command.CreateNode(this.m_command.CreateSingleRowTableOp());
                    Node node10 = this.m_command.CreateNode(this.m_command.CreateInternalConstantOp(this.m_command.IntegerType, i));
                    Node node11 = this.m_command.BuildProject(node9, node10, out var4);
                    list3.Add(node11);
                    list4.Add(var4);
                }
                this.m_command.BuildUnionAllLadder(list3, list4, out resultNode, out resultVar);
                List<Node> args = new List<Node>((n.Children.Count * 2) + 1);
                for (int j = 0; j < n.Children.Count; j++)
                {
                    if (j != (n.Children.Count - 1))
                    {
                        ComparisonOp op2 = this.m_command.CreateComparisonOp(OpType.EQ);
                        Node node12 = this.m_command.CreateNode(op2, this.m_command.CreateNode(this.m_command.CreateVarRefOp(resultVar)), this.m_command.CreateNode(this.m_command.CreateConstantOp(this.m_command.IntegerType, j)));
                        args.Add(node12);
                    }
                    args.Add(n.Children[j]);
                }
                Node node13 = this.m_command.CreateNode(this.m_command.CreateCaseOp(edmType.TypeUsage), args);
                resultNode = this.m_command.BuildProject(resultNode, node13, out resultVar);
            }
            PhysicalProjectOp op3 = this.m_command.CreatePhysicalProjectOp(resultVar);
            Node node14 = this.m_command.CreateNode(op3, resultNode);
            CollectOp op4 = this.m_command.CreateCollectOp(op.Type);
            Node node15 = this.m_command.CreateNode(op4, node14);
            return base.VisitNode(node15);
        }

        public override Node Visit(ProjectOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.HasChild0, "projectOp without input?");
            if ((OpType.Sort != n.Child0.Op.OpType) && (OpType.ConstrainedSort != n.Child0.Op.OpType))
            {
                return this.VisitRelOpDefault(op, n);
            }
            SortBaseOp op2 = (SortBaseOp) n.Child0.Op;
            IList<Node> args = new List<Node> {
                n
            };
            for (int i = 1; i < n.Child0.Children.Count; i++)
            {
                args.Add(n.Child0.Children[i]);
            }
            n.Child0 = n.Child0.Child0;
            foreach (SortKey key in op2.Keys)
            {
                op.Outputs.Set(key.Var);
            }
            return base.VisitNode(this.m_command.CreateNode(op2, args));
        }

        public override Node Visit(PropertyOp op, Node n)
        {
            if (Helper.IsNavigationProperty(op.PropertyInfo))
            {
                return this.VisitNavPropertyOp(op, n);
            }
            return this.VisitScalarOpDefault(op, n);
        }

        public override Node Visit(RefOp op, Node n)
        {
            this.VisitScalarOpDefault(op, n);
            this.AddEntitySetReference(op.EntitySet);
            return n;
        }

        public override Node Visit(ScanTableOp op, Node n)
        {
            IsOfOp typeFilter = null;
            return this.ProcessScanTable(n, op, ref typeFilter);
        }

        public override Node Visit(ScanViewOp op, Node n)
        {
            this.HandleTableOpMetadata(op);
            this.VisitRelOpDefault(op, n);
            return n;
        }

        public override Node Visit(SortOp op, Node n)
        {
            if (this.IsSortUnnecessary())
            {
                return base.VisitNode(n.Child0);
            }
            return this.VisitRelOpDefault(op, n);
        }

        public override Node Visit(TreatOp op, Node n)
        {
            n = base.Visit(op, n);
            if (this.CanRewriteTypeTest(op.Type.EdmType, n.Child0.Op.Type.EdmType))
            {
                return n.Child0;
            }
            return n;
        }

        public override Node Visit(UnnestOp op, Node n)
        {
            List<Node> list;
            this.VisitChildren(n);
            if (this.m_nodeSubqueries.TryGetValue(n, out list))
            {
                return this.AugmentWithSubqueries(n, list, false);
            }
            return n;
        }

        protected override Node VisitApplyOp(ApplyBaseOp op, Node n)
        {
            this.m_compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.JoinElimination);
            return this.VisitRelOpDefault(op, n);
        }

        protected override void VisitChildren(Node n)
        {
            this.m_ancestors.Push(n);
            for (int i = 0; i < n.Children.Count; i++)
            {
                n.Children[i] = base.VisitNode(n.Children[i]);
            }
            this.m_ancestors.Pop();
        }

        private Node VisitCollectionAggregateFunction(FunctionOp op, Node n)
        {
            TypeUsage type = null;
            Var var2;
            Node collectionNode = n.Child0;
            if (OpType.SoftCast == collectionNode.Op.OpType)
            {
                type = TypeHelpers.GetEdmType<CollectionType>(collectionNode.Op.Type).TypeUsage;
                collectionNode = collectionNode.Child0;
                while (OpType.SoftCast == collectionNode.Op.OpType)
                {
                    collectionNode = collectionNode.Child0;
                }
            }
            Node node2 = this.BuildUnnest(collectionNode);
            UnnestOp op2 = node2.Op as UnnestOp;
            Var v = op2.Table.Columns[0];
            AggregateOp op3 = this.m_command.CreateAggregateOp(op.Function, false);
            VarRefOp op4 = this.m_command.CreateVarRefOp(v);
            Node node3 = this.m_command.CreateNode(op4);
            if (type != null)
            {
                node3 = this.m_command.CreateNode(this.m_command.CreateSoftCastOp(type), node3);
            }
            Node definingExpr = this.m_command.CreateNode(op3, node3);
            VarVec gbyKeys = this.m_command.CreateVarVec();
            Node node5 = this.m_command.CreateNode(this.m_command.CreateVarDefListOp());
            VarVec outputs = this.m_command.CreateVarVec();
            Node node6 = this.m_command.CreateVarDefListNode(definingExpr, out var2);
            outputs.Set(var2);
            GroupByOp op5 = this.m_command.CreateGroupByOp(gbyKeys, outputs);
            Node subquery = this.m_command.CreateNode(op5, node2, node5, node6);
            return this.AddSubqueryToParentRelOp(var2, subquery);
        }

        private Node VisitCollectionFunction(FunctionOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(IsCollectionFunction(op), "non-TVF function?");
            Node node = this.BuildUnnest(n);
            UnnestOp op2 = node.Op as UnnestOp;
            PhysicalProjectOp op3 = this.m_command.CreatePhysicalProjectOp(op2.Table.Columns[0]);
            Node node2 = this.m_command.CreateNode(op3, node);
            CollectOp op4 = this.m_command.CreateCollectOp(n.Op.Type);
            Node node3 = this.m_command.CreateNode(op4, node2);
            this.m_compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.NestPullup);
            return node3;
        }

        protected override Node VisitJoinOp(JoinBaseOp op, Node n)
        {
            List<Node> list;
            this.VisitChildren(n);
            if ((op.OpType == OpType.InnerJoin) || (op.OpType == OpType.LeftOuterJoin))
            {
                this.m_compilerState.MarkPhaseAsNeeded(PlanCompilerPhase.JoinElimination);
            }
            if (this.m_nodeSubqueries.TryGetValue(n, out list))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(((n.Op.OpType == OpType.InnerJoin) || (n.Op.OpType == OpType.LeftOuterJoin)) || (n.Op.OpType == OpType.FullOuterJoin), "unexpected op?");
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(n.HasChild2, "missing second child to JoinOp?");
                Node node = n.Child2;
                Node input = this.m_command.CreateNode(this.m_command.CreateSingleRowTableOp());
                input = this.AugmentWithSubqueries(input, list, true);
                Node child = this.m_command.CreateNode(this.m_command.CreateFilterOp(), input, node);
                Node node4 = this.BuildDummyProjectForExists(child);
                Node node5 = this.m_command.CreateNode(this.m_command.CreateExistsOp(), node4);
                n.Child2 = node5;
            }
            return n;
        }

        private Node VisitNavPropertyOp(PropertyOp op, Node n)
        {
            this.ValidateNavPropertyOp(op, n);
            if (n.Child0.Op.OpType != OpType.Property)
            {
                this.VisitScalarOpDefault(op, n);
            }
            NavigationProperty propertyInfo = (NavigationProperty) op.PropertyInfo;
            Node node = this.RewriteNavigationProperty(propertyInfo, n.Child0, op.Type);
            return base.VisitNode(node);
        }

        protected override Node VisitRelOpDefault(RelOp op, Node n)
        {
            List<Node> list;
            this.VisitChildren(n);
            if (this.m_nodeSubqueries.TryGetValue(n, out list) && (list.Count > 0))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(((n.Op.OpType == OpType.Project) || (n.Op.OpType == OpType.Filter)) || (n.Op.OpType == OpType.GroupBy), "VisitRelOpDefault: Unexpected op?" + n.Op.OpType);
                Node node = this.AugmentWithSubqueries(n.Child0, list, true);
                n.Child0 = node;
            }
            return n;
        }

        protected override Node VisitScalarOpDefault(ScalarOp op, Node n)
        {
            this.VisitChildren(n);
            this.AddTypeReference(op.Type);
            return n;
        }

        private Command m_command =>
            this.m_compilerState.Command;

    }
}

