namespace Microsoft.Practices.Unity.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ParameterMatcher
    {
        private readonly List<InjectionParameterValue> parametersToMatch;

        public ParameterMatcher(IEnumerable<InjectionParameterValue> parametersToMatch)
        {
            this.parametersToMatch = new List<InjectionParameterValue>(parametersToMatch);
        }

        public virtual bool Matches(IEnumerable<ParameterInfo> candidate) => 
            this.Matches((IEnumerable<Type>) (from pi in candidate select pi.ParameterType));

        public virtual bool Matches(IEnumerable<Type> candidate)
        {
            List<Type> list = new List<Type>(candidate);
            if (this.parametersToMatch.Count == list.Count)
            {
                for (int i = 0; i < this.parametersToMatch.Count; i++)
                {
                    if (!this.parametersToMatch[i].MatchesType(list[i]))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}

