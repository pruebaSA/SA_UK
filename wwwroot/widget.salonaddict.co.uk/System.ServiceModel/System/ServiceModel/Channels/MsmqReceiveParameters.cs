namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal abstract class MsmqReceiveParameters
    {
        private MsmqUri.IAddressTranslator addressTranslator;
        private bool durable;
        private bool exactlyOnce;
        private int maxRetryCycles;
        private System.ServiceModel.ReceiveErrorHandling receiveErrorHandling;
        private int receiveRetryCount;
        private TimeSpan retryCycleDelay;
        private MsmqTransportSecurity transportSecurity;
        private bool useMsmqTracing;
        private bool useSourceJournal;

        internal MsmqReceiveParameters(MsmqBindingElementBase bindingElement) : this(bindingElement, bindingElement.AddressTranslator)
        {
        }

        internal MsmqReceiveParameters(MsmqBindingElementBase bindingElement, MsmqUri.IAddressTranslator addressTranslator)
        {
            this.addressTranslator = addressTranslator;
            this.durable = bindingElement.Durable;
            this.exactlyOnce = bindingElement.ExactlyOnce;
            this.maxRetryCycles = bindingElement.MaxRetryCycles;
            this.receiveErrorHandling = bindingElement.ReceiveErrorHandling;
            this.receiveRetryCount = bindingElement.ReceiveRetryCount;
            this.retryCycleDelay = bindingElement.RetryCycleDelay;
            this.transportSecurity = new MsmqTransportSecurity(bindingElement.MsmqTransportSecurity);
            this.useMsmqTracing = bindingElement.UseMsmqTracing;
            this.useSourceJournal = bindingElement.UseSourceJournal;
        }

        internal MsmqUri.IAddressTranslator AddressTranslator =>
            this.addressTranslator;

        internal bool Durable =>
            this.durable;

        internal bool ExactlyOnce =>
            this.exactlyOnce;

        internal int MaxRetryCycles =>
            this.maxRetryCycles;

        internal System.ServiceModel.ReceiveErrorHandling ReceiveErrorHandling =>
            this.receiveErrorHandling;

        internal int ReceiveRetryCount =>
            this.receiveRetryCount;

        internal TimeSpan RetryCycleDelay =>
            this.retryCycleDelay;

        internal MsmqTransportSecurity TransportSecurity =>
            this.transportSecurity;

        internal bool UseMsmqTracing =>
            this.useMsmqTracing;

        internal bool UseSourceJournal =>
            this.useSourceJournal;
    }
}

