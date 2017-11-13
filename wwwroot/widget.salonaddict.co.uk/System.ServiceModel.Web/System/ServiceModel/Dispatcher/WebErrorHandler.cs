namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    internal class WebErrorHandler : IErrorHandler
    {
        private bool includeExceptionDetailInFaults;

        public WebErrorHandler(bool includeExceptionDetailInFaults)
        {
            this.includeExceptionDetailInFaults = includeExceptionDetailInFaults;
        }

        public bool HandleError(Exception error) => 
            false;

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if ((version == MessageVersion.None) && (error != null))
            {
                MemoryStream stream = new MemoryStream();
                this.WriteErrorPage(stream, error);
                stream.Seek(0L, SeekOrigin.Begin);
                fault = new HttpStreamMessage(stream);
                fault.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.RawProperty);
                HttpResponseMessageProperty property = new HttpResponseMessageProperty {
                    StatusCode = HttpStatusCode.BadRequest,
                    Headers = { [HttpResponseHeader.ContentType] = "text/html" }
                };
                fault.Properties.Add(HttpResponseMessageProperty.Name, property);
            }
        }

        public void WriteErrorPage(Stream stream, Exception error)
        {
            string str;
            string stackTrace;
            if (this.includeExceptionDetailInFaults)
            {
                str = SR2.GetString(SR2.ServerErrorProcessingRequestWithDetails, new object[] { error.Message });
                stackTrace = error.StackTrace;
            }
            else
            {
                str = SR2.GetString(SR2.ServerErrorProcessingRequest, new object[0]);
                stackTrace = null;
            }
            using (XmlWriter writer = XmlWriter.Create(stream))
            {
                writer.WriteStartElement("HTML");
                writer.WriteStartElement("HEAD");
                writer.WriteRaw(string.Format(CultureInfo.InvariantCulture, "{0}\r\n<TITLE>{1}</TITLE>", new object[] { SR2.GetString(SR2.HelpPageLayout, new object[0]), SR2.GetString(SR2.WebErrorPageTitleText, new object[0]) }));
                writer.WriteEndElement();
                object[] args = new object[] { SR2.GetString(SR2.WebErrorPageTitleText, new object[0]), str, stackTrace ?? string.Empty };
                writer.WriteRaw(string.Format(CultureInfo.InvariantCulture, "<BODY>\r\n<DIV id=\"content\">\r\n<P class=\"heading1\">{0}</P>\r\n<BR/>\r\n<P class=\"intro\">{1}</P>\r\n<P class=\"intro\">{2}</P>\r\n</DIV>\r\n</BODY>", args));
                writer.WriteEndElement();
            }
        }
    }
}

