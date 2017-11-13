namespace AjaxControlToolkit.HTMLEditor
{
    using AjaxControlToolkit;
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ClientScriptResource("Sys.Extended.UI.HTMLEditor.Editor", "HTMLEditor.ModePanel.js")]
    public abstract class ModePanel : ScriptControlBase
    {
        private EditPanel _editPanel;

        protected ModePanel(HtmlTextWriterTag tag) : base(false, tag)
        {
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            if (this._editPanel != null)
            {
                descriptor.AddComponentProperty("editPanel", this._editPanel.ClientID);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.Style.Add(HtmlTextWriterStyle.Height, Unit.Percentage(100.0).ToString());
            base.Style.Add(HtmlTextWriterStyle.Width, Unit.Percentage(100.0).ToString());
            base.Style.Add(HtmlTextWriterStyle.Display, "none");
        }

        internal void setEditPanel(EditPanel editPanel)
        {
            this._editPanel = editPanel;
        }
    }
}

