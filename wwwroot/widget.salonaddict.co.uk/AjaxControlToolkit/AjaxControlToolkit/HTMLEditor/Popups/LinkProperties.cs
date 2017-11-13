namespace AjaxControlToolkit.HTMLEditor.Popups
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [RequiredScript(typeof(CommonToolkitScripts)), ParseChildren(true), ClientScriptResource("Sys.Extended.UI.HTMLEditor.Popups.LinkProperties", "HTMLEditor.Popups.LinkProperties.js")]
    internal class LinkProperties : OkCancelAttachedTemplatePopup
    {
        private string _defaultTarget = "_self";
        private HtmlSelect _target = new HtmlSelect();
        private TextBox _url = new TextBox();

        protected override void CreateChildControls()
        {
            HtmlGenericControl child = new HtmlGenericControl("span");
            HtmlGenericControl control2 = new HtmlGenericControl("span");
            Table item = new Table();
            item.Attributes.Add("border", "0");
            item.Attributes.Add("cellspacing", "0");
            item.Attributes.Add("cellpadding", "2");
            TableRow row = new TableRow();
            item.Rows.Add(row);
            TableCell cell = new TableCell();
            row.Cells.Add(cell);
            cell.HorizontalAlign = HorizontalAlign.Left;
            cell.Controls.Add(child);
            child.Controls.Add(new LiteralControl(base.GetField("URL")));
            cell.Controls.Add(new LiteralControl(":"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.HorizontalAlign = HorizontalAlign.Left;
            this._url.Style["width"] = "200px";
            this._url.MaxLength = 0xff;
            cell.Controls.Add(this._url);
            row = new TableRow();
            item.Rows.Add(row);
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.HorizontalAlign = HorizontalAlign.Left;
            cell.Controls.Add(control2);
            control2.Controls.Add(new LiteralControl(base.GetField("Target")));
            cell.Controls.Add(new LiteralControl(":"));
            cell = new TableCell();
            row.Cells.Add(cell);
            cell.HorizontalAlign = HorizontalAlign.Left;
            this._target.Style["width"] = "105px";
            this._target.Items.Add(new ListItem(base.GetField("Target", "New"), "_blank"));
            this._target.Items.Add(new ListItem(base.GetField("Target", "Current"), "_self"));
            this._target.Items.Add(new ListItem(base.GetField("Target", "Parent"), "_parent"));
            this._target.Items.Add(new ListItem(base.GetField("Target", "Top"), "_top"));
            cell.Controls.Add(this._target);
            base.Content.Add(item);
            base.RegisteredFields.Add(new RegisteredField("url", this._url));
            base.RegisteredFields.Add(new RegisteredField("target", this._target));
            base.CreateChildControls();
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddProperty("defaultTarget", this.DefaultTarget);
        }

        protected override void OnPreRender(EventArgs e)
        {
            this._url.Attributes.Add("id", this._url.ClientID);
            this._target.Attributes.Add("id", this._target.ClientID);
            base.OnPreRender(e);
        }

        [Category("Behavior"), DefaultValue("_self")]
        public string DefaultTarget
        {
            get => 
                this._defaultTarget;
            set
            {
                this._defaultTarget = value;
            }
        }
    }
}

