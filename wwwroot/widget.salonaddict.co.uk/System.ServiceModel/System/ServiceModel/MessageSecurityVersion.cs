namespace System.ServiceModel
{
    using System;
    using System.IdentityModel.Selectors;
    using System.ServiceModel.Security;

    public abstract class MessageSecurityVersion
    {
        internal MessageSecurityVersion()
        {
        }

        public abstract System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion { get; }

        public static MessageSecurityVersion Default =>
            WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion.Instance;

        internal abstract System.ServiceModel.Security.MessageSecurityTokenVersion MessageSecurityTokenVersion { get; }

        public System.ServiceModel.Security.SecureConversationVersion SecureConversationVersion =>
            this.MessageSecurityTokenVersion.SecureConversationVersion;

        public abstract System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion { get; }

        public System.IdentityModel.Selectors.SecurityTokenVersion SecurityTokenVersion =>
            this.MessageSecurityTokenVersion;

        public System.ServiceModel.Security.SecurityVersion SecurityVersion =>
            this.MessageSecurityTokenVersion.SecurityVersion;

        public System.ServiceModel.Security.TrustVersion TrustVersion =>
            this.MessageSecurityTokenVersion.TrustVersion;

        public static MessageSecurityVersion WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10 =>
            WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion.Instance;

        public static MessageSecurityVersion WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 =>
            WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion.Instance;

        public static MessageSecurityVersion WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12 =>
            WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion.Instance;

        public static MessageSecurityVersion WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10 =>
            WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion.Instance;

        public static MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11 =>
            WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion.Instance;

        public static MessageSecurityVersion WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10 =>
            WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion.Instance;

        internal static MessageSecurityVersion WSSXDefault =>
            WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion.Instance;

        private class WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion instance = new MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion();

            public override string ToString() => 
                "WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10";

            public override System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion =>
                null;

            public static MessageSecurityVersion Instance =>
                instance;

            internal override System.ServiceModel.Security.MessageSecurityTokenVersion MessageSecurityTokenVersion =>
                System.ServiceModel.Security.MessageSecurityTokenVersion.WSSecurity10WSTrust13WSSecureConversation13BasicSecurityProfile10;

            public override System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion =>
                System.ServiceModel.Security.SecurityPolicyVersion.WSSecurityPolicy12;
        }

        private class WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion instance = new MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion();

            public override string ToString() => 
                "WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10";

            public override System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion =>
                System.ServiceModel.Security.BasicSecurityProfileVersion.BasicSecurityProfile10;

            public static MessageSecurityVersion Instance =>
                instance;

            internal override System.ServiceModel.Security.MessageSecurityTokenVersion MessageSecurityTokenVersion =>
                System.ServiceModel.Security.MessageSecurityTokenVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005BasicSecurityProfile10;

            public override System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion =>
                System.ServiceModel.Security.SecurityPolicyVersion.WSSecurityPolicy11;
        }

        private class WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion instance = new MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10MessageSecurityVersion();

            public override string ToString() => 
                "WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10";

            public override System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion =>
                null;

            public static MessageSecurityVersion Instance =>
                instance;

            internal override System.ServiceModel.Security.MessageSecurityTokenVersion MessageSecurityTokenVersion =>
                System.ServiceModel.Security.MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13BasicSecurityProfile10;

            public override System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion =>
                System.ServiceModel.Security.SecurityPolicyVersion.WSSecurityPolicy12;
        }

        private class WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion instance = new MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12MessageSecurityVersion();

            public override string ToString() => 
                "WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12";

            public override System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion =>
                null;

            public static MessageSecurityVersion Instance =>
                instance;

            internal override System.ServiceModel.Security.MessageSecurityTokenVersion MessageSecurityTokenVersion =>
                System.ServiceModel.Security.MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13;

            public override System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion =>
                System.ServiceModel.Security.SecurityPolicyVersion.WSSecurityPolicy12;
        }

        private class WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion instance = new MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10MessageSecurityVersion();

            public override string ToString() => 
                "WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10";

            public override System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion =>
                System.ServiceModel.Security.BasicSecurityProfileVersion.BasicSecurityProfile10;

            public static MessageSecurityVersion Instance =>
                instance;

            internal override System.ServiceModel.Security.MessageSecurityTokenVersion MessageSecurityTokenVersion =>
                System.ServiceModel.Security.MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005BasicSecurityProfile10;

            public override System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion =>
                System.ServiceModel.Security.SecurityPolicyVersion.WSSecurityPolicy11;
        }

        private class WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion : MessageSecurityVersion
        {
            private static MessageSecurityVersion instance = new MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11MessageSecurityVersion();

            public override string ToString() => 
                "WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11";

            public override System.ServiceModel.Security.BasicSecurityProfileVersion BasicSecurityProfileVersion =>
                null;

            public static MessageSecurityVersion Instance =>
                instance;

            internal override System.ServiceModel.Security.MessageSecurityTokenVersion MessageSecurityTokenVersion =>
                System.ServiceModel.Security.MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005;

            public override System.ServiceModel.Security.SecurityPolicyVersion SecurityPolicyVersion =>
                System.ServiceModel.Security.SecurityPolicyVersion.WSSecurityPolicy11;
        }
    }
}

