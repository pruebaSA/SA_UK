namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;

    [Serializable, ComVisible(true)]
    public sealed class SoapToken : ISoapXsd
    {
        private string _value;

        public SoapToken()
        {
        }

        public SoapToken(string value)
        {
            this._value = this.Validate(value);
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapToken Parse(string value) => 
            new SoapToken(value);

        public override string ToString() => 
            SoapType.Escape(this._value);

        private string Validate(string value)
        {
            if ((value != null) && (value.Length != 0))
            {
                char[] anyOf = new char[] { '\r', '\t' };
                if (value.LastIndexOfAny(anyOf) > -1)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid"), new object[] { "xsd:token", value }));
                }
                if ((value.Length > 0) && (char.IsWhiteSpace(value[0]) || char.IsWhiteSpace(value[value.Length - 1])))
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid"), new object[] { "xsd:token", value }));
                }
                if (value.IndexOf("  ") > -1)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid"), new object[] { "xsd:token", value }));
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
            "token";
    }
}

