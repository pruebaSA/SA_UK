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

    internal class ListViewSingleRowLayout : ListViewAutoLayout
    {
        private Collection<ListViewAutoStyle> _styles;

        public ListViewSingleRowLayout(IDesignerHost designerHost, IDataSourceFieldSchema[] fieldSchema) : base(designerHost, fieldSchema)
        {
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
                    builder.Append(Environment.NewLine);
                }
            }
            if (enableEditing)
            {
                builder.Append(base.GetButtonString("Edit", AtlasWebDesign.ListViewConfigForm_Edit));
                builder.Append(Environment.NewLine);
            }
            if (enableDeleting)
            {
                builder.Append(base.GetButtonString("Delete", AtlasWebDesign.ListViewConfigForm_Delete));
                builder.Append(Environment.NewLine);
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
                    builder.Append(Environment.NewLine);
                }
            }
            builder.Append(base.GetButtonString("Update", AtlasWebDesign.ListViewConfigForm_Update));
            builder.Append(base.GetButtonString("Cancel", AtlasWebDesign.ListViewConfigForm_Cancel));
            builder.Append("</td>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateEmptyDataTemplate(ListViewAutoStyle style) => 
            ControlParser.ParseTemplate(base.DesignerHost, string.Format(CultureInfo.InvariantCulture, "<table style=\"{0}\"><tr><td>{1}</td></tr></table>", new object[] { style.GetStringValue(this, "emptyDataTableStyle"), AtlasWebDesign.ListViewLayout_NoData }));

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
            builder.Append(base.GetButtonString("Cancel", AtlasWebDesign.ListViewConfigForm_Clear));
            builder.Append("</td>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "itemCellStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        protected override ITemplate CreateLayoutTemplate(ListViewAutoStyle style, DesignerPagerStyle pagerStyle, string itemPlaceholderID, string groupPlaceholderID, bool hasButtons)
        {
            StringBuilder builder = new StringBuilder();
            HtmlTable control = new HtmlTable();
            control.Attributes.Add("style", style.GetStringValue(this, "tableStyle"));
            control.Border = style.GetIntValue(this, "tableBorder");
            HtmlTableRow row = new HtmlTableRow {
                ID = "itemPlaceholderContainer"
            };
            control.Rows.Add(row);
            HtmlTableCell cell = new HtmlTableCell {
                ID = itemPlaceholderID
            };
            row.Cells.Add(cell);
            builder.Append(ControlPersister.PersistControl(control, base.DesignerHost));
            builder.Append(Environment.NewLine);
            builder.Append("<div style=\"");
            builder.Append(style.GetStringValue(this, "pagerDivStyle"));
            builder.Append("\">");
            DataPager pager = pagerStyle.CreatePager();
            if (pager != null)
            {
                builder.Append(ControlPersister.PersistControl(pager, base.DesignerHost));
            }
            builder.Append(Environment.NewLine);
            builder.Append("</div>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateSelectedItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "selectedItemCellStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        public override string ID =>
            "singlerow";

        public override string LayoutName =>
            AtlasWebDesign.ListViewLayout_SingleRow;

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

