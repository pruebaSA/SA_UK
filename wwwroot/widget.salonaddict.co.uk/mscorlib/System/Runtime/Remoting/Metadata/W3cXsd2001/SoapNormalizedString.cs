namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;

    [Serializable, ComVisible(true)]
    public sealed class SoapNormalizedString : ISoapXsd
    {
        private string _value;

        public SoapNormalizedString()
        {
        }

        public SoapNormalizedString(string value)
        {
            this._value = this.Validate(value);
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapNormalizedString Parse(string value) => 
            new SoapNormalizedString(value);

        public override string ToString() => 
            SoapType.Escape(this._value);

        private string Validate(string value)
        {
            if ((value != null) && (value.Length != 0))
            {
                char[] anyOf = new char[] { '\r', '\n', '\t' };
                if (value.LastIndexOfAny(anyOf) > -1)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid"), new object[] { "xsd:normalizedString", value }));
                }
            }
            return value;
        }

        public string Value
        {
            get => 
                this._value;
            set
            {
                this._value = this.Validate(value);
            }
        }

        public static string XsdType =>
            "normalizedString";
    }
}

