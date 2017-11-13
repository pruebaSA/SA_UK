namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;

    [RequiredScript(typeof(TimerScript), 3), RequiredScript(typeof(CommonToolkitScripts), 1), RequiredScript(typeof(RoundedCornersExtender), 2), Designer("AjaxControlToolkit.DropShadowDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.DropShadowBehavior", "DropShadow.DropShadowBehavior.js"), ToolboxBitmap(typeof(DropShadowExtender), "DropShadow.DropShadow.ico"), TargetControlType(typeof(Control))]
    public class DropShadowExtender : ExtenderControlBase
    {
        [ExtenderControlProperty, DefaultValue((float) 1f)]
        public float Opacity
        {
            get => 
                base.GetPropertyValue<float>("Opacity", 1f);
            set
            {
                base.SetPropertyValue<float>("Opacity", value);
            }
        }

        [DefaultValue(5), ExtenderControlProperty]
        public int Radius
        {
            get => 
                base.GetPropertyValue<int>("Radius", 5);
            set
            {
                base.SetPropertyValue<int>("Radius", value);
            }
        }

        [DefaultValue(false), ExtenderControlProperty]
        public bool Rounded
        {
            get => 
                base.GetPropertyValue<bool>("Rounded", false);
            set
            {
                base.SetPropertyValue<bool>("Rounded", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool TrackPosition
        {
            get => 
                base.GetPropertyValue<bool>("TrackPosition", false);
            set
            {
                base.SetPropertyValue<bool>("TrackPosition", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(5)]
        public int Width
        {
            get => 
                base.GetPropertyValue<int>("Width", 5);
            set
            {
                base.SetPropertyValue<int>("Width", value);
            }
        }
    }
}

