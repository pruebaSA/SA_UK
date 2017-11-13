﻿namespace System.ServiceModel.Activation
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;

    internal abstract class MetabaseSettings
    {
        internal const char AboPathDelimiter = '/';
        private IDictionary<string, string[]> bindingsTable = new Dictionary<string, string[]>();
        internal const string DotDelimiter = ".";
        private List<string> enabledProtocols = new List<string>();
        internal const string LocalMachine = "localhost";

        protected MetabaseSettings()
        {
        }

        protected static ExtendedProtectionPolicy BuildExtendedProtectionPolicy(ExtendedProtectionTokenChecking tokenChecking, ExtendedProtectionFlags flags, List<string> spnList)
        {
            PolicyEnforcement whenSupported;
            ProtectionScenario transportSelected;
            ServiceNameCollection customServiceNames = null;
            if (tokenChecking == ExtendedProtectionTokenChecking.None)
            {
                return new ExtendedProtectionPolicy(PolicyEnforcement.Never);
            }
            if (tokenChecking == ExtendedProtectionTokenChecking.Allow)
            {
                whenSupported = PolicyEnforcement.WhenSupported;
            }
            else
            {
                if (tokenChecking != ExtendedProtectionTokenChecking.Require)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("Hosting_UnrecognizedTokenCheckingValue")));
                }
                whenSupported = PolicyEnforcement.Always;
            }
            bool flag = flags == ExtendedProtectionFlags.None;
            bool flag2 = flags == ExtendedProtectionFlags.AllowDotlessSpn;
            bool flag3 = ((flags & ExtendedProtectionFlags.Proxy) != ExtendedProtectionFlags.None) && ((flags & ExtendedProtectionFlags.ProxyCohosting) != ExtendedProtectionFlags.None);
            bool flag4 = (flags & ExtendedProtectionFlags.Proxy) != ExtendedProtectionFlags.None;
            if ((flag || flag2) || flag3)
            {
                transportSelected = ProtectionScenario.TransportSelected;
            }
            else
            {
                if (!flag4)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionFlagsNotSupport", new object[] { flags })));
                }
                transportSelected = ProtectionScenario.TrustedProxy;
            }
            if (spnList != null)
            {
                if ((flags & ExtendedProtectionFlags.AllowDotlessSpn) == ExtendedProtectionFlags.None)
                {
                    foreach (string str in spnList)
                    {
                        string[] strArray = str.Split(new char[] { '/' });
                        if (strArray.Length <= 1)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionSpnFormatError", new object[] { str })));
                        }
                        int index = strArray[1].IndexOf(".", StringComparison.CurrentCultureIgnoreCase);
                        if (index == -1)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionDotlessSpnNotEnabled", new object[] { str })));
                        }
                        if ((index == 0) || (index == (strArray[1].Length - 1)))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionSpnFormatError", new object[] { str })));
                        }
                    }
                }
                if (spnList.Count != 0)
                {
                    customServiceNames = new ServiceNameCollection(spnList);
                }
            }
            return new ExtendedProtectionPolicy(whenSupported, transportSelected, customServiceNames);
        }

        internal abstract HttpAccessSslFlags GetAccessSslFlags(string virtualPath);
        internal bool GetAllowSslOnly(string virtualPath) => 
            ((this.GetAccessSslFlags(virtualPath) & HttpAccessSslFlags.Ssl) != HttpAccessSslFlags.None);

        internal abstract AuthenticationSchemes GetAuthenticationSchemes(string virtualPath);
        internal string[] GetBindings(string scheme) => 
            this.bindingsTable[scheme];

        internal abstract ExtendedProtectionPolicy GetExtendedProtectionPolicy(string virtualPath);
        internal string[] GetProtocols() => 
            this.enabledProtocols.ToArray();

        internal abstract string GetRealm(string virtualPath);

        protected IDictionary<string, string[]> Bindings
        {
            get => 
                this.bindingsTable;
            set
            {
                this.bindingsTable = value;
            }
        }

        protected List<string> Protocols
        {
            get => 
                this.enabledProtocols;
            set
            {
                this.enabledProtocols = value;
            }
        }
    }
}

