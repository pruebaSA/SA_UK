namespace System.Configuration
{
    using System;

    internal class StreamUpdate
    {
        private string _newStreamname;
        private bool _writeCompleted;

        internal StreamUpdate(string newStreamname)
        {
            this._newStreamname = newStreamname;
        }

        internal string NewStreamname =>
            this._newStreamname;

        internal bool WriteCompleted
        {
            get => 
                this._writeCompleted;
            set
            {
                this._writeCompleted = value;
            }
        }
    }
}

