namespace MS.Internal
{
    using System;
    using System.Globalization;

    internal class AvTraceFormat : AvTraceDetails
    {
        private string _message;

        public AvTraceFormat(AvTraceDetails details, object[] args) : base(details.Id, details.Labels)
        {
            this._message = string.Format(CultureInfo.InvariantCulture, details.Labels[0], args);
        }

        public override string Message =>
            this._message;
    }
}

