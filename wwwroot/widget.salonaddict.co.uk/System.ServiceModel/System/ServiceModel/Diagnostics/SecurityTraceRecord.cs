namespace System.ServiceModel.Diagnostics
{
    using System;

    internal class SecurityTraceRecord : TraceRecord
    {
        private string traceName;

        internal SecurityTraceRecord(string traceName)
        {
            if (string.IsNullOrEmpty(traceName))
            {
                this.traceName = "Empty";
            }
            else
            {
                this.traceName = traceName;
            }
        }

        internal override string EventId =>
            ("http://schemas.microsoft.com/2006/08/ServiceModel/" + this.traceName + "TraceRecord");
    }
}

