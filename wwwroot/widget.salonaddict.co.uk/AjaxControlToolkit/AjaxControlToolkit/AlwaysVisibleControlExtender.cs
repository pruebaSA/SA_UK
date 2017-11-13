namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Globalization;
    using System.Web.UI;

    [ToolboxBitmap(typeof(AlwaysVisibleControlExtender), "AlwaysVisibleControl.AlwaysVisible.ico"), ClientScriptResource("Sys.Extended.UI.AlwaysVisibleControlBehavior", "AlwaysVisibleControl.AlwaysVisibleControlBehavior.js"), Designer("AjaxControlToolkit.AlwaysVisibleControlDesigner, AjaxControlToolkit"), RequiredScript(typeof(AnimationScripts)), DefaultProperty("VerticalOffset"), TargetControlType(typeof(Control))]
    public class AlwaysVisibleControlExtender : ExtenderControlBase
    {
        public override void EnsureValid()
        {
            base.EnsureValid();
            if (this.VerticalOffset < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "AlwaysVisibleControlExtender on '{0}' cannot have a negative VerticalOffset value", new object[] { base.TargetControlID }));
            }
            if (this.HorizontalOffset < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "AlwaysVisibleControlExtender on '{0}' cannot have a negative HorizontalOffset value", new object[] { base.TargetControlID }));
            }
            if (this.ScrollEffectDuration <= 0f)
            {
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, "AlwaysVisibleControlExtender on '{0}' must have a positive ScrollEffectDuration", new object[] { base.TargetControlID }));
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int HorizontalOffset
        {
            get => 
                base.GetPropertyValue<int>("HorizontalOffset", 0);
            set
            {
                base.SetPropertyValue<int>("HorizontalOffset", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public AjaxControlToolkit.HorizontalSide HorizontalSide
        {
            get => 
                base.GetPropertyValue<AjaxControlToolkit.HorizontalSide>("HorizontalSide", AjaxControlToolkit.HorizontalSide.Left);
            set
            {
                base.SetPropertyValue<AjaxControlToolkit.HorizontalSide>("HorizontalSide", value);
            }
        }

        [DefaultValue((float) 0.1f), ExtenderControlProperty]
        public float ScrollEffectDuration
        {
            get => 
                base.GetPropertyValue<float>("ScrollEffectDuration", 0.1f);
            set
            {
                base.SetPropertyValue<float>("ScrollEffectDuration", value);
            }
        }

        [DefaultValue(false), ExtenderControlProperty, ClientPropertyName("useAnimation")]
        public bool UseAnimation
        {
            get => 
                base.GetPropertyValue<bool>("UseAnimation", false);
            set
            {
                base.SetPropertyValue<bool>("UseAnimation", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int VerticalOffset
        {
            get => 
                base.GetPropertyValue<int>("VerticalOffset", 0);
            set
            {
                base.SetPropertyValue<int>("VerticalOffset", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public AjaxControlToolkit.VerticalSide VerticalSide
        {
            get => 
                base.GetPropertyValue<AjaxControlToolkit.VerticalSide>("VerticalSide", AjaxControlToolkit.VerticalSide.Top);
            set
            {
                base.SetPropertyValue<AjaxControlToolkit.VerticalSide>("VerticalSide", value);
            }
        }
    }
}

