namespace System.Web.Query.Dynamic
{
    using System;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ParseException : Exception
    {
        private int position;

        public ParseException(string message, int position) : base(message)
        {
            this.position = position;
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, AtlasWeb.ParseException_ParseExceptionFormat, new object[] { this.Message, this.position });

        public int Position =>
            this.position;
    }
}

