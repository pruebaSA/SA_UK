namespace MigraDoc.DocumentObjectModel.IO
{
    using System;

    public class DdlReaderError
    {
        public DdlErrorLevel ErrorLevel;
        public string ErrorMessage;
        public int ErrorNumber;
        public const int NoErrorNumber = -1;
        public int SourceColumn;
        public string SourceFile;
        public int SourceLine;

        public DdlReaderError(DdlErrorLevel errorLevel, string errorMessage, int errorNumber)
        {
            this.ErrorLevel = errorLevel;
            this.ErrorMessage = errorMessage;
            this.ErrorNumber = errorNumber;
        }

        public DdlReaderError(DdlErrorLevel errorLevel, string errorMessage, int errorNumber, string sourceFile, int sourceLine, int sourceColumn)
        {
            this.ErrorLevel = errorLevel;
            this.ErrorMessage = errorMessage;
            this.ErrorNumber = errorNumber;
            this.SourceFile = sourceFile;
            this.SourceLine = sourceLine;
            this.SourceColumn = sourceColumn;
        }

        public override string ToString() => 
            $"[{this.SourceFile}({this.SourceLine},{this.SourceColumn}):] {"xxx"} DDL{this.ErrorNumber}: {this.ErrorMessage}";
    }
}

