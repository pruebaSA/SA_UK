namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls.Adapters;
    using System.Web.Util;

    [SupportsEventValidation, Designer("System.Web.UI.Design.WebControls.MenuDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ControlValueProperty("SelectedValue"), DefaultEvent("MenuItemClick"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class Menu : HierarchicalDataBoundControl, IPostBackEventHandler, INamingContainer
    {
        private bool _accessKeyRendered;
        private MenuItemBindingCollection _bindings;
        private Collection<int> _cachedLevelsContainingCssClass;
        private List<string> _cachedMenuItemClassNames;
        private List<string> _cachedMenuItemHyperLinkClassNames;
        private List<MenuItemStyle> _cachedMenuItemStyles;
        private string _cachedPopOutImageUrl;
        private string _cachedScrollDownImageUrl;
        private string _cachedScrollUpImageUrl;
        private List<string> _cachedSubMenuClassNames;
        private List<SubMenuStyle> _cachedSubMenuStyles;
        private int _cssStyleIndex;
        private string _currentSiteMapNodeUrl;
        private bool _dataBound;
        private Type _designTimeTextWriterType;
        private HyperLinkStyle _dynamicHoverHyperLinkStyle;
        private Style _dynamicHoverStyle;
        private MenuItemStyle _dynamicItemStyle;
        private SubMenuStyle _dynamicMenuStyle;
        private MenuItemStyle _dynamicSelectedStyle;
        private ITemplate _dynamicTemplate;
        private const string _getDesignTimeDynamicHtml = "GetDesignTimeDynamicHtml";
        private const string _getDesignTimeStaticHtml = "GetDesignTimeStaticHtml";
        private string[] _imageUrls;
        private bool _isNotIE;
        private MenuItemStyleCollection _levelMenuItemStyles;
        private MenuItemStyleCollection _levelSelectedStyles;
        private SubMenuStyleCollection _levelStyles;
        private int _maximumDepth = 0;
        private static readonly object _menuItemClickedEvent = new object();
        private static readonly object _menuItemDataBoundEvent = new object();
        private int _nodeIndex = 0;
        private PopOutPanel _panel;
        private Style _panelStyle;
        private MenuItem _rootItem;
        private Style _rootMenuItemStyle;
        private MenuItem _selectedItem;
        private HyperLinkStyle _staticHoverHyperLinkStyle;
        private Style _staticHoverStyle;
        private MenuItemStyle _staticItemStyle;
        private SubMenuStyle _staticMenuStyle;
        private MenuItemStyle _staticSelectedStyle;
        private ITemplate _staticTemplate;
        private bool _subControlsDataBound;
        internal const int ImageUrlsCount = 3;
        public static readonly string MenuItemClickCommandName = "Click";
        internal const int PopOutImageIndex = 2;
        internal const int ScrollDownImageIndex = 1;
        internal const int ScrollUpImageIndex = 0;

        [WebCategory("Behavior"), WebSysDescription("Menu_MenuItemClick")]
        public event MenuEventHandler MenuItemClick
        {
            add
            {
                base.Events.AddHandler(_menuItemClickedEvent, value);
            }
            remove
            {
                base.Events.RemoveHandler(_menuItemClickedEvent, value);
            }
        }

        [WebSysDescription("Menu_MenuItemDataBound"), WebCategory("Behavior")]
        public event MenuEventHandler MenuItemDataBound
        {
            add
            {
                base.Events.AddHandler(_menuItemDataBoundEvent, value);
            }
            remove
            {
                base.Events.RemoveHandler(_menuItemDataBoundEvent, value);
            }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            if (this.Page != null)
            {
                this.Page.VerifyRenderingInServerForm(this);
            }
            string accessKey = this.AccessKey;
            if (accessKey.Length != 0)
            {
                try
                {
                    this.AccessKey = string.Empty;
                    base.AddAttributesToRender(writer);
                    return;
                }
                finally
                {
                    this.AccessKey = accessKey;
                }
            }
            base.AddAttributesToRender(writer);
        }

        private static bool AppendCssClassName(StringBuilder builder, MenuItemStyle style, bool hyperlink)
        {
            bool flag = false;
            if (style != null)
            {
                if (style.CssClass.Length != 0)
                {
                    builder.Append(style.CssClass);
                    builder.Append(' ');
                    flag = true;
                }
                string str = hyperlink ? style.HyperLinkStyle.RegisteredCssClass : style.RegisteredCssClass;
                if (str.Length > 0)
                {
                    builder.Append(str);
                    builder.Append(' ');
                }
            }
            return flag;
        }

        private static void AppendMenuCssClassName(StringBuilder builder, SubMenuStyle style)
        {
            if (style != null)
            {
                if (style.CssClass.Length != 0)
                {
                    builder.Append(style.CssClass);
                    builder.Append(' ');
                }
                string registeredCssClass = style.RegisteredCssClass;
                if (registeredCssClass.Length > 0)
                {
                    builder.Append(registeredCssClass);
                    builder.Append(' ');
                }
            }
        }

        private static T CacheGetItem<T>(List<T> cacheList, int index) where T: class
        {
            if (index < cacheList.Count)
            {
                return cacheList[index];
            }
            return default(T);
        }

        private static void CacheSetItem<T>(List<T> cacheList, int index, T item) where T: class
        {
            if (cacheList.Count > index)
            {
                cacheList[index] = item;
            }
            else
            {
                for (int i = cacheList.Count; i < index; i++)
                {
                    T local = default(T);
                    cacheList.Add(local);
                }
                cacheList.Add(item);
            }
        }

        protected internal override void CreateChildControls()
        {
            this.Controls.Clear();
            if ((this.StaticItemTemplate != null) || (this.DynamicItemTemplate != null))
            {
                if (base.RequiresDataBinding && (!string.IsNullOrEmpty(this.DataSourceID) || (this.DataSource != null)))
                {
                    this.EnsureDataBound();
                }
                else
                {
                    this.CreateChildControlsFromItems(false);
                    base.ClearChildViewState();
                }
            }
        }

        private void CreateChildControlsFromItems(bool dataBinding)
        {
            if ((this.StaticItemTemplate != null) || (this.DynamicItemTemplate != null))
            {
                int num = 0;
                foreach (MenuItem item in this.Items)
                {
                    this.CreateTemplatedControls(this.StaticItemTemplate, item, num++, 0, dataBinding);
                }
            }
        }

        internal int CreateItemIndex() => 
            this._nodeIndex++;

        private void CreateTemplatedControls(ITemplate template, MenuItem item, int position, int depth, bool dataBinding)
        {
            if (template != null)
            {
                MenuItemTemplateContainer container = new MenuItemTemplateContainer(position, item);
                item.Container = container;
                template.InstantiateIn(container);
                this.Controls.Add(container);
                if (dataBinding)
                {
                    container.DataBind();
                }
            }
            int num = 0;
            foreach (MenuItem item2 in item.ChildItems)
            {
                int num2 = depth + 1;
                if (template == this.DynamicItemTemplate)
                {
                    this.CreateTemplatedControls(this.DynamicItemTemplate, item2, num++, num2, dataBinding);
                }
                else if (num2 < this.StaticDisplayLevels)
                {
                    this.CreateTemplatedControls(template, item2, num++, num2, dataBinding);
                }
                else if (this.DynamicItemTemplate != null)
                {
                    this.CreateTemplatedControls(this.DynamicItemTemplate, item2, num++, num2, dataBinding);
                }
            }
        }

        public sealed override void DataBind()
        {
            base.DataBind();
        }

        private void DataBindItem(MenuItem item)
        {
            HierarchicalDataSourceView data = this.GetData(item.DataPath);
            if (base.IsBoundUsingDataSourceID || (this.DataSource != null))
            {
                if (data == null)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Menu_DataSourceReturnedNullView", new object[] { this.ID }));
                }
                IHierarchicalEnumerable enumerable = data.Select();
                item.ChildItems.Clear();
                if (enumerable != null)
                {
                    if (base.IsBoundUsingDataSourceID)
                    {
                        SiteMapDataSource dataSource = this.GetDataSource() as SiteMapDataSource;
                        if (dataSource != null)
                        {
                            SiteMapNode currentNode = dataSource.Provider.CurrentNode;
                            if (currentNode != null)
                            {
                                this._currentSiteMapNodeUrl = currentNode.Url;
                            }
                        }
                    }
                    try
                    {
                        this.DataBindRecursive(item, enumerable);
                    }
                    finally
                    {
                        this._currentSiteMapNodeUrl = null;
                    }
                }
            }
        }

        private void DataBindRecursive(MenuItem node, IHierarchicalEnumerable enumerable)
        {
            int depth = node.Depth + 1;
            if ((this.MaximumDynamicDisplayLevels == -1) || (depth < this.MaximumDepth))
            {
                foreach (object obj2 in enumerable)
                {
                    IHierarchyData hierarchyData = enumerable.GetHierarchyData(obj2);
                    string text = null;
                    string str2 = null;
                    string navigateUrl = string.Empty;
                    string imageUrl = string.Empty;
                    string popOutImageUrl = string.Empty;
                    string separatorImageUrl = string.Empty;
                    string target = string.Empty;
                    bool result = true;
                    bool flag2 = false;
                    bool selectable = true;
                    bool flag4 = false;
                    string toolTip = string.Empty;
                    string dataMember = string.Empty;
                    dataMember = hierarchyData.Type;
                    MenuItemBinding binding = this.DataBindings.GetBinding(dataMember, depth);
                    if (binding != null)
                    {
                        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj2);
                        string textField = binding.TextField;
                        if (textField.Length > 0)
                        {
                            PropertyDescriptor descriptor = properties.Find(textField, true);
                            if (descriptor == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { textField, "TextField" }));
                            }
                            object obj3 = descriptor.GetValue(obj2);
                            if (obj3 != null)
                            {
                                if (binding.FormatString.Length > 0)
                                {
                                    text = string.Format(CultureInfo.CurrentCulture, binding.FormatString, new object[] { obj3 });
                                }
                                else
                                {
                                    text = obj3.ToString();
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(binding.Text))
                        {
                            text = binding.Text;
                        }
                        string valueField = binding.ValueField;
                        if (valueField.Length > 0)
                        {
                            PropertyDescriptor descriptor2 = properties.Find(valueField, true);
                            if (descriptor2 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { valueField, "ValueField" }));
                            }
                            object obj4 = descriptor2.GetValue(obj2);
                            if (obj4 != null)
                            {
                                str2 = obj4.ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(str2) && !string.IsNullOrEmpty(binding.Value))
                        {
                            str2 = binding.Value;
                        }
                        string targetField = binding.TargetField;
                        if (targetField.Length > 0)
                        {
                            PropertyDescriptor descriptor3 = properties.Find(targetField, true);
                            if (descriptor3 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { targetField, "TargetField" }));
                            }
                            object obj5 = descriptor3.GetValue(obj2);
                            if (obj5 != null)
                            {
                                target = obj5.ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(target))
                        {
                            target = binding.Target;
                        }
                        string imageUrlField = binding.ImageUrlField;
                        if (imageUrlField.Length > 0)
                        {
                            PropertyDescriptor descriptor4 = properties.Find(imageUrlField, true);
                            if (descriptor4 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { imageUrlField, "ImageUrlField" }));
                            }
                            object obj6 = descriptor4.GetValue(obj2);
                            if (obj6 != null)
                            {
                                imageUrl = obj6.ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrl = binding.ImageUrl;
                        }
                        string navigateUrlField = binding.NavigateUrlField;
                        if (navigateUrlField.Length > 0)
                        {
                            PropertyDescriptor descriptor5 = properties.Find(navigateUrlField, true);
                            if (descriptor5 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { navigateUrlField, "NavigateUrlField" }));
                            }
                            object obj7 = descriptor5.GetValue(obj2);
                            if (obj7 != null)
                            {
                                navigateUrl = obj7.ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(navigateUrl))
                        {
                            navigateUrl = binding.NavigateUrl;
                        }
                        string popOutImageUrlField = binding.PopOutImageUrlField;
                        if (popOutImageUrlField.Length > 0)
                        {
                            PropertyDescriptor descriptor6 = properties.Find(popOutImageUrlField, true);
                            if (descriptor6 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { popOutImageUrlField, "PopOutImageUrlField" }));
                            }
                            object obj8 = descriptor6.GetValue(obj2);
                            if (obj8 != null)
                            {
                                popOutImageUrl = obj8.ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(popOutImageUrl))
                        {
                            popOutImageUrl = binding.PopOutImageUrl;
                        }
                        string separatorImageUrlField = binding.SeparatorImageUrlField;
                        if (separatorImageUrlField.Length > 0)
                        {
                            PropertyDescriptor descriptor7 = properties.Find(separatorImageUrlField, true);
                            if (descriptor7 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { separatorImageUrlField, "SeparatorImageUrlField" }));
                            }
                            object obj9 = descriptor7.GetValue(obj2);
                            if (obj9 != null)
                            {
                                separatorImageUrl = obj9.ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(separatorImageUrl))
                        {
                            separatorImageUrl = binding.SeparatorImageUrl;
                        }
                        string toolTipField = binding.ToolTipField;
                        if (toolTipField.Length > 0)
                        {
                            PropertyDescriptor descriptor8 = properties.Find(toolTipField, true);
                            if (descriptor8 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { toolTipField, "ToolTipField" }));
                            }
                            object obj10 = descriptor8.GetValue(obj2);
                            if (obj10 != null)
                            {
                                toolTip = obj10.ToString();
                            }
                        }
                        if (string.IsNullOrEmpty(toolTip))
                        {
                            toolTip = binding.ToolTip;
                        }
                        string enabledField = binding.EnabledField;
                        if (enabledField.Length > 0)
                        {
                            PropertyDescriptor descriptor9 = properties.Find(enabledField, true);
                            if (descriptor9 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { enabledField, "EnabledField" }));
                            }
                            object obj11 = descriptor9.GetValue(obj2);
                            if (obj11 != null)
                            {
                                if (obj11 is bool)
                                {
                                    result = (bool) obj11;
                                    flag2 = true;
                                }
                                else if (bool.TryParse(obj11.ToString(), out result))
                                {
                                    flag2 = true;
                                }
                            }
                        }
                        if (!flag2)
                        {
                            result = binding.Enabled;
                        }
                        string selectableField = binding.SelectableField;
                        if (selectableField.Length > 0)
                        {
                            PropertyDescriptor descriptor10 = properties.Find(selectableField, true);
                            if (descriptor10 == null)
                            {
                                throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDataBinding", new object[] { selectableField, "SelectableField" }));
                            }
                            object obj12 = descriptor10.GetValue(obj2);
                            if (obj12 != null)
                            {
                                if (obj12 is bool)
                                {
                                    selectable = (bool) obj12;
                                    flag4 = true;
                                }
                                else if (bool.TryParse(obj12.ToString(), out selectable))
                                {
                                    flag4 = true;
                                }
                            }
                        }
                        if (!flag4)
                        {
                            selectable = binding.Selectable;
                        }
                    }
                    else if (obj2 is INavigateUIData)
                    {
                        INavigateUIData data2 = (INavigateUIData) obj2;
                        text = data2.Name;
                        str2 = data2.Value;
                        navigateUrl = data2.NavigateUrl;
                        if (string.IsNullOrEmpty(navigateUrl))
                        {
                            selectable = false;
                        }
                        toolTip = data2.Description;
                    }
                    if (text == null)
                    {
                        text = obj2.ToString();
                    }
                    MenuItem child = null;
                    if ((text != null) || (str2 != null))
                    {
                        child = new MenuItem(text, str2, imageUrl, navigateUrl, target);
                    }
                    if (child != null)
                    {
                        if (toolTip.Length > 0)
                        {
                            child.ToolTip = toolTip;
                        }
                        if (popOutImageUrl.Length > 0)
                        {
                            child.PopOutImageUrl = popOutImageUrl;
                        }
                        if (separatorImageUrl.Length > 0)
                        {
                            child.SeparatorImageUrl = separatorImageUrl;
                        }
                        child.Enabled = result;
                        child.Selectable = selectable;
                        child.SetDataPath(hierarchyData.Path);
                        child.SetDataBound(true);
                        node.ChildItems.Add(child);
                        if (string.Equals(hierarchyData.Path, this._currentSiteMapNodeUrl, StringComparison.OrdinalIgnoreCase))
                        {
                            child.Selected = true;
                        }
                        child.SetDataItem(hierarchyData.Item);
                        this.OnMenuItemDataBound(new MenuEventArgs(child));
                        child.SetDataItem(null);
                        if (hierarchyData.HasChildren && (depth < this.MaximumDepth))
                        {
                            IHierarchicalEnumerable children = hierarchyData.GetChildren();
                            if (children != null)
                            {
                                this.DataBindRecursive(child, children);
                            }
                        }
                    }
                }
            }
        }

        protected override void EnsureDataBound()
        {
            base.EnsureDataBound();
            if (!this._subControlsDataBound)
            {
                foreach (Control control in this.Controls)
                {
                    control.DataBind();
                }
                this._subControlsDataBound = true;
            }
        }

        internal void EnsureRenderSettings()
        {
            if (this.Page != null)
            {
                if (this.Page.Header == null)
                {
                    if (this._staticHoverStyle != null)
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("NeedHeader", new object[] { "Menu.StaticHoverStyle" }));
                    }
                    if (this._dynamicHoverStyle != null)
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("NeedHeader", new object[] { "Menu.DynamicHoverStyle" }));
                    }
                }
                else
                {
                    this._isNotIE = this.Page.Request.Browser.MSDomVersion.Major < 4;
                    if (this.Page.SupportsStyleSheets || ((this.Page.ScriptManager != null) && this.Page.ScriptManager.IsInAsyncPostBack))
                    {
                        this._panelStyle = this.Panel.GetEmptyPopOutPanelStyle();
                        this.RegisterStyle(this._panelStyle);
                        this.RegisterStyle(this.RootMenuItemStyle);
                        this.RegisterStyle(base.ControlStyle);
                        if (this._staticItemStyle != null)
                        {
                            this._staticItemStyle.HyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(this._staticItemStyle.HyperLinkStyle);
                            this.RegisterStyle(this._staticItemStyle);
                        }
                        if (this._staticMenuStyle != null)
                        {
                            this.RegisterStyle(this._staticMenuStyle);
                        }
                        if (this._dynamicItemStyle != null)
                        {
                            this._dynamicItemStyle.HyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(this._dynamicItemStyle.HyperLinkStyle);
                            this.RegisterStyle(this._dynamicItemStyle);
                        }
                        if (this._dynamicMenuStyle != null)
                        {
                            this.RegisterStyle(this._dynamicMenuStyle);
                        }
                        foreach (MenuItemStyle style in this.LevelMenuItemStyles)
                        {
                            style.HyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(style.HyperLinkStyle);
                            this.RegisterStyle(style);
                        }
                        foreach (SubMenuStyle style2 in this.LevelSubMenuStyles)
                        {
                            this.RegisterStyle(style2);
                        }
                        if (this._staticSelectedStyle != null)
                        {
                            this._staticSelectedStyle.HyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(this._staticSelectedStyle.HyperLinkStyle);
                            this.RegisterStyle(this._staticSelectedStyle);
                        }
                        if (this._dynamicSelectedStyle != null)
                        {
                            this._dynamicSelectedStyle.HyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(this._dynamicSelectedStyle.HyperLinkStyle);
                            this.RegisterStyle(this._dynamicSelectedStyle);
                        }
                        foreach (MenuItemStyle style3 in this.LevelSelectedStyles)
                        {
                            style3.HyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(style3.HyperLinkStyle);
                            this.RegisterStyle(style3);
                        }
                        if (this._staticHoverStyle != null)
                        {
                            this._staticHoverHyperLinkStyle = new HyperLinkStyle(this._staticHoverStyle);
                            this._staticHoverHyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(this._staticHoverHyperLinkStyle);
                            this.RegisterStyle(this._staticHoverStyle);
                        }
                        if (this._dynamicHoverStyle != null)
                        {
                            this._dynamicHoverHyperLinkStyle = new HyperLinkStyle(this._dynamicHoverStyle);
                            this._dynamicHoverHyperLinkStyle.DoNotRenderDefaults = true;
                            this.RegisterStyle(this._dynamicHoverHyperLinkStyle);
                            this.RegisterStyle(this._dynamicHoverStyle);
                        }
                    }
                }
            }
        }

        private void EnsureRootMenuStyle()
        {
            if (this._rootMenuItemStyle == null)
            {
                this._rootMenuItemStyle = new Style();
                this._rootMenuItemStyle.Font.CopyFrom(this.Font);
                if (!this.ForeColor.IsEmpty)
                {
                    this._rootMenuItemStyle.ForeColor = this.ForeColor;
                }
                if (!base.ControlStyle.IsSet(0x2000))
                {
                    this._rootMenuItemStyle.Font.Underline = false;
                }
            }
        }

        public MenuItem FindItem(string valuePath)
        {
            if (valuePath == null)
            {
                return null;
            }
            return this.Items.FindItem(valuePath.Split(new char[] { this.PathSeparator }), 0);
        }

        internal string GetCssClassName(MenuItem item, bool hyperLink)
        {
            bool flag;
            return this.GetCssClassName(item, hyperLink, out flag);
        }

        internal string GetCssClassName(MenuItem item, bool hyperlink, out bool containsClassName)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            containsClassName = false;
            int depth = item.Depth;
            string str = CacheGetItem<string>(hyperlink ? this.CachedMenuItemHyperLinkClassNames : this.CachedMenuItemClassNames, depth);
            if (this.CachedLevelsContainingCssClass.Contains(depth))
            {
                containsClassName = true;
            }
            if (!item.Selected && (str != null))
            {
                return str;
            }
            StringBuilder builder = new StringBuilder();
            if (str != null)
            {
                if (!item.Selected)
                {
                    return str;
                }
                builder.Append(str);
                builder.Append(' ');
            }
            else
            {
                if (hyperlink)
                {
                    builder.Append(this.RootMenuItemStyle.RegisteredCssClass);
                    builder.Append(' ');
                }
                if (depth < this.StaticDisplayLevels)
                {
                    containsClassName |= AppendCssClassName(builder, this._staticItemStyle, hyperlink);
                }
                else
                {
                    containsClassName |= AppendCssClassName(builder, this._dynamicItemStyle, hyperlink);
                }
                if ((depth < this.LevelMenuItemStyles.Count) && (this.LevelMenuItemStyles[depth] != null))
                {
                    containsClassName |= AppendCssClassName(builder, this.LevelMenuItemStyles[depth], hyperlink);
                }
                str = builder.ToString().Trim();
                CacheSetItem<string>(hyperlink ? this.CachedMenuItemHyperLinkClassNames : this.CachedMenuItemClassNames, depth, str);
                if (containsClassName && !this.CachedLevelsContainingCssClass.Contains(depth))
                {
                    this.CachedLevelsContainingCssClass.Add(depth);
                }
            }
            if (!item.Selected)
            {
                return str;
            }
            if (depth < this.StaticDisplayLevels)
            {
                containsClassName |= AppendCssClassName(builder, this._staticSelectedStyle, hyperlink);
            }
            else
            {
                containsClassName |= AppendCssClassName(builder, this._dynamicSelectedStyle, hyperlink);
            }
            if ((depth < this.LevelSelectedStyles.Count) && (this.LevelSelectedStyles[depth] != null))
            {
                MenuItemStyle style = this.LevelSelectedStyles[depth];
                containsClassName |= AppendCssClassName(builder, style, hyperlink);
            }
            return builder.ToString().Trim();
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override IDictionary GetDesignModeState()
        {
            IDictionary designModeState = base.GetDesignModeState();
            this.CreateChildControls();
            foreach (Control control in this.Controls)
            {
                control.DataBind();
            }
            using (StringWriter writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                using (HtmlTextWriter writer2 = this.GetDesignTimeWriter(writer))
                {
                    this.RenderBeginTag(writer2);
                    this.RenderContents(writer2, true);
                    this.RenderEndTag(writer2, true);
                    designModeState["GetDesignTimeStaticHtml"] = writer.ToString();
                }
            }
            int staticDisplayLevels = this.StaticDisplayLevels;
            try
            {
                MenuItem oneDynamicItem = this.GetOneDynamicItem(this.RootItem);
                if (oneDynamicItem == null)
                {
                    this._dataBound = false;
                    this.StaticDisplayLevels = 1;
                    oneDynamicItem = new MenuItem();
                    oneDynamicItem.SetDepth(0);
                    oneDynamicItem.SetOwner(this);
                    string text = System.Web.SR.GetString("Menu_DesignTimeDummyItemText");
                    for (int i = 0; i < 5; i++)
                    {
                        MenuItem dataItem = new MenuItem(text);
                        if (this.DynamicItemTemplate != null)
                        {
                            MenuItemTemplateContainer container = new MenuItemTemplateContainer(i, dataItem);
                            dataItem.Container = container;
                            this.DynamicItemTemplate.InstantiateIn(container);
                            container.Site = base.Site;
                            container.DataBind();
                        }
                        oneDynamicItem.ChildItems.Add(dataItem);
                    }
                    oneDynamicItem.ChildItems[1].ChildItems.Add(new MenuItem());
                    this._cachedLevelsContainingCssClass = null;
                    this._cachedMenuItemStyles = null;
                    this._cachedSubMenuStyles = null;
                    this._cachedMenuItemClassNames = null;
                    this._cachedMenuItemHyperLinkClassNames = null;
                    this._cachedSubMenuClassNames = null;
                }
                else
                {
                    oneDynamicItem = oneDynamicItem.Parent;
                }
                using (StringWriter writer3 = new StringWriter(CultureInfo.CurrentCulture))
                {
                    using (HtmlTextWriter writer4 = this.GetDesignTimeWriter(writer3))
                    {
                        base.Attributes.AddAttributes(writer4);
                        writer4.RenderBeginTag(HtmlTextWriterTag.Table);
                        writer4.RenderBeginTag(HtmlTextWriterTag.Tr);
                        writer4.RenderBeginTag(HtmlTextWriterTag.Td);
                        oneDynamicItem.Render(writer4, true, false, false);
                        writer4.RenderEndTag();
                        writer4.RenderEndTag();
                        writer4.RenderEndTag();
                        designModeState["GetDesignTimeDynamicHtml"] = writer3.ToString();
                    }
                    return designModeState;
                }
            }
            finally
            {
                if (this.StaticDisplayLevels != staticDisplayLevels)
                {
                    this.StaticDisplayLevels = staticDisplayLevels;
                }
            }
            return designModeState;
        }

        private HtmlTextWriter GetDesignTimeWriter(StringWriter stringWriter)
        {
            if (this._designTimeTextWriterType == null)
            {
                return new HtmlTextWriter(stringWriter);
            }
            ConstructorInfo constructor = this._designTimeTextWriterType.GetConstructor(new Type[] { typeof(TextWriter) });
            if (constructor == null)
            {
                return new HtmlTextWriter(stringWriter);
            }
            return (HtmlTextWriter) constructor.Invoke(new object[] { stringWriter });
        }

        internal string GetImageUrl(int index)
        {
            if (this.ImageUrls[index] == null)
            {
                switch (index)
                {
                    case 0:
                        this.ImageUrls[index] = this.ScrollUpImageUrlInternal;
                        break;

                    case 1:
                        this.ImageUrls[index] = this.ScrollDownImageUrlInternal;
                        break;

                    case 2:
                        this.ImageUrls[index] = this.PopoutImageUrlInternal;
                        break;
                }
                this.ImageUrls[index] = base.ResolveClientUrl(this.ImageUrls[index]);
            }
            return this.ImageUrls[index];
        }

        internal MenuItemStyle GetMenuItemStyle(MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int depth = item.Depth;
            MenuItemStyle style = CacheGetItem<MenuItemStyle>(this.CachedMenuItemStyles, depth);
            if (!item.Selected && (style != null))
            {
                return style;
            }
            if (style == null)
            {
                style = new MenuItemStyle();
                style.CopyFrom(this.RootMenuItemStyle);
                if (depth < this.StaticDisplayLevels)
                {
                    if (this._staticItemStyle != null)
                    {
                        TreeView.GetMergedStyle(style, this._staticItemStyle);
                    }
                }
                else if ((depth >= this.StaticDisplayLevels) && (this._dynamicItemStyle != null))
                {
                    TreeView.GetMergedStyle(style, this._dynamicItemStyle);
                }
                if ((depth < this.LevelMenuItemStyles.Count) && (this.LevelMenuItemStyles[depth] != null))
                {
                    TreeView.GetMergedStyle(style, this.LevelMenuItemStyles[depth]);
                }
                CacheSetItem<MenuItemStyle>(this.CachedMenuItemStyles, depth, style);
            }
            if (!item.Selected)
            {
                return style;
            }
            MenuItemStyle style2 = new MenuItemStyle();
            style2.CopyFrom(style);
            if (depth < this.StaticDisplayLevels)
            {
                if (this._staticSelectedStyle != null)
                {
                    TreeView.GetMergedStyle(style2, this._staticSelectedStyle);
                }
            }
            else if ((depth >= this.StaticDisplayLevels) && (this._dynamicSelectedStyle != null))
            {
                TreeView.GetMergedStyle(style2, this._dynamicSelectedStyle);
            }
            if ((depth < this.LevelSelectedStyles.Count) && (this.LevelSelectedStyles[depth] != null))
            {
                TreeView.GetMergedStyle(style2, this.LevelSelectedStyles[depth]);
            }
            return style2;
        }

        private MenuItem GetOneDynamicItem(MenuItem item)
        {
            if (item.Depth >= this.StaticDisplayLevels)
            {
                return item;
            }
            for (int i = 0; i < item.ChildItems.Count; i++)
            {
                MenuItem oneDynamicItem = this.GetOneDynamicItem(item.ChildItems[i]);
                if (oneDynamicItem != null)
                {
                    return oneDynamicItem;
                }
            }
            return null;
        }

        internal string GetSubMenuCssClassName(MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = item.Depth + 1;
            string str = CacheGetItem<string>(this.CachedSubMenuClassNames, index);
            if (str == null)
            {
                StringBuilder builder = new StringBuilder();
                if (index < this.StaticDisplayLevels)
                {
                    AppendMenuCssClassName(builder, this._staticMenuStyle);
                }
                else
                {
                    SubMenuStyle style = this._panelStyle as SubMenuStyle;
                    if (style != null)
                    {
                        AppendMenuCssClassName(builder, style);
                    }
                    AppendMenuCssClassName(builder, this._dynamicMenuStyle);
                }
                if ((index < this.LevelSubMenuStyles.Count) && (this.LevelSubMenuStyles[index] != null))
                {
                    SubMenuStyle style2 = this.LevelSubMenuStyles[index];
                    AppendMenuCssClassName(builder, style2);
                }
                str = builder.ToString().Trim();
                CacheSetItem<string>(this.CachedSubMenuClassNames, index, str);
            }
            return str;
        }

        internal SubMenuStyle GetSubMenuStyle(MenuItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            int index = item.Depth + 1;
            SubMenuStyle style = CacheGetItem<SubMenuStyle>(this.CachedSubMenuStyles, index);
            if (style == null)
            {
                int staticDisplayLevels = this.StaticDisplayLevels;
                if ((index >= staticDisplayLevels) && !base.DesignMode)
                {
                    style = new PopOutPanel.PopOutPanelStyle(this.Panel);
                }
                else
                {
                    style = new SubMenuStyle();
                }
                if (index < staticDisplayLevels)
                {
                    if (this._staticMenuStyle != null)
                    {
                        style.CopyFrom(this._staticMenuStyle);
                    }
                }
                else if ((index >= staticDisplayLevels) && (this._dynamicMenuStyle != null))
                {
                    style.CopyFrom(this._dynamicMenuStyle);
                }
                if (((this._levelStyles != null) && (this._levelStyles.Count > index)) && (this._levelStyles[index] != null))
                {
                    TreeView.GetMergedStyle(style, this._levelStyles[index]);
                }
                CacheSetItem<SubMenuStyle>(this.CachedSubMenuStyles, index, style);
            }
            return style;
        }

        internal void InternalRaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.Length != 0)
            {
                string str = HttpUtility.HtmlDecode(eventArgument);
                int num = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if ((str[i] == '\\') && (++num >= this.MaximumDepth))
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDepth"));
                    }
                }
                MenuItem item = this.Items.FindItem(str.Split(new char[] { '\\' }), 0);
                if (item != null)
                {
                    this.OnMenuItemClick(new MenuEventArgs(item));
                }
            }
        }

        protected internal override void LoadControlState(object savedState)
        {
            Pair pair = savedState as Pair;
            if (pair == null)
            {
                base.LoadControlState(savedState);
            }
            else
            {
                base.LoadControlState(pair.First);
                this._selectedItem = null;
                if (pair.Second != null)
                {
                    string second = pair.Second as string;
                    if (second != null)
                    {
                        this._selectedItem = this.Items.FindItem(second.Split(new char[] { '\\' }), 0);
                    }
                }
            }
        }

        protected override void LoadViewState(object state)
        {
            if (state != null)
            {
                object[] objArray = (object[]) state;
                if (objArray[1] != null)
                {
                    ((IStateManager) this.StaticMenuItemStyle).LoadViewState(objArray[1]);
                }
                if (objArray[2] != null)
                {
                    ((IStateManager) this.StaticSelectedStyle).LoadViewState(objArray[2]);
                }
                if (objArray[3] != null)
                {
                    ((IStateManager) this.StaticHoverStyle).LoadViewState(objArray[3]);
                }
                if (objArray[4] != null)
                {
                    ((IStateManager) this.StaticMenuStyle).LoadViewState(objArray[4]);
                }
                if (objArray[5] != null)
                {
                    ((IStateManager) this.DynamicMenuItemStyle).LoadViewState(objArray[5]);
                }
                if (objArray[6] != null)
                {
                    ((IStateManager) this.DynamicSelectedStyle).LoadViewState(objArray[6]);
                }
                if (objArray[7] != null)
                {
                    ((IStateManager) this.DynamicHoverStyle).LoadViewState(objArray[7]);
                }
                if (objArray[8] != null)
                {
                    ((IStateManager) this.DynamicMenuStyle).LoadViewState(objArray[8]);
                }
                if (objArray[9] != null)
                {
                    ((IStateManager) this.LevelMenuItemStyles).LoadViewState(objArray[9]);
                }
                if (objArray[10] != null)
                {
                    ((IStateManager) this.LevelSelectedStyles).LoadViewState(objArray[10]);
                }
                if (objArray[11] != null)
                {
                    ((IStateManager) this.LevelSubMenuStyles).LoadViewState(objArray[11]);
                }
                if (objArray[12] != null)
                {
                    ((IStateManager) this.Items).LoadViewState(objArray[12]);
                    if (!string.IsNullOrEmpty(this.DataSourceID) || (this.DataSource != null))
                    {
                        this._dataBound = true;
                    }
                }
                if (objArray[0] != null)
                {
                    base.LoadViewState(objArray[0]);
                }
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            MenuEventArgs args = e as MenuEventArgs;
            if ((args != null) && StringUtil.EqualsIgnoreCase(args.CommandName, MenuItemClickCommandName))
            {
                if (base.IsEnabled)
                {
                    this.OnMenuItemClick(args);
                    if (base._adapter != null)
                    {
                        MenuAdapter adapter = base._adapter as MenuAdapter;
                        if (adapter != null)
                        {
                            MenuItem item = args.Item;
                            if (((item != null) && (item.ChildItems.Count > 0)) && ((item.Depth + 1) >= this.StaticDisplayLevels))
                            {
                                adapter.SetPath(args.Item.InternalValuePath);
                            }
                        }
                    }
                    base.RaiseBubbleEvent(this, e);
                }
                return true;
            }
            if (e is CommandEventArgs)
            {
                base.RaiseBubbleEvent(this, e);
                return true;
            }
            return false;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            this.EnsureChildControls();
            base.OnDataBinding(e);
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Page.RegisterRequiresControlState(this);
        }

        protected virtual void OnMenuItemClick(MenuEventArgs e)
        {
            this.SetSelectedItem(e.Item);
            MenuEventHandler handler = (MenuEventHandler) base.Events[_menuItemClickedEvent];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnMenuItemDataBound(MenuEventArgs e)
        {
            MenuEventHandler handler = (MenuEventHandler) base.Events[_menuItemDataBoundEvent];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            this.OnPreRender(e, base.IsEnabled);
        }

        internal void OnPreRender(EventArgs e, bool registerScript)
        {
            base.OnPreRender(e);
            this.EnsureRenderSettings();
            if ((this.Page != null) && registerScript)
            {
                this.Page.RegisterWebFormsScript();
                this.Page.ClientScript.RegisterClientScriptResource(this, typeof(Menu), "Menu.js");
                string clientDataObjectID = this.ClientDataObjectID;
                StringBuilder builder = new StringBuilder("var ");
                builder.Append(clientDataObjectID);
                builder.Append(" = new Object();\r\n");
                builder.Append(clientDataObjectID);
                builder.Append(".disappearAfter = ");
                builder.Append(this.DisappearAfter);
                builder.Append(";\r\n");
                builder.Append(clientDataObjectID);
                builder.Append(".horizontalOffset = ");
                builder.Append(this.DynamicHorizontalOffset);
                builder.Append(";\r\n");
                builder.Append(clientDataObjectID);
                builder.Append(".verticalOffset = ");
                builder.Append(this.DynamicVerticalOffset);
                builder.Append(";\r\n");
                if (this._dynamicHoverStyle != null)
                {
                    builder.Append(clientDataObjectID);
                    builder.Append(".hoverClass = '");
                    builder.Append(this._dynamicHoverStyle.RegisteredCssClass);
                    if (!string.IsNullOrEmpty(this._dynamicHoverStyle.CssClass))
                    {
                        if (!string.IsNullOrEmpty(this._dynamicHoverStyle.RegisteredCssClass))
                        {
                            builder.Append(' ');
                        }
                        builder.Append(this._dynamicHoverStyle.CssClass);
                    }
                    builder.Append("';\r\n");
                    if (this._dynamicHoverHyperLinkStyle != null)
                    {
                        builder.Append(clientDataObjectID);
                        builder.Append(".hoverHyperLinkClass = '");
                        builder.Append(this._dynamicHoverHyperLinkStyle.RegisteredCssClass);
                        if (!string.IsNullOrEmpty(this._dynamicHoverStyle.CssClass))
                        {
                            if (!string.IsNullOrEmpty(this._dynamicHoverHyperLinkStyle.RegisteredCssClass))
                            {
                                builder.Append(' ');
                            }
                            builder.Append(this._dynamicHoverStyle.CssClass);
                        }
                        builder.Append("';\r\n");
                    }
                }
                if ((this._staticHoverStyle != null) && (this._staticHoverHyperLinkStyle != null))
                {
                    builder.Append(clientDataObjectID);
                    builder.Append(".staticHoverClass = '");
                    builder.Append(this._staticHoverStyle.RegisteredCssClass);
                    if (!string.IsNullOrEmpty(this._staticHoverStyle.CssClass))
                    {
                        if (!string.IsNullOrEmpty(this._staticHoverStyle.RegisteredCssClass))
                        {
                            builder.Append(' ');
                        }
                        builder.Append(this._staticHoverStyle.CssClass);
                    }
                    builder.Append("';\r\n");
                    if (this._staticHoverHyperLinkStyle != null)
                    {
                        builder.Append(clientDataObjectID);
                        builder.Append(".staticHoverHyperLinkClass = '");
                        builder.Append(this._staticHoverHyperLinkStyle.RegisteredCssClass);
                        if (!string.IsNullOrEmpty(this._staticHoverStyle.CssClass))
                        {
                            if (!string.IsNullOrEmpty(this._staticHoverHyperLinkStyle.RegisteredCssClass))
                            {
                                builder.Append(' ');
                            }
                            builder.Append(this._staticHoverStyle.CssClass);
                        }
                        builder.Append("';\r\n");
                    }
                }
                if ((this.Page.RequestInternal != null) && string.Equals(this.Page.Request.Url.Scheme, "https", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Append(clientDataObjectID);
                    builder.Append(".iframeUrl = '");
                    builder.Append(Util.QuoteJScriptString(this.Page.ClientScript.GetWebResourceUrl(typeof(Menu), "SmartNav.htm"), false));
                    builder.Append("';\r\n");
                }
                this.Page.ClientScript.RegisterStartupScript(this, base.GetType(), this.ClientID + "_CreateDataObject", builder.ToString(), true);
            }
        }

        protected internal override void PerformDataBinding()
        {
            base.PerformDataBinding();
            this.DataBindItem(this.RootItem);
            if ((!base.DesignMode && this._dataBound) && (string.IsNullOrEmpty(this.DataSourceID) && (this.DataSource == null)))
            {
                this.Items.Clear();
                this.Controls.Clear();
                base.ClearChildViewState();
                this.TrackViewState();
                base.ChildControlsCreated = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(this.DataSourceID) || (this.DataSource != null))
                {
                    this.Controls.Clear();
                    base.ClearChildState();
                    this.TrackViewState();
                    this.CreateChildControlsFromItems(true);
                    base.ChildControlsCreated = true;
                    this._dataBound = true;
                }
                else if (!this._subControlsDataBound)
                {
                    foreach (Control control in this.Controls)
                    {
                        control.DataBind();
                    }
                }
                this._subControlsDataBound = true;
            }
        }

        protected internal virtual void RaisePostBackEvent(string eventArgument)
        {
            base.ValidateEvent(this.UniqueID, eventArgument);
            if (base.IsEnabled)
            {
                this.EnsureChildControls();
                if (base._adapter != null)
                {
                    IPostBackEventHandler handler = base._adapter as IPostBackEventHandler;
                    if (handler != null)
                    {
                        handler.RaisePostBackEvent(eventArgument);
                    }
                }
                else
                {
                    this.InternalRaisePostBackEvent(eventArgument);
                }
            }
        }

        private void RegisterStyle(Style style)
        {
            if ((this.Page != null) && this.Page.SupportsStyleSheets)
            {
                string cssClass = this.ClientID + "_" + this._cssStyleIndex++.ToString(NumberFormatInfo.InvariantInfo);
                this.Page.Header.StyleSheet.CreateStyleRule(style, this, "." + cssClass);
                style.SetRegisteredCssClass(cssClass);
            }
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            if (this.Page != null)
            {
                this.Page.VerifyRenderingInServerForm(this);
            }
            if (this.Items.Count > 0)
            {
                this.RenderBeginTag(writer);
                this.RenderContents(writer, false);
                this.RenderEndTag(writer, false);
            }
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            if ((this.SkipLinkText.Length != 0) && !base.DesignMode)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, '#' + this.ClientID + "_SkipLink");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, this.SkipLinkText);
                writer.AddAttribute(HtmlTextWriterAttribute.Src, base.SpacerImageUrl);
                writer.AddStyleAttribute(HtmlTextWriterStyle.BorderWidth, "0px");
                writer.AddAttribute(HtmlTextWriterAttribute.Width, "0");
                writer.AddAttribute(HtmlTextWriterAttribute.Height, "0");
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
                writer.RenderEndTag();
            }
            this.EnsureRootMenuStyle();
            if (this.Font != null)
            {
                this.Font.Reset();
            }
            this.ForeColor = Color.Empty;
            SubMenuStyle subMenuStyle = this.GetSubMenuStyle(this.RootItem);
            if ((this.Page != null) && this.Page.SupportsStyleSheets)
            {
                string subMenuCssClassName = this.GetSubMenuCssClassName(this.RootItem);
                if (subMenuCssClassName.Length > 0)
                {
                    if (this.CssClass.Length == 0)
                    {
                        this.CssClass = subMenuCssClassName;
                    }
                    else
                    {
                        this.CssClass = this.CssClass + ' ' + subMenuCssClassName;
                    }
                }
            }
            else if ((subMenuStyle != null) && !subMenuStyle.IsEmpty)
            {
                subMenuStyle.Font.Reset();
                subMenuStyle.ForeColor = Color.Empty;
                base.ControlStyle.CopyFrom(subMenuStyle);
            }
            this.AddAttributesToRender(writer);
            writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
            writer.AddAttribute(HtmlTextWriterAttribute.Border, "0");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);
        }

        protected internal override void RenderContents(HtmlTextWriter writer)
        {
            this.RenderContents(writer, false);
        }

        private void RenderContents(HtmlTextWriter writer, bool staticOnly)
        {
            if (this.Orientation == System.Web.UI.WebControls.Orientation.Horizontal)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
            }
            bool isEnabled = base.IsEnabled;
            if (this.StaticDisplayLevels > 1)
            {
                if (this.Orientation == System.Web.UI.WebControls.Orientation.Vertical)
                {
                    for (int i = 0; i < this.Items.Count; i++)
                    {
                        this.Items[i].RenderItem(writer, i, isEnabled, this.Orientation, staticOnly);
                        if (this.Items[i].ChildItems.Count != 0)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            this.Items[i].Render(writer, isEnabled, staticOnly);
                            writer.RenderEndTag();
                            writer.RenderEndTag();
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < this.Items.Count; j++)
                    {
                        this.Items[j].RenderItem(writer, j, isEnabled, this.Orientation, staticOnly);
                        if (this.Items[j].ChildItems.Count != 0)
                        {
                            writer.RenderBeginTag(HtmlTextWriterTag.Td);
                            this.Items[j].Render(writer, isEnabled, staticOnly);
                            writer.RenderEndTag();
                        }
                    }
                }
            }
            else
            {
                for (int k = 0; k < this.Items.Count; k++)
                {
                    this.Items[k].RenderItem(writer, k, isEnabled, this.Orientation, staticOnly);
                }
            }
            if (this.Orientation == System.Web.UI.WebControls.Orientation.Horizontal)
            {
                writer.RenderEndTag();
            }
            if (base.DesignMode)
            {
                if (this._dynamicItemStyle != null)
                {
                    this._dynamicItemStyle.ResetCachedStyles();
                }
                if (this._staticItemStyle != null)
                {
                    this._staticItemStyle.ResetCachedStyles();
                }
                if (this._dynamicSelectedStyle != null)
                {
                    this._dynamicSelectedStyle.ResetCachedStyles();
                }
                if (this._staticSelectedStyle != null)
                {
                    this._staticSelectedStyle.ResetCachedStyles();
                }
                if (this._staticHoverStyle != null)
                {
                    this._staticHoverHyperLinkStyle = new HyperLinkStyle(this._staticHoverStyle);
                }
                if (this._dynamicHoverStyle != null)
                {
                    this._dynamicHoverHyperLinkStyle = new HyperLinkStyle(this._dynamicHoverStyle);
                }
                foreach (MenuItemStyle style in this.LevelMenuItemStyles)
                {
                    style.ResetCachedStyles();
                }
                foreach (MenuItemStyle style2 in this.LevelSelectedStyles)
                {
                    style2.ResetCachedStyles();
                }
                if (this._imageUrls != null)
                {
                    for (int m = 0; m < this._imageUrls.Length; m++)
                    {
                        this._imageUrls[m] = null;
                    }
                }
                this._cachedPopOutImageUrl = null;
                this._cachedScrollDownImageUrl = null;
                this._cachedScrollUpImageUrl = null;
                this._cachedLevelsContainingCssClass = null;
                this._cachedMenuItemClassNames = null;
                this._cachedMenuItemHyperLinkClassNames = null;
                this._cachedMenuItemStyles = null;
                this._cachedSubMenuClassNames = null;
                this._cachedSubMenuStyles = null;
            }
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            this.RenderEndTag(writer, false);
        }

        private void RenderEndTag(HtmlTextWriter writer, bool staticOnly)
        {
            writer.RenderEndTag();
            if ((this.StaticDisplayLevels <= 1) && !staticOnly)
            {
                bool isEnabled = base.IsEnabled;
                for (int i = 0; i < this.Items.Count; i++)
                {
                    this.Items[i].Render(writer, isEnabled, staticOnly);
                }
            }
            if ((this.SkipLinkText.Length != 0) && !base.DesignMode)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID + "_SkipLink");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.RenderEndTag();
            }
        }

        protected internal override object SaveControlState()
        {
            object x = base.SaveControlState();
            if (this._selectedItem != null)
            {
                return new Pair(x, this._selectedItem.InternalValuePath);
            }
            return x;
        }

        protected override object SaveViewState()
        {
            object[] objArray = new object[13];
            objArray[0] = base.SaveViewState();
            bool flag = objArray[0] != null;
            if (this._staticItemStyle != null)
            {
                objArray[1] = ((IStateManager) this._staticItemStyle).SaveViewState();
                flag |= objArray[1] != null;
            }
            if (this._staticSelectedStyle != null)
            {
                objArray[2] = ((IStateManager) this._staticSelectedStyle).SaveViewState();
                flag |= objArray[2] != null;
            }
            if (this._staticHoverStyle != null)
            {
                objArray[3] = ((IStateManager) this._staticHoverStyle).SaveViewState();
                flag |= objArray[3] != null;
            }
            if (this._staticMenuStyle != null)
            {
                objArray[4] = ((IStateManager) this._staticMenuStyle).SaveViewState();
                flag |= objArray[4] != null;
            }
            if (this._dynamicItemStyle != null)
            {
                objArray[5] = ((IStateManager) this._dynamicItemStyle).SaveViewState();
                flag |= objArray[5] != null;
            }
            if (this._dynamicSelectedStyle != null)
            {
                objArray[6] = ((IStateManager) this._dynamicSelectedStyle).SaveViewState();
                flag |= objArray[6] != null;
            }
            if (this._dynamicHoverStyle != null)
            {
                objArray[7] = ((IStateManager) this._dynamicHoverStyle).SaveViewState();
                flag |= objArray[7] != null;
            }
            if (this._dynamicMenuStyle != null)
            {
                objArray[8] = ((IStateManager) this._dynamicMenuStyle).SaveViewState();
                flag |= objArray[8] != null;
            }
            if (this._levelMenuItemStyles != null)
            {
                objArray[9] = ((IStateManager) this._levelMenuItemStyles).SaveViewState();
                flag |= objArray[9] != null;
            }
            if (this._levelSelectedStyles != null)
            {
                objArray[10] = ((IStateManager) this._levelSelectedStyles).SaveViewState();
                flag |= objArray[10] != null;
            }
            if (this._levelStyles != null)
            {
                objArray[11] = ((IStateManager) this._levelStyles).SaveViewState();
                flag |= objArray[11] != null;
            }
            objArray[12] = ((IStateManager) this.Items).SaveViewState();
            if (flag | (objArray[12] != null))
            {
                return objArray;
            }
            return null;
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        protected override void SetDesignModeState(IDictionary data)
        {
            if (data.Contains("DesignTimeTextWriterType"))
            {
                Type type = data["DesignTimeTextWriterType"] as Type;
                if ((type != null) && type.IsSubclassOf(typeof(HtmlTextWriter)))
                {
                    this._designTimeTextWriterType = type;
                }
            }
            base.SetDesignModeState(data);
        }

        protected void SetItemDataBound(MenuItem node, bool dataBound)
        {
            node.SetDataBound(dataBound);
        }

        protected void SetItemDataItem(MenuItem node, object dataItem)
        {
            node.SetDataItem(dataItem);
        }

        protected void SetItemDataPath(MenuItem node, string dataPath)
        {
            node.SetDataPath(dataPath);
        }

        internal void SetSelectedItem(MenuItem node)
        {
            if (this._selectedItem != node)
            {
                if (node != null)
                {
                    if (node.Depth >= this.MaximumDepth)
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidDepth"));
                    }
                    if (!node.IsEnabledNoOwner || !node.Selectable)
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Menu_InvalidSelection"));
                    }
                }
                if ((this._selectedItem != null) && this._selectedItem.Selected)
                {
                    this._selectedItem.SetSelected(false);
                }
                this._selectedItem = node;
                if ((this._selectedItem != null) && !this._selectedItem.Selected)
                {
                    this._selectedItem.SetSelected(true);
                }
            }
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.RaisePostBackEvent(eventArgument);
        }

        protected override void TrackViewState()
        {
            base.TrackViewState();
            if (this._staticItemStyle != null)
            {
                ((IStateManager) this._staticItemStyle).TrackViewState();
            }
            if (this._staticSelectedStyle != null)
            {
                ((IStateManager) this._staticSelectedStyle).TrackViewState();
            }
            if (this._staticHoverStyle != null)
            {
                ((IStateManager) this._staticHoverStyle).TrackViewState();
            }
            if (this._staticMenuStyle != null)
            {
                ((IStateManager) this._staticMenuStyle).TrackViewState();
            }
            if (this._dynamicItemStyle != null)
            {
                ((IStateManager) this._dynamicItemStyle).TrackViewState();
            }
            if (this._dynamicSelectedStyle != null)
            {
                ((IStateManager) this._dynamicSelectedStyle).TrackViewState();
            }
            if (this._dynamicHoverStyle != null)
            {
                ((IStateManager) this._dynamicHoverStyle).TrackViewState();
            }
            if (this._dynamicMenuStyle != null)
            {
                ((IStateManager) this._dynamicMenuStyle).TrackViewState();
            }
            if (this._levelMenuItemStyles != null)
            {
                ((IStateManager) this._levelMenuItemStyles).TrackViewState();
            }
            if (this._levelSelectedStyles != null)
            {
                ((IStateManager) this._levelSelectedStyles).TrackViewState();
            }
            if (this._levelStyles != null)
            {
                ((IStateManager) this._levelStyles).TrackViewState();
            }
            if (this._bindings != null)
            {
                ((IStateManager) this._bindings).TrackViewState();
            }
            ((IStateManager) this.Items).TrackViewState();
        }

        internal bool AccessKeyRendered
        {
            get => 
                this._accessKeyRendered;
            set
            {
                this._accessKeyRendered = value;
            }
        }

        private Collection<int> CachedLevelsContainingCssClass
        {
            get
            {
                if (this._cachedLevelsContainingCssClass == null)
                {
                    this._cachedLevelsContainingCssClass = new Collection<int>();
                }
                return this._cachedLevelsContainingCssClass;
            }
        }

        private List<string> CachedMenuItemClassNames
        {
            get
            {
                if (this._cachedMenuItemClassNames == null)
                {
                    this._cachedMenuItemClassNames = new List<string>();
                }
                return this._cachedMenuItemClassNames;
            }
        }

        private List<string> CachedMenuItemHyperLinkClassNames
        {
            get
            {
                if (this._cachedMenuItemHyperLinkClassNames == null)
                {
                    this._cachedMenuItemHyperLinkClassNames = new List<string>();
                }
                return this._cachedMenuItemHyperLinkClassNames;
            }
        }

        private List<MenuItemStyle> CachedMenuItemStyles
        {
            get
            {
                if (this._cachedMenuItemStyles == null)
                {
                    this._cachedMenuItemStyles = new List<MenuItemStyle>();
                }
                return this._cachedMenuItemStyles;
            }
        }

        private List<string> CachedSubMenuClassNames
        {
            get
            {
                if (this._cachedSubMenuClassNames == null)
                {
                    this._cachedSubMenuClassNames = new List<string>();
                }
                return this._cachedSubMenuClassNames;
            }
        }

        private List<SubMenuStyle> CachedSubMenuStyles
        {
            get
            {
                if (this._cachedSubMenuStyles == null)
                {
                    this._cachedSubMenuStyles = new List<SubMenuStyle>();
                }
                return this._cachedSubMenuStyles;
            }
        }

        internal string ClientDataObjectID =>
            (this.ClientID + "_Data");

        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        [MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), Editor("System.Web.UI.Design.WebControls.MenuBindingsEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Data"), WebSysDescription("Menu_Bindings"), DefaultValue((string) null)]
        public MenuItemBindingCollection DataBindings
        {
            get
            {
                if (this._bindings == null)
                {
                    this._bindings = new MenuItemBindingCollection(this);
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._bindings).TrackViewState();
                    }
                }
                return this._bindings;
            }
        }

        [WebCategory("Behavior"), Themeable(false), DefaultValue(500), WebSysDescription("Menu_DisappearAfter")]
        public int DisappearAfter
        {
            get
            {
                object obj2 = this.ViewState["DisappearAfter"];
                if (obj2 == null)
                {
                    return 500;
                }
                return (int) obj2;
            }
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["DisappearAfter"] = value;
            }
        }

        [Themeable(true), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Appearance"), WebSysDescription("Menu_DynamicBottomSeparatorImageUrl"), UrlProperty, DefaultValue("")]
        public string DynamicBottomSeparatorImageUrl
        {
            get
            {
                object obj2 = this.ViewState["DynamicBottomSeparatorImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["DynamicBottomSeparatorImageUrl"] = value;
            }
        }

        [WebSysDescription("Menu_DynamicDisplayPopOutImage"), DefaultValue(true), WebCategory("Appearance")]
        public bool DynamicEnableDefaultPopOutImage
        {
            get
            {
                object obj2 = this.ViewState["DynamicEnableDefaultPopOutImage"];
                return ((obj2 == null) || ((bool) obj2));
            }
            set
            {
                this.ViewState["DynamicEnableDefaultPopOutImage"] = value;
            }
        }

        [WebSysDescription("Menu_DynamicHorizontalOffset"), WebCategory("Appearance"), DefaultValue(0)]
        public int DynamicHorizontalOffset
        {
            get
            {
                object obj2 = this.ViewState["DynamicHorizontalOffset"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                this.ViewState["DynamicHorizontalOffset"] = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Styles"), WebSysDescription("Menu_DynamicHoverStyle"), DefaultValue((string) null)]
        public Style DynamicHoverStyle
        {
            get
            {
                if (this._dynamicHoverStyle == null)
                {
                    this._dynamicHoverStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._dynamicHoverStyle).TrackViewState();
                    }
                }
                return this._dynamicHoverStyle;
            }
        }

        [WebSysDescription("Menu_DynamicItemFormatString"), DefaultValue(""), WebCategory("Appearance")]
        public string DynamicItemFormatString
        {
            get
            {
                object obj2 = this.ViewState["DynamicItemFormatString"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["DynamicItemFormatString"] = value;
            }
        }

        [Browsable(false), TemplateContainer(typeof(MenuItemTemplateContainer)), WebSysDescription("Menu_DynamicTemplate"), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate DynamicItemTemplate
        {
            get => 
                this._dynamicTemplate;
            set
            {
                this._dynamicTemplate = value;
            }
        }

        [NotifyParentProperty(true), WebSysDescription("Menu_DynamicMenuItemStyle"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Styles"), DefaultValue((string) null)]
        public MenuItemStyle DynamicMenuItemStyle
        {
            get
            {
                if (this._dynamicItemStyle == null)
                {
                    this._dynamicItemStyle = new MenuItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._dynamicItemStyle).TrackViewState();
                    }
                }
                return this._dynamicItemStyle;
            }
        }

        [WebSysDescription("Menu_DynamicMenuStyle"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string) null), WebCategory("Styles"), NotifyParentProperty(true)]
        public SubMenuStyle DynamicMenuStyle
        {
            get
            {
                if (this._dynamicMenuStyle == null)
                {
                    this._dynamicMenuStyle = new SubMenuStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._dynamicMenuStyle).TrackViewState();
                    }
                }
                return this._dynamicMenuStyle;
            }
        }

        [WebCategory("Appearance"), WebSysDefaultValue("MenuAdapter_Expand"), WebSysDescription("Menu_DynamicPopoutImageText")]
        public string DynamicPopOutImageTextFormatString
        {
            get
            {
                object obj2 = this.ViewState["DynamicPopOutImageTextFormatString"];
                if (obj2 == null)
                {
                    return System.Web.SR.GetString("MenuAdapter_Expand");
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["DynamicPopOutImageTextFormatString"] = value;
            }
        }

        [UrlProperty, WebSysDescription("Menu_DynamicPopoutImageUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), DefaultValue(""), WebCategory("Appearance")]
        public string DynamicPopOutImageUrl
        {
            get
            {
                object obj2 = this.ViewState["DynamicPopOutImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["DynamicPopOutImageUrl"] = value;
            }
        }

        [WebCategory("Styles"), WebSysDescription("Menu_DynamicSelectedStyle"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public MenuItemStyle DynamicSelectedStyle
        {
            get
            {
                if (this._dynamicSelectedStyle == null)
                {
                    this._dynamicSelectedStyle = new MenuItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._dynamicSelectedStyle).TrackViewState();
                    }
                }
                return this._dynamicSelectedStyle;
            }
        }

        [WebSysDescription("Menu_DynamicTopSeparatorImageUrl"), WebCategory("Appearance"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, DefaultValue("")]
        public string DynamicTopSeparatorImageUrl
        {
            get
            {
                object obj2 = this.ViewState["DynamicTopSeparatorImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["DynamicTopSeparatorImageUrl"] = value;
            }
        }

        [WebSysDescription("Menu_DynamicVerticalOffset"), DefaultValue(0), WebCategory("Appearance")]
        public int DynamicVerticalOffset
        {
            get
            {
                object obj2 = this.ViewState["DynamicVerticalOffset"];
                if (obj2 == null)
                {
                    return 0;
                }
                return (int) obj2;
            }
            set
            {
                this.ViewState["DynamicVerticalOffset"] = value;
            }
        }

        private string[] ImageUrls
        {
            get
            {
                if (this._imageUrls == null)
                {
                    this._imageUrls = new string[3];
                }
                return this._imageUrls;
            }
        }

        internal bool IsNotIE =>
            this._isNotIE;

        [WebSysDescription("Menu_Items"), DefaultValue((string) null), Editor("System.Web.UI.Design.WebControls.MenuItemCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false)]
        public MenuItemCollection Items =>
            this.RootItem.ChildItems;

        [DefaultValue(false), WebSysDescription("Menu_ItemWrap"), WebCategory("Appearance")]
        public bool ItemWrap
        {
            get
            {
                object obj2 = this.ViewState["ItemWrap"];
                if (obj2 == null)
                {
                    return false;
                }
                return (bool) obj2;
            }
            set
            {
                this.ViewState["ItemWrap"] = value;
            }
        }

        [DefaultValue((string) null), WebSysDescription("Menu_LevelMenuItemStyles"), Editor("System.Web.UI.Design.WebControls.MenuItemStyleCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), WebCategory("Styles")]
        public MenuItemStyleCollection LevelMenuItemStyles
        {
            get
            {
                if (this._levelMenuItemStyles == null)
                {
                    this._levelMenuItemStyles = new MenuItemStyleCollection();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._levelMenuItemStyles).TrackViewState();
                    }
                }
                return this._levelMenuItemStyles;
            }
        }

        [WebCategory("Styles"), WebSysDescription("Menu_LevelSelectedStyles"), Editor("System.Web.UI.Design.WebControls.MenuItemStyleCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null)]
        public MenuItemStyleCollection LevelSelectedStyles
        {
            get
            {
                if (this._levelSelectedStyles == null)
                {
                    this._levelSelectedStyles = new MenuItemStyleCollection();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._levelSelectedStyles).TrackViewState();
                    }
                }
                return this._levelSelectedStyles;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), Editor("System.Web.UI.Design.WebControls.SubMenuStyleCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), WebCategory("Styles"), WebSysDescription("Menu_LevelSubMenuStyles")]
        public SubMenuStyleCollection LevelSubMenuStyles
        {
            get
            {
                if (this._levelStyles == null)
                {
                    this._levelStyles = new SubMenuStyleCollection();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._levelStyles).TrackViewState();
                    }
                }
                return this._levelStyles;
            }
        }

        internal int MaximumDepth
        {
            get
            {
                if (this._maximumDepth <= 0)
                {
                    this._maximumDepth = this.MaximumDynamicDisplayLevels + this.StaticDisplayLevels;
                    if ((this._maximumDepth < this.MaximumDynamicDisplayLevels) || (this._maximumDepth < this.StaticDisplayLevels))
                    {
                        this._maximumDepth = 0x7fffffff;
                    }
                }
                return this._maximumDepth;
            }
        }

        [WebCategory("Behavior"), Themeable(true), DefaultValue(3), WebSysDescription("Menu_MaximumDynamicDisplayLevels")]
        public int MaximumDynamicDisplayLevels
        {
            get
            {
                object obj2 = this.ViewState["MaximumDynamicDisplayLevels"];
                if (obj2 == null)
                {
                    return 3;
                }
                return (int) obj2;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("MaximumDynamicDisplayLevels", System.Web.SR.GetString("Menu_MaximumDynamicDisplayLevelsInvalid"));
                }
                this.ViewState["MaximumDynamicDisplayLevels"] = value;
                this._maximumDepth = 0;
                if (this._dataBound)
                {
                    this._dataBound = false;
                    this.PerformDataBinding();
                }
            }
        }

        [WebSysDescription("Menu_Orientation"), WebCategory("Layout"), DefaultValue(1)]
        public System.Web.UI.WebControls.Orientation Orientation
        {
            get
            {
                object obj2 = this.ViewState["Orientation"];
                if (obj2 == null)
                {
                    return System.Web.UI.WebControls.Orientation.Vertical;
                }
                return (System.Web.UI.WebControls.Orientation) obj2;
            }
            set
            {
                this.ViewState["Orientation"] = value;
            }
        }

        internal PopOutPanel Panel
        {
            get
            {
                if (this._panel == null)
                {
                    this._panel = new PopOutPanel(this, this._panelStyle);
                    if (!base.DesignMode)
                    {
                        this._panel.Page = this.Page;
                    }
                }
                return this._panel;
            }
        }

        [DefaultValue('/'), WebSysDescription("Menu_PathSeparator")]
        public char PathSeparator
        {
            get
            {
                object obj2 = this.ViewState["PathSeparator"];
                if (obj2 == null)
                {
                    return '/';
                }
                return (char) obj2;
            }
            set
            {
                if (value == '\0')
                {
                    this.ViewState["PathSeparator"] = null;
                }
                else
                {
                    this.ViewState["PathSeparator"] = value;
                }
                foreach (MenuItem item in this.Items)
                {
                    item.ResetValuePathRecursive();
                }
            }
        }

        internal string PopoutImageUrlInternal
        {
            get
            {
                if (this._cachedPopOutImageUrl == null)
                {
                    this._cachedPopOutImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(Menu), "Menu_Popout.gif");
                }
                return this._cachedPopOutImageUrl;
            }
        }

        internal MenuItem RootItem
        {
            get
            {
                if (this._rootItem == null)
                {
                    this._rootItem = new MenuItem(this, true);
                }
                return this._rootItem;
            }
        }

        internal Style RootMenuItemStyle
        {
            get
            {
                this.EnsureRootMenuStyle();
                return this._rootMenuItemStyle;
            }
        }

        [WebSysDescription("Menu_ScrollDownImageUrl"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebCategory("Appearance")]
        public string ScrollDownImageUrl
        {
            get
            {
                object obj2 = this.ViewState["ScrollDownImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ScrollDownImageUrl"] = value;
            }
        }

        internal string ScrollDownImageUrlInternal
        {
            get
            {
                if (this._cachedScrollDownImageUrl == null)
                {
                    this._cachedScrollDownImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(Menu), "Menu_ScrollDown.gif");
                }
                return this._cachedScrollDownImageUrl;
            }
        }

        [WebSysDefaultValue("Menu_ScrollDown"), WebSysDescription("Menu_ScrollDownText"), Localizable(true), WebCategory("Appearance")]
        public string ScrollDownText
        {
            get
            {
                object obj2 = this.ViewState["ScrollDownText"];
                if (obj2 == null)
                {
                    return System.Web.SR.GetString("Menu_ScrollDown");
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ScrollDownText"] = value;
            }
        }

        [DefaultValue(""), WebSysDescription("Menu_ScrollUpImageUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebCategory("Appearance")]
        public string ScrollUpImageUrl
        {
            get
            {
                object obj2 = this.ViewState["ScrollUpImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ScrollUpImageUrl"] = value;
            }
        }

        internal string ScrollUpImageUrlInternal
        {
            get
            {
                if (this._cachedScrollUpImageUrl == null)
                {
                    this._cachedScrollUpImageUrl = this.Page.ClientScript.GetWebResourceUrl(typeof(Menu), "Menu_ScrollUp.gif");
                }
                return this._cachedScrollUpImageUrl;
            }
        }

        [WebSysDefaultValue("Menu_ScrollUp"), WebCategory("Appearance"), WebSysDescription("Menu_ScrollUpText"), Localizable(true)]
        public string ScrollUpText
        {
            get
            {
                object obj2 = this.ViewState["ScrollUpText"];
                if (obj2 == null)
                {
                    return System.Web.SR.GetString("Menu_ScrollUp");
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["ScrollUpText"] = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public MenuItem SelectedItem =>
            this._selectedItem;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), DefaultValue("")]
        public string SelectedValue
        {
            get
            {
                if (this.SelectedItem != null)
                {
                    return this.SelectedItem.Value;
                }
                return string.Empty;
            }
        }

        [WebCategory("Accessibility"), WebSysDefaultValue("Menu_SkipLinkTextDefault"), Localizable(true), WebSysDescription("WebControl_SkipLinkText")]
        public string SkipLinkText
        {
            get
            {
                object obj2 = this.ViewState["SkipLinkText"];
                if (obj2 == null)
                {
                    return System.Web.SR.GetString("Menu_SkipLinkTextDefault");
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["SkipLinkText"] = value;
            }
        }

        [DefaultValue(""), WebSysDescription("Menu_StaticBottomSeparatorImageUrl"), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebCategory("Appearance")]
        public string StaticBottomSeparatorImageUrl
        {
            get
            {
                object obj2 = this.ViewState["StaticBottomSeparatorImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["StaticBottomSeparatorImageUrl"] = value;
            }
        }

        [WebCategory("Behavior"), Themeable(true), DefaultValue(1), WebSysDescription("Menu_StaticDisplayLevels")]
        public int StaticDisplayLevels
        {
            get
            {
                object obj2 = this.ViewState["StaticDisplayLevels"];
                if (obj2 == null)
                {
                    return 1;
                }
                return (int) obj2;
            }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["StaticDisplayLevels"] = value;
                this._maximumDepth = 0;
                if (this._dataBound && !base.DesignMode)
                {
                    this._dataBound = false;
                    this.PerformDataBinding();
                }
            }
        }

        [WebSysDescription("Menu_StaticDisplayPopOutImage"), DefaultValue(true), WebCategory("Appearance")]
        public bool StaticEnableDefaultPopOutImage
        {
            get
            {
                object obj2 = this.ViewState["StaticEnableDefaultPopOutImage"];
                return ((obj2 == null) || ((bool) obj2));
            }
            set
            {
                this.ViewState["StaticEnableDefaultPopOutImage"] = value;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), WebSysDescription("Menu_StaticHoverStyle"), WebCategory("Styles")]
        public Style StaticHoverStyle
        {
            get
            {
                if (this._staticHoverStyle == null)
                {
                    this._staticHoverStyle = new Style();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._staticHoverStyle).TrackViewState();
                    }
                }
                return this._staticHoverStyle;
            }
        }

        [WebSysDescription("Menu_StaticItemFormatString"), DefaultValue(""), WebCategory("Appearance")]
        public string StaticItemFormatString
        {
            get
            {
                object obj2 = this.ViewState["StaticItemFormatString"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["StaticItemFormatString"] = value;
            }
        }

        [WebSysDescription("Menu_StaticTemplate"), Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(MenuItemTemplateContainer))]
        public ITemplate StaticItemTemplate
        {
            get => 
                this._staticTemplate;
            set
            {
                this._staticTemplate = value;
            }
        }

        [DefaultValue((string) null), WebCategory("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Menu_StaticMenuItemStyle")]
        public MenuItemStyle StaticMenuItemStyle
        {
            get
            {
                if (this._staticItemStyle == null)
                {
                    this._staticItemStyle = new MenuItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._staticItemStyle).TrackViewState();
                    }
                }
                return this._staticItemStyle;
            }
        }

        [NotifyParentProperty(true), WebCategory("Styles"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Menu_StaticMenuStyle")]
        public SubMenuStyle StaticMenuStyle
        {
            get
            {
                if (this._staticMenuStyle == null)
                {
                    this._staticMenuStyle = new SubMenuStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._staticMenuStyle).TrackViewState();
                    }
                }
                return this._staticMenuStyle;
            }
        }

        [WebSysDescription("Menu_StaticPopoutImageText"), WebSysDefaultValue("MenuAdapter_Expand"), WebCategory("Appearance")]
        public string StaticPopOutImageTextFormatString
        {
            get
            {
                object obj2 = this.ViewState["StaticPopOutImageTextFormatString"];
                if (obj2 == null)
                {
                    return System.Web.SR.GetString("MenuAdapter_Expand");
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["StaticPopOutImageTextFormatString"] = value;
            }
        }

        [DefaultValue(""), WebCategory("Appearance"), WebSysDescription("Menu_StaticPopoutImageUrl"), UrlProperty, Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string StaticPopOutImageUrl
        {
            get
            {
                object obj2 = this.ViewState["StaticPopOutImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["StaticPopOutImageUrl"] = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty), WebSysDescription("Menu_StaticSelectedStyle"), DefaultValue((string) null), NotifyParentProperty(true), WebCategory("Styles")]
        public MenuItemStyle StaticSelectedStyle
        {
            get
            {
                if (this._staticSelectedStyle == null)
                {
                    this._staticSelectedStyle = new MenuItemStyle();
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._staticSelectedStyle).TrackViewState();
                    }
                }
                return this._staticSelectedStyle;
            }
        }

        [WebSysDescription("Menu_StaticSubMenuIndent"), WebCategory("Appearance"), DefaultValue(typeof(Unit), "16px"), Themeable(true)]
        public Unit StaticSubMenuIndent
        {
            get
            {
                object obj2 = this.ViewState["StaticSubMenuIndent"];
                if (obj2 == null)
                {
                    return Unit.Pixel(0x10);
                }
                return (Unit) obj2;
            }
            set
            {
                if (value.Value < 0.0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.ViewState["StaticSubMenuIndent"] = value;
            }
        }

        [WebSysDescription("Menu_StaticTopSeparatorImageUrl"), DefaultValue(""), Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), UrlProperty, WebCategory("Appearance")]
        public string StaticTopSeparatorImageUrl
        {
            get
            {
                object obj2 = this.ViewState["StaticTopSeparatorImageUrl"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["StaticTopSeparatorImageUrl"] = value;
            }
        }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Table;

        [DefaultValue(""), WebSysDescription("MenuItem_Target")]
        public string Target
        {
            get
            {
                object obj2 = this.ViewState["Target"];
                if (obj2 == null)
                {
                    return string.Empty;
                }
                return (string) obj2;
            }
            set
            {
                this.ViewState["Target"] = value;
            }
        }
    }
}

