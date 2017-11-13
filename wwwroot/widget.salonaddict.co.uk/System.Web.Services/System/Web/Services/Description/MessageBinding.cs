namespace System.Web.Services.Description
{
    using System;

    public abstract class MessageBinding : NamedItem
    {
        private System.Web.Services.Description.OperationBinding parent;

        protected MessageBinding()
        {
        }

        internal void SetParent(System.Web.Services.Description.OperationBinding parent)
        {
            this.parent = parent;
        }

        public System.Web.Services.Description.OperationBinding OperationBinding =>
            this.parent;
    }
}

