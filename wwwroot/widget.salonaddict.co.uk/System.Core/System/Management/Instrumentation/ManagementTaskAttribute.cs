namespace System.Management.Instrumentation
{
    using System;
    using System.Security.Permissions;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class ManagementTaskAttribute : ManagementMemberAttribute
    {
        private Type _schema;

        public Type Schema
        {
            get => 
                this._schema;
            set
            {
                this._schema = value;
            }
        }
    }
}

