﻿namespace System.Web.UI.HtmlControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HtmlTableCellCollection : ICollection, IEnumerable
    {
        private HtmlTableRow owner;

        internal HtmlTableCellCollection(HtmlTableRow owner)
        {
            this.owner = owner;
        }

        public void Add(HtmlTableCell cell)
        {
            this.Insert(-1, cell);
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

        public IEnumerator GetEnumerator() => 
            this.owner.Controls.GetEnumerator();

        public void Insert(int index, HtmlTableCell cell)
        {
            this.owner.Controls.AddAt(index, cell);
        }

        public void Remove(HtmlTableCell cell)
        {
            this.owner.Controls.Remove(cell);
        }

        public void RemoveAt(int index)
        {
            this.owner.Controls.RemoveAt(index);
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

        public HtmlTableCell this[int index] =>
            ((HtmlTableCell) this.owner.Controls[index]);

        public object SyncRoot =>
            this;
    }
}

