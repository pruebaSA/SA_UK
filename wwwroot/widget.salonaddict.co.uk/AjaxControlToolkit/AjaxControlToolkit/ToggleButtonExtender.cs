namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;

    [TargetControlType(typeof(ICheckBoxControl)), ClientScriptResource("Sys.Extended.UI.ToggleButtonBehavior", "ToggleButton.ToggleButton.js"), ToolboxBitmap(typeof(ToggleButtonExtender), "ToggleButton.ToggleButton.ico"), Designer("AjaxControlToolkit.ToggleButtonExtenderDesigner, AjaxControlToolkit")]
    public class ToggleButtonExtender : ExtenderControlBase
    {
        private const string stringCheckedImageAlternateText = "CheckedImageAlternateText";
        private const string stringCheckedImageOverAlternateText = "CheckedImageOverAlternateText";
        private const string stringCheckedImageOverUrl = "CheckedImageOverUrl";
        private const string stringCheckedImageUrl = "CheckedImageUrl";
        private const string stringDisabledCheckedImageUrl = "DisabledCheckedImageUrl";
        private const string stringDisabledUncheckedImageUrl = "DisabledUncheckedImageUrl";
        private const string stringImageHeight = "ImageHeight";
        private const string stringImageWidth = "ImageWidth";
        private const string stringUncheckedImageAlternateText = "UncheckedImageAlternateText";
        private const string stringUncheckedImageOverAlternateText = "UncheckedImageOverAlternateText";
        private const string stringUncheckedImageOverUrl = "UncheckedImageOverUrl";
        private const string stringUncheckedImageUrl = "UncheckedImageUrl";

        [ExtenderControlProperty, DefaultValue("")]
        public string CheckedImageAlternateText
        {
            get => 
                base.GetPropertyValue<string>("CheckedImageAlternateText", "");
            set
            {
                base.SetPropertyValue<string>("CheckedImageAlternateText", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string CheckedImageOverAlternateText
        {
            get => 
                base.GetPropertyValue<string>("CheckedImageOverAlternateText", "");
            set
            {
                base.SetPropertyValue<string>("CheckedImageOverAlternateText", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), UrlProperty]
        public string CheckedImageOverUrl
        {
            get => 
                base.GetPropertyValue<string>("CheckedImageOverUrl", "");
            set
            {
                base.SetPropertyValue<string>("CheckedImageOverUrl", value);
            }
        }

        [RequiredProperty, ExtenderControlProperty, DefaultValue(""), UrlProperty]
        public string CheckedImageUrl
        {
            get => 
                base.GetPropertyValue<string>("CheckedImageUrl", "");
            set
            {
                base.SetPropertyValue<string>("CheckedImageUrl", value);
            }
        }

        [ExtenderControlProperty, UrlProperty, DefaultValue("")]
        public string DisabledCheckedImageUrl
        {
            get => 
                base.GetPropertyValue<string>("DisabledCheckedImageUrl", "");
            set
            {
                base.SetPropertyValue<string>("DisabledCheckedImageUrl", value);
            }
        }

        [ExtenderControlProperty, UrlProperty, DefaultValue("")]
        public string DisabledUncheckedImageUrl
        {
            get => 
                base.GetPropertyValue<string>("DisabledUncheckedImageUrl", "");
            set
            {
                base.SetPropertyValue<string>("DisabledUncheckedImageUrl", value);
            }
        }

        [ExtenderControlProperty, RequiredProperty, DefaultValue(0)]
        public int ImageHeight
        {
            get => 
                base.GetPropertyValue<int>("ImageHeight", 0);
            set
            {
                base.SetPropertyValue<int>("ImageHeight", value);
            }
        }

        [RequiredProperty, DefaultValue(0), ExtenderControlProperty]
        public int ImageWidth
        {
            get => 
                base.GetPropertyValue<int>("ImageWidth", 0);
            set
            {
                base.SetPropertyValue<int>("ImageWidth", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty]
        public string UncheckedImageAlternateText
        {
            get => 
                base.GetPropertyValue<string>("UncheckedImageAlternateText", "");
            set
            {
                base.SetPropertyValue<string>("UncheckedImageAlternateText", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string UncheckedImageOverAlternateText
        {
            get => 
                base.GetPropertyValue<string>("UncheckedImageOverAlternateText", "");
            set
            {
                base.SetPropertyValue<string>("UncheckedImageOverAlternateText", value);
            }
        }

        [UrlProperty, ExtenderControlProperty, DefaultValue("")]
        public string UncheckedImageOverUrl
        {
            get => 
                base.GetPropertyValue<string>("UncheckedImageOverUrl", "");
            set
            {
                base.SetPropertyValue<string>("UncheckedImageOverUrl", value);
            }
        }

        [DefaultValue(""), ExtenderControlProperty, UrlProperty, RequiredProperty]
        public string UncheckedImageUrl
        {
            get => 
                base.GetPropertyValue<string>("UncheckedImageUrl", "");
            set
            {
                base.SetPropertyValue<string>("UncheckedImageUrl", value);
            }
        }
    }
}

