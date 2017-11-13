namespace System.ServiceModel.Web
{
    using System;
    using System.ServiceModel;

    public class WebOperationContext : IExtension<OperationContext>
    {
        private OperationContext operationContext;

        public WebOperationContext(OperationContext operationContext)
        {
            if (operationContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operationContext");
            }
            this.operationContext = operationContext;
            if (operationContext.Extensions.Find<WebOperationContext>() == null)
            {
                operationContext.Extensions.Add(this);
            }
        }

        public void Attach(OperationContext owner)
        {
        }

        public void Detach(OperationContext owner)
        {
        }

        public static WebOperationContext Current
        {
            get
            {
                if (OperationContext.Current == null)
                {
                    return null;
                }
                WebOperationContext context = OperationContext.Current.Extensions.Find<WebOperationContext>();
                if (context != null)
                {
                    return context;
                }
                return new WebOperationContext(OperationContext.Current);
            }
        }

        public IncomingWebRequestContext IncomingRequest =>
            new IncomingWebRequestContext(this.operationContext);

        public IncomingWebResponseContext IncomingResponse =>
            new IncomingWebResponseContext(this.operationContext);

        public OutgoingWebRequestContext OutgoingRequest =>
            new OutgoingWebRequestContext(this.operationContext);

        public OutgoingWebResponseContext OutgoingResponse =>
            new OutgoingWebResponseContext(this.operationContext);
    }
}

