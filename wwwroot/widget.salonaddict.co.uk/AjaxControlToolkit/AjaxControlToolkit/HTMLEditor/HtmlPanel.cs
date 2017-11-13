namespace AjaxControlToolkit.HTMLEditor
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;

    [PersistChildren(false), RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), ClientCssResource("HTMLEditor.HtmlPanel.css"), ClientScriptResource("Sys.Extended.UI.HTMLEditor.HtmlPanel", "HTMLEditor.HtmlPanel.js")]
    internal class HtmlPanel : ModePanel
    {
        public HtmlPanel() : base(HtmlTextWriterTag.Textarea)
        {
        }
    }
}

