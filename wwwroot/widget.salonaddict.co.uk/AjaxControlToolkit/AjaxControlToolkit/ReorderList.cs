namespace AjaxControlToolkit
{
    using AjaxControlToolkit.Design;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    [ToolboxBitmap(typeof(ReorderList), "ReorderList.ReorderList.ico"), Designer("AjaxControlToolkit.ReorderListDesigner, AjaxControlToolkit")]
    public class ReorderList : CompositeDataBoundControl, IRepeatInfoUser, INamingContainer, ICallbackEventHandler, IPostBackEventHandler
    {
        private string _callbackResult = string.Empty;
        private AjaxControlToolkit.BulletedList _childList;
        private List<DraggableListItemInfo> _draggableItems;
        private ITemplate _dragHandleTemplate;
        private Control _dropTemplateControl;
        private DropWatcherExtender _dropWatcherExtender;
        private ITemplate _editItemTemplate;
        private ITemplate _emptyListTemplate;
        private ITemplate _insertItemTemplate;
        private ITemplate _itemTemplate;
        private ReorderListItemLayoutType _layoutType = ReorderListItemLayoutType.Table;
        private ITemplate _reorderTemplate;
        private const string ArgContext = "_~Context~_";
        private const string ArgError = "_~Error~_";
        private const string ArgReplace = "_~Arg~_";
        private const string ArgSuccess = "_~Success~_";
        private static object CancelCommandKey = new object();
        private static object DeleteCommandKey = new object();
        private static object EditCommandKey = new object();
        private static object InsertCommandKey = new object();
        private static object ItemCommandKey = new object();
        private static object ItemCreatedKey = new object();
        private static object ItemDataBoundKey = new object();
        private static object ItemReorderKey = new object();
        private ArrayList itemsArray;
        private static object KeysKey = new object();
        private static object UpdateCommandKey = new object();

        public event EventHandler<ReorderListCommandEventArgs> CancelCommand
        {
            add
            {
                base.Events.AddHandler(CancelCommandKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(CancelCommandKey, value);
            }
        }

        public event EventHandler<ReorderListCommandEventArgs> DeleteCommand
        {
            add
            {
                base.Events.AddHandler(DeleteCommandKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(DeleteCommandKey, value);
            }
        }

        public event EventHandler<ReorderListCommandEventArgs> EditCommand
        {
            add
            {
                base.Events.AddHandler(EditCommandKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(EditCommandKey, value);
            }
        }

        public event EventHandler<ReorderListCommandEventArgs> InsertCommand
        {
            add
            {
                base.Events.AddHandler(InsertCommandKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(InsertCommandKey, value);
            }
        }

        public event EventHandler<ReorderListCommandEventArgs> ItemCommand
        {
            add
            {
                base.Events.AddHandler(ItemCommandKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ItemCommandKey, value);
            }
        }

        public event EventHandler<ReorderListItemEventArgs> ItemCreated
        {
            add
            {
                base.Events.AddHandler(ItemCreatedKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ItemCreatedKey, value);
            }
        }

        public event EventHandler<ReorderListItemEventArgs> ItemDataBound
        {
            add
            {
                base.Events.AddHandler(ItemDataBoundKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ItemDataBoundKey, value);
            }
        }

        public event EventHandler<ReorderListItemReorderEventArgs> ItemReorder
        {
            add
            {
                base.Events.AddHandler(ItemReorderKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ItemReorderKey, value);
            }
        }

        public event EventHandler<ReorderListCommandEventArgs> UpdateCommand
        {
            add
            {
                base.Events.AddHandler(UpdateCommandKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(UpdateCommandKey, value);
            }
        }

        private void ClearChildren()
        {
            this.ChildList.Controls.Clear();
            this._dropTemplateControl = null;
            if (this._draggableItems != null)
            {
                foreach (DraggableListItemInfo info in this._draggableItems)
                {
                    if (info.Extender != null)
                    {
                        info.Extender.Dispose();
                    }
                }
            }
            this._draggableItems = null;
            for (int i = this.Controls.Count - 1; i >= 0; i--)
            {
                if (this.Controls[i] is DropWatcherExtender)
                {
                    this.Controls[i].Dispose();
                }
            }
        }

        private static IDictionary CopyDictionary(IDictionary source, IDictionary dest)
        {
            if (dest == null)
            {
                dest = new OrderedDictionary(source.Count);
            }
            foreach (DictionaryEntry entry in source)
            {
                dest[entry.Key] = entry.Value;
            }
            return dest;
        }

        protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
        {
            this.ClearChildren();
            int num = 0;
            ArrayList dataKeysArray = this.DataKeysArray;
            this.itemsArray = new ArrayList();
            int num2 = base.DesignMode ? 1 : 0;
            if (dataBinding)
            {
                dataKeysArray.Clear();
                ICollection is2 = dataSource as ICollection;
                if (is2 != null)
                {
                    dataKeysArray.Capacity = is2.Count;
                    this.itemsArray.Capacity = is2.Count;
                }
            }
            if (dataSource != null)
            {
                string dataKeyField = this.DataKeyField;
                bool flag = dataBinding && !string.IsNullOrEmpty(dataKeyField);
                bool hasDragHandle = this.AllowReorder && (this.DragHandleTemplate != null);
                num2 = 0;
                int index = 0;
                foreach (object obj2 in dataSource)
                {
                    if (flag)
                    {
                        dataKeysArray.Add(DataBinder.GetPropertyValue(obj2, dataKeyField));
                    }
                    ListItemType item = ListItemType.Item;
                    if (index == this.EditItemIndex)
                    {
                        item = ListItemType.EditItem;
                    }
                    this.CreateItem(index, dataBinding, obj2, item, hasDragHandle);
                    num2++;
                    index++;
                }
                if (this.ShowInsertItem && (this.InsertItemTemplate != null))
                {
                    this.CreateInsertItem(index);
                    num++;
                }
            }
            if ((this.AllowReorder && (num2 > 1)) && (this._draggableItems != null))
            {
                Control control;
                Control control2;
                foreach (DraggableListItemInfo info in this._draggableItems)
                {
                    info.Extender = new DraggableListItemExtender();
                    info.Extender.TargetControlID = info.TargetControl.ID;
                    info.Extender.Handle = info.HandleControl.ClientID;
                    info.Extender.ID = string.Format(CultureInfo.InvariantCulture, "{0}_{1}", new object[] { this.ID, info.Extender.TargetControlID });
                    this.Controls.Add(info.Extender);
                }
                this.GetDropTemplateControl(out control, out control2);
                this._dropWatcherExtender = new DropWatcherExtender();
                this._dropWatcherExtender.ArgReplaceString = "_~Arg~_";
                this._dropWatcherExtender.CallbackCssStyle = this.CallbackCssStyle;
                this._dropWatcherExtender.DropLayoutElement = control.ID;
                if (this.PostBackOnReorder)
                {
                    this._dropWatcherExtender.PostBackCode = this.Page.ClientScript.GetPostBackEventReference(this, "_~Arg~_");
                }
                else
                {
                    this._dropWatcherExtender.PostBackCode = this.Page.ClientScript.GetCallbackEventReference(this, "'_~Arg~_'", "_~Success~_", "'_~Context~_'", "_~Error~_", true);
                    this._dropWatcherExtender.ArgContextString = "_~Context~_";
                    this._dropWatcherExtender.ArgSuccessString = "_~Success~_";
                    this._dropWatcherExtender.ArgErrorString = "_~Error~_";
                }
                this._dropWatcherExtender.EnableClientState = !this.PostBackOnReorder;
                this._dropWatcherExtender.BehaviorID = this.UniqueID + "_dItemEx";
                this._dropWatcherExtender.TargetControlID = this.ChildList.ID;
                this.Controls.Add(this._dropWatcherExtender);
            }
            return (this.ChildList.Controls.Count - num);
        }

        protected virtual void CreateDragHandle(ReorderListItem item)
        {
            if (this.AllowReorder)
            {
                Control control = item;
                if (this.DragHandleTemplate == null)
                {
                    Panel dest = new Panel();
                    MoveChildren(item, dest);
                    control = dest;
                    item.Controls.Add(dest);
                }
                else
                {
                    Control child = null;
                    Control control3 = null;
                    if (this.LayoutType == ReorderListItemLayoutType.User)
                    {
                        child = new Panel();
                        Panel panel = new Panel();
                        Panel panel2 = new Panel();
                        control = panel2;
                        control3 = panel;
                        if ((this.DragHandleAlignment == ReorderHandleAlignment.Left) || (this.DragHandleAlignment == ReorderHandleAlignment.Top))
                        {
                            child.Controls.Add(panel2);
                            child.Controls.Add(panel);
                        }
                        else
                        {
                            child.Controls.Add(panel);
                            child.Controls.Add(panel2);
                        }
                    }
                    else
                    {
                        Table table = new Table();
                        child = table;
                        table.BorderWidth = 0;
                        table.CellPadding = 0;
                        table.CellSpacing = 0;
                        TableCell cell = new TableCell();
                        control3 = cell;
                        cell.Width = new Unit(100.0, UnitType.Percentage);
                        TableCell cell2 = new TableCell();
                        control = cell2;
                        switch (this.DragHandleAlignment)
                        {
                            case ReorderHandleAlignment.Top:
                            case ReorderHandleAlignment.Bottom:
                            {
                                TableRow row2 = new TableRow();
                                TableRow row3 = new TableRow();
                                row2.Cells.Add(cell);
                                row3.Cells.Add(cell2);
                                if (this.DragHandleAlignment != ReorderHandleAlignment.Top)
                                {
                                    table.Rows.Add(row2);
                                    table.Rows.Add(row3);
                                    break;
                                }
                                table.Rows.Add(row3);
                                table.Rows.Add(row2);
                                break;
                            }
                            case ReorderHandleAlignment.Left:
                            case ReorderHandleAlignment.Right:
                            {
                                TableRow row = new TableRow();
                                if (this.DragHandleAlignment != ReorderHandleAlignment.Left)
                                {
                                    row.Cells.Add(cell);
                                    row.Cells.Add(cell2);
                                }
                                else
                                {
                                    row.Cells.Add(cell2);
                                    row.Cells.Add(cell);
                                }
                                table.Rows.Add(row);
                                break;
                            }
                        }
                    }
                    MoveChildren(item, control3);
                    ReorderListItem container = new ReorderListItem(item, HtmlTextWriterTag.Div);
                    this.DragHandleTemplate.InstantiateIn(container);
                    control.Controls.Add(container);
                    item.Controls.Add(child);
                }
                control.ID = string.Format(CultureInfo.InvariantCulture, "__dih{0}", new object[] { item.ItemIndex });
                if (this._draggableItems == null)
                {
                    this._draggableItems = new List<DraggableListItemInfo>();
                }
                DraggableListItemInfo info = new DraggableListItemInfo {
                    TargetControl = item,
                    HandleControl = control
                };
                this._draggableItems.Add(info);
            }
        }

        protected virtual ReorderListItem CreateInsertItem(int index)
        {
            if ((this.InsertItemTemplate != null) && this.ShowInsertItem)
            {
                ReorderListItem container = new ReorderListItem(index, true);
                this.InsertItemTemplate.InstantiateIn(container);
                this.ChildList.Controls.Add(container);
                return container;
            }
            return null;
        }

        protected virtual ReorderListItem CreateItem(int index, bool dataBind, object dataItem, ListItemType itemType, bool hasDragHandle)
        {
            if (((itemType != ListItemType.Item) && (itemType != ListItemType.EditItem)) && (itemType != ListItemType.Separator))
            {
                throw new ArgumentException("Unknown value", "itemType");
            }
            ReorderListItem item = new ReorderListItem(dataItem, index, itemType);
            this.OnItemCreated(new ReorderListItemEventArgs(item));
            ITemplate itemTemplate = this.ItemTemplate;
            if (index == this.EditItemIndex)
            {
                itemTemplate = this.EditItemTemplate;
            }
            if (itemType == ListItemType.Separator)
            {
                itemTemplate = this.ReorderTemplate;
            }
            if (itemTemplate != null)
            {
                itemTemplate.InstantiateIn(item);
            }
            if (((itemType == ListItemType.Item) && (itemTemplate == null)) && ((dataItem != null) && (this.DataSource is IList)))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(dataItem);
                if (converter != null)
                {
                    Label child = new Label {
                        Text = converter.ConvertToString(null, CultureInfo.CurrentUICulture, dataItem)
                    };
                    item.Controls.Add(child);
                }
            }
            this.CreateDragHandle(item);
            this.ChildList.Controls.Add(item);
            if (dataBind)
            {
                item.DataBind();
                this.OnItemDataBound(new ReorderListItemEventArgs(item));
                item.DataItem = null;
            }
            return item;
        }

        private Control CreateReorderArea(int index, string reorderKey)
        {
            Panel container = new Panel {
                ID = string.Format(CultureInfo.InvariantCulture, "__drop{1}{0}", new object[] { 
                    index,
                    reorderKey
                })
            };
            if (this.ReorderTemplate != null)
            {
                this.ReorderTemplate.InstantiateIn(container);
            }
            return container;
        }

        protected virtual bool DoReorder(int oldIndex, int newIndex)
        {
            if (base.IsBoundUsingDataSourceID && (this.SortOrderField != null))
            {
                DataSourceViewSelectCallback callback = null;
                DataSourceView dsv = this.GetData();
                EventWaitHandle w = new EventWaitHandle(false, EventResetMode.AutoReset);
                bool success = false;
                base.RequiresDataBinding = true;
                try
                {
                    if (callback == null)
                    {
                        callback = delegate (IEnumerable dataSource) {
                            success = this.DoReorderInternal(dataSource, oldIndex, newIndex, dsv);
                            w.Set();
                        };
                    }
                    dsv.Select(new DataSourceSelectArguments(), callback);
                    w.WaitOne();
                }
                catch (Exception exception)
                {
                    this.CallbackResult = exception.Message;
                    throw;
                }
                return success;
            }
            if ((this.DataSource is DataTable) || (this.DataSource is DataView))
            {
                DataTable table = this.DataSource as DataTable;
                if (table == null)
                {
                    table = ((DataView) this.DataSource).Table;
                }
                return this.DoReorderInternal(table, oldIndex, newIndex);
            }
            if (!(this.DataSource is IList) || ((IList) this.DataSource).IsReadOnly)
            {
                return false;
            }
            IList dataSource = (IList) this.DataSource;
            object obj2 = dataSource[oldIndex];
            if (oldIndex > newIndex)
            {
                for (int i = oldIndex; i > newIndex; i--)
                {
                    dataSource[i] = dataSource[i - 1];
                }
            }
            else
            {
                for (int j = oldIndex; j < newIndex; j++)
                {
                    dataSource[j] = dataSource[j + 1];
                }
            }
            dataSource[newIndex] = obj2;
            return true;
        }

        private bool DoReorderInternal(DataTable dataSource, int oldIndex, int newIndex)
        {
            if (string.IsNullOrEmpty(this.SortOrderField))
            {
                return false;
            }
            int num = Math.Min(oldIndex, newIndex);
            int num2 = Math.Max(oldIndex, newIndex);
            string filterExpression = string.Format(CultureInfo.InvariantCulture, "{0} >= {1} AND {0} <= {2}", new object[] { this.SortOrderField, num, num2 });
            DataRow[] rowArray = dataSource.Select(filterExpression, this.SortOrderField + " ASC");
            DataColumn column = dataSource.Columns[this.SortOrderField];
            object obj2 = rowArray[newIndex - num][column];
            if (oldIndex > newIndex)
            {
                for (int i = 0; i < (rowArray.Length - 1); i++)
                {
                    rowArray[i][column] = rowArray[i + 1][column];
                }
            }
            else
            {
                for (int j = rowArray.Length - 1; j > 0; j--)
                {
                    rowArray[j][column] = rowArray[j - 1][column];
                }
            }
            rowArray[oldIndex - num][column] = obj2;
            dataSource.AcceptChanges();
            return true;
        }

        private bool DoReorderInternal(IEnumerable dataSource, int oldIndex, int newIndex, DataSourceView dsv)
        {
            string sortOrderField = this.SortOrderField;
            List<IOrderedDictionary> list = new List<IOrderedDictionary>(Math.Abs((int) (oldIndex - newIndex)));
            int num = Math.Min(oldIndex, newIndex);
            int num2 = Math.Max(oldIndex, newIndex);
            if (num == num2)
            {
                return false;
            }
            int num3 = 0;
            foreach (object obj2 in dataSource)
            {
                try
                {
                    if (num3 >= num)
                    {
                        if (num3 > num2)
                        {
                            break;
                        }
                        IOrderedDictionary dictionary = new OrderedDictionary();
                        IDictionary dictionary2 = new Hashtable();
                        foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj2))
                        {
                            object obj3 = descriptor.GetValue(obj2);
                            if (descriptor.PropertyType.IsValueType && (obj3 == DBNull.Value))
                            {
                                obj3 = null;
                            }
                            dictionary[descriptor.Name] = obj3;
                            if (descriptor.Name == this.DataKeyField)
                            {
                                dictionary2[descriptor.Name] = dictionary[descriptor.Name];
                                dictionary.Remove(descriptor.Name);
                            }
                        }
                        dictionary[KeysKey] = dictionary2;
                        list.Add(dictionary);
                    }
                }
                finally
                {
                    num3++;
                }
            }
            oldIndex -= num;
            newIndex -= num;
            int result = -2147483648;
            if ((list.Count <= 0) || !list[0].Contains(sortOrderField))
            {
                throw new InvalidOperationException("Couldn't find sort field '" + this.SortOrderField + "' in bound data.");
            }
            object obj4 = list[0][sortOrderField];
            if (obj4 is int)
            {
                result = (int) obj4;
            }
            else
            {
                string s = obj4 as string;
                if (s != null)
                {
                    if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                    {
                        return false;
                    }
                }
                else
                {
                    if (((obj4 != null) && obj4.GetType().IsValueType) && obj4.GetType().IsPrimitive)
                    {
                        result = Convert.ToInt32(obj4, CultureInfo.InvariantCulture);
                        return true;
                    }
                    return false;
                }
            }
            if (result == -2147483648)
            {
                result = 0;
            }
            IOrderedDictionary item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
            foreach (IOrderedDictionary dictionary4 in list)
            {
                IDictionary keys = (IDictionary) dictionary4[KeysKey];
                dictionary4.Remove(KeysKey);
                IDictionary oldValues = CopyDictionary(dictionary4, null);
                dictionary4[sortOrderField] = result++;
                dsv.Update(keys, dictionary4, oldValues, delegate (int rowsAffected, Exception ex) {
                    if (ex != null)
                    {
                        throw new Exception("Failed to reorder.", ex);
                    }
                    return true;
                });
            }
            return true;
        }

        private void ExtractRowValues(IOrderedDictionary fieldValues, ReorderListItem item, bool includePrimaryKey, bool isAddOperation)
        {
            if (fieldValues != null)
            {
                IBindableTemplate itemTemplate = this.ItemTemplate as IBindableTemplate;
                if (!isAddOperation)
                {
                    ListItemType itemType = item.ItemType;
                    if (itemType != ListItemType.Item)
                    {
                        if (itemType != ListItemType.EditItem)
                        {
                            return;
                        }
                        itemTemplate = this.EditItemTemplate as IBindableTemplate;
                    }
                }
                else
                {
                    itemTemplate = this.InsertItemTemplate as IBindableTemplate;
                }
                if (itemTemplate != null)
                {
                    string dataKeyField = this.DataKeyField;
                    foreach (DictionaryEntry entry in itemTemplate.ExtractValues(item))
                    {
                        if (includePrimaryKey || (string.Compare((string) entry.Key, dataKeyField, StringComparison.OrdinalIgnoreCase) != 0))
                        {
                            fieldValues[entry.Key] = entry.Value;
                        }
                    }
                }
            }
        }

        protected WebControl GetDropTemplateControl(out Control dropItem, out Control emptyItem)
        {
            dropItem = null;
            emptyItem = null;
            if (!this.AllowReorder || base.DesignMode)
            {
                return null;
            }
            if (this._dropTemplateControl == null)
            {
                AjaxControlToolkit.BulletedList child = new AjaxControlToolkit.BulletedList {
                    Style = { 
                        ["visibility"] = "hidden",
                        ["display"] = "none"
                    }
                };
                BulletedListItem container = new BulletedListItem {
                    ID = "_dat",
                    Style = { ["vertical-align"] = "middle" }
                };
                if (this.ReorderTemplate == null)
                {
                    container.Style["border"] = "1px solid black";
                }
                else
                {
                    this.ReorderTemplate.InstantiateIn(container);
                }
                dropItem = container;
                child.Controls.Add(container);
                this._dropTemplateControl = child;
                this.Controls.Add(child);
            }
            else
            {
                dropItem = this._dropTemplateControl.FindControl("_dat");
                emptyItem = null;
            }
            return (WebControl) this._dropTemplateControl;
        }

        private ReorderListItem GetItem(ListItemType itemType, int repeatIndex)
        {
            switch (itemType)
            {
                case ListItemType.Item:
                case ListItemType.EditItem:
                    return (ReorderListItem) this.Controls[repeatIndex];

                case ListItemType.Separator:
                    return (ReorderListItem) this.Controls[repeatIndex * 2];
            }
            throw new ArgumentException("Unknown value", "itemType");
        }

        public Style GetItemStyle(ListItemType itemType, int repeatIndex) => 
            this.GetItem(itemType, repeatIndex).ControlStyle;

        private int GetNewItemSortValue(out bool success)
        {
            DataSourceView data = this.GetData();
            EventWaitHandle w = new EventWaitHandle(false, EventResetMode.AutoReset);
            int newIndex = 0;
            bool bSuccess = false;
            data.Select(new DataSourceSelectArguments(), delegate (IEnumerable dataSource) {
                try
                {
                    IList list = dataSource as IList;
                    if (list != null)
                    {
                        if (list.Count == 0)
                        {
                            bSuccess = true;
                        }
                        else
                        {
                            object component = null;
                            int num = 1;
                            if (this.ItemInsertLocation == ReorderListInsertLocation.End)
                            {
                                component = list[list.Count - 1];
                            }
                            else
                            {
                                component = list[0];
                                num = -1;
                            }
                            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(component)[this.SortOrderField];
                            if (descriptor != null)
                            {
                                object obj3 = descriptor.GetValue(component);
                                if (obj3 is int)
                                {
                                    newIndex = ((int) obj3) + num;
                                    bSuccess = true;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    w.Set();
                }
            });
            w.WaitOne();
            success = bSuccess;
            return newIndex;
        }

        protected V GetPropertyValue<V>(string propertyName, V nullValue)
        {
            if (this.ViewState[propertyName] == null)
            {
                return nullValue;
            }
            return (V) this.ViewState[propertyName];
        }

        private void HandleCancel(ReorderListCommandEventArgs e)
        {
            if (base.IsBoundUsingDataSourceID)
            {
                this.EditItemIndex = -1;
                base.RequiresDataBinding = true;
            }
            this.OnCancelCommand(e);
        }

        private void HandleDelete(ReorderListCommandEventArgs e)
        {
            DataSourceViewOperationCallback callback = null;
            if (base.IsBoundUsingDataSourceID)
            {
                DataSourceView data = this.GetData();
                if (data != null)
                {
                    IDictionary dictionary;
                    IOrderedDictionary dictionary2;
                    IDictionary dictionary3;
                    this.PrepareRowValues(e, out dictionary, out dictionary2, out dictionary3);
                    if (callback == null)
                    {
                        callback = delegate (int rows, Exception ex) {
                            if (ex != null)
                            {
                                return false;
                            }
                            this.OnDeleteCommand(e);
                            return true;
                        };
                    }
                    data.Delete(dictionary3, dictionary, callback);
                    return;
                }
            }
            this.OnDeleteCommand(e);
            base.RequiresDataBinding = true;
        }

        private void HandleEdit(ReorderListCommandEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                this.EditItemIndex = e.Item.ItemIndex;
                base.RequiresDataBinding = true;
            }
            this.OnEditCommand(e);
        }

        private void HandleInsert(ReorderListCommandEventArgs e)
        {
            DataSourceViewOperationCallback callback = null;
            if (base.IsBoundUsingDataSourceID && (this.SortOrderField != null))
            {
                IDictionary dictionary;
                IOrderedDictionary dictionary2;
                IDictionary dictionary3;
                bool flag;
                this.PrepareRowValues(e, out dictionary, out dictionary2, out dictionary3, true);
                DataSourceView data = this.GetData();
                int newItemSortValue = this.GetNewItemSortValue(out flag);
                if (flag)
                {
                    dictionary2[this.SortOrderField] = newItemSortValue;
                }
                if (data != null)
                {
                    if (callback == null)
                    {
                        callback = delegate (int rows, Exception ex) {
                            if (ex != null)
                            {
                                return false;
                            }
                            this.OnInsertCommand(e);
                            return true;
                        };
                    }
                    data.Insert(dictionary2, callback);
                    return;
                }
            }
            this.OnInsertCommand(e);
            base.RequiresDataBinding = true;
        }

        private void HandleUpdate(ReorderListCommandEventArgs e, int itemIndex)
        {
            DataSourceViewOperationCallback callback = null;
            if (base.IsBoundUsingDataSourceID)
            {
                IDictionary dictionary;
                IOrderedDictionary dictionary2;
                IDictionary dictionary3;
                if ((e == null) && (itemIndex != -1))
                {
                    e = new ReorderListCommandEventArgs(new CommandEventArgs("Update", null), this, (ReorderListItem) this.ChildList.Controls[itemIndex]);
                }
                this.PrepareRowValues(e, out dictionary, out dictionary2, out dictionary3);
                DataSourceView data = this.GetData();
                if (data != null)
                {
                    if (callback == null)
                    {
                        callback = delegate (int rows, Exception ex) {
                            if (ex != null)
                            {
                                return false;
                            }
                            this.OnUpdateCommand(e);
                            this.EditItemIndex = -1;
                            return true;
                        };
                    }
                    data.Update(dictionary3, dictionary2, dictionary, callback);
                    return;
                }
            }
            this.OnUpdateCommand(e);
        }

        protected void Invoke(object key, EventArgs e)
        {
            Delegate delegate2 = base.Events[key];
            if (delegate2 != null)
            {
                delegate2.DynamicInvoke(new object[] { this, e });
            }
        }

        private static void MoveChildren(Control source, Control dest)
        {
            for (int i = source.Controls.Count - 1; i >= 0; i--)
            {
                dest.Controls.AddAt(0, source.Controls[i]);
            }
        }

        protected override bool OnBubbleEvent(object source, EventArgs args)
        {
            ReorderListCommandEventArgs e = args as ReorderListCommandEventArgs;
            if (e != null)
            {
                string str2;
                this.OnItemCommand(e);
                if ((e.CommandArgument != null) && ((str2 = e.CommandName.ToString(CultureInfo.InvariantCulture).ToUpperInvariant()) != null))
                {
                    if (str2 == "INSERT")
                    {
                        this.HandleInsert(e);
                        return true;
                    }
                    if (str2 == "UPDATE")
                    {
                        this.HandleUpdate(e, -1);
                        return true;
                    }
                    if (str2 == "EDIT")
                    {
                        this.HandleEdit(e);
                        return true;
                    }
                    if (str2 == "DELETE")
                    {
                        this.HandleDelete(e);
                        return true;
                    }
                    if (str2 == "CANCEL")
                    {
                        this.HandleCancel(e);
                        return true;
                    }
                }
            }
            return false;
        }

        protected virtual void OnCancelCommand(EventArgs e)
        {
            this.Invoke(CancelCommandKey, e);
        }

        protected virtual void OnDeleteCommand(EventArgs e)
        {
            this.Invoke(DeleteCommandKey, e);
        }

        protected virtual void OnEditCommand(EventArgs e)
        {
            this.Invoke(EditCommandKey, e);
        }

        protected virtual void OnInsertCommand(EventArgs e)
        {
            this.Invoke(InsertCommandKey, e);
        }

        protected virtual void OnItemCommand(EventArgs e)
        {
            this.Invoke(ItemCommandKey, e);
        }

        protected virtual void OnItemCreated(EventArgs e)
        {
            this.Invoke(ItemCreatedKey, e);
        }

        protected virtual void OnItemDataBound(EventArgs e)
        {
            this.Invoke(ItemDataBoundKey, e);
        }

        protected virtual void OnItemReorder(ReorderListItemReorderEventArgs e)
        {
            try
            {
                if (((this.DataSource != null) || base.IsBoundUsingDataSourceID) && !this.DoReorder(e.OldIndex, e.NewIndex))
                {
                    throw new InvalidOperationException("Can't reorder data source.  It is not a DataSource and does not implement IList.");
                }
            }
            catch (Exception exception)
            {
                this.CallbackResult = exception.Message;
                throw;
            }
            this.Invoke(ItemReorderKey, e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (this.DataBindPending)
            {
                base.RequiresDataBinding = true;
            }
            base.OnPreRender(e);
        }

        protected virtual void OnUpdateCommand(EventArgs e)
        {
            this.Invoke(UpdateCommandKey, e);
        }

        private static bool ParsePostBack(string eventArgument, out string eventName, out string itemId, out string[] args)
        {
            itemId = null;
            eventName = null;
            args = new string[0];
            string[] sourceArray = eventArgument.Split(new char[] { ':' });
            if (sourceArray.Length < 2)
            {
                return false;
            }
            eventName = sourceArray[0];
            itemId = sourceArray[1];
            if (sourceArray.Length > 2)
            {
                args = new string[sourceArray.Length - 2];
                Array.Copy(sourceArray, 2, args, 0, args.Length);
            }
            return true;
        }

        protected override void PerformDataBinding(IEnumerable data)
        {
            this.ClearChildren();
            base.PerformDataBinding(data);
            if ((base.IsBoundUsingDataSourceID && (this.EditItemIndex != -1)) && ((this.EditItemIndex < this.Controls.Count) && base.IsViewStateEnabled))
            {
                this.BoundFieldValues.Clear();
                this.ExtractRowValues(this.BoundFieldValues, this.ChildList.Controls[this.EditItemIndex] as ReorderListItem, false, false);
            }
        }

        private void PrepareRowValues(ReorderListCommandEventArgs e, out IDictionary oldValues, out IOrderedDictionary newValues, out IDictionary keys)
        {
            this.PrepareRowValues(e, out oldValues, out newValues, out keys, false);
        }

        private void PrepareRowValues(ReorderListCommandEventArgs e, out IDictionary oldValues, out IOrderedDictionary newValues, out IDictionary keys, bool isAddOperation)
        {
            if (!isAddOperation)
            {
                oldValues = CopyDictionary(this.BoundFieldValues, null);
            }
            else
            {
                oldValues = null;
            }
            newValues = new OrderedDictionary((oldValues == null) ? 0 : oldValues.Count);
            if ((this.DataKeyField != null) && !isAddOperation)
            {
                keys = new OrderedDictionary(1);
                keys[this.DataKeyField] = this.DataKeysArray[e.Item.ItemIndex];
            }
            else
            {
                keys = null;
            }
            this.ExtractRowValues(newValues, e.Item, true, isAddOperation);
        }

        private void ProcessReorder(int oldIndex, int newIndex)
        {
            try
            {
                if ((oldIndex != newIndex) && (Math.Max(oldIndex, newIndex) != this.DataKeysArray.Count))
                {
                    Control control = this.Items[oldIndex];
                    this.OnItemReorder(new ReorderListItemReorderEventArgs(control as ReorderListItem, oldIndex, newIndex));
                }
            }
            catch (Exception)
            {
            }
        }

        protected void RaisePostBackEvent(string eventArgument)
        {
            string str;
            string str2;
            string[] strArray;
            string str3;
            if ((ParsePostBack(eventArgument, out str, out str2, out strArray) && ((str3 = str) != null)) && (str3 == "reorder"))
            {
                this.ProcessReorder(int.Parse(strArray[0], CultureInfo.InvariantCulture), int.Parse(strArray[1], CultureInfo.InvariantCulture));
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (this.ChildList.Controls.Count == 0)
            {
                if (this.EmptyListTemplate != null)
                {
                    Panel container = new Panel {
                        ID = this.ClientID
                    };
                    this.EmptyListTemplate.InstantiateIn(container);
                    container.RenderControl(writer);
                }
            }
            else
            {
                base.RenderContents(writer);
            }
        }

        public void RenderItem(ListItemType itemType, int repeatIndex, RepeatInfo repeatInfo, HtmlTextWriter writer)
        {
            this.GetItem(itemType, repeatIndex).RenderControl(writer);
        }

        protected void SetPropertyValue<V>(string propertyName, V value)
        {
            this.ViewState[propertyName] = value;
        }

        string ICallbackEventHandler.GetCallbackResult() => 
            this.CallbackResult;

        void ICallbackEventHandler.RaiseCallbackEvent(string eventArgument)
        {
            this.CallbackResult = string.Empty;
            this.RaisePostBackEvent(eventArgument);
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.CallbackResult = string.Empty;
            this.RaisePostBackEvent(eventArgument);
        }

        public void UpdateItem(int rowIndex)
        {
            this.HandleUpdate(null, rowIndex);
        }

        [DefaultValue(false)]
        public bool AllowReorder
        {
            get => 
                this.GetPropertyValue<bool>("AllowReorder", true);
            set
            {
                this.SetPropertyValue<bool>("AllowReorder", value);
            }
        }

        private IOrderedDictionary BoundFieldValues
        {
            get
            {
                if (this.ViewState["BoundFieldValues"] == null)
                {
                    IOrderedDictionary dictionary = new OrderedDictionary();
                    this.ViewState["BoundFieldValues"] = dictionary;
                }
                return (IOrderedDictionary) this.ViewState["BoundFieldValues"];
            }
        }

        [DefaultValue("")]
        public string CallbackCssStyle
        {
            get => 
                this.GetPropertyValue<string>("CallbackCssStyle", "");
            set
            {
                this.SetPropertyValue<string>("CallbackCssStyle", value);
            }
        }

        private string CallbackResult
        {
            get => 
                this._callbackResult;
            set
            {
                this._callbackResult = value;
            }
        }

        internal AjaxControlToolkit.BulletedList ChildList
        {
            get
            {
                if (this._childList == null)
                {
                    this._childList = new AjaxControlToolkit.BulletedList();
                    this._childList.ID = "_rbl";
                    this.Controls.Add(this._childList);
                }
                else if (this._childList.Parent == null)
                {
                    this.Controls.Add(this._childList);
                }
                return this._childList;
            }
        }

        private bool DataBindPending
        {
            get
            {
                this.EnsureChildControls();
                return ((this._dropWatcherExtender != null) && !string.IsNullOrEmpty(this._dropWatcherExtender.ClientState));
            }
        }

        [DefaultValue("")]
        public string DataKeyField
        {
            get => 
                this.GetPropertyValue<string>("DataKeyName", "");
            set
            {
                this.SetPropertyValue<string>("DataKeyName", value);
            }
        }

        [Browsable(false)]
        public DataKeyCollection DataKeys =>
            new DataKeyCollection(this.DataKeysArray);

        protected ArrayList DataKeysArray
        {
            get
            {
                if (this.ViewState["DataKeysArray"] == null)
                {
                    this.ViewState["DataKeysArray"] = new ArrayList();
                }
                return (ArrayList) this.ViewState["DataKeysArray"];
            }
        }

        [TypeConverter(typeof(TypedControlIDConverter<IDataSource>))]
        public override string DataSourceID
        {
            get => 
                base.DataSourceID;
            set
            {
                base.DataSourceID = value;
            }
        }

        [DefaultValue(1)]
        public ReorderHandleAlignment DragHandleAlignment
        {
            get => 
                this.GetPropertyValue<ReorderHandleAlignment>("DragHandleAlignment", ReorderHandleAlignment.Left);
            set
            {
                this.SetPropertyValue<ReorderHandleAlignment>("DragHandleAlignment", value);
            }
        }

        [DefaultValue(""), Browsable(false), TemplateContainer(typeof(ReorderListItem)), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate DragHandleTemplate
        {
            get => 
                this._dragHandleTemplate;
            set
            {
                this._dragHandleTemplate = value;
            }
        }

        [DefaultValue(-1)]
        public int EditItemIndex
        {
            get => 
                this.GetPropertyValue<int>("EditItemIndex", -1);
            set
            {
                this.SetPropertyValue<int>("EditItemIndex", value);
            }
        }

        [TemplateContainer(typeof(IDataItemContainer), BindingDirection.TwoWay), DefaultValue(""), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate EditItemTemplate
        {
            get => 
                this._editItemTemplate;
            set
            {
                this._editItemTemplate = value;
            }
        }

        [TemplateContainer(typeof(ReorderListItem)), DefaultValue(""), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate EmptyListTemplate
        {
            get => 
                this._emptyListTemplate;
            set
            {
                this._emptyListTemplate = value;
            }
        }

        public bool HasFooter =>
            false;

        public bool HasHeader =>
            false;

        public bool HasSeparators =>
            false;

        [Browsable(false), DefaultValue(""), TemplateContainer(typeof(IDataItemContainer), BindingDirection.TwoWay), PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate InsertItemTemplate
        {
            get => 
                this._insertItemTemplate;
            set
            {
                this._insertItemTemplate = value;
            }
        }

        [DefaultValue(0)]
        public ReorderListInsertLocation ItemInsertLocation
        {
            get => 
                this.GetPropertyValue<ReorderListInsertLocation>("ItemInsertLocation", ReorderListInsertLocation.Beginning);
            set
            {
                this.SetPropertyValue<ReorderListInsertLocation>("ItemInsertLocation", value);
            }
        }

        [Browsable(false)]
        public ReorderListItemCollection Items
        {
            get
            {
                this.EnsureDataBound();
                return new ReorderListItemCollection(this);
            }
        }

        [TemplateContainer(typeof(IDataItemContainer), BindingDirection.TwoWay), DefaultValue(""), PersistenceMode(PersistenceMode.InnerProperty), Browsable(false)]
        public ITemplate ItemTemplate
        {
            get => 
                this._itemTemplate;
            set
            {
                this._itemTemplate = value;
            }
        }

        [DefaultValue(1)]
        public ReorderListItemLayoutType LayoutType
        {
            get => 
                this._layoutType;
            set
            {
                this._layoutType = value;
            }
        }

        [DefaultValue("true")]
        public bool PostBackOnReorder
        {
            get => 
                this.GetPropertyValue<bool>("PostBackOnReorder", false);
            set
            {
                this.SetPropertyValue<bool>("PostBackOnReorder", value);
            }
        }

        [DefaultValue(""), Browsable(false), TemplateContainer(typeof(ReorderListItem)), PersistenceMode(PersistenceMode.InnerDefaultProperty)]
        public ITemplate ReorderTemplate
        {
            get => 
                this._reorderTemplate;
            set
            {
                this._reorderTemplate = value;
            }
        }

        public int RepeatedItemCount
        {
            get
            {
                if (this.itemsArray != null)
                {
                    return this.itemsArray.Count;
                }
                return 0;
            }
        }

        private string ScriptResourceUrl
        {
            get
            {
                string absoluteUri = this.Page.Request.Url.AbsoluteUri;
                return (absoluteUri.Substring(0, absoluteUri.Length - this.Page.Request.Url.PathAndQuery.Length) + this.Page.ClientScript.GetWebResourceUrl(typeof(ReorderList), "WebUtilControls.DragDropList.js").Replace("&", "&amp;"));
            }
        }

        [DefaultValue(false)]
        public bool ShowInsertItem
        {
            get => 
                this.GetPropertyValue<bool>("ShowInsertItem", this.InsertItemTemplate != null);
            set
            {
                this.SetPropertyValue<bool>("ShowInsertItem", value);
            }
        }

        [DefaultValue("")]
        public string SortOrderField
        {
            get => 
                this.GetPropertyValue<string>("SortOrderField", "");
            set
            {
                this.SetPropertyValue<string>("SortOrderField", value);
            }
        }

        protected override HtmlTextWriterTag TagKey =>
            HtmlTextWriterTag.Div;

        private class DraggableListItemInfo
        {
            public DraggableListItemExtender Extender;
            public Control HandleControl;
            public Control TargetControl;
        }
    }
}

