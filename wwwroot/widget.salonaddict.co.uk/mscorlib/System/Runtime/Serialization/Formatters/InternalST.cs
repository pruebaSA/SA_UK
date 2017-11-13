namespace System.Runtime.Serialization.Formatters
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [ComVisible(true), StrongNameIdentityPermission(SecurityAction.LinkDemand, PublicKey="0x002400000480000094000000060200000024000052534131000400000100010007D1FA57C4AED9F0A32E84AA0FAEFD0DE9E8FD6AEC8F87FB03766C834C99921EB23BE79AD9D5DCC1DD9AD236132102900B723CF980957FC4E177108FC607774F29E8320E92EA05ECE4E821C0A5EFE8F1645C4C0C93C1AB99285D622CAA652C1DFAD63D745D6F2DE5F17E5EAF0FC4963D261C8A12436518206DC093344D5AD293", Name="System.Runtime.Serialization.Formatters.Soap")]
    public sealed class InternalST
    {
        private InternalST()
        {
        }

        [Conditional("_LOGGING")]
        public static void InfoSoap(params object[] messages)
        {
        }

        public static Assembly LoadAssemblyFromString(string assemblyString) => 
            FormatterServices.LoadAssemblyFromString(assemblyString);

        public static void SerializationSetValue(FieldInfo fi, object target, object value)
        {
            if (fi == null)
            {
                throw new ArgumentNullException("fi");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            FormatterServices.SerializationSetValue(fi, target, value);
        }

        [Conditional("SER_LOGGING")]
        public static void Soap(params object[] messages)
        {
            if (!(messages[0] is string))
            {
                messages[0] = messages[0].GetType().Name + " ";
            }
            else
            {
                messages[0] = messages[0] + " ";
            }
        }

        [Conditional("_DEBUG")]
        public static void SoapAssert(bool condition, string message)
        {
        }

        public static bool SoapCheckEnabled() => 
            BCLDebug.CheckEnabled("Soap");
    }
}

