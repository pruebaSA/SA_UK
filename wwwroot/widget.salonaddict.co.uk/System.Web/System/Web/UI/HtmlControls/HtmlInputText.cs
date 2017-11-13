namespace System.Web.UI.HtmlControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [SupportsEventValidation, DefaultEvent("ServerChange"), ValidationProperty("Value"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HtmlInputText : HtmlInputControl, IPostBackDataHandler
    {
        private static readonly object EventServerChange = new object();

        [WebSysDescription("HtmlInputText_ServerChange"), WebCategory("Action")]
        public event EventHandler ServerChange
        {
            add
            {
                base.Events.AddHandler(EventServerChange, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventServerChange, value);
            }
        }

        public HtmlInputText() : base("text")
        {
        }

        public HtmlInputText(string type) : base(type)
        {
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string str = this.Value;
            string str2 = postCollection.GetValues(postDataKey)[0];
            if (!str.Equals(str2))
            {
                base.ValidateEvent(postDataKey);
                this.Value = str2;
                return true;
            }
            return false;
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            bool disabled = base.Disabled;
            if (!disabled && (this.Page != null))
            {
                this.Page.RegisterEnabledControl(this);
            }
            if ((!disabled && (base.Events[EventServerChange] == null)) || base.Type.Equals("password", StringComparison.OrdinalIgnoreCase))
            {
                this.ViewState.SetItemDirty("value", false);
            }
        }

        protected virtual void OnServerChange(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventServerChange];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void RaisePostDataChangedEvent()
        {
            this.OnServerChange(EventArgs.Empty);
        }

        protected override void RenderAttributes(HtmlTextWriter writer)
        {
            base.RenderAttributes(writer);
            if (this.Page != null)
            {
                this.Page.ClientScript.RegisterForEventValidation(this.RenderedNameAttribute);
            }
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        [DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), WebCategory("Behavior")]
        public int MaxLength
        {
            get
            {
                string s = (string) this.ViewState["maxlength"];
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

        [DefaultValue(-1), WebCategory("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

        public override string Value
        {
            get
            {
                string str = base.Attributes["value"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                base.Attributes["value"] = HtmlControl.MapStringAttributeToString(value);
            }
        }
    }
}

