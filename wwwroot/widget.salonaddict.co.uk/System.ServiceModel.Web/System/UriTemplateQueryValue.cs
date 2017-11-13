namespace System
{
    using System.Collections.Specialized;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Text;

    internal abstract class UriTemplateQueryValue
    {
        private static UriTemplateQueryValue empty = new EmptyUriTemplateQueryValue();
        private readonly UriTemplatePartType nature;

        protected UriTemplateQueryValue(UriTemplatePartType nature)
        {
            this.nature = nature;
        }

        public abstract void Bind(string keyName, string[] values, ref int valueIndex, StringBuilder query);
        public static UriTemplateQueryValue CreateFromUriTemplate(string value, UriTemplate template)
        {
            if (value == null)
            {
                return Empty;
            }
            switch (UriTemplateHelpers.IdentifyPartType(value))
            {
                case UriTemplatePartType.Literal:
                    return UriTemplateLiteralQueryValue.CreateFromUriTemplate(value);

                case UriTemplatePartType.Compound:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.UTQueryCannotHaveCompoundValue, new object[] { template.originalTemplate })));

                case UriTemplatePartType.Variable:
                    return new UriTemplateVariableQueryValue(template.AddQueryVariable(value.Substring(1, value.Length - 2)));
            }
            return null;
        }

        public abstract bool IsEquivalentTo(UriTemplateQueryValue other);
        public static bool IsNullOrEmpty(UriTemplateQueryValue utqv) => 
            ((utqv == null) || (utqv == Empty));

        public abstract void Lookup(string value, NameValueCollection boundParameters);

        public static UriTemplateQueryValue Empty =>
            empty;

        public UriTemplatePartType Nature =>
            this.nature;

        private class EmptyUriTemplateQueryValue : UriTemplateQueryValue
        {
            public EmptyUriTemplateQueryValue() : base(UriTemplatePartType.Literal)
            {
            }

            public override void Bind(string keyName, string[] values, ref int valueIndex, StringBuilder query)
            {
                query.AppendFormat("&{0}", UrlUtility.UrlEncode(keyName, Encoding.UTF8));
            }

            public override bool IsEquivalentTo(UriTemplateQueryValue other) => 
                (other == UriTemplateQueryValue.Empty);

            public override void Lookup(string value, NameValueCollection boundParameters)
            {
            }
        }
    }
}

