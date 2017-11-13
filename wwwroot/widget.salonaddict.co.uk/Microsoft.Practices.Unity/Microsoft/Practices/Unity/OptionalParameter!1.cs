namespace Microsoft.Practices.Unity
{
    using System;

    public class OptionalParameter<T> : OptionalParameter
    {
        public OptionalParameter() : base(typeof(T))
        {
        }

        public OptionalParameter(string name) : base(typeof(T), name)
        {
        }
    }
}

