namespace System.Web.Util
{
    using Microsoft.Win32;
    using System;
    using System.Globalization;
    using System.Security.Permissions;

    internal static class EnableViewStateMacRegistryHelper
    {
        public static readonly bool EnforceViewStateMac;
        public static readonly bool SuppressMacValidationErrorsAlways;
        public static readonly bool SuppressMacValidationErrorsFromCrossPagePostbacks;
        public static readonly bool WriteViewStateGeneratorField;

        static EnableViewStateMacRegistryHelper()
        {
            if (IsMacEnforcementEnabledViaRegistry())
            {
                EnforceViewStateMac = true;
                SuppressMacValidationErrorsFromCrossPagePostbacks = true;
            }
            if (AppSettings.AllowInsecureDeserialization.HasValue)
            {
                EnforceViewStateMac = !AppSettings.AllowInsecureDeserialization.Value;
                SuppressMacValidationErrorsFromCrossPagePostbacks |= !AppSettings.AllowInsecureDeserialization.Value;
            }
            SuppressMacValidationErrorsAlways = AppSettings.AlwaysIgnoreViewStateValidationErrors;
            if (SuppressMacValidationErrorsAlways)
            {
                SuppressMacValidationErrorsFromCrossPagePostbacks = true;
            }
            else if (SuppressMacValidationErrorsFromCrossPagePostbacks)
            {
                WriteViewStateGeneratorField = true;
            }
        }

        [RegistryPermission(SecurityAction.Assert, Unrestricted=true)]
        private static bool IsMacEnforcementEnabledViaRegistry()
        {
            try
            {
                int num = (int) Registry.GetValue(string.Format(CultureInfo.InvariantCulture, @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v{0}", new object[] { Environment.Version.ToString(3) }), "AspNetEnforceViewStateMac", 0);
                return (num != 0);
            }
            catch
            {
                return true;
            }
        }
    }
}

