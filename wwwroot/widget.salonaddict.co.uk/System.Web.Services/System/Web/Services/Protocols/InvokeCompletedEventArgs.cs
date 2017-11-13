namespace System.Web.Services.Protocols
{
    using System;
    using System.ComponentModel;

    public class InvokeCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object[] results;

        internal InvokeCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
        {
            this.results = results;
        }

        public object[] Results =>
            this.results;
    }
}

