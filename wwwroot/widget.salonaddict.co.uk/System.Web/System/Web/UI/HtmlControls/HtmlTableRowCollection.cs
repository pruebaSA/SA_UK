﻿namespace System.Web.UI.HtmlControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HtmlTableRowCollection : ICollection, IEnumerable
    {
        private HtmlTable owner;

        internal HtmlTableRowCollection(HtmlTable owner)
        {
            this.owner = owner;
        }

        public void Add(HtmlTableRow row)
        {
            this.Insert(-1, row);
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

        public void Insert(int index, HtmlTableRow row)
        {
            this.owner.Controls.AddAt(index, row);
        }

        public void Remove(HtmlTableRow row)
        {
            this.owner.Controls.Remove(row);
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

        public HtmlTableRow this[int index] =>
            ((HtmlTableRow) this.owner.Controls[index]);

        public object SyncRoot =>
            this;
    }
}

