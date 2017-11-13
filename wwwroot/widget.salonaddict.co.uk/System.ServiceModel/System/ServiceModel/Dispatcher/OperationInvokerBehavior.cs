namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    internal class OperationInvokerBehavior : IOperationBehavior
    {
        void IOperationBehavior.AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            if (dispatch == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dispatch");
            }
            if (description.SyncMethod != null)
            {
                dispatch.Invoker = new SyncMethodInvoker(description.SyncMethod);
            }
            else if (description.BeginMethod != null)
            {
                dispatch.Invoker = new AsyncMethodInvoker(description.BeginMethod, description.EndMethod);
            }
        }

        void IOperationBehavior.Validate(OperationDescription description)
        {
        }
    }
}

