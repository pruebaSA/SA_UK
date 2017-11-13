namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;

    internal class DemultiplexingDispatchMessageFormatter : IDispatchMessageFormatter
    {
        private IDispatchMessageFormatter defaultFormatter;
        private Dictionary<WebContentFormat, IDispatchMessageFormatter> formatters;
        private string supportedFormats;

        public DemultiplexingDispatchMessageFormatter(IDictionary<WebContentFormat, IDispatchMessageFormatter> formatters, IDispatchMessageFormatter defaultFormatter)
        {
            if (formatters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("formatters");
            }
            this.formatters = new Dictionary<WebContentFormat, IDispatchMessageFormatter>();
            foreach (WebContentFormat format in formatters.Keys)
            {
                this.formatters.Add(format, formatters[format]);
            }
            this.defaultFormatter = defaultFormatter;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            if (message != null)
            {
                WebContentFormat format;
                IDispatchMessageFormatter defaultFormatter;
                if (TryGetEncodingFormat(message, out format))
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
                defaultFormatter.DeserializeRequest(message, parameters);
            }
        }

        private string GetSupportedFormats()
        {
            if (this.supportedFormats == null)
            {
                this.supportedFormats = GetSupportedFormats(this.formatters.Keys);
            }
            return this.supportedFormats;
        }

        internal static string GetSupportedFormats(IEnumerable<WebContentFormat> formats)
        {
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (WebContentFormat format in formats)
            {
                if (num > 0)
                {
                    builder.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                    builder.Append(" ");
                }
                builder.Append("'" + format.ToString() + "'");
                num++;
            }
            return builder.ToString();
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR2.GetString(SR2.SerializingReplyNotSupportedByFormatter, new object[] { this })));
        }

        internal static bool TryGetEncodingFormat(Message message, out WebContentFormat format)
        {
            object obj2;
            message.Properties.TryGetValue("WebBodyFormatMessageProperty", out obj2);
            WebBodyFormatMessageProperty property = obj2 as WebBodyFormatMessageProperty;
            if (property == null)
            {
                format = WebContentFormat.Default;
                return false;
            }
            format = property.Format;
            return true;
        }
    }
}

