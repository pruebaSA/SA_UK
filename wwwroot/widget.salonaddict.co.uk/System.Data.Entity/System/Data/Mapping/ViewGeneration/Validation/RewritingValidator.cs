namespace System.Data.Mapping.ViewGeneration.Validation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Entity;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Mapping.ViewGeneration.QueryRewriting;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Mapping.ViewGeneration.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    internal class RewritingValidator
    {
        private CellTreeNode _basicView;
        private MemberDomainMap _domainMap;
        private ErrorLog _errorLog;
        private IEnumerable<MemberPath> _keyAttributes;
        private CellNormalizer _normalizer;

        internal RewritingValidator(CellNormalizer normalizer, CellTreeNode basicView)
        {
            this._normalizer = normalizer;
            this._basicView = basicView;
            this._domainMap = this._normalizer.MemberMaps.UpdateDomainMap;
            this._keyAttributes = MemberPath.GetKeyMembers(this._normalizer.Extent, this._domainMap, this._normalizer.Workspace);
            this._errorLog = new ErrorLog();
        }

        private static FragmentQuery AddNullConditionOnCSideFragment(LeftCellWrapper wrapper, MemberPath member, MemberMaps memberMaps)
        {
            JoinTreeSlot slot = GetCSideMappedSlotForSMember(wrapper, member, memberMaps);
            if ((slot == null) || !slot.MemberPath.IsNullable)
            {
                return null;
            }
            BoolExpression whereClause = wrapper.RightCellQuery.WhereClause;
            IEnumerable<CellConstant> domain = memberMaps.QueryDomainMap.GetDomain(slot.MemberPath);
            System.Data.Common.Utils.Set<CellConstant> values = new System.Data.Common.Utils.Set<CellConstant>(CellConstant.EqualityComparer) {
                CellConstant.Null
            };
            OneOfConst literal = new OneOfScalarConst(slot.JoinTreeNode, values, domain);
            return FragmentQuery.Create(BoolExpression.CreateAnd(new BoolExpression[] { whereClause, BoolExpression.CreateLiteral(literal, memberMaps.QueryDomainMap) }));
        }

        private void CheckConstraintsOnNonNullableMembers(Dictionary<MemberValueBinding, CellTreeNode> memberValueTrees, LeftCellWrapper wrapper, CellTreeNode sQueryTree, BoolExpression inExtentCondition)
        {
            foreach (MemberPath path in this._domainMap.NonConditionMembers(this._normalizer.Extent))
            {
                bool flag = path.EdmType is SimpleType;
                if (!path.IsNullable && flag)
                {
                    FragmentQuery query = AddNullConditionOnCSideFragment(wrapper, path, this._normalizer.MemberMaps);
                    if ((query != null) && this._normalizer.RightFragmentQP.IsSatisfiable(query))
                    {
                        this._errorLog.AddEntry(new ErrorLog.Record(true, ViewGenErrorCode.NullableMappingForNonNullableColumn, System.Data.Entity.Strings.Viewgen_NullableMappingForNonNullableColumn(wrapper.LeftExtent.ToString(), path.ToFullString()), wrapper.Cells, ""));
                    }
                }
            }
        }

        private void CheckConstraintsOnProjectedConditionMembers(Dictionary<MemberValueBinding, CellTreeNode> memberValueTrees, LeftCellWrapper wrapper, CellTreeNode sQueryTree, BoolExpression inExtentCondition)
        {
            foreach (MemberPath path in this._domainMap.ConditionMembers(this._normalizer.Extent))
            {
                int index = this._normalizer.MemberMaps.ProjectedSlotMap.IndexOf(path);
                JoinTreeSlot slot = wrapper.RightCellQuery.ProjectedSlotAt(index) as JoinTreeSlot;
                if (slot != null)
                {
                    foreach (CellConstant constant in this._domainMap.GetDomain(path))
                    {
                        CellTreeNode node;
                        if (memberValueTrees.TryGetValue(new MemberValueBinding(path, constant), out node))
                        {
                            BoolExpression expression2;
                            FragmentQuery cQuery = FragmentQuery.Create(PropagateCellConstantsToWhereClause(wrapper, wrapper.RightCellQuery.WhereClause, constant, path, this._normalizer.MemberMaps));
                            CellTreeNode node2 = (sQueryTree == this._basicView) ? node : new OpCellTreeNode(this._normalizer, CellTreeOpType.IJ, new CellTreeNode[] { node, sQueryTree });
                            if (!this.CheckEquivalence(cQuery, node2.RightFragmentQuery, inExtentCondition, out expression2))
                            {
                                string message = System.Data.Entity.Strings.ViewGen_CQ_DomainConstraint_1(slot.ToUserString());
                                this.ReportConstraintViolation(message, expression2, ViewGenErrorCode.DomainConstraintViolation, node2.GetLeaves().Concat<LeftCellWrapper>(new LeftCellWrapper[] { wrapper }));
                            }
                        }
                    }
                }
            }
        }

        private bool CheckEquivalence(FragmentQuery cQuery, FragmentQuery sQuery, BoolExpression inExtentCondition, out BoolExpression unsatisfiedConstraint)
        {
            FragmentQuery query = this._normalizer.RightFragmentQP.Difference(cQuery, sQuery);
            FragmentQuery query2 = this._normalizer.RightFragmentQP.Difference(sQuery, cQuery);
            FragmentQuery query3 = FragmentQuery.Create(BoolExpression.CreateAnd(new BoolExpression[] { query.Condition, inExtentCondition }));
            FragmentQuery query4 = FragmentQuery.Create(BoolExpression.CreateAnd(new BoolExpression[] { query2.Condition, inExtentCondition }));
            unsatisfiedConstraint = null;
            bool flag = true;
            bool flag2 = true;
            if (this._normalizer.RightFragmentQP.IsSatisfiable(query3))
            {
                unsatisfiedConstraint = query3.Condition;
                flag = false;
            }
            if (this._normalizer.RightFragmentQP.IsSatisfiable(query4))
            {
                unsatisfiedConstraint = query4.Condition;
                flag2 = false;
            }
            if (flag && flag2)
            {
                return true;
            }
            unsatisfiedConstraint.ExpensiveSimplify();
            return false;
        }

        private static void ClauseToStringBuilder(DnfClause<DomainConstraint<BoolLiteral, CellConstant>> clause, LeftCellWrapper wrapper, MemberMaps memberMaps, StringBuilder builder, MetadataWorkspace workspace)
        {
            List<OneOfScalarConst> oneOfConsts = new List<OneOfScalarConst>();
            List<OneOfScalarConst> list2 = new List<OneOfScalarConst>();
            List<OneOfTypeConst> list3 = new List<OneOfTypeConst>();
            List<OneOfTypeConst> list4 = new List<OneOfTypeConst>();
            foreach (Literal<DomainConstraint<BoolLiteral, CellConstant>> literal in clause.Literals)
            {
                BoolLiteral boolLiteral = BoolExpression.GetBoolLiteral(literal.Term);
                if (!(boolLiteral is RoleBoolean))
                {
                    OneOfConst @const = (OneOfConst) boolLiteral;
                    OneOfTypeConst item = @const as OneOfTypeConst;
                    OneOfScalarConst const3 = @const as OneOfScalarConst;
                    if (const3 != null)
                    {
                        if (literal.IsTermPositive)
                        {
                            oneOfConsts.Add(const3);
                        }
                        else
                        {
                            list2.Add(const3);
                        }
                    }
                    else if (literal.IsTermPositive)
                    {
                        list3.Add(item);
                    }
                    else
                    {
                        list4.Add(item);
                    }
                }
            }
            if ((list3.Count > 0) || (list4.Count > 0))
            {
                builder.Append(System.Data.Entity.Strings.ViewGen_DomainConstraint_EntityTypes);
            }
            OneOfConstToString<OneOfTypeConst>(list3, builder, System.Data.Entity.Strings.ViewGen_AND, false, workspace);
            OneOfConstToString<OneOfTypeConst>(list4, builder, System.Data.Entity.Strings.ViewGen_AND, true, workspace);
            if ((list3.Count > 0) || (list4.Count > 0))
            {
                builder.AppendLine(string.Empty);
            }
            OneOfConstToString<OneOfScalarConst>(oneOfConsts, builder, System.Data.Entity.Strings.ViewGen_OR, true, workspace);
            OneOfConstToString<OneOfScalarConst>(list2, builder, System.Data.Entity.Strings.ViewGen_OR, false, workspace);
            builder.AppendLine(string.Empty);
        }

        private Dictionary<MemberValueBinding, CellTreeNode> CreateMemberValueTrees(bool complementElse)
        {
            Dictionary<MemberValueBinding, CellTreeNode> dictionary = new Dictionary<MemberValueBinding, CellTreeNode>();
            foreach (MemberPath path in this._domainMap.ConditionMembers(this._normalizer.Extent))
            {
                List<CellConstant> list = new List<CellConstant>(this._domainMap.GetDomain(path));
                OpCellTreeNode node = new OpCellTreeNode(this._normalizer, CellTreeOpType.Union);
                for (int i = 0; i < list.Count; i++)
                {
                    Tile<FragmentQuery> tile;
                    CellConstant constant = list[i];
                    MemberValueBinding binding = new MemberValueBinding(path, constant);
                    FragmentQuery query = QueryRewriter.CreateMemberConditionQuery(path, constant, this._keyAttributes, this._domainMap, this._normalizer.Workspace);
                    if (this._normalizer.TryGetCachedRewriting(query, out tile))
                    {
                        CellTreeNode child = QueryRewriter.TileToCellTree(tile, this._normalizer);
                        dictionary[binding] = child;
                        if (i < (list.Count - 1))
                        {
                            node.Add(child);
                        }
                    }
                }
                if (complementElse && (list.Count > 1))
                {
                    CellConstant constant2 = list[list.Count - 1];
                    MemberValueBinding binding2 = new MemberValueBinding(path, constant2);
                    dictionary[binding2] = new OpCellTreeNode(this._normalizer, CellTreeOpType.LASJ, new CellTreeNode[] { this._basicView, node });
                }
            }
            return dictionary;
        }

        internal static void EntityConfigurationToUserString(BoolExpression condition, StringBuilder builder)
        {
            condition.AsUserString(builder, "PK");
        }

        private static JoinTreeSlot GetCSideMappedSlotForSMember(LeftCellWrapper wrapper, MemberPath member, MemberMaps memberMaps)
        {
            int index = memberMaps.ProjectedSlotMap.IndexOf(member);
            ProjectedSlot slot = wrapper.RightCellQuery.ProjectedSlotAt(index);
            if ((slot == null) || (slot is ConstantSlot))
            {
                return null;
            }
            return (JoinTreeSlot) slot;
        }

        private static void OneOfConstToString<T>(IEnumerable<T> oneOfConsts, StringBuilder builder, string connect, bool toInvert, MetadataWorkspace workspace) where T: OneOfConst
        {
            bool flag = true;
            foreach (T local in oneOfConsts)
            {
                if (!flag)
                {
                    builder.Append(" ");
                    builder.Append(connect);
                }
                local.ToUserString(toInvert, builder, workspace);
                flag = false;
            }
        }

        internal static BoolExpression PropagateCellConstantsToWhereClause(LeftCellWrapper wrapper, BoolExpression expression, CellConstant constant, MemberPath member, MemberMaps memberMaps)
        {
            JoinTreeSlot slot = GetCSideMappedSlotForSMember(wrapper, member, memberMaps);
            if (slot == null)
            {
                return expression;
            }
            IEnumerable<CellConstant> domain = memberMaps.QueryDomainMap.GetDomain(slot.MemberPath);
            System.Data.Common.Utils.Set<CellConstant> values = new System.Data.Common.Utils.Set<CellConstant>(CellConstant.EqualityComparer);
            if (constant is NegatedCellConstant)
            {
                values.Unite(domain);
                values.Difference(((NegatedCellConstant) constant).Elements);
            }
            else
            {
                values.Add(constant);
            }
            OneOfConst literal = new OneOfScalarConst(slot.JoinTreeNode, values, domain);
            return BoolExpression.CreateAnd(new BoolExpression[] { expression, BoolExpression.CreateLiteral(literal, memberMaps.QueryDomainMap) });
        }

        private void ReportConstraintViolation(string message, BoolExpression extraConstraint, ViewGenErrorCode errorCode, IEnumerable<LeftCellWrapper> relevantWrappers)
        {
            if (!ErrorPatternMatcher.FindMappingErrors(this._normalizer, this._domainMap, this._errorLog))
            {
                extraConstraint.ExpensiveSimplify();
                HashSet<LeftCellWrapper> collection = new HashSet<LeftCellWrapper>(relevantWrappers);
                new List<LeftCellWrapper>(collection).Sort(LeftCellWrapper.OriginalCellIdComparer);
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(message);
                EntityConfigurationToUserString(extraConstraint, builder);
                this._errorLog.AddEntry(new ErrorLog.Record(true, errorCode, builder.ToString(), collection, ""));
            }
        }

        internal void Validate()
        {
            Dictionary<MemberValueBinding, CellTreeNode> memberValueTrees = this.CreateMemberValueTrees(false);
            Dictionary<MemberValueBinding, CellTreeNode> dictionary2 = this.CreateMemberValueTrees(true);
            WhereClauseVisitor visitor = new WhereClauseVisitor(this._basicView, memberValueTrees);
            WhereClauseVisitor visitor2 = new WhereClauseVisitor(this._basicView, dictionary2);
            foreach (LeftCellWrapper wrapper in this._normalizer.AllWrappersForExtent)
            {
                Cell onlyInputCell = wrapper.OnlyInputCell;
                CellTreeNode node = new LeafCellTreeNode(this._normalizer, wrapper);
                CellTreeNode cellTreeNode = visitor2.GetCellTreeNode(onlyInputCell.SQuery.WhereClause);
                if (cellTreeNode != null)
                {
                    CellTreeNode node2;
                    BoolExpression expression2;
                    if (cellTreeNode != this._basicView)
                    {
                        node2 = new OpCellTreeNode(this._normalizer, CellTreeOpType.IJ, new CellTreeNode[] { cellTreeNode, this._basicView });
                    }
                    else
                    {
                        node2 = this._basicView;
                    }
                    BoolExpression inExtentCondition = BoolExpression.CreateLiteral(wrapper.CreateRoleBoolean(), this._normalizer.MemberMaps.QueryDomainMap);
                    if (!this.CheckEquivalence(node.RightFragmentQuery, node2.RightFragmentQuery, inExtentCondition, out expression2))
                    {
                        string str = StringUtil.FormatInvariant("{0}", new object[] { this._normalizer.Extent });
                        node.RightFragmentQuery.Condition.ExpensiveSimplify();
                        node2.RightFragmentQuery.Condition.ExpensiveSimplify();
                        string message = System.Data.Entity.Strings.ViewGen_CQ_PartitionConstraint_1(str);
                        this.ReportConstraintViolation(message, expression2, ViewGenErrorCode.PartitionConstraintViolation, node.GetLeaves().Concat<LeftCellWrapper>(node2.GetLeaves()));
                    }
                    CellTreeNode node4 = visitor.GetCellTreeNode(onlyInputCell.SQuery.WhereClause);
                    if (node4 != null)
                    {
                        DomainConstraintVisitor.CheckConstraints(node4, wrapper, this._normalizer, this._errorLog);
                        this.CheckConstraintsOnProjectedConditionMembers(memberValueTrees, wrapper, node2, inExtentCondition);
                    }
                    this.CheckConstraintsOnNonNullableMembers(memberValueTrees, wrapper, node2, inExtentCondition);
                }
            }
            if (this._errorLog.Count > 0)
            {
                ExceptionHelpers.ThrowMappingException(this._errorLog, this._normalizer.Config);
            }
        }

        internal class DomainConstraintVisitor : CellTreeNode.SimpleCellTreeVisitor<bool, bool>
        {
            private ErrorLog m_errorLog;
            private CellNormalizer m_normalizer;
            private LeftCellWrapper m_wrapper;

            private DomainConstraintVisitor(LeftCellWrapper wrapper, CellNormalizer normalizer, ErrorLog errorLog)
            {
                this.m_wrapper = wrapper;
                this.m_normalizer = normalizer;
                this.m_errorLog = errorLog;
            }

            internal static void CheckConstraints(CellTreeNode node, LeftCellWrapper wrapper, CellNormalizer normalizer, ErrorLog errorLog)
            {
                RewritingValidator.DomainConstraintVisitor visitor = new RewritingValidator.DomainConstraintVisitor(wrapper, normalizer, errorLog);
                node.Accept<bool, bool>(visitor, true);
            }

            internal override bool VisitLeaf(LeafCellTreeNode node, bool dummy)
            {
                CellQuery rightCellQuery = this.m_wrapper.RightCellQuery;
                CellQuery query2 = node.LeftCellWrapper.RightCellQuery;
                List<MemberPath> members = new List<MemberPath>();
                if (rightCellQuery != query2)
                {
                    for (int i = 0; i < rightCellQuery.NumProjectedSlots; i++)
                    {
                        JoinTreeSlot slot = rightCellQuery.ProjectedSlotAt(i) as JoinTreeSlot;
                        if (slot != null)
                        {
                            JoinTreeSlot slot2 = query2.ProjectedSlotAt(i) as JoinTreeSlot;
                            if (slot2 != null)
                            {
                                MemberPath item = this.m_normalizer.MemberMaps.ProjectedSlotMap[i];
                                if (!item.IsPartOfKey)
                                {
                                    bool flag;
                                    if ((slot.MemberPath.Extent is EntitySet) && (slot2.MemberPath.Extent is EntitySet))
                                    {
                                        flag = MemberPath.EqualityComparer.Equals(slot.MemberPath, slot2.MemberPath);
                                    }
                                    else
                                    {
                                        flag = slot.MemberPath.LastMember == slot2.MemberPath.LastMember;
                                    }
                                    if (!flag)
                                    {
                                        members.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }
                if (members.Count > 0)
                {
                    string message = System.Data.Entity.Strings.ViewGen_NonKeyProjectedWithOverlappingPartitions_0(MemberPath.PropertiesToUserString(members, false));
                    ErrorLog.Record record = new ErrorLog.Record(true, ViewGenErrorCode.NonKeyProjectedWithOverlappingPartitions, message, new LeftCellWrapper[] { this.m_wrapper, node.LeftCellWrapper }, string.Empty);
                    this.m_errorLog.AddEntry(record);
                }
                return true;
            }

            internal override bool VisitOpNode(OpCellTreeNode node, bool dummy)
            {
                if (node.OpType == CellTreeOpType.LASJ)
                {
                    node.Children[0].Accept<bool, bool>(this, dummy);
                }
                else
                {
                    foreach (CellTreeNode node2 in node.Children)
                    {
                        node2.Accept<bool, bool>(this, dummy);
                    }
                }
                return true;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MemberValueBinding : IEquatable<RewritingValidator.MemberValueBinding>
        {
            internal readonly MemberPath Member;
            internal readonly CellConstant Value;
            public MemberValueBinding(MemberPath member, CellConstant value)
            {
                this.Member = member;
                this.Value = value;
            }

            public override string ToString() => 
                string.Format(CultureInfo.InvariantCulture, "{0}={1}", new object[] { this.Member, this.Value });

            public bool Equals(RewritingValidator.MemberValueBinding other) => 
                (MemberPath.EqualityComparer.Equals(this.Member, other.Member) && CellConstant.EqualityComparer.Equals(this.Value, other.Value));
        }

        private class WhereClauseVisitor : Visitor<DomainConstraint<BoolLiteral, CellConstant>, CellTreeNode>
        {
            private Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> _memberValueTrees;
            private CellNormalizer _normalizer;
            private CellTreeNode _topLevelTree;

            internal WhereClauseVisitor(CellTreeNode topLevelTree, Dictionary<RewritingValidator.MemberValueBinding, CellTreeNode> memberValueTrees)
            {
                this._topLevelTree = topLevelTree;
                this._memberValueTrees = memberValueTrees;
                this._normalizer = topLevelTree.CellNormalizer;
            }

            private IEnumerable<CellTreeNode> AcceptChildren(IEnumerable<BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>> children)
            {
                foreach (BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> iteratorVariable0 in children)
                {
                    yield return iteratorVariable0.Accept<CellTreeNode>(this);
                }
            }

            internal CellTreeNode GetCellTreeNode(BoolExpression whereClause) => 
                whereClause.Tree.Accept<CellTreeNode>(this);

            private bool TryGetCellTreeNode(MemberPath memberPath, CellConstant value, out CellTreeNode singleNode) => 
                this._memberValueTrees.TryGetValue(new RewritingValidator.MemberValueBinding(memberPath, value), out singleNode);

            internal override CellTreeNode VisitAnd(AndExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                IEnumerable<CellTreeNode> enumerable = this.AcceptChildren(expression.Children);
                OpCellTreeNode node = new OpCellTreeNode(this._normalizer, CellTreeOpType.IJ);
                foreach (CellTreeNode node2 in enumerable)
                {
                    if (node2 == null)
                    {
                        return null;
                    }
                    if (node2 != this._topLevelTree)
                    {
                        node.Add(node2);
                    }
                }
                if (node.Children.Count != 0)
                {
                    return node;
                }
                return this._topLevelTree;
            }

            internal override CellTreeNode VisitFalse(FalseExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                throw new NotImplementedException();
            }

            internal override CellTreeNode VisitNot(NotExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                throw new NotImplementedException();
            }

            internal override CellTreeNode VisitOr(OrExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                throw new NotImplementedException();
            }

            internal override CellTreeNode VisitTerm(TermExpr<DomainConstraint<BoolLiteral, CellConstant>> expression)
            {
                OneOfConst identifier = (OneOfConst) expression.Identifier.Variable.Identifier;
                System.Data.Common.Utils.Set<CellConstant> range = expression.Identifier.Range;
                OpCellTreeNode node = new OpCellTreeNode(this._normalizer, CellTreeOpType.Union);
                CellTreeNode singleNode = null;
                foreach (CellConstant constant in range)
                {
                    if (this.TryGetCellTreeNode(identifier.Slot.MemberPath, constant, out singleNode))
                    {
                        node.Add(singleNode);
                    }
                }
                switch (node.Children.Count)
                {
                    case 0:
                        return null;

                    case 1:
                        return singleNode;
                }
                return node;
            }

            internal override CellTreeNode VisitTrue(TrueExpr<DomainConstraint<BoolLiteral, CellConstant>> expression) => 
                this._topLevelTree;

        }
    }
}

