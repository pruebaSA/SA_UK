namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Web.Resources.Design;
    using System.Web.UI.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    public class DataPagerDesigner : ControlDesigner
    {
        private DataPagerActionList _actionLists;
        private static string _templateFieldTemplateName = "PagerTemplate";

        internal void EditFields()
        {
            Cursor current = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ControlDesigner.InvokeTransactedChange(base.Component, new TransactedChangeCallback(this.EditFieldsChangeCallback), null, AtlasWebDesign.DataPager_EditFieldsTransaction);
                this.UpdateDesignTimeHtml();
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        private bool EditFieldsChangeCallback(object context)
        {
            DataPagerFieldsEditor form = new DataPagerFieldsEditor(this);
            return (UIServiceHelper.ShowDialog(base.Component.Site, form) == DialogResult.OK);
        }

        internal DesignerPagerStyle GetDataPagerStyle()
        {
            NullPagerStyle style = new NullPagerStyle(base.Component.Site);
            if (style.IsPagerType((DataPager) base.Component))
            {
                return style;
            }
            NumericPagerStyle style2 = new NumericPagerStyle(base.Component.Site);
            if (style2.IsPagerType((DataPager) base.Component))
            {
                return style2;
            }
            NextPreviousPagerStyle style3 = new NextPreviousPagerStyle(base.Component.Site);
            if (style3.IsPagerType((DataPager) base.Component))
            {
                return style3;
            }
            return new CustomPagerStyle(base.Component.Site);
        }

        public override string GetDesignTimeHtml()
        {
            DataPager viewControl = (DataPager) base.ViewControl;
            bool flag = false;
            foreach (DataPagerField field in viewControl.Fields)
            {
                if (!(field is TemplatePagerField))
                {
                    flag = true;
                    break;
                }
            }
            if (flag)
            {
                TypeDescriptor.Refresh(base.Component);
                return base.GetDesignTimeHtml();
            }
            return this.GetEmptyDesignTimeHtml();
        }

        protected override string GetEmptyDesignTimeHtml() => 
            base.CreatePlaceHolderDesignTimeHtml(AtlasWebDesign.DataPager_NoFieldsDefined);

        public override void Initialize(IComponent component)
        {
            ControlDesigner.VerifyInitializeArgument(component, typeof(DataPager));
            base.Initialize(component);
            base.SetViewFlags(ViewFlags.TemplateEditing, true);
            base.SetViewFlags(ViewFlags.DesignTimeHtmlRequiresLoadComplete, true);
            if (base.RootDesigner != null)
            {
                if (base.RootDesigner.IsLoading)
                {
                    base.RootDesigner.LoadComplete += new EventHandler(this.OnDesignerLoadComplete);
                }
                else
                {
                    this.OnDesignerLoadComplete(null, EventArgs.Empty);
                }
            }
        }

        private void OnDesignerLoadComplete(object sender, EventArgs e)
        {
            this.UpdateDesignTimeHtml();
        }

        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);
            PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor) properties["PagedControlID"];
            oldPropertyDescriptor = TypeDescriptor.CreateProperty(base.GetType(), oldPropertyDescriptor, new Attribute[] { new TypeConverterAttribute(typeof(PagedControlIDConverter)) });
            properties["PagedControlID"] = oldPropertyDescriptor;
            if (base.InTemplateMode)
            {
                PropertyDescriptor descriptor2 = (PropertyDescriptor) properties["Fields"];
                properties["Fields"] = TypeDescriptor.CreateProperty(descriptor2.ComponentType, descriptor2, new Attribute[] { BrowsableAttribute.No });
            }
        }

        internal void SetDataPagerStyle(DesignerPagerStyle pagerStyle)
        {
            Cursor current = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ControlDesigner.InvokeTransactedChange(base.Component, new TransactedChangeCallback(this.SetDataPagerStyleCallback), pagerStyle, AtlasWebDesign.DataPager_SetDataPagerStyleTransaction);
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        private bool SetDataPagerStyleCallback(object context)
        {
            DesignerPagerStyle style = context as DesignerPagerStyle;
            if (style == null)
            {
                return false;
            }
            if ((this.GetDataPagerStyle().GetType() == typeof(CustomPagerStyle)) && (DialogResult.No == UIServiceHelper.ShowMessage(base.Component.Site, AtlasWebDesign.DataPager_RegenerateDataPager, AtlasWebDesign.DataPager_ResetCaption, MessageBoxButtons.YesNo)))
            {
                return false;
            }
            style.ApplyStyle((DataPager) base.Component);
            return true;
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                if (this._actionLists == null)
                {
                    this._actionLists = new DataPagerActionList(this);
                }
                lists.Add(this._actionLists);
                return lists;
            }
        }

        public string PagedControlID
        {
            get => 
                ((DataPager) base.Component).PagedControlID;
            set
            {
                if (value != this.PagedControlID)
                {
                    TypeDescriptor.Refresh(base.Component);
                    TypeDescriptor.GetProperties(typeof(DataPager))["PagedControlID"].SetValue(base.Component, value);
                }
            }
        }

        public override TemplateGroupCollection TemplateGroups
        {
            get
            {
                TemplateGroupCollection templateGroups = base.TemplateGroups;
                DataPagerFieldCollection fields = ((DataPager) base.Component).Fields;
                int count = fields.Count;
                if (count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (fields[i] is TemplatePagerField)
                        {
                            string groupName = string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.DataPager_Field, new object[] { i.ToString(NumberFormatInfo.InvariantInfo), _templateFieldTemplateName });
                            TemplateGroup group = new TemplateGroup(groupName);
                            TemplateDefinition templateDefinition = new TemplateDefinition(this, groupName, fields[i], _templateFieldTemplateName) {
                                SupportsDataBinding = true
                            };
                            group.AddTemplateDefinition(templateDefinition);
                            templateGroups.Add(group);
                        }
                    }
                }
                return templateGroups;
            }
        }

        protected override bool UsePreviewControl =>
            true;
    }
}

