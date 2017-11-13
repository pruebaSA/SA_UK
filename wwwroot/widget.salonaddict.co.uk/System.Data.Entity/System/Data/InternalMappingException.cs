namespace System.Data
{
    using System;
    using System.Data.Mapping.ViewGeneration.Structures;
    using System.Runtime.Serialization;

    [Serializable]
    internal class InternalMappingException : EntityException
    {
        private System.Data.Mapping.ViewGeneration.Structures.ErrorLog m_errorLog;

        internal InternalMappingException()
        {
        }

        internal InternalMappingException(string message) : base(message)
        {
        }

        protected InternalMappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal InternalMappingException(string message, System.Data.Mapping.ViewGeneration.Structures.ErrorLog errorLog) : base(message)
        {
            EntityUtil.CheckArgumentNull<System.Data.Mapping.ViewGeneration.Structures.ErrorLog>(errorLog, "errorLog");
            this.m_errorLog = errorLog;
        }

        internal InternalMappingException(string message, System.Data.Mapping.ViewGeneration.Structures.ErrorLog.Record record) : base(message)
        {
            EntityUtil.CheckArgumentNull<System.Data.Mapping.ViewGeneration.Structures.ErrorLog.Record>(record, "record");
            this.m_errorLog = new System.Data.Mapping.ViewGeneration.Structures.ErrorLog();
            this.m_errorLog.AddEntry(record);
        }

        internal InternalMappingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal System.Data.Mapping.ViewGeneration.Structures.ErrorLog ErrorLog =>
            this.m_errorLog;
    }
}

