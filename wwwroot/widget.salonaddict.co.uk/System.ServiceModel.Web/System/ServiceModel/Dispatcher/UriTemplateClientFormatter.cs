namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;
    using System.Text;

    internal class UriTemplateClientFormatter : IClientMessageFormatter
    {
        private Uri baseUri;
        private IClientMessageFormatter inner;
        private bool innerIsUntypedMessage;
        private bool isGet;
        private string method;
        internal Dictionary<int, string> pathMapping;
        private QueryStringConverter qsc;
        internal Dictionary<int, KeyValuePair<string, Type>> queryMapping;
        private int totalNumUTVars;
        private UriTemplate uriTemplate;

        public UriTemplateClientFormatter(OperationDescription operationDescription, IClientMessageFormatter inner, QueryStringConverter qsc, Uri baseUri, bool innerIsUntypedMessage, string contractName)
        {
            this.inner = inner;
            this.qsc = qsc;
            this.baseUri = baseUri;
            this.innerIsUntypedMessage = innerIsUntypedMessage;
            Populate(out this.pathMapping, out this.queryMapping, out this.totalNumUTVars, out this.uriTemplate, operationDescription, qsc, contractName);
            this.method = WebHttpBehavior.GetWebMethod(operationDescription);
            this.isGet = this.method == "GET";
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.QueryStringFormatterOperationNotSupportedClientSide, new object[0])));
        }

        internal static string GetUTStringOrDefault(OperationDescription operationDescription)
        {
            string webUriTemplate = WebHttpBehavior.GetWebUriTemplate(operationDescription);
            if ((webUriTemplate == null) && (WebHttpBehavior.GetWebMethod(operationDescription) == "GET"))
            {
                webUriTemplate = MakeDefaultGetUTString(operationDescription);
            }
            if (webUriTemplate == null)
            {
                webUriTemplate = operationDescription.Name;
            }
            return webUriTemplate;
        }

        private static string MakeDefaultGetUTString(OperationDescription od)
        {
            StringBuilder builder = new StringBuilder(od.XmlName.DecodedName);
            if (!WebHttpBehavior.IsUntypedMessage(od.Messages[0]))
            {
                builder.Append("?");
                foreach (MessagePartDescription description in od.Messages[0].Body.Parts)
                {
                    string decodedName = description.XmlName.DecodedName;
                    builder.Append(decodedName);
                    builder.Append("={");
                    builder.Append(decodedName);
                    builder.Append("}&");
                }
                builder.Remove(builder.Length - 1, 1);
            }
            return builder.ToString();
        }

        internal static void Populate(out Dictionary<int, string> pathMapping, out Dictionary<int, KeyValuePair<string, Type>> queryMapping, out int totalNumUTVars, out UriTemplate uriTemplate, OperationDescription operationDescription, QueryStringConverter qsc, string contractName)
        {
            pathMapping = new Dictionary<int, string>();
            queryMapping = new Dictionary<int, KeyValuePair<string, Type>>();
            string uTStringOrDefault = GetUTStringOrDefault(operationDescription);
            uriTemplate = new UriTemplate(uTStringOrDefault);
            List<string> collection = new List<string>(uriTemplate.PathSegmentVariableNames);
            List<string> list2 = new List<string>(uriTemplate.QueryValueVariableNames);
            Dictionary<string, byte> dictionary = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);
            totalNumUTVars = collection.Count + list2.Count;
            for (int i = 0; i < operationDescription.Messages[0].Body.Parts.Count; i++)
            {
                MessagePartDescription description = operationDescription.Messages[0].Body.Parts[i];
                string decodedName = description.XmlName.DecodedName;
                if (dictionary.ContainsKey(decodedName))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UriTemplateVarCaseDistinction, new object[] { operationDescription.XmlName.DecodedName, contractName, decodedName })));
                }
                List<string> list3 = new List<string>(collection);
                foreach (string str3 in list3)
                {
                    if (string.Compare(decodedName, str3, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (description.Type != typeof(string))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UriTemplatePathVarMustBeString, new object[] { operationDescription.XmlName.DecodedName, contractName, decodedName })));
                        }
                        pathMapping.Add(i, decodedName);
                        dictionary.Add(decodedName, 0);
                        collection.Remove(str3);
                    }
                }
                List<string> list4 = new List<string>(list2);
                foreach (string str4 in list4)
                {
                    if (string.Compare(decodedName, str4, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (!qsc.CanConvert(description.Type))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UriTemplateQueryVarMustBeConvertible, new object[] { operationDescription.XmlName.DecodedName, contractName, decodedName, description.Type, qsc.GetType().Name })));
                        }
                        queryMapping.Add(i, new KeyValuePair<string, Type>(decodedName, description.Type));
                        dictionary.Add(decodedName, 0);
                        list2.Remove(str4);
                    }
                }
            }
            if (collection.Count != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UriTemplateMissingVar, new object[] { operationDescription.XmlName.DecodedName, contractName, collection[0] })));
            }
            if (list2.Count != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UriTemplateMissingVar, new object[] { operationDescription.XmlName.DecodedName, contractName, list2[0] })));
            }
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            HttpRequestMessageProperty property;
            object[] objArray = new object[parameters.Length - this.totalNumUTVars];
            NameValueCollection values = new NameValueCollection();
            int index = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (this.pathMapping.ContainsKey(i))
                {
                    values[this.pathMapping[i]] = parameters[i] as string;
                }
                else if (this.queryMapping.ContainsKey(i))
                {
                    if (parameters[i] != null)
                    {
                        KeyValuePair<string, Type> pair = this.queryMapping[i];
                        KeyValuePair<string, Type> pair2 = this.queryMapping[i];
                        values[pair.Key] = this.qsc.ConvertValueToString(parameters[i], pair2.Value);
                    }
                }
                else
                {
                    objArray[index] = parameters[i];
                    index++;
                }
            }
            Message message = this.inner.SerializeRequest(messageVersion, objArray);
            bool flag = this.innerIsUntypedMessage && (message.Headers.To != null);
            bool flag2 = (OperationContext.Current != null) && (OperationContext.Current.OutgoingMessageHeaders.To != null);
            if (!flag && !flag2)
            {
                message.Headers.To = this.uriTemplate.BindByName(this.baseUri, values);
            }
            if (WebOperationContext.Current != null)
            {
                if (this.isGet)
                {
                    WebOperationContext.Current.OutgoingRequest.SuppressEntityBody = true;
                }
                if ((this.method != "*") && (WebOperationContext.Current.OutgoingRequest.Method != null))
                {
                    WebOperationContext.Current.OutgoingRequest.Method = this.method;
                }
                return message;
            }
            if (message.Properties.ContainsKey(HttpRequestMessageProperty.Name))
            {
                property = message.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            }
            else
            {
                property = new HttpRequestMessageProperty();
                message.Properties.Add(HttpRequestMessageProperty.Name, property);
            }
            if (this.isGet)
            {
                property.SuppressEntityBody = true;
            }
            if (this.method != "*")
            {
                property.Method = this.method;
            }
            return message;
        }
    }
}

