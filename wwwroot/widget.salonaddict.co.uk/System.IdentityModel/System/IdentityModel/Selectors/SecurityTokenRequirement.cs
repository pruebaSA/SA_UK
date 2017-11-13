namespace System.IdentityModel.Selectors
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel;
    using System.IdentityModel.Tokens;
    using System.Runtime.InteropServices;

    public class SecurityTokenRequirement
    {
        private const bool defaultIsOptionalToken = false;
        private const int defaultKeySize = 0;
        private const SecurityKeyType defaultKeyType = SecurityKeyType.SymmetricKey;
        private const SecurityKeyUsage defaultKeyUsage = SecurityKeyUsage.Signature;
        private const bool defaultRequireCryptographicToken = false;
        private const string isOptionalTokenProperty = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/IsOptionalTokenProperty";
        private const string keySizeProperty = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/KeySize";
        private const string keyTypeProperty = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/KeyType";
        private const string keyUsageProperty = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/KeyUsage";
        private const string Namespace = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement";
        private const string peerAuthenticationMode = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/PeerAuthenticationMode";
        private Dictionary<string, object> properties = new Dictionary<string, object>();
        private const string requireCryptographicTokenProperty = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/RequireCryptographicToken";
        private const string tokenTypeProperty = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/TokenType";

        public SecurityTokenRequirement()
        {
            this.Initialize();
        }

        public TValue GetProperty<TValue>(string propertyName)
        {
            TValue local;
            if (!this.TryGetProperty<TValue>(propertyName, out local))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.IdentityModel.SR.GetString("SecurityTokenRequirementDoesNotContainProperty", new object[] { propertyName })));
            }
            return local;
        }

        private void Initialize()
        {
            this.KeyType = SecurityKeyType.SymmetricKey;
            this.KeyUsage = SecurityKeyUsage.Signature;
            this.RequireCryptographicToken = false;
            this.KeySize = 0;
            this.IsOptionalToken = false;
        }

        public bool TryGetProperty<TValue>(string propertyName, out TValue result)
        {
            object obj2;
            if (!this.Properties.TryGetValue(propertyName, out obj2))
            {
                result = default(TValue);
                return false;
            }
            if ((obj2 != null) && !typeof(TValue).IsAssignableFrom(obj2.GetType()))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.IdentityModel.SR.GetString("SecurityTokenRequirementHasInvalidTypeForProperty", new object[] { propertyName, obj2.GetType(), typeof(TValue) })));
            }
            result = (TValue) obj2;
            return true;
        }

        internal bool IsOptionalToken
        {
            get
            {
                bool flag;
                if (!this.TryGetProperty<bool>(IsOptionalTokenProperty, out flag))
                {
                    return false;
                }
                return flag;
            }
            set
            {
                this.properties[IsOptionalTokenProperty] = value;
            }
        }

        public static string IsOptionalTokenProperty =>
            "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/IsOptionalTokenProperty";

        public int KeySize
        {
            get
            {
                int num;
                if (!this.TryGetProperty<int>(KeySizeProperty, out num))
                {
                    return 0;
                }
                return num;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", System.IdentityModel.SR.GetString("ValueMustBeNonNegative")));
                }
                this.Properties[KeySizeProperty] = value;
            }
        }

        public static string KeySizeProperty =>
            "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/KeySize";

        public SecurityKeyType KeyType
        {
            get
            {
                SecurityKeyType type;
                if (!this.TryGetProperty<SecurityKeyType>(KeyTypeProperty, out type))
                {
                    return SecurityKeyType.SymmetricKey;
                }
                return type;
            }
            set
            {
                SecurityKeyTypeHelper.Validate(value);
                this.properties[KeyTypeProperty] = value;
            }
        }

        public static string KeyTypeProperty =>
            "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/KeyType";

        public SecurityKeyUsage KeyUsage
        {
            get
            {
                SecurityKeyUsage usage;
                if (!this.TryGetProperty<SecurityKeyUsage>(KeyUsageProperty, out usage))
                {
                    return SecurityKeyUsage.Signature;
                }
                return usage;
            }
            set
            {
                SecurityKeyUsageHelper.Validate(value);
                this.properties[KeyUsageProperty] = value;
            }
        }

        public static string KeyUsageProperty =>
            "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/KeyUsage";

        public static string PeerAuthenticationMode =>
            "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/PeerAuthenticationMode";

        public IDictionary<string, object> Properties =>
            this.properties;

        public bool RequireCryptographicToken
        {
            get
            {
                bool flag;
                if (!this.TryGetProperty<bool>(RequireCryptographicTokenProperty, out flag))
                {
                    return false;
                }
                return flag;
            }
            set
            {
                this.properties[RequireCryptographicTokenProperty] = value;
            }
        }

        public static string RequireCryptographicTokenProperty =>
            "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/RequireCryptographicToken";

        public string TokenType
        {
            get
            {
                string str;
                if (!this.TryGetProperty<string>(TokenTypeProperty, out str))
                {
                    return null;
                }
                return str;
            }
            set
            {
                this.properties[TokenTypeProperty] = value;
            }
        }

        public static string TokenTypeProperty =>
            "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/TokenType";
    }
}

