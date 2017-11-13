namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;

    [RequiredScript(typeof(CommonToolkitScripts)), PersistChildren(false), ParseChildren(true), ToolboxItem(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.BulletedList", "HTMLEditor.Toolbar_buttons.BulletedList.js")]
    public class BulletedList : MethodButton
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.RegisterButtonImages("ed_list_bullet");
            base.OnPreRender(e);
        }
    }
}

