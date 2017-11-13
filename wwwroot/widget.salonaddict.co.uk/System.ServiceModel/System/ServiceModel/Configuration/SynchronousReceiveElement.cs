namespace System.ServiceModel.Configuration
{
    using System;
    using System.ServiceModel.Description;

    public sealed class SynchronousReceiveElement : BehaviorExtensionElement
    {
        protected internal override object CreateBehavior() => 
            new SynchronousReceiveBehavior();

        public override Type BehaviorType =>
            typeof(SynchronousReceiveBehavior);
    }
}

