namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    [Serializable]
    public abstract class EdmError
    {
        private string _message;

        internal EdmError(string message)
        {
            EntityUtil.CheckStringArgument(message, "message");
            this._message = message;
        }

        public string Message =>
            this._message;
    }
}

