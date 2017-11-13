namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class WizardNavigationEventArgs : EventArgs
    {
        private bool _cancel;
        private int _currentStepIndex;
        private int _nextStepIndex;

        public WizardNavigationEventArgs(int currentStepIndex, int nextStepIndex)
        {
            this._currentStepIndex = currentStepIndex;
            this._nextStepIndex = nextStepIndex;
        }

        internal void SetNextStepIndex(int nextStepIndex)
        {
            this._nextStepIndex = nextStepIndex;
        }

        public bool Cancel
        {
            get => 
                this._cancel;
            set
            {
                this._cancel = value;
            }
        }

        public int CurrentStepIndex =>
            this._currentStepIndex;

        public int NextStepIndex =>
            this._nextStepIndex;
    }
}

