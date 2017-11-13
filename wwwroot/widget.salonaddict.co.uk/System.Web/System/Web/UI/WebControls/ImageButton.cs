namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [Designer("System.Web.UI.Design.WebControls.PreviewControlDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), SupportsEventValidation, DefaultEvent("Click"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ImageButton : Image, IPostBackDataHandler, IPostBackEventHandler, IButtonControl
    {
        private static readonly object EventButtonClick = new object();
        private static readonly object EventClick = new object();
        private static readonly object EventCommand = new object();
        private int x;
        private int y;

        [WebSysDescription("ImageButton_OnClick"), WebCategory("Action")]
        public event ImageClickEventHandler Click
        {
            add
            {
                base.Events.AddHandler(EventClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventClick, value);
            }
        }

        [WebCategory("Action"), WebSysDescription("ImageButton_OnCommand")]
        public event CommandEventHandler Command
        {
            add
            {
                base.Events.AddHandler(EventCommand, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventCommand, value);
            }
        }

        event EventHandler IButtonControl.Click
        {
            add
            {
                base.Events.AddHandler(EventButtonClick, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventButtonClick, value);
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            Page page = this.Page;
            if (page != null)
            {
                page.VerifyRenderingInServerForm(this);
            }
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "image");
            string uniqueID = this.UniqueID;
            PostBackOptions postBackOptions = this.GetPostBackOptions();
            if ((uniqueID != null) && ((postBackOptions == null) || (postBackOptions.TargetControl == this)))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Name, uniqueID);
            }
            bool isEnabled = base.IsEnabled;
            string firstScript = string.Empty;
            if (isEnabled)
            {
                firstScript = Util.EnsureEndWithSemiColon(this.OnClientClick);
                if (base.HasAttributes)
                {
                    string str3 = base.Attributes["onclick"];
                    if (str3 != null)
                    {
                        firstScript = firstScript + Util.EnsureEndWithSemiColon(str3);
                        base.Attributes.Remove("onclick");
                    }
                }
            }
            if (this.Enabled && !isEnabled)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
            }
            base.AddAttributesToRender(writer);
            if ((page != null) && (postBackOptions != null))
            {
                page.ClientScript.RegisterForEventValidation(postBackOptions);
                if (isEnabled)
                {
                    string postBackEventReference = page.ClientScript.GetPostBackEventReference(postBackOptions, false);
                    if (!string.IsNullOrEmpty(postBackEventReference))
                    {
                        firstScript = Util.MergeScript(firstScript, postBackEventReference);
                    }
                }
            }
            if (firstScript.Length > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Onclick, firstScript);
                if (base.EnableLegacyRendering)
                {
                    writer.AddAttribute("language", "javascript", false);
                }
            }
        }

        protected virtual PostBackOptions GetPostBackOptions()
        {
            PostBackOptions options = new PostBackOptions(this, string.Empty) {
                ClientSubmit = false
            };
            if (!string.IsNullOrEmpty(this.PostBackUrl))
            {
                options.ActionUrl = HttpUtility.UrlPathEncode(base.ResolveClientUrl(this.PostBackUrl));
            }
            if ((this.CausesValidation && (this.Page != null)) && (this.Page.GetValidators(this.ValidationGroup).Count > 0))
            {
                options.PerformValidation = true;
                options.ValidationGroup = this.ValidationGroup;
            }
            return options;
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            string uniqueID = this.UniqueID;
            string requestValue = postCollection[uniqueID + ".x"];
            string str3 = postCollection[uniqueID + ".y"];
            if (((requestValue != null) && (str3 != null)) && ((requestValue.Length > 0) && (str3.Length > 0)))
            {
                this.x = (int) ReadPositionFromPost(requestValue);
                this.y = (int) ReadPositionFromPost(str3);
                if (this.Page != null)
                {
                    this.Page.RegisterRequiresRaiseEvent(this);
                }
            }
            return false;
        }

        protected virtual void OnClick(ImageClickEventArgs e)
        {
            ImageClickEventHandler handler = (ImageClickEventHandler) base.Events[EventClick];
            if (handler != null)
            {
                handler(this, e);
            }
            EventHandler handler2 = (EventHandler) base.Events[EventButtonClick];
            if (handler2 != null)
            {
                handler2(this, e);
            }
        }

        protected virtual void OnCommand(CommandEventArgs e)
        {
            CommandEventHandler handler = (CommandEventHandler) base.Events[EventCommand];
            if (handler != null)
            {
                handler(this, e);
            }
            base.RaiseBubbleEvent(this, e);
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Page != null)
            {
                this.Page.RegisterRequiresPostBack(this);
                if (base.IsEnabled && ((this.CausesValidation && (this.Page.GetValidators(this.ValidationGroup).Count > 0)) || !string.IsNullOrEmpty(this.PostBackUrl)))
                {
                    this.Page.RegisterWebFormsScript();
                }
            }
        }

        protected virtual void RaisePostBackEvent(string eventArgument)
        {
            base.ValidateEvent(this.UniqueID, eventArgument);
            if (this.CausesValidation)
            {
                this.Page.Validate(this.ValidationGroup);
            }
            this.OnClick(new ImageClickEventArgs(this.x, this.y));
            this.OnCommand(new CommandEventArgs(this.CommandName, this.CommandArgument));
        }

        protected virtual void RaisePostDataChangedEvent()
        {
        }

        private static double ReadPositionFromPost(string requestValue)
        {
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.Integer;
            return double.Parse(requestValue, style, CultureInfo.InvariantCulture);
        }

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.RaisePostBackEvent(eventArgument);
        }

        [Themeable(false), WebSysDescription("Button_CausesValidation"), DefaultValue(true), WebCategory("Behavior")]
        public virtual bool CausesValidation
        {
            get
            {
                object obj2 = this.ViewState["CausesValidation"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["CausesValidation"] = value;
            }
        }

        [Bindable(true), DefaultValue(""), Themeable(false), WebSysDescription("WebControl_CommandArgument"), WebCategory("Behavior")]
        public string CommandArgument
        {
            get
            {
                string str = (string) this.ViewState["CommandArgument"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["CommandArgument"] = value;
            }
        }

        [DefaultValue(""), Themeable(false), WebSysDescription("WebControl_CommandName"), WebCategory("Behavior")]
        public string CommandName
        {
            get
            {
                string str = (string) this.ViewState["CommandName"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["CommandName"] = value;
            }
        }

        [DefaultValue(true), Browsable(true), WebCategory("Behavior"), EditorBrowsable(EditorBrowsableState.Always), Bindable(true), WebSysDescription("WebControl_Enabled")]
        public override bool Enabled
        {
            get => 
                base.Enabled;
            set
            {
                base.Enabled = value;
            }
        }

        [Themeable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool GenerateEmptyAlternateText
        {
            get => 
                base.GenerateEmptyAlternateText;
            set
            {
                throw new NotSupportedException(System.Web.SR.GetString("Property_Set_Not_Supported", new object[] { "GenerateEmptyAlternateText", base.GetType().ToString() }));
            }
        }

        [WebCategory("Behavior"), WebSysDescription("Button_OnClientClick"), DefaultValue(""), Themeable(false)]
        public virtual string OnClientClick
        {
            get
            {
                string str = (string) this.ViewState["OnClientClick"];
                if (str == null)
                {
                    return string.Empty;
                }
                return str;
            }
            set
            {
                this.ViewState["OnClientClick"] = value;
            }
        }

        [Editor("System.Web.UI.Design.UrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), WebSysDescription("Button_PostBackUrl"), Themeable(false), UrlProperty("*.aspx"), WebCategory("Behavior")]
        public virtual string PostBackUrl
        {
            get
            {
                string str = (string) this.ViewState["PostBackUrl"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["PostBackUrl"] = value;
            }
        }

        string IButtonControl.Text
        {
            get => 
                this.Text;
            set
            {
                this.Text = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Input;

        protected virtual string Text
        {
            get => 
                this.AlternateText;
            set
            {
                this.AlternateText = value;
            }
        }

        [WebSysDescription("PostBackControl_ValidationGroup"), WebCategory("Behavior"), Themeable(false), DefaultValue("")]
        public virtual string ValidationGroup
        {
            get
            {
                string str = (string) this.ViewState["ValidationGroup"];
                if (str != null)
                {
                    return str;
                }
                return string.Empty;
            }
            set
            {
                this.ViewState["ValidationGroup"] = value;
            }
        }
    }
}

