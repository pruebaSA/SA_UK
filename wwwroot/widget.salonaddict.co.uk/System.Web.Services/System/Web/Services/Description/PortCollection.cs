namespace System.Web.Services.Description
{
    using System;
    using System.Reflection;

    public sealed class PortCollection : ServiceDescriptionBaseCollection
    {
        internal PortCollection(Service service) : base(service)
        {
        }

        public int Add(Port port) => 
            base.List.Add(port);

        public bool Contains(Port port) => 
            base.List.Contains(port);

        public void CopyTo(Port[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        protected override string GetKey(object value) => 
            ((Port) value).Name;

        public int IndexOf(Port port) => 
            base.List.IndexOf(port);

        public void Insert(int index, Port port)
        {
            base.List.Insert(index, port);
        }

        public void Remove(Port port)
        {
            base.List.Remove(port);
        }

        protected override void SetParent(object value, object parent)
        {
            ((Port) value).SetParent((Service) parent);
        }

        public Port this[int index]
        {
            get => 
                ((Port) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }

        public Port this[string name] =>
            ((Port) this.Table[name]);
    }
}

