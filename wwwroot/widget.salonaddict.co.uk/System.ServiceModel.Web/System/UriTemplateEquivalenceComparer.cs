namespace System
{
    using System.Collections.Generic;
    using System.ServiceModel;

    public class UriTemplateEquivalenceComparer : IEqualityComparer<UriTemplate>
    {
        private static UriTemplateEquivalenceComparer instance;

        public bool Equals(UriTemplate x, UriTemplate y) => 
            x?.IsEquivalentTo(y);

        public int GetHashCode(UriTemplate obj)
        {
            if (obj == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("obj");
            }
            for (int i = obj.segments.Count - 1; i >= 0; i--)
            {
                if (obj.segments[i].Nature == UriTemplatePartType.Literal)
                {
                    return obj.segments[i].GetHashCode();
                }
            }
            return (obj.segments.Count + obj.queries.Count);
        }

        internal static UriTemplateEquivalenceComparer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UriTemplateEquivalenceComparer();
                }
                return instance;
            }
        }
    }
}

