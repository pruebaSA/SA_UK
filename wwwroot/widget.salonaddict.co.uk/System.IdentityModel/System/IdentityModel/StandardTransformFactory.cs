namespace System.IdentityModel
{
    using System;
    using System.Security.Cryptography;

    internal class StandardTransformFactory : TransformFactory
    {
        private static StandardTransformFactory instance = new StandardTransformFactory();

        protected StandardTransformFactory()
        {
        }

        public override Transform CreateTransform(string transformAlgorithmUri)
        {
            if (transformAlgorithmUri == "http://www.w3.org/2001/10/xml-exc-c14n#")
            {
                return new ExclusiveCanonicalizationTransform();
            }
            if (transformAlgorithmUri != "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#STR-Transform")
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(System.IdentityModel.SR.GetString("UnsupportedTransformAlgorithm")));
            }
            return new StrTransform();
        }

        internal static StandardTransformFactory Instance =>
            instance;
    }
}

