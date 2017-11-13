namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(CommonToolkitScripts)), ToolboxItem(false), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.OkCancelAttachedTemplatePopup", "HTMLEditor.Popups.OkCancelAttachedTemplatePopup.js")]
    public class OkCancelAttachedTemplatePopup : AttachedTemplatePopup
    {
        protected override void CreateChildControls()
        {
            PopupBGIButton child = new PopupBGIButton {
                Text = base.GetButton("OK"),
                Name = "OK"
            };
            child.CssClass = child.CssClass + " ajax__htmleditor_popup_confirmbutton ";
            PopupBGIButton button2 = new PopupBGIButton {
                Text = base.GetButton("Cancel"),
                Name = "Cancel"
            };
            button2.CssClass = button2.CssClass + " ajax__htmleditor_popup_confirmbutton";
            Table item = new Table();
            item.Attributes.Add("border", "0");
            item.Attributes.Add("cellspacing", "0");
            item.Attributes.Add("cellpadding", "0");
            item.Style["width"] = "100%";
            TableRow row = new TableRow();
            item.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.HorizontalAlign = HorizontalAlign.Right;
            cell.Controls.Add(child);
            cell.Controls.Add(button2);
            base.Content.Add(item);
            base.RegisteredHandlers.Add(new RegisteredField("OK", child));
            base.RegisteredHandlers.Add(new RegisteredField("Cancel", button2));
            base.CreateChildControls();
        }
    }
}

