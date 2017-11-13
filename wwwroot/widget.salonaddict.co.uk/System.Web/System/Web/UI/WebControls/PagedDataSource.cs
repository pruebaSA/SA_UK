namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PagedDataSource : ICollection, IEnumerable, ITypedList
    {
        private bool allowCustomPaging = false;
        private bool allowPaging = false;
        private bool allowServerPaging = false;
        private int currentPageIndex = 0;
        private IEnumerable dataSource;
        private int pageSize = 10;
        private int virtualCount = 0;

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
            int firstIndexInPage = this.FirstIndexInPage;
            int count = -1;
            if (this.dataSource is ICollection)
            {
                count = this.Count;
            }
            if (this.dataSource is IList)
            {
                return new EnumeratorOnIList((IList) this.dataSource, firstIndexInPage, count);
            }
            if (this.dataSource is Array)
            {
                return new EnumeratorOnArray((object[]) this.dataSource, firstIndexInPage, count);
            }
            if (this.dataSource is ICollection)
            {
                return new EnumeratorOnICollection((ICollection) this.dataSource, firstIndexInPage, count);
            }
            if (!this.allowCustomPaging && !this.allowServerPaging)
            {
                return this.dataSource.GetEnumerator();
            }
            return new EnumeratorOnIEnumerator(this.dataSource.GetEnumerator(), this.Count);
        }

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if ((this.dataSource != null) && (this.dataSource is ITypedList))
            {
                return ((ITypedList) this.dataSource).GetItemProperties(listAccessors);
            }
            return null;
        }

        public string GetListName(PropertyDescriptor[] listAccessors) => 
            string.Empty;

        public bool AllowCustomPaging
        {
            get => 
                this.allowCustomPaging;
            set
            {
                this.allowCustomPaging = value;
            }
        }

        public bool AllowPaging
        {
            get => 
                this.allowPaging;
            set
            {
                this.allowPaging = value;
            }
        }

        public bool AllowServerPaging
        {
            get => 
                this.allowServerPaging;
            set
            {
                this.allowServerPaging = value;
            }
        }

        public int Count
        {
            get
            {
                if (this.dataSource == null)
                {
                    return 0;
                }
                if (!this.IsPagingEnabled)
                {
                    return this.DataSourceCount;
                }
                if (!this.IsCustomPagingEnabled && this.IsLastPage)
                {
                    return (this.DataSourceCount - this.FirstIndexInPage);
                }
                return this.pageSize;
            }
        }

        public int CurrentPageIndex
        {
            get => 
                this.currentPageIndex;
            set
            {
                this.currentPageIndex = value;
            }
        }

        public IEnumerable DataSource
        {
            get => 
                this.dataSource;
            set
            {
                this.dataSource = value;
            }
        }

        public int DataSourceCount
        {
            get
            {
                if (this.dataSource == null)
                {
                    return 0;
                }
                if (this.IsCustomPagingEnabled || this.IsServerPagingEnabled)
                {
                    return this.virtualCount;
                }
                if (!(this.dataSource is ICollection))
                {
                    throw new HttpException(System.Web.SR.GetString("PagedDataSource_Cannot_Get_Count"));
                }
                return ((ICollection) this.dataSource).Count;
            }
        }

        public int FirstIndexInPage
        {
            get
            {
                if (((this.dataSource != null) && this.IsPagingEnabled) && (!this.IsCustomPagingEnabled && !this.IsServerPagingEnabled))
                {
                    return (this.currentPageIndex * this.pageSize);
                }
                return 0;
            }
        }

        public bool IsCustomPagingEnabled =>
            (this.IsPagingEnabled && this.allowCustomPaging);

        public bool IsFirstPage
        {
            get
            {
                if (this.IsPagingEnabled)
                {
                    return (this.CurrentPageIndex == 0);
                }
                return true;
            }
        }

        public bool IsLastPage
        {
            get
            {
                if (this.IsPagingEnabled)
                {
                    return (this.CurrentPageIndex == (this.PageCount - 1));
                }
                return true;
            }
        }

        public bool IsPagingEnabled =>
            (this.allowPaging && (this.pageSize != 0));

        public bool IsReadOnly =>
            false;

        public bool IsServerPagingEnabled =>
            (this.IsPagingEnabled && this.allowServerPaging);

        public bool IsSynchronized =>
            false;

        public int PageCount
        {
            get
            {
                if (this.dataSource == null)
                {
                    return 0;
                }
                int dataSourceCount = this.DataSourceCount;
                if (!this.IsPagingEnabled || (dataSourceCount <= 0))
                {
                    return 1;
                }
                int num2 = (dataSourceCount + this.pageSize) - 1;
                if (num2 < 0)
                {
                    return 1;
                }
                return (num2 / this.pageSize);
            }
        }

        public int PageSize
        {
            get => 
                this.pageSize;
            set
            {
                this.pageSize = value;
            }
        }

        public object SyncRoot =>
            this;

        public int VirtualCount
        {
            get => 
                this.virtualCount;
            set
            {
                this.virtualCount = value;
            }
        }

        private sealed class EnumeratorOnArray : IEnumerator
        {
            private object[] array;
            private int index;
            private int indexBounds;
            private int startIndex;

            public EnumeratorOnArray(object[] array, int startIndex, int count)
            {
                this.array = array;
                this.startIndex = startIndex;
                this.index = -1;
                this.indexBounds = startIndex + count;
                if (this.indexBounds > array.Length)
                {
                    this.indexBounds = array.Length;
                }
            }

            public bool MoveNext()
            {
                this.index++;
                return ((this.startIndex + this.index) < this.indexBounds);
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
                        throw new InvalidOperationException(System.Web.SR.GetString("Enumerator_MoveNext_Not_Called"));
                    }
                    return this.array[this.startIndex + this.index];
                }
            }
        }

        private sealed class EnumeratorOnICollection : IEnumerator
        {
            private ICollection collection;
            private IEnumerator collectionEnum;
            private int index;
            private int indexBounds;
            private int startIndex;

            public EnumeratorOnICollection(ICollection collection, int startIndex, int count)
            {
                this.collection = collection;
                this.startIndex = startIndex;
                this.index = -1;
                this.indexBounds = startIndex + count;
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
                    for (int i = 0; i < this.startIndex; i++)
                    {
                        this.collectionEnum.MoveNext();
                    }
                }
                this.collectionEnum.MoveNext();
                this.index++;
                return ((this.startIndex + this.index) < this.indexBounds);
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
            private int startIndex;

            public EnumeratorOnIList(IList collection, int startIndex, int count)
            {
                this.collection = collection;
                this.startIndex = startIndex;
                this.index = -1;
                this.indexBounds = startIndex + count;
                if (this.indexBounds > collection.Count)
                {
                    this.indexBounds = collection.Count;
                }
            }

            public bool MoveNext()
            {
                this.index++;
                return ((this.startIndex + this.index) < this.indexBounds);
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
                        throw new InvalidOperationException(System.Web.SR.GetString("Enumerator_MoveNext_Not_Called"));
                    }
                    return this.collection[this.startIndex + this.index];
                }
            }
        }
    }
}

