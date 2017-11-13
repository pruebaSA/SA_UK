namespace System.Xml.Schema
{
    using System;

    internal class Datatype_date : Datatype_dateTimeBase
    {
        internal Datatype_date() : base(XsdDateTimeFlags.Date)
        {
        }

        public override XmlTypeCode TypeCode =>
            XmlTypeCode.Date;
    }
}

