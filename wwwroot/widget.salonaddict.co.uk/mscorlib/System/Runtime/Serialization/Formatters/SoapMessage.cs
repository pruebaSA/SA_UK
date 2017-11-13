namespace System.Runtime.Serialization.Formatters
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;

    [Serializable, ComVisible(true)]
    public class SoapMessage : ISoapMessage
    {
        internal Header[] headers;
        internal string methodName;
        internal string[] paramNames;
        internal Type[] paramTypes;
        internal object[] paramValues;
        internal string xmlNameSpace;

        public Header[] Headers
        {
            get => 
                this.headers;
            set
            {
                this.headers = value;
            }
        }

        public string MethodName
        {
            get => 
                this.methodName;
            set
            {
                this.methodName = value;
            }
        }

        public string[] ParamNames
        {
            get => 
                this.paramNames;
            set
            {
                this.paramNames = value;
            }
        }

        public Type[] ParamTypes
        {
            get => 
                this.paramTypes;
            set
            {
                this.paramTypes = value;
            }
        }

        public object[] ParamValues
        {
            get => 
                this.paramValues;
            set
            {
                this.paramValues = value;
            }
        }

        public string XmlNameSpace
        {
            get => 
                this.xmlNameSpace;
            set
            {
                this.xmlNameSpace = value;
            }
        }
    }
}

