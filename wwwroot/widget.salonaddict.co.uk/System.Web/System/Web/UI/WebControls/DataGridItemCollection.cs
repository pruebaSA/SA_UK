namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataGridItemCollection : ICollection, IEnumerable
    {
        private ArrayList items;

        public DataGridItemCollection(ArrayList items)
        {
            this.items = items;
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
            this.items.GetEnumerator();

        public int Count =>
            this.items.Count;

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public DataGridItem this[int index] =>
            ((DataGridItem) this.items[index]);

        public object SyncRoot =>
            this;
    }
}

