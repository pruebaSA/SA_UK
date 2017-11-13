namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(PopupExtender)), ClientScriptResource("Sys.Extended.UI.DropDownBehavior", "DropDown.DropDownBehavior.js"), Designer("AjaxControlToolkit.DropDownDesigner, AjaxControlToolkit"), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ToolboxBitmap(typeof(DropDownExtender), "DropDown.DropDown.ico"), RequiredScript(typeof(AnimationExtender)), ClientCssResource("DropDown.DropDown_resource.css"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(HoverExtender)), TargetControlType(typeof(Control))]
    public class DropDownExtender : DynamicPopulateExtenderControlBase
    {
        private Animation _onHide;
        private Animation _onShow;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (((!string.IsNullOrEmpty(base.DynamicContextKey) || !string.IsNullOrEmpty(base.DynamicServicePath)) || !string.IsNullOrEmpty(base.DynamicServiceMethod)) && string.IsNullOrEmpty(base.DynamicControlID))
            {
                base.DynamicControlID = this.DropDownControlID;
            }
            base.ResolveControlIDs(this._onShow);
            base.ResolveControlIDs(this._onHide);
        }

        [DefaultValue(typeof(Color), ""), ExtenderControlProperty, ClientPropertyName("dropArrowBackgroundColor")]
        public Color DropArrowBackColor
        {
            get => 
                (this.ViewState["DropArrowBackColor"] ?? Color.Empty);
            set
            {
                this.ViewState["DropArrowBackColor"] = value;
            }
        }

        [ClientPropertyName("dropArrowImageUrl"), ExtenderControlProperty, UrlProperty, DefaultValue("")]
        public string DropArrowImageUrl
        {
            get => 
                (this.ViewState["DropArrowImageUrl"] ?? string.Empty);
            set
            {
                this.ViewState["DropArrowImageUrl"] = value;
            }
        }

        [DefaultValue(typeof(Unit), ""), ClientPropertyName("dropArrowWidth"), ExtenderControlProperty]
        public Unit DropArrowWidth
        {
            get => 
                (this.ViewState["DropArrowWidth"] ?? Unit.Empty);
            set
            {
                this.ViewState["DropArrowWidth"] = value;
            }
        }

        [ExtenderControlProperty, ClientPropertyName("dropDownControl"), ElementReference, DefaultValue(""), IDReferenceProperty(typeof(Control))]
        public string DropDownControlID
        {
            get => 
                (this.ViewState["DropDownControlID"] ?? string.Empty);
            set
            {
                this.ViewState["DropDownControlID"] = value;
            }
        }

        [ExtenderControlProperty, DefaultValue(typeof(Color), ""), ClientPropertyName("highlightBackgroundColor")]
        public Color HighlightBackColor
        {
            get => 
                (this.ViewState["HighlightBackColor"] ?? Color.Empty);
            set
            {
                this.ViewState["HighlightBackColor"] = value;
            }
        }

        [DefaultValue(typeof(Color), ""), ClientPropertyName("highlightBorderColor"), ExtenderControlProperty]
        public Color HighlightBorderColor
        {
            get => 
                (this.ViewState["HighlightBorderColor"] ?? Color.Empty);
            set
            {
                this.ViewState["HighlightBorderColor"] = value;
            }
        }

        [ClientPropertyName("populated"), ExtenderControlEvent, DefaultValue(""), Category("Behavior")]
        public string OnClientPopulated
        {
            get => 
                (this.ViewState["OnClientPopulated"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientPopulated"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(""), ClientPropertyName("populating"), ExtenderControlEvent]
        public string OnClientPopulating
        {
            get => 
                (this.ViewState["OnClientPopulating"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientPopulating"] = value;
            }
        }

        [ExtenderControlEvent, DefaultValue(""), ClientPropertyName("popup"), Category("Behavior")]
        public string OnClientPopup
        {
            get => 
                (this.ViewState["OnClientPopup"] ?? string.Empty);
            set
            {
                this.ViewState["OnClientPopup"] = value;
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

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue((string) null), ExtenderControlProperty, ClientPropertyName("onShow"), Browsable(false)]
        public Animation OnShow
        {
            get => 
                base.GetAnimation(ref this._onShow, "OnShow");
            set
            {
                base.SetAnimation(ref this._onShow, "OnShow", value);
            }
        }
    }
}

