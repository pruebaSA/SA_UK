namespace System.DirectoryServices.ActiveDirectory
{
    using System;

    public class ReplicationOperationInformation
    {
        internal ReplicationOperationCollection collection;
        internal ReplicationOperation currentOp;
        internal DateTime startTime;

        public ReplicationOperation CurrentOperation =>
            this.currentOp;

        public DateTime OperationStartTime =>
            this.startTime;

        public ReplicationOperationCollection PendingOperations =>
            this.collection;
    }
}

