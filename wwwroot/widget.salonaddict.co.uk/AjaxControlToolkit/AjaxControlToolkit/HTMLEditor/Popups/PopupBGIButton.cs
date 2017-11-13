namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(CommonToolkitScripts)), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.PopupBGIButton", "HTMLEditor.Popups.PopupBGIButton.js"), ParseChildren(true), PersistChildren(false)]
    internal class PopupBGIButton : PopupBoxButton
    {
        private string _text;

        public PopupBGIButton() : base(HtmlTextWriterTag.Div)
        {
            this._text = "";
        }

        public PopupBGIButton(HtmlTextWriterTag tag) : base(tag)
        {
            this._text = "";
        }

        protected override void CreateChildControls()
        {
            HtmlGenericControl child = new HtmlGenericControl("span");
            Table item = new Table();
            item.Attributes.Add("border", "0");
            item.Attributes.Add("cellspacing", "0");
            item.Attributes.Add("cellpadding", "0");
            TableRow row = new TableRow();
            item.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.VerticalAlign = VerticalAlign.Middle;
            cell.HorizontalAlign = HorizontalAlign.Center;
            cell.CssClass = "ajax__htmleditor_popup_bgibutton";
            LiteralControl control2 = new LiteralControl(this.Text);
            child.Controls.Add(control2);
            cell.Controls.Add(child);
            base.Content.Add(item);
            base.CreateChildControls();
        }

        [DefaultValue(""), Category("Appearance")]
        public string Text
        {
            get => 
                this._text;
            set
            {
                this._text = value;
            }
        }
    }
}

