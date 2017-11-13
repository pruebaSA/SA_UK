namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.Animation.UpdatePanelAnimationBehavior", "UpdatePanelAnimation.UpdatePanelAnimationBehavior.js"), RequiredScript(typeof(AnimationScripts), 1), RequiredScript(typeof(AnimationExtender), 2), ToolboxBitmap(typeof(UpdatePanelAnimationExtender), "UpdatePanelAnimation.UpdatePanelAnimation.ico"), TargetControlType(typeof(UpdatePanel)), RequiredScript(typeof(CommonToolkitScripts), 0), ToolboxItem("System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), Designer("AjaxControlToolkit.UpdatePanelAnimationDesigner, AjaxControlToolkit")]
    public class UpdatePanelAnimationExtender : AnimationExtenderControlBase
    {
        private Animation _updated;
        private Animation _updating;

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            base.ResolveControlIDs(this._updating);
            base.ResolveControlIDs(this._updated);
            this.ReplaceStaticAnimationTargets(this._updating);
            this.ReplaceStaticAnimationTargets(this._updated);
        }

        private void ReplaceStaticAnimationTargets(Animation animation)
        {
            if (animation != null)
            {
                string str;
                string str2;
                if (((animation.Properties.TryGetValue("AnimationTarget", out str) && !string.IsNullOrEmpty(str)) && (!animation.Properties.TryGetValue("AnimationTargetScript", out str2) || string.IsNullOrEmpty(str2))) && (!animation.Properties.TryGetValue("TargetScript", out str2) || string.IsNullOrEmpty(str2)))
                {
                    animation.Properties.Remove("AnimationTarget");
                    animation.Properties["TargetScript"] = string.Format(CultureInfo.InvariantCulture, "$get('{0}')", new object[] { str });
                }
                foreach (Animation animation2 in animation.Children)
                {
                    this.ReplaceStaticAnimationTargets(animation2);
                }
            }
        }

        [ExtenderControlProperty, Browsable(true), DefaultValue(false)]
        public bool AlwaysFinishOnUpdatingAnimation
        {
            get => 
                base.GetPropertyValue<bool>("AlwaysFinishOnUpdatingAnimation", false);
            set
            {
                base.SetPropertyValue<bool>("AlwaysFinishOnUpdatingAnimation", value);
            }
        }

        [DefaultValue((string) null), Browsable(false), ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Animation OnUpdated
        {
            get => 
                base.GetAnimation(ref this._updated, "OnUpdated");
            set
            {
                base.SetAnimation(ref this._updated, "OnUpdated", value);
            }
        }

        [DefaultValue((string) null), Browsable(false), ExtenderControlProperty, DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Animation OnUpdating
        {
            get => 
                base.GetAnimation(ref this._updating, "OnUpdating");
            set
            {
                base.SetAnimation(ref this._updating, "OnUpdating", value);
            }
        }
    }
}

