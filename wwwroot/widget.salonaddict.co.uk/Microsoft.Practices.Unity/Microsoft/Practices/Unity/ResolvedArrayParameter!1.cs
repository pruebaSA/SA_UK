namespace Microsoft.Practices.Unity
{
    using System;

    public class ResolvedArrayParameter<TElement> : ResolvedArrayParameter
    {
        public ResolvedArrayParameter(params object[] elementValues) : base(typeof(TElement[]), typeof(TElement), elementValues)
        {
        }
    }
}

