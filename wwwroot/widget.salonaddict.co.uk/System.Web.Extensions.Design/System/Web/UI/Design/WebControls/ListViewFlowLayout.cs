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
    using System.Web.UI.WebControls;

    internal class ListViewFlowLayout : ListViewAutoLayout
    {
        private Collection<ListViewAutoStyle> _styles;

        public ListViewFlowLayout(IDesignerHost designerHost, IDataSourceFieldSchema[] fieldSchema) : base(designerHost, fieldSchema)
        {
        }

        private ITemplate BuildItemTemplate(string spanStyle, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<span style=\"");
            builder.Append(spanStyle);
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
            if (enableEditing || enableDeleting)
            {
                builder.Append("<br />");
            }
            builder.Append("<br />");
            builder.Append("</span>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateAlternatingItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "alternatingItemSpanStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        protected override ITemplate CreateEditItemTemplate(ListViewAutoStyle style, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<span style=\"");
            builder.Append(style.GetStringValue(this, "editItemSpanStyle"));
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
            builder.Append("<br />");
            builder.Append("<br />");
            builder.Append("</span>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateEmptyDataTemplate(ListViewAutoStyle style) => 
            ControlParser.ParseTemplate(base.DesignerHost, string.Format(CultureInfo.InvariantCulture, "<span>{0}</span>", new object[] { AtlasWebDesign.ListViewLayout_NoData }));

        protected override ITemplate CreateInsertItemTemplate(ListViewAutoStyle style, bool isDynamic)
        {
            StringBuilder builder = new StringBuilder("<span style=\"");
            builder.Append(style.GetStringValue(this, "insertItemSpanStyle"));
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
            builder.Append("<br />");
            builder.Append("<br />");
            builder.Append("</span>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "itemSpanStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        protected override ITemplate CreateLayoutTemplate(ListViewAutoStyle style, DesignerPagerStyle pagerStyle, string itemPlaceholderID, string groupPlaceholderID, bool hasButtons)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<div runat=\"server\" id=\"itemPlaceholderContainer\" style=\"");
            builder.Append(style.GetStringValue(this, "divStyle"));
            builder.Append("\" >");
            builder.Append("<span runat=\"server\" id=\"" + itemPlaceholderID + "\" />");
            builder.Append("</div>");
            builder.Append(Environment.NewLine);
            builder.Append("<div style=\"");
            builder.Append(style.GetStringValue(this, "pagerDivStyle"));
            builder.Append("\">");
            DataPager control = pagerStyle.CreatePager();
            if (control != null)
            {
                builder.Append(ControlPersister.PersistControl(control, base.DesignerHost));
            }
            builder.Append(Environment.NewLine);
            builder.Append("</div>");
            return ControlParser.ParseTemplate(base.DesignerHost, builder.ToString());
        }

        protected override ITemplate CreateSelectedItemTemplate(ListViewAutoStyle style, bool enableDeleting, bool enableEditing, bool hasButtons, bool isDynamic) => 
            this.BuildItemTemplate(style.GetStringValue(this, "selectedItemSpanStyle"), enableDeleting, enableEditing, hasButtons, isDynamic);

        public override string ID =>
            "flow";

        public override string LayoutName =>
            AtlasWebDesign.ListViewLayout_Flow;

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

