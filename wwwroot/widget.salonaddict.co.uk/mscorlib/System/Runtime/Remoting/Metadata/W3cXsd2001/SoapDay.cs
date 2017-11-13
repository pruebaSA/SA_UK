namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class SoapDay : ISoapXsd
    {
        private DateTime _value;
        private static string[] formats = new string[] { "---dd", "---ddzzz" };

        public SoapDay()
        {
            this._value = DateTime.MinValue;
        }

        public SoapDay(DateTime value)
        {
            this._value = DateTime.MinValue;
            this._value = value;
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapDay Parse(string value) => 
            new SoapDay(DateTime.ParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None));

        public override string ToString() => 
            this._value.ToString("---dd", CultureInfo.InvariantCulture);

        public DateTime Value
        {
            get => 
                this._value;
            set
            {
                this._value = value;
            }
        }

        public static string XsdType =>
            "gDay";
    }
}

