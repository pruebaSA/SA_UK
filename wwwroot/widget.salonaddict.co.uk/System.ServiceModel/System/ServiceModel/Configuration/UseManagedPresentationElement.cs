namespace System.ServiceModel.Configuration
{
    using System;
    using System.ServiceModel.Channels;

    public class UseManagedPresentationElement : BindingElementExtensionElement
    {
        protected internal override BindingElement CreateBindingElement()
        {
            UseManagedPresentationBindingElement bindingElement = new UseManagedPresentationBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        public override Type BindingElementType =>
            typeof(UseManagedPresentationBindingElement);
    }
}

