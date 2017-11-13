namespace MigraDoc.DocumentObjectModel.IO
{
    using MigraDoc.DocumentObjectModel;
    using System;

    internal class DdlParserException : Exception
    {
        private DdlReaderError error;

        public DdlParserException(DdlReaderError error) : base(error.ErrorMessage)
        {
            this.error = error;
        }

        public DdlParserException(string message) : base(message)
        {
            this.error = new DdlReaderError(DdlErrorLevel.Error, message, 0);
        }

        public DdlParserException(string message, Exception innerException) : base(message, innerException)
        {
            this.error = new DdlReaderError(DdlErrorLevel.Error, message, 0);
        }

        public DdlParserException(DdlErrorLevel level, string message, DomMsgID errorCode) : base(message)
        {
            this.error = new DdlReaderError(level, message, (int) errorCode);
        }

        public DdlReaderError Error =>
            this.error;
    }
}

