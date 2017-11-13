namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    [Designer("System.Web.UI.Design.WebControls.ListViewDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ToolboxBitmap(typeof(ListView), "ListView.ico"), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), ControlValueProperty("SelectedValue"), SupportsEventValidation, DefaultProperty("SelectedValue"), DefaultEvent("SelectedIndexChanged"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListView : DataBoundControl, INamingContainer, IPageableItemContainer, IPersistedSelector
    {
        private ITemplate _alternatingItemTemplate;
        private int _autoIDIndex;
        private const string _automaticIDPrefix = "ctrl";
        private OrderedDictionary _boundFieldValues;
        private DataKeyArray _dataKeyArray;
        private string[] _dataKeyNames;
        private ArrayList _dataKeysArrayList;
        private int _deletedItemIndex;
        private IOrderedDictionary _deleteKeys;
        private IOrderedDictionary _deleteValues;
        private int _editIndex = -1;
        private ITemplate _editItemTemplate;
        private ITemplate _emptyDataTemplate;
        private ITemplate _emptyItemTemplate;
        private int _groupItemCount = 1;
        private ITemplate _groupSeparatorTemplate;
        private Control _groupsGroupPlaceholderContainer;
        private int _groupsItemCreatedCount;
        private int _groupsOriginalIndexOfGroupPlaceholderInContainer = -1;
        private ITemplate _groupTemplate;
        private ListViewItem _insertItem;
        private ITemplate _insertItemTemplate;
        private IOrderedDictionary _insertValues;
        private bool _instantiatedEmptyDataTemplate;
        private IList<ListViewDataItem> _itemList;
        private ITemplate _itemSeparatorTemplate;
        private ITemplate _itemTemplate;
        private ITemplate _layoutTemplate;
        private int _maximumRows = -1;
        private string _modelValidationGroup;
        private int _noGroupsItemCreatedCount;
        private Control _noGroupsItemPlaceholderContainer;
        private int _noGroupsOriginalIndexOfItemPlaceholderInContainer = -1;
        private DataKey _persistedDataKey;
        private int _selectedIndex = -1;
        private ITemplate _selectedItemTemplate;
        private System.Web.UI.WebControls.SortDirection _sortDirection;
        private string _sortExpression = string.Empty;
        private int _startRowIndex;
        private int _totalRowCount = -1;
        private IOrderedDictionary _updateKeys;
        private IOrderedDictionary _updateNewValues;
        private IOrderedDictionary _updateOldValues;
        private static readonly object EventItemCanceling = new object();
        private static readonly object EventItemCommand = new object();
        private static readonly object EventItemCreated = new object();
        private static readonly object EventItemDataBound = new object();
        private static readonly object EventItemDeleted = new object();
        private static readonly object EventItemDeleting = new object();
        private static readonly object EventItemEditing = new object();
        private static readonly object EventItemInserted = new object();
        private static readonly object EventItemInserting = new object();
        private static readonly object EventItemUpdated = new object();
        private static readonly object EventItemUpdating = new object();
        private static readonly object EventLayoutCreated = new object();
        private static readonly object EventPagePropertiesChanged = new object();
        private static readonly object EventPagePropertiesChanging = new object();
        private static readonly object EventSelectedIndexChanged = new object();
        private static readonly object EventSelectedIndexChanging = new object();
        private static readonly object EventSorted = new object();
        private static readonly object EventSorting = new object();
        private static readonly object EventTotalRowCountAvailable = new object();
        internal const string ItemCountViewStateKey = "_!ItemCount";

        [Category("Action"), ResourceDescription("ListView_OnItemCanceling")]
        public event EventHandler<ListViewCancelEventArgs> ItemCanceling
        {
            add
            {
                base.Events.AddHandler(EventItemCanceling, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemCanceling, value);
            }
        }

        [ResourceDescription("ListView_OnItemCommand"), Category("Action")]
        public event EventHandler<ListViewCommandEventArgs> ItemCommand
        {
            add
            {
                base.Events.AddHandler(EventItemCommand, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemCommand, value);
            }
        }

        [Category("Behavior"), ResourceDescription("ListView_OnItemCreated")]
        public event EventHandler<ListViewItemEventArgs> ItemCreated
        {
            add
            {
                base.Events.AddHandler(EventItemCreated, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemCreated, value);
            }
        }

        [Category("Data"), ResourceDescription("ListView_OnItemDataBound")]
        public event EventHandler<ListViewItemEventArgs> ItemDataBound
        {
            add
            {
                base.Events.AddHandler(EventItemDataBound, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemDataBound, value);
            }
        }

        [Category("Action"), ResourceDescription("ListView_OnItemDeleted")]
        public event EventHandler<ListViewDeletedEventArgs> ItemDeleted
        {
            add
            {
                base.Events.AddHandler(EventItemDeleted, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemDeleted, value);
            }
        }

        [Category("Action"), ResourceDescription("ListView_OnItemDeleting")]
        public event EventHandler<ListViewDeleteEventArgs> ItemDeleting
        {
            add
            {
                base.Events.AddHandler(EventItemDeleting, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemDeleting, value);
            }
        }

        [ResourceDescription("ListView_OnItemEditing"), Category("Action")]
        public event EventHandler<ListViewEditEventArgs> ItemEditing
        {
            add
            {
                base.Events.AddHandler(EventItemEditing, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemEditing, value);
            }
        }

        [ResourceDescription("ListView_OnItemInserted"), Category("Action")]
        public event EventHandler<ListViewInsertedEventArgs> ItemInserted
        {
            add
            {
                base.Events.AddHandler(EventItemInserted, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemInserted, value);
            }
        }

        [ResourceDescription("ListView_OnItemInserting"), Category("Action")]
        public event EventHandler<ListViewInsertEventArgs> ItemInserting
        {
            add
            {
                base.Events.AddHandler(EventItemInserting, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemInserting, value);
            }
        }

        [Category("Action"), ResourceDescription("ListView_OnItemUpdated")]
        public event EventHandler<ListViewUpdatedEventArgs> ItemUpdated
        {
            add
            {
                base.Events.AddHandler(EventItemUpdated, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemUpdated, value);
            }
        }

        [ResourceDescription("ListView_OnItemUpdating"), Category("Action")]
        public event EventHandler<ListViewUpdateEventArgs> ItemUpdating
        {
            add
            {
                base.Events.AddHandler(EventItemUpdating, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventItemUpdating, value);
            }
        }

        [Category("Behavior"), ResourceDescription("ListView_OnLayoutCreated")]
        public event EventHandler LayoutCreated
        {
            add
            {
                base.Events.AddHandler(EventLayoutCreated, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventLayoutCreated, value);
            }
        }

        [ResourceDescription("ListView_OnPagePropertiesChanged"), Category("Behavior")]
        public event EventHandler PagePropertiesChanged
        {
            add
            {
                base.Events.AddHandler(EventPagePropertiesChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventPagePropertiesChanged, value);
            }
        }

        [Category("Behavior"), ResourceDescription("ListView_OnPagePropertiesChanging")]
        public event EventHandler<PagePropertiesChangingEventArgs> PagePropertiesChanging
        {
            add
            {
                base.Events.AddHandler(EventPagePropertiesChanging, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventPagePropertiesChanging, value);
            }
        }

        [Category("Action"), ResourceDescription("ListView_OnSelectedIndexChanged")]
        public event EventHandler SelectedIndexChanged
        {
            add
            {
                base.Events.AddHandler(EventSelectedIndexChanged, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSelectedIndexChanged, value);
            }
        }

        [ResourceDescription("ListView_OnSelectedIndexChanging"), Category("Action")]
        public event EventHandler<ListViewSelectEventArgs> SelectedIndexChanging
        {
            add
            {
                base.Events.AddHandler(EventSelectedIndexChanging, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSelectedIndexChanging, value);
            }
        }

        [Category("Action"), ResourceDescription("ListView_OnSorted")]
        public event EventHandler Sorted
        {
            add
            {
                base.Events.AddHandler(EventSorted, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSorted, value);
            }
        }

        [Category("Action"), ResourceDescription("ListView_OnSorting")]
        public event EventHandler<ListViewSortEventArgs> Sorting
        {
            add
            {
                base.Events.AddHandler(EventSorting, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventSorting, value);
            }
        }

        event EventHandler<PageEventArgs> IPageableItemContainer.TotalRowCountAvailable
        {
            add
            {
                base.Events.AddHandler(EventTotalRowCountAvailable, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventTotalRowCountAvailable, value);
            }
        }

        protected virtual void AddControlToContainer(Control control, Control container, int addLocation)
        {
            if (container is HtmlTable)
            {
                ListViewTableRow child = new ListViewTableRow();
                container.Controls.AddAt(addLocation, child);
                child.Controls.Add(control);
            }
            else if (container is HtmlTableRow)
            {
                ListViewTableCell cell = new ListViewTableCell();
                container.Controls.AddAt(addLocation, cell);
                cell.Controls.Add(control);
            }
            else
            {
                container.Controls.AddAt(addLocation, control);
            }
        }

        private void AutoIDControl(Control control)
        {
            control.ID = "ctrl" + this._autoIDIndex++.ToString(CultureInfo.InvariantCulture);
        }

        private void ClearDataKeys()
        {
            this._dataKeysArrayList = null;
        }

        protected internal override void CreateChildControls()
        {
            object obj2 = this.ViewState["_!ItemCount"];
            if ((obj2 == null) && base.RequiresDataBinding)
            {
                this.EnsureDataBound();
            }
            if ((obj2 != null) && (((int) obj2) != -1))
            {
                object[] dataSource = new object[(int) obj2];
                this.CreateChildControls(dataSource, false);
                base.ClearChildViewState();
            }
        }

        protected virtual int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            ListViewPagedDataSource source = null;
            this.EnsureLayoutTemplate();
            this.RemoveItems();
            if ((dataSource == null) && (this.InsertItemPosition != System.Web.UI.WebControls.InsertItemPosition.None))
            {
                dataSource = new object[0];
            }
            bool flag = (this._startRowIndex > 0) || (this._maximumRows > 0);
            if (dataBinding)
            {
                DataSourceView data = this.GetData();
                DataSourceSelectArguments selectArguments = base.SelectArguments;
                if (data == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NullView, new object[] { this.ID }));
                }
                bool flag2 = data.CanPage && flag;
                if ((!data.CanPage && flag2) && ((dataSource != null) && !(dataSource is ICollection)))
                {
                    selectArguments.StartRowIndex = this._startRowIndex;
                    selectArguments.MaximumRows = this._maximumRows;
                    data.Select(selectArguments, new DataSourceViewSelectCallback(this.SelectCallback));
                }
                if (flag2)
                {
                    int totalRowCount;
                    if (data.CanRetrieveTotalRowCount)
                    {
                        totalRowCount = selectArguments.TotalRowCount;
                    }
                    else
                    {
                        ICollection is2 = dataSource as ICollection;
                        if (is2 == null)
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NeedICollectionOrTotalRowCount, new object[] { base.GetType().Name }));
                        }
                        totalRowCount = this._startRowIndex + is2.Count;
                    }
                    source = this.CreateServerPagedDataSource(totalRowCount);
                }
                else
                {
                    source = this.CreatePagedDataSource();
                }
            }
            else
            {
                source = this.CreatePagedDataSource();
            }
            ArrayList dataKeysArrayList = this.DataKeysArrayList;
            this._dataKeyArray = null;
            ICollection is3 = dataSource as ICollection;
            if (dataBinding)
            {
                dataKeysArrayList.Clear();
                if (((dataSource != null) && (is3 == null)) && (!source.IsServerPagingEnabled && flag))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_Missing_VirtualItemCount, new object[] { this.ID }));
                }
            }
            else if (is3 == null)
            {
                throw new InvalidOperationException(AtlasWeb.ListView_DataSourceMustBeCollectionWhenNotDataBinding);
            }
            if (dataSource != null)
            {
                source.DataSource = dataSource;
                if (dataBinding && flag)
                {
                    dataKeysArrayList.Capacity = source.DataSourceCount;
                }
                if (this._groupTemplate != null)
                {
                    this._itemList = this.CreateItemsInGroups(source, dataBinding, this.InsertItemPosition, dataKeysArrayList);
                }
                else
                {
                    if (this.GroupItemCount != 1)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_GroupItemCountNoGroupTemplate, new object[] { this.ID, this.GroupPlaceholderID }));
                    }
                    this._itemList = this.CreateItemsWithoutGroups(source, dataBinding, this.InsertItemPosition, dataKeysArrayList);
                }
                this._totalRowCount = flag ? source.DataSourceCount : this._itemList.Count;
                this.OnTotalRowCountAvailable(new PageEventArgs(this._startRowIndex, this._maximumRows, this._totalRowCount));
                if ((this._itemList.Count == 0) && (this.InsertItemPosition == System.Web.UI.WebControls.InsertItemPosition.None))
                {
                    this.Controls.Clear();
                    this.CreateEmptyDataItem();
                }
            }
            else
            {
                this.Controls.Clear();
                this.CreateEmptyDataItem();
            }
            return this._totalRowCount;
        }

        protected override Style CreateControlStyle()
        {
            if (!base.DesignMode)
            {
                throw new NotSupportedException(AtlasWeb.ListView_StyleNotSupported);
            }
            return base.CreateControlStyle();
        }

        protected virtual ListViewDataItem CreateDataItem(int dataItemIndex, int displayIndex) => 
            new ListViewDataItem(dataItemIndex, displayIndex);

        protected override DataSourceSelectArguments CreateDataSourceSelectArguments()
        {
            DataSourceSelectArguments arguments = new DataSourceSelectArguments();
            DataSourceView data = this.GetData();
            bool canPage = data.CanPage;
            string sortExpressionInternal = this.SortExpressionInternal;
            if ((this.SortDirectionInternal == System.Web.UI.WebControls.SortDirection.Descending) && !string.IsNullOrEmpty(sortExpressionInternal))
            {
                sortExpressionInternal = sortExpressionInternal + " DESC";
            }
            arguments.SortExpression = sortExpressionInternal;
            if (canPage)
            {
                if (data.CanRetrieveTotalRowCount)
                {
                    arguments.RetrieveTotalRowCount = true;
                    arguments.MaximumRows = this._maximumRows;
                }
                else
                {
                    arguments.MaximumRows = -1;
                }
                arguments.StartRowIndex = this._startRowIndex;
            }
            return arguments;
        }

        protected virtual void CreateEmptyDataItem()
        {
            if (this._emptyDataTemplate != null)
            {
                this._instantiatedEmptyDataTemplate = true;
                ListViewItem control = this.CreateItem(ListViewItemType.EmptyItem);
                this.AutoIDControl(control);
                this.InstantiateEmptyDataTemplate(control);
                this.OnItemCreated(new ListViewItemEventArgs(control));
                this.AddControlToContainer(control, this, 0);
            }
        }

        protected virtual ListViewItem CreateEmptyItem()
        {
            if (this._emptyItemTemplate != null)
            {
                ListViewItem control = this.CreateItem(ListViewItemType.EmptyItem);
                this.AutoIDControl(control);
                this.InstantiateEmptyItemTemplate(control);
                this.OnItemCreated(new ListViewItemEventArgs(control));
                return control;
            }
            return null;
        }

        protected virtual ListViewItem CreateInsertItem()
        {
            if (this.InsertItemTemplate == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_InsertTemplateRequired, new object[] { this.ID }));
            }
            ListViewItem control = this.CreateItem(ListViewItemType.InsertItem);
            this.AutoIDControl(control);
            this.InstantiateInsertItemTemplate(control);
            this.OnItemCreated(new ListViewItemEventArgs(control));
            return control;
        }

        protected virtual ListViewItem CreateItem(ListViewItemType itemType)
        {
            ListViewItem item = new ListViewItem(itemType);
            if (itemType == ListViewItemType.InsertItem)
            {
                this._insertItem = item;
            }
            return item;
        }

        protected virtual IList<ListViewDataItem> CreateItemsInGroups(ListViewPagedDataSource dataSource, bool dataBinding, System.Web.UI.WebControls.InsertItemPosition insertPosition, ArrayList keyArray)
        {
            if (this._groupsOriginalIndexOfGroupPlaceholderInContainer == -1)
            {
                this._groupsGroupPlaceholderContainer = this.GetPreparedContainerInfo(this, false, out this._groupsOriginalIndexOfGroupPlaceholderInContainer);
            }
            int addLocation = this._groupsOriginalIndexOfGroupPlaceholderInContainer;
            this._groupsItemCreatedCount = 0;
            int placeholderIndex = 0;
            Control control = null;
            List<ListViewDataItem> list = new List<ListViewDataItem>();
            int num3 = 0;
            int displayIndex = 0;
            if (insertPosition == System.Web.UI.WebControls.InsertItemPosition.FirstItem)
            {
                ListViewContainer container = new ListViewContainer();
                this.AutoIDControl(container);
                this.InstantiateGroupTemplate(container);
                this.AddControlToContainer(container, this._groupsGroupPlaceholderContainer, addLocation);
                addLocation++;
                control = this.GetPreparedContainerInfo(container, true, out placeholderIndex);
                ListViewItem item = this.CreateInsertItem();
                this.AddControlToContainer(item, control, placeholderIndex);
                placeholderIndex++;
                num3++;
            }
            foreach (object obj2 in dataSource)
            {
                if ((num3 % this._groupItemCount) == 0)
                {
                    if ((num3 != 0) && (this._groupSeparatorTemplate != null))
                    {
                        ListViewContainer container2 = new ListViewContainer();
                        this.AutoIDControl(container2);
                        this.InstantiateGroupSeparatorTemplate(container2);
                        this.AddControlToContainer(container2, this._groupsGroupPlaceholderContainer, addLocation);
                        addLocation++;
                    }
                    ListViewContainer container3 = new ListViewContainer();
                    this.AutoIDControl(container3);
                    this.InstantiateGroupTemplate(container3);
                    this.AddControlToContainer(container3, this._groupsGroupPlaceholderContainer, addLocation);
                    addLocation++;
                    control = this.GetPreparedContainerInfo(container3, true, out placeholderIndex);
                }
                ListViewDataItem item2 = this.CreateDataItem(displayIndex + this.StartRowIndex, displayIndex);
                this.InstantiateItemTemplate(item2, displayIndex);
                if (dataBinding)
                {
                    item2.DataItem = obj2;
                    OrderedDictionary keyTable = new OrderedDictionary(this.DataKeyNamesInternal.Length);
                    foreach (string str in this.DataKeyNamesInternal)
                    {
                        object propertyValue = DataBinder.GetPropertyValue(obj2, str);
                        keyTable.Add(str, propertyValue);
                    }
                    if (keyArray.Count == displayIndex)
                    {
                        keyArray.Add(new DataKey(keyTable, this.DataKeyNamesInternal));
                    }
                    else
                    {
                        keyArray[displayIndex] = new DataKey(keyTable, this.DataKeyNamesInternal);
                    }
                }
                this.OnItemCreated(new ListViewItemEventArgs(item2));
                if (((num3 % this._groupItemCount) != 0) && (this._itemSeparatorTemplate != null))
                {
                    ListViewContainer container4 = new ListViewContainer();
                    this.InstantiateItemSeparatorTemplate(container4);
                    this.AddControlToContainer(container4, control, placeholderIndex);
                    placeholderIndex++;
                }
                this.AddControlToContainer(item2, control, placeholderIndex);
                placeholderIndex++;
                list.Add(item2);
                if (dataBinding)
                {
                    item2.DataBind();
                    this.OnItemDataBound(new ListViewItemEventArgs(item2));
                    item2.DataItem = null;
                }
                num3++;
                displayIndex++;
            }
            if (insertPosition == System.Web.UI.WebControls.InsertItemPosition.LastItem)
            {
                if ((num3 % this._groupItemCount) == 0)
                {
                    if ((num3 != 0) && (this._groupSeparatorTemplate != null))
                    {
                        ListViewContainer container5 = new ListViewContainer();
                        this.AutoIDControl(container5);
                        this.InstantiateGroupSeparatorTemplate(container5);
                        this.AddControlToContainer(container5, this._groupsGroupPlaceholderContainer, addLocation);
                        addLocation++;
                    }
                    ListViewContainer container6 = new ListViewContainer();
                    this.AutoIDControl(container6);
                    this.InstantiateGroupTemplate(container6);
                    this.AddControlToContainer(container6, this._groupsGroupPlaceholderContainer, addLocation);
                    addLocation++;
                    control = this.GetPreparedContainerInfo(container6, true, out placeholderIndex);
                }
                if (((num3 % this._groupItemCount) != 0) && (this._itemSeparatorTemplate != null))
                {
                    ListViewContainer container7 = new ListViewContainer();
                    this.InstantiateItemSeparatorTemplate(container7);
                    this.AddControlToContainer(container7, control, placeholderIndex);
                    placeholderIndex++;
                }
                ListViewItem item3 = this.CreateInsertItem();
                this.AddControlToContainer(item3, control, placeholderIndex);
                placeholderIndex++;
                num3++;
            }
            if (this._emptyItemTemplate != null)
            {
                while ((num3 % this._groupItemCount) != 0)
                {
                    if (this._itemSeparatorTemplate != null)
                    {
                        ListViewContainer container8 = new ListViewContainer();
                        this.InstantiateItemSeparatorTemplate(container8);
                        this.AddControlToContainer(container8, control, placeholderIndex);
                        placeholderIndex++;
                    }
                    ListViewItem item4 = this.CreateEmptyItem();
                    this.AddControlToContainer(item4, control, placeholderIndex);
                    placeholderIndex++;
                    num3++;
                }
            }
            this._groupsItemCreatedCount = addLocation - this._groupsOriginalIndexOfGroupPlaceholderInContainer;
            return list;
        }

        protected virtual IList<ListViewDataItem> CreateItemsWithoutGroups(ListViewPagedDataSource dataSource, bool dataBinding, System.Web.UI.WebControls.InsertItemPosition insertPosition, ArrayList keyArray)
        {
            if (this._noGroupsOriginalIndexOfItemPlaceholderInContainer == -1)
            {
                this._noGroupsItemPlaceholderContainer = this.GetPreparedContainerInfo(this, true, out this._noGroupsOriginalIndexOfItemPlaceholderInContainer);
            }
            int addLocation = this._noGroupsOriginalIndexOfItemPlaceholderInContainer;
            List<ListViewDataItem> list = new List<ListViewDataItem>();
            int num2 = 0;
            int displayIndex = 0;
            if (insertPosition == System.Web.UI.WebControls.InsertItemPosition.FirstItem)
            {
                ListViewItem control = this.CreateInsertItem();
                this.AddControlToContainer(control, this._noGroupsItemPlaceholderContainer, addLocation);
                addLocation++;
                num2++;
            }
            foreach (object obj2 in dataSource)
            {
                if ((num2 != 0) && (this._itemSeparatorTemplate != null))
                {
                    ListViewContainer container = new ListViewContainer();
                    this.AutoIDControl(container);
                    this.InstantiateItemSeparatorTemplate(container);
                    this.AddControlToContainer(container, this._noGroupsItemPlaceholderContainer, addLocation);
                    addLocation++;
                }
                ListViewDataItem item2 = this.CreateDataItem(displayIndex + dataSource.StartRowIndex, displayIndex);
                this.AutoIDControl(item2);
                this.InstantiateItemTemplate(item2, displayIndex);
                if (dataBinding)
                {
                    item2.DataItem = obj2;
                    OrderedDictionary keyTable = new OrderedDictionary(this.DataKeyNamesInternal.Length);
                    foreach (string str in this.DataKeyNamesInternal)
                    {
                        object propertyValue = DataBinder.GetPropertyValue(obj2, str);
                        keyTable.Add(str, propertyValue);
                    }
                    if (keyArray.Count == displayIndex)
                    {
                        keyArray.Add(new DataKey(keyTable, this.DataKeyNamesInternal));
                    }
                    else
                    {
                        keyArray[displayIndex] = new DataKey(keyTable, this.DataKeyNamesInternal);
                    }
                }
                this.OnItemCreated(new ListViewItemEventArgs(item2));
                this.AddControlToContainer(item2, this._noGroupsItemPlaceholderContainer, addLocation);
                addLocation++;
                list.Add(item2);
                if (dataBinding)
                {
                    item2.DataBind();
                    this.OnItemDataBound(new ListViewItemEventArgs(item2));
                    item2.DataItem = null;
                }
                displayIndex++;
                num2++;
            }
            if (insertPosition == System.Web.UI.WebControls.InsertItemPosition.LastItem)
            {
                if (this._itemSeparatorTemplate != null)
                {
                    ListViewContainer container2 = new ListViewContainer();
                    this.AutoIDControl(container2);
                    this.InstantiateItemSeparatorTemplate(container2);
                    this.AddControlToContainer(container2, this._noGroupsItemPlaceholderContainer, addLocation);
                    addLocation++;
                }
                ListViewItem item3 = this.CreateInsertItem();
                this.AddControlToContainer(item3, this._noGroupsItemPlaceholderContainer, addLocation);
                addLocation++;
                num2++;
            }
            this._noGroupsItemCreatedCount = addLocation - this._noGroupsOriginalIndexOfItemPlaceholderInContainer;
            return list;
        }

        protected virtual void CreateLayoutTemplate()
        {
            this._noGroupsOriginalIndexOfItemPlaceholderInContainer = -1;
            this._noGroupsItemCreatedCount = 0;
            this._noGroupsItemPlaceholderContainer = null;
            this._groupsOriginalIndexOfGroupPlaceholderInContainer = -1;
            this._groupsItemCreatedCount = 0;
            this._groupsGroupPlaceholderContainer = null;
            Control container = new Control();
            if (this._layoutTemplate != null)
            {
                this._layoutTemplate.InstantiateIn(container);
                this.Controls.Add(container);
            }
            this.OnLayoutCreated(new EventArgs());
        }

        private ListViewPagedDataSource CreatePagedDataSource() => 
            new ListViewPagedDataSource { 
                StartRowIndex = this._startRowIndex,
                MaximumRows = this._maximumRows,
                AllowServerPaging = false,
                TotalRowCount = 0
            };

        private ListViewPagedDataSource CreateServerPagedDataSource(int totalRowCount) => 
            new ListViewPagedDataSource { 
                StartRowIndex = this._startRowIndex,
                MaximumRows = this._maximumRows,
                AllowServerPaging = true,
                TotalRowCount = totalRowCount
            };

        public virtual void DeleteItem(int itemIndex)
        {
            this.HandleDelete(null, itemIndex);
        }

        protected virtual void EnsureLayoutTemplate()
        {
            if ((this.Controls.Count == 0) || this._instantiatedEmptyDataTemplate)
            {
                this.Controls.Clear();
                this.CreateLayoutTemplate();
            }
        }

        public virtual void ExtractItemValues(IOrderedDictionary itemValues, ListViewItem item, bool includePrimaryKey)
        {
            if (itemValues == null)
            {
                throw new ArgumentNullException("itemValues");
            }
            DataBoundControlHelper.ExtractValuesFromBindableControls(itemValues, item);
            IBindableTemplate editItemTemplate = null;
            if (item.ItemType == ListViewItemType.DataItem)
            {
                ListViewDataItem item2 = item as ListViewDataItem;
                if (item2 == null)
                {
                    throw new InvalidOperationException(AtlasWeb.ListView_ItemsNotDataItems);
                }
                if (item2.DisplayIndex == this.EditIndex)
                {
                    editItemTemplate = this.EditItemTemplate as IBindableTemplate;
                }
                else if (item2.DisplayIndex == this.SelectedIndex)
                {
                    editItemTemplate = this.SelectedItemTemplate as IBindableTemplate;
                }
                else if (((item2.DisplayIndex % 2) == 1) && (this.AlternatingItemTemplate != null))
                {
                    editItemTemplate = this.AlternatingItemTemplate as IBindableTemplate;
                }
                else
                {
                    editItemTemplate = this.ItemTemplate as IBindableTemplate;
                }
            }
            else if ((item.ItemType == ListViewItemType.InsertItem) && (this.InsertItemTemplate != null))
            {
                editItemTemplate = this.InsertItemTemplate as IBindableTemplate;
            }
            if (editItemTemplate != null)
            {
                OrderedDictionary dictionary = new OrderedDictionary();
                bool convertEmptyStringToNull = this.ConvertEmptyStringToNull;
                foreach (DictionaryEntry entry in editItemTemplate.ExtractValues(item))
                {
                    object obj2 = entry.Value;
                    if ((convertEmptyStringToNull && (obj2 is string)) && (((string) obj2).Length == 0))
                    {
                        dictionary[entry.Key] = null;
                    }
                    else
                    {
                        dictionary[entry.Key] = obj2;
                    }
                }
                foreach (DictionaryEntry entry2 in dictionary)
                {
                    if (includePrimaryKey || (Array.IndexOf<object>(this.DataKeyNamesInternal, entry2.Key) == -1))
                    {
                        itemValues[entry2.Key] = entry2.Value;
                    }
                }
            }
        }

        private DataPager FindDataPager(Control control)
        {
            foreach (Control control2 in control.Controls)
            {
                DataPager pager = control2 as DataPager;
                if (pager != null)
                {
                    return pager;
                }
            }
            foreach (Control control3 in control.Controls)
            {
                if (control3 is IPageableItemContainer)
                {
                    return null;
                }
                DataPager pager2 = this.FindDataPager(control3);
                if (pager2 != null)
                {
                    return pager2;
                }
            }
            return null;
        }

        protected virtual Control FindPlaceholder(string containerID, Control container) => 
            container.FindControl(containerID);

        private int GetItemIndex(ListViewItem item, string commandArgument)
        {
            if (item == null)
            {
                return Convert.ToInt32(commandArgument, CultureInfo.InvariantCulture);
            }
            ListViewDataItem item2 = item as ListViewDataItem;
            if (item2 != null)
            {
                return item2.DisplayIndex;
            }
            return -1;
        }

        private Control GetPreparedContainerInfo(Control outerContainer, bool isItem, out int placeholderIndex)
        {
            string containerID = isItem ? this.ItemPlaceholderID : this.GroupPlaceholderID;
            Control control = this.FindPlaceholder(containerID, outerContainer);
            if (control == null)
            {
                if (isItem)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NoItemPlaceholder, new object[] { this.ID, this.ItemPlaceholderID }));
                }
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NoGroupPlaceholder, new object[] { this.ID, this.GroupPlaceholderID }));
            }
            Control parent = control.Parent;
            placeholderIndex = parent.Controls.IndexOf(control);
            parent.Controls.Remove(control);
            return parent;
        }

        private void HandleCancel(int itemIndex)
        {
            ListViewCancelMode cancelingInsert = ListViewCancelMode.CancelingInsert;
            if ((itemIndex == this.EditIndex) && (itemIndex >= 0))
            {
                cancelingInsert = ListViewCancelMode.CancelingEdit;
            }
            else if (itemIndex != -1)
            {
                throw new InvalidOperationException(AtlasWeb.ListView_InvalidCancel);
            }
            ListViewCancelEventArgs e = new ListViewCancelEventArgs(itemIndex, cancelingInsert);
            this.OnItemCanceling(e);
            if (!e.Cancel)
            {
                if (base.IsBoundUsingDataSourceID && (e.CancelMode == ListViewCancelMode.CancelingEdit))
                {
                    this.EditIndex = -1;
                }
                base.RequiresDataBinding = true;
            }
        }

        private void HandleDelete(ListViewItem item, int itemIndex)
        {
            ListViewDataItem item2 = item as ListViewDataItem;
            if ((itemIndex < 0) && (item2 == null))
            {
                throw new InvalidOperationException(AtlasWeb.ListView_InvalidDelete);
            }
            DataSourceView data = null;
            bool isBoundUsingDataSourceID = base.IsBoundUsingDataSourceID;
            if (isBoundUsingDataSourceID)
            {
                data = this.GetData();
                if (data == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NullView, new object[] { this.ID }));
                }
            }
            if ((item == null) && (itemIndex < this.Items.Count))
            {
                item = this.Items[itemIndex];
            }
            ListViewDeleteEventArgs e = new ListViewDeleteEventArgs(itemIndex);
            if (isBoundUsingDataSourceID)
            {
                if (item != null)
                {
                    this.ExtractItemValues(e.Values, item, false);
                }
                if (this.DataKeys.Count > itemIndex)
                {
                    foreach (DictionaryEntry entry in this.DataKeys[itemIndex].Values)
                    {
                        e.Keys.Add(entry.Key, entry.Value);
                        if (e.Values.Contains(entry.Key))
                        {
                            e.Values.Remove(entry.Key);
                        }
                    }
                }
            }
            this.OnItemDeleting(e);
            if (!e.Cancel)
            {
                this._deletedItemIndex = itemIndex;
                if (isBoundUsingDataSourceID)
                {
                    this._deleteKeys = e.Keys;
                    this._deleteValues = e.Values;
                    data.Delete(e.Keys, e.Values, new DataSourceViewOperationCallback(this.HandleDeleteCallback));
                }
            }
        }

        private bool HandleDeleteCallback(int affectedRows, Exception ex)
        {
            ListViewDeletedEventArgs e = new ListViewDeletedEventArgs(affectedRows, ex);
            e.SetKeys(this._deleteKeys);
            e.SetValues(this._deleteValues);
            this.OnItemDeleted(e);
            this._deleteKeys = null;
            this._deleteValues = null;
            if (((ex != null) && !e.ExceptionHandled) && this.PageIsValidAfterModelException())
            {
                return false;
            }
            this.EditIndex = -1;
            if (((affectedRows > 0) && (this._totalRowCount > 0)) && ((this._deletedItemIndex == this.SelectedIndex) && ((this._deletedItemIndex + this._startRowIndex) == this._totalRowCount)))
            {
                this.SelectedIndex--;
            }
            this._deletedItemIndex = -1;
            base.RequiresDataBinding = true;
            return true;
        }

        private void HandleEdit(int itemIndex)
        {
            if (itemIndex < 0)
            {
                throw new InvalidOperationException(AtlasWeb.ListView_InvalidEdit);
            }
            ListViewEditEventArgs e = new ListViewEditEventArgs(itemIndex);
            this.OnItemEditing(e);
            if (!e.Cancel)
            {
                if (base.IsBoundUsingDataSourceID)
                {
                    this.EditIndex = e.NewEditIndex;
                }
                base.RequiresDataBinding = true;
            }
        }

        private bool HandleEvent(EventArgs e, bool causesValidation, string validationGroup)
        {
            bool flag = false;
            this._modelValidationGroup = null;
            if (causesValidation)
            {
                this.Page.Validate(validationGroup);
                if (this.EnableModelValidation)
                {
                    this._modelValidationGroup = validationGroup;
                }
            }
            ListViewCommandEventArgs args = e as ListViewCommandEventArgs;
            if (args != null)
            {
                this.OnItemCommand(args);
                flag = true;
                string commandName = args.CommandName;
                if (string.Equals(commandName, "Select", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleSelect(this.GetItemIndex(args.Item, (string) args.CommandArgument));
                    return flag;
                }
                if (string.Equals(commandName, "Sort", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleSort((string) args.CommandArgument);
                    return flag;
                }
                if (string.Equals(commandName, "Edit", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleEdit(this.GetItemIndex(args.Item, (string) args.CommandArgument));
                    return flag;
                }
                if (string.Equals(commandName, "Cancel", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleCancel(this.GetItemIndex(args.Item, (string) args.CommandArgument));
                    return flag;
                }
                if (string.Equals(commandName, "Update", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleUpdate(args.Item, this.GetItemIndex(args.Item, (string) args.CommandArgument), causesValidation);
                    return flag;
                }
                if (string.Equals(commandName, "Delete", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleDelete(args.Item, this.GetItemIndex(args.Item, (string) args.CommandArgument));
                    return flag;
                }
                if (string.Equals(commandName, "Insert", StringComparison.OrdinalIgnoreCase))
                {
                    this.HandleInsert(args.Item, causesValidation);
                }
            }
            return flag;
        }

        private void HandleInsert(ListViewItem item, bool causesValidation)
        {
            if ((item != null) && (item.ItemType != ListViewItemType.InsertItem))
            {
                throw new InvalidOperationException(AtlasWeb.ListView_InvalidInsert);
            }
            if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
            {
                if (item == null)
                {
                    item = this._insertItem;
                }
                if (item == null)
                {
                    throw new InvalidOperationException(AtlasWeb.ListView_NoInsertItem);
                }
                DataSourceView data = null;
                bool isBoundUsingDataSourceID = base.IsBoundUsingDataSourceID;
                if (isBoundUsingDataSourceID)
                {
                    data = this.GetData();
                    if (data == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NullView, new object[] { this.ID }));
                    }
                }
                ListViewInsertEventArgs e = new ListViewInsertEventArgs(item);
                if (isBoundUsingDataSourceID)
                {
                    this.ExtractItemValues(e.Values, item, true);
                }
                this.OnItemInserting(e);
                if (!e.Cancel && isBoundUsingDataSourceID)
                {
                    this._insertValues = e.Values;
                    data.Insert(e.Values, new DataSourceViewOperationCallback(this.HandleInsertCallback));
                }
            }
        }

        private bool HandleInsertCallback(int affectedRows, Exception ex)
        {
            ListViewInsertedEventArgs e = new ListViewInsertedEventArgs(affectedRows, ex);
            e.SetValues(this._insertValues);
            this.OnItemInserted(e);
            this._insertValues = null;
            if ((ex != null) && !e.ExceptionHandled)
            {
                if (this.PageIsValidAfterModelException())
                {
                    return false;
                }
                e.KeepInInsertMode = true;
            }
            if (!e.KeepInInsertMode)
            {
                base.RequiresDataBinding = true;
            }
            return true;
        }

        private void HandleSelect(int itemIndex)
        {
            if (itemIndex < 0)
            {
                throw new InvalidOperationException(AtlasWeb.ListView_InvalidSelect);
            }
            ListViewSelectEventArgs e = new ListViewSelectEventArgs(itemIndex);
            this.OnSelectedIndexChanging(e);
            if (!e.Cancel)
            {
                if (base.IsBoundUsingDataSourceID)
                {
                    this.SelectedIndex = e.NewSelectedIndex;
                }
                this.OnSelectedIndexChanged(EventArgs.Empty);
                base.RequiresDataBinding = true;
            }
        }

        private void HandleSort(string sortExpression)
        {
            System.Web.UI.WebControls.SortDirection ascending = System.Web.UI.WebControls.SortDirection.Ascending;
            if ((this.SortExpressionInternal == sortExpression) && (this.SortDirectionInternal == System.Web.UI.WebControls.SortDirection.Ascending))
            {
                ascending = System.Web.UI.WebControls.SortDirection.Descending;
            }
            this.HandleSort(sortExpression, ascending);
        }

        private void HandleSort(string sortExpression, System.Web.UI.WebControls.SortDirection sortDirection)
        {
            ListViewSortEventArgs e = new ListViewSortEventArgs(sortExpression, sortDirection);
            this.OnSorting(e);
            if (!e.Cancel)
            {
                if (base.IsBoundUsingDataSourceID)
                {
                    this.ClearDataKeys();
                    if (this.GetData() == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NullView, new object[] { this.ID }));
                    }
                    this.EditIndex = -1;
                    this.SortExpressionInternal = e.SortExpression;
                    this.SortDirectionInternal = e.SortDirection;
                    this._startRowIndex = 0;
                }
                this.OnSorted(EventArgs.Empty);
                base.RequiresDataBinding = true;
            }
        }

        private void HandleUpdate(ListViewItem item, int itemIndex, bool causesValidation)
        {
            ListViewDataItem item2 = item as ListViewDataItem;
            if ((itemIndex < 0) && (item2 == null))
            {
                throw new InvalidOperationException(AtlasWeb.ListView_InvalidUpdate);
            }
            if ((!causesValidation || (this.Page == null)) || this.Page.IsValid)
            {
                DataSourceView data = null;
                bool isBoundUsingDataSourceID = base.IsBoundUsingDataSourceID;
                if (isBoundUsingDataSourceID)
                {
                    data = this.GetData();
                    if (data == null)
                    {
                        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_NullView, new object[] { this.ID }));
                    }
                }
                ListViewUpdateEventArgs e = new ListViewUpdateEventArgs(itemIndex);
                if (isBoundUsingDataSourceID)
                {
                    foreach (DictionaryEntry entry in this.BoundFieldValues)
                    {
                        e.OldValues.Add(entry.Key, entry.Value);
                    }
                    if (this.DataKeys.Count > itemIndex)
                    {
                        foreach (DictionaryEntry entry2 in this.DataKeys[itemIndex].Values)
                        {
                            e.Keys.Add(entry2.Key, entry2.Value);
                        }
                    }
                    if ((item2 == null) && (this.Items.Count > itemIndex))
                    {
                        item2 = this.Items[itemIndex];
                    }
                    if (item2 != null)
                    {
                        this.ExtractItemValues(e.NewValues, item2, true);
                    }
                }
                this.OnItemUpdating(e);
                if (!e.Cancel && isBoundUsingDataSourceID)
                {
                    this._updateKeys = e.Keys;
                    this._updateOldValues = e.OldValues;
                    this._updateNewValues = e.NewValues;
                    data.Update(e.Keys, e.NewValues, e.OldValues, new DataSourceViewOperationCallback(this.HandleUpdateCallback));
                }
            }
        }

        private bool HandleUpdateCallback(int affectedRows, Exception ex)
        {
            ListViewUpdatedEventArgs e = new ListViewUpdatedEventArgs(affectedRows, ex);
            e.SetKeys(this._updateKeys);
            e.SetOldValues(this._updateOldValues);
            e.SetNewValues(this._updateNewValues);
            this.OnItemUpdated(e);
            this._updateKeys = null;
            this._updateOldValues = null;
            this._updateNewValues = null;
            if ((ex != null) && !e.ExceptionHandled)
            {
                if (this.PageIsValidAfterModelException())
                {
                    return false;
                }
                e.KeepInEditMode = true;
            }
            if (!e.KeepInEditMode)
            {
                this.EditIndex = -1;
                base.RequiresDataBinding = true;
            }
            return true;
        }

        public virtual void InsertNewItem(bool causesValidation)
        {
            this.HandleInsert(null, causesValidation);
        }

        protected virtual void InstantiateEmptyDataTemplate(Control container)
        {
            if (this._emptyDataTemplate != null)
            {
                this._emptyDataTemplate.InstantiateIn(container);
            }
        }

        protected virtual void InstantiateEmptyItemTemplate(Control container)
        {
            if (this._emptyItemTemplate != null)
            {
                this._emptyItemTemplate.InstantiateIn(container);
            }
        }

        protected virtual void InstantiateGroupSeparatorTemplate(Control container)
        {
            if (this._groupSeparatorTemplate != null)
            {
                this._groupSeparatorTemplate.InstantiateIn(container);
            }
        }

        protected virtual void InstantiateGroupTemplate(Control container)
        {
            if (this._groupTemplate != null)
            {
                this._groupTemplate.InstantiateIn(container);
            }
        }

        protected virtual void InstantiateInsertItemTemplate(Control container)
        {
            if (this._insertItemTemplate != null)
            {
                this._insertItemTemplate.InstantiateIn(container);
            }
        }

        protected virtual void InstantiateItemSeparatorTemplate(Control container)
        {
            if (this._itemSeparatorTemplate != null)
            {
                this._itemSeparatorTemplate.InstantiateIn(container);
            }
        }

        protected virtual void InstantiateItemTemplate(Control container, int displayIndex)
        {
            ITemplate template = this._itemTemplate;
            if (template == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_ItemTemplateRequired, new object[] { this.ID }));
            }
            if (((displayIndex % 2) == 1) && (this._alternatingItemTemplate != null))
            {
                template = this._alternatingItemTemplate;
            }
            if ((displayIndex == this._selectedIndex) && (this._selectedItemTemplate != null))
            {
                template = this._selectedItemTemplate;
            }
            if ((displayIndex == this._editIndex) && (this._editItemTemplate != null))
            {
                template = this._editItemTemplate;
            }
            template.InstantiateIn(container);
        }

        protected internal override void LoadControlState(object savedState)
        {
            this._editIndex = -1;
            this._selectedIndex = -1;
            this._groupItemCount = 1;
            this._sortExpression = string.Empty;
            this._sortDirection = System.Web.UI.WebControls.SortDirection.Ascending;
            this._dataKeyNames = new string[0];
            object[] objArray = savedState as object[];
            if (objArray != null)
            {
                base.LoadControlState(objArray[0]);
                if (objArray[1] != null)
                {
                    this._editIndex = (int) objArray[1];
                }
                if (objArray[2] != null)
                {
                    this._selectedIndex = (int) objArray[2];
                }
                if (objArray[3] != null)
                {
                    this._groupItemCount = (int) objArray[3];
                }
                if (objArray[4] != null)
                {
                    this._sortExpression = (string) objArray[4];
                }
                if (objArray[5] != null)
                {
                    this._sortDirection = (System.Web.UI.WebControls.SortDirection) objArray[5];
                }
                if (objArray[6] != null)
                {
                    this._dataKeyNames = (string[]) objArray[6];
                }
                if (objArray[7] != null)
                {
                    this.LoadDataKeysState(objArray[7]);
                }
                if (objArray[8] != null)
                {
                    this._totalRowCount = (int) objArray[8];
                }
                if (((objArray[9] != null) && (this._dataKeyNames != null)) && (this._dataKeyNames.Length > 0))
                {
                    this._persistedDataKey = new DataKey(new OrderedDictionary(this._dataKeyNames.Length), this._dataKeyNames);
                    ((IStateManager) this._persistedDataKey).LoadViewState(objArray[9]);
                }
            }
            else
            {
                base.LoadControlState(null);
            }
            if (!base.IsViewStateEnabled)
            {
                this.OnTotalRowCountAvailable(new PageEventArgs(this._startRowIndex, this._maximumRows, this._totalRowCount));
            }
        }

        private void LoadDataKeysState(object state)
        {
            if (state != null)
            {
                object[] objArray = (object[]) state;
                string[] dataKeyNamesInternal = this.DataKeyNamesInternal;
                int length = dataKeyNamesInternal.Length;
                this.ClearDataKeys();
                for (int i = 0; i < objArray.Length; i++)
                {
                    this.DataKeysArrayList.Add(new DataKey(new OrderedDictionary(length), dataKeyNamesInternal));
                    ((IStateManager) this.DataKeysArrayList[i]).LoadViewState(objArray[i]);
                }
            }
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] objArray = (object[]) savedState;
                base.LoadViewState(objArray[0]);
                if (objArray[1] != null)
                {
                    OrderedDictionaryStateHelper.LoadViewState((OrderedDictionary) this.BoundFieldValues, (ArrayList) objArray[1]);
                }
            }
            else
            {
                base.LoadViewState(savedState);
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs e)
        {
            bool causesValidation = false;
            string validationGroup = string.Empty;
            ListViewCommandEventArgs args = e as ListViewCommandEventArgs;
            if ((args == null) && (e is CommandEventArgs))
            {
                args = new ListViewCommandEventArgs(new ListViewItem(ListViewItemType.EmptyItem), source, (CommandEventArgs) e);
            }
            if (args != null)
            {
                IButtonControl commandSource = args.CommandSource as IButtonControl;
                if (commandSource != null)
                {
                    causesValidation = commandSource.CausesValidation;
                    validationGroup = commandSource.ValidationGroup;
                }
            }
            return this.HandleEvent(args, causesValidation, validationGroup);
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (this.Page != null)
            {
                if (this.DataKeyNames.Length > 0)
                {
                    this.Page.RegisterRequiresViewStateEncryption();
                }
                this.Page.RegisterRequiresControlState(this);
            }
        }

        protected virtual void OnItemCanceling(ListViewCancelEventArgs e)
        {
            EventHandler<ListViewCancelEventArgs> handler = (EventHandler<ListViewCancelEventArgs>) base.Events[EventItemCanceling];
            if (handler != null)
            {
                handler(this, e);
            }
            else if (!base.IsBoundUsingDataSourceID && !e.Cancel)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_UnhandledEvent, new object[] { this.ID, "ItemCanceling" }));
            }
        }

        protected virtual void OnItemCommand(ListViewCommandEventArgs e)
        {
            EventHandler<ListViewCommandEventArgs> handler = (EventHandler<ListViewCommandEventArgs>) base.Events[EventItemCommand];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemCreated(ListViewItemEventArgs e)
        {
            EventHandler<ListViewItemEventArgs> handler = (EventHandler<ListViewItemEventArgs>) base.Events[EventItemCreated];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemDataBound(ListViewItemEventArgs e)
        {
            EventHandler<ListViewItemEventArgs> handler = (EventHandler<ListViewItemEventArgs>) base.Events[EventItemDataBound];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemDeleted(ListViewDeletedEventArgs e)
        {
            EventHandler<ListViewDeletedEventArgs> handler = (EventHandler<ListViewDeletedEventArgs>) base.Events[EventItemDeleted];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemDeleting(ListViewDeleteEventArgs e)
        {
            EventHandler<ListViewDeleteEventArgs> handler = (EventHandler<ListViewDeleteEventArgs>) base.Events[EventItemDeleting];
            if (handler != null)
            {
                handler(this, e);
            }
            else if (!base.IsBoundUsingDataSourceID && !e.Cancel)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_UnhandledEvent, new object[] { this.ID, "ItemDeleting" }));
            }
        }

        protected virtual void OnItemEditing(ListViewEditEventArgs e)
        {
            EventHandler<ListViewEditEventArgs> handler = (EventHandler<ListViewEditEventArgs>) base.Events[EventItemEditing];
            if (handler != null)
            {
                handler(this, e);
            }
            else if (!base.IsBoundUsingDataSourceID && !e.Cancel)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_UnhandledEvent, new object[] { this.ID, "ItemEditing" }));
            }
        }

        protected virtual void OnItemInserted(ListViewInsertedEventArgs e)
        {
            EventHandler<ListViewInsertedEventArgs> handler = (EventHandler<ListViewInsertedEventArgs>) base.Events[EventItemInserted];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemInserting(ListViewInsertEventArgs e)
        {
            EventHandler<ListViewInsertEventArgs> handler = (EventHandler<ListViewInsertEventArgs>) base.Events[EventItemInserting];
            if (handler != null)
            {
                handler(this, e);
            }
            else if (!base.IsBoundUsingDataSourceID && !e.Cancel)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_UnhandledEvent, new object[] { this.ID, "ItemInserting" }));
            }
        }

        protected virtual void OnItemUpdated(ListViewUpdatedEventArgs e)
        {
            EventHandler<ListViewUpdatedEventArgs> handler = (EventHandler<ListViewUpdatedEventArgs>) base.Events[EventItemUpdated];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnItemUpdating(ListViewUpdateEventArgs e)
        {
            EventHandler<ListViewUpdateEventArgs> handler = (EventHandler<ListViewUpdateEventArgs>) base.Events[EventItemUpdating];
            if (handler != null)
            {
                handler(this, e);
            }
            else if (!base.IsBoundUsingDataSourceID && !e.Cancel)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_UnhandledEvent, new object[] { this.ID, "ItemUpdating" }));
            }
        }

        protected virtual void OnLayoutCreated(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventLayoutCreated];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPagePropertiesChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventPagePropertiesChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnPagePropertiesChanging(PagePropertiesChangingEventArgs e)
        {
            EventHandler<PagePropertiesChangingEventArgs> handler = (EventHandler<PagePropertiesChangingEventArgs>) base.Events[EventPagePropertiesChanging];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            if (this.DataKeyNamesInternal.Length > 0)
            {
                this.SelectedPersistedDataKey = this.SelectedDataKey;
            }
            EventHandler handler = (EventHandler) base.Events[EventSelectedIndexChanged];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSelectedIndexChanging(ListViewSelectEventArgs e)
        {
            EventHandler<ListViewSelectEventArgs> handler = (EventHandler<ListViewSelectEventArgs>) base.Events[EventSelectedIndexChanging];
            if (handler != null)
            {
                handler(this, e);
            }
            else if (!base.IsBoundUsingDataSourceID && !e.Cancel)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_UnhandledEvent, new object[] { this.ID, "SelectedIndexChanging" }));
            }
        }

        protected virtual void OnSorted(EventArgs e)
        {
            EventHandler handler = (EventHandler) base.Events[EventSorted];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSorting(ListViewSortEventArgs e)
        {
            EventHandler<ListViewSortEventArgs> handler = (EventHandler<ListViewSortEventArgs>) base.Events[EventSorting];
            if (handler != null)
            {
                handler(this, e);
            }
            else if (!base.IsBoundUsingDataSourceID && !e.Cancel)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_UnhandledEvent, new object[] { this.ID, "Sorting" }));
            }
        }

        protected virtual void OnTotalRowCountAvailable(PageEventArgs e)
        {
            EventHandler<PageEventArgs> handler = (EventHandler<PageEventArgs>) base.Events[EventTotalRowCountAvailable];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool PageIsValidAfterModelException()
        {
            if (this._modelValidationGroup == null)
            {
                return true;
            }
            this.Page.Validate(this._modelValidationGroup);
            return this.Page.IsValid;
        }

        protected internal override void PerformDataBinding(IEnumerable data)
        {
            base.PerformDataBinding(data);
            this.TrackViewState();
            int num = this.CreateChildControls(data, true);
            base.ChildControlsCreated = true;
            this.ViewState["_!ItemCount"] = num;
            int editIndex = this.EditIndex;
            if ((base.IsBoundUsingDataSourceID && (editIndex != -1)) && ((editIndex < this.Items.Count) && base.IsViewStateEnabled))
            {
                this.BoundFieldValues.Clear();
                this.ExtractItemValues(this.BoundFieldValues, this.Items[editIndex], false);
            }
            if (this._persistedDataKey == null)
            {
                DataKeyArray dataKeys = this.DataKeys;
                if ((dataKeys != null) && (dataKeys.Count > 0))
                {
                    this._persistedDataKey = dataKeys[0];
                }
            }
        }

        protected override void PerformSelect()
        {
            this.EnsureLayoutTemplate();
            if (base.DesignMode)
            {
                DataPager pager = this.FindDataPager(this);
                if (pager != null)
                {
                    this._maximumRows = pager.PageSize;
                }
            }
            base.PerformSelect();
        }

        protected virtual void RemoveItems()
        {
            if (this._groupTemplate != null)
            {
                if (this._groupsItemCreatedCount > 0)
                {
                    for (int i = 0; i < this._groupsItemCreatedCount; i++)
                    {
                        this._groupsGroupPlaceholderContainer.Controls.RemoveAt(this._groupsOriginalIndexOfGroupPlaceholderInContainer);
                    }
                    this._groupsItemCreatedCount = 0;
                }
            }
            else if (this._noGroupsItemCreatedCount > 0)
            {
                for (int j = 0; j < this._noGroupsItemCreatedCount; j++)
                {
                    this._noGroupsItemPlaceholderContainer.Controls.RemoveAt(this._noGroupsOriginalIndexOfItemPlaceholderInContainer);
                }
                this._noGroupsItemCreatedCount = 0;
            }
            this._autoIDIndex = 0;
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.RenderContents(writer);
        }

        protected internal override object SaveControlState()
        {
            object obj2 = base.SaveControlState();
            if (((((obj2 != null) || (this._editIndex != -1)) || ((this._selectedIndex != -1) || (this._groupItemCount != 1))) || (((this._sortExpression != null) && (this._sortExpression.Length != 0)) || ((this._sortDirection != System.Web.UI.WebControls.SortDirection.Ascending) || (this._totalRowCount != -1)))) || (((this._dataKeyNames != null) && (this._dataKeyNames.Length != 0)) || ((this._dataKeysArrayList != null) && (this._dataKeysArrayList.Count > 0))))
            {
                return new object[] { obj2, ((this._editIndex == -1) ? null : ((object) this._editIndex)), ((this._selectedIndex == -1) ? null : ((object) this._selectedIndex)), ((this._groupItemCount == 1) ? null : ((object) this._groupItemCount)), (((this._sortExpression == null) || (this._sortExpression.Length == 0)) ? null : this._sortExpression), ((this._sortDirection == System.Web.UI.WebControls.SortDirection.Ascending) ? null : ((object) ((int) this._sortDirection))), (((this._dataKeyNames == null) || (this._dataKeyNames.Length == 0)) ? null : this._dataKeyNames), this.SaveDataKeysState(), ((this._totalRowCount == -1) ? null : ((object) this._totalRowCount)), ((this._persistedDataKey == null) ? null : ((IStateManager) this._persistedDataKey).SaveViewState()) };
            }
            return true;
        }

        private object SaveDataKeysState()
        {
            object obj2 = new object();
            int count = 0;
            if ((this._dataKeysArrayList != null) && (this._dataKeysArrayList.Count > 0))
            {
                count = this._dataKeysArrayList.Count;
                obj2 = new object[count];
                for (int i = 0; i < count; i++)
                {
                    ((object[]) obj2)[i] = ((IStateManager) this._dataKeysArrayList[i]).SaveViewState();
                }
            }
            if ((this._dataKeysArrayList != null) && (count != 0))
            {
                return obj2;
            }
            return null;
        }

        protected override object SaveViewState()
        {
            object obj2 = base.SaveViewState();
            object obj3 = (this._boundFieldValues != null) ? OrderedDictionaryStateHelper.SaveViewState(this._boundFieldValues) : null;
            return new object[] { obj2, obj3 };
        }

        private void SelectCallback(IEnumerable data)
        {
            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_DataSourceDoesntSupportPaging, new object[] { this.DataSourceID }));
        }

        protected virtual void SetPageProperties(int startRowIndex, int maximumRows, bool databind)
        {
            if (maximumRows < 1)
            {
                throw new ArgumentOutOfRangeException("maximumRows");
            }
            if (startRowIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startRowIndex");
            }
            if ((this._startRowIndex != startRowIndex) || (this._maximumRows != maximumRows))
            {
                PagePropertiesChangingEventArgs e = new PagePropertiesChangingEventArgs(startRowIndex, maximumRows);
                if (databind)
                {
                    this.OnPagePropertiesChanging(e);
                }
                this._startRowIndex = e.StartRowIndex;
                this._maximumRows = e.MaximumRows;
                if (databind)
                {
                    this.OnPagePropertiesChanged(EventArgs.Empty);
                }
            }
            if (databind)
            {
                base.RequiresDataBinding = true;
            }
        }

        private void SetRequiresDataBindingIfInitialized()
        {
            if (base.Initialized)
            {
                base.RequiresDataBinding = true;
            }
        }

        public virtual void Sort(string sortExpression, System.Web.UI.WebControls.SortDirection sortDirection)
        {
            this.HandleSort(sortExpression, sortDirection);
        }

        void IPageableItemContainer.SetPageProperties(int startRowIndex, int maximumRows, bool databind)
        {
            this.SetPageProperties(startRowIndex, maximumRows, databind);
        }

        public virtual void UpdateItem(int itemIndex, bool causesValidation)
        {
            this.HandleUpdate(null, itemIndex, causesValidation);
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override string AccessKey
        {
            get => 
                base.AccessKey;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [ResourceDescription("ListView_AlternatingItemTemplate"), Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ListViewDataItem), BindingDirection.TwoWay)]
        public virtual ITemplate AlternatingItemTemplate
        {
            get => 
                this._alternatingItemTemplate;
            set
            {
                this._alternatingItemTemplate = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor
        {
            get => 
                base.BackColor;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BorderColor
        {
            get => 
                base.BorderColor;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override System.Web.UI.WebControls.BorderStyle BorderStyle
        {
            get => 
                base.BorderStyle;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Unit BorderWidth
        {
            get => 
                base.BorderWidth;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        private IOrderedDictionary BoundFieldValues
        {
            get
            {
                if (this._boundFieldValues == null)
                {
                    this._boundFieldValues = new OrderedDictionary();
                }
                return this._boundFieldValues;
            }
        }

        public override ControlCollection Controls
        {
            get
            {
                this.EnsureChildControls();
                return base.Controls;
            }
        }

        [DefaultValue(true), Category("Behavior"), ResourceDescription("ListView_ConvertEmptyStringToNull")]
        public virtual bool ConvertEmptyStringToNull
        {
            get
            {
                object obj2 = this.ViewState["ConvertEmptyStringToNull"];
                if (obj2 != null)
                {
                    return (bool) obj2;
                }
                return true;
            }
            set
            {
                this.ViewState["ConvertEmptyStringToNull"] = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), CssClassProperty]
        public override string CssClass
        {
            get => 
                base.CssClass;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [TypeConverter(typeof(StringArrayConverter)), DefaultValue((string) null), Editor("System.Web.UI.Design.WebControls.DataFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Data"), ResourceDescription("ListView_DataKeyNames")]
        public virtual string[] DataKeyNames
        {
            get
            {
                object obj2 = this._dataKeyNames;
                if (obj2 != null)
                {
                    return (string[]) ((string[]) obj2).Clone();
                }
                return new string[0];
            }
            set
            {
                if (!DataBoundControlHelper.CompareStringArrays(value, this.DataKeyNamesInternal))
                {
                    if (value != null)
                    {
                        this._dataKeyNames = (string[]) value.Clone();
                    }
                    else
                    {
                        this._dataKeyNames = null;
                    }
                    this.ClearDataKeys();
                    this.SetRequiresDataBindingIfInitialized();
                }
            }
        }

        private string[] DataKeyNamesInternal
        {
            get
            {
                object obj2 = this._dataKeyNames;
                if (obj2 != null)
                {
                    return (string[]) obj2;
                }
                return new string[0];
            }
        }

        [ResourceDescription("ListView_DataKeys"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual DataKeyArray DataKeys
        {
            get
            {
                if (this._dataKeyArray == null)
                {
                    this._dataKeyArray = new DataKeyArray(this.DataKeysArrayList);
                    if (base.IsTrackingViewState)
                    {
                        ((IStateManager) this._dataKeyArray).TrackViewState();
                    }
                }
                return this._dataKeyArray;
            }
        }

        private ArrayList DataKeysArrayList
        {
            get
            {
                if (this._dataKeysArrayList == null)
                {
                    this._dataKeysArrayList = new ArrayList();
                }
                return this._dataKeysArrayList;
            }
        }

        [ResourceDescription("ListView_EditIndex"), DefaultValue(-1), Category("Default")]
        public virtual int EditIndex
        {
            get => 
                this._editIndex;
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this._editIndex)
                {
                    if (value == -1)
                    {
                        this.BoundFieldValues.Clear();
                    }
                    this._editIndex = value;
                    this.SetRequiresDataBindingIfInitialized();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ResourceDescription("ListView_EditItem")]
        public virtual ListViewItem EditItem
        {
            get
            {
                if ((this._editIndex > -1) && (this._editIndex < this.Items.Count))
                {
                    return this.Items[this._editIndex];
                }
                return null;
            }
        }

        [Browsable(false), ResourceDescription("ListView_EditItemTemplate"), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ListViewDataItem), BindingDirection.TwoWay)]
        public virtual ITemplate EditItemTemplate
        {
            get => 
                this._editItemTemplate;
            set
            {
                this._editItemTemplate = value;
            }
        }

        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ListView)), ResourceDescription("ListView_EmptyDataTemplate"), DefaultValue((string) null)]
        public virtual ITemplate EmptyDataTemplate
        {
            get => 
                this._emptyDataTemplate;
            set
            {
                this._emptyDataTemplate = value;
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), Browsable(false), TemplateContainer(typeof(ListViewItem)), ResourceDescription("ListView_EmptyItemTemplate"), DefaultValue((string) null)]
        public virtual ITemplate EmptyItemTemplate
        {
            get => 
                this._emptyItemTemplate;
            set
            {
                this._emptyItemTemplate = value;
            }
        }

        [DefaultValue(false), ResourceDescription("ListView_EnableModelValidation"), WebCategory("Behavior")]
        public virtual bool EnableModelValidation
        {
            get
            {
                object obj2 = this.ViewState["EnableModelValidation"];
                return ((obj2 != null) && ((bool) obj2));
            }
            set
            {
                this.ViewState["EnableModelValidation"] = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        public override FontInfo Font =>
            base.Font;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override Color ForeColor
        {
            get => 
                base.ForeColor;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [ResourceDescription("ListView_GroupItemCount"), Category("Default"), DefaultValue(1)]
        public virtual int GroupItemCount
        {
            get => 
                this._groupItemCount;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._groupItemCount = value;
                this.SetRequiresDataBindingIfInitialized();
            }
        }

        [Category("Behavior"), ResourceDescription("ListView_GroupPlaceholderID"), DefaultValue("groupPlaceholder")]
        public virtual string GroupPlaceholderID
        {
            get
            {
                object obj2 = this.ViewState["GroupPlaceholderID"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return "groupPlaceholder";
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_ContainerNameMustNotBeEmpty, new object[] { "GroupPlaceholderID" }));
                }
                this.ViewState["GroupPlaceholderID"] = value;
            }
        }

        [Browsable(false), ResourceDescription("ListView_GroupSeparatorTemplate"), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ListViewItem))]
        public virtual ITemplate GroupSeparatorTemplate
        {
            get => 
                this._groupSeparatorTemplate;
            set
            {
                this._groupSeparatorTemplate = value;
            }
        }

        [Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ListViewItem)), ResourceDescription("ListView_GroupTemplate"), DefaultValue((string) null)]
        public virtual ITemplate GroupTemplate
        {
            get => 
                this._groupTemplate;
            set
            {
                this._groupTemplate = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Unit Height
        {
            get => 
                base.Height;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), ResourceDescription("ListView_InsertItem")]
        public virtual ListViewItem InsertItem =>
            this._insertItem;

        [ResourceDescription("ListView_InsertItemPosition"), Category("Default"), DefaultValue(0)]
        public virtual System.Web.UI.WebControls.InsertItemPosition InsertItemPosition
        {
            get
            {
                object obj2 = this.ViewState["InsertItemPosition"];
                if (obj2 != null)
                {
                    return (System.Web.UI.WebControls.InsertItemPosition) obj2;
                }
                return System.Web.UI.WebControls.InsertItemPosition.None;
            }
            set
            {
                if (this.InsertItemPosition != value)
                {
                    this.ViewState["InsertItemPosition"] = value;
                    this.SetRequiresDataBindingIfInitialized();
                }
            }
        }

        [PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string) null), ResourceDescription("ListView_InsertItemTemplate"), TemplateContainer(typeof(ListViewItem), BindingDirection.TwoWay), Browsable(false)]
        public virtual ITemplate InsertItemTemplate
        {
            get => 
                this._insertItemTemplate;
            set
            {
                this._insertItemTemplate = value;
            }
        }

        [ResourceDescription("ListView_ItemPlaceholderID"), DefaultValue("itemPlaceholder"), Category("Behavior")]
        public virtual string ItemPlaceholderID
        {
            get
            {
                object obj2 = this.ViewState["ItemPlaceholderID"];
                if (obj2 != null)
                {
                    return (string) obj2;
                }
                return "itemPlaceholder";
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentOutOfRangeException("value", string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_ContainerNameMustNotBeEmpty, new object[] { "ItemPlaceholderID" }));
                }
                this.ViewState["ItemPlaceholderID"] = value;
            }
        }

        [ResourceDescription("ListView_Items"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual IList<ListViewDataItem> Items
        {
            get
            {
                if (this._itemList == null)
                {
                    this._itemList = new List<ListViewDataItem>();
                }
                return this._itemList;
            }
        }

        [ResourceDescription("ListView_ItemSeparatorTemplate"), TemplateContainer(typeof(ListViewItem)), Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate ItemSeparatorTemplate
        {
            get => 
                this._itemSeparatorTemplate;
            set
            {
                this._itemSeparatorTemplate = value;
            }
        }

        [ResourceDescription("ListView_ItemTemplate"), Browsable(false), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(ListViewDataItem), BindingDirection.TwoWay)]
        public virtual ITemplate ItemTemplate
        {
            get => 
                this._itemTemplate;
            set
            {
                this._itemTemplate = value;
            }
        }

        [TemplateContainer(typeof(ListView)), DefaultValue((string) null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), ResourceDescription("ListView_LayoutTemplate")]
        public virtual ITemplate LayoutTemplate
        {
            get => 
                this._layoutTemplate;
            set
            {
                this._layoutTemplate = value;
            }
        }

        protected virtual int MaximumRows =>
            this._maximumRows;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public virtual DataKey SelectedDataKey
        {
            get
            {
                if ((this.DataKeyNamesInternal == null) || (this.DataKeyNamesInternal.Length == 0))
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ListView_DataKeyNamesMustBeSpecified, new object[] { this.ID }));
                }
                DataKeyArray dataKeys = this.DataKeys;
                int selectedIndex = this.SelectedIndex;
                if (((dataKeys != null) && (selectedIndex < dataKeys.Count)) && (selectedIndex > -1))
                {
                    return dataKeys[selectedIndex];
                }
                return null;
            }
        }

        [DefaultValue(-1), ResourceDescription("ListView_SelectedIndex"), Category("Default")]
        public virtual int SelectedIndex
        {
            get => 
                this._selectedIndex;
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (value != this._selectedIndex)
                {
                    this._selectedIndex = value;
                    this.SetRequiresDataBindingIfInitialized();
                }
            }
        }

        [DefaultValue((string) null), TemplateContainer(typeof(ListViewDataItem), BindingDirection.TwoWay), ResourceDescription("ListView_SelectedItemTemplate"), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty)]
        public virtual ITemplate SelectedItemTemplate
        {
            get => 
                this._selectedItemTemplate;
            set
            {
                this._selectedItemTemplate = value;
            }
        }

        [Browsable(false)]
        public virtual DataKey SelectedPersistedDataKey
        {
            get => 
                this._persistedDataKey;
            set
            {
                this._persistedDataKey = value;
                if (base.IsTrackingViewState && (this._persistedDataKey != null))
                {
                    ((IStateManager) this._persistedDataKey).TrackViewState();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public object SelectedValue
        {
            get
            {
                if (this.SelectedDataKey != null)
                {
                    return this.SelectedDataKey.Value;
                }
                return null;
            }
        }

        [DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ResourceCategory("Sorting"), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), ResourceDescription("ListView_SortDirection")]
        public virtual System.Web.UI.WebControls.SortDirection SortDirection =>
            this.SortDirectionInternal;

        private System.Web.UI.WebControls.SortDirection SortDirectionInternal
        {
            get => 
                this._sortDirection;
            set
            {
                if ((value < System.Web.UI.WebControls.SortDirection.Ascending) || (value > System.Web.UI.WebControls.SortDirection.Descending))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (this._sortDirection != value)
                {
                    this._sortDirection = value;
                    this.SetRequiresDataBindingIfInitialized();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), ResourceDescription("ListView_SortExpression"), ResourceCategory("Sorting"), Browsable(false)]
        public virtual string SortExpression =>
            this.SortExpressionInternal;

        private string SortExpressionInternal
        {
            get => 
                this._sortExpression;
            set
            {
                if (this._sortExpression != value)
                {
                    this._sortExpression = value;
                    this.SetRequiresDataBindingIfInitialized();
                }
            }
        }

        protected virtual int StartRowIndex =>
            this._startRowIndex;

        int IPageableItemContainer.MaximumRows =>
            this.MaximumRows;

        int IPageableItemContainer.StartRowIndex =>
            this.StartRowIndex;

        DataKey IPersistedSelector.DataKey
        {
            get => 
                this.SelectedPersistedDataKey;
            set
            {
                this.SelectedPersistedDataKey = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public override short TabIndex
        {
            get => 
                base.TabIndex;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override string ToolTip
        {
            get => 
                base.ToolTip;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Unit Width
        {
            get => 
                base.Width;
            set
            {
                throw new NotSupportedException(AtlasWeb.ListView_StylePropertiesNotSupported);
            }
        }
    }
}

