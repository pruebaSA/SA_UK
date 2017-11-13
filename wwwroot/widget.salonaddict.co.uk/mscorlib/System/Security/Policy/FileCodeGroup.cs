namespace System.Security.Policy
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class FileCodeGroup : CodeGroup, IUnionSemanticCodeGroup
    {
        private FileIOPermissionAccess m_access;

        internal FileCodeGroup()
        {
        }

        public FileCodeGroup(IMembershipCondition membershipCondition, FileIOPermissionAccess access) : base(membershipCondition, (PolicyStatement) null)
        {
            this.m_access = access;
        }

        private PolicyStatement CalculateAssemblyPolicy(Evidence evidence)
        {
            PolicyStatement statement = null;
            IEnumerator hostEnumerator = evidence.GetHostEnumerator();
            while (hostEnumerator.MoveNext())
            {
                Url current = hostEnumerator.Current as Url;
                if (current != null)
                {
                    statement = this.CalculatePolicy(current);
                }
            }
            if (statement == null)
            {
                statement = new PolicyStatement(new PermissionSet(false), PolicyStatementAttribute.Nothing);
            }
            return statement;
        }

        internal PolicyStatement CalculatePolicy(Url url)
        {
            URLString uRLString = url.GetURLString();
            if (string.Compare(uRLString.Scheme, "file", StringComparison.OrdinalIgnoreCase) != 0)
            {
                return null;
            }
            string directoryName = uRLString.GetDirectoryName();
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.SetPermission(new FileIOPermission(this.m_access, Path.GetFullPath(directoryName)));
            return new PolicyStatement(permSet, PolicyStatementAttribute.Nothing);
        }

        public override CodeGroup Copy()
        {
            FileCodeGroup group = new FileCodeGroup(base.MembershipCondition, this.m_access) {
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

        protected override void CreateXml(SecurityElement element, PolicyLevel level)
        {
            element.AddAttribute("Access", XMLUtil.BitFieldEnumToString(typeof(FileIOPermissionAccess), this.m_access));
        }

        public override bool Equals(object o)
        {
            FileCodeGroup group = o as FileCodeGroup;
            return (((group != null) && base.Equals(group)) && (this.m_access == group.m_access));
        }

        public override int GetHashCode() => 
            (base.GetHashCode() + this.m_access.GetHashCode());

        internal override string GetTypeName() => 
            "System.Security.Policy.FileCodeGroup";

        protected override void ParseXml(SecurityElement e, PolicyLevel level)
        {
            string str = e.Attribute("Access");
            if (str != null)
            {
                this.m_access = (FileIOPermissionAccess) Enum.Parse(typeof(FileIOPermissionAccess), str);
            }
            else
            {
                this.m_access = FileIOPermissionAccess.NoAccess;
            }
        }

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
            PolicyStatement statement = this.CalculateAssemblyPolicy(evidence);
            IDelayEvaluatedEvidence dependentEvidence = usedEvidence as IDelayEvaluatedEvidence;
            if ((dependentEvidence != null) && !dependentEvidence.IsVerified)
            {
                statement.AddDependentEvidence(dependentEvidence);
            }
            bool flag2 = false;
            IEnumerator enumerator = base.Children.GetEnumerator();
            while (enumerator.MoveNext() && !flag2)
            {
                PolicyStatement childPolicy = PolicyManager.ResolveCodeGroup(enumerator.Current as CodeGroup, evidence);
                if (childPolicy != null)
                {
                    statement.InplaceUnion(childPolicy);
                    if ((childPolicy.Attributes & PolicyStatementAttribute.Exclusive) == PolicyStatementAttribute.Exclusive)
                    {
                        flag2 = true;
                    }
                }
            }
            return statement;
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
                return this.CalculateAssemblyPolicy(evidence);
            }
            return null;
        }

        public override string AttributeString =>
            null;

        public override string MergeLogic =>
            Environment.GetResourceString("MergeLogic_Union");

        public override string PermissionSetName =>
            string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("FileCodeGroup_PermissionSet"), new object[] { XMLUtil.BitFieldEnumToString(typeof(FileIOPermissionAccess), this.m_access) });
    }
}

