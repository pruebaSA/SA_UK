namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Runtime.InteropServices;

    internal class PropertyPushdownHelper : BasicOpVisitor
    {
        private readonly Dictionary<Node, PropertyRefList> m_nodePropertyRefMap;
        private readonly StructuredTypeInfo m_structuredTypeInfo;
        private readonly Dictionary<Var, PropertyRefList> m_varPropertyRefMap;

        private PropertyPushdownHelper(StructuredTypeInfo structuredTypeInfo)
        {
            this.m_structuredTypeInfo = structuredTypeInfo;
            this.m_varPropertyRefMap = new Dictionary<Var, PropertyRefList>();
            this.m_nodePropertyRefMap = new Dictionary<Node, PropertyRefList>();
        }

        private void AddPropertyRefs(Node node, PropertyRefList propertyRefs)
        {
            this.GetPropertyRefList(node).Append(propertyRefs);
        }

        private void AddPropertyRefs(Var v, PropertyRefList propertyRefs)
        {
            this.GetPropertyRefList(v).Append(propertyRefs);
        }

        private static PropertyRefList GetIdentityProperties(EntityType type)
        {
            PropertyRefList keyProperties = GetKeyProperties(type);
            keyProperties.Add(EntitySetIdPropertyRef.Instance);
            return keyProperties;
        }

        private static PropertyRefList GetKeyProperties(EntityType entityType)
        {
            PropertyRefList list = new PropertyRefList();
            foreach (EdmMember member in entityType.KeyMembers)
            {
                EdmProperty property = member as EdmProperty;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(property != null, "EntityType had non-EdmProperty key member?");
                SimplePropertyRef ref2 = new SimplePropertyRef(property);
                list.Add(ref2);
            }
            return list;
        }

        private PropertyRefList GetPropertyRefList(Node node)
        {
            PropertyRefList list;
            if (!this.m_nodePropertyRefMap.TryGetValue(node, out list))
            {
                list = new PropertyRefList();
                this.m_nodePropertyRefMap[node] = list;
            }
            return list;
        }

        private PropertyRefList GetPropertyRefList(Var v)
        {
            PropertyRefList list;
            if (!this.m_varPropertyRefMap.TryGetValue(v, out list))
            {
                list = new PropertyRefList();
                this.m_varPropertyRefMap[v] = list;
            }
            return list;
        }

        private void Process(Node rootNode)
        {
            rootNode.Op.Accept(this, rootNode);
        }

        internal static void Process(Command itree, StructuredTypeInfo structuredTypeInfo, out Dictionary<Var, PropertyRefList> varPropertyRefs, out Dictionary<Node, PropertyRefList> nodePropertyRefs)
        {
            PropertyPushdownHelper helper = new PropertyPushdownHelper(structuredTypeInfo);
            helper.Process(itree.Root);
            varPropertyRefs = helper.m_varPropertyRefMap;
            nodePropertyRefs = helper.m_nodePropertyRefMap;
        }

        public override void Visit(CaseOp op, Node n)
        {
            PropertyRefList propertyRefList = this.GetPropertyRefList(n);
            for (int i = 1; i < (n.Children.Count - 1); i += 2)
            {
                PropertyRefList propertyRefs = propertyRefList.Clone();
                this.AddPropertyRefs(n.Children[i], propertyRefs);
            }
            this.AddPropertyRefs(n.Children[n.Children.Count - 1], propertyRefList.Clone());
            this.VisitChildren(n);
        }

        public override void Visit(CollectOp op, Node n)
        {
            this.VisitChildren(n);
        }

        public override void Visit(ComparisonOp op, Node n)
        {
            TypeUsage type = (n.Child0.Op as ScalarOp).Type;
            if (!TypeUtils.IsStructuredType(type))
            {
                this.VisitChildren(n);
            }
            else if (TypeSemantics.IsRowType(type) || TypeSemantics.IsReferenceType(type))
            {
                this.VisitDefault(n);
            }
            else
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeSemantics.IsEntityType(type), "unexpected childOpType?");
                PropertyRefList identityProperties = GetIdentityProperties(TypeHelpers.GetEdmType<EntityType>(type));
                foreach (Node node in n.Children)
                {
                    this.AddPropertyRefs(node, identityProperties);
                }
                this.VisitChildren(n);
            }
        }

        public override void Visit(DistinctOp op, Node n)
        {
            foreach (Var var in op.Keys)
            {
                if (TypeUtils.IsStructuredType(var.Type))
                {
                    this.AddPropertyRefs(var, PropertyRefList.All);
                }
            }
            this.VisitChildren(n);
        }

        public override void Visit(ElementOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override void Visit(FilterOp op, Node n)
        {
            this.VisitNode(n.Child1);
            this.VisitNode(n.Child0);
        }

        public override void Visit(GetEntityRefOp op, Node n)
        {
            ScalarOp op2 = n.Child0.Op as ScalarOp;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op2 != null, "input to GetEntityRefOp is not a ScalarOp?");
            PropertyRefList identityProperties = GetIdentityProperties(TypeHelpers.GetEdmType<EntityType>(op2.Type));
            this.AddPropertyRefs(n.Child0, identityProperties);
            this.VisitNode(n.Child0);
        }

        public override void Visit(GroupByOp op, Node n)
        {
            foreach (Var var in op.Keys)
            {
                if (TypeUtils.IsStructuredType(var.Type))
                {
                    this.AddPropertyRefs(var, PropertyRefList.All);
                }
            }
            this.VisitNode(n.Child2);
            this.VisitNode(n.Child1);
            this.VisitNode(n.Child0);
        }

        public override void Visit(IsOfOp op, Node n)
        {
            PropertyRefList propertyRefs = new PropertyRefList();
            propertyRefs.Add(TypeIdPropertyRef.Instance);
            this.AddPropertyRefs(n.Child0, propertyRefs);
            this.VisitChildren(n);
        }

        public override void Visit(MultiStreamNestOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override void Visit(PhysicalProjectOp op, Node n)
        {
            foreach (Var var in op.Outputs)
            {
                if (TypeUtils.IsStructuredType(var.Type))
                {
                    this.AddPropertyRefs(var, PropertyRefList.All);
                }
            }
            this.VisitChildren(n);
        }

        public override void Visit(ProjectOp op, Node n)
        {
            this.VisitNode(n.Child1);
            this.VisitNode(n.Child0);
        }

        public override void Visit(PropertyOp op, Node n)
        {
            this.VisitPropertyOp(op, n, new SimplePropertyRef(op.PropertyInfo));
        }

        public override void Visit(RelPropertyOp op, Node n)
        {
            this.VisitPropertyOp(op, n, new RelPropertyRef(op.PropertyInfo));
        }

        public override void Visit(ScanTableOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(!n.HasChild0, "scanTableOp with an input?");
        }

        public override void Visit(ScanViewOp op, Node n)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(op.Table.Columns.Count == 1, "ScanViewOp with multiple columns?");
            Var v = op.Table.Columns[0];
            PropertyRefList propertyRefList = this.GetPropertyRefList(v);
            Var singletonVar = NominalTypeEliminator.GetSingletonVar(n.Child0);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(singletonVar != null, "cannot determine single Var from ScanViewOp's input");
            this.AddPropertyRefs(singletonVar, propertyRefList.Clone());
            this.VisitChildren(n);
        }

        public override void Visit(SingleStreamNestOp op, Node n)
        {
            throw EntityUtil.NotSupported();
        }

        public override void Visit(SoftCastOp op, Node n)
        {
            PropertyRefList propertyRefs = null;
            if (TypeSemantics.IsReferenceType(op.Type))
            {
                propertyRefs = PropertyRefList.All;
            }
            else if (TypeSemantics.IsNominalType(op.Type))
            {
                propertyRefs = this.m_nodePropertyRefMap[n].Clone();
            }
            else if (TypeSemantics.IsRowType(op.Type))
            {
                propertyRefs = PropertyRefList.All;
            }
            if (propertyRefs != null)
            {
                this.AddPropertyRefs(n.Child0, propertyRefs);
            }
            this.VisitChildren(n);
        }

        public override void Visit(TreatOp op, Node n)
        {
            PropertyRefList propertyRefs = this.GetPropertyRefList(n).Clone();
            propertyRefs.Add(TypeIdPropertyRef.Instance);
            this.AddPropertyRefs(n.Child0, propertyRefs);
            this.VisitChildren(n);
        }

        public override void Visit(UnnestOp op, Node n)
        {
            this.VisitChildren(n);
        }

        public override void Visit(VarDefListOp op, Node n)
        {
            this.VisitChildren(n);
        }

        public override void Visit(VarDefOp op, Node n)
        {
            if (TypeUtils.IsStructuredType(op.Var.Type))
            {
                PropertyRefList propertyRefList = this.GetPropertyRefList(op.Var);
                this.AddPropertyRefs(n.Child0, propertyRefList);
            }
            this.VisitChildren(n);
        }

        public override void Visit(VarRefOp op, Node n)
        {
            if (TypeUtils.IsStructuredType(op.Var.Type))
            {
                PropertyRefList propertyRefList = this.GetPropertyRefList(n);
                this.AddPropertyRefs(op.Var, propertyRefList);
            }
        }

        protected override void VisitApplyOp(ApplyBaseOp op, Node n)
        {
            this.VisitNode(n.Child1);
            this.VisitNode(n.Child0);
        }

        protected override void VisitDefault(Node n)
        {
            foreach (Node node in n.Children)
            {
                ScalarOp op = node.Op as ScalarOp;
                if ((op != null) && TypeUtils.IsStructuredType(op.Type))
                {
                    this.AddPropertyRefs(node, PropertyRefList.All);
                }
            }
            this.VisitChildren(n);
        }

        protected override void VisitJoinOp(JoinBaseOp op, Node n)
        {
            if (n.Op.OpType == OpType.CrossJoin)
            {
                this.VisitChildren(n);
            }
            else
            {
                this.VisitNode(n.Child2);
                this.VisitNode(n.Child0);
                this.VisitNode(n.Child1);
            }
        }

        private void VisitPropertyOp(Op op, Node n, PropertyRef propertyRef)
        {
            PropertyRefList propertyRefs = new PropertyRefList();
            if (!TypeUtils.IsStructuredType(op.Type))
            {
                propertyRefs.Add(propertyRef);
            }
            else
            {
                PropertyRefList propertyRefList = this.GetPropertyRefList(n);
                if (propertyRefList.AllProperties)
                {
                    propertyRefs = propertyRefList;
                }
                else
                {
                    foreach (PropertyRef ref2 in propertyRefList.Properties)
                    {
                        propertyRefs.Add(ref2.CreateNestedPropertyRef(propertyRef));
                    }
                }
            }
            this.AddPropertyRefs(n.Child0, propertyRefs);
            this.VisitChildren(n);
        }

        protected override void VisitSetOp(SetOp op, Node n)
        {
            foreach (VarMap map in op.VarMap)
            {
                foreach (KeyValuePair<Var, Var> pair in map)
                {
                    if (TypeUtils.IsStructuredType(pair.Key.Type))
                    {
                        PropertyRefList propertyRefList = this.GetPropertyRefList(pair.Key);
                        if ((op.OpType == OpType.Intersect) || (op.OpType == OpType.Except))
                        {
                            propertyRefList = PropertyRefList.All;
                            this.AddPropertyRefs(pair.Key, propertyRefList);
                        }
                        else
                        {
                            propertyRefList = propertyRefList.Clone();
                        }
                        this.AddPropertyRefs(pair.Value, propertyRefList);
                    }
                }
            }
            this.VisitChildren(n);
        }

        protected override void VisitSortOp(SortBaseOp op, Node n)
        {
            foreach (SortKey key in op.Keys)
            {
                if (TypeUtils.IsStructuredType(key.Var.Type))
                {
                    this.AddPropertyRefs(key.Var, PropertyRefList.All);
                }
            }
            if (n.HasChild1)
            {
                this.VisitNode(n.Child1);
            }
            this.VisitNode(n.Child0);
        }
    }
}

