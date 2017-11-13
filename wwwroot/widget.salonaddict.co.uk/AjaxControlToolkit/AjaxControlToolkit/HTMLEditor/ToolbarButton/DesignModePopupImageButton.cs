namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor.Popups;
    using System;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.DesignModePopupImageButton", "HTMLEditor.Toolbar_buttons.DesignModePopupImageButton.js"), PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true)]
    public abstract class DesignModePopupImageButton : MethodButton
    {
        private bool _autoClose = true;
        private Popup _popup;

        protected DesignModePopupImageButton()
        {
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            if ((this.RelatedPopup != null) && !base.IsDesign)
            {
                descriptor.AddComponentProperty("relatedPopup", this.RelatedPopup.ClientID);
            }
            descriptor.AddProperty("autoClose", this.AutoClose);
        }

        protected bool AutoClose
        {
            get => 
                this._autoClose;
            set
            {
                this._autoClose = value;
            }
        }

        protected Popup RelatedPopup
        {
            get => 
                this._popup;
            set
            {
                this._popup = value;
                if (!base.IsDesign)
                {
                    Popup existingPopup = Popup.GetExistingPopup(this.Parent, this.RelatedPopup.GetType());
                    if (existingPopup == null)
                    {
                        base.ExportedControls.Add(this._popup);
                    }
                    else
                    {
                        this._popup = existingPopup;
                    }
                }
            }
        }
    }
}

