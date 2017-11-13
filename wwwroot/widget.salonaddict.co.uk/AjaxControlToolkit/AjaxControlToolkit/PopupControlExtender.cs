namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [Designer("AjaxControlToolkit.PopupControlDesigner, AjaxControlToolkit"), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ClientScriptResource("Sys.Extended.UI.PopupControlBehavior", "PopupControl.PopupControlBehavior.js"), RequiredScript(typeof(PopupExtender)), RequiredScript(typeof(CommonToolkitScripts)), TargetControlType(typeof(Control)), ToolboxBitmap(typeof(PopupControlExtender), "PopupControl.PopupControl.ico")]
    public class PopupControlExtender : DynamicPopulateExtenderControlBase
    {
        private string _closeString;
        private Animation _onHide;
        private Animation _onShow;
        private EventHandler _pagePreRenderHandler;
        private Page _proxyForCurrentPopup;
        private bool _shouldClose;

        public PopupControlExtender()
        {
        }

        private PopupControlExtender(Page page)
        {
            this._proxyForCurrentPopup = page;
            this._pagePreRenderHandler = new EventHandler(this.Page_PreRender);
            this._proxyForCurrentPopup.PreRender += this._pagePreRenderHandler;
        }

        public void Cancel()
        {
            this._closeString = "$$CANCEL$$";
            this._shouldClose = true;
        }

        private void Close(string result)
        {
            if (this._proxyForCurrentPopup == null)
            {
                ScriptManager.GetCurrent(this.Page).RegisterDataItem(base.TargetControl, result);
            }
            else
            {
                LiteralControl child = new LiteralControl {
                    ID = "_PopupControl_Proxy_ID_"
                };
                this._proxyForCurrentPopup.Controls.Add(child);
                ScriptManager.GetCurrent(this._proxyForCurrentPopup).RegisterDataItem(child, result);
            }
        }

        public void Commit(string result)
        {
            this._closeString = result;
            this._shouldClose = true;
        }

        public static PopupControlExtender GetProxyForCurrentPopup(Page page) => 
            new PopupControlExtender(page);

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (this._pagePreRenderHandler == null)
            {
                this._pagePreRenderHandler = new EventHandler(this.Page_PreRender);
                this.Page.PreRender += this._pagePreRenderHandler;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ResolveControlIDs(this._onShow);
            base.ResolveControlIDs(this._onHide);
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (this._shouldClose)
            {
                this.Close(this._closeString);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string CommitProperty
        {
            get => 
                base.GetPropertyValue<string>("CommitProperty", "");
            set
            {
                base.SetPropertyValue<string>("CommitProperty", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string CommitScript
        {
            get => 
                base.GetPropertyValue<string>("CommitScript", "");
            set
            {
                base.SetPropertyValue<string>("CommitScript", value);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public string ExtenderControlID
        {
            get => 
                base.GetPropertyValue<string>("ExtenderControlID", "");
            set
            {
                base.SetPropertyValue<string>("ExtenderControlID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int OffsetX
        {
            get => 
                base.GetPropertyValue<int>("OffsetX", 0);
            set
            {
                base.SetPropertyValue<int>("OffsetX", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int OffsetY
        {
            get => 
                base.GetPropertyValue<int>("OffsetY", 0);
            set
            {
                base.SetPropertyValue<int>("OffsetY", value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue((string) null), ExtenderControlProperty, ClientPropertyName("onHide"), Browsable(false)]
        public Animation OnHide
        {
            get => 
                base.GetAnimation(ref this._onHide, "OnHide");
            set
            {
                base.SetAnimation(ref this._onHide, "OnHide", value);
            }
        }

        [ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ClientPropertyName("onShow"), Browsable(false), DefaultValue((string) null)]
        public Animation OnShow
        {
            get => 
                base.GetAnimation(ref this._onShow, "OnShow");
            set
            {
                base.SetAnimation(ref this._onShow, "OnShow", value);
            }
        }

        [ExtenderControlProperty, RequiredProperty, DefaultValue(""), IDReferenceProperty(typeof(WebControl))]
        public string PopupControlID
        {
            get => 
                base.GetPropertyValue<string>("PopupControlID", "");
            set
            {
                base.SetPropertyValue<string>("PopupControlID", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public PopupControlPopupPosition Position
        {
            get => 
                base.GetPropertyValue<PopupControlPopupPosition>("Position", PopupControlPopupPosition.Center);
            set
            {
                base.SetPropertyValue<PopupControlPopupPosition>("Position", value);
            }
        }
    }
}

