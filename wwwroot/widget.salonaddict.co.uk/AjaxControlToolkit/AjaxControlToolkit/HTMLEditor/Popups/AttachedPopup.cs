namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.AttachedPopup", "HTMLEditor.Popups.AttachedPopup.js"), ParseChildren(true), ToolboxItem(false)]
    public class AttachedPopup : Popup
    {
    }
}

