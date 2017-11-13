namespace System.Security.Policy
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Util;

    [Serializable, ComVisible(true)]
    public sealed class ApplicationDirectoryMembershipCondition : IConstantMembershipCondition, IReportMatchMembershipCondition, IMembershipCondition, ISecurityEncodable, ISecurityPolicyEncodable
    {
        public bool Check(Evidence evidence)
        {
            object usedEvidence = null;
            return ((IReportMatchMembershipCondition) this).Check(evidence, out usedEvidence);
        }

        public IMembershipCondition Copy() => 
            new ApplicationDirectoryMembershipCondition();

        public override bool Equals(object o) => 
            (o is ApplicationDirectoryMembershipCondition);

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
            typeof(ApplicationDirectoryMembershipCondition).GetHashCode();

        bool IReportMatchMembershipCondition.Check(Evidence evidence, out object usedEvidence)
        {
            usedEvidence = null;
            if (evidence != null)
            {
                IEnumerator hostEnumerator = evidence.GetHostEnumerator();
                while (hostEnumerator.MoveNext())
                {
                    ApplicationDirectory current = hostEnumerator.Current as ApplicationDirectory;
                    if (current != null)
                    {
                        IEnumerator enumerator2 = evidence.GetHostEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Url url = enumerator2.Current as Url;
                            if (url != null)
                            {
                                string directory = current.Directory;
                                if ((directory != null) && (directory.Length > 1))
                                {
                                    if (directory[directory.Length - 1] == '/')
                                    {
                                        directory = directory + "*";
                                    }
                                    else
                                    {
                                        directory = directory + "/*";
                                    }
                                    URLString operand = new URLString(directory);
                                    if (url.GetURLString().IsSubsetOf(operand))
                                    {
                                        usedEvidence = current;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override string ToString() => 
            Environment.GetResourceString("ApplicationDirectory_ToString");

        public SecurityElement ToXml() => 
            this.ToXml(null);

        public SecurityElement ToXml(PolicyLevel level)
        {
            SecurityElement element = new SecurityElement("IMembershipCondition");
            XMLUtil.AddClassAttribute(element, base.GetType(), "System.Security.Policy.ApplicationDirectoryMembershipCondition");
            element.AddAttribute("version", "1");
            return element;
        }
    }
}

