namespace System.Web.Services.Protocols
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public abstract class MimeParameterReader : MimeFormatter
    {
        protected MimeParameterReader()
        {
        }

        public abstract object[] Read(HttpRequest request);
    }
}

