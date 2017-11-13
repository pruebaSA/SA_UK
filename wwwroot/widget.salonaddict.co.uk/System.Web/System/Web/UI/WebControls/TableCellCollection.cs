namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [Editor("System.Web.UI.Design.WebControls.TableCellsCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TableCellCollection : IList, ICollection, IEnumerable
    {
        private TableRow owner;

        internal TableCellCollection(TableRow owner)
        {
            this.owner = owner;
        }

        public int Add(TableCell cell)
        {
            this.AddAt(-1, cell);
            return (this.owner.Controls.Count - 1);
        }

        public void AddAt(int index, TableCell cell)
        {
            this.owner.Controls.AddAt(index, cell);
        }

        public void AddRange(TableCell[] cells)
        {
            if (cells == null)
            {
                throw new ArgumentNullException("cells");
            }
            foreach (TableCell cell in cells)
            {
                this.Add(cell);
            }
        }

        public void Clear()
        {
            if (this.owner.HasControls())
            {
                this.owner.Controls.Clear();
            }
        }

        public void CopyTo(Array array, int index)
        {
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array.SetValue(enumerator.Current, index++);
            }
        }

        public int GetCellIndex(TableCell cell)
        {
            if (this.owner.HasControls())
            {
                return this.owner.Controls.IndexOf(cell);
            }
            return -1;
        }

        public IEnumerator GetEnumerator() => 
            this.owner.Controls.GetEnumerator();

        public void Remove(TableCell cell)
        {
            this.owner.Controls.Remove(cell);
        }

        public void RemoveAt(int index)
        {
            this.owner.Controls.RemoveAt(index);
        }

        int IList.Add(object o) => 
            this.Add((TableCell) o);

        bool IList.Contains(object o) => 
            this.owner.Controls.Contains((TableCell) o);

        int IList.IndexOf(object o) => 
            this.owner.Controls.IndexOf((TableCell) o);

        void IList.Insert(int index, object o)
        {
            this.owner.Controls.AddAt(index, (TableCell) o);
        }

        void IList.Remove(object o)
        {
            this.owner.Controls.Remove((TableCell) o);
        }

        public int Count
        {
            get
            {
                if (this.owner.HasControls())
                {
                    return this.owner.Controls.Count;
                }
                return 0;
            }
        }

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public TableCell this[int index] =>
            ((TableCell) this.owner.Controls[index]);

        public object SyncRoot =>
            this;

        bool IList.IsFixedSize =>
            false;

        object IList.this[int index]
        {
            get => 
                this.owner.Controls[index];
            set
            {
                this.RemoveAt(index);
                this.AddAt(index, (TableCell) value);
            }
        }
    }
}

