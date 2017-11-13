namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Text;
    using System.Web.Resources.Design;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    internal class ListViewGridLayout : ListViewAutoLayout
    {
        private Collection<ListViewAutoStyle> _styles;

        public ListViewGridLayout(IDesignerHost designerHost, IDataSourceFieldSchema[] fieldSchema) : base(designerHost, fieldSchema)
        {
        }

        private void AddHeader(HtmlTable table, string rowStyle, bool hasButtons)
        {
            HtmlTableRow row = new HtmlTableRow();
            row.Attributes.Add("style", rowStyle);
            if (hasButtons)
            {
                HtmlTableCell cell = new HtmlTableCell("th");
                row.Cells.Add(cell);
            }
            IDataSourceFieldSchema[] fieldSchema = base.FieldSchema;
            if ((fieldSchema != null) && (fieldSchema.Length > 0))
            {
                foreach (IDataSourceFieldSchema schema in fieldSchema)
                {
                    string name = schema.Name;
                    HtmlTableCell cell2 = new HtmlTableCell("th");
                    LiteralControl child = new LiteralControl {
                        Text = name
                    };
                    cell2.Controls.Add(child);
                    row.Cells.Add(cell2);
                }
            }
            table.Rows.Add(row);
        }

        private ITemplate BuildItemTemplate(string rowStyle, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<tr style=\"");
            builder.Append(rowStyle);
            builder.Append("\" >");
            builder.Append(Environment.NewLine);
            if (hasButtons)
            {
                builder.Append("<td>");
                if (enableDeleting)
                {
                    builder.Append(base.GetButtonString("Delete", AtlasWebDesign.ListViewConfigForm_Delete));
                    builder.Append(Environment.NewLine);
                }
                if (enableEditing)
                {
                    builder.Append(base.GetButtonString("Edit", AtlasWebDesign.ListViewConfigForm_Edit));
                    builder.Append(Environment.NewLine);
                }
                builder.Append("</td>");
            }
            IDataSourceFieldSchema[] fieldSchema = base.FieldSchema;
            if ((fieldSchema != null) && (fieldSchema.Length > 0))
            {
                foreach (IDataSourceFieldSchema schema in fieldSchema)
                {
                    string name = schema.Name;
                    builder.Append("<td>");
                    if (isDynamic)
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:DynamicControl DataField=\"{0}\" Mode=\"{1}\" runat=\"server\" />", new object[] { name, "ReadOnly" }));
                    }
                    else if (schema.PrimaryKey || schema.Identity)
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else if (schema.DataType == typeof(bool))
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Enabled=\"false\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    builder.Append("</td>");
                    builder.Append(Environment.NewLine);
                }
            }
            builder.Append("</tr>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateAlternatingItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "alternatingItemRowStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        protected override ITemplate CreateEditItemTemplate(ListViewAutoStyle style, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<tr style=\"");
            builder.Append(style.GetStringValue(this, "editItemRowStyle"));
            builder.Append("\" >");
            builder.Append(Environment.NewLine);
            builder.Append("<td>");
            builder.Append(Environment.NewLine);
            builder.Append(base.GetButtonString("Update", AtlasWebDesign.ListViewConfigForm_Update));
            builder.Append(Environment.NewLine);
            builder.Append(base.GetButtonString("Cancel", AtlasWebDesign.ListViewConfigForm_Cancel));
            builder.Append(Environment.NewLine);
            builder.Append("</td>");
            builder.Append(Environment.NewLine);
            IDataSourceFieldSchema[] fieldSchema = base.FieldSchema;
            if ((fieldSchema != null) && (fieldSchema.Length > 0))
            {
                foreach (IDataSourceFieldSchema schema in fieldSchema)
                {
                    string name = schema.Name;
                    builder.Append("<td>");
                    if (isDynamic)
                    {
                        string str2 = (schema.PrimaryKey || schema.Identity) ? "ReadOnly" : "Edit";
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:DynamicControl DataField=\"{0}\" Mode=\"{1}\" runat=\"server\" />", new object[] { name, str2 }));
                    }
                    else if (schema.PrimaryKey || schema.Identity)
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label1\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else if (schema.DataType == typeof(bool))
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    builder.Append("</td>");
                    builder.Append(Environment.NewLine);
                }
            }
            builder.Append("</tr>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateEmptyDataTemplate(ListViewAutoStyle style)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format(CultureInfo.InvariantCulture, "<table runat=\"server\" style=\"{0}\">", new object[] { style.GetStringValue(this, "emptyDataTableStyle") }));
            builder.Append(Environment.NewLine);
            builder.Append("<tr>");
            builder.Append(Environment.NewLine);
            builder.Append(string.Format(CultureInfo.InvariantCulture, "<td>{0}</td>", new object[] { AtlasWebDesign.ListViewLayout_NoData }));
            builder.Append(Environment.NewLine);
            builder.Append("</tr>");
            builder.Append(Environment.NewLine);
            builder.Append("</table>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateInsertItemTemplate(ListViewAutoStyle style, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<tr style=\"");
            builder.Append(style.GetStringValue(this, "insertItemRowStyle"));
            builder.Append("\" >");
            builder.Append(Environment.NewLine);
            builder.Append("<td>");
            builder.Append(Environment.NewLine);
            if (isDynamic)
            {
                builder.Append(base.GetInsertButtonWithValidationGroupString(AtlasWebDesign.ListViewConfigForm_Insert));
            }
            else
            {
                builder.Append(base.GetButtonString("Insert", AtlasWebDesign.ListViewConfigForm_Insert));
            }
            builder.Append(Environment.NewLine);
            builder.Append(base.GetButtonString("Cancel", AtlasWebDesign.ListViewConfigForm_Clear));
            builder.Append(Environment.NewLine);
            builder.Append("</td>");
            builder.Append(Environment.NewLine);
            IDataSourceFieldSchema[] fieldSchema = base.FieldSchema;
            if ((fieldSchema != null) && (fieldSchema.Length > 0))
            {
                foreach (IDataSourceFieldSchema schema in fieldSchema)
                {
                    string name = schema.Name;
                    builder.Append("<td>");
                    if (isDynamic)
                    {
                        if (schema.Identity)
                        {
                            builder.Append("&nbsp;");
                        }
                        else
                        {
                            builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:DynamicControl DataField=\"{0}\" Mode=\"Insert\" ValidationGroup=\"Insert\" runat=\"server\" />", new object[] { name }));
                        }
                    }
                    else if (schema.Identity)
                    {
                        builder.Append("&nbsp;");
                    }
                    else if (schema.PrimaryKey)
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else if (schema.DataType == typeof(bool))
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    builder.Append("</td>");
                    builder.Append(Environment.NewLine);
                }
            }
            builder.Append("</tr>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "itemRowStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        protected override ITemplate CreateLayoutTemplate(ListViewAutoStyle style, DesignerPagerStyle pagerStyle, string itemPlaceholderID, string groupPlaceholderID, bool hasButtons)
        {
            string templateText = string.Empty;
            HtmlTable control = new HtmlTable {
                Rows = { 
                    new HtmlTableRow(),
                    Item = { Cells = { new HtmlTableCell() } }
                }
            };
            HtmlTable table = new HtmlTable();
            table.Attributes.Add("style", style.GetStringValue(this, "tableStyle"));
            table.Border = style.GetIntValue(this, "tableBorder");
            table.ID = "itemPlaceholderContainer";
            this.AddHeader(table, style.GetStringValue(this, "itemRowStyle"), hasButtons);
            HtmlTableRow row = new HtmlTableRow {
                ID = itemPlaceholderID
            };
            table.Rows.Add(row);
            control.Rows[0].Cells[0].Controls.Add(table);
            control.Rows.Add(new HtmlTableRow());
            HtmlTableCell cell = new HtmlTableCell();
            cell.Attributes.Add("style", style.GetStringValue(this, "pagerCellStyle"));
            control.Rows[1].Cells.Add(cell);
            DataPager child = pagerStyle.CreatePager();
            if (child != null)
            {
                cell.Controls.Add(child);
            }
            templateText = ControlPersister.PersistControl(control, base.DesignerHost);
            return ControlParser.ParseTemplate(base.DesignerHost, templateText);
        }

        protected override ITemplate CreateSelectedItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "selectedItemRowStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        public override string ID =>
            "grid";

        public override string LayoutName =>
            AtlasWebDesign.ListViewLayout_Grid;

        public override Collection<ListViewAutoStyle> Styles
        {
            get
            {
                if (this._styles == null)
                {
                    this._styles = new Collection<ListViewAutoStyle>();
                    this._styles.Add(new ListViewNoFormatStyle());
                    this._styles.Add(new ListViewColorfulStyle());
                    this._styles.Add(new ListViewProfessionalStyle());
                    this._styles.Add(new ListViewBlueStyle());
                }
                return this._styles;
            }
        }
    }
}

