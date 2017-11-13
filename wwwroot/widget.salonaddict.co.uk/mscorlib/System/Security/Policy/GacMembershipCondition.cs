namespace System.Security.Policy
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class GacMembershipCondition : IConstantMembershipCondition, IReportMatchMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
    {
        public bool Check(Evidence evidence)
        {
            object usedEvidence = null;
            return ((IReportMatchMembershipCondition) this).Check(evidence, out usedEvidence);
        }

        public IMembershipCondition Copy() => 
            new GacMembershipCondition();

        public override bool Equals(object o) => 
            (o is GacMembershipCondition);

        public void FromXml(SecurityElement e)
        {
            this.FromXml(e, null);
        }

        public void FromXml(SecurityElement e, PolicyLevel level)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }
            if (!e.Tag.Equals("IMembershipCondition"))
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_MembershipConditionElement"));
            }
        }

        public override int GetHashCode() => 
            0;

        bool IReportMatchMembershipCondition.Check(Evidence evidence, out object usedEvidence)
        {
            usedEvidence = null;
            if (evidence != null)
            {
                IEnumerator hostEnumerator = evidence.GetHostEnumerator();
                while (hostEnumerator.MoveNext())
                {
                    object current = hostEnumerator.Current;
                    if (current is GacInstalled)
                    {
                        usedEvidence = current;
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString() => 
            Environment.GetResourceString("GAC_ToString");

        public SecurityElement ToXml() => 
            this.ToXml(null);

        public SecurityElement ToXml(PolicyLevel level)
        {
            SecurityElement element = new SecurityElement("IMembershipCondition");
            XMLUtil.AddClassAttribute(element, base.GetType(), base.GetType().FullName);
            element.AddAttribute("version", "1");
            return element;
        }
    }
}

