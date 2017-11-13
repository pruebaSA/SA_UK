namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.PopupBehavior", "PopupExtender.PopupBehavior.js"), RequiredScript(typeof(AnimationExtender)), Designer("AjaxControlToolkit.PopupExtenderDesigner, AjaxControlToolkit"), RequiredScript(typeof(CommonToolkitScripts)), TargetControlType(typeof(Control))]
    public class PopupExtender : AnimationExtenderControlBase
    {
        private Animation _onHide;
        private Animation _onShow;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ResolveControlIDs(this._onShow);
            base.ResolveControlIDs(this._onHide);
        }

        [DefaultValue((string) null), ClientPropertyName("onHide"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ExtenderControlProperty, Browsable(false)]
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

        [RequiredProperty, ElementReference, ClientPropertyName("parentElement"), ExtenderControlProperty, IDReferenceProperty]
        public string ParentElementID
        {
            get => 
                base.GetPropertyValue<string>("ParentElementID", "");
            set
            {
                base.SetPropertyValue<string>("ParentElementID", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty, ClientPropertyName("positioningMode")]
        public AjaxControlToolkit.PositioningMode PositioningMode
        {
            get => 
                base.GetPropertyValue<AjaxControlToolkit.PositioningMode>("PositioningMode", AjaxControlToolkit.PositioningMode.Absolute);
            set
            {
                base.SetPropertyValue<AjaxControlToolkit.PositioningMode>("PositioningMode", value);
            }
        }

        [ClientPropertyName("reparent"), ExtenderControlProperty, DefaultValue(false)]
        public bool Reparent
        {
            get => 
                base.GetPropertyValue<bool>("Reparent", false);
            set
            {
                base.SetPropertyValue<bool>("Reparent", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0), ClientPropertyName("x")]
        public int X
        {
            get => 
                base.GetPropertyValue<int>("X", 0);
            set
            {
                base.SetPropertyValue<int>("X", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty, ClientPropertyName("y")]
        public int Y
        {
            get => 
                base.GetPropertyValue<int>("Y", 0);
            set
            {
                base.SetPropertyValue<int>("Y", value);
            }
        }
    }
}

