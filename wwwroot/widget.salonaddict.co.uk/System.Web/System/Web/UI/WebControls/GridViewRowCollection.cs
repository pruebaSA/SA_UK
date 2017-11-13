namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewRowCollection : ICollection, IEnumerable
    {
        private ArrayList _rows;

        public GridViewRowCollection(ArrayList rows)
        {
            this._rows = rows;
        }

        public void CopyTo(GridViewRow[] array, int index)
        {
            ((ICollection) this).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() => 
            this._rows.GetEnumerator();

        void ICollection.CopyTo(Array array, int index)
        {
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array.SetValue(enumerator.Current, index++);
            }
        }

        public int Count =>
            this._rows.Count;

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public GridViewRow this[int index] =>
            ((GridViewRow) this._rows[index]);

        public object SyncRoot =>
            this;
    }
}

