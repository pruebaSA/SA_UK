namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class DependencyMissingException : Exception
    {
        public DependencyMissingException()
        {
        }

        public DependencyMissingException(object buildKey) : base(string.Format(CultureInfo.CurrentCulture, Resources.MissingDependency, new object[] { buildKey }))
        {
        }

        public DependencyMissingException(string message) : base(message)
        {
        }

        protected DependencyMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DependencyMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

