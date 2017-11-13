namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;

    [RequiredScript(typeof(AnimationScripts)), TargetControlType(typeof(Control)), ClientScriptResource("Sys.Extended.UI.Animation.AnimationBehavior", "Animation.AnimationBehavior.js"), ToolboxBitmap(typeof(AnimationExtender), "Animation.Animation.ico"), Designer("AjaxControlToolkit.AnimationExtenderDesigner, AjaxControlToolkit"), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class AnimationExtender : AnimationExtenderControlBase
    {
        private Animation _onClick;
        private Animation _onHoverOut;
        private Animation _onHoverOver;
        private Animation _onLoad;
        private Animation _onMouseOut;
        private Animation _onMouseOver;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ResolveControlIDs(this._onLoad);
            base.ResolveControlIDs(this._onClick);
            base.ResolveControlIDs(this._onMouseOver);
            base.ResolveControlIDs(this._onMouseOut);
            base.ResolveControlIDs(this._onHoverOver);
            base.ResolveControlIDs(this._onHoverOut);
        }

        [DefaultValue((string) null), Browsable(false), ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Animation OnClick
        {
            get => 
                base.GetAnimation(ref this._onClick, "OnClick");
            set
            {
                base.SetAnimation(ref this._onClick, "OnClick", value);
            }
        }

        [Browsable(false), ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue((string) null)]
        public Animation OnHoverOut
        {
            get => 
                base.GetAnimation(ref this._onHoverOut, "OnHoverOut");
            set
            {
                base.SetAnimation(ref this._onHoverOut, "OnHoverOut", value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ExtenderControlProperty, DefaultValue((string) null), Browsable(false)]
        public Animation OnHoverOver
        {
            get => 
                base.GetAnimation(ref this._onHoverOver, "OnHoverOver");
            set
            {
                base.SetAnimation(ref this._onHoverOver, "OnHoverOver", value);
            }
        }

        [DefaultValue((string) null), Browsable(false), ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Animation OnLoad
        {
            get => 
                base.GetAnimation(ref this._onLoad, "OnLoad");
            set
            {
                base.SetAnimation(ref this._onLoad, "OnLoad", value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), ExtenderControlProperty, DefaultValue((string) null)]
        public Animation OnMouseOut
        {
            get => 
                base.GetAnimation(ref this._onMouseOut, "OnMouseOut");
            set
            {
                base.SetAnimation(ref this._onMouseOut, "OnMouseOut", value);
            }
        }

        [DefaultValue((string) null), Browsable(false), ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Animation OnMouseOver
        {
            get => 
                base.GetAnimation(ref this._onMouseOver, "OnMouseOver");
            set
            {
                base.SetAnimation(ref this._onMouseOver, "OnMouseOver", value);
            }
        }
    }
}

