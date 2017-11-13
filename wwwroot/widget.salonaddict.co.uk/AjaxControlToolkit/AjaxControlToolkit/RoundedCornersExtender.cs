namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), Designer("AjaxControlToolkit.RoundedCornersDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.RoundedCornersBehavior", "RoundedCorners.RoundedCornersBehavior.js"), TargetControlType(typeof(Control)), ToolboxBitmap(typeof(RoundedCornersExtender), "RoundedCorners.RoundedCorners.ico")]
    public class RoundedCornersExtender : ExtenderControlBase
    {
        [ExtenderControlProperty, DefaultValue(typeof(System.Drawing.Color), "")]
        public System.Drawing.Color BorderColor
        {
            get => 
                base.GetPropertyValue<System.Drawing.Color>("BorderColor", System.Drawing.Color.Empty);
            set
            {
                base.SetPropertyValue<System.Drawing.Color>("BorderColor", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(typeof(System.Drawing.Color), "")]
        public System.Drawing.Color Color
        {
            get => 
                base.GetPropertyValue<System.Drawing.Color>("Color", System.Drawing.Color.Empty);
            set
            {
                base.SetPropertyValue<System.Drawing.Color>("Color", value);
            }
        }

        [DefaultValue(15), ExtenderControlProperty]
        public BoxCorners Corners
        {
            get => 
                base.GetPropertyValue<BoxCorners>("Corners", BoxCorners.All);
            set
            {
                base.SetPropertyValue<BoxCorners>("Corners", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(5)]
        public int Radius
        {
            get => 
                base.GetPropertyValue<int>("Radius", 5);
            set
            {
                base.SetPropertyValue<int>("Radius", value);
            }
        }
    }
}

