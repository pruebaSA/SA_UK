namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Web.UI;

    [TargetControlType(typeof(Accordion)), ClientScriptResource("Sys.Extended.UI.AccordionBehavior", "Accordion.AccordionBehavior.js"), RequiredScript(typeof(CommonToolkitScripts)), ToolboxItem(false), Designer("AjaxControlToolkit.AccordionExtenderDesigner, AjaxControlToolkit"), RequiredScript(typeof(AnimationScripts))]
    public class AccordionExtender : ExtenderControlBase
    {
        public AccordionExtender()
        {
            base.EnableClientState = true;
        }

        [DefaultValue(0), ExtenderControlProperty]
        public AjaxControlToolkit.AutoSize AutoSize
        {
            get => 
                base.GetPropertyValue<AjaxControlToolkit.AutoSize>("AutoSize", AjaxControlToolkit.AutoSize.None);
            set
            {
                base.SetPropertyValue<AjaxControlToolkit.AutoSize>("AutoSize", value);
            }
        }

        [DefaultValue("")]
        public string ContentCssClass
        {
            get => 
                base.GetPropertyValue<string>("ContentCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("ContentCssClass", value);
            }
        }

        [DefaultValue(false), ExtenderControlProperty]
        public bool FadeTransitions
        {
            get => 
                base.GetPropertyValue<bool>("FadeTransitions", false);
            set
            {
                base.SetPropertyValue<bool>("FadeTransitions", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(30)]
        public int FramesPerSecond
        {
            get => 
                base.GetPropertyValue<int>("FramesPerSecond", 30);
            set
            {
                base.SetPropertyValue<int>("FramesPerSecond", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string HeaderCssClass
        {
            get => 
                base.GetPropertyValue<string>("HeaderCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("HeaderCssClass", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string HeaderSelectedCssClass
        {
            get => 
                base.GetPropertyValue<string>("HeaderSelectedCssClass", string.Empty);
            set
            {
                base.SetPropertyValue<string>("HeaderSelectedCssClass", value);
            }
        }

        [DefaultValue(true), ExtenderControlProperty, ClientPropertyName("requireOpenedPane")]
        public bool RequireOpenedPane
        {
            get => 
                base.GetPropertyValue<bool>("RequireOpenedPane", true);
            set
            {
                base.SetPropertyValue<bool>("RequireOpenedPane", value);
            }
        }

        [DefaultValue(0), ExtenderControlProperty]
        public int SelectedIndex
        {
            get
            {
                int num;
                if (!string.IsNullOrEmpty(base.ClientState) && int.TryParse(base.ClientState, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
                {
                    return num;
                }
                return 0;
            }
            set
            {
                base.ClientState = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        [DefaultValue(false), ExtenderControlProperty, ClientPropertyName("suppressHeaderPostbacks")]
        public bool SuppressHeaderPostbacks
        {
            get => 
                base.GetPropertyValue<bool>("SuppressHeaderPostbacks", false);
            set
            {
                base.SetPropertyValue<bool>("SuppressHeaderPostbacks", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(250)]
        public int TransitionDuration
        {
            get => 
                base.GetPropertyValue<int>("TransitionDuration", 250);
            set
            {
                base.SetPropertyValue<int>("TransitionDuration", value);
            }
        }
    }
}

