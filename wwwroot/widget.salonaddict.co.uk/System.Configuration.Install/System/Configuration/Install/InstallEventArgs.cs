namespace System.Configuration.Install
{
    using System;
    using System.Collections;

    public class InstallEventArgs : EventArgs
    {
        private IDictionary savedState;

        public InstallEventArgs()
        {
        }

        public InstallEventArgs(IDictionary savedState)
        {
            this.savedState = savedState;
        }

        public IDictionary SavedState =>
            this.savedState;
    }
}

