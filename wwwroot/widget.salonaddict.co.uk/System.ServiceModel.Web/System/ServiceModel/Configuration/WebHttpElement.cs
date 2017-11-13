namespace System.ServiceModel.Configuration
{
    using System;
    using System.ServiceModel.Description;

    public sealed class WebHttpElement : BehaviorExtensionElement
    {
        protected internal override object CreateBehavior() => 
            new WebHttpBehavior();

        public override Type BehaviorType =>
            typeof(WebHttpBehavior);
    }
}

