namespace System.ComponentModel
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class RunWorkerCompletedEventArgs : AsyncCompletedEventArgs
    {
        private object result;

        public RunWorkerCompletedEventArgs(object result, Exception error, bool cancelled) : base(error, cancelled, null)
        {
            this.result = result;
        }

        public object Result
        {
            get
            {
                base.RaiseExceptionIfNecessary();
                return this.result;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public object UserState =>
            base.UserState;
    }
}

