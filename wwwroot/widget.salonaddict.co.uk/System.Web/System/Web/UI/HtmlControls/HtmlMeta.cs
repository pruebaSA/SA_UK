namespace System.Web.UI.HtmlControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ControlBuilder(typeof(HtmlEmptyTagControlBuilder)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlMeta : HtmlControl
    {
        public HtmlMeta() : base("meta")
        {
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            if (base.EnableLegacyRendering)
            {
                base.Render(writer);
            }
            else
            {
                writer.WriteBeginTag(this.TagName);
                this.RenderAttributes(writer);
                writer.Write(" />");
            }
        }

        [WebCategory("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue("")]
        public virtual string Content
        {
            get
            {
                string str = base.Attributes["content"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["content"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        [WebCategory("Appearance"), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string HttpEquiv
        {
            get
            {
                string str = base.Attributes["http-equiv"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["http-equiv"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        [DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Appearance")]
        public virtual string Name
        {
            get
            {
                string str = base.Attributes["name"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["name"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        [WebCategory("Appearance"), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual string Scheme
        {
            get
            {
                string str = base.Attributes["scheme"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["scheme"] = HtmlControl.MapStringAttributeToString(value);
            }
        }
    }
}

