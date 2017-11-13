namespace System.ServiceModel.Administration
{
    using System;
    using System.ServiceModel.Channels;

    internal static class AdministrationHelpers
    {
        public static Type GetServiceModelBaseType(Type type)
        {
            Type baseType = type;
            while (baseType != null)
            {
                if (baseType.IsPublic && (baseType.Assembly == typeof(BindingElement).Assembly))
                {
                    return baseType;
                }
                baseType = baseType.BaseType;
            }
            return baseType;
        }
    }
}

