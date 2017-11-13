namespace System.Configuration.Install
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class InstallException : SystemException
    {
        public InstallException()
        {
            base.HResult = -2146232057;
        }

        public InstallException(string message) : base(message)
        {
        }

        protected InstallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InstallException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

