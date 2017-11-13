namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ProxyRpc
    {
        internal readonly string Action;
        internal ServiceModelActivity Activity;
        internal readonly ServiceChannel Channel;
        internal object[] Correlation;
        internal readonly object[] InputParameters;
        internal readonly ProxyOperationRuntime Operation;
        internal object[] OutputParameters;
        internal Message Request;
        internal Message Reply;
        internal object ReturnValue;
        internal System.ServiceModel.Channels.MessageVersion MessageVersion;
        internal readonly System.ServiceModel.TimeoutHelper TimeoutHelper;
        internal ProxyRpc(ServiceChannel channel, ProxyOperationRuntime operation, string action, object[] inputs, TimeSpan timeout)
        {
            this.Action = action;
            this.Activity = null;
            this.Channel = channel;
            this.Correlation = EmptyArray.Allocate(operation.Parent.CorrelationCount);
            this.InputParameters = inputs;
            this.Operation = operation;
            this.OutputParameters = null;
            this.Request = null;
            this.Reply = null;
            this.ReturnValue = null;
            this.MessageVersion = channel.MessageVersion;
            this.TimeoutHelper = new System.ServiceModel.TimeoutHelper(timeout);
        }
    }
}

