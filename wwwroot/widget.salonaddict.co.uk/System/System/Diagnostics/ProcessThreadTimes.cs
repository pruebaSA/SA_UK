namespace System.Diagnostics
{
    using System;

    internal class ProcessThreadTimes
    {
        internal long create;
        internal long exit;
        internal long kernel;
        internal long user;

        public DateTime ExitTime =>
            DateTime.FromFileTime(this.exit);

        public TimeSpan PrivilegedProcessorTime =>
            new TimeSpan(this.kernel);

        public DateTime StartTime =>
            DateTime.FromFileTime(this.create);

        public TimeSpan TotalProcessorTime =>
            new TimeSpan(this.user + this.kernel);

        public TimeSpan UserProcessorTime =>
            new TimeSpan(this.user);
    }
}

