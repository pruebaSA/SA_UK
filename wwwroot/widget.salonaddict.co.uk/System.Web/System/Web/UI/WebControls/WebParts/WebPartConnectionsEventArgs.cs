namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WebPartConnectionsEventArgs : EventArgs
    {
        private WebPartConnection _connection;
        private WebPart _consumer;
        private System.Web.UI.WebControls.WebParts.ConsumerConnectionPoint _consumerConnectionPoint;
        private WebPart _provider;
        private System.Web.UI.WebControls.WebParts.ProviderConnectionPoint _providerConnectionPoint;

        public WebPartConnectionsEventArgs(WebPart provider, System.Web.UI.WebControls.WebParts.ProviderConnectionPoint providerConnectionPoint, WebPart consumer, System.Web.UI.WebControls.WebParts.ConsumerConnectionPoint consumerConnectionPoint)
        {
            this._provider = provider;
            this._providerConnectionPoint = providerConnectionPoint;
            this._consumer = consumer;
            this._consumerConnectionPoint = consumerConnectionPoint;
        }

        public WebPartConnectionsEventArgs(WebPart provider, System.Web.UI.WebControls.WebParts.ProviderConnectionPoint providerConnectionPoint, WebPart consumer, System.Web.UI.WebControls.WebParts.ConsumerConnectionPoint consumerConnectionPoint, WebPartConnection connection) : this(provider, providerConnectionPoint, consumer, consumerConnectionPoint)
        {
            this._connection = connection;
        }

        public WebPartConnection Connection =>
            this._connection;

        public WebPart Consumer =>
            this._consumer;

        public System.Web.UI.WebControls.WebParts.ConsumerConnectionPoint ConsumerConnectionPoint =>
            this._consumerConnectionPoint;

        public WebPart Provider =>
            this._provider;

        public System.Web.UI.WebControls.WebParts.ProviderConnectionPoint ProviderConnectionPoint =>
            this._providerConnectionPoint;
    }
}

