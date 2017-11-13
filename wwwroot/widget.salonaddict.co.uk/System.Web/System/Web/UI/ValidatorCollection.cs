namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ValidatorCollection : ICollection, IEnumerable
    {
        private ArrayList data = new ArrayList();

        public void Add(IValidator validator)
        {
            this.data.Add(validator);
        }

        public bool Contains(IValidator validator) => 
            this.data.Contains(validator);

        public void CopyTo(Array array, int index)
        {
            IEnumerator enumerator = this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                array.SetValue(enumerator.Current, index++);
            }
        }

        public IEnumerator GetEnumerator() => 
            this.data.GetEnumerator();

        public void Remove(IValidator validator)
        {
            this.data.Remove(validator);
        }

        public int Count =>
            this.data.Count;

        public bool IsReadOnly =>
            false;

        public bool IsSynchronized =>
            false;

        public IValidator this[int index] =>
            ((IValidator) this.data[index]);

        public object SyncRoot =>
            this;
    }
}

