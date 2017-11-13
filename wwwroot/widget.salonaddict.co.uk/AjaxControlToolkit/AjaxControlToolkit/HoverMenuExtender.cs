namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(HoverMenuExtender), "HoverMenu.HoverMenu.ico"), RequiredScript(typeof(PopupExtender)), RequiredScript(typeof(AnimationExtender)), TargetControlType(typeof(Control)), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ClientScriptResource("Sys.Extended.UI.HoverMenuBehavior", "HoverMenu.HoverMenuBehavior.js"), Designer("AjaxControlToolkit.HoverMenuDesigner, AjaxControlToolkit"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(HoverExtender))]
    public class HoverMenuExtender : DynamicPopulateExtenderControlBase
    {
        private Animation _onHide;
        private Animation _onShow;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ResolveControlIDs(this._onShow);
            base.ResolveControlIDs(this._onHide);
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string HoverCssClass
        {
            get => 
                base.GetPropertyValue<string>("HoverCssClass", "");
            set
            {
                base.SetPropertyValue<string>("HoverCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int HoverDelay
        {
            get => 
                base.GetPropertyValue<int>("HoverDelay", 0);
            set
            {
                base.SetPropertyValue<int>("HoverDelay", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
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

        [Browsable(false), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ExtenderControlProperty, ClientPropertyName("onHide")]
        public Animation OnHide
        {
            get => 
                base.GetAnimation(ref this._onHide, "OnHide");
            set
            {
                base.SetAnimation(ref this._onHide, "OnHide", value);
            }
        }

        [ClientPropertyName("onShow"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ExtenderControlProperty, DefaultValue((string) null)]
        public Animation OnShow
        {
            get => 
                base.GetAnimation(ref this._onShow, "OnShow");
            set
            {
                base.SetAnimation(ref this._onShow, "OnShow", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int PopDelay
        {
            get => 
                base.GetPropertyValue<int>("PopDelay", 0);
            set
            {
                base.SetPropertyValue<int>("PopDelay", value);
            }
        }

        [RequiredProperty, DefaultValue(""), ElementReference, IDReferenceProperty(typeof(WebControl)), ExtenderControlProperty, ClientPropertyName("popupElement")]
        public string PopupControlID
        {
            get => 
                base.GetPropertyValue<string>("PopupControlID", "");
            set
            {
                base.SetPropertyValue<string>("PopupControlID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public HoverMenuPopupPosition PopupPosition
        {
            get => 
                base.GetPropertyValue<HoverMenuPopupPosition>("Position", HoverMenuPopupPosition.Center);
            set
            {
                base.SetPropertyValue<HoverMenuPopupPosition>("Position", value);
            }
        }
    }
}

