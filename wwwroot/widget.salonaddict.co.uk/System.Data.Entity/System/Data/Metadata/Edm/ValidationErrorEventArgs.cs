namespace System.Data.Metadata.Edm
{
    using System;

    internal class ValidationErrorEventArgs : EventArgs
    {
        private EdmItemError _validationError;

        public ValidationErrorEventArgs(EdmItemError validationError)
        {
            this._validationError = validationError;
        }

        public EdmItemError ValidationError =>
            this._validationError;
    }
}

