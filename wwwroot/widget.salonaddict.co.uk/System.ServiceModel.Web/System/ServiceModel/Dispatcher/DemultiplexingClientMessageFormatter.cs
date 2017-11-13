namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class DemultiplexingClientMessageFormatter : IClientMessageFormatter
    {
        private IClientMessageFormatter defaultFormatter;
        private Dictionary<WebContentFormat, IClientMessageFormatter> formatters;
        private string supportedFormats;

        public DemultiplexingClientMessageFormatter(IDictionary<WebContentFormat, IClientMessageFormatter> formatters, IClientMessageFormatter defaultFormatter)
        {
            if (formatters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("formatters");
            }
            this.formatters = new Dictionary<WebContentFormat, IClientMessageFormatter>();
            foreach (WebContentFormat format in formatters.Keys)
            {
                this.formatters.Add(format, formatters[format]);
            }
            this.defaultFormatter = defaultFormatter;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            WebContentFormat format;
            IClientMessageFormatter defaultFormatter;
            if (message == null)
            {
                return null;
            }
            if (DemultiplexingDispatchMessageFormatter.TryGetEncodingFormat(message, out format))
            {
                this.formatters.TryGetValue(format, out defaultFormatter);
                if (defaultFormatter == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.UnrecognizedHttpMessageFormat, new object[] { format, this.GetSupportedFormats() })));
                }
            }
            else
            {
                defaultFormatter = this.defaultFormatter;
                if (defaultFormatter == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.MessageFormatPropertyNotFound3, new object[0])));
                }
            }
            return defaultFormatter.DeserializeReply(message, parameters);
        }

        private string GetSupportedFormats()
        {
            if (this.supportedFormats == null)
            {
                this.supportedFormats = DemultiplexingDispatchMessageFormatter.GetSupportedFormats(this.formatters.Keys);
            }
            return this.supportedFormats;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.SerializingRequestNotSupportedByFormatter, new object[] { this })));
        }
    }
}

