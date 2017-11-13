namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(AnimationScripts)), TargetControlType(typeof(TextBox)), ToolboxBitmap(typeof(SliderExtender), "Slider.Slider.ico"), RequiredScript(typeof(TimerScript)), Designer("AjaxControlToolkit.SliderDesigner, AjaxControlToolkit"), ClientCssResource("Slider.Slider_resource.css"), ClientScriptResource("Sys.Extended.UI.SliderBehavior", "Slider.SliderBehavior_resource.js"), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(DragDropScripts))]
    public class SliderExtender : ExtenderControlBase
    {
        [DefaultValue(""), ExtenderControlProperty, IDReferenceProperty(typeof(WebControl))]
        public string BoundControlID
        {
            get => 
                base.GetPropertyValue<string>("BoundControlID", "");
            set
            {
                base.SetPropertyValue<string>("BoundControlID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public int Decimals
        {
            get => 
                base.GetPropertyValue<int>("Decimals", 0);
            set
            {
                base.SetPropertyValue<int>("Decimals", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(false)]
        public bool EnableHandleAnimation
        {
            get => 
                base.GetPropertyValue<bool>("EnableHandleAnimation", false);
            set
            {
                base.SetPropertyValue<bool>("EnableHandleAnimation", value);
            }
        }

        [Description("Determines if the slider will respond to arrow keys when it has focus."), ExtenderControlProperty, DefaultValue(true), ClientPropertyName("enableKeyboard")]
        public bool EnableKeyboard
        {
            get => 
                base.GetPropertyValue<bool>("EnableKeyboard", true);
            set
            {
                base.SetPropertyValue<bool>("EnableKeyboard", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string HandleCssClass
        {
            get => 
                base.GetPropertyValue<string>("HandleCssClass", "");
            set
            {
                base.SetPropertyValue<string>("HandleCssClass", value);
            }
        }

        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, DefaultValue(""), ExtenderControlProperty]
        public string HandleImageUrl
        {
            get => 
                base.GetPropertyValue<string>("HandleImageUrl", "");
            set
            {
                base.SetPropertyValue<string>("HandleImageUrl", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(150)]
        public int Length
        {
            get => 
                base.GetPropertyValue<int>("Length", 150);
            set
            {
                base.SetPropertyValue<int>("Length", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(100)]
        public double Maximum
        {
            get => 
                base.GetPropertyValue<double>("Maximum", 100.0);
            set
            {
                base.SetPropertyValue<double>("Maximum", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public double Minimum
        {
            get => 
                base.GetPropertyValue<double>("Minimum", 0.0);
            set
            {
                base.SetPropertyValue<double>("Minimum", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0)]
        public SliderOrientation Orientation
        {
            get => 
                base.GetPropertyValue<SliderOrientation>("Orientation", SliderOrientation.Horizontal);
            set
            {
                base.SetPropertyValue<SliderOrientation>("Orientation", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string RailCssClass
        {
            get => 
                base.GetPropertyValue<string>("RailCssClass", "");
            set
            {
                base.SetPropertyValue<string>("RailCssClass", value);
            }
        }

        [DefaultValue(true), ExtenderControlProperty]
        public bool RaiseChangeOnlyOnMouseUp
        {
            get => 
                base.GetPropertyValue<bool>("RaiseChangeOnlyOnMouseUp", true);
            set
            {
                base.SetPropertyValue<bool>("RaiseChangeOnlyOnMouseUp", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int Steps
        {
            get => 
                base.GetPropertyValue<int>("Steps", 0);
            set
            {
                base.SetPropertyValue<int>("Steps", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string TooltipText
        {
            get => 
                base.GetPropertyValue<string>("TooltipText", string.Empty);
            set
            {
                base.SetPropertyValue<string>("TooltipText", value);
            }
        }
    }
}

