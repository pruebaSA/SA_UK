namespace System.ServiceModel.Web
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Administration;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class WebGetAttribute : Attribute, IOperationBehavior, IWmiInstanceProvider
    {
        private WebMessageBodyStyle bodyStyle;
        private bool isBodyStyleDefined;
        private bool isRequestMessageFormatSet;
        private bool isResponseMessageFormatSet;
        private WebMessageFormat requestMessageFormat;
        private WebMessageFormat responseMessageFormat;
        private string uriTemplate;

        internal WebMessageBodyStyle GetBodyStyleOrDefault(WebMessageBodyStyle defaultStyle)
        {
            if (this.IsBodyStyleSetExplicitly)
            {
                return this.BodyStyle;
            }
            return defaultStyle;
        }

        void IWmiInstanceProvider.FillInstance(IWmiInstance wmiInstance)
        {
            if (wmiInstance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("wmiInstance");
            }
            wmiInstance.SetProperty("BodyStyle", this.BodyStyle.ToString());
            wmiInstance.SetProperty("IsBodyStyleSetExplicitly", this.IsBodyStyleSetExplicitly.ToString());
            wmiInstance.SetProperty("RequestFormat", this.RequestFormat.ToString());
            wmiInstance.SetProperty("IsRequestFormatSetExplicitly", this.IsRequestFormatSetExplicitly.ToString());
            wmiInstance.SetProperty("ResponseFormat", this.ResponseFormat.ToString());
            wmiInstance.SetProperty("IsResponseFormatSetExplicitly", this.IsResponseFormatSetExplicitly.ToString());
            wmiInstance.SetProperty("UriTemplate", this.UriTemplate);
        }

        string IWmiInstanceProvider.GetInstanceType() => 
            "WebGetAttribute";

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }

        public WebMessageBodyStyle BodyStyle
        {
            get => 
                this.bodyStyle;
            set
            {
                if (!WebMessageBodyStyleHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.bodyStyle = value;
                this.isBodyStyleDefined = true;
            }
        }

        public bool IsBodyStyleSetExplicitly =>
            this.isBodyStyleDefined;

        public bool IsRequestFormatSetExplicitly =>
            this.isRequestMessageFormatSet;

        public bool IsResponseFormatSetExplicitly =>
            this.isResponseMessageFormatSet;

        public WebMessageFormat RequestFormat
        {
            get => 
                this.requestMessageFormat;
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.requestMessageFormat = value;
                this.isRequestMessageFormatSet = true;
            }
        }

        public WebMessageFormat ResponseFormat
        {
            get => 
                this.responseMessageFormat;
            set
            {
                if (!WebMessageFormatHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.responseMessageFormat = value;
                this.isResponseMessageFormatSet = true;
            }
        }

        public string UriTemplate
        {
            get => 
                this.uriTemplate;
            set
            {
                this.uriTemplate = value;
            }
        }
    }
}

