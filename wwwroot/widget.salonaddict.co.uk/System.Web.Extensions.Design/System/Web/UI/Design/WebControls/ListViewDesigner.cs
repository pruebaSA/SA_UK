namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Resources.Design;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.Design.Util;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Windows.Forms;

    public class ListViewDesigner : DataBoundControlDesigner
    {
        private ListViewActionList _actionLists;
        private const int _alternatingItemTemplateIndex = 1;
        private static string[] _controlTemplateNames = new string[] { AtlasWebDesign.ListView_RuntimeView, "AlternatingItemTemplate", "EditItemTemplate", "EmptyDataTemplate", "EmptyItemTemplate", "GroupSeparatorTemplate", "GroupTemplate", "InsertItemTemplate", "ItemSeparatorTemplate", "ItemTemplate", "LayoutTemplate", "SelectedItemTemplate" };
        private static bool[] _controlTemplateSupportsDataBinding = new bool[] { false, true, true, false, false, false, false, true, false, true, false, true };
        private const string _designTimeRegionHtml = "<table cellspacing=0 cellpadding=0 border=0 style=\"display:inline-block\">\r\n                <tr>\r\n                    <td nowrap align=center valign=middle style=\"color:{0}; background-color:{1}; \">{2} - {3}</td>\r\n                </tr>\r\n                <tr>\r\n                    <td style=\"vertical-align:top;\" {4}='0'></td>\r\n                </tr>\r\n          </table>";
        private const int _editItemTemplateIndex = 2;
        private const int _emptyDataTemplateIndex = 3;
        private const int _emptyItemTemplateIndex = 4;
        private const int _groupSeparatorTemplateIndex = 5;
        private bool _ignoreComponentChanged;
        private const int _insertItemTemplateIndex = 7;
        private const int _itemSeparatorTemplateIndex = 8;
        private const int _itemTemplateIndex = 9;
        private const int _runtimeViewIndex = 0;
        private const int _selectedItemTemplateIndex = 11;
        private TemplateData _templateData = new TemplateData();

        internal void Configure()
        {
            Cursor current = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ControlDesigner.InvokeTransactedChange(base.Component, new TransactedChangeCallback(this.ConfigureChangeCallback), null, AtlasWebDesign.ListView_ConfigureTransaction);
                this.UpdateDesignTimeHtml();
            }
            finally
            {
                Cursor.Current = current;
            }
        }

        private bool ConfigureChangeCallback(object context)
        {
            ListViewConfigurationForm form = new ListViewConfigurationForm(this);
            return (UIServiceHelper.ShowDialog(base.Component.Site, form) == DialogResult.OK);
        }

        private System.Web.UI.Control FindControlInternal(System.Web.UI.Control container, string ID)
        {
            foreach (System.Web.UI.Control control in container.Controls)
            {
                if (string.Equals(control.ID, ID))
                {
                    return control;
                }
                System.Web.UI.Control control2 = this.FindControlInternal(control, ID);
                if (control2 != null)
                {
                    return control2;
                }
            }
            return null;
        }

        private IDataSourceViewSchema GetDataSourceSchema()
        {
            DesignerDataSourceView designerView = base.DesignerView;
            if (designerView != null)
            {
                try
                {
                    return designerView.Schema;
                }
                catch (Exception exception)
                {
                    IComponentDesignerDebugService service = (IComponentDesignerDebugService) base.Component.Site.GetService(typeof(IComponentDesignerDebugService));
                    if (service != null)
                    {
                        service.Fail(string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.DataSource_DebugService_FailedCall, new object[] { "DesignerDataSourceView.Schema", exception.Message }));
                    }
                }
            }
            return null;
        }

        public override string GetDesignTimeHtml()
        {
            System.Web.UI.WebControls.ListView viewControl = (System.Web.UI.WebControls.ListView) base.ViewControl;
            bool flag = false;
            string[] dataKeyNames = null;
            if (this.EssentialTemplatesExist)
            {
                bool flag2 = false;
                IDataSourceViewSchema dataSourceSchema = this.GetDataSourceSchema();
                if (dataSourceSchema != null)
                {
                    IDataSourceFieldSchema[] fields = dataSourceSchema.GetFields();
                    if ((fields != null) && (fields.Length > 0))
                    {
                        flag2 = true;
                    }
                }
                try
                {
                    if (!flag2)
                    {
                        dataKeyNames = viewControl.DataKeyNames;
                        viewControl.DataKeyNames = new string[0];
                        flag = true;
                    }
                    TypeDescriptor.Refresh(base.Component);
                    return base.GetDesignTimeHtml();
                }
                finally
                {
                    if (flag)
                    {
                        viewControl.DataKeyNames = dataKeyNames;
                    }
                }
            }
            return this.GetEmptyDesignTimeHtml();
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            ListViewRegionContentType type;
            bool flag = base.UseRegions(regions, this.ListView.LayoutTemplate, ((System.Web.UI.WebControls.ListView) base.ViewControl).LayoutTemplate);
            int currentView = this.CurrentView;
            if ((!this.EssentialTemplatesExist || !flag) || (currentView == 0))
            {
                return this.GetDesignTimeHtml();
            }
            string str = _controlTemplateNames[currentView];
            if (!this.TemplateExists(currentView))
            {
                return ControlDesigner.CreateErrorDesignTimeHtml(string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_CurrentTemplateUndefined, new object[] { str }), null, base.Component);
            }
            ListViewEditableRegion region = new ListViewEditableRegion(currentView, this.CurrentTemplateDefinition);
            TemplateData templateData = new TemplateData();
            string instruction = this.GetPersistedListViewContent(currentView, templateData, out type);
            switch (type)
            {
                case ListViewRegionContentType.Error:
                    return ControlDesigner.CreateErrorDesignTimeHtml(string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_ErrorRenderingTemplate, new object[] { str, instruction }), null, base.Component);

                case ListViewRegionContentType.Message:
                    return base.CreatePlaceHolderDesignTimeHtml(instruction);
            }
            string str3 = this.SetListViewTemplates(instruction, currentView, templateData, true, out type);
            switch (type)
            {
                case ListViewRegionContentType.Error:
                    return ControlDesigner.CreateErrorDesignTimeHtml(string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_ErrorRenderingTemplate, new object[] { str, str3 }), null, base.Component);

                case ListViewRegionContentType.Message:
                    return base.CreatePlaceHolderDesignTimeHtml(instruction);
            }
            ((WebControl) base.ViewControl).Enabled = true;
            regions.Add(region);
            return string.Format(CultureInfo.InvariantCulture, "<table cellspacing=0 cellpadding=0 border=0 style=\"display:inline-block\">\r\n                <tr>\r\n                    <td nowrap align=center valign=middle style=\"color:{0}; background-color:{1}; \">{2} - {3}</td>\r\n                </tr>\r\n                <tr>\r\n                    <td style=\"vertical-align:top;\" {4}='0'></td>\r\n                </tr>\r\n          </table>", new object[] { ColorTranslator.ToHtml(SystemColors.ControlText), ColorTranslator.ToHtml(SystemColors.Control), this.ListView.ID, str, DesignerRegion.DesignerRegionAttributeName });
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            ListViewEditableRegion region2 = region as ListViewEditableRegion;
            if (region2 != null)
            {
                ListViewRegionContentType type;
                return this.GetPersistedListViewContent(region2.TemplateIndex, this._templateData, out type);
            }
            return base.GetEditableDesignerRegionContent(region);
        }

        protected override string GetEmptyDesignTimeHtml()
        {
            if (this.HasDataSourceWithoutSchema)
            {
                return base.CreatePlaceHolderDesignTimeHtml(string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoTemplatesInstNoSchemaHasDS, new object[] { this.ID }));
            }
            if (this.AllowConfiguration)
            {
                return base.CreatePlaceHolderDesignTimeHtml(string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoTemplatesInstHasSchema, new object[] { this.ID }));
            }
            return base.CreatePlaceHolderDesignTimeHtml(string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoTemplatesInstNoSchema, new object[] { this.ID }));
        }

        private string GetPersistedListViewContent(int view, TemplateData templateData, out ListViewRegionContentType contentType)
        {
            if (view == 3)
            {
                return this.GetPersistedListViewEmptyDataContent(out contentType);
            }
            return this.GetPersistedListViewLayoutContent(view, templateData, out contentType);
        }

        private string GetPersistedListViewEmptyDataContent(out ListViewRegionContentType contentType)
        {
            IDesignerHost service = (IDesignerHost) this.GetService(typeof(IDesignerHost));
            DesignerPanel container = new DesignerPanel {
                ID = "__dtpanel"
            };
            contentType = ListViewRegionContentType.Valid;
            if (this.ListView.EmptyDataTemplate != null)
            {
                ITemplate template = TrimTemplate(this.DesignerHost, this.ListView.EmptyDataTemplate);
                if (template != null)
                {
                    template.InstantiateIn(container);
                }
            }
            StringBuilder builder = new StringBuilder();
            foreach (System.Web.UI.Control control in container.Controls)
            {
                builder.Append(ControlPersister.PersistControl(control, service));
            }
            return builder.ToString();
        }

        private string GetPersistedListViewLayoutContent(int view, TemplateData templateData, out ListViewRegionContentType contentType)
        {
            templateData.ItemPlaceholderContainerUniqueID = null;
            templateData.ItemPlaceholder = null;
            templateData.IndexOfItemPlaceholderInParent = 0;
            templateData.ControlsAddedToItemPlaceholderContainer = 0;
            templateData.GroupPlaceholderContainerUniqueID = null;
            templateData.GroupPlaceholder = null;
            templateData.IndexOfGroupPlaceholderInParent = 0;
            templateData.ControlsAddedToGroupPlaceholderContainer = 0;
            IDesignerHost designerHost = this.DesignerHost;
            DesignerPanel container = new DesignerPanel {
                ID = "__dtpanel"
            };
            if ((this.ListView.LayoutTemplate != null) && (this.ListView.ItemTemplate != null))
            {
                ITemplate template = TrimTemplate(this.DesignerHost, this.ListView.LayoutTemplate);
                if (template == null)
                {
                    contentType = ListViewRegionContentType.Error;
                    return AtlasWebDesign.ListView_EmptyLayoutTemplate;
                }
                template.InstantiateIn(container);
                System.Web.UI.Control control = container;
                if (this.ListView.GroupTemplate != null)
                {
                    ITemplate groupSeparatorTemplate;
                    templateData.GroupPlaceholder = container.FindControl(this.ListView.GroupPlaceholderID);
                    if (templateData.GroupPlaceholder == null)
                    {
                        contentType = ListViewRegionContentType.Error;
                        return string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoGroupPlaceholder, new object[] { this.ID, this.ListView.GroupPlaceholderID });
                    }
                    System.Web.UI.Control parent = templateData.GroupPlaceholder.Parent;
                    if (string.IsNullOrEmpty(parent.ID))
                    {
                        contentType = ListViewRegionContentType.Error;
                        return string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoGroupPlaceholderContainerID, new object[] { this.ID, this.ListView.GroupPlaceholderID });
                    }
                    templateData.GroupPlaceholderContainerUniqueID = parent.UniqueID;
                    templateData.IndexOfGroupPlaceholderInParent = parent.Controls.IndexOf(templateData.GroupPlaceholder);
                    parent.Controls.Remove(templateData.GroupPlaceholder);
                    if (view == 5)
                    {
                        groupSeparatorTemplate = this.ListView.GroupSeparatorTemplate;
                    }
                    else
                    {
                        groupSeparatorTemplate = this.ListView.GroupTemplate;
                    }
                    templateData.ControlsAddedToGroupPlaceholderContainer = InstantiateTemplateInContainer(this.DesignerHost, groupSeparatorTemplate, parent, templateData.IndexOfGroupPlaceholderInParent, out contentType);
                    if (contentType != ListViewRegionContentType.Valid)
                    {
                        return AtlasWebDesign.ListView_MustEditInSourceView;
                    }
                    control = parent;
                }
                if (view != 5)
                {
                    ITemplate alternatingItemTemplate;
                    templateData.ItemPlaceholder = control.FindControl(this.ListView.ItemPlaceholderID);
                    if (templateData.ItemPlaceholder == null)
                    {
                        templateData.ItemPlaceholder = this.FindControlInternal(control, this.ListView.ItemPlaceholderID);
                        if (templateData.ItemPlaceholder == null)
                        {
                            contentType = ListViewRegionContentType.Error;
                            return string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoItemPlaceholder, new object[] { this.ID, this.ListView.ItemPlaceholderID });
                        }
                    }
                    System.Web.UI.Control placeholderContainer = templateData.ItemPlaceholder.Parent;
                    if (string.IsNullOrEmpty(placeholderContainer.ID))
                    {
                        contentType = ListViewRegionContentType.Error;
                        return string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoItemPlaceholderContainerID, new object[] { this.ID, this.ListView.ItemPlaceholderID });
                    }
                    templateData.ItemPlaceholderContainerUniqueID = placeholderContainer.UniqueID;
                    templateData.IndexOfItemPlaceholderInParent = placeholderContainer.Controls.IndexOf(templateData.ItemPlaceholder);
                    placeholderContainer.Controls.Remove(templateData.ItemPlaceholder);
                    switch (view)
                    {
                        case 1:
                            alternatingItemTemplate = this.ListView.AlternatingItemTemplate;
                            break;

                        case 2:
                            alternatingItemTemplate = this.ListView.EditItemTemplate;
                            break;

                        case 4:
                            alternatingItemTemplate = this.ListView.EmptyItemTemplate;
                            break;

                        case 7:
                            alternatingItemTemplate = this.ListView.InsertItemTemplate;
                            break;

                        case 8:
                            alternatingItemTemplate = this.ListView.ItemSeparatorTemplate;
                            break;

                        case 9:
                            alternatingItemTemplate = this.ListView.ItemTemplate;
                            break;

                        case 11:
                            alternatingItemTemplate = this.ListView.SelectedItemTemplate;
                            break;

                        default:
                            contentType = ListViewRegionContentType.Error;
                            return AtlasWebDesign.ListView_UnknownView;
                    }
                    templateData.ControlsAddedToItemPlaceholderContainer = InstantiateTemplateInContainer(this.DesignerHost, alternatingItemTemplate, placeholderContainer, templateData.IndexOfItemPlaceholderInParent, out contentType);
                    if (contentType != ListViewRegionContentType.Valid)
                    {
                        return AtlasWebDesign.ListView_MustEditInSourceView;
                    }
                }
            }
            StringBuilder builder = new StringBuilder();
            foreach (System.Web.UI.Control control4 in container.Controls)
            {
                builder.Append(ControlPersister.PersistControl(control4, designerHost));
            }
            contentType = ListViewRegionContentType.Valid;
            return builder.ToString();
        }

        internal string[] GetTemplateNames() => 
            ((string[]) _controlTemplateNames.Clone());

        internal List<int> GetViewIndices()
        {
            List<int> list = new List<int>(10) { 0 };
            if (this.TemplateExists(1))
            {
                list.Add(1);
            }
            if (this.TemplateExists(2))
            {
                list.Add(2);
            }
            if (this.TemplateExists(3))
            {
                list.Add(3);
            }
            if (this.TemplateExists(4))
            {
                list.Add(4);
            }
            if (this.TemplateExists(5))
            {
                list.Add(5);
            }
            if (this.TemplateExists(7))
            {
                list.Add(7);
            }
            if (this.TemplateExists(8))
            {
                list.Add(8);
            }
            if (this.TemplateExists(9))
            {
                list.Add(9);
            }
            if (this.TemplateExists(11))
            {
                list.Add(11);
            }
            return list;
        }

        public override void Initialize(IComponent component)
        {
            ControlDesigner.VerifyInitializeArgument(component, typeof(System.Web.UI.WebControls.ListView));
            base.Initialize(component);
            base.SetViewFlags(ViewFlags.TemplateEditing, true);
        }

        private static int InstantiateTemplateInContainer(IDesignerHost host, ITemplate template, System.Web.UI.Control placeholderContainer, int indexOfPlaceholderInParent, out ListViewRegionContentType contentType)
        {
            int num = 0;
            ITemplate template2 = TrimTemplate(host, template);
            if (template2 != null)
            {
                System.Web.UI.Control container = new System.Web.UI.Control();
                template2.InstantiateIn(container);
                int index = indexOfPlaceholderInParent;
                int count = placeholderContainer.Controls.Count;
                int num4 = container.Controls.Count;
                for (int i = 0; i < num4; i++)
                {
                    System.Web.UI.Control child = container.Controls[0];
                    container.Controls.RemoveAt(0);
                    int num6 = placeholderContainer.Controls.Count;
                    if ((placeholderContainer is HtmlTable) && !(child is HtmlTableRow))
                    {
                        contentType = ListViewRegionContentType.Message;
                        return 0;
                    }
                    if ((placeholderContainer is HtmlTableRow) && !(child is HtmlTableCell))
                    {
                        contentType = ListViewRegionContentType.Message;
                        return 0;
                    }
                    placeholderContainer.Controls.AddAt(index, child);
                    int num7 = placeholderContainer.Controls.Count;
                    index += num7 - num6;
                }
                num = placeholderContainer.Controls.Count - count;
            }
            contentType = ListViewRegionContentType.Valid;
            return num;
        }

        public override void OnComponentChanged(object sender, ComponentChangedEventArgs ce)
        {
            if (!this._ignoreComponentChanged)
            {
                base.OnComponentChanged(sender, ce);
            }
        }

        private void PersistTemplate(IDesignerHost host, ITemplate template, string propertyName)
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(base.Component)[propertyName];
            using (DesignerTransaction transaction = host.CreateTransaction("SetEditableDesignerRegionContent"))
            {
                descriptor.SetValue(base.Component, template);
                transaction.Commit();
            }
        }

        private void PersistTemplateContent(IDesignerHost host, string content, string propertyName)
        {
            ITemplate template = ControlParser.ParseTemplate(host, content);
            this.PersistTemplate(host, template, propertyName);
        }

        private static string SerializePlaceholderContainerContents(IDesignerHost host, System.Web.UI.Control placeholderContainer, int startIndex, int controlsToSerialize, System.Web.UI.Control placeholder)
        {
            StringBuilder builder = new StringBuilder();
            int num = Math.Min(startIndex + controlsToSerialize, placeholderContainer.Controls.Count);
            for (int i = startIndex; i < num; i++)
            {
                builder.Append(ControlPersister.PersistControl(placeholderContainer.Controls[startIndex], host));
                placeholderContainer.Controls.RemoveAt(startIndex);
            }
            placeholderContainer.Controls.AddAt(startIndex, placeholder);
            return builder.ToString();
        }

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            try
            {
                this._ignoreComponentChanged = true;
                ListViewEditableRegion region2 = region as ListViewEditableRegion;
                if (region2 != null)
                {
                    ListViewRegionContentType type;
                    this.SetListViewTemplates(content, region2.TemplateIndex, this._templateData, false, out type);
                }
                else
                {
                    base.SetEditableDesignerRegionContent(region, content);
                }
            }
            finally
            {
                this._ignoreComponentChanged = false;
                this.OnComponentChanged(this, new ComponentChangedEventArgs(base.Component, null, null, null));
            }
        }

        private string SetListViewTemplates(string content, int view, TemplateData templateData, bool dontPersist, out ListViewRegionContentType contentType)
        {
            IDesignerHost designerHost = this.DesignerHost;
            DesignerPanel panel = new DesignerPanel();
            DesignerPanel child = new DesignerPanel {
                ID = "__dtpanel"
            };
            panel.Controls.Add(child);
            ITemplate template = ControlParser.ParseTemplate(designerHost, content);
            if (view == 0)
            {
                contentType = ListViewRegionContentType.Valid;
                return string.Empty;
            }
            if (view == 3)
            {
                if (!dontPersist)
                {
                    this.PersistTemplate(designerHost, template, _controlTemplateNames[view]);
                }
                contentType = ListViewRegionContentType.Valid;
                return string.Empty;
            }
            if (template != null)
            {
                template.InstantiateIn(child);
            }
            System.Web.UI.Control placeholderContainer = null;
            System.Web.UI.Control control2 = null;
            if (!string.IsNullOrEmpty(templateData.GroupPlaceholderContainerUniqueID))
            {
                control2 = panel.FindControl(templateData.GroupPlaceholderContainerUniqueID);
            }
            if (control2 != null)
            {
                if (!string.IsNullOrEmpty(templateData.ItemPlaceholderContainerUniqueID))
                {
                    placeholderContainer = panel.FindControl(templateData.ItemPlaceholderContainerUniqueID);
                }
            }
            else if (!string.IsNullOrEmpty(templateData.ItemPlaceholderContainerUniqueID))
            {
                placeholderContainer = panel.FindControl(templateData.ItemPlaceholderContainerUniqueID);
            }
            if (view != 5)
            {
                if (placeholderContainer == null)
                {
                    contentType = ListViewRegionContentType.Error;
                    return string.Format(CultureInfo.CurrentCulture, AtlasWebDesign.ListView_NoItemPlaceholderContainer, new object[] { this.ID, this.ListView.ItemPlaceholderID });
                }
                string str = SerializePlaceholderContainerContents(designerHost, placeholderContainer, templateData.IndexOfItemPlaceholderInParent, templateData.ControlsAddedToItemPlaceholderContainer, templateData.ItemPlaceholder);
                if (!dontPersist)
                {
                    this.PersistTemplateContent(designerHost, str, _controlTemplateNames[view]);
                }
            }
            if (control2 != null)
            {
                string propertyName = "GroupTemplate";
                if (view == 5)
                {
                    propertyName = "GroupSeparatorTemplate";
                }
                string str3 = SerializePlaceholderContainerContents(designerHost, control2, templateData.IndexOfGroupPlaceholderInParent, templateData.ControlsAddedToGroupPlaceholderContainer, templateData.GroupPlaceholder);
                if (!dontPersist)
                {
                    this.PersistTemplateContent(designerHost, str3, propertyName);
                }
            }
            StringBuilder builder = new StringBuilder();
            foreach (System.Web.UI.Control control3 in child.Controls)
            {
                builder.Append(ControlPersister.PersistControl(control3, designerHost));
            }
            if (!dontPersist)
            {
                this.PersistTemplateContent(designerHost, builder.ToString(), "LayoutTemplate");
            }
            contentType = ListViewRegionContentType.Valid;
            return string.Empty;
        }

        private bool TemplateExists(int viewIndex)
        {
            ITemplate alternatingItemTemplate = null;
            switch (viewIndex)
            {
                case 1:
                    alternatingItemTemplate = this.ListView.AlternatingItemTemplate;
                    break;

                case 2:
                    alternatingItemTemplate = this.ListView.EditItemTemplate;
                    break;

                case 3:
                    alternatingItemTemplate = this.ListView.EmptyDataTemplate;
                    break;

                case 4:
                    alternatingItemTemplate = this.ListView.EmptyItemTemplate;
                    break;

                case 5:
                    alternatingItemTemplate = this.ListView.GroupSeparatorTemplate;
                    break;

                case 7:
                    alternatingItemTemplate = this.ListView.InsertItemTemplate;
                    break;

                case 8:
                    alternatingItemTemplate = this.ListView.ItemSeparatorTemplate;
                    break;

                case 9:
                    alternatingItemTemplate = this.ListView.ItemTemplate;
                    break;

                case 11:
                    alternatingItemTemplate = this.ListView.SelectedItemTemplate;
                    break;
            }
            return this.TemplateExists(alternatingItemTemplate);
        }

        private bool TemplateExists(ITemplate template)
        {
            if (template == null)
            {
                return false;
            }
            IDesignerHost service = (IDesignerHost) base.ViewControl.Site.GetService(typeof(IDesignerHost));
            string str = ControlPersister.PersistTemplate(template, service);
            return ((str != null) && (str.Length > 0));
        }

        private static ITemplate TrimTemplate(IDesignerHost host, ITemplate template)
        {
            if (template == null)
            {
                return null;
            }
            string templateText = ControlPersister.PersistTemplate(template, host).Trim();
            return ControlParser.ParseTemplate(host, templateText);
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                if (this._actionLists == null)
                {
                    this._actionLists = new ListViewActionList(this);
                }
                lists.Add(this._actionLists);
                return lists;
            }
        }

        internal bool AllowConfiguration
        {
            get
            {
                DesignerDataSourceView designerView = base.DesignerView;
                return (((!base.InTemplateMode && (designerView != null)) && (designerView.Schema != null)) && (designerView.Schema.GetFields().Length > 0));
            }
        }

        internal Collection<ListViewAutoLayout> AutoLayouts
        {
            get
            {
                IDesignerHost service = (IDesignerHost) base.Component.Site.GetService(typeof(IDesignerHost));
                DesignerDataSourceView designerView = base.DesignerView;
                IDataSourceViewSchema schema = null;
                if (designerView != null)
                {
                    schema = designerView.Schema;
                }
                return ListViewAutoLayouts.GetLayouts(service, schema);
            }
        }

        private TemplateDefinition CurrentTemplateDefinition
        {
            get
            {
                int currentView = this.CurrentView;
                if (currentView != 0)
                {
                    return new TemplateDefinition(this, _controlTemplateNames[currentView], base.Component, _controlTemplateNames[currentView]) { SupportsDataBinding = _controlTemplateSupportsDataBinding[currentView] };
                }
                return null;
            }
        }

        internal int CurrentView
        {
            get
            {
                object obj2 = base.DesignerState["CurrentView"];
                if (obj2 != null)
                {
                    return (int) obj2;
                }
                return 0;
            }
            set
            {
                base.DesignerState["CurrentView"] = value;
                this.UpdateDesignTimeHtml();
            }
        }

        private IDesignerHost DesignerHost =>
            ((IDesignerHost) this.GetService(typeof(IDesignerHost)));

        internal bool EssentialTemplatesExist =>
            (this.TemplateExists(((System.Web.UI.WebControls.ListView) base.ViewControl).ItemTemplate) && this.TemplateExists(((System.Web.UI.WebControls.ListView) base.ViewControl).LayoutTemplate));

        internal bool HasDataSourceWithoutSchema
        {
            get
            {
                DesignerDataSourceView designerView = base.DesignerView;
                return ((!base.InTemplateMode && (designerView != null)) && (designerView.Schema == null));
            }
        }

        private System.Web.UI.WebControls.ListView ListView =>
            ((System.Web.UI.WebControls.ListView) base.Component);

        protected override int SampleRowCount =>
            0x15;

        protected override bool UsePreviewControl =>
            true;

        private sealed class ListViewEditableRegion : TemplatedEditableDesignerRegion
        {
            private int _templateIndex;

            public ListViewEditableRegion(int templateIndex, TemplateDefinition templateDefinition) : base(templateDefinition)
            {
                this._templateIndex = templateIndex;
            }

            public int TemplateIndex =>
                this._templateIndex;
        }

        private enum ListViewRegionContentType
        {
            Valid,
            Error,
            Message
        }

        private sealed class TemplateData
        {
            public int ControlsAddedToGroupPlaceholderContainer;
            public int ControlsAddedToItemPlaceholderContainer;
            public System.Web.UI.Control GroupPlaceholder;
            public string GroupPlaceholderContainerUniqueID;
            public int IndexOfGroupPlaceholderInParent;
            public int IndexOfItemPlaceholderInParent;
            public System.Web.UI.Control ItemPlaceholder;
            public string ItemPlaceholderContainerUniqueID;
        }
    }
}

