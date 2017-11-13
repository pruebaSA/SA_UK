namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(BalloonPopupExtender), "BalloonPopup.BalloonPopup.ico"), ClientScriptResource("Sys.Extended.UI.BalloonPopupControlBehavior", "BalloonPopup.BalloonPopupExtenderBehavior.js"), RequiredScript(typeof(PopupExtender)), RequiredScript(typeof(CommonToolkitScripts)), TargetControlType(typeof(Control)), ClientCssResource("BalloonPopup.Rectangle.BalloonPopup_resource.css"), ClientCssResource("BalloonPopup.Cloud.BalloonPopup_resource.css"), Designer("AjaxControlToolkit.BalloonPopupDesigner, AjaxControlToolkit"), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class BalloonPopupExtender : DynamicPopulateExtenderControlBase
    {
        private Animation _onHide;
        private Animation _onShow;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.BalloonStyle == BalloonPopupStyle.Custom)
            {
                if (this.CustomCssUrl == "")
                {
                    throw new ArgumentException("Must pass CustomCssUrl value.");
                }
                if (this.CustomClassName == "")
                {
                    throw new ArgumentException("Must pass CustomClassName value.");
                }
                bool flag = false;
                foreach (Control control in this.Page.Header.Controls)
                {
                    if (control.ID == "customCssUrl")
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    HtmlLink child = new HtmlLink {
                        Href = base.ResolveUrl(this.CustomCssUrl),
                        Attributes = { 
                            ["id"] = "customCssUrl",
                            ["rel"] = "stylesheet",
                            ["type"] = "text/css",
                            ["media"] = "all"
                        }
                    };
                    this.Page.Header.Controls.Add(child);
                }
            }
        }

        [DefaultValue(""), ExtenderControlProperty, IDReferenceProperty(typeof(WebControl)), RequiredProperty]
        public string BalloonPopupControlID
        {
            get => 
                base.GetPropertyValue<string>("BalloonPopupControlID", "");
            set
            {
                base.SetPropertyValue<string>("BalloonPopupControlID", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("balloonSize"), DefaultValue(0)]
        public BalloonPopupSize BalloonSize
        {
            get => 
                base.GetPropertyValue<BalloonPopupSize>("BalloonSize", BalloonPopupSize.Small);
            set
            {
                base.SetPropertyValue<BalloonPopupSize>("BalloonSize", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty, ClientPropertyName("balloonPopupStyle")]
        public BalloonPopupStyle BalloonStyle { get; set; }

        [DefaultValue(""), ClientPropertyName("customClassName"), ExtenderControlProperty]
        public string CustomClassName
        {
            get => 
                base.GetPropertyValue<string>("CustomClassName", "");
            set
            {
                base.SetPropertyValue<string>("CustomClassName", value);
            }
        }

        [DefaultValue("")]
        public string CustomCssUrl { get; set; }

        [ExtenderControlProperty, ClientPropertyName("displayOnClick"), DefaultValue(true)]
        public bool DisplayOnClick
        {
            get => 
                base.GetPropertyValue<bool>("DisplayOnClick", true);
            set
            {
                base.SetPropertyValue<bool>("DisplayOnClick", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("displayOnFocus"), DefaultValue(false)]
        public bool DisplayOnFocus
        {
            get => 
                base.GetPropertyValue<bool>("DisplayOnFocus", false);
            set
            {
                base.SetPropertyValue<bool>("DisplayOnFocus", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false), ClientPropertyName("displayOnMouseOver")]
        public bool DisplayOnMouseOver
        {
            get => 
                base.GetPropertyValue<bool>("DisplayOnMouseOver", false);
            set
            {
                base.SetPropertyValue<bool>("DisplayOnMouseOver", value);
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

        [ExtenderControlProperty, DefaultValue(0)]
        public int OffsetY
        {
            get => 
                base.GetPropertyValue<int>("OffsetY", 0);
            set
            {
                base.SetPropertyValue<int>("OffsetY", value);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ExtenderControlProperty, ClientPropertyName("onHide"), DefaultValue((string) null)]
        public Animation OnHide
        {
            get => 
                base.GetAnimation(ref this._onHide, "OnHide");
            set
            {
                base.SetAnimation(ref this._onHide, "OnHide", value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ExtenderControlProperty, ClientPropertyName("onShow"), Browsable(false), DefaultValue((string) null)]
        public Animation OnShow
        {
            get => 
                base.GetAnimation(ref this._onShow, "OnShow");
            set
            {
                base.SetAnimation(ref this._onShow, "OnShow", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0), ClientPropertyName("balloonPopupPosition")]
        public BalloonPopupPosition Position { get; set; }

        [ExtenderControlProperty, DefaultValue(4), Category("Behavior"), ClientPropertyName("scrollBars"), Description("Scroll bars behavior when content is overflow")]
        public System.Web.UI.WebControls.ScrollBars ScrollBars
        {
            get => 
                base.GetPropertyValue<System.Web.UI.WebControls.ScrollBars>("ScrollBars", System.Web.UI.WebControls.ScrollBars.Auto);
            set
            {
                base.SetPropertyValue<System.Web.UI.WebControls.ScrollBars>("ScrollBars", value);
            }
        }

        [DefaultValue(true), ExtenderControlProperty, ClientPropertyName("useShadow")]
        public bool UseShadow
        {
            get => 
                base.GetPropertyValue<bool>("UseShadow", true);
            set
            {
                base.SetPropertyValue<bool>("UseShadow", value);
            }
        }
    }
}

