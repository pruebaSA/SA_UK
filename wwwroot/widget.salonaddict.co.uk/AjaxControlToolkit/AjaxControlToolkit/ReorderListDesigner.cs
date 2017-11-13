namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.Design.WebControls;
    using System.Web.UI.WebControls;

    internal class ReorderListDesigner : DataBoundControlDesigner
    {
        private const string _designtimeHTML = "<table cellspacing=0 cellpadding=0 border=0 style=\"display:inline-block\">\r\n                <tr>\r\n                    <td nowrap align=center valign=middle style=\"color:{0}; background-color:{1}; \">{2}</td>\r\n                </tr>\r\n                <tr>\r\n                    <td style=\"vertical-align:top;\" {3}='0'>{4}</td>\r\n                </tr>\r\n          </table>";
        private const string _designtimeHTML_Template = "\r\n                <table cellspacing=0 cellpadding=0 border=0 style=\"display:inline-block;border:outset white 1px;\">\r\n                <tr>\r\n                    <td nowrap align=center valign=middle style=\"background-color:{6}; \"><span style=\"font:messagebox;color:{5}\"><b>{8}</b> - {7}</span></td>\r\n                </tr>               \r\n                <tr>                \r\n                <td>\r\n                  <table cellspacing=0 cellpadding=2 border=0 style=\"margin:2px;border:solid 1px buttonface\">\r\n                    <tr style=\"font:messagebox;background-color:lightblue;color:black\">\r\n                      <td style=\"border:solid 1px buttonshadow\">\r\n                        &nbsp;{0}&nbsp;&nbsp;&nbsp;\r\n                      </td>\r\n                    </tr>\r\n                    <tr style=\"{1}\" height=100%>\r\n                      <td style=\"{2}\">\r\n                        <div style=\"width:100%;height:100%\" {3}='0'>{4}</div>\r\n                      </td>\r\n                    </tr>\r\n                  </table>\r\n                </td>\r\n              </tr></table>";
        private TemplateGroupCollection _templateGroups;
        private const int DefaultTemplateIndex = 0;
        private static TemplateItem[] TemplateItems = new TemplateItem[] { new TemplateItem("ItemTemplate", true), new TemplateItem("EditItemTemplate", true), new TemplateItem("DragHandleTemplate", true), new TemplateItem("ReorderTemplate", false), new TemplateItem("InsertItemTemplate", true), new TemplateItem("EmptyListTemplate", false) };

        private EditableDesignerRegion BuildRegion() => 
            new ReorderListDesignerRegion(this.CurrentObject, this.CurrentTemplate, this.CurrentTemplateDescriptor, this.TemplateDefinition) { Description = this.CurrentViewName };

        public override string GetDesignTimeHtml()
        {
            string designTimeHtml = string.Empty;
            if (this.CurrentViewControlTemplate != null)
            {
                AjaxControlToolkit.ReorderList viewControl = (AjaxControlToolkit.ReorderList) base.ViewControl;
                IDictionary data = new HybridDictionary(1) {
                    ["TemplateIndex"] = this.CurrentView
                };
                ((IControlDesignerAccessor) viewControl).SetDesignModeState(data);
                designTimeHtml = base.GetDesignTimeHtml();
            }
            return designTimeHtml;
        }

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            string str = string.Empty;
            regions.Add(this.BuildRegion());
            StringBuilder builder = new StringBuilder(0x400);
            if (this.CurrentTemplate == null)
            {
                builder.Append(string.Format(CultureInfo.InvariantCulture, "<table cellspacing=0 cellpadding=0 border=0 style=\"display:inline-block\">\r\n                <tr>\r\n                    <td nowrap align=center valign=middle style=\"color:{0}; background-color:{1}; \">{2}</td>\r\n                </tr>\r\n                <tr>\r\n                    <td style=\"vertical-align:top;\" {3}='0'>{4}</td>\r\n                </tr>\r\n          </table>", new object[] { ColorTranslator.ToHtml(SystemColors.ControlText), ColorTranslator.ToHtml(SystemColors.Control), this.ReorderList.ID, DesignerRegion.DesignerRegionAttributeName, str }));
            }
            else
            {
                DataList list = new DataList();
                builder.Append(string.Format(CultureInfo.InvariantCulture, "\r\n                <table cellspacing=0 cellpadding=0 border=0 style=\"display:inline-block;border:outset white 1px;\">\r\n                <tr>\r\n                    <td nowrap align=center valign=middle style=\"background-color:{6}; \"><span style=\"font:messagebox;color:{5}\"><b>{8}</b> - {7}</span></td>\r\n                </tr>               \r\n                <tr>                \r\n                <td>\r\n                  <table cellspacing=0 cellpadding=2 border=0 style=\"margin:2px;border:solid 1px buttonface\">\r\n                    <tr style=\"font:messagebox;background-color:lightblue;color:black\">\r\n                      <td style=\"border:solid 1px buttonshadow\">\r\n                        &nbsp;{0}&nbsp;&nbsp;&nbsp;\r\n                      </td>\r\n                    </tr>\r\n                    <tr style=\"{1}\" height=100%>\r\n                      <td style=\"{2}\">\r\n                        <div style=\"width:100%;height:100%\" {3}='0'>{4}</div>\r\n                      </td>\r\n                    </tr>\r\n                  </table>\r\n                </td>\r\n              </tr></table>", new object[] { this.CurrentViewName, list.HeaderStyle, this.ReorderList.ControlStyle, DesignerRegion.DesignerRegionAttributeName, str, ColorTranslator.ToHtml(SystemColors.ControlText), ColorTranslator.ToHtml(SystemColors.Control), this.ReorderList.ID, this.ReorderList.GetType().Name }));
                list.Dispose();
            }
            return builder.ToString();
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            ReorderListDesignerRegion region2 = region as ReorderListDesignerRegion;
            if (region2 != null)
            {
                ITemplate template = region2.Template;
                if (template != null)
                {
                    IDesignerHost service = (IDesignerHost) base.Component.Site.GetService(typeof(IDesignerHost));
                    return ControlPersister.PersistTemplate(template, service);
                }
            }
            return base.GetEditableDesignerRegionContent(region);
        }

        protected override string GetEmptyDesignTimeHtml()
        {
            string instruction = "<br />Empty " + TemplateItems[this.CurrentView].Name + "<br />";
            return base.CreatePlaceHolderDesignTimeHtml(instruction);
        }

        protected override string GetErrorDesignTimeHtml(Exception e) => 
            base.CreatePlaceHolderDesignTimeHtml("Error rendering ReorderList:<br />" + e.Message);

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            ReorderListDesignerRegion region2 = region as ReorderListDesignerRegion;
            if (region2 != null)
            {
                IDesignerHost service = (IDesignerHost) base.Component.Site.GetService(typeof(IDesignerHost));
                ITemplate template = ControlParser.ParseTemplate(service, content);
                using (DesignerTransaction transaction = service.CreateTransaction("SetEditableDesignerRegionContent"))
                {
                    region2.PropertyDescriptor.SetValue(region2.Object, template);
                    transaction.Commit();
                }
                region2.Template = template;
            }
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                lists.Add(new ReorderListDesignerActionList(this));
                return lists;
            }
        }

        private object CurrentObject =>
            base.Component;

        private ITemplate CurrentTemplate
        {
            get
            {
                if (this.CurrentTemplateDescriptor != null)
                {
                    return (ITemplate) this.CurrentTemplateDescriptor.GetValue(base.Component);
                }
                return null;
            }
        }

        private PropertyDescriptor CurrentTemplateDescriptor
        {
            get
            {
                string name = TemplateItems[this.CurrentView].Name;
                return TypeDescriptor.GetProperties(base.Component)[name];
            }
        }

        private int CurrentView
        {
            get
            {
                object obj2 = base.DesignerState["CurrentView"];
                int num = (obj2 == null) ? 0 : ((int) obj2);
                if (num >= TemplateItems.Length)
                {
                    num = 0;
                }
                return num;
            }
            set
            {
                base.DesignerState["CurrentView"] = value;
            }
        }

        private ITemplate CurrentViewControlTemplate =>
            this.CurrentTemplate;

        private string CurrentViewName =>
            TemplateItems[this.CurrentView].Name;

        private AjaxControlToolkit.ReorderList ReorderList =>
            ((AjaxControlToolkit.ReorderList) base.Component);

        private System.Web.UI.Design.TemplateDefinition TemplateDefinition =>
            new System.Web.UI.Design.TemplateDefinition(this, this.CurrentViewName, this.ReorderList, this.CurrentViewName) { SupportsDataBinding=true };

        public override TemplateGroupCollection TemplateGroups
        {
            get
            {
                TemplateGroupCollection templateGroups = base.TemplateGroups;
                if (this._templateGroups == null)
                {
                    this._templateGroups = new TemplateGroupCollection();
                    foreach (TemplateItem item in TemplateItems)
                    {
                        TemplateGroup group = new TemplateGroup(item.Name);
                        System.Web.UI.Design.TemplateDefinition templateDefinition = new System.Web.UI.Design.TemplateDefinition(this, item.Name, this.ReorderList, item.Name) {
                            SupportsDataBinding = item.SupportsDataBinding
                        };
                        group.AddTemplateDefinition(templateDefinition);
                        this._templateGroups.Add(group);
                    }
                }
                templateGroups.AddRange(this._templateGroups);
                return templateGroups;
            }
        }

        protected override bool UsePreviewControl =>
            true;

        private class ReorderListDesignerActionList : DesignerActionList
        {
            private ReorderListDesigner _designer;

            public ReorderListDesignerActionList(ReorderListDesigner designer) : base(designer.Component)
            {
                this._designer = designer;
            }

            [TypeConverter(typeof(ReorderListViewTypeConverter))]
            public string View
            {
                get => 
                    this._designer.CurrentViewName;
                set
                {
                    int num = Array.FindIndex<ReorderListDesigner.TemplateItem>(ReorderListDesigner.TemplateItems, t => t.Name == value);
                    if (num != -1)
                    {
                        this._designer.CurrentView = num;
                    }
                    this._designer.UpdateDesignTimeHtml();
                }
            }

            private class ReorderListViewTypeConverter : TypeConverter
            {
                public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
                {
                    string[] values = new string[ReorderListDesigner.TemplateItems.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        values[i] = ReorderListDesigner.TemplateItems[i].Name;
                    }
                    return new TypeConverter.StandardValuesCollection(values);
                }

                public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) => 
                    true;

                public override bool GetStandardValuesSupported(ITypeDescriptorContext context) => 
                    true;
            }
        }

        private class ReorderListDesignerRegion : TemplatedEditableDesignerRegion
        {
            private object _object;
            private System.ComponentModel.PropertyDescriptor _prop;
            private ITemplate _template;

            public ReorderListDesignerRegion(object obj, ITemplate template, System.ComponentModel.PropertyDescriptor descriptor, TemplateDefinition definition) : base(definition)
            {
                this._template = template;
                this._object = obj;
                this._prop = descriptor;
                base.EnsureSize = true;
            }

            public object Object =>
                this._object;

            public System.ComponentModel.PropertyDescriptor PropertyDescriptor =>
                this._prop;

            public ITemplate Template
            {
                get => 
                    this._template;
                set
                {
                    this._template = value;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct TemplateItem
        {
            public readonly string Name;
            public readonly bool SupportsDataBinding;
            public TemplateItem(string name, bool supportsDataBinding)
            {
                this.Name = name;
                this.SupportsDataBinding = supportsDataBinding;
            }
        }
    }
}

