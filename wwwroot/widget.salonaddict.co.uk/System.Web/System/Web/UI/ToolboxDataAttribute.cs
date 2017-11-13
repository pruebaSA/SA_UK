namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AttributeUsage(AttributeTargets.Class), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ToolboxDataAttribute : Attribute
    {
        private string data = string.Empty;
        public static readonly ToolboxDataAttribute Default = new ToolboxDataAttribute(string.Empty);

        public ToolboxDataAttribute(string data)
        {
            this.data = data;
        }

        public override bool Equals(object obj) => 
            ((obj == this) || (((obj != null) && (obj is ToolboxDataAttribute)) && StringUtil.EqualsIgnoreCase(((ToolboxDataAttribute) obj).Data, this.data)));

        public override int GetHashCode() => 
            this.Data?.GetHashCode();

        public override bool IsDefaultAttribute() => 
            this.Equals(Default);

        public string Data =>
            this.data;
    }
}

