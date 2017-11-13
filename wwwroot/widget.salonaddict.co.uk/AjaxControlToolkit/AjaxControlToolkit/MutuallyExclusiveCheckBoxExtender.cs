namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;

    [TargetControlType(typeof(ICheckBoxControl)), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.MutuallyExclusiveCheckBoxBehavior", "MutuallyExclusiveCheckBox.MutuallyExclusiveCheckBoxBehavior.js"), ToolboxBitmap(typeof(MutuallyExclusiveCheckBoxExtender), "MutuallyExclusiveCheckBox.MutuallyExclusiveCheckBox.ico"), Designer("AjaxControlToolkit.MutuallyExclusiveCheckBoxDesigner, AjaxControlToolkit")]
    public class MutuallyExclusiveCheckBoxExtender : ExtenderControlBase
    {
        [RequiredProperty, ExtenderControlProperty]
        public string Key
        {
            get => 
                base.GetPropertyValue<string>("Key", string.Empty);
            set
            {
                base.SetPropertyValue<string>("Key", value);
            }
        }
    }
}

