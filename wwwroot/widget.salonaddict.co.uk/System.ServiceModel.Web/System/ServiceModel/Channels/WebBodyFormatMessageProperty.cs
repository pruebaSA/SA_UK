namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.ServiceModel;

    public sealed class WebBodyFormatMessageProperty : IMessageProperty
    {
        private WebContentFormat format;
        private static WebBodyFormatMessageProperty jsonProperty;
        public const string Name = "WebBodyFormatMessageProperty";
        private static WebBodyFormatMessageProperty rawProperty;
        private static WebBodyFormatMessageProperty xmlProperty;

        public WebBodyFormatMessageProperty(WebContentFormat format)
        {
            if (format == WebContentFormat.Default)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR2.GetString(SR2.DefaultContentFormatNotAllowedInProperty, new object[0])));
            }
            this.format = format;
        }

        public IMessageProperty CreateCopy() => 
            this;

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, SR2.GetString(SR2.WebBodyFormatPropertyToString, new object[] { this.Format.ToString() }), new object[0]);

        public WebContentFormat Format =>
            this.format;

        internal static WebBodyFormatMessageProperty JsonProperty
        {
            get
            {
                if (jsonProperty == null)
                {
                    jsonProperty = new WebBodyFormatMessageProperty(WebContentFormat.Json);
                }
                return jsonProperty;
            }
        }

        internal static WebBodyFormatMessageProperty RawProperty
        {
            get
            {
                if (rawProperty == null)
                {
                    rawProperty = new WebBodyFormatMessageProperty(WebContentFormat.Raw);
                }
                return rawProperty;
            }
        }

        internal static WebBodyFormatMessageProperty XmlProperty
        {
            get
            {
                if (xmlProperty == null)
                {
                    xmlProperty = new WebBodyFormatMessageProperty(WebContentFormat.Xml);
                }
                return xmlProperty;
            }
        }
    }
}

