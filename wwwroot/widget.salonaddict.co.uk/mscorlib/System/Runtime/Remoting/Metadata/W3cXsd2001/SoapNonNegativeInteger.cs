namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;

    [Serializable, ComVisible(true)]
    public sealed class SoapNonNegativeInteger : ISoapXsd
    {
        private decimal _value;

        public SoapNonNegativeInteger()
        {
        }

        public SoapNonNegativeInteger(decimal value)
        {
            this._value = decimal.Truncate(value);
            if (this._value < 0M)
            {
                throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid"), new object[] { "xsd:nonNegativeInteger", value }));
            }
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapNonNegativeInteger Parse(string value) => 
            new SoapNonNegativeInteger(decimal.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));

        public override string ToString() => 
            this._value.ToString(CultureInfo.InvariantCulture);

        public decimal Value
        {
            get => 
                this._value;
            set
            {
                this._value = decimal.Truncate(value);
                if (this._value < 0M)
                {
                    throw new RemotingException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Remoting_SOAPInteropxsdInvalid"), new object[] { "xsd:nonNegativeInteger", value }));
                }
            }
        }

        public static string XsdType =>
            "nonNegativeInteger";
    }
}

