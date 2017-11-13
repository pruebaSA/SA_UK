namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class SoapNotation : ISoapXsd
    {
        private string _value;

        public SoapNotation()
        {
        }

        public SoapNotation(string value)
        {
            this._value = value;
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapNotation Parse(string value) => 
            new SoapNotation(value);

        public override string ToString() => 
            this._value;

        public string Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
            }
        }

        public static string XsdType =>
            "NOTATION";
    }
}

