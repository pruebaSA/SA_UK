namespace System.Windows.Forms
{
    using System;

    public class TypeValidationEventArgs : EventArgs
    {
        private bool cancel;
        private bool isValidInput;
        private string message;
        private object returnValue;
        private System.Type validatingType;

        public TypeValidationEventArgs(System.Type validatingType, bool isValidInput, object returnValue, string message)
        {
            this.validatingType = validatingType;
            this.isValidInput = isValidInput;
            this.returnValue = returnValue;
            this.message = message;
        }

        public bool Cancel
        {
            get => 
                this.cancel;
            set
            {
                this.cancel = value;
            }
        }

        public bool IsValidInput =>
            this.isValidInput;

        public string Message =>
            this.message;

        public object ReturnValue =>
            this.returnValue;

        public System.Type ValidatingType =>
            this.validatingType;
    }
}

