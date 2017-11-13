namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class SoapInteger : ISoapXsd
    {
        private decimal _value;

        public SoapInteger()
        {
        }

        public SoapInteger(decimal value)
        {
            this._value = decimal.Truncate(value);
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapInteger Parse(string value) => 
            new SoapInteger(decimal.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture));

        public override string ToString() => 
            this._value.ToString(CultureInfo.InvariantCulture);

        public decimal Value
        {
            get => 
                this._value;
            set
            {
                this._value = decimal.Truncate(value);
            }
        }

        public static string XsdType =>
            "integer";
    }
}

