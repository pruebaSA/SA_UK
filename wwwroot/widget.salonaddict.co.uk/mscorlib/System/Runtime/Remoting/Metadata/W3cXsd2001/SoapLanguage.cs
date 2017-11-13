namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class SoapLanguage : ISoapXsd
    {
        private string _value;

        public SoapLanguage()
        {
        }

        public SoapLanguage(string value)
        {
            this._value = value;
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapLanguage Parse(string value) => 
            new SoapLanguage(value);

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
            "language";
    }
}

