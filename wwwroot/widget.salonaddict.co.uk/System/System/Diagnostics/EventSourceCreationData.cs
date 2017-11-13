namespace System.Diagnostics
{
    using System;

    public class EventSourceCreationData
    {
        private int _categoryCount;
        private string _categoryResourceFile;
        private string _logName;
        private string _machineName;
        private string _messageResourceFile;
        private string _parameterResourceFile;
        private string _source;

        private EventSourceCreationData()
        {
            this._logName = "Application";
            this._machineName = ".";
        }

        public EventSourceCreationData(string source, string logName)
        {
            this._logName = "Application";
            this._machineName = ".";
            this._source = source;
            this._logName = logName;
        }

        internal EventSourceCreationData(string source, string logName, string machineName)
        {
            this._logName = "Application";
            this._machineName = ".";
            this._source = source;
            this._logName = logName;
            this._machineName = machineName;
        }

        private EventSourceCreationData(string source, string logName, string machineName, string messageResourceFile, string parameterResourceFile, string categoryResourceFile, short categoryCount)
        {
            this._logName = "Application";
            this._machineName = ".";
            this._source = source;
            this._logName = logName;
            this._machineName = machineName;
            this._messageResourceFile = messageResourceFile;
            this._parameterResourceFile = parameterResourceFile;
            this._categoryResourceFile = categoryResourceFile;
            this.CategoryCount = categoryCount;
        }

        public int CategoryCount
        {
            get => 
                this._categoryCount;
            set
            {
                if ((value > 0xffff) || (value < 0))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._categoryCount = value;
            }
        }

        public string CategoryResourceFile
        {
            get => 
                this._categoryResourceFile;
            set
            {
                this._categoryResourceFile = value;
            }
        }

        public string LogName
        {
            get => 
                this._logName;
            set
            {
                this._logName = value;
            }
        }

        public string MachineName
        {
            get => 
                this._machineName;
            set
            {
                this._machineName = value;
            }
        }

        public string MessageResourceFile
        {
            get => 
                this._messageResourceFile;
            set
            {
                this._messageResourceFile = value;
            }
        }

        public string ParameterResourceFile
        {
            get => 
                this._parameterResourceFile;
            set
            {
                this._parameterResourceFile = value;
            }
        }

        public string Source
        {
            get => 
                this._source;
            set
            {
                this._source = value;
            }
        }
    }
}

