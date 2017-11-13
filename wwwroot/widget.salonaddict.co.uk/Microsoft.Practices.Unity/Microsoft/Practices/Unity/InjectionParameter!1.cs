namespace Microsoft.Practices.Unity
{
    using System;

    public class InjectionParameter<TParameter> : InjectionParameter
    {
        public InjectionParameter(TParameter parameterValue) : base(typeof(TParameter), parameterValue)
        {
        }
    }
}

