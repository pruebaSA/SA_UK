namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data.Entity;
    using System.Globalization;

    [Serializable]
    public sealed class EdmSchemaError : EdmError
    {
        private int _column;
        private int _errorCode;
        private int _line;
        private string _schemaLocation;
        private EdmSchemaErrorSeverity _severity;
        private string _stackTrace;

        internal EdmSchemaError(string message, int errorCode, EdmSchemaErrorSeverity severity) : this(message, errorCode, severity, null)
        {
        }

        internal EdmSchemaError(string message, int errorCode, EdmSchemaErrorSeverity severity, Exception exception) : base(message)
        {
            this._line = -1;
            this._column = -1;
            this._stackTrace = string.Empty;
            this.Initialize(errorCode, severity, null, -1, -1, exception);
        }

        internal EdmSchemaError(string message, int errorCode, EdmSchemaErrorSeverity severity, string schemaLocation, int line, int column) : this(message, errorCode, severity, schemaLocation, line, column, null)
        {
        }

        internal EdmSchemaError(string message, int errorCode, EdmSchemaErrorSeverity severity, string schemaLocation, int line, int column, Exception exception) : base(message)
        {
            this._line = -1;
            this._column = -1;
            this._stackTrace = string.Empty;
            if ((severity < EdmSchemaErrorSeverity.Warning) || (severity > EdmSchemaErrorSeverity.Error))
            {
                throw new ArgumentOutOfRangeException("severity", severity, Strings.ArgumentOutOfRange(severity));
            }
            if (line < 0)
            {
                throw new ArgumentOutOfRangeException("line", line, Strings.ArgumentOutOfRangeExpectedPostiveNumber(line));
            }
            if (column < 0)
            {
                throw new ArgumentOutOfRangeException("column", column, Strings.ArgumentOutOfRangeExpectedPostiveNumber(column));
            }
            this.Initialize(errorCode, severity, schemaLocation, line, column, exception);
        }

        private static string GetNameFromSchemaLocation(string schemaLocation)
        {
            if (string.IsNullOrEmpty(schemaLocation))
            {
                return schemaLocation;
            }
            int num = Math.Max(schemaLocation.LastIndexOf('/'), schemaLocation.LastIndexOf('\\'));
            int startIndex = num + 1;
            if (num < 0)
            {
                return schemaLocation;
            }
            if (startIndex >= schemaLocation.Length)
            {
                return string.Empty;
            }
            return schemaLocation.Substring(startIndex);
        }

        private void Initialize(int errorCode, EdmSchemaErrorSeverity severity, string schemaLocation, int line, int column, Exception exception)
        {
            if (errorCode < 0)
            {
                throw new ArgumentOutOfRangeException("errorCode", errorCode, Strings.ArgumentOutOfRangeExpectedPostiveNumber(errorCode));
            }
            this._errorCode = errorCode;
            this._severity = severity;
            this._schemaLocation = schemaLocation;
            this._line = line;
            this._column = column;
            if (exception != null)
            {
                this._stackTrace = exception.StackTrace;
            }
        }

        public override string ToString()
        {
            string generatorErrorSeverityWarning;
            switch (this.Severity)
            {
                case EdmSchemaErrorSeverity.Warning:
                    generatorErrorSeverityWarning = Strings.GeneratorErrorSeverityWarning;
                    break;

                case EdmSchemaErrorSeverity.Error:
                    generatorErrorSeverityWarning = Strings.GeneratorErrorSeverityError;
                    break;

                default:
                    generatorErrorSeverityWarning = Strings.GeneratorErrorSeverityUnknown;
                    break;
            }
            if ((string.IsNullOrEmpty(this.SchemaName) && (this.Line < 0)) && (this.Column < 0))
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} {1:0000}: {2}", new object[] { generatorErrorSeverityWarning, this.ErrorCode, base.Message });
            }
            return string.Format(CultureInfo.CurrentCulture, "{0}({1},{2}) : {3} {4:0000}: {5}", new object[] { (this.SchemaName == null) ? Strings.SourceUriUnknown : this.SchemaName, this.Line, this.Column, generatorErrorSeverityWarning, this.ErrorCode, base.Message });
        }

        public int Column =>
            this._column;

        public int ErrorCode =>
            this._errorCode;

        public int Line =>
            this._line;

        public string SchemaLocation =>
            this._schemaLocation;

        public string SchemaName =>
            GetNameFromSchemaLocation(this.SchemaLocation);

        public EdmSchemaErrorSeverity Severity =>
            this._severity;

        public string StackTrace =>
            this._stackTrace;
    }
}

