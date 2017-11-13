namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ClientScriptResource("Sys.Extended.UI.TextBoxWatermarkBehavior", "TextboxWatermark.TextboxWatermark.js"), RequiredScript(typeof(CommonToolkitScripts)), TargetControlType(typeof(TextBox)), Designer("AjaxControlToolkit.TextBoxWatermarkExtenderDesigner, AjaxControlToolkit"), ToolboxBitmap(typeof(TextBoxWatermarkExtender), "TextboxWatermark.TextboxWatermark.ico")]
    public class TextBoxWatermarkExtender : ExtenderControlBase
    {
        private const string stringWatermarkCssClass = "WatermarkCssClass";
        private const string stringWatermarkText = "WatermarkText";

        public TextBoxWatermarkExtender()
        {
            base.EnableClientState = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptManager.RegisterOnSubmitStatement(this, typeof(TextBoxWatermarkExtender), "TextBoxWatermarkExtenderOnSubmit", "null;");
            base.ClientState = (string.Compare(this.Page.Form.DefaultFocus, base.TargetControlID, StringComparison.OrdinalIgnoreCase) == 0) ? "Focused" : null;
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string WatermarkCssClass
        {
            get => 
                base.GetPropertyValue<string>("WatermarkCssClass", "");
            set
            {
                base.SetPropertyValue<string>("WatermarkCssClass", value);
            }
        }

        [ExtenderControlProperty, RequiredProperty, DefaultValue("")]
        public string WatermarkText
        {
            get => 
                base.GetPropertyValue<string>("WatermarkText", "");
            set
            {
                base.SetPropertyValue<string>("WatermarkText", value);
            }
        }
    }
}

