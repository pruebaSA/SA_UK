namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;

    internal class UriTemplateDispatchFormatter : IDispatchMessageFormatter
    {
        private Uri baseAddress;
        private IDispatchMessageFormatter inner;
        private string operationName;
        internal Dictionary<int, string> pathMapping;
        private QueryStringConverter qsc;
        internal Dictionary<int, KeyValuePair<string, Type>> queryMapping;
        private int totalNumUTVars;
        private UriTemplate uriTemplate;

        public UriTemplateDispatchFormatter(OperationDescription operationDescription, IDispatchMessageFormatter inner, QueryStringConverter qsc, string contractName, Uri baseAddress)
        {
            this.inner = inner;
            this.qsc = qsc;
            this.baseAddress = baseAddress;
            this.operationName = operationDescription.Name;
            UriTemplateClientFormatter.Populate(out this.pathMapping, out this.queryMapping, out this.totalNumUTVars, out this.uriTemplate, operationDescription, qsc, contractName);
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            object[] objArray = new object[parameters.Length - this.totalNumUTVars];
            if (objArray.Length != 0)
            {
                this.inner.DeserializeRequest(message, objArray);
            }
            int index = 0;
            UriTemplateMatch match = null;
            string name = "UriTemplateMatchResults";
            if (message.Properties.ContainsKey(name))
            {
                match = message.Properties[name] as UriTemplateMatch;
            }
            else if ((message.Headers.To != null) && message.Headers.To.IsAbsoluteUri)
            {
                match = this.uriTemplate.Match(this.baseAddress, message.Headers.To);
            }
            NameValueCollection values = (match == null) ? new NameValueCollection() : match.BoundVariables;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (this.pathMapping.ContainsKey(i) && (match != null))
                {
                    parameters[i] = values[this.pathMapping[i]];
                }
                else if (this.queryMapping.ContainsKey(i) && (match != null))
                {
                    KeyValuePair<string, Type> pair2 = this.queryMapping[i];
                    string parameter = values[pair2.Key];
                    KeyValuePair<string, Type> pair3 = this.queryMapping[i];
                    parameters[i] = this.qsc.ConvertStringToValue(parameter, pair3.Value);
                }
                else
                {
                    parameters[i] = objArray[index];
                    index++;
                }
            }
            if (DiagnosticUtility.ShouldTraceInformation && (match != null))
            {
                foreach (string str3 in match.QueryParameters.Keys)
                {
                    bool flag = true;
                    foreach (KeyValuePair<string, Type> pair in this.queryMapping.Values)
                    {
                        if (string.Compare(str3, pair.Key, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.WebUnknownQueryParameterIgnored, SR2.GetString(SR2.TraceCodeWebRequestUnknownQueryParameterIgnored, new object[] { str3, this.operationName }));
                    }
                }
            }
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.QueryStringFormatterOperationNotSupportedServerSide, new object[0])));
        }
    }
}

