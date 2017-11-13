namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Collections;
    using System.Xml;

    internal class DictionaryTraceRecord : TraceRecord
    {
        private IDictionary dictionary;

        internal DictionaryTraceRecord(IDictionary dictionary)
        {
            this.dictionary = dictionary;
        }

        internal override void WriteTo(XmlWriter xml)
        {
            if (this.dictionary != null)
            {
                foreach (object obj2 in this.dictionary.Keys)
                {
                    xml.WriteElementString(obj2.ToString(), (this.dictionary[obj2] == null) ? string.Empty : this.dictionary[obj2].ToString());
                }
            }
        }

        internal override string EventId =>
            "http://schemas.microsoft.com/2006/08/ServiceModel/DictionaryTraceRecord";
    }
}

