namespace AjaxControlToolkit
{
    using System;
    using System.Web.UI;

    public static class Utility
    {
        internal const string ToolBoxItemTypeName = "System.Web.UI.Design.WebControlToolboxItem, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

        public static void SetFocusOnLoad(Control control)
        {
            if (control == null)
            {
                throw new ArgumentNullException("control", "Control cannot be null!");
            }
            string script = "(function() { var fn = function() { var control = $get('" + control.ClientID + "'); if (control && control.focus) { control.focus(); } Sys.Application.remove_load(fn);};Sys.Application.add_load(fn);})();";
            ScriptManager.RegisterStartupScript(control.Page, control.GetType(), control.ClientID + "_SetFocusOnLoad", script, true);
        }
    }
}

