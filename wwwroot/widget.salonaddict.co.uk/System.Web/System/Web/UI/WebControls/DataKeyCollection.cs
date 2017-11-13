namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DataKeyCollection : ICollection, IEnumerable
    {
        private ArrayList keys;

        public DataKeyCollection(ArrayList keys)
        {
            this.keys = keys;
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
            this.keys.GetEnumerator();

        public int Count =>
            this.keys.Count;

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public object this[int index] =>
            this.keys[index];

        public object SyncRoot =>
            this;
    }
}

