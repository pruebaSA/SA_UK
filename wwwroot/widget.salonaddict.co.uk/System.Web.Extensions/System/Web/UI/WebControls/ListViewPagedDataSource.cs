namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Resources;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewPagedDataSource : ICollection, IEnumerable, ITypedList
    {
        private bool _allowServerPaging = false;
        private IEnumerable _dataSource;
        private int _maximumRows;
        private int _startRowIndex;
        private int _totalRowCount = 0;

        public void CopyTo(Array array, int index)
        {
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array.SetValue(enumerator.Current, index++);
            }
        }

        public IEnumerator GetEnumerator()
        {
            int startRowIndex = 0;
            int count = -1;
            if (!this.IsServerPagingEnabled)
            {
                startRowIndex = this.StartRowIndex;
            }
            if (this._dataSource is ICollection)
            {
                count = this.Count;
            }
            if (this._dataSource is IList)
            {
                return new EnumeratorOnIList((IList) this._dataSource, startRowIndex, count);
            }
            if (this._dataSource is Array)
            {
                return new EnumeratorOnArray((object[]) this._dataSource, startRowIndex, count);
            }
            if (this._dataSource is ICollection)
            {
                return new EnumeratorOnICollection((ICollection) this._dataSource, startRowIndex, count);
            }
            if (this._allowServerPaging)
            {
                return new EnumeratorOnIEnumerator(this._dataSource.GetEnumerator(), this.Count);
            }
            return this._dataSource.GetEnumerator();
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if ((this._dataSource != null) && (this._dataSource is ITypedList))
            {
                return ((ITypedList) this._dataSource).GetItemProperties(listAccessors);
            }
            return null;
        }

        public string GetListName(PropertyDescriptor[] listAccessors) => 
            string.Empty;

        public bool AllowServerPaging
        {
            get => 
                this._allowServerPaging;
            set
            {
                this._allowServerPaging = value;
            }
        }

        public int Count
        {
            get
            {
                if (this._dataSource == null)
                {
                    return 0;
                }
                if (!this.IsLastPage && (this.MaximumRows >= 0))
                {
                    return this.MaximumRows;
                }
                return (this.DataSourceCount - this.StartRowIndex);
            }
        }

        public IEnumerable DataSource
        {
            get => 
                this._dataSource;
            set
            {
                this._dataSource = value;
            }
        }

        public int DataSourceCount
        {
            get
            {
                if (this._dataSource == null)
                {
                    return 0;
                }
                if (this.IsServerPagingEnabled)
                {
                    return this._totalRowCount;
                }
                if (!(this._dataSource is ICollection))
                {
                    throw new InvalidOperationException(AtlasWeb.ListViewPagedDataSource_CannotGetCount);
                }
                return ((ICollection) this._dataSource).Count;
            }
        }

        private bool IsLastPage =>
            ((this.StartRowIndex + this.MaximumRows) >= this.DataSourceCount);

        public bool IsReadOnly =>
            false;

        public bool IsServerPagingEnabled =>
            this._allowServerPaging;

        public bool IsSynchronized =>
            false;

        public int MaximumRows
        {
            get => 
                this._maximumRows;
            set
            {
                this._maximumRows = value;
            }
        }

        public int StartRowIndex
        {
            get => 
                this._startRowIndex;
            set
            {
                this._startRowIndex = value;
            }
        }

        public object SyncRoot =>
            this;

        public int TotalRowCount
        {
            get => 
                this._totalRowCount;
            set
            {
                this._totalRowCount = value;
            }
        }

        private sealed class EnumeratorOnArray : IEnumerator
        {
            private object[] array;
            private int index;
            private int indexBounds;
            private int startRowIndex;

            public EnumeratorOnArray(object[] array, int startRowIndex, int count)
            {
                this.array = array;
                this.startRowIndex = startRowIndex;
                this.index = -1;
                this.indexBounds = startRowIndex + count;
                if (this.indexBounds > array.Length)
                {
                    this.indexBounds = array.Length;
                }
            }

            public bool MoveNext()
            {
                this.index++;
                return ((this.startRowIndex + this.index) < this.indexBounds);
            }

            public void Reset()
            {
                this.index = -1;
            }

            public object Current
            {
                get
                {
                    if (this.index < 0)
                    {
                        throw new InvalidOperationException(AtlasWeb.ListViewPagedDataSource_EnumeratorMoveNextNotCalled);
                    }
                    return this.array[this.startRowIndex + this.index];
                }
            }
        }

        private sealed class EnumeratorOnICollection : IEnumerator
        {
            private ICollection collection;
            private IEnumerator collectionEnum;
            private int index;
            private int indexBounds;
            private int startRowIndex;

            public EnumeratorOnICollection(ICollection collection, int startRowIndex, int count)
            {
                this.collection = collection;
                this.startRowIndex = startRowIndex;
                this.index = -1;
                this.indexBounds = startRowIndex + count;
                if (this.indexBounds > collection.Count)
                {
                    this.indexBounds = collection.Count;
                }
            }

            public bool MoveNext()
            {
                if (this.collectionEnum == null)
                {
                    this.collectionEnum = this.collection.GetEnumerator();
                    for (int i = 0; i < this.startRowIndex; i++)
                    {
                        this.collectionEnum.MoveNext();
                    }
                }
                this.collectionEnum.MoveNext();
                this.index++;
                return ((this.startRowIndex + this.index) < this.indexBounds);
            }

            public void Reset()
            {
                this.collectionEnum = null;
                this.index = -1;
            }

            public object Current =>
                this.collectionEnum.Current;
        }

        private sealed class EnumeratorOnIEnumerator : IEnumerator
        {
            private int index;
            private int indexBounds;
            private IEnumerator realEnum;

            public EnumeratorOnIEnumerator(IEnumerator realEnum, int count)
            {
                this.realEnum = realEnum;
                this.index = -1;
                this.indexBounds = count;
            }

            public bool MoveNext()
            {
                bool flag = this.realEnum.MoveNext();
                this.index++;
                return (flag && (this.index < this.indexBounds));
            }

            public void Reset()
            {
                this.realEnum.Reset();
                this.index = -1;
            }

            public object Current =>
                this.realEnum.Current;
        }

        private sealed class EnumeratorOnIList : IEnumerator
        {
            private IList collection;
            private int index;
            private int indexBounds;
            private int startRowIndex;

            public EnumeratorOnIList(IList collection, int startRowIndex, int count)
            {
                this.collection = collection;
                this.startRowIndex = startRowIndex;
                this.index = -1;
                this.indexBounds = startRowIndex + count;
                if (this.indexBounds > collection.Count)
                {
                    this.indexBounds = collection.Count;
                }
            }

            public bool MoveNext()
            {
                this.index++;
                return ((this.startRowIndex + this.index) < this.indexBounds);
            }

            public void Reset()
            {
                this.index = -1;
            }

            public object Current
            {
                get
                {
                    if (this.index < 0)
                    {
                        throw new InvalidOperationException(AtlasWeb.ListViewPagedDataSource_EnumeratorMoveNextNotCalled);
                    }
                    return this.collection[this.startRowIndex + this.index];
                }
            }
        }
    }
}

