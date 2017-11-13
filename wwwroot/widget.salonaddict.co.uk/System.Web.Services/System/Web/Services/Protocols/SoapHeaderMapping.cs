namespace System.Web.Services.Protocols
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust")]
    public sealed class SoapHeaderMapping
    {
        internal bool custom;
        internal SoapHeaderDirection direction;
        internal Type headerType;
        internal System.Reflection.MemberInfo memberInfo;
        internal bool repeats;

        internal SoapHeaderMapping()
        {
        }

        public bool Custom =>
            this.custom;

        public SoapHeaderDirection Direction =>
            this.direction;

        public Type HeaderType =>
            this.headerType;

        public System.Reflection.MemberInfo MemberInfo =>
            this.memberInfo;

        public bool Repeats =>
            this.repeats;
    }
}

