namespace System.Security.Policy
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Security;

    [Serializable, ComVisible(true)]
    public sealed class UnionCodeGroup : CodeGroup, IUnionSemanticCodeGroup
    {
        internal UnionCodeGroup()
        {
        }

        internal UnionCodeGroup(IMembershipCondition membershipCondition, PermissionSet permSet) : base(membershipCondition, permSet)
        {
        }

        public UnionCodeGroup(IMembershipCondition membershipCondition, PolicyStatement policy) : base(membershipCondition, policy)
        {
        }

        public override CodeGroup Copy()
        {
            UnionCodeGroup group = new UnionCodeGroup {
                MembershipCondition = base.MembershipCondition,
                PolicyStatement = base.PolicyStatement,
                Name = base.Name,
                Description = base.Description
            };
            IEnumerator enumerator = base.Children.GetEnumerator();
            while (enumerator.MoveNext())
            {
                group.AddChild((CodeGroup) enumerator.Current);
            }
            return group;
        }

        internal override string GetTypeName() => 
            "System.Security.Policy.UnionCodeGroup";

        public override PolicyStatement Resolve(Evidence evidence)
        {
            if (evidence == null)
            {
                throw new ArgumentNullException("evidence");
            }
            object usedEvidence = null;
            if (!PolicyManager.CheckMembershipCondition(base.MembershipCondition, evidence, out usedEvidence))
            {
                return null;
            }
            PolicyStatement policyStatement = base.PolicyStatement;
            IDelayEvaluatedEvidence dependentEvidence = usedEvidence as IDelayEvaluatedEvidence;
            if ((dependentEvidence != null) && !dependentEvidence.IsVerified)
            {
                policyStatement.AddDependentEvidence(dependentEvidence);
            }
            bool flag2 = false;
            IEnumerator enumerator = base.Children.GetEnumerator();
            while (enumerator.MoveNext() && !flag2)
            {
                PolicyStatement childPolicy = PolicyManager.ResolveCodeGroup(enumerator.Current as CodeGroup, evidence);
                if (childPolicy != null)
                {
                    policyStatement.InplaceUnion(childPolicy);
                    if ((childPolicy.Attributes & PolicyStatementAttribute.Exclusive) == PolicyStatementAttribute.Exclusive)
                    {
                        flag2 = true;
                    }
                }
            }
            return policyStatement;
        }

        public override CodeGroup ResolveMatchingCodeGroups(Evidence evidence)
        {
            if (evidence == null)
            {
                throw new ArgumentNullException("evidence");
            }
            if (!base.MembershipCondition.Check(evidence))
            {
                return null;
            }
            CodeGroup group = this.Copy();
            group.Children = new ArrayList();
            IEnumerator enumerator = base.Children.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CodeGroup group2 = ((CodeGroup) enumerator.Current).ResolveMatchingCodeGroups(evidence);
                if (group2 != null)
                {
                    group.AddChild(group2);
                }
            }
            return group;
        }

        PolicyStatement IUnionSemanticCodeGroup.InternalResolve(Evidence evidence)
        {
            if (evidence == null)
            {
                throw new ArgumentNullException("evidence");
            }
            if (base.MembershipCondition.Check(evidence))
            {
                return base.PolicyStatement;
            }
            return null;
        }

        public override string MergeLogic =>
            Environment.GetResourceString("MergeLogic_Union");
    }
}

