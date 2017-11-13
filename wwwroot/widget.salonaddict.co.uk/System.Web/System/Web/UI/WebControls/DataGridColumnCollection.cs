namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DataGridColumnCollection : ICollection, IEnumerable, IStateManager
    {
        private ArrayList columns;
        private bool marked;
        private DataGrid owner;

        public DataGridColumnCollection(DataGrid owner, ArrayList columns)
        {
            this.owner = owner;
            this.columns = columns;
        }

        public void Add(DataGridColumn column)
        {
            this.AddAt(-1, column);
        }

        public void AddAt(int index, DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }
            if (index == -1)
            {
                this.columns.Add(column);
            }
            else
            {
                this.columns.Insert(index, column);
            }
            column.SetOwner(this.owner);
            if (this.marked)
            {
                ((IStateManager) column).TrackViewState();
            }
            this.OnColumnsChanged();
        }

        public void Clear()
        {
            this.columns.Clear();
            this.OnColumnsChanged();
        }

        public void CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array.SetValue(enumerator.Current, index++);
            }
        }

        public IEnumerator GetEnumerator() => 
            this.columns.GetEnumerator();

        public int IndexOf(DataGridColumn column)
        {
            if (column != null)
            {
                return this.columns.IndexOf(column);
            }
            return -1;
        }

        private void OnColumnsChanged()
        {
            if (this.owner != null)
            {
                this.owner.OnColumnsChanged();
            }
        }

        public void Remove(DataGridColumn column)
        {
            int index = this.IndexOf(column);
            if (index >= 0)
            {
                this.RemoveAt(index);
            }
        }

        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= this.Count))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            this.columns.RemoveAt(index);
            this.OnColumnsChanged();
        }

        void IStateManager.LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] objArray = (object[]) savedState;
                if (objArray.Length == this.columns.Count)
                {
                    for (int i = 0; i < objArray.Length; i++)
                    {
                        if (objArray[i] != null)
                        {
                            ((IStateManager) this.columns[i]).LoadViewState(objArray[i]);
                        }
                    }
                }
            }
        }

        object IStateManager.SaveViewState()
        {
            int count = this.columns.Count;
            object[] objArray = new object[count];
            bool flag = false;
            for (int i = 0; i < count; i++)
            {
                objArray[i] = ((IStateManager) this.columns[i]).SaveViewState();
                if (objArray[i] != null)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                return null;
            }
            return objArray;
        }

        void IStateManager.TrackViewState()
        {
            this.marked = true;
            int count = this.columns.Count;
            for (int i = 0; i < count; i++)
            {
                ((IStateManager) this.columns[i]).TrackViewState();
            }
        }

        [Browsable(false)]
        public int Count =>
            this.columns.Count;

        [Browsable(false)]
        public bool IsReadOnly =>
            false;

        [Browsable(false)]
        public bool IsSynchronized =>
            false;

        [Browsable(false)]
        public DataGridColumn this[int index] =>
            ((DataGridColumn) this.columns[index]);

        [Browsable(false)]
        public object SyncRoot =>
            this;

        bool IStateManager.IsTrackingViewState =>
            this.marked;
    }
}

