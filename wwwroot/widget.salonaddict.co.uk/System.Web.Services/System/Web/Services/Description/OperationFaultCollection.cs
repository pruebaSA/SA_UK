namespace System.Web.Services.Description
{
    using System;
    using System.Reflection;

    public sealed class OperationFaultCollection : ServiceDescriptionBaseCollection
    {
        internal OperationFaultCollection(Operation operation) : base(operation)
        {
        }

        public int Add(OperationFault operationFaultMessage) => 
            base.List.Add(operationFaultMessage);

        public bool Contains(OperationFault operationFaultMessage) => 
            base.List.Contains(operationFaultMessage);

        public void CopyTo(OperationFault[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        protected override string GetKey(object value) => 
            ((OperationFault) value).Name;

        public int IndexOf(OperationFault operationFaultMessage) => 
            base.List.IndexOf(operationFaultMessage);

        public void Insert(int index, OperationFault operationFaultMessage)
        {
            base.List.Insert(index, operationFaultMessage);
        }

        public void Remove(OperationFault operationFaultMessage)
        {
            base.List.Remove(operationFaultMessage);
        }

        protected override void SetParent(object value, object parent)
        {
            ((OperationFault) value).SetParent((Operation) parent);
        }

        public OperationFault this[int index]
        {
            get => 
                ((OperationFault) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }

        public OperationFault this[string name] =>
            ((OperationFault) this.Table[name]);
    }
}

