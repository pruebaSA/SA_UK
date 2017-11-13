namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using AjaxControlToolkit.HTMLEditor.Popups;
    using System;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.ColorButton", "HTMLEditor.Toolbar_buttons.ColorButton.js"), ParseChildren(true)]
    public abstract class ColorButton : DesignModePopupImageButton
    {
        protected ColorButton()
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.RelatedPopup = new BaseColorsPopup();
        }
    }
}

