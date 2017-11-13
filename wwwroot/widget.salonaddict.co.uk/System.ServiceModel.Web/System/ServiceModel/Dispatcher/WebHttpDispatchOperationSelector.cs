namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;

    public class WebHttpDispatchOperationSelector : IDispatchOperationSelector
    {
        private string catchAllOperationName;
        public const string HttpOperationSelectorUriMatchedPropertyName = "UriMatched";
        internal const string redirectOperationName = "";
        internal const string RedirectPropertyName = "WebHttpRedirect";
        private UriTemplateTable table;

        protected WebHttpDispatchOperationSelector()
        {
            this.catchAllOperationName = "";
        }

        public WebHttpDispatchOperationSelector(ServiceEndpoint endpoint)
        {
            this.catchAllOperationName = "";
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
            }
            if (endpoint.Address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.EndpointAddressCannotBeNull, new object[0])));
            }
            this.table = new UriTemplateTable(endpoint.Address.Uri);
            Dictionary<WCFKey, string> dictionary = new Dictionary<WCFKey, string>();
            foreach (OperationDescription description in endpoint.Contract.Operations)
            {
                if (description.Messages[0].Direction == MessageDirection.Input)
                {
                    string webMethod = WebHttpBehavior.GetWebMethod(description);
                    string uTStringOrDefault = UriTemplateClientFormatter.GetUTStringOrDefault(description);
                    if (UriTemplateHelpers.IsWildcardPath(uTStringOrDefault) && (webMethod == "*"))
                    {
                        if (this.catchAllOperationName != "")
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.MultipleOperationsInContractWithPathMethod, new object[] { endpoint.Contract.Name, uTStringOrDefault, webMethod })));
                        }
                        this.catchAllOperationName = description.Name;
                    }
                    UriTemplate uriTemplate = new UriTemplate(uTStringOrDefault);
                    WCFKey key = new WCFKey(uriTemplate, webMethod);
                    if (dictionary.ContainsKey(key))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.MultipleOperationsInContractWithPathMethod, new object[] { endpoint.Contract.Name, uTStringOrDefault, webMethod })));
                    }
                    dictionary.Add(key, description.Name);
                    WCFLookupResult result = new WCFLookupResult(webMethod, description.Name);
                    this.table.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(uriTemplate, result));
                }
            }
            if (this.table.KeyValuePairs.Count == 0)
            {
                this.table = null;
            }
            else
            {
                this.table.MakeReadOnly(true);
            }
        }

        public string SelectOperation(ref Message message)
        {
            bool flag;
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            string str = this.SelectOperation(ref message, out flag);
            message.Properties.Add("UriMatched", flag);
            if ((str != null) && DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.WebRequestMatchesOperation, SR2.GetString(SR2.TraceCodeWebRequestMatchesOperation, new object[] { message.Headers.To, str }));
            }
            return str;
        }

        protected virtual string SelectOperation(ref Message message, out bool uriMatched)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            uriMatched = false;
            if (this.table == null)
            {
                return this.catchAllOperationName;
            }
            if (!message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                return this.catchAllOperationName;
            }
            HttpRequestMessageProperty property = message.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            if (property == null)
            {
                return this.catchAllOperationName;
            }
            string method = property.Method;
            Uri to = message.Headers.To;
            if (to == null)
            {
                return this.catchAllOperationName;
            }
            Collection<UriTemplateMatch> collection = this.table.Match(to);
            if (collection.Count == 0)
            {
                UriBuilder builder = new UriBuilder(to);
                if (!builder.Path.EndsWith("/", StringComparison.Ordinal))
                {
                    builder.Path = builder.Path + "/";
                    Uri uri = builder.Uri;
                    collection = this.table.Match(uri);
                    if (collection.Count != 0)
                    {
                        if (message.Properties.ContainsKey("OriginalHttpRequestUri"))
                        {
                            builder.Host = ((Uri) message.Properties["OriginalHttpRequestUri"]).Host;
                            uri = builder.Uri;
                        }
                        message.Properties.Add("WebHttpRedirect", uri);
                        return "";
                    }
                }
            }
            uriMatched = collection.Count != 0;
            string catchAllOperationName = this.catchAllOperationName;
            UriTemplateMatch match = null;
            for (int i = 0; i < collection.Count; i++)
            {
                UriTemplateMatch match2 = collection[i];
                WCFLookupResult data = match2.Data as WCFLookupResult;
                if (data.Method == method)
                {
                    message.Properties.Add("UriTemplateMatchResults", match2);
                    return data.OperationName;
                }
                if (data.Method == "*")
                {
                    match = match2;
                    catchAllOperationName = data.OperationName;
                }
            }
            if (match != null)
            {
                message.Properties.Add("UriTemplateMatchResults", match);
            }
            return catchAllOperationName;
        }

        private class WCFKey
        {
            private string method;
            private UriTemplate uriTemplate;

            public WCFKey(UriTemplate uriTemplate, string method)
            {
                this.uriTemplate = uriTemplate;
                this.method = method;
            }

            public override bool Equals(object obj)
            {
                WebHttpDispatchOperationSelector.WCFKey key = obj as WebHttpDispatchOperationSelector.WCFKey;
                if (key == null)
                {
                    return false;
                }
                return (this.uriTemplate.IsEquivalentTo(key.uriTemplate) && (this.method == key.method));
            }

            public override int GetHashCode() => 
                UriTemplateEquivalenceComparer.Instance.GetHashCode(this.uriTemplate);
        }

        private class WCFLookupResult
        {
            private string method;
            private string operationName;

            public WCFLookupResult(string method, string operationName)
            {
                this.method = method;
                this.operationName = operationName;
            }

            public string Method =>
                this.method;

            public string OperationName =>
                this.operationName;
        }
    }
}

