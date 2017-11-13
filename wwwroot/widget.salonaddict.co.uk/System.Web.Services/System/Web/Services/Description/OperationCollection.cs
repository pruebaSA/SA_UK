namespace System.Web.Services.Description
{
    using System;
    using System.Reflection;

    public sealed class OperationCollection : ServiceDescriptionBaseCollection
    {
        internal OperationCollection(PortType portType) : base(portType)
        {
        }

        public int Add(Operation operation) => 
            base.List.Add(operation);

        public bool Contains(Operation operation) => 
            base.List.Contains(operation);

        public void CopyTo(Operation[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        public int IndexOf(Operation operation) => 
            base.List.IndexOf(operation);

        public void Insert(int index, Operation operation)
        {
            base.List.Insert(index, operation);
        }

        public void Remove(Operation operation)
        {
            base.List.Remove(operation);
        }

        protected override void SetParent(object value, object parent)
        {
            ((Operation) value).SetParent((PortType) parent);
        }

        public Operation this[int index]
        {
            get => 
                ((Operation) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }
    }
}

