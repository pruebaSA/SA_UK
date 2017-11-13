﻿namespace Microsoft.InfoCards
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal abstract class InfoCardBaseException : Exception
    {
        private string m_extendedMessage;
        private bool m_logged;

        protected InfoCardBaseException(int result)
        {
            base.HResult = result;
        }

        protected InfoCardBaseException(int result, string message) : base(message)
        {
            base.HResult = result;
        }

        protected InfoCardBaseException(int result, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            base.HResult = result;
        }

        protected InfoCardBaseException(int result, string message, Exception innerException) : base(message, innerException)
        {
            base.HResult = result;
        }

        protected InfoCardBaseException(int result, string message, string extendedMessage) : base(message)
        {
            base.HResult = result;
            this.m_extendedMessage = extendedMessage;
        }

        public void MarkLogged()
        {
            this.m_logged = true;
        }

        public string ExtendedMessage =>
            this.m_extendedMessage;

        public bool Logged =>
            this.m_logged;

        public int NativeHResult =>
            base.HResult;
    }
}

