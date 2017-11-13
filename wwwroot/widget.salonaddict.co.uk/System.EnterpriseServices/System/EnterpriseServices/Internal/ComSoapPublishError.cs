namespace System.EnterpriseServices.Internal
{
    using System;
    using System.Diagnostics;

    public class ComSoapPublishError
    {
        public static void Report(string s)
        {
            try
            {
                new EventLog { Source = "COM+ SOAP Services" }.WriteEntry(s, EventLogEntryType.Warning);
            }
            catch
            {
            }
        }
    }
}

