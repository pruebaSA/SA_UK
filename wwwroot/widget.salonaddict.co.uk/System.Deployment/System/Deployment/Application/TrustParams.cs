﻿namespace System.Deployment.Application
{
    using System;

    internal class TrustParams
    {
        private bool noPrompt;

        public bool NoPrompt
        {
            get => 
                this.noPrompt;
            set
            {
                this.noPrompt = value;
            }
        }
    }
}

