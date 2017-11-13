namespace System
{
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class ResolveEventArgs : EventArgs
    {
        private string _Name;

        public ResolveEventArgs(string name)
        {
            this._Name = name;
        }

        public string Name =>
            this._Name;
    }
}

