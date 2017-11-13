namespace System.Security.Policy
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class GacInstalled : IIdentityPermissionFactory, IBuiltInEvidence
    {
        public object Copy() => 
            new GacInstalled();

        public IPermission CreateIdentityPermission(Evidence evidence) => 
            new GacIdentityPermission();

        public override bool Equals(object o) => 
            (o is GacInstalled);

        public override int GetHashCode() => 
            0;

        int IBuiltInEvidence.GetRequiredSize(bool verbose) => 
            1;

        int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position) => 
            position;

        int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
        {
            buffer[position] = '\t';
            return (position + 1);
        }

        public override string ToString() => 
            this.ToXml().ToString();

        internal SecurityElement ToXml()
        {
            SecurityElement element = new SecurityElement(base.GetType().FullName);
            element.AddAttribute("version", "1");
            return element;
        }
    }
}

