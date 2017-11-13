namespace System.Configuration.Install
{
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Text;

    public class InstallContext
    {
        private string logFilePath;
        private StringDictionary parameters;

        public InstallContext() : this(null, null)
        {
        }

        public InstallContext(string logFilePath, string[] commandLine)
        {
            this.parameters = ParseCommandLine(commandLine);
            if (this.Parameters["logfile"] != null)
            {
                this.logFilePath = this.Parameters["logfile"];
            }
            else if (logFilePath != null)
            {
                this.logFilePath = logFilePath;
                this.Parameters["logfile"] = logFilePath;
            }
        }

        public bool IsParameterTrue(string paramName)
        {
            string strA = this.Parameters[paramName.ToLower(CultureInfo.InvariantCulture)];
            if (strA == null)
            {
                return false;
            }
            if (((string.Compare(strA, "true", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(strA, "yes", StringComparison.OrdinalIgnoreCase) != 0)) && (string.Compare(strA, "1", StringComparison.OrdinalIgnoreCase) != 0))
            {
                return "".Equals(strA);
            }
            return true;
        }

        public void LogMessage(string message)
        {
            this.logFilePath = this.Parameters["logfile"];
            if ((this.logFilePath != null) && !"".Equals(this.logFilePath))
            {
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(this.logFilePath, true, Encoding.UTF8);
                    writer.WriteLine(message);
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }
                }
            }
            if (this.IsParameterTrue("LogToConsole") || (this.Parameters["logtoconsole"] == null))
            {
                Console.WriteLine(message);
            }
        }

        protected static StringDictionary ParseCommandLine(string[] args)
        {
            StringDictionary dictionary = new StringDictionary();
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].StartsWith("/", StringComparison.Ordinal) || args[i].StartsWith("-", StringComparison.Ordinal))
                    {
                        args[i] = args[i].Substring(1);
                    }
                    int index = args[i].IndexOf('=');
                    if (index < 0)
                    {
                        dictionary[args[i].ToLower(CultureInfo.InvariantCulture)] = "";
                    }
                    else
                    {
                        dictionary[args[i].Substring(0, index).ToLower(CultureInfo.InvariantCulture)] = args[i].Substring(index + 1);
                    }
                }
            }
            return dictionary;
        }

        public StringDictionary Parameters =>
            this.parameters;
    }
}

