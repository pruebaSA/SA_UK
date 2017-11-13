namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class SoapNmtoken : ISoapXsd
    {
        private string _value;

        public SoapNmtoken()
        {
        }

        public SoapNmtoken(string value)
        {
            this._value = value;
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapNmtoken Parse(string value) => 
            new SoapNmtoken(value);

        public override string ToString() => 
            SoapType.Escape(this._value);

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
            "NMTOKEN";
    }
}

