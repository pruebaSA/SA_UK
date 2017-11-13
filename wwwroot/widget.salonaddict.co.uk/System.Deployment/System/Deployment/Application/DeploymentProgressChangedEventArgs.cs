namespace System.Deployment.Application
{
    using System;
    using System.ComponentModel;

    public class DeploymentProgressChangedEventArgs : ProgressChangedEventArgs
    {
        private readonly long _bytesCompleted;
        private readonly long _bytesTotal;
        private readonly string _groupName;
        private readonly DeploymentProgressState _state;

        internal DeploymentProgressChangedEventArgs(int progressPercentage, object userState, long bytesCompleted, long bytesTotal, DeploymentProgressState state, string groupName) : base(progressPercentage, userState)
        {
            this._bytesCompleted = bytesCompleted;
            this._bytesTotal = bytesTotal;
            this._state = state;
            this._groupName = groupName;
        }

        public long BytesCompleted =>
            this._bytesCompleted;

        public long BytesTotal =>
            this._bytesTotal;

        public string Group =>
            this._groupName;

        public DeploymentProgressState State =>
            this._state;
    }
}

