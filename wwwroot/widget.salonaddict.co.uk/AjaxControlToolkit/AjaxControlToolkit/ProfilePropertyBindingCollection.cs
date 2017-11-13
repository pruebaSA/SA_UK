namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public class ProfilePropertyBindingCollection : CollectionBase
    {
        internal event EventHandler CollectionChanged;

        internal ProfilePropertyBindingCollection()
        {
        }

        public void Add(ProfilePropertyBinding binding)
        {
            base.InnerList.Add(binding);
        }

        public void Insert(int index, ProfilePropertyBinding binding)
        {
            base.InnerList.Insert(index, binding);
        }

        protected override void OnClearComplete()
        {
            base.OnClearComplete();
            this.OnCollectionChanged(EventArgs.Empty);
        }

        protected virtual void OnCollectionChanged(EventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        protected override void OnInsertComplete(int index, object value)
        {
            base.OnInsertComplete(index, value);
            this.OnCollectionChanged(EventArgs.Empty);
        }

        protected override void OnRemoveComplete(int index, object value)
        {
            base.OnRemoveComplete(index, value);
            this.OnCollectionChanged(EventArgs.Empty);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            base.OnSetComplete(index, oldValue, newValue);
            this.OnCollectionChanged(EventArgs.Empty);
        }

        public void Remove(ProfilePropertyBinding binding)
        {
            base.InnerList.Remove(binding);
        }

        public ProfilePropertyBinding this[int index]
        {
            get => 
                ((ProfilePropertyBinding) base.InnerList[index]);
            set
            {
                base.InnerList[index] = value;
            }
        }
    }
}

