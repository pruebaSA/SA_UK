namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.Globalization;
    using System.ServiceModel;

    internal class Fault
    {
        private string action;
        private FaultCode code;
        private FaultReason reason;
        private string reasonText;

        public Fault(string action, FaultCode code, string reasonText)
        {
            this.action = action;
            this.code = code;
            this.reasonText = reasonText;
            this.reason = new FaultReason(reasonText, CultureInfo.CurrentCulture);
        }

        public string Action =>
            this.action;

        public FaultCode Code =>
            this.code;

        public FaultReason Reason =>
            this.reason;

        public string ReasonText =>
            this.reasonText;
    }
}

