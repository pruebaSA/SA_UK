namespace System.ServiceModel.Description
{
    using System;
    using System.ServiceModel;

    public class MetadataConversionError
    {
        private bool isWarning;
        private string message;

        public MetadataConversionError(string message) : this(message, false)
        {
        }

        public MetadataConversionError(string message, bool isWarning)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            this.message = message;
            this.isWarning = isWarning;
        }

        public override bool Equals(object obj)
        {
            MetadataConversionError error = obj as MetadataConversionError;
            return ((error?.IsWarning == this.IsWarning) && (error.Message == this.Message));
        }

        public override int GetHashCode() => 
            this.message.GetHashCode();

        public bool IsWarning =>
            this.isWarning;

        public string Message =>
            this.message;
    }
}

