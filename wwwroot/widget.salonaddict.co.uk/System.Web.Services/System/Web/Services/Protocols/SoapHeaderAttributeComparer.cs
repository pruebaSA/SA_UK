namespace System.Web.Services.Protocols
{
    using System;
    using System.Collections;

    internal class SoapHeaderAttributeComparer : IComparer
    {
        public int Compare(object x, object y) => 
            string.Compare(((SoapHeaderAttribute) x).MemberName, ((SoapHeaderAttribute) y).MemberName, StringComparison.Ordinal);
    }
}

