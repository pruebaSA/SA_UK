namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [Designer("AjaxControlToolkit.ValidatorCalloutDesigner, AjaxControlToolkit"), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ClientScriptResource("Sys.Extended.UI.ValidatorCalloutBehavior", "ValidatorCallout.ValidatorCalloutBehavior.js"), TargetControlType(typeof(IValidator)), RequiredScript(typeof(PopupExtender)), RequiredScript(typeof(AnimationExtender)), RequiredScript(typeof(CommonToolkitScripts)), ToolboxBitmap(typeof(ValidatorCalloutExtender), "ValidatorCallout.ValidatorCallout.ico"), ClientCssResource("ValidatorCallout.ValidatorCallout.css")]
    public class ValidatorCalloutExtender : AnimationExtenderControlBase
    {
        private Animation _onHide;
        private Animation _onShow;

        public ValidatorCalloutExtender()
        {
            base.EnableClientState = true;
        }

        protected override void OnPreRender(EventArgs e)
        {
            BaseValidator targetControl = base.TargetControl as BaseValidator;
            if ((targetControl != null) && !targetControl.IsValid)
            {
                base.ClientState = "INVALID";
            }
            else
            {
                base.ClientState = "";
            }
            base.OnPreRender(e);
            base.ResolveControlIDs(this._onShow);
            base.ResolveControlIDs(this._onHide);
        }

        [ExtenderControlProperty, ClientPropertyName("closeImageUrl"), DefaultValue(""), UrlProperty]
        public string CloseImageUrl
        {
            get
            {
                // This item is obfuscated and can not be translated.
                string expressionStack_F_0;
                string propertyValue = base.GetPropertyValue<string>("CloseImageUrl", null);
                if (propertyValue != null)
                {
                    return propertyValue;
                }
                else
                {
                    expressionStack_F_0 = propertyValue;
                }
                expressionStack_F_0 = "";
                if (!base.DesignMode)
                {
                    return this.Page.ClientScript.GetWebResourceUrl(typeof(ValidatorCalloutExtender), "ValidatorCallout.close.gif");
                }
                return "";
            }
            set
            {
                base.SetPropertyValue<string>("CloseImageUrl", value);
            }
        }

        [ClientPropertyName("cssClass"), DefaultValue(""), ExtenderControlProperty]
        public string CssClass
        {
            get => 
                base.GetPropertyValue<string>("CssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("CssClass", value);
            }
        }

        [ClientPropertyName("highlightCssClass"), DefaultValue(""), ExtenderControlProperty]
        public string HighlightCssClass
        {
            get => 
                base.GetPropertyValue<string>("HighlightCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("HighlightCssClass", value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), ClientPropertyName("onHide"), DefaultValue((string) null), ExtenderControlProperty]
        public Animation OnHide
        {
            get => 
                base.GetAnimation(ref this._onHide, "OnHide");
            set
            {
                base.SetAnimation(ref this._onHide, "OnHide", value);
            }
        }

        [DefaultValue((string) null), ClientPropertyName("onShow"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), ExtenderControlProperty]
        public Animation OnShow
        {
            get => 
                base.GetAnimation(ref this._onShow, "OnShow");
            set
            {
                base.SetAnimation(ref this._onShow, "OnShow", value);
            }
        }

        [ExtenderControlProperty, Description("Indicates where you want the ValidatorCallout displayed."), ClientPropertyName("popupPosition"), DefaultValue(0)]
        public virtual ValidatorCalloutPosition PopupPosition
        {
            get => 
                base.GetPropertyValue<ValidatorCalloutPosition>("PopupPosition", ValidatorCalloutPosition.Right);
            set
            {
                base.SetPropertyValue<ValidatorCalloutPosition>("PopupPosition", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("warningIconImageUrl"), UrlProperty, DefaultValue("")]
        public string WarningIconImageUrl
        {
            get
            {
                // This item is obfuscated and can not be translated.
                string expressionStack_F_0;
                string propertyValue = base.GetPropertyValue<string>("WarningIconImageUrl", null);
                if (propertyValue != null)
                {
                    return propertyValue;
                }
                else
                {
                    expressionStack_F_0 = propertyValue;
                }
                expressionStack_F_0 = "";
                if (!base.DesignMode)
                {
                    return this.Page.ClientScript.GetWebResourceUrl(typeof(ValidatorCalloutExtender), "ValidatorCallout.alert-large.gif");
                }
                return "";
            }
            set
            {
                base.SetPropertyValue<string>("WarningIconImageUrl", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("width"), DefaultValue(typeof(Unit), "")]
        public Unit Width
        {
            get => 
                base.GetPropertyValue<Unit>("Width", Unit.Empty);
            set
            {
                base.SetPropertyValue<Unit>("Width", value);
            }
        }
    }
}

