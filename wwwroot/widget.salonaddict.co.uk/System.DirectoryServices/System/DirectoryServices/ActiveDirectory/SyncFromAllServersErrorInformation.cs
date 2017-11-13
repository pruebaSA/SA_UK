namespace System.DirectoryServices.ActiveDirectory
{
    using System;

    public class SyncFromAllServersErrorInformation
    {
        private SyncFromAllServersErrorCategory category;
        private int errorCode;
        private string errorMessage;
        private string sourceServer;
        private string targetServer;

        internal SyncFromAllServersErrorInformation(SyncFromAllServersErrorCategory category, int errorCode, string errorMessage, string sourceServer, string targetServer)
        {
            this.category = category;
            this.errorCode = errorCode;
            this.errorMessage = errorMessage;
            this.sourceServer = sourceServer;
            this.targetServer = targetServer;
        }

        public SyncFromAllServersErrorCategory ErrorCategory =>
            this.category;

        public int ErrorCode =>
            this.errorCode;

        public string ErrorMessage =>
            this.errorMessage;

        public string SourceServer =>
            this.sourceServer;

        public string TargetServer =>
            this.targetServer;
    }
}

