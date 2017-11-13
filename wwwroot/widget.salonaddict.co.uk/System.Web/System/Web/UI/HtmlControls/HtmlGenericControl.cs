namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ConstructorNeedsTag(true), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlGenericControl : HtmlContainerControl
    {
        public HtmlGenericControl() : this("span")
        {
        }

        public HtmlGenericControl(string tag)
        {
            if (tag == null)
            {
                tag = string.Empty;
            }
            base._tagName = tag;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Appearance"), DefaultValue("")]
        public string TagName
        {
            get => 
                base._tagName;
            set
            {
                base._tagName = value;
            }
        }
    }
}

