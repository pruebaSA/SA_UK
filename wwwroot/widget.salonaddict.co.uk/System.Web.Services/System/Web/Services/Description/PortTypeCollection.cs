﻿namespace System.Web.Services.Description
{
    using System;
    using System.Reflection;

    public sealed class PortTypeCollection : ServiceDescriptionBaseCollection
    {
        internal PortTypeCollection(ServiceDescription serviceDescription) : base(serviceDescription)
        {
        }

        public int Add(PortType portType) => 
            base.List.Add(portType);

        public bool Contains(PortType portType) => 
            base.List.Contains(portType);

        public void CopyTo(PortType[] array, int index)
        {
            base.List.CopyTo(array, index);
        }

        protected override string GetKey(object value) => 
            ((PortType) value).Name;

        public int IndexOf(PortType portType) => 
            base.List.IndexOf(portType);

        public void Insert(int index, PortType portType)
        {
            base.List.Insert(index, portType);
        }

        public void Remove(PortType portType)
        {
            base.List.Remove(portType);
        }

        protected override void SetParent(object value, object parent)
        {
            ((PortType) value).SetParent((ServiceDescription) parent);
        }

        public PortType this[int index]
        {
            get => 
                ((PortType) base.List[index]);
            set
            {
                base.List[index] = value;
            }
        }

        public PortType this[string name] =>
            ((PortType) this.Table[name]);
    }
}

