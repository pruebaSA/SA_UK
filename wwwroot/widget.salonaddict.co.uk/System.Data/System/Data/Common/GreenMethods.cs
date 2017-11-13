namespace System.Data.Common
{
    using System;
    using System.Reflection;
    using System.Security.Permissions;

    internal static class GreenMethods
    {
        private const string ExtensionAssemblyRef = "System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        internal static Type SystemDataCommonDbProviderServices_Type = Type.GetType("System.Data.Common.DbProviderServices, System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false);
        private const string SystemDataCommonDbProviderServices_TypeName = "System.Data.Common.DbProviderServices, System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
        private static FieldInfo SystemDataSqlClientSqlProviderServices_Instance_FieldInfo;
        private const string SystemDataSqlClientSqlProviderServices_TypeName = "System.Data.SqlClient.SqlProviderServices, System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

        internal static object SystemDataSqlClientSqlProviderServices_Instance()
        {
            if (SystemDataSqlClientSqlProviderServices_Instance_FieldInfo == null)
            {
                Type type = Type.GetType("System.Data.SqlClient.SqlProviderServices, System.Data.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089", false);
                if (type != null)
                {
                    SystemDataSqlClientSqlProviderServices_Instance_FieldInfo = type.GetField("Instance", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                }
            }
            return SystemDataSqlClientSqlProviderServices_Instance_GetValue();
        }

        [ReflectionPermission(SecurityAction.Assert, MemberAccess=true)]
        private static object SystemDataSqlClientSqlProviderServices_Instance_GetValue()
        {
            object obj2 = null;
            if (SystemDataSqlClientSqlProviderServices_Instance_FieldInfo != null)
            {
                obj2 = SystemDataSqlClientSqlProviderServices_Instance_FieldInfo.GetValue(null);
            }
            return obj2;
        }
    }
}

