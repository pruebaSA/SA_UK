namespace System.Web
{
    using System;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TraceContextRecord
    {
        private string _category;
        private Exception _errorInfo;
        private bool _isWarning;
        private string _message;

        public TraceContextRecord(string category, string msg, bool isWarning, Exception errorInfo)
        {
            this._category = category;
            this._message = msg;
            this._isWarning = isWarning;
            this._errorInfo = errorInfo;
        }

        public string Category =>
            this._category;

        public Exception ErrorInfo =>
            this._errorInfo;

        public bool IsWarning =>
            this._isWarning;

        public string Message =>
            this._message;
    }
}

