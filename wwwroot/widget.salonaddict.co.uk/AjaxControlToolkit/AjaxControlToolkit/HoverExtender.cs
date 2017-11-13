namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ToolboxItem(false), Designer("AjaxControlToolkit.HoverExtenderDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.HoverBehavior", "HoverExtender.HoverBehavior.js"), TargetControlType(typeof(Control))]
    public class HoverExtender : ExtenderControlBase
    {
        [DefaultValue(0), ClientPropertyName("hoverDelay"), ExtenderControlProperty]
        public int HoverDelay
        {
            get => 
                base.GetPropertyValue<int>("hoverDelay", 0);
            set
            {
                base.SetPropertyValue<int>("hoverDelay", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("hoverScript")]
        public string HoverScript
        {
            get => 
                base.GetPropertyValue<string>("HoverScript", "");
            set
            {
                base.SetPropertyValue<string>("HoverScript", value);
            }
        }

        [ExtenderControlProperty, ClientPropertyName("unhoverDelay"), DefaultValue(0)]
        public int UnhoverDelay
        {
            get => 
                base.GetPropertyValue<int>("UnhoverDelay", 0);
            set
            {
                base.SetPropertyValue<int>("UnhoverDelay", value);
            }
        }

        [ClientPropertyName("unhoverScript"), ExtenderControlProperty]
        public string UnhoverScript
        {
            get => 
                base.GetPropertyValue<string>("UnhoverScript", "");
            set
            {
                base.SetPropertyValue<string>("UnhoverScript", value);
            }
        }
    }
}

