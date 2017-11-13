namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Web;
    using System.Xml;

    internal class HttpUnhandledOperationInvoker : IOperationInvoker
    {
        private const string HtmlContentType = "text/html; charset=UTF-8";

        public object[] AllocateInputs() => 
            new object[1];

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            Message message = inputs[0] as Message;
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.HttpUnhandledOperationInvokerCalledWithoutMessage, new object[0])));
            }
            Uri newLocation = null;
            Uri to = message.Headers.To;
            if (message.Properties.ContainsKey("WebHttpRedirect"))
            {
                newLocation = message.Properties["WebHttpRedirect"] as Uri;
            }
            if ((newLocation != null) && (to != null))
            {
                Message message2 = new HttpTransferRedirectMessage(to, newLocation);
                HttpResponseMessageProperty property = new HttpResponseMessageProperty {
                    StatusCode = HttpStatusCode.TemporaryRedirect
                };
                property.Headers.Add(HttpResponseHeader.Location, newLocation.AbsoluteUri);
                property.Headers.Add(HttpResponseHeader.ContentType, "text/html; charset=UTF-8");
                message2.Properties.Add(HttpResponseMessageProperty.Name, property);
                outputs = null;
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.WebRequestRedirect, SR2.GetString(SR2.TraceCodeWebRequestRedirect, new object[] { to, newLocation }));
                }
                return message2;
            }
            bool methodNotAllowed = false;
            if (message.Properties.ContainsKey("UriMatched"))
            {
                methodNotAllowed = (bool) message.Properties["UriMatched"];
            }
            Message message3 = new HttpTransferHelpPageMessage(methodNotAllowed);
            HttpResponseMessageProperty property2 = new HttpResponseMessageProperty();
            if (methodNotAllowed)
            {
                property2.StatusCode = HttpStatusCode.MethodNotAllowed;
            }
            else
            {
                property2.StatusCode = HttpStatusCode.NotFound;
            }
            try
            {
                if (property2.StatusCode == HttpStatusCode.NotFound)
                {
                    if (Debugger.IsAttached)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.WebRequestDidNotMatchOperation, new object[] { OperationContext.Current.IncomingMessageHeaders.To })));
                    }
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(new InvalidOperationException(SR2.GetString(SR2.WebRequestDidNotMatchOperation, new object[] { OperationContext.Current.IncomingMessageHeaders.To })), TraceEventType.Warning);
                    }
                }
                else
                {
                    if (Debugger.IsAttached)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.WebRequestDidNotMatchMethod, new object[] { WebOperationContext.Current.IncomingRequest.Method, OperationContext.Current.IncomingMessageHeaders.To })));
                    }
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(new InvalidOperationException(SR2.GetString(SR2.WebRequestDidNotMatchMethod, new object[] { WebOperationContext.Current.IncomingRequest.Method, OperationContext.Current.IncomingMessageHeaders.To })), TraceEventType.Warning);
                    }
                }
            }
            catch (InvalidOperationException)
            {
            }
            property2.Headers.Add(HttpResponseHeader.ContentType, "text/html; charset=UTF-8");
            message3.Properties.Add(HttpResponseMessageProperty.Name, property2);
            outputs = null;
            return message3;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public bool IsSynchronous =>
            true;

        private class HttpTransferHelpPageMessage : ContentOnlyMessage
        {
            private bool methodNotAllowed;

            public HttpTransferHelpPageMessage()
            {
                this.methodNotAllowed = false;
            }

            public HttpTransferHelpPageMessage(bool methodNotAllowed)
            {
                this.methodNotAllowed = methodNotAllowed;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("HTML");
                writer.WriteStartElement("HEAD");
                writer.WriteRaw(string.Format(CultureInfo.InvariantCulture, "{0}\r\n<TITLE>{1}</TITLE>", new object[] { SR2.GetString(SR2.HelpPageLayout, new object[0]), SR2.GetString(SR2.HelpPageTitleText, new object[0]) }));
                writer.WriteEndElement();
                writer.WriteRaw(string.Format(CultureInfo.InvariantCulture, "<BODY>\r\n<DIV id=\"content\">\r\n<P class=\"heading1\">{0}</P>\r\n<BR/>\r\n<P class=\"intro\">{1}</P>\r\n</DIV>\r\n</BODY>", new object[] { SR2.GetString(SR2.HelpPageTitleText, new object[0]), this.methodNotAllowed ? SR2.GetString(SR2.HelpPageMethodNotAllowedText, new object[0]) : SR2.GetString(SR2.HelpPageText, new object[0]) }));
                writer.WriteEndElement();
            }
        }

        private class HttpTransferRedirectMessage : ContentOnlyMessage
        {
            private Uri newLocation;
            private Uri originalTo;

            public HttpTransferRedirectMessage(Uri originalTo, Uri newLocation)
            {
                this.originalTo = originalTo;
                this.newLocation = newLocation;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement("HTML");
                writer.WriteStartElement("HEAD");
                writer.WriteRaw(string.Format(CultureInfo.InvariantCulture, "{0}\r\n<TITLE>{1}</TITLE>", new object[] { SR2.GetString(SR2.HelpPageLayout, new object[0]), SR2.GetString(SR2.HelpPageTitleText, new object[0]) }));
                writer.WriteEndElement();
                writer.WriteRaw(string.Format(CultureInfo.InvariantCulture, "<BODY>\r\n<DIV id=\"content\">\r\n<P class=\"heading1\">{0}</P>\r\n<BR/>\r\n<P class=\"intro\">{1}</P>\r\n</DIV>\r\n</BODY>", new object[] { SR2.GetString(SR2.HelpPageTitleText, new object[0]), SR2.GetString(SR2.RedirectPageText, new object[] { this.originalTo.AbsoluteUri, this.newLocation.AbsoluteUri }) }));
                writer.WriteEndElement();
            }
        }
    }
}

