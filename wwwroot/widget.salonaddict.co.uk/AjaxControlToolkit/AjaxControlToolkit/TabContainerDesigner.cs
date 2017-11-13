namespace AjaxControlToolkit
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.Design;
    using System.Web.UI.WebControls;

    public class TabContainerDesigner : ControlDesigner
    {
        private const string ActiveTabLink = "{0}";
        private const string AddTabName = "#addtab";
        private const string ClickRegionHtml = "<div style='float:left;padding:2px;color:{3}; background-color:{4};{5};height:20px;' {0}='{1}'>{2}</div>";
        private const string DesignTimeHtml = "<div style=\"padding:2px;width:{7};height:{8}\">\r\n                <div style='text-align:center;color:{0}; background-color:{1};border-left:thin white outset; border-right:thin white outset;height:20px;'>{2}</div>\r\n                <div style='color:{3}; background-color:{4};border-left:thin white outset; border-right:thin white outset;height:24px;text-align:left;'>{5}</div>\r\n                <div style='clear:both;text-align:left;border-left:thin white outset; border-bottom:thin white outset; border-right:thin white outset;background-color:{10};height:{11}; padding:8px; overflow:{12};' {9}='0'>{6}</div>\r\n            </div>";
        private const string EmptyDesignTimeHtml = "<div style='display:inline-block;padding:2px;'>\r\n                    <div style='color:{0}; background-color:{1};border-left:thin white outset; border-right:thin white outset;'>{2}</div>\r\n                    <div style=\"text-align:center;border-left:thin white outset; border-bottom:thin white outset; border-right:thin white outset;\" {3}='0'>\r\n        <a href='#'>Add New Tab</a></div>\r\n                </div>";
        private const string TabLink = "<a style='padding:2px;border-top:thin white inset;border-left:thin white inset; border-right:thin white inset;' href='#'>{0}</a>";

        public override string GetDesignTimeHtml(DesignerRegionCollection regions)
        {
            if (regions == null)
            {
                throw new ArgumentNullException("regions");
            }
            if (this.TabContainer.ActiveTab != null)
            {
                EditableDesignerRegion region = new EditableDesignerRegion(this, string.Format(CultureInfo.InvariantCulture, "c{0}", new object[] { this.TabContainer.ActiveTab.ID }));
                regions.Add(region);
                string tabContent = this.GetTabContent(this.TabContainer.ActiveTab, true);
                StringBuilder builder = new StringBuilder();
                int num = 2;
                foreach (TabPanel panel in this.TabContainer.Tabs)
                {
                    bool active = panel.Active;
                    string str2 = this.GetTabContent(panel, false);
                    builder.AppendFormat(CultureInfo.InvariantCulture, "<div style='float:left;padding:2px;color:{3}; background-color:{4};{5};height:20px;' {0}='{1}'>{2}</div>", new object[] { DesignerRegion.DesignerRegionAttributeName, active ? 1 : num, string.Format(CultureInfo.InvariantCulture, active ? "{0}" : "<a style='padding:2px;border-top:thin white inset;border-left:thin white inset; border-right:thin white inset;' href='#'>{0}</a>", new object[] { str2 }), ColorTranslator.ToHtml(SystemColors.ControlText), active ? ColorTranslator.ToHtml(SystemColors.Window) : "transparent", active ? "border-left:thin white outset;border-right:thin white outset;" : "" });
                    if (active)
                    {
                        regions.Insert(1, new EditableDesignerRegion(this, string.Format(CultureInfo.InvariantCulture, "h{0}", new object[] { panel.ID })));
                    }
                    else
                    {
                        DesignerRegion region2 = new DesignerRegion(this, string.Format(CultureInfo.InvariantCulture, "t{0}", new object[] { panel.ID })) {
                            Selectable = true
                        };
                        num++;
                        regions.Add(region2);
                    }
                }
                StringBuilder builder2 = new StringBuilder(0x400);
                string str3 = (!this.TabContainer.Height.IsEmpty && (this.TabContainer.Height.Type == UnitType.Pixel)) ? (((this.TabContainer.Height.Value - 62.0)).ToString() + "px") : "100%";
                builder2.Append(string.Format(CultureInfo.InvariantCulture, "<div style=\"padding:2px;width:{7};height:{8}\">\r\n                <div style='text-align:center;color:{0}; background-color:{1};border-left:thin white outset; border-right:thin white outset;height:20px;'>{2}</div>\r\n                <div style='color:{3}; background-color:{4};border-left:thin white outset; border-right:thin white outset;height:24px;text-align:left;'>{5}</div>\r\n                <div style='clear:both;text-align:left;border-left:thin white outset; border-bottom:thin white outset; border-right:thin white outset;background-color:{10};height:{11}; padding:8px; overflow:{12};' {9}='0'>{6}</div>\r\n            </div>", new object[] { ColorTranslator.ToHtml(SystemColors.ControlText), ColorTranslator.ToHtml(SystemColors.ControlDark), this.TabContainer.ID, ColorTranslator.ToHtml(SystemColors.ControlText), ColorTranslator.ToHtml(SystemColors.Control), builder.ToString(), tabContent, this.TabContainer.Width, this.TabContainer.Height, DesignerRegion.DesignerRegionAttributeName, ColorTranslator.ToHtml(SystemColors.Window), str3, this.HideOverflowContent ? "hidden" : "visible" }));
                return builder2.ToString();
            }
            StringBuilder builder3 = new StringBuilder(0x200);
            builder3.AppendFormat(CultureInfo.InvariantCulture, "<div style='display:inline-block;padding:2px;'>\r\n                    <div style='color:{0}; background-color:{1};border-left:thin white outset; border-right:thin white outset;'>{2}</div>\r\n                    <div style=\"text-align:center;border-left:thin white outset; border-bottom:thin white outset; border-right:thin white outset;\" {3}='0'>\r\n        <a href='#'>Add New Tab</a></div>\r\n                </div>", new object[] { ColorTranslator.ToHtml(SystemColors.ControlText), ColorTranslator.ToHtml(SystemColors.ControlDark), this.TabContainer.ID, DesignerRegion.DesignerRegionAttributeName });
            DesignerRegion region3 = new DesignerRegion(this, "#addtab");
            regions.Add(region3);
            return builder3.ToString();
        }

        public override string GetEditableDesignerRegionContent(EditableDesignerRegion region)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }
            string name = region.Name;
            bool content = name[0] == 'c';
            name = name.Substring(1);
            TabPanel tab = (TabPanel) this.TabContainer.FindControl(name);
            return this.GetTabContent(tab, content);
        }

        private string GetTabContent(TabPanel tab, bool content)
        {
            if (tab != null)
            {
                if (content && (tab.ContentTemplate != null))
                {
                    return this.GetTemplateContent(tab.ContentTemplate, "_content");
                }
                if (!content)
                {
                    if (tab.HeaderTemplate != null)
                    {
                        return this.GetTemplateContent(tab.HeaderTemplate, "_header");
                    }
                    return tab.HeaderText;
                }
            }
            return "";
        }

        private string GetTemplateContent(ITemplate template, string id)
        {
            DesignerPanel container = new DesignerPanel {
                ID = id
            };
            template.InstantiateIn(container);
            IDesignerHost service = (IDesignerHost) this.GetService(typeof(IDesignerHost));
            StringBuilder builder = new StringBuilder(0x400);
            foreach (Control control in container.Controls)
            {
                builder.Append(ControlPersister.PersistControl(control, service));
            }
            return builder.ToString();
        }

        private static string GetUniqueName(Type t, Control parent)
        {
            string name = t.Name;
            int num = 1;
            while (parent.FindControl(name + num.ToString(CultureInfo.InvariantCulture)) != null)
            {
                num++;
            }
            return (name + num.ToString(CultureInfo.InvariantCulture));
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);
            base.SetViewFlags(ViewFlags.TemplateEditing, true);
            foreach (TabPanel panel in this.TabContainer.Tabs)
            {
                if (string.IsNullOrEmpty(panel.ID))
                {
                    throw new InvalidOperationException("TabPanels must have IDs set.");
                }
            }
        }

        private void OnAddTabPanel()
        {
            IDesignerHost host = (IDesignerHost) this.GetService(typeof(IDesignerHost));
            if (host != null)
            {
                AjaxControlToolkit.TabContainer tabContainer = this.TabContainer;
                using (DesignerTransaction transaction = host.CreateTransaction("Add new TabPanel"))
                {
                    TabPanel child = (TabPanel) host.CreateComponent(typeof(TabPanel));
                    if (child != null)
                    {
                        child.ID = GetUniqueName(typeof(TabPanel), tabContainer);
                        child.HeaderText = child.ID;
                        IComponentChangeService service = (IComponentChangeService) this.GetService(typeof(IComponentChangeService));
                        try
                        {
                            service.OnComponentChanging(tabContainer, TypeDescriptor.GetProperties(tabContainer)["Tabs"]);
                            tabContainer.Tabs.Add(child);
                        }
                        finally
                        {
                            service.OnComponentChanged(tabContainer, TypeDescriptor.GetProperties(tabContainer)["Tabs"], tabContainer.Tabs, tabContainer.Tabs);
                        }
                        TypeDescriptor.GetProperties(tabContainer)["ActiveTab"].SetValue(tabContainer, child);
                        this.CurrentTabID = child.ID;
                    }
                    transaction.Commit();
                }
            }
        }

        protected override void OnClick(DesignerRegionMouseEventArgs e)
        {
            if ((e.Region != null) && e.Region.Name.StartsWith("t", StringComparison.Ordinal))
            {
                this.CurrentTabID = e.Region.Name.Substring(1);
            }
            else if ((e.Region != null) && (e.Region.Name == "#addtab"))
            {
                this.OnAddTabPanel();
            }
            base.OnClick(e);
        }

        private void OnRemoveTabPanel()
        {
            AjaxControlToolkit.TabContainer tabContainer = this.TabContainer;
            if (tabContainer.ActiveTab != null)
            {
                int activeTabIndex = tabContainer.ActiveTabIndex;
                IDesignerHost host = (IDesignerHost) this.GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    using (DesignerTransaction transaction = host.CreateTransaction("Remove TabPanel"))
                    {
                        TabPanel activeTab = tabContainer.ActiveTab;
                        IComponentChangeService service = (IComponentChangeService) this.GetService(typeof(IComponentChangeService));
                        try
                        {
                            service.OnComponentChanging(tabContainer, TypeDescriptor.GetProperties(tabContainer)["Tabs"]);
                            tabContainer.Tabs.Remove(activeTab);
                        }
                        finally
                        {
                            service.OnComponentChanged(tabContainer, TypeDescriptor.GetProperties(tabContainer)["Tabs"], tabContainer.Tabs, tabContainer.Tabs);
                        }
                        activeTab.Dispose();
                        if (tabContainer.Tabs.Count > 0)
                        {
                            TypeDescriptor.GetProperties(tabContainer)["ActiveTabIndex"].SetValue(tabContainer, Math.Min(activeTabIndex, tabContainer.Tabs.Count - 1));
                        }
                        this.UpdateDesignTimeHtml();
                        transaction.Commit();
                    }
                }
            }
        }

        private static void PersistTemplate(TabPanel panel, IDesignerHost host, ITemplate template, string propertyName)
        {
            using (DesignerTransaction transaction = host.CreateTransaction("SetEditableDesignerRegionContent"))
            {
                PropertyInfo property = panel.GetType().GetProperty(propertyName);
                if (property != null)
                {
                    property.SetValue(panel, template, null);
                    transaction.Commit();
                }
            }
        }

        private static void PersistTemplateContent(TabPanel panel, IDesignerHost host, string content, string propertyName)
        {
            ITemplate template = ControlParser.ParseTemplate(host, content);
            PersistTemplate(panel, host, template, propertyName);
        }

        public override void SetEditableDesignerRegionContent(EditableDesignerRegion region, string content)
        {
            if (region == null)
            {
                throw new ArgumentNullException("region");
            }
            string name = region.Name;
            bool flag = name[0] == 'c';
            name = name.Substring(1);
            TabPanel panel = (TabPanel) this.TabContainer.FindControl(name);
            IDesignerHost service = (IDesignerHost) this.GetService(typeof(IDesignerHost));
            PersistTemplateContent(panel, service, content, flag ? "ContentTemplate" : "HeaderTemplate");
        }

        public override DesignerActionListCollection ActionLists
        {
            get
            {
                DesignerActionListCollection lists = new DesignerActionListCollection();
                lists.AddRange(base.ActionLists);
                lists.Add(new TabContainerDesignerActionList(this));
                return lists;
            }
        }

        private string CurrentTabID
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    TabPanel panel = this.TabContainer.FindControl(value) as TabPanel;
                    if (panel == null)
                    {
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Can't find child tab '{0}'", new object[] { value }));
                    }
                    int num = -1;
                    AjaxControlToolkit.TabContainer tabContainer = this.TabContainer;
                    for (int i = 0; i < tabContainer.Tabs.Count; i++)
                    {
                        if (tabContainer.Tabs[i] == panel)
                        {
                            num = i;
                            break;
                        }
                    }
                    if (num != -1)
                    {
                        TypeDescriptor.GetProperties(tabContainer)["ActiveTabIndex"].SetValue(tabContainer, num);
                    }
                }
                this.UpdateDesignTimeHtml();
            }
        }

        [DesignOnly(true), DefaultValue(false), Category("Design"), Description("Hide overflow content at design-time.")]
        public bool HideOverflowContent
        {
            get
            {
                object obj2 = base.DesignerState["HideOverflowContent"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                object obj2 = base.DesignerState["HideOverflowContent"];
                if ((obj2 == null) || (((bool) obj2) != value))
                {
                    base.DesignerState["HideOverflowContent"] = value;
                    this.UpdateDesignTimeHtml();
                }
            }
        }

        private AjaxControlToolkit.TabContainer TabContainer =>
            ((AjaxControlToolkit.TabContainer) base.Component);

        protected override bool UsePreviewControl =>
            true;

        internal class DesignerPanel : Panel, INamingContainer
        {
        }

        private class TabContainerDesignerActionList : DesignerActionList
        {
            private TabContainerDesigner _designer;

            public TabContainerDesignerActionList(TabContainerDesigner designer) : base(designer.Component)
            {
                this._designer = designer;
            }

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();
                DesignerActionMethodItem item = new DesignerActionMethodItem(this, "OnAddTabPanel", "Add Tab Panel", true);
                DesignerActionMethodItem item2 = new DesignerActionMethodItem(this, "OnRemoveTabPanel", "Remove Tab Panel", true);
                DesignerActionPropertyItem item3 = new DesignerActionPropertyItem("HideOverflowContent", "Hide overflow content at design-time");
                items.Add(item);
                items.Add(item2);
                items.Add(item3);
                return items;
            }

            private void OnAddTabPanel()
            {
                this._designer.OnAddTabPanel();
            }

            private void OnRemoveTabPanel()
            {
                this._designer.OnRemoveTabPanel();
            }

            public bool HideOverflowContent
            {
                get => 
                    this._designer.HideOverflowContent;
                set
                {
                    this._designer.HideOverflowContent = value;
                }
            }
        }
    }
}

