namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.NoBotBehavior", "NoBot.NoBotBehavior.js"), TargetControlType(typeof(Label))]
    public class NoBotExtender : ExtenderControlBase
    {
        public NoBotExtender()
        {
            base.EnableClientState = true;
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string ChallengeScript
        {
            get => 
                base.GetPropertyValue<string>("ChallengeScript", "");
            set
            {
                base.SetPropertyValue<string>("ChallengeScript", value);
            }
        }
    }
}

