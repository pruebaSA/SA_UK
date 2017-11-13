namespace System.Data.Design
{
    using System;

    internal sealed class DSGeneratorProblem
    {
        private string message;
        private DataSourceComponent problemSource;
        private ProblemSeverity severity;

        internal DSGeneratorProblem(string message, ProblemSeverity severity, DataSourceComponent problemSource)
        {
            this.message = message;
            this.severity = severity;
            this.problemSource = problemSource;
        }

        internal string Message =>
            this.message;

        internal DataSourceComponent ProblemSource =>
            this.problemSource;

        internal ProblemSeverity Severity =>
            this.severity;
    }
}

