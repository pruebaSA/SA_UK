namespace System.ServiceModel.Security.Tokens
{
    using System;

    public static class ServiceModelSecurityTokenTypes
    {
        private const string anonymousSslnego = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/AnonymousSslnego";
        private const string mutualSslnego = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/MutualSslnego";
        private const string Namespace = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens";
        private const string secureConversation = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/SecureConversation";
        private const string securityContext = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/SecurityContextToken";
        private const string spnego = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/Spnego";
        private const string sspiCredential = "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/SspiCredential";

        public static string AnonymousSslnego =>
            "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/AnonymousSslnego";

        public static string MutualSslnego =>
            "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/MutualSslnego";

        public static string SecureConversation =>
            "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/SecureConversation";

        public static string SecurityContext =>
            "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/SecurityContextToken";

        public static string Spnego =>
            "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/Spnego";

        public static string SspiCredential =>
            "http://schemas.microsoft.com/ws/2006/05/servicemodel/tokens/SspiCredential";
    }
}

