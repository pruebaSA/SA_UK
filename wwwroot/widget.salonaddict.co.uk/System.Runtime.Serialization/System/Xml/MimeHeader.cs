namespace System.Xml
{
    using System;
    using System.Runtime.Serialization;

    internal class MimeHeader
    {
        private string name;
        private string value;

        public MimeHeader(string name, string value)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
            }
            this.name = name;
            this.value = value;
        }

        public string Name =>
            this.name;

        public string Value =>
            this.value;
    }
}

