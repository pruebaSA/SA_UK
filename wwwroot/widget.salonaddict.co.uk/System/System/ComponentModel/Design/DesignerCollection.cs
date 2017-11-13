namespace System.ComponentModel.Design
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, SharedState=true), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class DesignerCollection : ICollection, IEnumerable
    {
        private IList designers;

        public DesignerCollection(IDesignerHost[] designers)
        {
            if (designers != null)
            {
                this.designers = new ArrayList(designers);
            }
            else
            {
                this.designers = new ArrayList();
            }
        }

        public DesignerCollection(IList designers)
        {
            this.designers = designers;
        }

        public IEnumerator GetEnumerator() => 
            this.designers.GetEnumerator();

        void ICollection.CopyTo(Array array, int index)
        {
            this.designers.CopyTo(array, index);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public int Count =>
            this.designers.Count;

        public virtual IDesignerHost this[int index] =>
            ((IDesignerHost) this.designers[index]);

        int ICollection.Count =>
            this.Count;

        bool ICollection.IsSynchronized =>
            false;

        object ICollection.SyncRoot =>
            null;
    }
}

