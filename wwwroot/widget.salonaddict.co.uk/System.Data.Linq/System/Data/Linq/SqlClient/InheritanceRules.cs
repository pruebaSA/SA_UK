namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Reflection;

    internal static class InheritanceRules
    {
        internal static bool AreSameMember(MemberInfo mi1, MemberInfo mi2) => 
            DistinguishedMemberName(mi1).Equals(DistinguishedMemberName(mi2));

        internal static object DistinguishedMemberName(MemberInfo mi)
        {
            PropertyInfo info = mi as PropertyInfo;
            if (!(mi is FieldInfo))
            {
                if (info == null)
                {
                    throw Error.ArgumentOutOfRange("mi");
                }
                MethodInfo getMethod = null;
                if (info.CanRead)
                {
                    getMethod = info.GetGetMethod();
                }
                if ((getMethod == null) && info.CanWrite)
                {
                    getMethod = info.GetSetMethod();
                }
                if ((getMethod != null) && getMethod.IsVirtual)
                {
                    return mi.Name;
                }
            }
            return new MetaPosition(mi);
        }

        internal static object InheritanceCodeForClientCompare(object rawCode, ProviderType providerType)
        {
            if (!providerType.IsFixedSize || (rawCode.GetType() != typeof(string)))
            {
                return rawCode;
            }
            string str = (string) rawCode;
            if (providerType.Size.HasValue)
            {
                if (str.Length != providerType.Size)
                {
                    str = str.PadRight(providerType.Size.Value).Substring(0, providerType.Size.Value);
                }
            }
            return str;
        }
    }
}

