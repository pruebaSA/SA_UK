﻿namespace System.Web.Services.Description
{
    using System;
    using System.Reflection;

    public sealed class FaultBindingCollection : ServiceDescriptionBaseCollection
    {
        internal FaultBindingCollection(OperationBinding operationBinding) : base(operationBinding)
        {
        }

        public int Add(FaultBinding bindingOperationFault) => 
            base.List.Add(bindingOperationFault);

        public bool Contains(FaultBinding bindingOperationFault) => 
            base.List.Contains(bindingOperationFault);

        public void CopyTo(FaultBinding[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        protected override string GetKey(object value) => 
            ((FaultBinding) value).Name;

        public int IndexOf(FaultBinding bindingOperationFault) => 
            base.List.IndexOf(bindingOperationFault);

        public void Insert(int index, FaultBinding bindingOperationFault)
        {
            base.List.Insert(index, bindingOperationFault);
        }

        public void Remove(FaultBinding bindingOperationFault)
        {
            base.List.Remove(bindingOperationFault);
        }

        protected override void SetParent(object value, object parent)
        {
            ((FaultBinding) value).SetParent((OperationBinding) parent);
        }

        public FaultBinding this[int index]
        {
            get => 
                ((FaultBinding) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }

        public FaultBinding this[string name] =>
            ((FaultBinding) this.Table[name]);
    }
}

