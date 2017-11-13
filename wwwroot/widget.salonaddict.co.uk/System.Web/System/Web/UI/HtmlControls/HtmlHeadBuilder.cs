namespace System.Web.UI.HtmlControls
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlHeadBuilder : ControlBuilder
    {
        public override bool AllowWhitespaceLiterals() => 
            false;

        public override Type GetChildControlType(string tagName, IDictionary attribs)
        {
            if (string.Equals(tagName, "title", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(HtmlTitle);
            }
            if (string.Equals(tagName, "link", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(HtmlLink);
            }
            if (string.Equals(tagName, "meta", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(HtmlMeta);
            }
            return null;
        }
    }
}

