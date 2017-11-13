namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(ConfirmButtonExtender), "ConfirmButton.ConfirmButton.ico"), Designer("AjaxControlToolkit.ConfirmButtonDesigner, AjaxControlToolkit"), ClientScriptResource("Sys.Extended.UI.ConfirmButtonBehavior", "ConfirmButton.confirmButtonBehavior.js"), TargetControlType(typeof(IButtonControl))]
    public class ConfirmButtonExtender : ExtenderControlBase
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ScriptManager.RegisterOnSubmitStatement(this, typeof(ConfirmButtonExtender), "ConfirmButtonExtenderOnSubmit", "null;");
            this.RegisterDisplayModalPopup();
        }

        public void RegisterDisplayModalPopup()
        {
            if (!string.IsNullOrEmpty(this.DisplayModalPopupID))
            {
                ModalPopupExtender extender = base.FindControlHelper(this.DisplayModalPopupID) as ModalPopupExtender;
                if (extender == null)
                {
                    throw new ArgumentException("Unable to find specified ModalPopupExtender.");
                }
                if (extender.TargetControlID != base.TargetControlID)
                {
                    throw new ArgumentException("ConfirmButton and the ModalPopupExtender specified by its DisplayModalPopupID must specify the same TargetControlID.");
                }
                if (string.IsNullOrEmpty(extender.OkControlID) && string.IsNullOrEmpty(extender.CancelControlID))
                {
                    throw new ArgumentException("Specified ModalPopupExtender must set at least OkControlID and/or CancelControlID.");
                }
                if (!string.IsNullOrEmpty(extender.OnOkScript) || !string.IsNullOrEmpty(extender.OnCancelScript))
                {
                    throw new ArgumentException("Specified ModalPopupExtender may not set OnOkScript or OnCancelScript.");
                }
                Button child = new Button {
                    ID = this.ID + "_CBE_MPE_Placeholder",
                    Style = { [HtmlTextWriterStyle.Display] = "none" }
                };
                this.Controls.Add(child);
                extender.TargetControlID = child.ID;
                this.PostBackScript = this.Page.ClientScript.GetPostBackEventReference(base.TargetControl, "");
            }
        }

        [DefaultValue(false), ExtenderControlProperty]
        public bool ConfirmOnFormSubmit
        {
            get => 
                base.GetPropertyValue<bool>("ConfirmOnFormSubmit", false);
            set
            {
                base.SetPropertyValue<bool>("ConfirmOnFormSubmit", value);
            }
        }

        [RequiredProperty, ExtenderControlProperty]
        public string ConfirmText
        {
            get => 
                base.GetPropertyValue<string>("ConfirmText", "");
            set
            {
                base.SetPropertyValue<string>("ConfirmText", value);
            }
        }

        [ExtenderControlProperty, IDReferenceProperty(typeof(ModalPopupExtender)), ClientPropertyName("displayModalPopupID"), DefaultValue("")]
        public string DisplayModalPopupID
        {
            get => 
                base.GetPropertyValue<string>("DisplayModalPopupID", string.Empty);
            set
            {
                base.SetPropertyValue<string>("DisplayModalPopupID", value);
            }
        }

        [ExtenderControlProperty, DefaultValue("")]
        public string OnClientCancel
        {
            get => 
                base.GetPropertyValue<string>("OnClientCancel", string.Empty);
            set
            {
                base.SetPropertyValue<string>("OnClientCancel", value);
            }
        }

        [ExtenderControlProperty, EditorBrowsable(EditorBrowsableState.Never), ClientPropertyName("postBackScript"), DefaultValue(""), Browsable(false)]
        public string PostBackScript
        {
            get => 
                base.GetPropertyValue<string>("PostBackScript", string.Empty);
            set
            {
                base.SetPropertyValue<string>("PostBackScript", value);
            }
        }
    }
}

