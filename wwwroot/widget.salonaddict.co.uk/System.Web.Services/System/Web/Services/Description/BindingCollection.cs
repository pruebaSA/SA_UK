namespace System.Web.Services.Description
{
    using System;
    using System.Reflection;

    public sealed class BindingCollection : ServiceDescriptionBaseCollection
    {
        internal BindingCollection(ServiceDescription serviceDescription) : base(serviceDescription)
        {
        }

        public int Add(Binding binding) => 
            base.List.Add(binding);

        public bool Contains(Binding binding) => 
            base.List.Contains(binding);

        public void CopyTo(Binding[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        protected override string GetKey(object value) => 
            ((Binding) value).Name;

        public int IndexOf(Binding binding) => 
            base.List.IndexOf(binding);

        public void Insert(int index, Binding binding)
        {
            base.List.Insert(index, binding);
        }

        public void Remove(Binding binding)
        {
            base.List.Remove(binding);
        }

        protected override void SetParent(object value, object parent)
        {
            ((Binding) value).SetParent((ServiceDescription) parent);
        }

        public Binding this[int index]
        {
            get => 
                ((Binding) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }

        public Binding this[string name] =>
            ((Binding) this.Table[name]);
    }
}

