namespace System.Runtime.Remoting.Messaging
{
    using System;

    [Serializable]
    internal class CallContextRemotingData : ICloneable
    {
        private string _logicalCallID;

        public object Clone() => 
            new CallContextRemotingData { LogicalCallID = this.LogicalCallID };

        internal bool HasInfo =>
            (this._logicalCallID != null);

        internal string LogicalCallID
        {
            get => 
                this._logicalCallID;
            set
            {
                this._logicalCallID = value;
            }
        }
    }
}

