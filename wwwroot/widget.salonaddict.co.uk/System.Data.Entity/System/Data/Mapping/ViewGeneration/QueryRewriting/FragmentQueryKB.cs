namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Common.Utils.Boolean;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Data.Metadata.Edm;
    using System.Linq;

    internal class FragmentQueryKB : KnowledgeBase<DomainConstraint<BoolLiteral, CellConstant>>
    {
        private BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> _kbExpression = TrueExpr<DomainConstraint<BoolLiteral, CellConstant>>.Value;

        internal override void AddFact(BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> fact)
        {
            base.AddFact(fact);
            this._kbExpression = new AndExpr<DomainConstraint<BoolLiteral, CellConstant>>(new BoolExpr<DomainConstraint<BoolLiteral, CellConstant>>[] { this._kbExpression, fact });
        }

        internal void CreateAssociationConstraints(EntitySetBase extent, MemberDomainMap domainMap, MetadataWorkspace workspace)
        {
            AssociationSet set = extent as AssociationSet;
            if (set != null)
            {
                BoolExpression expression = BoolExpression.CreateLiteral(new RoleBoolean(set), domainMap);
                HashSet<Pair<EdmMember, EntityType>> associationkeys = new HashSet<Pair<EdmMember, EntityType>>();
                foreach (AssociationEndMember member in set.ElementType.AssociationEndMembers)
                {
                    EntityType type = (EntityType) ((RefType) member.TypeUsage.EdmType).ElementType;
                    type.KeyMembers.All<EdmMember>(delegate (EdmMember member) {
                        associationkeys.Add(new Pair<EdmMember, EntityType>(member, type));
                        return true;
                    });
                }
                foreach (AssociationSetEnd end in set.AssociationSetEnds)
                {
                    BoolExpression expression2 = CreateIsOfTypeCondition(new MemberPath(end.EntitySet, workspace), end.CorrespondingAssociationEndMember.TypeUsage.EdmType, domainMap, workspace);
                    BoolExpression expression3 = BoolExpression.CreateLiteral(new RoleBoolean(end), domainMap);
                    BoolExpression expression4 = BoolExpression.CreateAnd(new BoolExpression[] { BoolExpression.CreateLiteral(new RoleBoolean(end.EntitySet), domainMap), expression2 });
                    base.AddImplication(expression3.Tree, expression4.Tree);
                    if (MetadataHelper.IsEveryOtherEndAtLeastOne(set, end.CorrespondingAssociationEndMember))
                    {
                        base.AddImplication(expression4.Tree, expression3.Tree);
                    }
                    if (MetadataHelper.DoesEndKeySubsumeAssociationSetKey(set, end.CorrespondingAssociationEndMember, associationkeys))
                    {
                        base.AddEquivalence(expression3.Tree, expression.Tree);
                    }
                }
                foreach (ReferentialConstraint constraint in set.ElementType.ReferentialConstraints)
                {
                    AssociationEndMember toRole = (AssociationEndMember) constraint.ToRole;
                    EntitySet entitySetAtEnd = MetadataHelper.GetEntitySetAtEnd(set, toRole);
                    if (Helpers.IsSetEqual<EdmMember>(Helpers.AsSuperTypeList<EdmProperty, EdmMember>(constraint.ToProperties), entitySetAtEnd.ElementType.KeyMembers, EqualityComparer<EdmMember>.Default) && constraint.FromRole.RelationshipMultiplicity.Equals(RelationshipMultiplicity.One))
                    {
                        BoolExpression expression5 = BoolExpression.CreateLiteral(new RoleBoolean(set.AssociationSetEnds[0]), domainMap);
                        BoolExpression expression6 = BoolExpression.CreateLiteral(new RoleBoolean(set.AssociationSetEnds[1]), domainMap);
                        base.AddEquivalence(expression5.Tree, expression6.Tree);
                    }
                }
            }
        }

        private static BoolExpression CreateIsOfTypeCondition(MemberPath currentPath, EdmType possibleType, MemberDomainMap domainMap, MetadataWorkspace workspace)
        {
            HashSet<EdmType> set = new HashSet<EdmType>();
            set.UnionWith(MetadataHelper.GetTypeAndSubtypesOf(possibleType, workspace, false));
            CellConstantDomain domain = new CellConstantDomain(from derivedType in set select new TypeConstant(derivedType), domainMap.GetDomain(currentPath));
            return BoolExpression.CreateLiteral(new OneOfTypeConst(FragmentQuery.CreateSlot(currentPath, workspace), domain), domainMap);
        }

        internal void CreateVariableConstraints(EntitySetBase extent, MemberDomainMap domainMap, MetadataWorkspace workspace)
        {
            this.CreateVariableConstraintsRecursion(extent.ElementType, new MemberPath(extent, workspace), domainMap, workspace);
        }

        private void CreateVariableConstraintsRecursion(EdmType edmType, MemberPath currentPath, MemberDomainMap domainMap, MetadataWorkspace workspace)
        {
            HashSet<EdmType> set = new HashSet<EdmType>();
            set.UnionWith(MetadataHelper.GetTypeAndSubtypesOf(edmType, workspace, false));
            foreach (EdmType type in set)
            {
                BoolExpression expression2 = BoolExpression.CreateNot(CreateIsOfTypeCondition(currentPath, type, domainMap, workspace));
                if (expression2.IsSatisfiable())
                {
                    StructuralType type2 = (StructuralType) type;
                    foreach (EdmProperty property in type2.GetDeclaredOnlyMembers<EdmProperty>())
                    {
                        MemberPath path = new MemberPath(currentPath, property);
                        bool flag = MetadataHelper.IsNonRefSimpleMember(property);
                        if (domainMap.IsConditionMember(path) || domainMap.IsProjectedConditionMember(path))
                        {
                            BoolExpression expression3;
                            List<CellConstant> possibleDiscreteValues = new List<CellConstant>(domainMap.GetDomain(path));
                            if (flag)
                            {
                                expression3 = BoolExpression.CreateLiteral(new OneOfScalarConst(FragmentQuery.CreateSlot(path, workspace), new CellConstantDomain(CellConstant.Undefined, possibleDiscreteValues)), domainMap);
                            }
                            else
                            {
                                expression3 = BoolExpression.CreateLiteral(new OneOfTypeConst(FragmentQuery.CreateSlot(path, workspace), new CellConstantDomain(CellConstant.Undefined, possibleDiscreteValues)), domainMap);
                            }
                            base.AddEquivalence(expression2.Tree, expression3.Tree);
                        }
                        if (!flag)
                        {
                            this.CreateVariableConstraintsRecursion(path.EdmType, path, domainMap, workspace);
                        }
                    }
                }
            }
        }

        internal BoolExpr<DomainConstraint<BoolLiteral, CellConstant>> KbExpression =>
            this._kbExpression;
    }
}

