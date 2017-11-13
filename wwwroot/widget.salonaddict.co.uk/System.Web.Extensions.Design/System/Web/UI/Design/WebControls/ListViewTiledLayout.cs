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

    internal class ListViewTiledLayout : ListViewAutoLayout
    {
        private Collection<ListViewAutoStyle> _styles;

        public ListViewTiledLayout(IDesignerHost designerHost, IDataSourceFieldSchema[] fieldSchema) : base(designerHost, fieldSchema)
        {
        }

        public override void ApplyLayout(ListView listView, string styleID, bool enableDeleting, bool enableEditing, bool enableInserting, DesignerPagerStyle pagerStyle, bool isDynamic)
        {
            base.ApplyLayout(listView, styleID, enableDeleting, enableEditing, enableInserting, pagerStyle, isDynamic);
            listView.GroupItemCount = 3;
        }

        private ITemplate BuildItemTemplate(string cellStyle, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<td runat=\"server\" style=\"");
            builder.Append(cellStyle);
            builder.Append("\" >");
            builder.Append(Environment.NewLine);
            IDataSourceFieldSchema[] fieldSchema = base.FieldSchema;
            if ((fieldSchema != null) && (fieldSchema.Length > 0))
            {
                foreach (IDataSourceFieldSchema schema in fieldSchema)
                {
                    string name = schema.Name;
                    if (isDynamic)
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:DynamicControl DataField=\"{0}\" Mode=\"{1}\" runat=\"server\" />", new object[] { name, "ReadOnly" }));
                    }
                    else if (schema.PrimaryKey || schema.Identity)
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else if (schema.DataType == typeof(bool))
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Enabled=\"false\" Text=\"{0}\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    builder.Append("<br />");
                }
            }
            if (enableDeleting)
            {
                builder.Append(base.GetButtonString("Delete", AtlasWebDesign.ListViewConfigForm_Delete));
                builder.Append("<br />");
            }
            if (enableEditing)
            {
                builder.Append(base.GetButtonString("Edit", AtlasWebDesign.ListViewConfigForm_Edit));
                builder.Append("<br />");
            }
            builder.Append("</td>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateAlternatingItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "alternatingItemCellStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        protected override ITemplate CreateEditItemTemplate(ListViewAutoStyle style, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<td runat=\"server\" style=\"");
            builder.Append(style.GetStringValue(this, "editItemCellStyle"));
            builder.Append("\" >");
            builder.Append(Environment.NewLine);
            IDataSourceFieldSchema[] fieldSchema = base.FieldSchema;
            if ((fieldSchema != null) && (fieldSchema.Length > 0))
            {
                foreach (IDataSourceFieldSchema schema in fieldSchema)
                {
                    string name = schema.Name;
                    if (isDynamic)
                    {
                        string str2 = (schema.PrimaryKey || schema.Identity) ? "ReadOnly" : "Edit";
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:DynamicControl DataField=\"{0}\" Mode=\"{1}\" runat=\"server\" />", new object[] { name, str2 }));
                    }
                    else if (schema.PrimaryKey || schema.Identity)
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:Label Text='<%# {1} %>' runat=\"server\" id=\"{2}Label1\" />", new object[] { name, DesignTimeDataBinding.CreateEvalExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else if (schema.DataType == typeof(bool))
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Text=\"{0}\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    else
                    {
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                    }
                    builder.Append("<br />");
                }
            }
            builder.Append(base.GetButtonString("Update", AtlasWebDesign.ListViewConfigForm_Update));
            builder.Append("<br />");
            builder.Append(base.GetButtonString("Cancel", AtlasWebDesign.ListViewConfigForm_Cancel));
            builder.Append("<br />");
            builder.Append("</td>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateEmptyDataTemplate(ListViewAutoStyle style)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(string.Format(CultureInfo.InvariantCulture, "<table runat=\"server\" style=\"{0}\">", new object[] { style.GetStringValue(this, "emptyTableStyle") }));
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

        protected override ITemplate CreateEmptyItemTemplate(ListViewAutoStyle style) => 
            ControlParser.ParseTemplate(base.DesignerHost, "<td runat=\"server\" />");

        protected override ITemplate CreateGroupTemplate(ListViewAutoStyle style, string itemPlaceholderID)
        {
            string templateText = string.Empty;
            HtmlTableRow control = new HtmlTableRow {
                ID = "itemPlaceholderContainer"
            };
            HtmlTableCell cell = new HtmlTableCell {
                ID = itemPlaceholderID
            };
            control.Cells.Add(cell);
            templateText = templateText + ControlPersister.PersistControl(control, base.DesignerHost);
            return ControlParser.ParseTemplate(base.DesignerHost, templateText);
        }

        protected override ITemplate CreateInsertItemTemplate(ListViewAutoStyle style, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<td runat=\"server\" style=\"");
            builder.Append(style.GetStringValue(this, "insertItemCellStyle"));
            builder.Append("\" >");
            builder.Append(Environment.NewLine);
            IDataSourceFieldSchema[] fieldSchema = base.FieldSchema;
            if ((fieldSchema != null) && (fieldSchema.Length > 0))
            {
                foreach (IDataSourceFieldSchema schema in fieldSchema)
                {
                    string name = schema.Name;
                    if (isDynamic)
                    {
                        if (schema.Identity)
                        {
                            continue;
                        }
                        builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:DynamicControl DataField=\"{0}\" Mode=\"Insert\" ValidationGroup=\"Insert\" runat=\"server\" />", new object[] { name }));
                    }
                    else
                    {
                        if (schema.Identity)
                        {
                            continue;
                        }
                        if (schema.PrimaryKey)
                        {
                            builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                        }
                        else if (schema.DataType == typeof(bool))
                        {
                            builder.Append(string.Format(CultureInfo.InvariantCulture, "<asp:CheckBox Checked='<%# {1} %>' runat=\"server\" id=\"{2}CheckBox\" Text=\"{0}\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                        }
                        else
                        {
                            builder.Append(string.Format(CultureInfo.InvariantCulture, "{0}: <asp:TextBox Text='<%# {1} %>' runat=\"server\" id=\"{2}TextBox\" />", new object[] { name, DesignTimeDataBinding.CreateBindExpression(name, string.Empty), ListViewAutoLayout.GetFieldDerivedID(name) }));
                        }
                    }
                    builder.Append("<br />");
                }
            }
            if (isDynamic)
            {
                builder.Append(base.GetInsertButtonWithValidationGroupString(AtlasWebDesign.ListViewConfigForm_Insert));
            }
            else
            {
                builder.Append(base.GetButtonString("Insert", AtlasWebDesign.ListViewConfigForm_Insert));
            }
            builder.Append("<br />");
            builder.Append(base.GetButtonString("Cancel", AtlasWebDesign.ListViewConfigForm_Clear));
            builder.Append("<br />");
            builder.Append("</td>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "itemCellStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        protected override ITemplate CreateLayoutTemplate(ListViewAutoStyle style, DesignerPagerStyle pagerStyle, string itemPlaceholderID, string groupPlaceholderID, bool hasButtons)
        {
            string templateText = string.Empty;
            HtmlTable control = new HtmlTable {
                Rows = { 
                    new HtmlTableRow(),
                    Item = { Cells = { new HtmlTableCell() } }
                }
            };
            HtmlTable child = new HtmlTable();
            child.Attributes.Add("style", style.GetStringValue(this, "tableStyle"));
            child.Border = style.GetIntValue(this, "tableBorder");
            child.ID = "groupPlaceholderContainer";
            HtmlTableRow row = new HtmlTableRow {
                ID = groupPlaceholderID
            };
            child.Rows.Add(row);
            control.Rows[0].Cells[0].Controls.Add(child);
            control.Rows.Add(new HtmlTableRow());
            HtmlTableCell cell = new HtmlTableCell();
            cell.Attributes.Add("style", style.GetStringValue(this, "pagerCellStyle"));
            control.Rows[1].Cells.Add(cell);
            DataPager pager = pagerStyle.CreatePager();
            if (pager != null)
            {
                pager.PageSize = 12;
                cell.Controls.Add(pager);
            }
            templateText = ControlPersister.PersistControl(control, base.DesignerHost);
            return ControlParser.ParseTemplate(base.DesignerHost, templateText);
        }

        protected override ITemplate CreateSelectedItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "selectedItemCellStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        public override string ID =>
            "tiled";

        public override string LayoutName =>
            AtlasWebDesign.ListViewLayout_Tiled;

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

