namespace System.Web.UI.HtmlControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [ValidationProperty("Value"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlInputFile : HtmlInputControl, IPostBackDataHandler
    {
        public HtmlInputFile() : base("file")
        {
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            false;

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            HtmlForm form = this.Page.Form;
            if ((form != null) && (form.Enctype.Length == 0))
            {
                form.Enctype = "multipart/form-data";
            }
        }

        protected virtual void RaisePostDataChangedEvent()
        {
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Behavior"), DefaultValue("")]
        public string Accept
        {
            get
            {
                string str = base.Attributes["accept"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["accept"] = HtmlControl.MapStringAttributeToString(value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(""), WebCategory("Behavior")]
        public int MaxLength
        {
            get
            {
                string s = base.Attributes["maxlength"];
                if (s == null)
                {
                    return -1;
                }
                return int.Parse(s, CultureInfo.InvariantCulture);
            }
            set
            {
                base.Attributes["maxlength"] = HtmlControl.MapIntegerAttributeToString(value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Default"), DefaultValue("")]
        public HttpPostedFile PostedFile =>
            this.Context.Request.Files[this.RenderedNameAttribute];

        [DefaultValue(-1), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Appearance")]
        public int Size
        {
            get
            {
                string s = base.Attributes["size"];
                if (s == null)
                {
                    return -1;
                }
                return int.Parse(s, CultureInfo.InvariantCulture);
            }
            set
            {
                base.Attributes["size"] = HtmlControl.MapIntegerAttributeToString(value);
            }
        }

        [Browsable(false)]
        public override string Value
        {
            get
            {
                HttpPostedFile postedFile = this.PostedFile;
                if (postedFile != null)
                {
                    return postedFile.FileName;
                }
                return string.Empty;
            }
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("Value_Set_Not_Supported", new object[] { base.GetType().Name }));
            }
        }
    }
}

