namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(SlideShowExtender), "SlideShow.SlideShow.ico"), Designer("AjaxControlToolkit.SlideShowDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.SlideShowBehavior", "SlideShow.SlideShowBehavior.js"), TargetControlType(typeof(System.Web.UI.WebControls.Image)), RequiredScript(typeof(CommonToolkitScripts)), RequiredScript(typeof(TimerScript))]
    public class SlideShowExtender : ExtenderControlBase
    {
        [DefaultValue(false), ClientPropertyName("autoPlay"), ExtenderControlProperty]
        public bool AutoPlay
        {
            get => 
                base.GetPropertyValue<bool>("AutoPlay", false);
            set
            {
                base.SetPropertyValue<bool>("AutoPlay", value);
            }
        }

        [ClientPropertyName("contextKey"), ExtenderControlProperty, DefaultValue((string) null)]
        public string ContextKey
        {
            get => 
                base.GetPropertyValue<string>("ContextKey", null);
            set
            {
                base.SetPropertyValue<string>("ContextKey", value);
                this.UseContextKey = true;
            }
        }

        [ExtenderControlProperty, IDReferenceProperty(typeof(WebControl)), ClientPropertyName("imageDescriptionLabelID"), DefaultValue("")]
        public string ImageDescriptionLabelID
        {
            get => 
                base.GetPropertyValue<string>("ImageDescriptionLabelID", "");
            set
            {
                base.SetPropertyValue<string>("ImageDescriptionLabelID", value);
            }
        }

        [IDReferenceProperty(typeof(WebControl)), ExtenderControlProperty, ClientPropertyName("imageTitleLabelID"), DefaultValue("")]
        public string ImageTitleLabelID
        {
            get => 
                base.GetPropertyValue<string>("ImageTitleLabelID", "");
            set
            {
                base.SetPropertyValue<string>("ImageTitleLabelID", value);
            }
        }

        [ClientPropertyName("loop"), ExtenderControlProperty, DefaultValue(false)]
        public bool Loop
        {
            get => 
                base.GetPropertyValue<bool>("Loop", false);
            set
            {
                base.SetPropertyValue<bool>("Loop", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("nextButtonID"), ExtenderControlProperty, IDReferenceProperty(typeof(WebControl))]
        public string NextButtonID
        {
            get => 
                base.GetPropertyValue<string>("NextButtonID", "");
            set
            {
                base.SetPropertyValue<string>("NextButtonID", value);
            }
        }

        [ClientPropertyName("playButtonID"), ExtenderControlProperty, DefaultValue(""), IDReferenceProperty(typeof(WebControl))]
        public string PlayButtonID
        {
            get => 
                base.GetPropertyValue<string>("PlayButtonID", "");
            set
            {
                base.SetPropertyValue<string>("PlayButtonID", value);
            }
        }

        [DefaultValue(""), ClientPropertyName("playButtonText"), ExtenderControlProperty]
        public string PlayButtonText
        {
            get => 
                base.GetPropertyValue<string>("PlayButtonText", "");
            set
            {
                base.SetPropertyValue<string>("PlayButtonText", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(0xbb8), ClientPropertyName("playInterval")]
        public int PlayInterval
        {
            get => 
                base.GetPropertyValue<int>("PlayInterval", 0xbb8);
            set
            {
                base.SetPropertyValue<int>("PlayInterval", value);
            }
        }

        [IDReferenceProperty(typeof(WebControl)), DefaultValue(""), ExtenderControlProperty, ClientPropertyName("previousButtonID")]
        public string PreviousButtonID
        {
            get => 
                base.GetPropertyValue<string>("PreviousButtonID", "");
            set
            {
                base.SetPropertyValue<string>("PreviousButtonID", value);
            }
        }

        [ClientPropertyName("slideShowServiceMethod"), RequiredProperty, DefaultValue(""), ExtenderControlProperty]
        public string SlideShowServiceMethod
        {
            get => 
                base.GetPropertyValue<string>("SlideShowServiceMethod", "");
            set
            {
                base.SetPropertyValue<string>("SlideShowServiceMethod", value);
            }
        }

        [ExtenderControlProperty, TypeConverter(typeof(ServicePathConverter)), ClientPropertyName("slideShowServicePath"), UrlProperty]
        public string SlideShowServicePath
        {
            get => 
                base.GetPropertyValue<string>("SlideShowServicePath", "");
            set
            {
                base.SetPropertyValue<string>("SlideShowServicePath", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(""), ClientPropertyName("stopButtonText")]
        public string StopButtonText
        {
            get => 
                base.GetPropertyValue<string>("StopButtonText", "");
            set
            {
                base.SetPropertyValue<string>("StopButtonText", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("useContextKey"), DefaultValue(false)]
        public bool UseContextKey
        {
            get => 
                base.GetPropertyValue<bool>("UseContextKey", false);
            set
            {
                base.SetPropertyValue<bool>("UseContextKey", value);
            }
        }
    }
}

