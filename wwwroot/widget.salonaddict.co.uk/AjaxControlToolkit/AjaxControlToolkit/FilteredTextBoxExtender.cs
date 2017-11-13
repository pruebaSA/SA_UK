namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [TargetControlType(typeof(TextBox)), RequiredScript(typeof(CommonToolkitScripts)), ToolboxBitmap(typeof(FilteredTextBoxExtender), "FilteredTextBox.FilteredTextBox.ico"), Designer("AjaxControlToolkit.FilteredTextBoxDesigner, AjaxControlToolkit"), DefaultProperty("ValidChars"), ClientScriptResource("Sys.Extended.UI.FilteredTextBoxBehavior", "FilteredTextBox.FilteredTextBoxBehavior.js")]
    public class FilteredTextBoxExtender : ExtenderControlBase
    {
        protected override bool CheckIfValid(bool throwException)
        {
            if ((this.FilterType != FilterTypes.Custom) || (((this.FilterMode != FilterModes.ValidChars) || !string.IsNullOrEmpty(this.ValidChars)) && ((this.FilterMode != FilterModes.InvalidChars) || !string.IsNullOrEmpty(this.InvalidChars))))
            {
                return base.CheckIfValid(throwException);
            }
            if (throwException)
            {
                throw new InvalidOperationException("If FilterTypes.Custom is specified, please provide a value for ValidChars or InvalidChars");
            }
            return false;
        }

        [DefaultValue(250), ExtenderControlProperty]
        public int FilterInterval
        {
            get => 
                base.GetPropertyValue<int>("FilterInterval", 250);
            set
            {
                base.SetPropertyValue<int>("FilterInterval", value);
            }
        }

        [DefaultValue(1), ExtenderControlProperty]
        public FilterModes FilterMode
        {
            get => 
                base.GetPropertyValue<FilterModes>("FilterMode", FilterModes.ValidChars);
            set
            {
                base.SetPropertyValue<FilterModes>("FilterMode", value);
            }
        }

        [ExtenderControlProperty, DefaultValue(1)]
        public FilterTypes FilterType
        {
            get => 
                base.GetPropertyValue<FilterTypes>("FilterType", FilterTypes.Custom);
            set
            {
                base.SetPropertyValue<FilterTypes>("FilterType", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string InvalidChars
        {
            get => 
                base.GetPropertyValue<string>("InvalidChars", "");
            set
            {
                base.SetPropertyValue<string>("InvalidChars", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string ValidChars
        {
            get => 
                base.GetPropertyValue<string>("ValidChars", "");
            set
            {
                base.SetPropertyValue<string>("ValidChars", value);
            }
        }
    }
}

