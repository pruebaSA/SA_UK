namespace System.ServiceModel
{
    using System;

    public class FaultImportOptions
    {
        private bool useMessageFormat;

        public bool UseMessageFormat
        {
            get => 
                this.useMessageFormat;
            set
            {
                this.useMessageFormat = value;
            }
        }
    }
}

