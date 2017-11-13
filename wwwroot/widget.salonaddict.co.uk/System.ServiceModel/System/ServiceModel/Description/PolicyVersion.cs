namespace System.ServiceModel.Description
{
    using System;

    public sealed class PolicyVersion
    {
        private string policyNamespace;
        private static PolicyVersion policyVersion12 = new PolicyVersion("http://schemas.xmlsoap.org/ws/2004/09/policy");
        private static PolicyVersion policyVersion15 = new PolicyVersion("http://www.w3.org/ns/ws-policy");

        private PolicyVersion(string policyNamespace)
        {
            this.policyNamespace = policyNamespace;
        }

        public override string ToString() => 
            this.policyNamespace;

        public static PolicyVersion Default =>
            policyVersion12;

        public string Namespace =>
            this.policyNamespace;

        public static PolicyVersion Policy12 =>
            policyVersion12;

        public static PolicyVersion Policy15 =>
            policyVersion15;
    }
}

