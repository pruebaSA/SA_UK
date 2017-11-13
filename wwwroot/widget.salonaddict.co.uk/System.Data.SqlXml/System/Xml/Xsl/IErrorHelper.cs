namespace System.Xml.Xsl
{
    using System;

    internal interface IErrorHelper
    {
        void ReportError(string res, params string[] args);
        void ReportWarning(string res, params string[] args);
    }
}

