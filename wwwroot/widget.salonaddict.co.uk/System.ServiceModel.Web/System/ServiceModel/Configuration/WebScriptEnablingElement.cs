namespace System.ServiceModel.Configuration
{
    using System;
    using System.ServiceModel.Description;

    public sealed class WebScriptEnablingElement : BehaviorExtensionElement
    {
        protected internal override object CreateBehavior() => 
            new WebScriptEnablingBehavior();

        public override Type BehaviorType =>
            typeof(WebScriptEnablingBehavior);
    }
}

