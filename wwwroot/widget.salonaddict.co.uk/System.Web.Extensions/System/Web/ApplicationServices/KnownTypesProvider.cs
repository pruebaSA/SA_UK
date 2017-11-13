namespace System.Web.ApplicationServices
{
    using System;
    using System.Configuration;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Profile;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public static class KnownTypesProvider
    {
        public static Type[] GetKnownTypes(ICustomAttributeProvider knownTypeAttributeTarget)
        {
            if (ProfileBase.Properties == null)
            {
                return new Type[0];
            }
            Type[] typeArray = new Type[ProfileBase.Properties.Count];
            int num = 0;
            foreach (SettingsProperty property in ProfileBase.Properties)
            {
                typeArray[num++] = property.PropertyType;
            }
            return typeArray;
        }
    }
}

