namespace AjaxControlToolkit.HTMLEditor.ToolbarButton
{
    using AjaxControlToolkit;
    using System;
    using System.ComponentModel;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [PersistChildren(false), ClientScriptResource("Sys.Extended.UI.HTMLEditor.ToolbarButton.FixedColorButton", "HTMLEditor.Toolbar_buttons.FixedColorButton.js"), ParseChildren(true), RequiredScript(typeof(CommonToolkitScripts))]
    public abstract class FixedColorButton : DesignModeBoxButton
    {
        private DesignModeBoxButton _colorDiv;
        private string _defaultColor = "#000000";
        private AjaxControlToolkit.HTMLEditor.ToolbarButton.MethodButton _methodButton;

        protected FixedColorButton()
        {
        }

        protected override void CreateChildControls()
        {
            Table item = new Table();
            item.Attributes.Add("border", "0");
            item.Attributes.Add("cellspacing", "0");
            item.Attributes.Add("cellpadding", "0");
            item.Style[HtmlTextWriterStyle.Margin] = "1px";
            item.Style[HtmlTextWriterStyle.Padding] = "0px";
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            item.Rows.Add(row);
            row.Cells.Add(cell);
            if (this.MethodButton != null)
            {
                cell.Controls.Add(this.MethodButton);
            }
            row = new TableRow();
            cell = new TableCell();
            item.Rows.Add(row);
            row.Cells.Add(cell);
            this.ColorDiv = new DesignModeBoxButton();
            this.ColorDiv.CssClass = "";
            this.ColorDiv.Style[HtmlTextWriterStyle.Margin] = "0px";
            this.ColorDiv.Style[HtmlTextWriterStyle.Padding] = "0px";
            this.ColorDiv.Width = new Unit(21.0, UnitType.Pixel);
            this.ColorDiv.Height = new Unit(5.0, UnitType.Pixel);
            this.ColorDiv.Style["background-color"] = this.DefaultColor;
            this.ColorDiv.Style["font-size"] = "1px";
            cell.Controls.Add(this.ColorDiv);
            base.Content.Add(item);
            base.CreateChildControls();
        }

        internal override void CreateChilds(DesignerWithMapPath designer)
        {
            if (this.MethodButton != null)
            {
                this.MethodButton._designer = designer;
            }
            base.Content.Clear();
            base.CreateChilds(designer);
        }

        protected override void DescribeComponent(ScriptComponentDescriptor descriptor)
        {
            base.DescribeComponent(descriptor);
            descriptor.AddComponentProperty("colorDiv", this.ColorDiv.ClientID);
            if (this.MethodButton != null)
            {
                descriptor.AddComponentProperty("methodButton", this.MethodButton.ClientID);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            this.ColorDiv.ToolTip = this.ToolTip;
            if (this.MethodButton != null)
            {
                this.MethodButton.ToolTip = this.ToolTip;
            }
            base.OnPreRender(e);
        }

        protected DesignModeBoxButton ColorDiv
        {
            get => 
                this._colorDiv;
            set
            {
                this._colorDiv = value;
            }
        }

        [DefaultValue("#000000"), ExtenderControlProperty, ClientPropertyName("defaultColor"), Category("Behavior")]
        public string DefaultColor
        {
            get => 
                this._defaultColor;
            set
            {
                this._defaultColor = value;
            }
        }

        protected AjaxControlToolkit.HTMLEditor.ToolbarButton.MethodButton MethodButton
        {
            get => 
                this._methodButton;
            set
            {
                this._methodButton = value;
            }
        }
    }
}

