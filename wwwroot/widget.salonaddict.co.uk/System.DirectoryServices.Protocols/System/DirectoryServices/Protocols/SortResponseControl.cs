namespace System.DirectoryServices.Protocols
{
    using System;

    public class SortResponseControl : DirectoryControl
    {
        private string name;
        private ResultCode result;

        internal SortResponseControl(ResultCode result, string attributeName, bool critical, byte[] value) : base("1.2.840.113556.1.4.474", value, critical, true)
        {
            this.result = result;
            this.name = attributeName;
        }

        public string AttributeName =>
            this.name;

        public ResultCode Result =>
            this.result;
    }
}

