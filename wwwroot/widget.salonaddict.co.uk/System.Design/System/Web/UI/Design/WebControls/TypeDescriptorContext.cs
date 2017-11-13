namespace System.Web.UI.Design.WebControls
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    internal sealed class TypeDescriptorContext : ITypeDescriptorContext, IServiceProvider
    {
        private IDesignerHost _designerHost;
        private object _instance;
        private System.ComponentModel.PropertyDescriptor _propDesc;

        public TypeDescriptorContext(IDesignerHost designerHost, System.ComponentModel.PropertyDescriptor propDesc, object instance)
        {
            this._designerHost = designerHost;
            this._propDesc = propDesc;
            this._instance = instance;
        }

        public object GetService(Type serviceType) => 
            this._designerHost.GetService(serviceType);

        public void OnComponentChanged()
        {
            if (this.ComponentChangeService != null)
            {
                this.ComponentChangeService.OnComponentChanged(this._instance, this._propDesc, null, null);
            }
        }

        public bool OnComponentChanging()
        {
            if (this.ComponentChangeService != null)
            {
                try
                {
                    this.ComponentChangeService.OnComponentChanging(this._instance, this._propDesc);
                }
                catch (CheckoutException exception)
                {
                    if (exception != CheckoutException.Canceled)
                    {
                        throw exception;
                    }
                    return false;
                }
            }
            return true;
        }

        private IComponentChangeService ComponentChangeService =>
            ((IComponentChangeService) this._designerHost.GetService(typeof(IComponentChangeService)));

        public IContainer Container =>
            ((IContainer) this._designerHost.GetService(typeof(IContainer)));

        public object Instance =>
            this._instance;

        public System.ComponentModel.PropertyDescriptor PropertyDescriptor =>
            this._propDesc;
    }
}

