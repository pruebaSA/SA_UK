namespace System.Web.UI.HtmlControls
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlSelectBuilder : ControlBuilder
    {
        public override bool AllowWhitespaceLiterals() => 
            false;

        public override Type GetChildControlType(string tagName, IDictionary attribs)
        {
            if (StringUtil.EqualsIgnoreCase(tagName, "option"))
            {
                return typeof(ListItem);
            }
            return null;
        }
    }
}

