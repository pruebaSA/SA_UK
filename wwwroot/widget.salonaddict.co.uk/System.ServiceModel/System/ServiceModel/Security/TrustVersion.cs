﻿namespace System.ServiceModel.Security
{
    using System;
    using System.Xml;

    public abstract class TrustVersion
    {
        private readonly XmlDictionaryString prefix;
        private readonly XmlDictionaryString trustNamespace;

        internal TrustVersion(XmlDictionaryString ns, XmlDictionaryString prefix)
        {
            this.trustNamespace = ns;
            this.prefix = prefix;
        }

        public static TrustVersion Default =>
            WSTrustFeb2005;

        public XmlDictionaryString Namespace =>
            this.trustNamespace;

        public XmlDictionaryString Prefix =>
            this.prefix;

        public static TrustVersion WSTrust13 =>
            WSTrustVersion13.Instance;

        public static TrustVersion WSTrustFeb2005 =>
            WSTrustVersionFeb2005.Instance;

        private class WSTrustVersion13 : TrustVersion
        {
            private static readonly TrustVersion.WSTrustVersion13 instance = new TrustVersion.WSTrustVersion13();

            protected WSTrustVersion13() : base(DXD.TrustDec2005Dictionary.Namespace, DXD.TrustDec2005Dictionary.Prefix)
            {
            }

            public static TrustVersion Instance =>
                instance;
        }

        private class WSTrustVersionFeb2005 : TrustVersion
        {
            private static readonly TrustVersion.WSTrustVersionFeb2005 instance = new TrustVersion.WSTrustVersionFeb2005();

            protected WSTrustVersionFeb2005() : base(XD.TrustFeb2005Dictionary.Namespace, XD.TrustFeb2005Dictionary.Prefix)
            {
            }

            public static TrustVersion Instance =>
                instance;
        }
    }
}

