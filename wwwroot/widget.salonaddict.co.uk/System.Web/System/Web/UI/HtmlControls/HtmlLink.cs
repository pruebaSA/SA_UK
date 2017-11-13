namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ControlBuilder(typeof(HtmlEmptyTagControlBuilder)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlLink : HtmlControl
    {
        public HtmlLink() : base("link")
        {
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            writer.WriteBeginTag(this.TagName);
            this.RenderAttributes(writer);
            writer.Write(" />");
        }

        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(this.Href))
            {
                base.Attributes["href"] = base.ResolveClientUrl(this.Href);
            }
            base.RenderAttributes(writer);
        }

        [UrlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Action"), DefaultValue("")]
        public virtual string Href
        {
            get
            {
                string str = base.Attributes["href"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["href"] = HtmlControl.MapStringAttributeToString(value);
            }
        }
    }
}

