namespace System.Runtime.Remoting.Metadata.W3cXsd2001
{
    using System;
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class SoapMonthDay : ISoapXsd
    {
        private DateTime _value;
        private static string[] formats = new string[] { "--MM-dd", "--MM-ddzzz" };

        public SoapMonthDay()
        {
            this._value = DateTime.MinValue;
        }

        public SoapMonthDay(DateTime value)
        {
            this._value = DateTime.MinValue;
            this._value = value;
        }

        public string GetXsdType() => 
            XsdType;

        public static SoapMonthDay Parse(string value) => 
            new SoapMonthDay(DateTime.ParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None));

        public override string ToString() => 
            this._value.ToString("'--'MM'-'dd", CultureInfo.InvariantCulture);

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
            "gMonthDay";
    }
}

