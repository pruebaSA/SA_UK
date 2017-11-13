namespace Microsoft.Practices.Unity
{
    using System;

    public class ResolvedParameter<TParameter> : ResolvedParameter
    {
        public ResolvedParameter() : base(typeof(TParameter))
        {
        }

        public ResolvedParameter(string name) : base(typeof(TParameter), name)
        {
        }
    }
}

