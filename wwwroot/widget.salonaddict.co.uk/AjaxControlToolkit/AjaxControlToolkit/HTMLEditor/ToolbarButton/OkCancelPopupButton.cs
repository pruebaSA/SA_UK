namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.OkCancelPopupButton", "HTMLEditor.Toolbar_buttons.OkCancelPopupButton.js"), RequiredScript(typeof(CommonToolkitScripts)), PersistChildren(false), ParseChildren(true)]
    public abstract class OkCancelPopupButton : DesignModePopupImageButton
    {
        protected OkCancelPopupButton()
        {
        }
    }
}

