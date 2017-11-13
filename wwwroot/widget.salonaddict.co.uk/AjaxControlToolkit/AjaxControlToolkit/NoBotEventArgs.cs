namespace AjaxControlToolkit
{
    using System;

    public class NoBotEventArgs : EventArgs
    {
        private string challengeScript = "";
        private string requiredResponse = "";

        public string ChallengeScript
        {
            get => 
                this.challengeScript;
            set
            {
                this.challengeScript = value;
            }
        }

        public string RequiredResponse
        {
            get => 
                this.requiredResponse;
            set
            {
                this.requiredResponse = value;
            }
        }
    }
}

